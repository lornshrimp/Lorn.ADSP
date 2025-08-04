namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 定向匹配器统计信息
/// </summary>
public record TargetingMatcherStatistics
{
    /// <summary>
    /// 已注册的匹配器总数
    /// </summary>
    public int TotalRegisteredMatchers { get; init; }

    /// <summary>
    /// 启用的匹配器数量
    /// </summary>
    public int EnabledMatchersCount { get; init; }

    /// <summary>
    /// 禁用的匹配器数量
    /// </summary>
    public int DisabledMatchersCount { get; init; }

    /// <summary>
    /// 按类型分组的匹配器数量
    /// </summary>
    public IReadOnlyDictionary<string, int> MatchersByType { get; init; } =
        new Dictionary<string, int>();

    /// <summary>
    /// 按优先级分组的匹配器数量
    /// </summary>
    public IReadOnlyDictionary<int, int> MatchersByPriority { get; init; } =
        new Dictionary<int, int>();

    /// <summary>
    /// 支持并行执行的匹配器数量
    /// </summary>
    public int ParallelCapableMatchersCount { get; init; }

    /// <summary>
    /// 平均预期执行时间（毫秒）
    /// </summary>
    public double AverageExpectedExecutionTimeMs { get; init; }

    /// <summary>
    /// 最长预期执行时间（毫秒）
    /// </summary>
    public double MaxExpectedExecutionTimeMs { get; init; }

    /// <summary>
    /// 最短预期执行时间（毫秒）
    /// </summary>
    public double MinExpectedExecutionTimeMs { get; init; }

    /// <summary>
    /// 统计生成时间
    /// </summary>
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 匹配器健康状态统计
    /// </summary>
    public HealthStatistics HealthStats { get; init; } = new();

    /// <summary>
    /// 性能统计信息
    /// </summary>
    public PerformanceStatistics PerformanceStats { get; init; } = new();
}

/// <summary>
/// 健康状态统计
/// </summary>
public record HealthStatistics
{
    /// <summary>
    /// 健康的匹配器数量
    /// </summary>
    public int HealthyMatchersCount { get; init; }

    /// <summary>
    /// 不健康的匹配器数量
    /// </summary>
    public int UnhealthyMatchersCount { get; init; }

    /// <summary>
    /// 未知状态的匹配器数量
    /// </summary>
    public int UnknownStatusMatchersCount { get; init; }

    /// <summary>
    /// 健康率
    /// </summary>
    public double HealthRate { get; init; }

    /// <summary>
    /// 最后健康检查时间
    /// </summary>
    public DateTime LastHealthCheckAt { get; init; } = DateTime.UtcNow;
}