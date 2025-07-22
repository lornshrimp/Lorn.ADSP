using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Lorn.ADSP.Infrastructure.DataAccess.Core.Chain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Routing;

/// <summary>
/// 数据访问路由器实现
/// 基于规则和策略的智能路由决策
/// </summary>
public class DataAccessRouter : IDataAccessRouter
{
    private readonly IDataProviderRegistry _registry;
    private readonly ILogger<DataAccessRouter> _logger;
    private readonly ConcurrentDictionary<string, RoutingRule> _routingRules = new();
    private readonly object _lock = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="registry">数据提供者注册表</param>
    /// <param name="logger">日志记录器</param>
    public DataAccessRouter(IDataProviderRegistry registry, ILogger<DataAccessRouter> logger)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<IDataAccessProvider> RouteAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var decision = await GetRoutingDecisionAsync(context, cancellationToken);

            _logger.LogDebug("Routed request {RequestId} to provider '{ProviderName}' in {Duration}ms",
                context.RequestId, decision.SelectedProvider.GetMetadata().ProviderName, stopwatch.ElapsedMilliseconds);

            return decision.SelectedProvider;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to route request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IDataAccessProvider>> GetCandidateProvidersAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // 根据上下文构建查询条件
        var query = BuildProviderQuery(context);

        var providers = await _registry.GetProvidersAsync(query, cancellationToken);

        _logger.LogDebug("Found {Count} candidate providers for request {RequestId}",
            providers.Count(), context.RequestId);

