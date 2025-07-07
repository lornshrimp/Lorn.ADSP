using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 用户偏好设置值对象
/// </summary>
public class UserPreferences : ValueObject
{
    /// <summary>
    /// 广告频次控制（每日最大展示次数）
    /// </summary>
    public int? MaxDailyAdImpressions { get; private set; }

    /// <summary>
    /// 偏好的广告类型
    /// </summary>
    public IReadOnlyList<AdType> PreferredAdTypes { get; private set; } = new List<AdType>();

    /// <summary>
    /// 不感兴趣的广告类别
    /// </summary>
    public IReadOnlyList<string> BlockedCategories { get; private set; } = new List<string>();

    /// <summary>
    /// 偏好的内容主题
    /// </summary>
    public IReadOnlyList<string> PreferredTopics { get; private set; } = new List<string>();

    /// <summary>
    /// 是否允许个性化广告
    /// </summary>
    public bool AllowPersonalizedAds { get; private set; } = true;

    /// <summary>
    /// 是否允许行为追踪
    /// </summary>
    public bool AllowBehaviorTracking { get; private set; } = true;

    /// <summary>
    /// 是否允许跨设备追踪
    /// </summary>
    public bool AllowCrossDeviceTracking { get; private set; } = true;

    /// <summary>
    /// 偏好的广告投放时间段
    /// </summary>
    public IReadOnlyList<TimeWindow> PreferredTimeSlots { get; private set; } = new List<TimeWindow>();

    /// <summary>
    /// 通知偏好设置
    /// </summary>
    public NotificationPreferences? NotificationPreferences { get; private set; }

    /// <summary>
    /// 内容偏好设置
    /// </summary>
    public ContentPreferences? ContentPreferences { get; private set; }

    /// <summary>
    /// 隐私级别
    /// </summary>
    public PrivacyLevel PrivacyLevel { get; private set; } = PrivacyLevel.Standard;

