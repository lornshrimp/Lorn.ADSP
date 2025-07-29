using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 用户偏好定向条件
    /// 实现基于用户偏好和隐私设置的定向规则配置
    /// </summary>
    public class UserPreferenceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "偏好定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Preference";

        /// <summary>
        /// 目标广告类型
        /// </summary>
        public IReadOnlyList<AdType> TargetAdTypes => GetRule<List<AdType>>("TargetAdTypes") ?? new List<AdType>();

        /// <summary>
        /// 排除的内容类别
        /// </summary>
        public IReadOnlyList<string> ExcludedCategories => GetRule<List<string>>("ExcludedCategories") ?? new List<string>();

        /// <summary>
        /// 目标话题
        /// </summary>
        public IReadOnlyList<string> TargetTopics => GetRule<List<string>>("TargetTopics") ?? new List<string>();

        /// <summary>
        /// 目标语言
        /// </summary>
        public IReadOnlyList<string> TargetLanguages => GetRule<List<string>>("TargetLanguages") ?? new List<string>();

        /// <summary>
        /// 最大隐私级别要求
        /// </summary>
        public PrivacyLevel MaxPrivacyLevel => GetRule("MaxPrivacyLevel", PrivacyLevel.Maximum);

        /// <summary>
        /// 最大内容成熟度级别
        /// </summary>
        public ContentMaturityLevel MaxContentMaturityLevel => GetRule("MaxContentMaturityLevel", ContentMaturityLevel.Restricted);

        /// <summary>
        /// 最大每日广告展示次数
        /// </summary>
        public int? MaxDailyImpressions => GetRule<int?>("MaxDailyImpressions");

        /// <summary>
        /// 是否要求允许个性化广告
        /// </summary>
        public bool RequirePersonalizedAdsConsent => GetRule("RequirePersonalizedAdsConsent", true);

        /// <summary>
        /// 是否要求允许行为追踪
        /// </summary>
        public bool RequireBehaviorTrackingConsent => GetRule("RequireBehaviorTrackingConsent", false);

        /// <summary>
        /// 是否要求允许跨设备追踪
        /// </summary>
        public bool RequireCrossDeviceTrackingConsent => GetRule("RequireCrossDeviceTrackingConsent", false);

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserPreferenceTargeting(
            IList<AdType>? targetAdTypes = null,
            IList<string>? excludedCategories = null,
            IList<string>? targetTopics = null,
            IList<string>? targetLanguages = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Maximum,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.Restricted,
            int? maxDailyImpressions = null,
            bool requirePersonalizedAdsConsent = true,
            bool requireBehaviorTrackingConsent = false,
            bool requireCrossDeviceTrackingConsent = false,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetAdTypes, excludedCategories, targetTopics, targetLanguages, maxPrivacyLevel, maxContentMaturityLevel, maxDailyImpressions, requirePersonalizedAdsConsent, requireBehaviorTrackingConsent, requireCrossDeviceTrackingConsent), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则集合
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<AdType>? targetAdTypes,
            IList<string>? excludedCategories,
            IList<string>? targetTopics,
            IList<string>? targetLanguages,
            PrivacyLevel maxPrivacyLevel,
            ContentMaturityLevel maxContentMaturityLevel,
            int? maxDailyImpressions,
            bool requirePersonalizedAdsConsent,
            bool requireBehaviorTrackingConsent,
            bool requireCrossDeviceTrackingConsent)
        {
            var rules = new List<TargetingRule>();

            if (targetAdTypes != null && targetAdTypes.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetAdTypes",
                    System.Text.Json.JsonSerializer.Serialize(targetAdTypes.Select(t => (int)t)),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "目标广告类型"));
            }

            if (excludedCategories != null && excludedCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "ExcludedCategories",
                    System.Text.Json.JsonSerializer.Serialize(excludedCategories),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "not_in",
                    "排除的内容类别"));
            }

            if (targetTopics != null && targetTopics.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTopics",
                    System.Text.Json.JsonSerializer.Serialize(targetTopics),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "目标话题"));
            }

            if (targetLanguages != null && targetLanguages.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetLanguages",
                    System.Text.Json.JsonSerializer.Serialize(targetLanguages),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "目标语言"));
            }

            rules.Add(new TargetingRule(
                "MaxPrivacyLevel",
                ((int)maxPrivacyLevel).ToString(),
                "Integer",
                "Preference",
                true,
                1.0m,
                "lte",
                "最大隐私级别"));

            rules.Add(new TargetingRule(
                "MaxContentMaturityLevel",
                ((int)maxContentMaturityLevel).ToString(),
                "Integer",
                "Preference",
                false,
                1.0m,
                "lte",
                "最大内容成熟度级别"));

            if (maxDailyImpressions.HasValue)
            {
                rules.Add(new TargetingRule(
                    "MaxDailyImpressions",
                    maxDailyImpressions.Value.ToString(),
                    "Integer",
                    "Preference",
                    false,
                    1.0m,
                    "lte",
                    "最大每日广告展示次数"));
            }

            rules.Add(new TargetingRule(
                "RequirePersonalizedAdsConsent",
                requirePersonalizedAdsConsent.ToString(),
                "Boolean",
                "Preference",
                true,
                1.0m,
                "eq",
                "是否要求个性化广告同意"));

            if (requireBehaviorTrackingConsent)
            {
                rules.Add(new TargetingRule(
                    "RequireBehaviorTrackingConsent",
                    requireBehaviorTrackingConsent.ToString(),
                    "Boolean",
                    "Preference",
                    true,
                    1.0m,
                    "eq",
                    "是否要求行为追踪同意"));
            }

            if (requireCrossDeviceTrackingConsent)
            {
                rules.Add(new TargetingRule(
                    "RequireCrossDeviceTrackingConsent",
                    requireCrossDeviceTrackingConsent.ToString(),
                    "Boolean",
                    "Preference",
                    true,
                    1.0m,
                    "eq",
                    "是否要求跨设备追踪同意"));
            }

            return rules;
        }

        /// <summary>
        /// 创建偏好定向条件
        /// </summary>
        public static UserPreferenceTargeting Create(
            IList<AdType>? targetAdTypes = null,
            IList<string>? excludedCategories = null,
            IList<string>? targetTopics = null,
            IList<string>? targetLanguages = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Maximum,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.Restricted,
            int? maxDailyImpressions = null,
            bool requirePersonalizedAdsConsent = true,
            bool requireBehaviorTrackingConsent = false,
            bool requireCrossDeviceTrackingConsent = false,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserPreferenceTargeting(targetAdTypes, excludedCategories, targetTopics, targetLanguages, maxPrivacyLevel, maxContentMaturityLevel, maxDailyImpressions, requirePersonalizedAdsConsent, requireBehaviorTrackingConsent, requireCrossDeviceTrackingConsent, weight, isEnabled);
        }

        /// <summary>
        /// 创建隐私友好的偏好定向
        /// </summary>
        public static UserPreferenceTargeting CreatePrivacyFriendly(
            IList<AdType>? targetAdTypes = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Standard,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetAdTypes: targetAdTypes,
                maxPrivacyLevel: maxPrivacyLevel,
                requirePersonalizedAdsConsent: false,
                requireBehaviorTrackingConsent: false,
                requireCrossDeviceTrackingConsent: false,
                weight: weight);
        }

        /// <summary>
        /// 创建基于内容偏好的定向
        /// </summary>
        public static UserPreferenceTargeting CreateContentBased(
            IList<string> targetTopics,
            IList<string>? excludedCategories = null,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.General,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetTopics: targetTopics,
                excludedCategories: excludedCategories,
                maxContentMaturityLevel: maxContentMaturityLevel,
                weight: weight);
        }

        /// <summary>
        /// 创建基于语言的定向
        /// </summary>
        public static UserPreferenceTargeting CreateLanguageBased(
            IList<string> targetLanguages,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetLanguages: targetLanguages,
                weight: weight);
        }

        /// <summary>
        /// 添加目标广告类型
        /// </summary>
        /// <param name="adType">广告类型</param>
        public void AddTargetAdType(AdType adType)
        {
            var currentList = TargetAdTypes.ToList();
            if (!currentList.Contains(adType))
            {
                currentList.Add(adType);
                SetRule("TargetAdTypes", currentList.Select(t => (int)t).ToList());
            }
        }

        /// <summary>
        /// 添加排除的内容类别
        /// </summary>
        /// <param name="category">内容类别</param>
        public void AddExcludedCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("内容类别不能为空", nameof(category));

            var currentList = ExcludedCategories.ToList();
            if (!currentList.Contains(category))
            {
                currentList.Add(category);
                SetRule("ExcludedCategories", currentList);
            }
        }

        /// <summary>
        /// 添加目标话题
        /// </summary>
        /// <param name="topic">话题</param>
        public void AddTargetTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("话题不能为空", nameof(topic));

            var currentList = TargetTopics.ToList();
            if (!currentList.Contains(topic))
            {
                currentList.Add(topic);
                SetRule("TargetTopics", currentList);
            }
        }

        /// <summary>
        /// 添加目标语言
        /// </summary>
        /// <param name="language">语言</param>
        public void AddTargetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("语言不能为空", nameof(language));

            var currentList = TargetLanguages.ToList();
            if (!currentList.Contains(language))
            {
                currentList.Add(language);
                SetRule("TargetLanguages", currentList);
            }
        }

        /// <summary>
        /// 更新隐私级别要求
        /// </summary>
        /// <param name="privacyLevel">隐私级别</param>
        public void UpdatePrivacyLevel(PrivacyLevel privacyLevel)
        {
            SetRule("MaxPrivacyLevel", (int)privacyLevel);
        }

        /// <summary>
        /// 更新内容成熟度级别要求
        /// </summary>
        /// <param name="maturityLevel">内容成熟度级别</param>
        public void UpdateContentMaturityLevel(ContentMaturityLevel maturityLevel)
        {
            SetRule("MaxContentMaturityLevel", (int)maturityLevel);
        }

        /// <summary>
        /// 设置每日展示次数限制
        /// </summary>
        /// <param name="maxImpressions">最大展示次数</param>
        public void SetDailyImpressionsLimit(int? maxImpressions)
        {
            if (maxImpressions.HasValue && maxImpressions.Value < 0)
                throw new ArgumentException("每日展示次数不能为负数", nameof(maxImpressions));

            if (maxImpressions.HasValue)
                SetRule("MaxDailyImpressions", maxImpressions.Value);
            else
                RemoveRule("MaxDailyImpressions");
        }

        /// <summary>
        /// 验证偏好定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证字符串列表不能包含空值
            if (ExcludedCategories.Any(string.IsNullOrWhiteSpace) ||
                TargetTopics.Any(string.IsNullOrWhiteSpace) ||
                TargetLanguages.Any(string.IsNullOrWhiteSpace))
                return false;

            // 验证每日展示次数限制
            if (MaxDailyImpressions.HasValue && MaxDailyImpressions.Value < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"AdTypes: {TargetAdTypes.Count}, Topics: {TargetTopics.Count}, Languages: {TargetLanguages.Count}, Privacy: {MaxPrivacyLevel}, Content: {MaxContentMaturityLevel}";
            return $"{summary} - {details}";
        }
    }
}