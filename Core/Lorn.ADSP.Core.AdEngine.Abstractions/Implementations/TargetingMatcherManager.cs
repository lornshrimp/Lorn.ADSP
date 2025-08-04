using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.AdEngine.Abstractions.Events;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Entities;
using System.Collections.Concurrent;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Implementations;

/// <summary>
/// 定向匹配器管理器实现
/// 负责管理多个定向匹配器的注册、发现和协调执行
/// </summary>
public class TargetingMatcherManager : ITargetingMatcherManager
{
    private readonly ConcurrentDictionary<string, ITargetingMatcher> _matchers;
    private readonly ConcurrentDictionary<string, List<ITargetingMatcher>> _matchersByType;
    private readonly object _lockObject = new();

    /// <summary>
    /// 匹配器注册事件
    /// </summary>
    public event EventHandler<MatcherRegisteredEventArgs>? MatcherRegistered;

    /// <summary>
    /// 匹配器注销事件
    /// </summary>
    public event EventHandler<MatcherUnregisteredEventArgs>? MatcherUnregistered;

    /// <summary>
    /// 匹配器状态变更事件
    /// </summary>
    public event EventHandler<MatcherStatusChangedEventArgs>? MatcherStatusChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    public TargetingMatcherManager()
    {
        _matchers = new ConcurrentDictionary<string, ITargetingMatcher>();
        _matchersByType = new ConcurrentDictionary<string, List<ITargetingMatcher>>();
    }

    /// <summary>
    /// 获取所有已注册的匹配器
    /// </summary>
    /// <returns>匹配器列表</returns>
    public IReadOnlyList<ITargetingMatcher> GetAllMatchers()
    {
        return _matchers.Values
            .OrderBy(m => m.Priority)
            .ThenBy(m => m.MatcherId)
            .ToList();
    }

    /// <summary>
    /// 根据匹配器ID获取匹配器
    /// </summary>
    /// <param name="matcherId">匹配器ID</param>
    /// <returns>匹配器实例，如果不存在则返回null</returns>
    public ITargetingMatcher? GetMatcher(string matcherId)
    {
        if (string.IsNullOrWhiteSpace(matcherId))
            return null;

        _matchers.TryGetValue(matcherId, out var matcher);
        return matcher;
    }

    /// <summary>
    /// 根据匹配器类型获取匹配器列表
    /// </summary>
    /// <param name="matcherType">匹配器类型</param>
    /// <returns>匹配器列表</returns>
    public IReadOnlyList<ITargetingMatcher> GetMatchersByType(string matcherType)
    {
        if (string.IsNullOrWhiteSpace(matcherType))
            return [];

        if (_matchersByType.TryGetValue(matcherType, out var matchers))
        {
            lock (_lockObject)
            {
                return matchers
                    .Where(m => m.IsEnabled)
                    .OrderBy(m => m.Priority)
                    .ToList();
            }
        }

        return [];
    }

    /// <summary>
    /// 获取支持指定定向条件类型的匹配器列表
    /// </summary>
    /// <param name="criteriaType">定向条件类型</param>
    /// <returns>支持该条件类型的匹配器列表</returns>
    public IReadOnlyList<ITargetingMatcher> GetSupportedMatchers(string criteriaType)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            return [];

