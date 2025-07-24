using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Targeting;

/// <summary>
/// 用户偏好定向上下文
/// 继承自TargetingContextBase，提供用户偏好数据的定向上下文功能
/// 专注于广告投放相关的用户偏好设置和隐私控制
/// </summary>
public class UserPreference : TargetingContextBase
{
    /// <summary>
    /// 上下文名称
    /// </summary>
    public override string ContextName => "用户偏好上下文";

    /// <summary>
    /// 每日广告曝光次数限制
    /// </summary>
    public int? MaxDailyAdImpressions => GetPropertyValue<int?>("MaxDailyAdImpressions");

    /// <summary>
    /// 偏好的广告类型
    /// </summary>
    public IReadOnlyList<AdType> PreferredAdTypes => GetPropertyValue<List<AdType>>("PreferredAdTypes") ?? new List<AdType>();

    /// <summary>
    /// 屏蔽的广告类别
    /// </summary>
    public IReadOnlyList<string> BlockedCategories => GetPropertyValue<List<string>>("BlockedCategories") ?? new List<string>();

    /// <summary>
    /// 偏好的内容主题
    /// </summary>
    public IReadOnlyList<string> PreferredTopics => GetPropertyValue<List<string>>("PreferredTopics") ?? new List<string>();

    /// <summary>
    /// 是否允许个性化广告
    /// </summary>
    public bool AllowPersonalizedAds => GetPropertyValue("AllowPersonalizedAds", true);

    /// <summary>
    /// 是否允许行为追踪
    /// </summary>
    public bool AllowBehaviorTracking => GetPropertyValue("AllowBehaviorTracking", true);

    /// <summary>
    /// 是否允许跨设备追踪
    /// </summary>
    public bool AllowCrossDeviceTracking => GetPropertyValue("AllowCrossDeviceTracking", true);

    /// <summary>
    /// 偏好的广告投放时段
    /// </summary>
    public IReadOnlyList<TimeWindow> PreferredTimeSlots => GetPropertyValue<List<TimeWindow>>("PreferredTimeSlots") ?? new List<TimeWindow>();

    /// <summary>
    /// 隐私级别
    /// </summary>
    public PrivacyLevel PrivacyLevel => GetPropertyValue("PrivacyLevel", PrivacyLevel.Standard);

    /// <summary>
    /// 偏好的语言列表
    /// </summary>
    public IReadOnlyList<string> PreferredLanguages => GetPropertyValue<List<string>>("PreferredLanguages") ?? new List<string>();

    /// <summary>
    /// 内容成熟度级别偏好
    /// </summary>
    public ContentMaturityLevel ContentMaturityLevel => GetPropertyValue("ContentMaturityLevel", ContentMaturityLevel.General);

    /// <summary>
    /// 自定义偏好设置
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomPreferences => GetPropertyValue<Dictionary<string, object>>("CustomPreferences") ?? new Dictionary<string, object>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserPreference() : base("UserPreference") { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserPreference(
        int? maxDailyAdImpressions = null,
        IList<AdType>? preferredAdTypes = null,
        IList<string>? blockedCategories = null,
        IList<string>? preferredTopics = null,
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        bool allowCrossDeviceTracking = true,
        IList<TimeWindow>? preferredTimeSlots = null,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard,
        IList<string>? preferredLanguages = null,
        ContentMaturityLevel contentMaturityLevel = ContentMaturityLevel.General,
        IDictionary<string, object>? customPreferences = null,
        string? dataSource = null)
        : base("UserPreference", CreateProperties(maxDailyAdImpressions, preferredAdTypes, blockedCategories, preferredTopics, allowPersonalizedAds, allowBehaviorTracking, allowCrossDeviceTracking, preferredTimeSlots, privacyLevel, preferredLanguages, contentMaturityLevel, customPreferences), dataSource)
    {
        ValidateMaxDailyImpressions(maxDailyAdImpressions);
    }

    /// <summary>
    /// 创建属性字典
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        int? maxDailyAdImpressions,
        IList<AdType>? preferredAdTypes,
        IList<string>? blockedCategories,
        IList<string>? preferredTopics,
        bool allowPersonalizedAds,
        bool allowBehaviorTracking,
        bool allowCrossDeviceTracking,
        IList<TimeWindow>? preferredTimeSlots,
        PrivacyLevel privacyLevel,
        IList<string>? preferredLanguages,
        ContentMaturityLevel contentMaturityLevel,
        IDictionary<string, object>? customPreferences)
    {
        var properties = new Dictionary<string, object>
        {
            ["AllowPersonalizedAds"] = allowPersonalizedAds,
            ["AllowBehaviorTracking"] = allowBehaviorTracking,
            ["AllowCrossDeviceTracking"] = allowCrossDeviceTracking,
            ["PrivacyLevel"] = privacyLevel,
            ["ContentMaturityLevel"] = contentMaturityLevel
        };

        if (maxDailyAdImpressions.HasValue)
            properties["MaxDailyAdImpressions"] = maxDailyAdImpressions.Value;

        if (preferredAdTypes != null && preferredAdTypes.Any())
            properties["PreferredAdTypes"] = preferredAdTypes.ToList();

        if (blockedCategories != null && blockedCategories.Any())
            properties["BlockedCategories"] = blockedCategories.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        if (preferredTopics != null && preferredTopics.Any())
            properties["PreferredTopics"] = preferredTopics.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

        if (preferredTimeSlots != null && preferredTimeSlots.Any())
            properties["PreferredTimeSlots"] = preferredTimeSlots.ToList();

        if (preferredLanguages != null && preferredLanguages.Any())
            properties["PreferredLanguages"] = preferredLanguages.ToList();

        if (customPreferences != null && customPreferences.Any())
            properties["CustomPreferences"] = customPreferences.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return properties;
    }

