using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �û�ƫ������ֵ����
/// </summary>
public class UserPreferences : ValueObject
{
    /// <summary>
    /// ���Ƶ�ο��ƣ�ÿ�����չʾ������
    /// </summary>
    public int? MaxDailyAdImpressions { get; private set; }

    /// <summary>
    /// ƫ�õĹ������
    /// </summary>
    public IReadOnlyList<AdType> PreferredAdTypes { get; private set; } = new List<AdType>();

    /// <summary>
    /// ������Ȥ�Ĺ�����
    /// </summary>
    public IReadOnlyList<string> BlockedCategories { get; private set; } = new List<string>();

    /// <summary>
    /// ƫ�õ���������
    /// </summary>
    public IReadOnlyList<string> PreferredTopics { get; private set; } = new List<string>();

    /// <summary>
    /// �Ƿ�������Ի����
    /// </summary>
    public bool AllowPersonalizedAds { get; private set; } = true;

    /// <summary>
    /// �Ƿ�������Ϊ׷��
    /// </summary>
    public bool AllowBehaviorTracking { get; private set; } = true;

    /// <summary>
    /// �Ƿ�������豸׷��
    /// </summary>
    public bool AllowCrossDeviceTracking { get; private set; } = true;

    /// <summary>
    /// ƫ�õĹ��Ͷ��ʱ���
    /// </summary>
    public IReadOnlyList<TimeWindow> PreferredTimeSlots { get; private set; } = new List<TimeWindow>();

    /// <summary>
    /// ֪ͨƫ������
    /// </summary>
    public NotificationPreferences? NotificationPreferences { get; private set; }

    /// <summary>
    /// ����ƫ������
    /// </summary>
    public ContentPreferences? ContentPreferences { get; private set; }

    /// <summary>
    /// ��˽����
    /// </summary>
    public PrivacyLevel PrivacyLevel { get; private set; } = PrivacyLevel.Standard;

    /// <summary>
    /// �Զ���ƫ������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomPreferences { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserPreferences() { }

    /// <summary>
    /// ���캯��
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
    /// ����Ĭ��ƫ������
    /// </summary>
    public static UserPreferences CreateDefault()
    {
        return new UserPreferences();
    }

    /// <summary>
    /// ��������ƫ������
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
    /// �����ϸ���˽����
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
    /// �Ƿ�ƫ��ָ���������
    /// </summary>
    public bool PrefersAdType(AdType adType)
    {
        return PreferredAdTypes.Contains(adType);
    }

    /// <summary>
    /// �Ƿ�����ָ�����
    /// </summary>
    public bool IsBlockedCategory(string category)
    {
        return BlockedCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ�ƫ��ָ������
    /// </summary>
    public bool PrefersTopicCategory(string topic)
    {
        return PreferredTopics.Contains(topic, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ���ƫ��ʱ�����
    /// </summary>
    public bool IsInPreferredTimeSlot(DateTime timestamp)
    {
        if (!PreferredTimeSlots.Any())
            return true; // ���û������ƫ��ʱ��Σ�Ĭ������ʱ�䶼����

        var timeOfDay = timestamp.TimeOfDay;
        return PreferredTimeSlots.Any(slot => IsWithinTimeWindow(timeOfDay, slot));
    }

    /// <summary>
    /// ��ȡ�Զ���ƫ��ֵ
    /// </summary>
    public T? GetCustomPreference<T>(string key)
    {
        if (CustomPreferences.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// �Ƿ���������ʹ��
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
    /// ����Ƿ���ʱ�䴰����
    /// </summary>
    private static bool IsWithinTimeWindow(TimeSpan timeOfDay, TimeWindow timeWindow)
    {
        // ���û��������ʼʱ�䣬��Ϊʱ�䴰�ڴӵ���0�㿪ʼ
        var startTime = timeWindow.StartTime?.TimeOfDay ?? TimeSpan.Zero;
        var endTime = startTime.Add(timeWindow.Size);

        // �����������
        if (endTime > TimeSpan.FromHours(24))
        {
            endTime = endTime.Subtract(TimeSpan.FromHours(24));
            return timeOfDay >= startTime || timeOfDay <= endTime;
        }

        return timeOfDay >= startTime && timeOfDay <= endTime;
    }

    /// <summary>
    /// ��֤ÿ�����չʾ����
    /// </summary>
    private static void ValidateMaxDailyImpressions(int? maxDailyAdImpressions)
    {
        if (maxDailyAdImpressions.HasValue && (maxDailyAdImpressions.Value < 0 || maxDailyAdImpressions.Value > 1000))
            throw new ArgumentOutOfRangeException(nameof(maxDailyAdImpressions), "ÿ�������չʾ����������0-1000֮��");
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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
/// ֪ͨƫ������
/// </summary>
public class NotificationPreferences : ValueObject
{
    /// <summary>
    /// �Ƿ���������ʼ�֪ͨ
    /// </summary>
    public bool AllowEmailNotifications { get; private set; } = true;

    /// <summary>
    /// �Ƿ���������֪ͨ
    /// </summary>
    public bool AllowPushNotifications { get; private set; } = true;

    /// <summary>
    /// �Ƿ��������֪ͨ
    /// </summary>
    public bool AllowSmsNotifications { get; private set; } = false;

    /// <summary>
    /// ֪ͨƵ��
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
/// ����ƫ������
/// </summary>
public class ContentPreferences : ValueObject
{
    /// <summary>
    /// ƫ�õ���������
    /// </summary>
    public IReadOnlyList<string> PreferredLanguages { get; private set; } = new List<string>();

    /// <summary>
    /// ���ݳ���ȼ���
    /// </summary>
    public ContentMaturityLevel MaturityLevel { get; private set; } = ContentMaturityLevel.General;

    /// <summary>
    /// ƫ�õ����ݸ�ʽ
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



