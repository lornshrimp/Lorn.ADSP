using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.AdEngine.Abstractions.Events;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Entities;
using Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Services;

/// <summary>
/// 定向匹配器管理器实现
/// Targeting Matcher Manager Implementation
/// </summary>
public class TargetingMatcherManager : ITargetingMatcherManager
{
    private readonly IEnumerable<ITargetingMatcher> _matchers;
    private readonly ILogger<TargetingMatcherManager> _logger;
    private readonly TargetingMatcherOptions _options;
    private readonly ConcurrentDictionary<string, ITargetingMatcher> _matcherCache;

    // 事件定义
    // Event definitions
    public event EventHandler<MatcherRegisteredEventArgs>? MatcherRegistered;
    public event EventHandler<MatcherUnregisteredEventArgs>? MatcherUnregistered;
    public event EventHandler<MatcherStatusChangedEventArgs>? MatcherStatusChanged;

    /// <summary>
    /// 构造函数
    /// Constructor
    /// </summary>
    public TargetingMatcherManager(
        IEnumerable<ITargetingMatcher> matchers,
        ILogger<TargetingMatcherManager> logger,
        IOptions<TargetingMatcherOptions> options)
    {
        _matchers = matchers ?? throw new ArgumentNullException(nameof(matchers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _matcherCache = new ConcurrentDictionary<string, ITargetingMatcher>();

        // 初始化缓存
        // Initialize cache
        foreach (var matcher in _matchers)
        {
            _matcherCache.TryAdd(matcher.MatcherId, matcher);
        }

        _logger.LogInformation("Initialized TargetingMatcherManager with {MatcherCount} matchers",
            _matchers.Count());
    }

    /// <summary>
    /// 获取所有已注册的匹配器
    /// Get all registered matchers
    /// </summary>
    public IReadOnlyList<ITargetingMatcher> GetAllMatchers()
    {
        return _matchers.Where(m => m.IsEnabled).ToList().AsReadOnly();
    }

    /// <summary>
    /// 根据匹配器ID获取匹配器
    /// Get matcher by ID
    /// </summary>
    public ITargetingMatcher? GetMatcher(string matcherId)
    {
        if (string.IsNullOrWhiteSpace(matcherId))
        {
            return null;
        }

        _matcherCache.TryGetValue(matcherId, out var matcher);
        return matcher?.IsEnabled == true ? matcher : null;
    }

    /// <summary>
    /// 根据匹配器类型获取匹配器列表
    /// Get matchers by type
    /// </summary>
    public IReadOnlyList<ITargetingMatcher> GetMatchersByType(string matcherType)
    {
        if (string.IsNullOrWhiteSpace(matcherType))
        {
            return Array.Empty<ITargetingMatcher>();
        }

        return _matchers
            .Where(m => m.IsEnabled && string.Equals(m.MatcherType, matcherType, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.Priority)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// 获取支持指定定向条件类型的匹配器列表
    /// Get matchers that support the specified criteria type
    /// </summary>
    public IReadOnlyList<ITargetingMatcher> GetSupportedMatchers(string criteriaType)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
        {
            return Array.Empty<ITargetingMatcher>();
        }

        return _matchers
            .Where(m => m.IsEnabled && m.IsSupported(criteriaType))
            .OrderBy(m => m.Priority)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// 注册匹配器
    /// Register matcher
    /// </summary>
    public bool RegisterMatcher(ITargetingMatcher matcher)
    {
        if (matcher == null)
        {
            _logger.LogWarning("Attempted to register null matcher");
            return false;
        }

        var success = _matcherCache.TryAdd(matcher.MatcherId, matcher);
        if (success)
        {
            _logger.LogInformation("Registered matcher: {MatcherId} ({MatcherType})",
                matcher.MatcherId, matcher.MatcherType);

            // 触发注册事件
            // Trigger registration event
            MatcherRegistered?.Invoke(this, new MatcherRegisteredEventArgs
            {
                Matcher = matcher,
                Source = "Dynamic Registration",
                IsHotRegistration = true
            });
        }
        else
        {
            _logger.LogWarning("Failed to register matcher {MatcherId} - already exists",
                matcher.MatcherId);
        }

        return success;
    }

    /// <summary>
    /// 注销匹配器
    /// Unregister matcher
    /// </summary>
    public bool UnregisterMatcher(string matcherId)
    {
        if (string.IsNullOrWhiteSpace(matcherId))
        {
            return false;
        }

        var success = _matcherCache.TryRemove(matcherId, out var matcher);
        if (success && matcher != null)
        {
            _logger.LogInformation("Unregistered matcher: {MatcherId}", matcherId);

            // 触发注销事件
            // Trigger unregistration event
            MatcherUnregistered?.Invoke(this, new MatcherUnregisteredEventArgs
            {
                MatcherId = matcher.MatcherId,
                MatcherName = matcher.MatcherName,
                MatcherType = matcher.MatcherType,
                Reason = "Manual Unregistration",
                IsHotUnregistration = true
            });

            // 清理资源
            // Cleanup resources
            _ = Task.Run(async () =>
            {
                try
                {
                    await matcher.CleanupAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during matcher cleanup: {MatcherId}", matcherId);
                }
            });
        }

        return success;
    }

    /// <summary>
    /// 执行多维度定向匹配
    /// Execute multi-dimensional targeting matching
    /// </summary>
    public async Task<OverallMatchResult> ExecuteMatchingAsync(
        ITargetingContext context,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (criteriaList == null) throw new ArgumentNullException(nameof(criteriaList));
        if (callbackProvider == null) throw new ArgumentNullException(nameof(callbackProvider));

        var stopwatch = Stopwatch.StartNew();
        var matchResults = new List<MatchResult>();
        var enabledMatchers = GetAllMatchers();

        _logger.LogDebug("Starting matching execution with {MatcherCount} matchers and {CriteriaCount} criteria",
            enabledMatchers.Count, criteriaList.Count);

        try
        {
            // 为每个定向条件找到合适的匹配器并执行
            // Find appropriate matchers for each targeting criteria and execute
            var matchingTasks = new List<Task<MatchResult>>();

            foreach (var criteria in criteriaList)
            {
                var supportedMatchers = GetSupportedMatchers(criteria.GetType().Name);

                foreach (var matcher in supportedMatchers)
                {
                    var task = ExecuteMatcherWithTimeout(
                        matcher, context, criteria, callbackProvider, cancellationToken);
                    matchingTasks.Add(task);
                }
            }

            // 等待所有匹配任务完成
            // Wait for all matching tasks to complete
            var results = await Task.WhenAll(matchingTasks);
            matchResults.AddRange(results.Where(r => r != null));

            // 计算整体匹配结果
            // Calculate overall match result
            var overallResult = CalculateOverallResult(matchResults, stopwatch.Elapsed);

            _logger.LogDebug("Matching execution completed in {ElapsedMs}ms with result: {IsMatch}",
                stopwatch.ElapsedMilliseconds, overallResult.IsOverallMatch);

            return overallResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during matching execution");
            return CreateFailureResult(stopwatch.Elapsed, ex.Message);
        }
    }

    /// <summary>
    /// 批量执行定向匹配
    /// Execute batch targeting matching
    /// </summary>
    public async Task<IReadOnlyList<OverallMatchResult>> ExecuteBatchMatchingAsync(
        IReadOnlyList<ITargetingContext> contexts,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        if (contexts == null) throw new ArgumentNullException(nameof(contexts));
        if (criteriaList == null) throw new ArgumentNullException(nameof(criteriaList));
        if (callbackProvider == null) throw new ArgumentNullException(nameof(callbackProvider));

        var results = new List<OverallMatchResult>();

        // 并行处理多个上下文
        // Process multiple contexts in parallel
        var batchTasks = contexts.Select(context =>
            ExecuteMatchingAsync(context, criteriaList, callbackProvider, cancellationToken));

        var batchResults = await Task.WhenAll(batchTasks);
        results.AddRange(batchResults);

        return results.AsReadOnly();
    }

    /// <summary>
    /// 验证所有匹配器的配置
    /// Validate all matcher configurations
    /// </summary>
    public IReadOnlyList<ValidationResult> ValidateAllMatchers()
    {
        var results = new List<ValidationResult>();

        foreach (var matcher in _matchers)
        {
            try
            {
                // 这里需要具体的验证逻辑，暂时返回成功
                // Specific validation logic needed here, returning success for now
                results.Add(new ValidationResult(null) { IsValid = true });
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult(ex.Message) { IsValid = false });
            }
        }

        return results.AsReadOnly();
    }

    /// <summary>
    /// 预热所有匹配器
    /// Warm up all matchers
    /// </summary>
    public async Task WarmUpAllMatchersAsync(ICallbackProvider callbackProvider, CancellationToken cancellationToken = default)
    {
        var warmupTasks = _matchers
            .Where(m => m.IsEnabled)
            .Select(m => WarmUpMatcherSafely(m, callbackProvider, cancellationToken));

        await Task.WhenAll(warmupTasks);
    }

    /// <summary>
    /// 安全地预热单个匹配器
    /// Safely warm up individual matcher
    /// </summary>
    private async Task WarmUpMatcherSafely(ITargetingMatcher matcher, ICallbackProvider callbackProvider, CancellationToken cancellationToken)
    {
        try
        {
            await matcher.WarmUpAsync(callbackProvider, cancellationToken);
            _logger.LogDebug("Successfully warmed up matcher: {MatcherId}", matcher.MatcherId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error warming up matcher: {MatcherId}", matcher.MatcherId);
        }
    }

    /// <summary>
    /// 清理所有匹配器资源
    /// Cleanup all matcher resources
    /// </summary>
    public async Task CleanupAllMatchersAsync()
    {
        var cleanupTasks = _matchers
            .Where(m => m.IsEnabled)
            .Select(m => CleanupMatcherSafely(m));

        await Task.WhenAll(cleanupTasks);
        _logger.LogInformation("All matchers have been cleaned up");
    }

    /// <summary>
    /// 获取匹配器统计信息
    /// Get matcher statistics
    /// </summary>
    public TargetingMatcherStatistics GetStatistics()
    {
        var enabledMatchers = _matchers.Where(m => m.IsEnabled).ToList();
        var disabledMatchers = _matchers.Where(m => !m.IsEnabled).ToList();

        var matchersByType = _matchers
            .GroupBy(m => m.MatcherType)
            .ToDictionary(g => g.Key, g => g.Count());

        var matchersByPriority = _matchers
            .GroupBy(m => m.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        return new TargetingMatcherStatistics
        {
            TotalRegisteredMatchers = _matchers.Count(),
            EnabledMatchersCount = enabledMatchers.Count,
            DisabledMatchersCount = disabledMatchers.Count,
            MatchersByType = matchersByType,
            MatchersByPriority = matchersByPriority,
            ParallelCapableMatchersCount = _matchers.Count(m => m.CanRunInParallel),
            AverageExpectedExecutionTimeMs = _matchers.Average(m => m.ExpectedExecutionTime.TotalMilliseconds),
            MaxExpectedExecutionTimeMs = _matchers.Max(m => m.ExpectedExecutionTime.TotalMilliseconds)
        };
    }

    /// <summary>
    /// 安全地清理单个匹配器
    /// Safely cleanup individual matcher
    /// </summary>
    private async Task CleanupMatcherSafely(ITargetingMatcher matcher)
    {
        try
        {
            await matcher.CleanupAsync();
            _logger.LogDebug("Successfully cleaned up matcher: {MatcherId}", matcher.MatcherId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up matcher: {MatcherId}", matcher.MatcherId);
        }
    }

    /// <summary>
    /// 带超时执行单个匹配器
    /// Execute single matcher with timeout
    /// </summary>
    private async Task<MatchResult> ExecuteMatcherWithTimeout(
        ITargetingMatcher matcher,
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken)
    {
        var timeout = _options.Matchers.GetValueOrDefault(matcher.MatcherType.ToLower())?.TimeoutMs
                     ?? _options.DefaultTimeoutMs;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await matcher.CalculateMatchScoreAsync(context, criteria, callbackProvider, timeoutCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            _logger.LogWarning("Matcher {MatcherId} timed out after {TimeoutMs}ms", matcher.MatcherId, timeout);
            return CreateTimeoutResult(matcher.MatcherId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing matcher {MatcherId}", matcher.MatcherId);
            return CreateErrorResult(matcher.MatcherId, ex.Message);
        }
    }

    /// <summary>
    /// 计算整体匹配结果
    /// Calculate overall match result
    /// </summary>
    private OverallMatchResult CalculateOverallResult(List<MatchResult> matchResults, TimeSpan executionTime)
    {
        // 使用OverallMatchResult的Create方法创建结果
        // Use OverallMatchResult's Create method to create result
        var isMatch = matchResults.All(r => r.IsMatch);
        var averageScore = matchResults.Count > 0 ? matchResults.Average(r => (double)r.MatchScore) : 0.0;

        // 创建一个临时的广告候选ID和上下文ID
        // Create temporary ad candidate ID and context ID
        var adCandidateId = Guid.NewGuid();
        var adContextId = $"context-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return OverallMatchResult.Create(adCandidateId, adContextId, matchResults);
    }

    /// <summary>
    /// 创建失败结果
    /// Create failure result
    /// </summary>
    private OverallMatchResult CreateFailureResult(TimeSpan executionTime, string error)
    {
        var adCandidateId = Guid.NewGuid();
        var adContextId = $"error-context-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return OverallMatchResult.CreateNotMatched(adCandidateId, adContextId, error);
    }

    /// <summary>
    /// 创建超时结果
    /// Create timeout result
    /// </summary>
    private MatchResult CreateTimeoutResult(string matcherId)
    {
        return MatchResult.CreateNoMatch(
            "timeout",
            Guid.NewGuid(),
            "Execution timeout",
            TimeSpan.FromMilliseconds(_options.DefaultTimeoutMs));
    }

    /// <summary>
    /// 创建错误结果
    /// Create error result
    /// </summary>
    private MatchResult CreateErrorResult(string matcherId, string error)
    {
        return MatchResult.CreateNoMatch(
            "error",
            Guid.NewGuid(),
            error,
            TimeSpan.Zero);
    }
}