        return providers;
    }

    /// <inheritdoc />
    public async Task<IDataAccessProviderChain> BuildProviderChainAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 1. 获取所有候选提供者
            var candidateProviders = (await GetCandidateProvidersAsync(context, cancellationToken)).ToList();

            if (!candidateProviders.Any())
            {
                _logger.LogWarning("No candidate providers found for request {RequestId}", context.RequestId);
                throw new InvalidOperationException($"No suitable providers found for request {context.RequestId}");
            }

            // 2. 按类型分组提供者
            var cacheProviders = candidateProviders
                .Where(p => p.GetMetadata().ProviderType == DataProviderType.Cache)
                .OrderBy(p => p.GetMetadata().Priority)
                .ToList();

            var databaseProviders = candidateProviders
                .Where(p => p.GetMetadata().ProviderType == DataProviderType.Database)
                .OrderBy(p => p.GetMetadata().Priority)
                .ToList();

            // 3. 构建提供者链节点
            var chainNodes = new List<DataProviderChainNode>();

            // 添加缓存提供者（作为主要数据源）
            if (cacheProviders.Any())
            {
                var primaryCacheProvider = cacheProviders.First();
                chainNodes.Add(new DataProviderChainNode
                {
                    Provider = primaryCacheProvider,
                    Role = ProviderNodeRole.Cache
                });
            }

            // 添加数据库提供者（作为回退数据源）
            if (databaseProviders.Any())
            {
                var primaryDatabaseProvider = databaseProviders.First();
                chainNodes.Add(new DataProviderChainNode
                {
                    Provider = primaryDatabaseProvider,
                    Role = ProviderNodeRole.Database
                });
            }

            // 添加缓存回写节点（如果有缓存提供者）
            if (cacheProviders.Any())
            {
                var writeBackCacheProvider = cacheProviders.First();
                chainNodes.Add(new DataProviderChainNode
                {
                    Provider = writeBackCacheProvider,
                    Role = ProviderNodeRole.Cache // 缓存回写也是 Cache 角色
                });
            }

            // 4. 创建并返回提供者链执行器
            var chainId = $"Chain_{context.RequestId}_{Guid.NewGuid():N}";

            // 使用类型不安全但可工作的日志记录器转换
            var chainLogger = (ILogger<DataAccessProviderChain>)(object)_logger;

            var chainExecutor = new DataAccessProviderChain(
                chainId,
                chainNodes.AsReadOnly(),
                CacheWriteBackStrategy.Asynchronous,
                chainLogger);

            stopwatch.Stop();

            _logger.LogDebug("Built provider chain with {NodeCount} nodes for request {RequestId} in {Duration}ms",
                chainNodes.Count, context.RequestId, stopwatch.ElapsedMilliseconds);

            return chainExecutor;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error building provider chain for request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <inheritdoc />
    public Task UpdateRoutingRulesAsync(RoutingRule[] rules, CancellationToken cancellationToken = default)
    {
        if (rules == null)
            throw new ArgumentNullException(nameof(rules));

        lock (_lock)
        {
            _routingRules.Clear();

            foreach (var rule in rules)
            {
                if (rule.IsEnabled)
                {
                    _routingRules.TryAdd(rule.RuleId, rule);
                }
            }
        }

        _logger.LogInformation("Updated routing rules: {EnabledRules} enabled out of {TotalRules} total",
            _routingRules.Count, rules.Length);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<RoutingDecision> GetRoutingDecisionAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var stopwatch = Stopwatch.StartNew();

        // 获取候选提供者
        var candidateProviders = (await GetCandidateProvidersAsync(context, cancellationToken)).ToList();

        if (!candidateProviders.Any())
        {
            throw new InvalidOperationException($"No suitable providers found for request {context.RequestId}");
        }

        // 应用路由规则
        var appliedRule = FindMatchingRule(context);
        var selectedProvider = SelectProvider(candidateProviders, appliedRule, context);

        stopwatch.Stop();

        var decision = new RoutingDecision
        {
            SelectedProvider = selectedProvider,
            CandidateProviders = candidateProviders,
            AppliedRule = appliedRule,
            Reason = GenerateDecisionReason(selectedProvider, appliedRule, candidateProviders.Count),
            DecisionTime = DateTimeOffset.UtcNow,
            DecisionDuration = stopwatch.Elapsed.TotalMilliseconds,
            IsFailoverDecision = IsFailoverScenario(context, candidateProviders),
            Confidence = CalculateConfidence(selectedProvider, candidateProviders)
        };

        _logger.LogDebug("Routing decision made for request {RequestId}: {Reason}",
            context.RequestId, decision.Reason);

        return decision;
    }

    /// <inheritdoc />
    public Task<IEnumerable<RoutingRule>> GetRoutingRulesAsync(CancellationToken cancellationToken = default)
    {
        var rules = _routingRules.Values.OrderByDescending(r => r.Priority).ToList();
        return Task.FromResult<IEnumerable<RoutingRule>>(rules);
    }

    /// <inheritdoc />
    public Task AddRoutingRuleAsync(RoutingRule rule, CancellationToken cancellationToken = default)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        if (!rule.IsEnabled)
        {
            _logger.LogDebug("Skipping disabled routing rule: {RuleId}", rule.RuleId);
            return Task.CompletedTask;
        }

        var success = _routingRules.TryAdd(rule.RuleId, rule);

        if (success)
        {
            _logger.LogInformation("Added routing rule: {RuleName} (ID: {RuleId})", rule.Name, rule.RuleId);
        }
        else
        {
            _logger.LogWarning("Failed to add routing rule {RuleId}: already exists", rule.RuleId);
            throw new InvalidOperationException($"Routing rule with ID '{rule.RuleId}' already exists");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> RemoveRoutingRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Rule ID cannot be null or empty", nameof(ruleId));

        var success = _routingRules.TryRemove(ruleId, out var removedRule);

        if (success)
        {
            _logger.LogInformation("Removed routing rule: {RuleName} (ID: {RuleId})", removedRule!.Name, ruleId);
        }
        else
        {
            _logger.LogWarning("Failed to remove routing rule {RuleId}: not found", ruleId);
        }

        return Task.FromResult(success);
    }

    /// <inheritdoc />
    public Task ClearRoutingRulesAsync(CancellationToken cancellationToken = default)
    {
        var count = _routingRules.Count;
        _routingRules.Clear();

        _logger.LogInformation("Cleared all routing rules: {Count} rules removed", count);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据上下文构建提供者查询条件
    /// </summary>
    private DataProviderQuery BuildProviderQuery(DataAccessContext context)
    {
        var query = new DataProviderQuery
        {
            OnlyEnabled = true,
            OnlyHealthy = true,
            OrderByPriority = true,
            Tags = context.Tags
        };

        // 如果指定了业务实体，优先查找业务逻辑提供者
        if (!string.IsNullOrWhiteSpace(context.EntityType))
        {
            return new DataProviderQuery
            {
                BusinessEntity = context.EntityType,
                ProviderType = DataProviderType.BusinessLogic,
                TechnologyType = query.TechnologyType,
                PlatformType = query.PlatformType,
                Tags = query.Tags,
                SupportedOperations = query.SupportedOperations,
                OnlyEnabled = query.OnlyEnabled,
                OnlyHealthy = query.OnlyHealthy,
                MinPriority = query.MinPriority,
                MaxPriority = query.MaxPriority,
                Version = query.Version,
                ProviderIds = query.ProviderIds,
                ExtendedMetadataFilter = query.ExtendedMetadataFilter,
                Limit = query.Limit,
                OrderByPriority = query.OrderByPriority
            };
        }

        return query;
    }

    /// <summary>
    /// 查找匹配的路由规则
    /// </summary>
    private RoutingRule? FindMatchingRule(DataAccessContext context)
    {
        var rules = _routingRules.Values
            .Where(r => r.IsEnabled)
            .OrderByDescending(r => r.Priority)
            .ToList();

        foreach (var rule in rules)
        {
            if (IsRuleMatching(rule, context))
            {
                _logger.LogDebug("Applied routing rule: {RuleName} for request {RequestId}",
                    rule.Name, context.RequestId);
                return rule;
            }
        }

        _logger.LogDebug("No matching routing rule found for request {RequestId}", context.RequestId);
        return null;
    }

    /// <summary>
    /// 检查规则是否匹配上下文
    /// </summary>
    private bool IsRuleMatching(RoutingRule rule, DataAccessContext context)
    {
        var condition = rule.Condition;

        // 检查操作类型
        if (condition.OperationTypes?.Any() == true &&
            !condition.OperationTypes.Contains(context.OperationType, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        // 检查实体类型
        if (condition.EntityTypes?.Any() == true &&
            !condition.EntityTypes.Contains(context.EntityType, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        // 检查标签
        if (condition.Tags?.Any() == true &&
            !condition.Tags.Any(tag => context.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
        {
            return false;
        }

        // 检查一致性级别
        if (condition.ConsistencyLevels?.Any() == true &&
            !condition.ConsistencyLevels.Contains(context.ConsistencyLevel))
        {
            return false;
        }

        // 检查用户ID模式
        if (!string.IsNullOrWhiteSpace(condition.UserIdPattern) &&
            !string.IsNullOrWhiteSpace(context.UserId))
        {
            // 简单的模式匹配，实际实现可以使用正则表达式
            if (!context.UserId.Contains(condition.UserIdPattern, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        // 检查租户ID模式
        if (!string.IsNullOrWhiteSpace(condition.TenantIdPattern) &&
            !string.IsNullOrWhiteSpace(context.TenantId))
        {
            if (!context.TenantId.Contains(condition.TenantIdPattern, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        // 检查时间范围
        if (condition.TimeRange != null)
        {
            var now = DateTime.Now;

            if (condition.TimeRange.StartTime.HasValue &&
                now.TimeOfDay < condition.TimeRange.StartTime.Value)
            {
                return false;
            }

            if (condition.TimeRange.EndTime.HasValue &&
                now.TimeOfDay > condition.TimeRange.EndTime.Value)
            {
                return false;
            }

            if (condition.TimeRange.DaysOfWeek?.Any() == true)
            {
                var dayOfWeek = (int)now.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7; // 将周日从0改为7

                if (!condition.TimeRange.DaysOfWeek.Contains(dayOfWeek))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 选择提供者
    /// </summary>
    private IDataAccessProvider SelectProvider(
        List<IDataAccessProvider> candidates,
        RoutingRule? appliedRule,
        DataAccessContext context)
    {
        if (!candidates.Any())
            throw new InvalidOperationException("No candidate providers available");

        var strategy = appliedRule?.Strategy ?? RoutingStrategy.Priority;

        return strategy switch
        {
            RoutingStrategy.Priority => SelectByPriority(candidates),
            RoutingStrategy.RoundRobin => SelectByRoundRobin(candidates, context),
            RoutingStrategy.Random => SelectByRandom(candidates),
            RoutingStrategy.LoadBalance => SelectByLoadBalance(candidates),
            RoutingStrategy.Failover => SelectByFailover(candidates),
            RoutingStrategy.Custom => SelectByCustom(candidates, appliedRule, context),
            _ => SelectByPriority(candidates)
        };
    }

    /// <summary>
    /// 按优先级选择提供者
    /// </summary>
    private IDataAccessProvider SelectByPriority(List<IDataAccessProvider> candidates)
    {
        return candidates
            .OrderByDescending(p => p.GetMetadata().Priority)
            .First();
    }

    /// <summary>
    /// 按轮询选择提供者
    /// </summary>
    private IDataAccessProvider SelectByRoundRobin(List<IDataAccessProvider> candidates, DataAccessContext context)
    {
        // 简单实现：基于请求ID的哈希值
        var hash = context.RequestId.GetHashCode();
        var index = Math.Abs(hash) % candidates.Count;
        return candidates[index];
    }

    /// <summary>
    /// 随机选择提供者
    /// </summary>
    private IDataAccessProvider SelectByRandom(List<IDataAccessProvider> candidates)
    {
        var random = new Random();
        var index = random.Next(candidates.Count);
        return candidates[index];
    }

    /// <summary>
    /// 按负载均衡选择提供者
    /// </summary>
    private IDataAccessProvider SelectByLoadBalance(List<IDataAccessProvider> candidates)
    {
        // 简单实现：选择优先级最高的健康提供者
        return candidates
            .Where(p => p.GetMetadata().HealthStatus == HealthStatus.Healthy)
            .OrderByDescending(p => p.GetMetadata().Priority)
            .FirstOrDefault() ?? candidates.First();
    }

    /// <summary>
    /// 按故障转移选择提供者
    /// </summary>
    private IDataAccessProvider SelectByFailover(List<IDataAccessProvider> candidates)
    {
        // 选择第一个健康的提供者
        return candidates
            .FirstOrDefault(p => p.GetMetadata().HealthStatus == HealthStatus.Healthy) ??
               candidates.First();
    }

    /// <summary>
    /// 按自定义规则选择提供者
    /// </summary>
    private IDataAccessProvider SelectByCustom(
        List<IDataAccessProvider> candidates,
        RoutingRule? rule,
        DataAccessContext context)
    {
        // 默认降级到优先级策略
        _logger.LogWarning("Custom routing strategy not implemented, falling back to priority strategy");
        return SelectByPriority(candidates);
    }

    /// <summary>
    /// 生成决策原因说明
    /// </summary>
    private string GenerateDecisionReason(
        IDataAccessProvider selectedProvider,
        RoutingRule? appliedRule,
        int candidateCount)
    {
        var metadata = selectedProvider.GetMetadata();

        if (appliedRule != null)
        {
            return $"Selected '{metadata.ProviderName}' by rule '{appliedRule.Name}' from {candidateCount} candidates";
        }

        return $"Selected '{metadata.ProviderName}' by default priority from {candidateCount} candidates";
    }

    /// <summary>
    /// 判断是否为故障转移场景
    /// </summary>
    private bool IsFailoverScenario(DataAccessContext context, List<IDataAccessProvider> candidates)
    {
        // 如果有不健康的提供者，可能是故障转移场景
        return candidates.Any(p => p.GetMetadata().HealthStatus != HealthStatus.Healthy);
    }

    /// <summary>
    /// 计算决策置信度
    /// </summary>
    private double CalculateConfidence(IDataAccessProvider selectedProvider, List<IDataAccessProvider> candidates)
    {
        var selectedMetadata = selectedProvider.GetMetadata();

        // 如果只有一个候选者，置信度较低
        if (candidates.Count == 1)
            return 0.7;

        // 如果选中的提供者健康且优先级高，置信度高
        if (selectedMetadata.HealthStatus == HealthStatus.Healthy &&
            selectedMetadata.Priority > 0)
            return 0.95;

        // 默认置信度
        return 0.8;
    }
}