        return _matchers.Values
            .Where(m => m.IsEnabled && m.IsSupported(criteriaType))
            .OrderBy(m => m.Priority)
            .ToList();
    }

    /// <summary>
    /// 注册匹配器
    /// </summary>
    /// <param name="matcher">要注册的匹配器</param>
    /// <returns>注册是否成功</returns>
    public bool RegisterMatcher(ITargetingMatcher matcher)
    {
        if (matcher == null)
        {
            // 尝试注册空的匹配器
            return false;
        }

        try
        {
            // 检查是否已存在相同ID的匹配器
            if (_matchers.ContainsKey(matcher.MatcherId))
            {
                // 匹配器已存在，无法重复注册
                return false;
            }

            // 验证匹配器配置
            var validationResult = matcher.ValidateCriteria(null!); // 基础验证
            if (!validationResult.IsValid)
            {
                // 匹配器配置验证失败
                return false;
            }

            // 注册匹配器
            if (_matchers.TryAdd(matcher.MatcherId, matcher))
            {
                // 按类型分组
                lock (_lockObject)
                {
                    if (!_matchersByType.ContainsKey(matcher.MatcherType))
                    {
                        _matchersByType[matcher.MatcherType] = new List<ITargetingMatcher>();
                    }
                    _matchersByType[matcher.MatcherType].Add(matcher);
                }

                // 成功注册匹配器

                // 触发注册事件
                MatcherRegistered?.Invoke(this, new MatcherRegisteredEventArgs
                {
                    Matcher = matcher,
                    IsHotRegistration = true
                });

                return true;
            }

            return false;
        }
        catch (Exception)
        {
            // 注册匹配器时发生异常
            return false;
        }
    }

    /// <summary>
    /// 注销匹配器
    /// </summary>
    /// <param name="matcherId">要注销的匹配器ID</param>
    /// <returns>注销是否成功</returns>
    public bool UnregisterMatcher(string matcherId)
    {
        if (string.IsNullOrWhiteSpace(matcherId))
        {
            // 尝试注销空的匹配器ID
            return false;
        }

        try
        {
            if (_matchers.TryRemove(matcherId, out var matcher))
            {
                // 从类型分组中移除
                lock (_lockObject)
                {
                    if (_matchersByType.TryGetValue(matcher.MatcherType, out var typeMatchers))
                    {
                        typeMatchers.Remove(matcher);
                        if (typeMatchers.Count == 0)
                        {
                            _matchersByType.TryRemove(matcher.MatcherType, out _);
                        }
                    }
                }

                // 成功注销匹配器

                // 触发注销事件
                MatcherUnregistered?.Invoke(this, new MatcherUnregisteredEventArgs
                {
                    MatcherId = matcher.MatcherId,
                    MatcherName = matcher.MatcherName,
                    MatcherType = matcher.MatcherType,
                    IsHotUnregistration = true
                });

                // 清理匹配器资源
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await matcher.CleanupAsync();
                    }
                    catch (Exception)
                    {
                        // 清理匹配器资源时发生异常，继续执行
                    }
                });

                return true;
            }

            // 匹配器不存在
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 执行多维度定向匹配
    /// </summary>
    /// <param name="context">定向上下文</param>
    /// <param name="criteriaList">定向条件列表</param>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>整体匹配结果</returns>
    public async Task<OverallMatchResult> ExecuteMatchingAsync(
        ITargetingContext context,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid();
        var requestIdString = requestId.ToString();
        var startTime = DateTime.UtcNow;

        try
        {
            // 开始执行定向匹配

            // 获取所有启用的匹配器
            var enabledMatchers = _matchers.Values
                .Where(m => m.IsEnabled)
                .OrderBy(m => m.Priority)
                .ToList();

            if (!enabledMatchers.Any())
            {
                // 没有可用的匹配器
                return OverallMatchResult.CreateNotMatched(
                    requestId,
                    context.ContextId.ToString(),
                    "NO_MATCHERS_AVAILABLE");
            }

            // 并行执行匹配
            var matchTasks = new List<Task<MatchResult>>();

            foreach (var matcher in enabledMatchers)
            {
                foreach (var criteria in criteriaList)
                {
                    if (matcher.IsSupported(criteria.GetType().Name))
                    {
                        var matchTask = ExecuteMatcherAsync(matcher, context, criteria, callbackProvider, cancellationToken);
                        matchTasks.Add(matchTask);
                    }
                }
            }

            // 等待所有匹配任务完成
            var matchResults = await Task.WhenAll(matchTasks);

            // 创建整体匹配结果
            var overallResult = OverallMatchResult.Create(requestId, context.ContextId.ToString(), matchResults);

            var endTime = DateTime.UtcNow;
            var executionTime = endTime - startTime;

            // 定向匹配完成，记录执行时间和结果统计信息

            return overallResult;
        }
        catch (Exception ex)
        {
            // 执行定向匹配时发生异常
            return OverallMatchResult.CreateNotMatched(requestId, "MATCHING_EXECUTION_FAILED", ex.Message);
        }
    }

    /// <summary>
    /// 批量执行定向匹配
    /// </summary>
    /// <param name="contexts">定向上下文列表</param>
    /// <param name="criteriaList">定向条件列表</param>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量匹配结果</returns>
    public async Task<IReadOnlyList<OverallMatchResult>> ExecuteBatchMatchingAsync(
        IReadOnlyList<ITargetingContext> contexts,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        // 开始执行批量定向匹配

        var batchTasks = contexts.Select(context =>
            ExecuteMatchingAsync(context, criteriaList, callbackProvider, cancellationToken));

        var results = await Task.WhenAll(batchTasks);

        // 批量定向匹配完成

        return results;
    }

    /// <summary>
    /// 执行单个匹配器
    /// </summary>
    private async Task<MatchResult> ExecuteMatcherAsync(
        ITargetingMatcher matcher,
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(matcher.ExpectedExecutionTime.Add(TimeSpan.FromMilliseconds(100)));

            var result = await matcher.CalculateMatchScoreAsync(context, criteria, callbackProvider, timeoutCts.Token);

            // 匹配器执行完成

            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // 匹配器执行被取消
            return MatchResult.CreateNoMatch("MATCHER", Guid.NewGuid(), "匹配器执行被取消", TimeSpan.Zero);
        }
        catch (OperationCanceledException)
        {
            // 匹配器执行超时
            return MatchResult.CreateNoMatch("MATCHER", Guid.NewGuid(), "匹配器执行超时", matcher.ExpectedExecutionTime);
        }
        catch (Exception ex)
        {
            // 匹配器执行时发生异常
            return MatchResult.CreateNoMatch("MATCHER", Guid.NewGuid(), $"匹配器执行失败: {ex.Message}", TimeSpan.Zero);
        }
    }

    /// <summary>
    /// 验证所有匹配器的配置
    /// </summary>
    /// <returns>验证结果列表</returns>
    public IReadOnlyList<ValidationResult> ValidateAllMatchers()
    {
        var results = new List<ValidationResult>();

        foreach (var matcher in _matchers.Values)
        {
            try
            {
                var result = matcher.ValidateCriteria(null!); // 基础验证
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult($"匹配器 {matcher.MatcherId} 验证时发生异常: {ex.Message}")
                {
                    IsValid = false,
                    Errors = [ValidationError.FromException(ex, matcher.MatcherId)]
                });
            }
        }

        return results;
    }

    /// <summary>
    /// 预热所有匹配器
    /// </summary>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预热任务</returns>
    public async Task WarmUpAllMatchersAsync(ICallbackProvider callbackProvider, CancellationToken cancellationToken = default)
    {
        // 开始预热所有匹配器

        var warmUpTasks = _matchers.Values.Select(async matcher =>
        {
            try
            {
                await matcher.WarmUpAsync(callbackProvider, cancellationToken);
                // 匹配器预热完成
            }
            catch (Exception)
            {
                // 匹配器预热失败，继续处理其他匹配器
            }
        });

        await Task.WhenAll(warmUpTasks);
        // 所有匹配器预热完成
    }

    /// <summary>
    /// 清理所有匹配器资源
    /// </summary>
    /// <returns>清理任务</returns>
    public async Task CleanupAllMatchersAsync()
    {
        // 开始清理所有匹配器资源

        var cleanupTasks = _matchers.Values.Select(async matcher =>
        {
            try
            {
                await matcher.CleanupAsync();
                // 匹配器资源清理完成
            }
            catch (Exception)
            {
                // 匹配器资源清理失败，继续处理其他匹配器
            }
        });

        await Task.WhenAll(cleanupTasks);
        // 所有匹配器资源清理完成
    }

    /// <summary>
    /// 获取匹配器统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public TargetingMatcherStatistics GetStatistics()
    {
        var matchers = _matchers.Values.ToList();
        var enabledMatchers = matchers.Where(m => m.IsEnabled).ToList();
        var disabledMatchers = matchers.Where(m => !m.IsEnabled).ToList();

        var matchersByType = matchers
            .GroupBy(m => m.MatcherType)
            .ToDictionary(g => g.Key, g => g.Count());

        var matchersByPriority = matchers
            .GroupBy(m => m.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        var parallelCapableCount = matchers.Count(m => m.CanRunInParallel);

        var executionTimes = matchers.Select(m => m.ExpectedExecutionTime.TotalMilliseconds).ToList();

        return new TargetingMatcherStatistics
        {
            TotalRegisteredMatchers = matchers.Count,
            EnabledMatchersCount = enabledMatchers.Count,
            DisabledMatchersCount = disabledMatchers.Count,
            MatchersByType = matchersByType,
            MatchersByPriority = matchersByPriority,
            ParallelCapableMatchersCount = parallelCapableCount,
            AverageExpectedExecutionTimeMs = executionTimes.Any() ? executionTimes.Average() : 0,
            MaxExpectedExecutionTimeMs = executionTimes.Any() ? executionTimes.Max() : 0,
            MinExpectedExecutionTimeMs = executionTimes.Any() ? executionTimes.Min() : 0,
            HealthStats = new HealthStatistics
            {
                HealthyMatchersCount = enabledMatchers.Count,
                UnhealthyMatchersCount = disabledMatchers.Count,
                HealthRate = matchers.Count > 0 ? (double)enabledMatchers.Count / matchers.Count : 0
            }
        };
    }
}