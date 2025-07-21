using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Routing;

/// <summary>
/// 默认路由策略实现
/// 提供基于优先级的默认路由逻辑
/// </summary>
public class DefaultRoutingStrategy
{
    private readonly ILogger<DefaultRoutingStrategy> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public DefaultRoutingStrategy(ILogger<DefaultRoutingStrategy> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 根据策略选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <param name="strategy">路由策略</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>选中的提供者</returns>
    public IDataAccessProvider SelectProvider(
        IList<IDataAccessProvider> candidates,
        RoutingStrategy strategy,
        DataAccessContext context)
    {
        if (candidates == null || !candidates.Any())
            throw new ArgumentException("Candidate providers cannot be null or empty", nameof(candidates));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        _logger.LogDebug("Selecting provider using strategy '{Strategy}' from {Count} candidates for request {RequestId}",
            strategy, candidates.Count, context.RequestId);

        var selectedProvider = strategy switch
        {
            RoutingStrategy.Priority => SelectByPriority(candidates),
            RoutingStrategy.RoundRobin => SelectByRoundRobin(candidates, context),
            RoutingStrategy.Random => SelectByRandom(candidates),
            RoutingStrategy.LoadBalance => SelectByLoadBalance(candidates),
            RoutingStrategy.Failover => SelectByFailover(candidates),
            RoutingStrategy.Custom => SelectByCustom(candidates, context),
            _ => SelectByPriority(candidates) // 默认使用优先级策略
        };

        _logger.LogDebug("Selected provider '{ProviderName}' using strategy '{Strategy}'",
            selectedProvider.GetMetadata().ProviderName, strategy);

        return selectedProvider;
    }

    /// <summary>
    /// 按优先级选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <returns>优先级最高的提供者</returns>
    private IDataAccessProvider SelectByPriority(IList<IDataAccessProvider> candidates)
    {
        var selected = candidates
            .Where(p => p.GetMetadata().IsEnabled)
            .OrderByDescending(p => p.GetMetadata().Priority)
            .ThenByDescending(p => p.GetMetadata().HealthStatus)
            .First();

        _logger.LogTrace("Selected provider '{ProviderName}' with priority {Priority}",
            selected.GetMetadata().ProviderName, selected.GetMetadata().Priority);

        return selected;
    }

    /// <summary>
    /// 按轮询选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>轮询选中的提供者</returns>
    private IDataAccessProvider SelectByRoundRobin(IList<IDataAccessProvider> candidates, DataAccessContext context)
    {
        // 使用请求ID的哈希值进行轮询
        var hash = Math.Abs(context.RequestId.GetHashCode());
        var index = hash % candidates.Count;
        var selected = candidates[index];

        _logger.LogTrace("Selected provider '{ProviderName}' by round-robin (index: {Index})",
            selected.GetMetadata().ProviderName, index);

        return selected;
    }

    /// <summary>
    /// 随机选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <returns>随机选中的提供者</returns>
    private IDataAccessProvider SelectByRandom(IList<IDataAccessProvider> candidates)
    {
        var random = Random.Shared;
        var index = random.Next(candidates.Count);
        var selected = candidates[index];

        _logger.LogTrace("Selected provider '{ProviderName}' by random selection (index: {Index})",
            selected.GetMetadata().ProviderName, index);

        return selected;
    }

    /// <summary>
    /// 按负载均衡选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <returns>负载最低的提供者</returns>
    private IDataAccessProvider SelectByLoadBalance(IList<IDataAccessProvider> candidates)
    {
        // 简单实现：优先选择健康且优先级高的提供者
        var selected = candidates
            .Where(p => p.GetMetadata().IsEnabled)
            .Where(p => p.GetMetadata().HealthStatus == HealthStatus.Healthy)
            .OrderByDescending(p => p.GetMetadata().Priority)
            .FirstOrDefault() ?? candidates.First();

        _logger.LogTrace("Selected provider '{ProviderName}' by load balance strategy",
            selected.GetMetadata().ProviderName);

        return selected;
    }

    /// <summary>
    /// 按故障转移选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <returns>第一个健康的提供者</returns>
    private IDataAccessProvider SelectByFailover(IList<IDataAccessProvider> candidates)
    {
        // 按优先级排序，选择第一个健康的提供者
        var selected = candidates
            .Where(p => p.GetMetadata().IsEnabled)
            .OrderByDescending(p => p.GetMetadata().Priority)
            .FirstOrDefault(p => p.GetMetadata().HealthStatus == HealthStatus.Healthy)
            ?? candidates.OrderByDescending(p => p.GetMetadata().Priority).First();

        var metadata = selected.GetMetadata();
        _logger.LogTrace("Selected provider '{ProviderName}' by failover strategy (Health: {HealthStatus})",
            metadata.ProviderName, metadata.HealthStatus);

        return selected;
    }

    /// <summary>
    /// 按自定义策略选择提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>自定义策略选中的提供者</returns>
    private IDataAccessProvider SelectByCustom(IList<IDataAccessProvider> candidates, DataAccessContext context)
    {
        // 默认降级到优先级策略
        _logger.LogWarning("Custom routing strategy not implemented for request {RequestId}, falling back to priority strategy",
            context.RequestId);

        return SelectByPriority(candidates);
    }

    /// <summary>
    /// 评估提供者的适用性得分
    /// </summary>
    /// <param name="provider">数据提供者</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>适用性得分（0-100）</returns>
    public int CalculateProviderScore(IDataAccessProvider provider, DataAccessContext context)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var metadata = provider.GetMetadata();
        var score = 0;

        // 启用状态权重 (20分)
        if (metadata.IsEnabled)
            score += 20;

        // 健康状态权重 (30分)
        score += metadata.HealthStatus switch
        {
            HealthStatus.Healthy => 30,
            HealthStatus.Degraded => 20,
            HealthStatus.Unhealthy => 0,
            _ => 0
        };

        // 优先级权重 (30分)
        var priorityScore = Math.Min(metadata.Priority, 30);
        score += Math.Max(0, priorityScore);

        // 类型匹配权重 (20分)
        if (!string.IsNullOrWhiteSpace(context.EntityType) &&
            string.Equals(metadata.BusinessEntity, context.EntityType, StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
        }

        // 标签匹配奖励
        if (context.Tags.Any() && metadata.Tags.Any())
        {
            var matchingTags = context.Tags.Intersect(metadata.Tags, StringComparer.OrdinalIgnoreCase).Count();
            var tagScore = (matchingTags * 10) / context.Tags.Length;
            score += Math.Min(tagScore, 10);
        }

        _logger.LogTrace("Provider '{ProviderName}' scored {Score} points for request {RequestId}",
            metadata.ProviderName, score, context.RequestId);

        return Math.Min(100, Math.Max(0, score));
    }

    /// <summary>
    /// 过滤不可用的提供者
    /// </summary>
    /// <param name="candidates">候选提供者列表</param>
    /// <param name="includeUnhealthy">是否包含不健康的提供者</param>
    /// <returns>过滤后的提供者列表</returns>
    public IList<IDataAccessProvider> FilterAvailableProviders(
        IList<IDataAccessProvider> candidates,
        bool includeUnhealthy = false)
    {
        var filtered = candidates.Where(p => p.GetMetadata().IsEnabled);

        if (!includeUnhealthy)
        {
            filtered = filtered.Where(p => p.GetMetadata().HealthStatus != HealthStatus.Unhealthy);
        }

        var result = filtered.ToList();

        _logger.LogDebug("Filtered {ResultCount} available providers from {TotalCount} candidates (includeUnhealthy: {IncludeUnhealthy})",
            result.Count, candidates.Count, includeUnhealthy);

        return result;
    }
}
