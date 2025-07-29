using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 用户标签定向条件
    /// 实现基于用户标签的定向规则配置
    /// </summary>
    public class UserTagTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "标签定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Tag";

        /// <summary>
        /// 目标标签名称列表
        /// </summary>
        public IReadOnlyList<string> TargetTagNames => GetRule<List<string>>("TargetTagNames") ?? new List<string>();

        /// <summary>
        /// 目标标签类型列表
        /// </summary>
        public IReadOnlyList<string> TargetTagTypes => GetRule<List<string>>("TargetTagTypes") ?? new List<string>();

        /// <summary>
        /// 目标标签分类列表
        /// </summary>
        public IReadOnlyList<string> TargetCategories => GetRule<List<string>>("TargetCategories") ?? new List<string>();

        /// <summary>
        /// 排除的标签名称列表
        /// </summary>
        public IReadOnlyList<string> ExcludedTagNames => GetRule<List<string>>("ExcludedTagNames") ?? new List<string>();

        /// <summary>
        /// 最小置信度阈值 (0-1)
        /// </summary>
        public decimal MinConfidenceThreshold => GetRule("MinConfidenceThreshold", 0.0m);

        /// <summary>
        /// 最小权重阈值
        /// </summary>
        public decimal MinWeightThreshold => GetRule("MinWeightThreshold", 0.0m);

        /// <summary>
        /// 是否包含已过期标签
        /// </summary>
        public bool IncludeExpiredTags => GetRule("IncludeExpiredTags", false);

        /// <summary>
        /// 标签匹配模式（Any=任意匹配, All=全部匹配）
        /// </summary>
        public string MatchMode => GetRule("MatchMode", "Any");

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserTagTargeting(
            IList<string>? targetTagNames = null,
            IList<string>? targetTagTypes = null,
            IList<string>? targetCategories = null,
            IList<string>? excludedTagNames = null,
            decimal minConfidenceThreshold = 0.0m,
            decimal minWeightThreshold = 0.0m,
            bool includeExpiredTags = false,
            string matchMode = "Any",
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetTagNames, targetTagTypes, targetCategories, excludedTagNames, minConfidenceThreshold, minWeightThreshold, includeExpiredTags, matchMode), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则集合
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<string>? targetTagNames,
            IList<string>? targetTagTypes,
            IList<string>? targetCategories,
            IList<string>? excludedTagNames,
            decimal minConfidenceThreshold,
            decimal minWeightThreshold,
            bool includeExpiredTags,
            string matchMode)
        {
            var rules = new List<TargetingRule>();

            if (targetTagNames != null && targetTagNames.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTagNames",
                    System.Text.Json.JsonSerializer.Serialize(targetTagNames),
                    "Json",
                    "Tag",
                    true,
                    1.0m,
                    "in",
                    "目标标签名称"));
            }

            if (targetTagTypes != null && targetTagTypes.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTagTypes",
                    System.Text.Json.JsonSerializer.Serialize(targetTagTypes),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "in",
                    "目标标签类型"));
            }

            if (targetCategories != null && targetCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetCategories",
                    System.Text.Json.JsonSerializer.Serialize(targetCategories),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "in",
                    "目标标签分类"));
            }

            if (excludedTagNames != null && excludedTagNames.Any())
            {
                rules.Add(new TargetingRule(
                    "ExcludedTagNames",
                    System.Text.Json.JsonSerializer.Serialize(excludedTagNames),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "not_in",
                    "排除的标签名称"));
            }

            if (minConfidenceThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConfidenceThreshold",
                    minConfidenceThreshold.ToString(),
                    "Decimal",
                    "Tag",
                    false,
                    1.0m,
                    "gte",
                    "最小置信度阈值"));
            }

            if (minWeightThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinWeightThreshold",
                    minWeightThreshold.ToString(),
                    "Decimal",
                    "Tag",
                    false,
                    1.0m,
                    "gte",
                    "最小权重阈值"));
            }

            rules.Add(new TargetingRule(
                "IncludeExpiredTags",
                includeExpiredTags.ToString(),
                "Boolean",
                "Tag",
                false,
                1.0m,
                "eq",
                "是否包含已过期标签"));

            rules.Add(new TargetingRule(
                "MatchMode",
                matchMode,
                "String",
                "Tag",
                false,
                1.0m,
                "eq",
                "标签匹配模式"));

            return rules;
        }

        /// <summary>
        /// 创建标签定向条件
        /// </summary>
        public static UserTagTargeting Create(
            IList<string>? targetTagNames = null,
            IList<string>? targetTagTypes = null,
            IList<string>? targetCategories = null,
            IList<string>? excludedTagNames = null,
            decimal minConfidenceThreshold = 0.0m,
            decimal minWeightThreshold = 0.0m,
            bool includeExpiredTags = false,
            string matchMode = "Any",
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserTagTargeting(targetTagNames, targetTagTypes, targetCategories, excludedTagNames, minConfidenceThreshold, minWeightThreshold, includeExpiredTags, matchMode, weight, isEnabled);
        }

        /// <summary>
        /// 创建基于标签名称的定向
        /// </summary>
        public static UserTagTargeting CreateByTagNames(
            IList<string> tagNames,
            decimal minConfidenceThreshold = 0.5m,
            string matchMode = "Any",
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagNames: tagNames,
                minConfidenceThreshold: minConfidenceThreshold,
                matchMode: matchMode,
                weight: weight);
        }

        /// <summary>
        /// 创建基于标签类型的定向
        /// </summary>
        public static UserTagTargeting CreateByTagTypes(
            IList<string> tagTypes,
            decimal minConfidenceThreshold = 0.3m,
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagTypes: tagTypes,
                minConfidenceThreshold: minConfidenceThreshold,
                weight: weight);
        }

        /// <summary>
        /// 创建排除性标签定向
        /// </summary>
        public static UserTagTargeting CreateExclusive(
            IList<string> includeTags,
            IList<string> excludeTags,
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagNames: includeTags,
                excludedTagNames: excludeTags,
                weight: weight);
        }

        /// <summary>
        /// 添加目标标签名称
        /// </summary>
        /// <param name="tagName">标签名称</param>
        public void AddTargetTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("标签名称不能为空", nameof(tagName));

            var currentList = TargetTagNames.ToList();
            if (!currentList.Contains(tagName))
            {
                currentList.Add(tagName);
                SetRule("TargetTagNames", currentList);
            }
        }

        /// <summary>
        /// 添加目标标签类型
        /// </summary>
        /// <param name="tagType">标签类型</param>
        public void AddTargetTagType(string tagType)
        {
            if (string.IsNullOrEmpty(tagType))
                throw new ArgumentException("标签类型不能为空", nameof(tagType));

            var currentList = TargetTagTypes.ToList();
            if (!currentList.Contains(tagType))
            {
                currentList.Add(tagType);
                SetRule("TargetTagTypes", currentList);
            }
        }

        /// <summary>
        /// 添加排除的标签名称
        /// </summary>
        /// <param name="tagName">要排除的标签名称</param>
        public void AddExcludedTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("标签名称不能为空", nameof(tagName));

            var currentList = ExcludedTagNames.ToList();
            if (!currentList.Contains(tagName))
            {
                currentList.Add(tagName);
                SetRule("ExcludedTagNames", currentList);
            }
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
        /// 更新权重阈值
        /// </summary>
        /// <param name="threshold">新的权重阈值</param>
        public void UpdateWeightThreshold(decimal threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("权重阈值不能为负数", nameof(threshold));

            SetRule("MinWeightThreshold", threshold);
        }

        /// <summary>
        /// 设置匹配模式
        /// </summary>
        /// <param name="mode">匹配模式（Any/All）</param>
        public void SetMatchMode(string mode)
        {
            if (string.IsNullOrEmpty(mode) || (mode != "Any" && mode != "All"))
                throw new ArgumentException("匹配模式必须是 'Any' 或 'All'", nameof(mode));

            SetRule("MatchMode", mode);
        }

        /// <summary>
        /// 验证标签定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 至少需要配置一个目标标签条件
            if (!TargetTagNames.Any() && !TargetTagTypes.Any() && !TargetCategories.Any())
                return false;

            // 验证标签名称、类型和分类不能为空字符串
            if (TargetTagNames.Any(string.IsNullOrWhiteSpace) || 
                TargetTagTypes.Any(string.IsNullOrWhiteSpace) ||
                TargetCategories.Any(string.IsNullOrWhiteSpace) ||
                ExcludedTagNames.Any(string.IsNullOrWhiteSpace))
                return false;

            // 验证阈值范围
            if (MinConfidenceThreshold < 0 || MinConfidenceThreshold > 1)
                return false;

            if (MinWeightThreshold < 0)
                return false;

            // 验证匹配模式
            if (MatchMode != "Any" && MatchMode != "All")
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"TagNames: {TargetTagNames.Count}, TagTypes: {TargetTagTypes.Count}, Categories: {TargetCategories.Count}, Excluded: {ExcludedTagNames.Count}, Mode: {MatchMode}";
            return $"{summary} - {details}";
        }
    }
}