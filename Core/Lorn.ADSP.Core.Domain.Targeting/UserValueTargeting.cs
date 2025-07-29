using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 用户价值定向条件
    /// 实现基于用户价值评估的定向规则配置
    /// </summary>
    public class UserValueTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "价值定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Value";

        /// <summary>
        /// 最小参与度评分 (0-100)
        /// </summary>
        public int MinEngagementScore => GetRule("MinEngagementScore", 0);

        /// <summary>
        /// 最小忠诚度评分 (0-100)
        /// </summary>
        public int MinLoyaltyScore => GetRule("MinLoyaltyScore", 0);

        /// <summary>
        /// 最小消费价值评分 (0-100)
        /// </summary>
        public int MinMonetaryScore => GetRule("MinMonetaryScore", 0);

        /// <summary>
        /// 最小潜力评分 (0-100)
        /// </summary>
        public int MinPotentialScore => GetRule("MinPotentialScore", 0);

        /// <summary>
        /// 最小综合评分 (0-100)
        /// </summary>
        public int MinOverallScore => GetRule("MinOverallScore", 0);

        /// <summary>
        /// 最小预估生命周期价值（元）
        /// </summary>
        public decimal MinEstimatedLTV => GetRule("MinEstimatedLTV", 0.0m);

        /// <summary>
        /// 目标消费能力等级
        /// </summary>
        public IReadOnlyList<SpendingLevel> TargetSpendingLevels => GetRule<List<SpendingLevel>>("TargetSpendingLevels") ?? new List<SpendingLevel>();

        /// <summary>
        /// 目标用户价值等级
        /// </summary>
        public IReadOnlyList<ValueTier> TargetValueTiers => GetRule<List<ValueTier>>("TargetValueTiers") ?? new List<ValueTier>();

        /// <summary>
        /// 最小转化概率 (0.0-1.0)
        /// </summary>
        public decimal MinConversionProbability => GetRule("MinConversionProbability", 0.0m);

        /// <summary>
        /// 是否仅定向高价值用户
        /// </summary>
        public bool HighValueUsersOnly => GetRule("HighValueUsersOnly", false);

        /// <summary>
        /// 是否仅定向活跃用户
        /// </summary>
        public bool ActiveUsersOnly => GetRule("ActiveUsersOnly", false);

        /// <summary>
        /// 是否仅定向忠诚用户
        /// </summary>
        public bool LoyalUsersOnly => GetRule("LoyalUsersOnly", false);

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserValueTargeting(
            int minEngagementScore = 0,
            int minLoyaltyScore = 0,
            int minMonetaryScore = 0,
            int minPotentialScore = 0,
            int minOverallScore = 0,
            decimal minEstimatedLTV = 0.0m,
            IList<SpendingLevel>? targetSpendingLevels = null,
            IList<ValueTier>? targetValueTiers = null,
            decimal minConversionProbability = 0.0m,
            bool highValueUsersOnly = false,
            bool activeUsersOnly = false,
            bool loyalUsersOnly = false,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(minEngagementScore, minLoyaltyScore, minMonetaryScore, minPotentialScore, minOverallScore, minEstimatedLTV, targetSpendingLevels, targetValueTiers, minConversionProbability, highValueUsersOnly, activeUsersOnly, loyalUsersOnly), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则集合
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            int minEngagementScore,
            int minLoyaltyScore,
            int minMonetaryScore,
            int minPotentialScore,
            int minOverallScore,
            decimal minEstimatedLTV,
            IList<SpendingLevel>? targetSpendingLevels,
            IList<ValueTier>? targetValueTiers,
            decimal minConversionProbability,
            bool highValueUsersOnly,
            bool activeUsersOnly,
            bool loyalUsersOnly)
        {
            var rules = new List<TargetingRule>();

            if (minEngagementScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinEngagementScore",
                    minEngagementScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小参与度评分"));
            }

            if (minLoyaltyScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinLoyaltyScore",
                    minLoyaltyScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小忠诚度评分"));
            }

            if (minMonetaryScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinMonetaryScore",
                    minMonetaryScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小消费价值评分"));
            }

            if (minPotentialScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinPotentialScore",
                    minPotentialScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小潜力评分"));
            }

            if (minOverallScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinOverallScore",
                    minOverallScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小综合评分"));
            }

            if (minEstimatedLTV > 0)
            {
                rules.Add(new TargetingRule(
                    "MinEstimatedLTV",
                    minEstimatedLTV.ToString(),
                    "Decimal",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小预估生命周期价值"));
            }

            if (targetSpendingLevels != null && targetSpendingLevels.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetSpendingLevels",
                    System.Text.Json.JsonSerializer.Serialize(targetSpendingLevels.Select(s => (int)s)),
                    "Json",
                    "Value",
                    false,
                    1.0m,
                    "in",
                    "目标消费能力等级"));
            }

            if (targetValueTiers != null && targetValueTiers.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetValueTiers",
                    System.Text.Json.JsonSerializer.Serialize(targetValueTiers.Select(v => (int)v)),
                    "Json",
                    "Value",
                    false,
                    1.0m,
                    "in",
                    "目标用户价值等级"));
            }

            if (minConversionProbability > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConversionProbability",
                    minConversionProbability.ToString(),
                    "Decimal",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "最小转化概率"));
            }

            if (highValueUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "HighValueUsersOnly",
                    highValueUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "仅定向高价值用户"));
            }

            if (activeUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "ActiveUsersOnly",
                    activeUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "仅定向活跃用户"));
            }

            if (loyalUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "LoyalUsersOnly",
                    loyalUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "仅定向忠诚用户"));
            }

            return rules;
        }

        /// <summary>
        /// 创建价值定向条件
        /// </summary>
        public static UserValueTargeting Create(
            int minEngagementScore = 0,
            int minLoyaltyScore = 0,
            int minMonetaryScore = 0,
            int minPotentialScore = 0,
            int minOverallScore = 0,
            decimal minEstimatedLTV = 0.0m,
            IList<SpendingLevel>? targetSpendingLevels = null,
            IList<ValueTier>? targetValueTiers = null,
            decimal minConversionProbability = 0.0m,
            bool highValueUsersOnly = false,
            bool activeUsersOnly = false,
            bool loyalUsersOnly = false,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserValueTargeting(minEngagementScore, minLoyaltyScore, minMonetaryScore, minPotentialScore, minOverallScore, minEstimatedLTV, targetSpendingLevels, targetValueTiers, minConversionProbability, highValueUsersOnly, activeUsersOnly, loyalUsersOnly, weight, isEnabled);
        }

        /// <summary>
        /// 创建高价值用户定向
        /// </summary>
        public static UserValueTargeting CreateHighValue(
            int minOverallScore = 80,
            IList<ValueTier>? targetValueTiers = null,
            decimal minEstimatedLTV = 500.0m,
            decimal weight = 1.5m)
        {
            var valueTiers = targetValueTiers ?? new List<ValueTier> { ValueTier.Premium, ValueTier.VIP };
            return new UserValueTargeting(
                minOverallScore: minOverallScore,
                targetValueTiers: valueTiers,
                minEstimatedLTV: minEstimatedLTV,
                highValueUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// 创建基于消费能力的定向
        /// </summary>
        public static UserValueTargeting CreateBySpendingLevel(
            IList<SpendingLevel> targetSpendingLevels,
            int minMonetaryScore = 50,
            decimal weight = 1.0m)
        {
            return new UserValueTargeting(
                minMonetaryScore: minMonetaryScore,
                targetSpendingLevels: targetSpendingLevels,
                weight: weight);
        }

        /// <summary>
        /// 创建基于转化概率的定向
        /// </summary>
        public static UserValueTargeting CreateByConversionProbability(
            decimal minConversionProbability,
            int minPotentialScore = 60,
            decimal weight = 1.2m)
        {
            return new UserValueTargeting(
                minPotentialScore: minPotentialScore,
                minConversionProbability: minConversionProbability,
                weight: weight);
        }

        /// <summary>
        /// 创建活跃用户定向
        /// </summary>
        public static UserValueTargeting CreateActiveUsers(
            int minEngagementScore = 60,
            decimal weight = 1.0m)
        {
            return new UserValueTargeting(
                minEngagementScore: minEngagementScore,
                activeUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// 创建忠诚用户定向
        /// </summary>
        public static UserValueTargeting CreateLoyalUsers(
            int minLoyaltyScore = 70,
            decimal weight = 1.1m)
        {
            return new UserValueTargeting(
                minLoyaltyScore: minLoyaltyScore,
                loyalUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// 添加目标消费能力等级
        /// </summary>
        /// <param name="spendingLevel">消费能力等级</param>
        public void AddTargetSpendingLevel(SpendingLevel spendingLevel)
        {
            var currentList = TargetSpendingLevels.ToList();
            if (!currentList.Contains(spendingLevel))
            {
                currentList.Add(spendingLevel);
                SetRule("TargetSpendingLevels", currentList.Select(s => (int)s).ToList());
            }
        }

        /// <summary>
        /// 添加目标价值等级
        /// </summary>
        /// <param name="valueTier">价值等级</param>
        public void AddTargetValueTier(ValueTier valueTier)
        {
            var currentList = TargetValueTiers.ToList();
            if (!currentList.Contains(valueTier))
            {
                currentList.Add(valueTier);
                SetRule("TargetValueTiers", currentList.Select(v => (int)v).ToList());
            }
        }

        /// <summary>
        /// 更新最小评分阈值
        /// </summary>
        /// <param name="scoreType">评分类型</param>
        /// <param name="minScore">最小评分</param>
        public void UpdateMinScore(string scoreType, int minScore)
        {
            if (minScore < 0 || minScore > 100)
                throw new ArgumentException("评分必须在0-100之间", nameof(minScore));

            var validScoreTypes = new[] { "EngagementScore", "LoyaltyScore", "MonetaryScore", "PotentialScore", "OverallScore" };
            if (!validScoreTypes.Contains(scoreType))
                throw new ArgumentException($"无效的评分类型: {scoreType}", nameof(scoreType));

            SetRule($"Min{scoreType}", minScore);
        }

        /// <summary>
        /// 更新最小生命周期价值
        /// </summary>
        /// <param name="minLTV">最小生命周期价值</param>
        public void UpdateMinEstimatedLTV(decimal minLTV)
        {
            if (minLTV < 0)
                throw new ArgumentException("生命周期价值不能为负数", nameof(minLTV));

            SetRule("MinEstimatedLTV", minLTV);
        }

        /// <summary>
        /// 更新最小转化概率
        /// </summary>
        /// <param name="minProbability">最小转化概率</param>
        public void UpdateMinConversionProbability(decimal minProbability)
        {
            if (minProbability < 0 || minProbability > 1)
                throw new ArgumentException("转化概率必须在0-1之间", nameof(minProbability));

            SetRule("MinConversionProbability", minProbability);
        }

        /// <summary>
        /// 验证价值定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证评分范围
            var scores = new[] { MinEngagementScore, MinLoyaltyScore, MinMonetaryScore, MinPotentialScore, MinOverallScore };
            if (scores.Any(score => score < 0 || score > 100))
                return false;

            // 验证转化概率范围
            if (MinConversionProbability < 0 || MinConversionProbability > 1)
                return false;

            // 验证生命周期价值
            if (MinEstimatedLTV < 0)
                return false;

            // 至少需要配置一个价值条件
            if (!HasAnyValueCondition())
                return false;

            return true;
        }

        /// <summary>
        /// 检查是否配置了任何价值条件
        /// </summary>
        private bool HasAnyValueCondition()
        {
            return MinEngagementScore > 0 ||
                   MinLoyaltyScore > 0 ||
                   MinMonetaryScore > 0 ||
                   MinPotentialScore > 0 ||
                   MinOverallScore > 0 ||
                   MinEstimatedLTV > 0 ||
                   TargetSpendingLevels.Any() ||
                   TargetValueTiers.Any() ||
                   MinConversionProbability > 0 ||
                   HighValueUsersOnly ||
                   ActiveUsersOnly ||
                   LoyalUsersOnly;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var conditions = new List<string>();

            if (MinOverallScore > 0) conditions.Add($"Overall≥{MinOverallScore}");
            if (MinEstimatedLTV > 0) conditions.Add($"LTV≥{MinEstimatedLTV:C}");
            if (TargetValueTiers.Any()) conditions.Add($"ValueTiers:{TargetValueTiers.Count}");
            if (TargetSpendingLevels.Any()) conditions.Add($"SpendingLevels:{TargetSpendingLevels.Count}");
            if (HighValueUsersOnly) conditions.Add("HighValueOnly");
            if (ActiveUsersOnly) conditions.Add("ActiveOnly");
            if (LoyalUsersOnly) conditions.Add("LoyalOnly");

            var details = string.Join(", ", conditions);
            return $"{summary} - {details}";
        }
    }
}