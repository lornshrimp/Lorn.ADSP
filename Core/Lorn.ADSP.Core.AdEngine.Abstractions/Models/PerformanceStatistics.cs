namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 性能统计信息
/// </summary>
public record PerformanceStatistics
{
    /// <summary>
    /// 总执行次数
    /// </summary>
    public long TotalExecutions { get; init; }

    /// <summary>
    /// 成功执行次数
    /// </summary>
    public long SuccessfulExecutions { get; init; }

    /// <summary>
    /// 失败执行次数
    /// </summary>
    public long FailedExecutions { get; init; }

    /// <summary>
    /// 平均执行时间（毫秒）
    /// </summary>
    public double AverageExecutionTimeMs { get; init; }

    /// <summary>
    /// 最大执行时间（毫秒）
    /// </summary>
    public double MaxExecutionTimeMs { get; init; }

    /// <summary>
    /// 最小执行时间（毫秒）
    /// </summary>
    public double MinExecutionTimeMs { get; init; }

    /// <summary>
    /// 第95百分位执行时间（毫秒）
    /// </summary>
    public double P95ExecutionTimeMs { get; init; }

    /// <summary>
    /// 第99百分位执行时间（毫秒）
    /// </summary>
    public double P99ExecutionTimeMs { get; init; }

    /// <summary>
    /// 吞吐量（每秒执行次数）
    /// </summary>
    public double ThroughputPerSecond { get; init; }

    /// <summary>
    /// 错误率
    /// </summary>
    public double ErrorRate => TotalExecutions > 0 ? (double)FailedExecutions / TotalExecutions : 0;

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 0;

    /// <summary>
    /// 平均内存使用量（字节）
    /// </summary>
    public long AverageMemoryUsage { get; init; }

    /// <summary>
    /// 峰值内存使用量（字节）
    /// </summary>
    public long PeakMemoryUsage { get; init; }

    /// <summary>
    /// 平均CPU使用时间（毫秒）
    /// </summary>
    public double AverageCpuTimeMs { get; init; }

    /// <summary>
    /// 缓存命中率
    /// </summary>
    public double CacheHitRate { get; init; }

    /// <summary>
    /// 数据库查询平均时间（毫秒）
    /// </summary>
    public double AverageDatabaseQueryTimeMs { get; init; }

    /// <summary>
    /// 外部调用平均时间（毫秒）
    /// </summary>
    public double AverageExternalCallTimeMs { get; init; }

    /// <summary>
    /// 统计时间窗口开始时间
    /// </summary>
    public DateTime WindowStartTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 统计时间窗口结束时间
    /// </summary>
    public DateTime WindowEndTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 统计时间窗口持续时间
    /// </summary>
    public TimeSpan WindowDuration => WindowEndTime - WindowStartTime;

    /// <summary>
    /// 扩展性能指标
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedMetrics { get; init; } = new Dictionary<string, object>();
}