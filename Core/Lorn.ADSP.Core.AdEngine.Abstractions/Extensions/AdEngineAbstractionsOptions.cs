namespace Lorn.ADSP.Core.AdEngine.Abstractions.Extensions
{
    /// <summary>
    /// 广告引擎抽象层选项
    /// </summary>
    public class AdEngineAbstractionsOptions
    {
        /// <summary>
        /// 是否注册策略服务
        /// </summary>
        public bool RegisterStrategyServices { get; set; } = true;

        /// <summary>
        /// 是否注册回调服务
        /// </summary>
        public bool RegisterCallbackServices { get; set; } = true;

        /// <summary>
        /// 是否注册监控服务
        /// </summary>
        public bool RegisterMonitoringServices { get; set; } = true;

        /// <summary>
        /// 默认超时时间
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;

        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableVerboseLogging { get; set; } = false;
    }
}