    /// <summary>
    /// 创建默认偏好上下文
    /// </summary>
    public static UserPreference CreateDefault(string? dataSource = null)
    {
        return new UserPreference(dataSource: dataSource);
    }

    /// <summary>
    /// 创建基础偏好上下文
    /// </summary>
    public static UserPreference CreateBasic(
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard,
        string? dataSource = null)
    {
        return new UserPreference(
            allowPersonalizedAds: allowPersonalizedAds,
            allowBehaviorTracking: allowBehaviorTracking,
            privacyLevel: privacyLevel,
            dataSource: dataSource);
    }

    /// <summary>
    /// 创建严格隐私偏好上下文
    /// </summary>
    public static UserPreference CreateStrictPrivacy(string? dataSource = null)
    {
        return new UserPreference(
            allowPersonalizedAds: false,
            allowBehaviorTracking: false,
            allowCrossDeviceTracking: false,
            privacyLevel: PrivacyLevel.High,
            dataSource: dataSource);
    }

    /// <summary>
    /// 是否偏好指定广告类型
    /// </summary>
    public bool PrefersAdType(AdType adType)
    {
        return PreferredAdTypes.Contains(adType);
    }

    /// <summary>
    /// 是否屏蔽指定类别
    /// </summary>
    public bool IsBlockedCategory(string category)
    {
        return BlockedCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否偏好指定主题
    /// </summary>
    public bool PrefersTopicCategory(string topic)
    {
        return PreferredTopics.Contains(topic, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否在偏好时段内
    /// </summary>
    public bool IsInPreferredTimeSlot(DateTime timestamp)
    {
        if (!PreferredTimeSlots.Any())
            return true; // 没有设置偏好时段时，默认所有时间都合适

        return PreferredTimeSlots.Any(slot => slot.ContainsTime(timestamp));
    }

    /// <summary>
    /// 获取自定义偏好值
    /// </summary>
    public T? GetCustomPreference<T>(string key)
    {
        if (CustomPreferences.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// 是否允许数据使用
    /// </summary>
    public bool AllowsDataUsage(DataUsageType usageType)
    {
        return usageType switch
        {
            DataUsageType.Personalization => AllowPersonalizedAds,
            DataUsageType.BehaviorTracking => AllowBehaviorTracking,
            DataUsageType.CrossDeviceTracking => AllowCrossDeviceTracking,
            _ => false
        };
    }

    /// <summary>
    /// 获取广告类型偏好评分
    /// </summary>
    public decimal GetAdTypePreferenceScore(AdType adType)
    {
        if (!PreferredAdTypes.Any())
            return 1.0m; // 没有偏好设置时默认接受所有类型

        return PrefersAdType(adType) ? 1.0m : 0.0m;
    }

    /// <summary>
    /// 获取内容主题匹配度
    /// </summary>
    public decimal GetTopicMatchScore(IEnumerable<string> targetTopics)
    {
        if (!targetTopics.Any() || !PreferredTopics.Any())
            return 1.0m;

        var matchCount = targetTopics.Count(topic => PrefersTopicCategory(topic));
        return (decimal)matchCount / targetTopics.Count();
    }

    /// <summary>
    /// 验证每日广告曝光次数
    /// </summary>
    private static void ValidateMaxDailyImpressions(int? maxDailyAdImpressions)
    {
        if (maxDailyAdImpressions.HasValue && (maxDailyAdImpressions.Value < 0 || maxDailyAdImpressions.Value > 1000))
            throw new ArgumentOutOfRangeException(nameof(maxDailyAdImpressions), "每日最大广告曝光次数必须在0-1000之间");
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var preferenceInfo = $"Privacy:{PrivacyLevel} Personalized:{AllowPersonalizedAds} AdTypes:{PreferredAdTypes.Count} Blocked:{BlockedCategories.Count}";
        return $"{baseInfo} | {preferenceInfo}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // 验证每日曝光次数限制
        if (MaxDailyAdImpressions.HasValue && (MaxDailyAdImpressions.Value < 0 || MaxDailyAdImpressions.Value > 1000))
            return false;

        return true;
    }
}

/// <summary>
/// 隐私级别枚举
/// </summary>
public enum PrivacyLevel
{
    Low = 1,
    Standard = 2,
    High = 3
}

/// <summary>
/// 内容成熟度级别枚举
/// </summary>
public enum ContentMaturityLevel
{
    General = 1,
    Teen = 2,
    Mature = 3,
    Adult = 4
}

/// <summary>
/// 数据使用类型枚举
/// </summary>
public enum DataUsageType
{
    Personalization = 1,
    BehaviorTracking = 2,
    CrossDeviceTracking = 3
}