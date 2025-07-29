using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 用户兴趣定向条件
    /// 实现基于用户兴趣的定向规则配置
    /// </summary>
    public class UserInterestTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "兴趣定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Interest";

        /// <summary>
        /// 目标兴趣类别
        /// </summary>
        public IReadOnlyList<string> TargetCategories => GetRule<List<string>>("TargetCategories") ?? new List<string>();

        /// <summary>
        /// 目标兴趣标签
        /// </summary>
        public IReadOnlyList<string> TargetTags => GetRule<List<string>>("TargetTags") ?? new List<string>();

        /// <summary>
        /// 最小兴趣分数阈值 (0-1)
        /// </summary>
        public decimal MinScoreThreshold => GetRule("MinScoreThreshold", 0.0m);

        /// <summary>
        /// 最小置信度阈值 (0-1)
        /// </summary>
        public decimal MinConfidenceThreshold => GetRule("MinConfidenceThreshold", 0.0m);

        /// <summary>
        /// 子类别匹配模式（是否包含子类别）
        /// </summary>
        public bool IncludeSubCategories => GetRule("IncludeSubCategories", true);

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserInterestTargeting(
            IList<string>? targetCategories = null,
            IList<string>? targetTags = null,
            decimal minScoreThreshold = 0.0m,
            decimal minConfidenceThreshold = 0.0m,
            bool includeSubCategories = true,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetCategories, targetTags, minScoreThreshold, minConfidenceThreshold, includeSubCategories), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则集合
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<string>? targetCategories,
            IList<string>? targetTags,
            decimal minScoreThreshold,
            decimal minConfidenceThreshold,
            bool includeSubCategories)
        {
            var rules = new List<TargetingRule>();

            if (targetCategories != null && targetCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetCategories",
                    System.Text.Json.JsonSerializer.Serialize(targetCategories),
                    "Json",
                    "Interest",
                    true,
                    1.0m,
                    "in",
                    "目标兴趣类别"));
            }

            if (targetTags != null && targetTags.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTags",
                    System.Text.Json.JsonSerializer.Serialize(targetTags),
                    "Json",
                    "Interest",
                    false,
                    1.0m,
                    "in",
                    "目标兴趣标签"));
            }

            if (minScoreThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinScoreThreshold",
                    minScoreThreshold.ToString(),
                    "Decimal",
                    "Interest",
                    false,
                    1.0m,
                    "gte",
                    "最小兴趣分数阈值"));
            }

            if (minConfidenceThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConfidenceThreshold",
                    minConfidenceThreshold.ToString(),
                    "Decimal",
                    "Interest",
                    false,
                    1.0m,
                    "gte",
                    "最小置信度阈值"));
            }

            rules.Add(new TargetingRule(
                "IncludeSubCategories",
                includeSubCategories.ToString(),
                "Boolean",
                "Interest",
                false,
                1.0m,
                "eq",
                "是否包含子类别"));

            return rules;
        }

        /// <summary>
        /// 创建兴趣定向条件
        /// </summary>
        public static UserInterestTargeting Create(
            IList<string>? targetCategories = null,
            IList<string>? targetTags = null,
            decimal minScoreThreshold = 0.0m,
            decimal minConfidenceThreshold = 0.0m,
            bool includeSubCategories = true,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserInterestTargeting(targetCategories, targetTags, minScoreThreshold, minConfidenceThreshold, includeSubCategories, weight, isEnabled);
        }

        /// <summary>
        /// 创建基于类别的兴趣定向
        /// </summary>
        public static UserInterestTargeting CreateByCategories(
            IList<string> categories,
            decimal minScoreThreshold = 0.3m,
            bool includeSubCategories = true,
            decimal weight = 1.0m)
        {
            return new UserInterestTargeting(
                targetCategories: categories,
                minScoreThreshold: minScoreThreshold,
                includeSubCategories: includeSubCategories,
                weight: weight);
        }

        /// <summary>
        /// 创建基于标签的兴趣定向
        /// </summary>
        public static UserInterestTargeting CreateByTags(
            IList<string> tags,
            decimal minConfidenceThreshold = 0.5m,
            decimal weight = 1.0m)
        {
            return new UserInterestTargeting(
                targetTags: tags,
                minConfidenceThreshold: minConfidenceThreshold,
                weight: weight);
        }

        /// <summary>
        /// 添加兴趣类别
        /// </summary>
        /// <param name="category">兴趣类别</param>
        public void AddTargetCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("兴趣类别不能为空", nameof(category));

            var currentList = TargetCategories.ToList();
            if (!currentList.Contains(category))
            {
                currentList.Add(category);
                SetRule("TargetCategories", currentList);
            }
        }

        /// <summary>
        /// 添加兴趣标签
        /// </summary>
        /// <param name="tag">兴趣标签</param>
        public void AddTargetTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("兴趣标签不能为空", nameof(tag));

            var currentList = TargetTags.ToList();
            if (!currentList.Contains(tag))
            {
                currentList.Add(tag);
                SetRule("TargetTags", currentList);
            }
        }

        /// <summary>
        /// 更新分数阈值
        /// </summary>
        /// <param name="threshold">新的分数阈值</param>
        public void UpdateScoreThreshold(decimal threshold)
        {
            if (threshold < 0 || threshold > 1)
                throw new ArgumentException("分数阈值必须在0-1之间", nameof(threshold));

            SetRule("MinScoreThreshold", threshold);
        }

        /// <summary>
        /// 更新置信度阈值
        /// </summary>
        /// <param name="threshold">新的置信度阈值</param>
        public void UpdateConfidenceThreshold(decimal threshold)
        {
            if (threshold < 0 || threshold > 1)
                throw new ArgumentException("置信度阈值必须在0-1之间", nameof(threshold));

            SetRule("MinConfidenceThreshold", threshold);
        }

        /// <summary>
        /// 验证兴趣定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 至少需要配置一个目标类别或标签
            if (!TargetCategories.Any() && !TargetTags.Any())
                return false;

            // 验证类别和标签不能为空字符串
            if (TargetCategories.Any(string.IsNullOrWhiteSpace) || TargetTags.Any(string.IsNullOrWhiteSpace))
                return false;

            // 验证阈值范围
            if (MinScoreThreshold < 0 || MinScoreThreshold > 1)
                return false;

            if (MinConfidenceThreshold < 0 || MinConfidenceThreshold > 1)
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"Categories: {TargetCategories.Count}, Tags: {TargetTags.Count}, MinScore: {MinScoreThreshold:F2}, MinConfidence: {MinConfidenceThreshold:F2}";
            return $"{summary} - {details}";
        }
    }
}