using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 活动目标值对象
    /// </summary>
    public class CampaignObjective : ValueObject
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public string ObjectiveType { get; private set; } = string.Empty;

        /// <summary>
        /// 目标展示次数
        /// </summary>
        public long? TargetImpressions { get; private set; }

        /// <summary>
        /// 目标点击次数
        /// </summary>
        public long? TargetClicks { get; private set; }

        /// <summary>
        /// 目标转化次数
        /// </summary>
        public long? TargetConversions { get; private set; }

        /// <summary>
        /// 目标点击率
        /// </summary>
        public decimal? TargetClickThroughRate { get; private set; }

        /// <summary>
        /// 目标转化率
        /// </summary>
        public decimal? TargetConversionRate { get; private set; }

        /// <summary>
        /// 目标每次点击成本
        /// </summary>
        public decimal? TargetCostPerClick { get; private set; }

        /// <summary>
        /// 目标每次转化成本
        /// </summary>
        public decimal? TargetCostPerConversion { get; private set; }

        /// <summary>
        /// 目标广告支出回报率
        /// </summary>
        public decimal? TargetReturnOnAdSpend { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private CampaignObjective() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CampaignObjective(
            string objectiveType,
            long? targetImpressions = null,
            long? targetClicks = null,
            long? targetConversions = null,
            decimal? targetClickThroughRate = null,
            decimal? targetConversionRate = null,
            decimal? targetCostPerClick = null,
            decimal? targetCostPerConversion = null,
            decimal? targetReturnOnAdSpend = null)
        {
            if (string.IsNullOrWhiteSpace(objectiveType))
                throw new ArgumentException("目标类型不能为空", nameof(objectiveType));

            ObjectiveType = objectiveType;
            TargetImpressions = targetImpressions;
            TargetClicks = targetClicks;
            TargetConversions = targetConversions;
            TargetClickThroughRate = targetClickThroughRate;
            TargetConversionRate = targetConversionRate;
            TargetCostPerClick = targetCostPerClick;
            TargetCostPerConversion = targetCostPerConversion;
            TargetReturnOnAdSpend = targetReturnOnAdSpend;
        }

        /// <summary>
        /// 创建品牌知名度目标
        /// </summary>
        public static CampaignObjective CreateBrandAwareness(long targetImpressions)
        {
            return new CampaignObjective("BrandAwareness", targetImpressions: targetImpressions);
        }

        /// <summary>
        /// 创建流量目标
        /// </summary>
        public static CampaignObjective CreateTraffic(long targetClicks, decimal? targetCostPerClick = null)
        {
            return new CampaignObjective("Traffic", targetClicks: targetClicks, targetCostPerClick: targetCostPerClick);
        }

        /// <summary>
        /// 创建转化目标
        /// </summary>
        public static CampaignObjective CreateConversions(long targetConversions, decimal? targetCostPerConversion = null)
        {
            return new CampaignObjective("Conversions", targetConversions: targetConversions, targetCostPerConversion: targetCostPerConversion);
        }

        /// <summary>
        /// 创建销售目标
        /// </summary>
        public static CampaignObjective CreateSales(decimal targetReturnOnAdSpend)
        {
            return new CampaignObjective("Sales", targetReturnOnAdSpend: targetReturnOnAdSpend);
        }

        /// <summary>
        /// 计算目标完成率
        /// </summary>
        public decimal CalculateCompletionRate(long actualImpressions, long actualClicks, long actualConversions, decimal actualSpent)
        {
            return ObjectiveType switch
            {
                "BrandAwareness" => TargetImpressions.HasValue ? (decimal)actualImpressions / TargetImpressions.Value : 0m,
                "Traffic" => TargetClicks.HasValue ? (decimal)actualClicks / TargetClicks.Value : 0m,
                "Conversions" => TargetConversions.HasValue ? (decimal)actualConversions / TargetConversions.Value : 0m,
                _ => 0m
            };
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ObjectiveType;
            yield return TargetImpressions ?? 0;
            yield return TargetClicks ?? 0;
            yield return TargetConversions ?? 0;
            yield return TargetClickThroughRate ?? 0m;
            yield return TargetConversionRate ?? 0m;
            yield return TargetCostPerClick ?? 0m;
            yield return TargetCostPerConversion ?? 0m;
            yield return TargetReturnOnAdSpend ?? 0m;
        }
    }
}
