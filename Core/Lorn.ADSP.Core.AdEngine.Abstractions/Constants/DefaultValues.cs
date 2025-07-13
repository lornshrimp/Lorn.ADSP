namespace Lorn.ADSP.Core.AdEngine.Abstractions.Constants
{
    /// <summary>
    /// 默认值常量
    /// </summary>
    public static class DefaultValues
    {
        /// <summary>
        /// 默认策略版本
        /// </summary>
        public const string DefaultStrategyVersion = "1.0.0";

        /// <summary>
        /// 默认策略名称
        /// </summary>
        public const string DefaultStrategyName = "UnnamedStrategy";

        /// <summary>
        /// 默认策略作者
        /// </summary>
        public const string DefaultStrategyAuthor = "System";

        /// <summary>
        /// 默认配置版本
        /// </summary>
        public const string DefaultConfigVersion = "1.0";

        /// <summary>
        /// 默认回调超时时间（毫秒）
        /// </summary>
        public const int DefaultCallbackTimeoutMs = 5000;

        /// <summary>
        /// 默认缓存过期时间（分钟）
        /// </summary>
        public const int DefaultCacheExpirationMinutes = 30;

        /// <summary>
        /// 默认最大候选数量
        /// </summary>
        public const int DefaultMaxCandidates = 1000;

        /// <summary>
        /// 默认最小候选数量
        /// </summary>
        public const int DefaultMinCandidates = 1;

        /// <summary>
        /// 默认质量分阈值
        /// </summary>
        public const decimal DefaultQualityThreshold = 0.5m;

        /// <summary>
        /// 默认相关性阈值
        /// </summary>
        public const decimal DefaultRelevanceThreshold = 0.6m;

        /// <summary>
        /// 默认置信度阈值
        /// </summary>
        public const decimal DefaultConfidenceThreshold = 0.7m;

        /// <summary>
        /// 默认权重值
        /// </summary>
        public const decimal DefaultWeight = 1.0m;

        /// <summary>
        /// 默认CPU需求等级
        /// </summary>
        public const int DefaultCpuRequirement = 5;

        /// <summary>
        /// 默认内存需求（MB）
        /// </summary>
        public const int DefaultMemoryRequirementMB = 100;

        /// <summary>
        /// 默认网络需求等级
        /// </summary>
        public const int DefaultNetworkRequirement = 5;

        /// <summary>
        /// 默认最大并发数
        /// </summary>
        public const int DefaultMaxConcurrency = 10;
    }
}
