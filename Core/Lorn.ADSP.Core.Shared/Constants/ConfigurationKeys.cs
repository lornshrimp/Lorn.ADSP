namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 配置键常量
/// </summary>
public static class ConfigurationKeys
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// 主数据库连接字符串
        /// </summary>
        public const string MainConnectionString = "Database:Main:ConnectionString";

        /// <summary>
        /// 只读数据库连接字符串
        /// </summary>
        public const string ReadOnlyConnectionString = "Database:ReadOnly:ConnectionString";

        /// <summary>
        /// 数据库连接池大小
        /// </summary>
        public const string ConnectionPoolSize = "Database:ConnectionPoolSize";

        /// <summary>
        /// 数据库命令超时时间
        /// </summary>
        public const string CommandTimeout = "Database:CommandTimeout";
    }

    /// <summary>
    /// Redis缓存配置
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public const string ConnectionString = "Redis:ConnectionString";

        /// <summary>
        /// Redis数据库编号
        /// </summary>
        public const string Database = "Redis:Database";

        /// <summary>
        /// Redis连接池大小
        /// </summary>
        public const string PoolSize = "Redis:PoolSize";

        /// <summary>
        /// Redis键前缀
        /// </summary>
        public const string KeyPrefix = "Redis:KeyPrefix";
    }

    /// <summary>
    /// 广告投放配置
    /// </summary>
    public static class AdDelivery
    {
        /// <summary>
        /// 广告请求超时时间
        /// </summary>
        public const string RequestTimeout = "AdDelivery:RequestTimeout";

        /// <summary>
        /// 最大并发请求数
        /// </summary>
        public const string MaxConcurrentRequests = "AdDelivery:MaxConcurrentRequests";

        /// <summary>
        /// 默认QPS限制
        /// </summary>       
        public const string DefaultQpsLimit = "AdDelivery:DefaultQpsLimit";

        /// <summary>
        /// 广告缓存过期时间
        /// </summary>
        public const string AdCacheExpiration = "AdDelivery:AdCacheExpiration";
    }

    /// <summary>
    /// 竞价服务配置
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// 竞价超时时间
        /// </summary>
        public const string BidTimeout = "Bidding:BidTimeout";

        /// <summary>
        /// 最低竞价价格
        /// </summary>
        public const string MinBidPrice = "Bidding:MinBidPrice";

        /// <summary>
        /// 最高竞价价格
        /// </summary>
        public const string MaxBidPrice = "Bidding:MaxBidPrice";

        /// <summary>
        /// 竞价保护期
        /// </summary>
        public const string BidProtectionPeriod = "Bidding:BidProtectionPeriod";
    }

    /// <summary>
    /// 日志配置
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public const string LogLevel = "Logging:LogLevel:Default";

        /// <summary>
        /// 结构化日志开关
        /// </summary>
        public const string StructuredLogging = "Logging:StructuredLogging";

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public const string LogFilePath = "Logging:File:Path";

        /// <summary>
        /// 日志保留天数
        /// </summary>
        public const string LogRetentionDays = "Logging:RetentionDays";
    }

    /// <summary>
    /// 监控配置
    /// </summary>
    public static class Monitoring
    {
        /// <summary>
        /// 健康检查端点
        /// </summary>
        public const string HealthCheckEndpoint = "Monitoring:HealthCheck:Endpoint";

        /// <summary>
        /// 指标收集间隔
        /// </summary>
        public const string MetricsInterval = "Monitoring:Metrics:Interval";

        /// <summary>
        /// 告警阈值
        /// </summary>
        public const string AlertThreshold = "Monitoring:Alert:Threshold";

        /// <summary>
        /// 性能监控开关
        /// </summary>
        public const string PerformanceMonitoring = "Monitoring:Performance:Enabled";
    }
}