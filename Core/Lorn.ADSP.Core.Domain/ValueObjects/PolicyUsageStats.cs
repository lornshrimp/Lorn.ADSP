namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 策略使用统计
    /// </summary>
    public class PolicyUsageStats
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string PolicyId { get; set; } = string.Empty;

        /// <summary>
        /// 总配置数量
        /// </summary>
        public int TotalConfigs { get; set; }

        /// <summary>
        /// 活跃配置数量
        /// </summary>
        public int ActiveConfigs { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        /// <summary>
        /// 平均性能表现
        /// </summary>
        public decimal AveragePerformance { get; set; }
    }
}
