namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models
{
    /// <summary>
    /// A/B测试配置
    /// </summary>
    public record ABTestConfig
    {
        /// <summary>
        /// 实验ID
        /// </summary>
        public required string ExperimentId { get; init; }

        /// <summary>
        /// 实验名称
        /// </summary>
        public required string ExperimentName { get; init; }

        /// <summary>
        /// 实验组配置
        /// </summary>
        public required IReadOnlyList<ExperimentGroup> Groups { get; init; }

        /// <summary>
        /// 流量分配比例
        /// </summary>
        public decimal TrafficAllocation { get; init; } = 1.0m;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; init; } = true;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; init; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; init; }

        /// <summary>
        /// 分组策略
        /// </summary>
        public string GroupingStrategy { get; init; } = "hash";

        /// <summary>
        /// 用户分组
        /// </summary>
        public ExperimentGroup? GetUserGroup(string userId)
        {
            if (!IsEnabled || Groups.Count == 0)
                return null;

            var hash = Math.Abs(userId.GetHashCode());
            var totalWeight = Groups.Sum(g => g.Weight);
            var targetWeight = (hash % 100) * totalWeight / 100;

            decimal currentWeight = 0;
            foreach (var group in Groups)
            {
                currentWeight += group.Weight;
                if (targetWeight <= currentWeight)
                    return group;
            }

            return Groups.Last();
        }
    }
}
