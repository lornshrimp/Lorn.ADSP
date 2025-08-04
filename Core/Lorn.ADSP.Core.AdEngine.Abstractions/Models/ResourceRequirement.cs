namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 资源需求信息
/// 描述匹配器运行所需的系统资源
/// </summary>
public record ResourceRequirement
{
    /// <summary>
    /// 预期内存使用量（MB）
    /// </summary>
    public double ExpectedMemoryUsageMB { get; init; } = 10.0;

    /// <summary>
    /// 最大内存使用量（MB）
    /// </summary>
    public double MaxMemoryUsageMB { get; init; } = 50.0;

    /// <summary>
    /// 预期CPU使用率（百分比）
    /// </summary>
    public double ExpectedCpuUsagePercent { get; init; } = 5.0;

    /// <summary>
    /// 最大CPU使用率（百分比）
    /// </summary>
    public double MaxCpuUsagePercent { get; init; } = 20.0;

    /// <summary>
    /// 是否需要网络访问
    /// </summary>
    public bool RequiresNetworkAccess { get; init; } = false;

    /// <summary>
    /// 是否需要磁盘访问
    /// </summary>
    public bool RequiresDiskAccess { get; init; } = false;

    /// <summary>
    /// 是否需要数据库访问
    /// </summary>
    public bool RequiresDatabaseAccess { get; init; } = false;

    /// <summary>
    /// 是否需要缓存访问
    /// </summary>
    public bool RequiresCacheAccess { get; init; } = true;

    /// <summary>
    /// 预期磁盘使用量（MB）
    /// </summary>
    public double ExpectedDiskUsageMB { get; init; } = 0.0;

    /// <summary>
    /// 预期网络带宽使用量（KB/s）
    /// </summary>
    public double ExpectedNetworkBandwidthKBps { get; init; } = 0.0;

    /// <summary>
    /// 并发执行能力
    /// </summary>
    public int ConcurrencyCapability { get; init; } = 100;

    /// <summary>
    /// 是否为CPU密集型任务
    /// </summary>
    public bool IsCpuIntensive { get; init; } = false;

    /// <summary>
    /// 是否为内存密集型任务
    /// </summary>
    public bool IsMemoryIntensive { get; init; } = false;

    /// <summary>
    /// 是否为I/O密集型任务
    /// </summary>
    public bool IsIOIntensive { get; init; } = false;

    /// <summary>
    /// 依赖的外部服务列表
    /// </summary>
    public IReadOnlyList<string> ExternalDependencies { get; init; } = [];

    /// <summary>
    /// 资源使用模式
    /// </summary>
    public ResourceUsagePattern UsagePattern { get; init; } = ResourceUsagePattern.Steady;

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// 资源使用模式
/// </summary>
public enum ResourceUsagePattern
{
    /// <summary>
    /// 稳定使用
    /// </summary>
    Steady,

    /// <summary>
    /// 突发使用
    /// </summary>
    Burst,

    /// <summary>
    /// 周期性使用
    /// </summary>
    Periodic,

    /// <summary>
    /// 渐增使用
    /// </summary>
    Incremental,

    /// <summary>
    /// 峰值使用
    /// </summary>
    Peak
}