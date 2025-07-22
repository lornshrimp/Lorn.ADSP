using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 策略使用统计 - 值对象
    /// </summary>
    public class PolicyUsageStats : ValueObject
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string PolicyId { get; }

        /// <summary>
        /// 总配置数量
        /// </summary>
        public int TotalConfigs { get; }

        /// <summary>
        /// 活跃配置数量
        /// </summary>
        public int ActiveConfigs { get; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime? LastUsedAt { get; }

        /// <summary>
        /// 平均性能表现
        /// </summary>
        public decimal AveragePerformance { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        private PolicyUsageStats(
            string policyId,
            int totalConfigs,
            int activeConfigs,
            DateTime? lastUsedAt,
            decimal averagePerformance)
        {
            PolicyId = policyId ?? throw new ArgumentNullException(nameof(policyId));
            TotalConfigs = totalConfigs >= 0 ? totalConfigs : throw new ArgumentException("总配置数量不能为负数", nameof(totalConfigs));
            ActiveConfigs = activeConfigs >= 0 ? activeConfigs : throw new ArgumentException("活跃配置数量不能为负数", nameof(activeConfigs));
            LastUsedAt = lastUsedAt;
            AveragePerformance = averagePerformance >= 0 ? averagePerformance : throw new ArgumentException("平均性能表现不能为负数", nameof(averagePerformance));

            if (ActiveConfigs > TotalConfigs)
            {
                throw new ArgumentException("活跃配置数量不能超过总配置数量");
            }
        }

        /// <summary>
        /// 创建策略使用统计
        /// </summary>
        public static PolicyUsageStats Create(
            string policyId,
            int totalConfigs,
            int activeConfigs,
            DateTime? lastUsedAt = null,
            decimal averagePerformance = 0m)
        {
            return new PolicyUsageStats(
                policyId,
                totalConfigs,
                activeConfigs,
                lastUsedAt,
                averagePerformance);
        }

        /// <summary>
        /// 计算配置使用率
        /// </summary>
        public decimal GetUsageRate()
        {
            if (TotalConfigs == 0)
                return 0m;

            return (decimal)ActiveConfigs / TotalConfigs;
        }

        /// <summary>
        /// 是否为高使用率策略
        /// </summary>
        public bool IsHighUsage()
        {
            return GetUsageRate() >= 0.8m;
        }

        /// <summary>
        /// 是否为低使用率策略
        /// </summary>
        public bool IsLowUsage()
        {
            return GetUsageRate() <= 0.2m;
        }

        /// <summary>
        /// 是否为活跃策略（最近30天内使用过）
        /// </summary>
        public bool IsActive()
        {
            if (!LastUsedAt.HasValue)
                return false;

            return LastUsedAt.Value >= DateTime.UtcNow.AddDays(-30);
        }

        /// <summary>
        /// 是否为高性能策略
        /// </summary>
        public bool IsHighPerformance()
        {
            return AveragePerformance >= 0.7m;
        }

        /// <summary>
        /// 是否为低性能策略
        /// </summary>
        public bool IsLowPerformance()
        {
            return AveragePerformance <= 0.3m;
        }

        /// <summary>
        /// 获取策略状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            var status = new List<string>();

            if (IsHighUsage())
                status.Add("高使用率");
            else if (IsLowUsage())
                status.Add("低使用率");
            else
                status.Add("中等使用率");

            if (IsActive())
                status.Add("活跃");
            else
                status.Add("非活跃");

            if (IsHighPerformance())
                status.Add("高性能");
            else if (IsLowPerformance())
                status.Add("低性能");
            else
                status.Add("中等性能");

            return string.Join(", ", status);
        }

        /// <summary>
        /// 更新平均性能表现
        /// </summary>
        public PolicyUsageStats WithUpdatedPerformance(decimal newPerformance)
        {
            if (newPerformance < 0)
                throw new ArgumentException("性能表现不能为负数", nameof(newPerformance));

            return new PolicyUsageStats(
                PolicyId,
                TotalConfigs,
                ActiveConfigs,
                LastUsedAt,
                newPerformance);
        }

        /// <summary>
        /// 更新最后使用时间
        /// </summary>
        public PolicyUsageStats WithUpdatedLastUsed(DateTime lastUsedAt)
        {
            return new PolicyUsageStats(
                PolicyId,
                TotalConfigs,
                ActiveConfigs,
                lastUsedAt,
                AveragePerformance);
        }

        /// <summary>
        /// 更新配置数量
        /// </summary>
        public PolicyUsageStats WithUpdatedConfigs(int totalConfigs, int activeConfigs)
        {
            return new PolicyUsageStats(
                PolicyId,
                totalConfigs,
                activeConfigs,
                LastUsedAt,
                AveragePerformance);
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PolicyId;
            yield return TotalConfigs;
            yield return ActiveConfigs;
            yield return LastUsedAt ?? DateTime.MinValue;
            yield return AveragePerformance;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"PolicyUsageStats(PolicyId={PolicyId}, " +
                   $"Configs={ActiveConfigs}/{TotalConfigs}, " +
                   $"Performance={AveragePerformance:P}, " +
                   $"LastUsed={LastUsedAt?.ToString("yyyy-MM-dd") ?? "Never"})";
        }
    }
}
