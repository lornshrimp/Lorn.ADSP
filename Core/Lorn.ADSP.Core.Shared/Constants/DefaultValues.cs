namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 系统默认值常量
/// </summary>
public static class DefaultValues
{
    /// <summary>
    /// 广告相关默认值
    /// </summary>
    public static class Advertisement
    {
        /// <summary>
        /// 默认广告权重
        /// </summary>
        public const int DefaultWeight = 100;

        /// <summary>
        /// 默认质量得分
        /// </summary>
        public const decimal DefaultQualityScore = 1.0m;

        /// <summary>
        /// 默认点击率
        /// </summary>
        public const decimal DefaultClickThroughRate = 0.01m;

        /// <summary>
        /// 默认转化率
        /// </summary>
        public const decimal DefaultConversionRate = 0.001m;

        /// <summary>
        /// 默认广告有效期 (天)
        /// </summary>
        public const int DefaultValidityPeriodDays = 30;
    }

    /// <summary>
    /// 竞价相关默认值
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// 默认底价 (分)
        /// </summary>
        public const decimal DefaultFloorPrice = 1.0m;

        /// <summary>
        /// 默认竞价倍数
        /// </summary>
        public const decimal DefaultBidMultiplier = 1.0m;

        /// <summary>
        /// 默认竞价优先级
        /// </summary>
        public const int DefaultBidPriority = 5;

        /// <summary>
        /// 默认竞价有效期 (秒)
        /// </summary>
        public const int DefaultBidValiditySeconds = 3600;
    }

    /// <summary>
    /// 定向相关默认值
    /// </summary>
    public static class Targeting
    {
        /// <summary>
        /// 默认定向权重
        /// </summary>
        public const decimal DefaultTargetingWeight = 1.0m;

        /// <summary>
        /// 默认匹配度阈值
        /// </summary>
        public const decimal DefaultMatchThreshold = 0.5m;

        /// <summary>
        /// 默认定向半径 (公里)
        /// </summary>
        public const int DefaultTargetingRadiusKm = 50;

        /// <summary>
        /// 默认年龄范围下限
        /// </summary>
        public const int DefaultMinAge = 18;

        /// <summary>
        /// 默认年龄范围上限
        /// </summary>
        public const int DefaultMaxAge = 65;
    }

    /// <summary>
    /// 预算相关默认值
    /// </summary>
    public static class Budget
    {
        /// <summary>
        /// 默认日预算 (元)
        /// </summary>
        public const decimal DefaultDailyBudget = 100.0m;

        /// <summary>
        /// 默认总预算 (元)
        /// </summary>
        public const decimal DefaultTotalBudget = 1000.0m;

        /// <summary>
        /// 默认预算分配比例
        /// </summary>
        public const decimal DefaultBudgetAllocationRatio = 1.0m;

        /// <summary>
        /// 默认预算预警阈值
        /// </summary>
        public const decimal DefaultBudgetAlertThreshold = 0.8m;
    }

    /// <summary>
    /// 频次控制默认值
    /// </summary>
    public static class FrequencyControl
    {
        /// <summary>
        /// 默认每日频次上限
        /// </summary>
        public const int DefaultDailyFrequencyCap = 10;

        /// <summary>
        /// 默认每小时频次上限
        /// </summary>
        public const int DefaultHourlyFrequencyCap = 3;

        /// <summary>
        /// 默认频次控制窗口 (小时)
        /// </summary>
        public const int DefaultFrequencyWindowHours = 24;

        /// <summary>
        /// 默认频次统计精度 (分钟)
        /// </summary>
        public const int DefaultFrequencyPrecisionMinutes = 5;
    }

    /// <summary>
    /// 缓存相关默认值
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// 默认缓存过期时间 (分钟)
        /// </summary>
        public const int DefaultExpirationMinutes = 60;

        /// <summary>
        /// 默认缓存刷新间隔 (分钟)
        /// </summary>
        public const int DefaultRefreshIntervalMinutes = 30;

        /// <summary>
        /// 默认缓存大小限制 (MB)
        /// </summary>
        public const int DefaultCacheSizeLimitMB = 1024;

        /// <summary>
        /// 默认缓存压缩阈值 (KB)
        /// </summary>
        public const int DefaultCompressionThresholdKB = 1024;
    }
}