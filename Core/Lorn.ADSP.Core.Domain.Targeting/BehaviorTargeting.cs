using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 行为定向条件
    /// 实现基于用户行为的定向规则配置
    /// </summary>
    public class BehaviorTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "行为定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Behavior";

        /// <summary>
        /// 目标兴趣标签
        /// </summary>
        public IReadOnlyList<string> InterestTags => GetRule<List<string>>("InterestTags") ?? new List<string>();

        /// <summary>
        /// 目标行为类型
        /// </summary>
        public IReadOnlyList<string> BehaviorTypes => GetRule<List<string>>("BehaviorTypes") ?? new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorTargeting(
            IList<string>? interestTags = null,
            IList<string>? behaviorTypes = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(interestTags, behaviorTypes), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则集合
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<string>? interestTags,
            IList<string>? behaviorTypes)
        {
            var rules = new List<TargetingRule>();

            if (interestTags != null && interestTags.Any())
            {
                rules.Add(new TargetingRule(
                    "InterestTags",
                    System.Text.Json.JsonSerializer.Serialize(interestTags),
                    "Json",
                    "Behavior",
                    true,
                    1.0m,
                    "in",
                    "用户兴趣标签"));
            }

            if (behaviorTypes != null && behaviorTypes.Any())
            {
                rules.Add(new TargetingRule(
                    "BehaviorTypes",
                    System.Text.Json.JsonSerializer.Serialize(behaviorTypes),
                    "Json",
                    "Behavior",
                    true,
                    1.0m,
                    "in",
                    "用户行为类型"));
            }

            return rules;
        }

        /// <summary>
        /// 创建行为定向条件
        /// </summary>
        public static BehaviorTargeting Create(
            IList<string>? interestTags = null,
            IList<string>? behaviorTypes = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new BehaviorTargeting(interestTags, behaviorTypes, weight, isEnabled);
        }

        /// <summary>
        /// 添加兴趣标签
        /// </summary>
        /// <param name="tag">兴趣标签</param>
        public void AddInterestTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("兴趣标签不能为空", nameof(tag));

            var currentList = InterestTags.ToList();
            if (!currentList.Contains(tag))
            {
                currentList.Add(tag);
                SetRule("InterestTags", currentList);
            }
        }

        /// <summary>
        /// 添加行为类型
        /// </summary>
        /// <param name="behaviorType">行为类型</param>
        public void AddBehaviorType(string behaviorType)
        {
            if (string.IsNullOrEmpty(behaviorType))
                throw new ArgumentException("行为类型不能为空", nameof(behaviorType));

            var currentList = BehaviorTypes.ToList();
            if (!currentList.Contains(behaviorType))
            {
                currentList.Add(behaviorType);
                SetRule("BehaviorTypes", currentList);
            }
        }

        /// <summary>
        /// 验证行为定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 至少需要配置一个兴趣标签或行为类型
            if (!InterestTags.Any() && !BehaviorTypes.Any())
                return false;

            // 验证兴趣标签和行为类型不能为空字符串
            if (InterestTags.Any(string.IsNullOrWhiteSpace) || BehaviorTypes.Any(string.IsNullOrWhiteSpace))
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"InterestTags: {InterestTags.Count}, BehaviorTypes: {BehaviorTypes.Count}";
            return $"{summary} - {details}";
        }
    }
}
