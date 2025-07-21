using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Configuration;

/// <summary>
/// 数据访问配置
/// 定义数据访问层的全局配置选项
/// </summary>
public class DataAccessConfiguration
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "DataAccess";

    /// <summary>
    /// 是否启用自动发现
    /// </summary>
    public bool EnableAutoDiscovery { get; set; } = true;

    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// 默认路由策略
    /// </summary>
    public RoutingStrategy DefaultRoutingStrategy { get; set; } = RoutingStrategy.Priority;

    /// <summary>
    /// 默认一致性级别
    /// </summary>
    public DataConsistencyLevel DefaultConsistencyLevel { get; set; } = DataConsistencyLevel.Eventual;

    /// <summary>
    /// 默认操作超时时间（毫秒）
    /// </summary>
    public int DefaultTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// 是否启用统计信息收集
    /// </summary>
    public bool EnableStatistics { get; set; } = true;

    /// <summary>
    /// 统计信息收集间隔（秒）
    /// </summary>
    public int StatisticsIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// 健康检查间隔（秒）
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// 是否启用调试模式
    /// </summary>
    public bool EnableDebugMode { get; set; } = false;

    /// <summary>
    /// 缓存配置
    /// </summary>
    public CacheConfiguration Cache { get; set; } = new();

    /// <summary>
    /// 路由配置
    /// </summary>
    public RoutingConfiguration Routing { get; set; } = new();

    /// <summary>
    /// 重试配置
    /// </summary>
    public RetryConfiguration Retry { get; set; } = new();

    /// <summary>
    /// 监控配置
    /// </summary>
    public MonitoringConfiguration Monitoring { get; set; } = new();
}

/// <summary>
/// 缓存配置
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// 是否启用缓存
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 默认缓存过期时间（分钟）
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// 最大缓存大小（MB）
    /// </summary>
    public int MaxSizeMB { get; set; } = 100;

    /// <summary>
    /// 是否启用分布式缓存
    /// </summary>
    public bool EnableDistributedCache { get; set; } = false;

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string KeyPrefix { get; set; } = "ADSP_";

    /// <summary>
    /// 是否启用缓存压缩
    /// </summary>
    public bool EnableCompression { get; set; } = false;
}

/// <summary>
/// 路由配置
/// </summary>
public class RoutingConfiguration
{
    /// <summary>
    /// 是否启用智能路由
    /// </summary>
    public bool EnableSmartRouting { get; set; } = true;

    /// <summary>
    /// 是否启用故障转移
    /// </summary>
    public bool EnableFailover { get; set; } = true;

    /// <summary>
    /// 故障转移超时时间（毫秒）
    /// </summary>
    public int FailoverTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 是否启用负载均衡
    /// </summary>
    public bool EnableLoadBalancing { get; set; } = false;

    /// <summary>
    /// 路由决策缓存时间（秒）
    /// </summary>
    public int DecisionCacheSeconds { get; set; } = 300;

    /// <summary>
    /// 最大并发路由请求数
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 1000;
}

/// <summary>
/// 重试配置
/// </summary>
public class RetryConfiguration
{
    /// <summary>
    /// 是否启用重试
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// 重试延迟（毫秒）
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// 重试延迟递增因子
    /// </summary>
    public double RetryDelayMultiplier { get; set; } = 2.0;

    /// <summary>
    /// 最大重试延迟（毫秒）
    /// </summary>
    public int MaxRetryDelayMs { get; set; } = 30000;

    /// <summary>
    /// 重试的异常类型
    /// </summary>
    public string[] RetryableExceptions { get; set; } =
    {
        "System.TimeoutException",
        "System.Net.Http.HttpRequestException",
        "Microsoft.Data.SqlClient.SqlException"
    };
}

/// <summary>
/// 监控配置
/// </summary>
public class MonitoringConfiguration
{
    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// 是否启用详细日志
    /// </summary>
    public bool EnableVerboseLogging { get; set; } = false;

    /// <summary>
    /// 是否启用指标收集
    /// </summary>
    public bool EnableMetricsCollection { get; set; } = true;

    /// <summary>
    /// 指标收集间隔（秒）
    /// </summary>
    public int MetricsIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// 是否启用事件跟踪
    /// </summary>
    public bool EnableEventTracking { get; set; } = false;

    /// <summary>
    /// 慢查询阈值（毫秒）
    /// </summary>
    public int SlowQueryThresholdMs { get; set; } = 1000;

    /// <summary>
    /// 是否记录慢查询
    /// </summary>
    public bool LogSlowQueries { get; set; } = true;
}
