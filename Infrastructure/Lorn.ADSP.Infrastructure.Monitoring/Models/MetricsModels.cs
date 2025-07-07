using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Infrastructure.Monitoring.Models;

/// <summary>
/// 策略指标
/// </summary>
public record StrategyMetrics
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// 执行ID
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// 指标时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public double ExecutionTimeMs { get; init; }

    /// <summary>
    /// 内存使用量（字节）
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// CPU使用率（百分比）
    /// </summary>
    public double CpuUsage { get; init; }

    /// <summary>
    /// 处理的候选数量
    /// </summary>
    public int ProcessedCount { get; init; }

    /// <summary>
    /// 成功处理的数量
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// 失败处理的数量
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// 处理速率（个/秒）
    /// </summary>
    public double ProcessingRate => ExecutionTimeMs > 0 ? ProcessedCount * 1000.0 / ExecutionTimeMs : 0;

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate => ProcessedCount > 0 ? (double)SuccessCount / ProcessedCount : 0;

    /// <summary>
    /// 自定义指标
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomMetrics { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// 性能指标
/// </summary>
public record PerformanceMetrics
{
    /// <summary>
    /// 组件名称
    /// </summary>
    public required string ComponentName { get; init; }

    /// <summary>
    /// 指标时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 响应时间（毫秒）
    /// </summary>
    public double ResponseTimeMs { get; init; }

    /// <summary>
    /// 吞吐量（请求/秒）
    /// </summary>
    public double Throughput { get; init; }

    /// <summary>
    /// 错误率（百分比）
    /// </summary>
    public double ErrorRate { get; init; }

    /// <summary>
    /// 并发数
    /// </summary>
    public int ConcurrentRequests { get; init; }

    /// <summary>
    /// 队列长度
    /// </summary>
    public int QueueLength { get; init; }

    /// <summary>
    /// 资源使用情况
    /// </summary>
    public ResourceUsage ResourceUsage { get; init; } = new();

    /// <summary>
    /// 延迟分布
    /// </summary>
    public LatencyDistribution LatencyDistribution { get; init; } = new();

    /// <summary>
    /// 业务指标
    /// </summary>
    public IReadOnlyDictionary<string, double> BusinessMetrics { get; init; } =
        new Dictionary<string, double>();
}

/// <summary>
/// 业务指标
/// </summary>
public record BusinessMetrics
{
    /// <summary>
    /// 指标时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 广告展示数
    /// </summary>
    public long Impressions { get; init; }

    /// <summary>
    /// 广告点击数
    /// </summary>
    public long Clicks { get; init; }

    /// <summary>
    /// 转化数
    /// </summary>
    public long Conversions { get; init; }

    /// <summary>
    /// 收入（分）
    /// </summary>
    public decimal Revenue { get; init; }

    /// <summary>
    /// 成本（分）
    /// </summary>
    public decimal Cost { get; init; }

    /// <summary>
    /// 点击率
    /// </summary>
    public double ClickThroughRate => Impressions > 0 ? (double)Clicks / Impressions : 0;

    /// <summary>
    /// 转化率
    /// </summary>
    public double ConversionRate => Clicks > 0 ? (double)Conversions / Clicks : 0;

    /// <summary>
    /// 平均每次点击成本（分）
    /// </summary>
    public decimal CostPerClick => Clicks > 0 ? Cost / Clicks : 0;

    /// <summary>
    /// 平均每次转化成本（分）
    /// </summary>
    public decimal CostPerConversion => Conversions > 0 ? Cost / Conversions : 0;

    /// <summary>
    /// 投资回报率
    /// </summary>
    public decimal ReturnOnInvestment => Cost > 0 ? (Revenue - Cost) / Cost : 0;

    /// <summary>
    /// 填充率
    /// </summary>
    public double FillRate { get; init; }

    /// <summary>
    /// 质量评分
    /// </summary>
    public double QualityScore { get; init; }

    /// <summary>
    /// 广告主数量
    /// </summary>
    public int ActiveAdvertisers { get; init; }

    /// <summary>
    /// 活跃广告数量
    /// </summary>
    public int ActiveAds { get; init; }

    /// <summary>
    /// 自定义业务指标
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomMetrics { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// 执行统计
/// </summary>
public record ExecutionStatistics
{
    /// <summary>
    /// 执行ID
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// 策略ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// 执行状态
    /// </summary>
    public ExecutionStatus Status { get; init; } = ExecutionStatus.Running;

    /// <summary>
    /// 输入数据大小
    /// </summary>
    public int InputSize { get; init; }

    /// <summary>
    /// 输出数据大小
    /// </summary>
    public int OutputSize { get; init; }

    /// <summary>
    /// 处理的数据量
    /// </summary>
    public long ProcessedDataSize { get; init; }

    /// <summary>
    /// 数据库操作次数
    /// </summary>
    public int DatabaseOperations { get; init; }

    /// <summary>
    /// 缓存操作次数
    /// </summary>
    public int CacheOperations { get; init; }

    /// <summary>
    /// 网络调用次数
    /// </summary>
    public int NetworkCalls { get; init; }

    /// <summary>
    /// 执行时长
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// 处理速度（条/秒）
    /// </summary>
    public double ProcessingSpeed => Duration.TotalSeconds > 0 ? InputSize / Duration.TotalSeconds : 0;

    /// <summary>
    /// 详细统计信息
    /// </summary>
    public IReadOnlyDictionary<string, object> DetailedStats { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// 指标批次
/// </summary>
public record MetricsBatch
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string BatchId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 批次时间戳
    /// </summary>
    public DateTime BatchTimestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 策略指标列表
    /// </summary>
    public IReadOnlyList<StrategyMetrics> StrategyMetrics { get; init; } = Array.Empty<StrategyMetrics>();

    /// <summary>
    /// 性能指标列表
    /// </summary>
    public IReadOnlyList<PerformanceMetrics> PerformanceMetrics { get; init; } = Array.Empty<PerformanceMetrics>();

    /// <summary>
    /// 业务指标列表
    /// </summary>
    public IReadOnlyList<BusinessMetrics> BusinessMetrics { get; init; } = Array.Empty<BusinessMetrics>();

    /// <summary>
    /// 批次大小
    /// </summary>
    public int BatchSize => StrategyMetrics.Count + PerformanceMetrics.Count + BusinessMetrics.Count;
}

/// <summary>
/// 告警事件
/// </summary>
public record AlertEvent
{
    /// <summary>
    /// 告警ID
    /// </summary>
    public string AlertId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 告警时间
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 告警级别
    /// </summary>
    public AlertLevel Level { get; init; } = AlertLevel.Warning;

    /// <summary>
    /// 告警类型
    /// </summary>
    public required string AlertType { get; init; }

    /// <summary>
    /// 告警消息
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 告警来源
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// 受影响的组件
    /// </summary>
    public IReadOnlyList<string> AffectedComponents { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 告警详情
    /// </summary>
    public IReadOnlyDictionary<string, object> Details { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// 推荐操作
    /// </summary>
    public IReadOnlyList<string> RecommendedActions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 是否已解决
    /// </summary>
    public bool IsResolved { get; init; } = false;

    /// <summary>
    /// 解决时间
    /// </summary>
    public DateTime? ResolvedAt { get; init; }
}

/// <summary>
/// 审计日志
/// </summary>
public record AuditLog
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public string LogId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 操作类型
    /// </summary>
    public required string Action { get; init; }

    /// <summary>
    /// 操作用户
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// 资源类型
    /// </summary>
    public required string ResourceType { get; init; }

    /// <summary>
    /// 资源ID
    /// </summary>
    public required string ResourceId { get; init; }

    /// <summary>
    /// 操作结果
    /// </summary>
    public AuditResult Result { get; init; } = AuditResult.Success;

    /// <summary>
    /// 操作详情
    /// </summary>
    public IReadOnlyDictionary<string, object> Details { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string? SessionId { get; init; }
}

/// <summary>
/// 资源使用情况
/// </summary>
public record ResourceUsage
{
    /// <summary>
    /// CPU使用率（百分比）
    /// </summary>
    public double CpuUsage { get; init; }

    /// <summary>
    /// 内存使用量（字节）
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// 磁盘使用量（字节）
    /// </summary>
    public long DiskUsage { get; init; }

    /// <summary>
    /// 网络流入量（字节/秒）
    /// </summary>
    public long NetworkInBytes { get; init; }

    /// <summary>
    /// 网络流出量（字节/秒）
    /// </summary>
    public long NetworkOutBytes { get; init; }

    /// <summary>
    /// 连接数
    /// </summary>
    public int ConnectionCount { get; init; }

    /// <summary>
    /// 线程数
    /// </summary>
    public int ThreadCount { get; init; }
}

/// <summary>
/// 延迟分布
/// </summary>
public record LatencyDistribution
{
    /// <summary>
    /// P50延迟（毫秒）
    /// </summary>
    public double P50 { get; init; }

    /// <summary>
    /// P90延迟（毫秒）
    /// </summary>
    public double P90 { get; init; }

    /// <summary>
    /// P95延迟（毫秒）
    /// </summary>
    public double P95 { get; init; }

    /// <summary>
    /// P99延迟（毫秒）
    /// </summary>
    public double P99 { get; init; }

    /// <summary>
    /// 最小延迟（毫秒）
    /// </summary>
    public double Min { get; init; }

    /// <summary>
    /// 最大延迟（毫秒）
    /// </summary>
    public double Max { get; init; }

    /// <summary>
    /// 平均延迟（毫秒）
    /// </summary>
    public double Average { get; init; }

    /// <summary>
    /// 标准差
    /// </summary>
    public double StandardDeviation { get; init; }
}