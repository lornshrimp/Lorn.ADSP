namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 数据访问抽象层选项配置
    /// </summary>
    public class DataAccessOptions
    {
        /// <summary>
        /// 默认超时时间
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 默认重试次数
        /// </summary>
        public int DefaultRetryCount { get; set; } = 3;

        /// <summary>
        /// 默认批量操作大小
        /// </summary>
        public int DefaultBatchSize { get; set; } = 100;

        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;

        /// <summary>
        /// 是否启用健康检查
        /// </summary>
        public bool EnableHealthChecks { get; set; } = true;

        /// <summary>
        /// 健康检查间隔
        /// </summary>
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 路由缓存过期时间
        /// </summary>
        public TimeSpan RouteCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
    }
}