    /// <summary>
    /// 自定义偏好设置
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomPreferences { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserPreferences() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserPreferences(
        int? maxDailyAdImpressions = null,
        IList<AdType>? preferredAdTypes = null,
        IList<string>? blockedCategories = null,
        IList<string>? preferredTopics = null,
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        bool allowCrossDeviceTracking = true,
        IList<TimeWindow>? preferredTimeSlots = null,
        NotificationPreferences? notificationPreferences = null,
        ContentPreferences? contentPreferences = null,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard,
        IDictionary<string, object>? customPreferences = null)
    {
        ValidateMaxDailyImpressions(maxDailyAdImpressions);

        MaxDailyAdImpressions = maxDailyAdImpressions;
        PreferredAdTypes = preferredAdTypes?.ToList() ?? new List<AdType>();
        BlockedCategories = blockedCategories?.Where(c => !string.IsNullOrWhiteSpace(c)).ToList() ?? new List<string>();
        PreferredTopics = preferredTopics?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList() ?? new List<string>();
        AllowPersonalizedAds = allowPersonalizedAds;
        AllowBehaviorTracking = allowBehaviorTracking;
        AllowCrossDeviceTracking = allowCrossDeviceTracking;
        PreferredTimeSlots = preferredTimeSlots?.ToList() ?? new List<TimeWindow>();
        NotificationPreferences = notificationPreferences;
        ContentPreferences = contentPreferences;
        PrivacyLevel = privacyLevel;
        CustomPreferences = customPreferences?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// 创建默认偏好设置
    /// </summary>
    public static UserPreferences CreateDefault()
    {
        return new UserPreferences();
    }

    /// <summary>
    /// 创建基本偏好设置
    /// </summary>
    public static UserPreferences CreateBasic(
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard)
    {
        return new UserPreferences(
            allowPersonalizedAds: allowPersonalizedAds,
            allowBehaviorTracking: allowBehaviorTracking,
            privacyLevel: privacyLevel);
    }

    /// <summary>
    /// 创建严格隐私设置
    /// </summary>
    public static UserPreferences CreateStrictPrivacy()
    {
        return new UserPreferences(
            allowPersonalizedAds: false,
            allowBehaviorTracking: false,
            allowCrossDeviceTracking: false,
            privacyLevel: PrivacyLevel.High);
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
    /// 是否在偏好时间段内
    /// </summary>
    public bool IsInPreferredTimeSlot(DateTime timestamp)
    {
        if (!PreferredTimeSlots.Any())
            return true; // 如果没有设置偏好时间段，默认所有时间都可以

        var timeOfDay = timestamp.TimeOfDay;
        return PreferredTimeSlots.Any(slot => IsWithinTimeWindow(timeOfDay, slot));
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
    /// 检查是否在时间窗口内
    /// </summary>
    private static bool IsWithinTimeWindow(TimeSpan timeOfDay, TimeWindow timeWindow)
    {
        // 如果没有设置起始时间，认为时间窗口从当天0点开始
        var startTime = timeWindow.StartTime?.TimeOfDay ?? TimeSpan.Zero;
        var endTime = startTime.Add(timeWindow.Size);

        // 处理跨天的情况
        if (endTime > TimeSpan.FromHours(24))
        {
            endTime = endTime.Subtract(TimeSpan.FromHours(24));
            return timeOfDay >= startTime || timeOfDay <= endTime;
        }

        return timeOfDay >= startTime && timeOfDay <= endTime;
    }

    /// <summary>
    /// 验证每日最大展示次数
    /// </summary>
    private static void ValidateMaxDailyImpressions(int? maxDailyAdImpressions)
    {
        if (maxDailyAdImpressions.HasValue && (maxDailyAdImpressions.Value < 0 || maxDailyAdImpressions.Value > 1000))
            throw new ArgumentOutOfRangeException(nameof(maxDailyAdImpressions), "每日最大广告展示次数必须在0-1000之间");
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MaxDailyAdImpressions ?? 0;
        yield return string.Join(",", PreferredAdTypes.Select(t => t.ToString()));
        yield return string.Join(",", BlockedCategories);
        yield return string.Join(",", PreferredTopics);
        yield return AllowPersonalizedAds;
        yield return AllowBehaviorTracking;
        yield return AllowCrossDeviceTracking;
        yield return NotificationPreferences ?? new object();
        yield return ContentPreferences ?? new object();
        yield return PrivacyLevel;
    }
}





/// <summary>
/// 通知偏好设置
/// </summary>
public class NotificationPreferences : ValueObject
{
    /// <summary>
    /// 是否允许电子邮件通知
    /// </summary>
    public bool AllowEmailNotifications { get; private set; } = true;

    /// <summary>
    /// 是否允许推送通知
    /// </summary>
    public bool AllowPushNotifications { get; private set; } = true;

    /// <summary>
    /// 是否允许短信通知
    /// </summary>
    public bool AllowSmsNotifications { get; private set; } = false;

    /// <summary>
    /// 通知频率
    /// </summary>
    public NotificationFrequency Frequency { get; private set; } = NotificationFrequency.Normal;

    public NotificationPreferences(
        bool allowEmailNotifications = true,
        bool allowPushNotifications = true,
        bool allowSmsNotifications = false,
        NotificationFrequency frequency = NotificationFrequency.Normal)
    {
        AllowEmailNotifications = allowEmailNotifications;
        AllowPushNotifications = allowPushNotifications;
        AllowSmsNotifications = allowSmsNotifications;
        Frequency = frequency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AllowEmailNotifications;
        yield return AllowPushNotifications;
        yield return AllowSmsNotifications;
        yield return Frequency;
    }
}

/// <summary>
/// 内容偏好设置
/// </summary>
public class ContentPreferences : ValueObject
{
    /// <summary>
    /// 偏好的内容语言
    /// </summary>
    public IReadOnlyList<string> PreferredLanguages { get; private set; } = new List<string>();

    /// <summary>
    /// 内容成熟度级别
    /// </summary>
    public ContentMaturityLevel MaturityLevel { get; private set; } = ContentMaturityLevel.General;

    /// <summary>
    /// 偏好的内容格式
    /// </summary>
    public IReadOnlyList<string> PreferredFormats { get; private set; } = new List<string>();

    public ContentPreferences(
        IList<string>? preferredLanguages = null,
        ContentMaturityLevel maturityLevel = ContentMaturityLevel.General,
        IList<string>? preferredFormats = null)
    {
        PreferredLanguages = preferredLanguages?.ToList() ?? new List<string>();
        MaturityLevel = maturityLevel;
        PreferredFormats = preferredFormats?.ToList() ?? new List<string>();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return string.Join(",", PreferredLanguages);
        yield return MaturityLevel;
        yield return string.Join(",", PreferredFormats);
    }
}



