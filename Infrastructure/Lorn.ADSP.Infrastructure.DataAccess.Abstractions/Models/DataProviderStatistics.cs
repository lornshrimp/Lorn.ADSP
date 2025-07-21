namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 数据提供者统计信息
/// 包含提供者的性能和使用统计数据
/// </summary>
public sealed class DataProviderStatistics
{
    /// <summary>
    /// 提供者ID
    /// </summary>
    public required string ProviderId { get; init; }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public DateTimeOffset StartTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 统计结束时间
    /// </summary>
    public DateTimeOffset EndTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 总请求数
    /// </summary>
    public long TotalRequests { get; init; }

    /// <summary>
    /// 成功请求数
    /// </summary>
    public long SuccessfulRequests { get; init; }

    /// <summary>
    /// 失败请求数
    /// </summary>
    public long FailedRequests { get; init; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageResponseTime { get; init; }

    /// <summary>
    /// 最大响应时间（毫秒）
    /// </summary>
    public double MaxResponseTime { get; init; }

    /// <summary>
    /// 最小响应时间（毫秒）
    /// </summary>
    public double MinResponseTime { get; init; }

    /// <summary>
    /// 缓存命中次数
    /// </summary>
    public long CacheHits { get; init; }

    /// <summary>
    /// 缓存未命中次数
    /// </summary>
    public long CacheMisses { get; init; }

    /// <summary>
    /// 当前并发请求数
    /// </summary>
    public int CurrentConcurrentRequests { get; init; }

    /// <summary>
    /// 最大并发请求数
    /// </summary>
    public int MaxConcurrentRequests { get; init; }

    /// <summary>
    /// 错误率（0-1之间的值）
    /// </summary>
    public double ErrorRate => TotalRequests > 0 ? (double)FailedRequests / TotalRequests : 0;

    /// <summary>
    /// 成功率（0-1之间的值）
    /// </summary>
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests : 0;

    /// <summary>
    /// 缓存命中率（0-1之间的值）
    /// </summary>
    public double CacheHitRate => (CacheHits + CacheMisses) > 0 ? (double)CacheHits / (CacheHits + CacheMisses) : 0;

    /// <summary>
    /// 扩展统计信息
    /// </summary>
    public Dictionary<string, object> ExtendedStatistics { get; init; } = new();
}
