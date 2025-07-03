using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �������ֵ����
/// </summary>
public class TargetingPolicy : ValueObject
{
    /// <summary>
    /// ����λ�ö���
    /// </summary>
    public GeoTargeting? GeoTargeting { get; private set; }

    /// <summary>
    /// �˿����Զ���
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; private set; }

    /// <summary>
    /// �豸����
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; private set; }

    /// <summary>
    /// ʱ�䶨��
    /// </summary>
    public TimeTargeting? TimeTargeting { get; private set; }

    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; private set; }

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// �Ƿ����ö���
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TargetingPolicy() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TargetingPolicy(
        GeoTargeting? geoTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        GeoTargeting = geoTargeting;
        DemographicTargeting = demographicTargeting;
        DeviceTargeting = deviceTargeting;
        TimeTargeting = timeTargeting;
        BehaviorTargeting = behaviorTargeting;
        Weight = weight;
        IsEnabled = isEnabled;
    }

    /// <summary>
    /// �����յĶ������
    /// </summary>
    public static TargetingPolicy CreateEmpty()
    {
        return new TargetingPolicy();
    }

    /// <summary>
    /// ����ȫ��������ԣ������ƣ�
    /// </summary>
    public static TargetingPolicy CreateUnrestricted()
    {
        return new TargetingPolicy(isEnabled: false);
    }

    /// <summary>
    /// ����ƥ���
    /// </summary>
    public decimal CalculateMatchScore(TargetingContext context)
    {
        if (!IsEnabled)
            return 1.0m; // δ���ö���ʱ��ƥ���Ϊ100%

        decimal totalScore = 0m;
        int targetingCount = 0;

        // ����λ�ö���ƥ��
        if (GeoTargeting != null)
        {
            totalScore += GeoTargeting.CalculateMatchScore(context.GeoLocation);
            targetingCount++;
        }

        // �˿����Զ���ƥ��
        if (DemographicTargeting != null)
        {
            totalScore += DemographicTargeting.CalculateMatchScore(context.UserProfile);
            targetingCount++;
        }

        // �豸����ƥ��
        if (DeviceTargeting != null)
        {
            totalScore += DeviceTargeting.CalculateMatchScore(context.DeviceInfo);
            targetingCount++;
        }

        // ʱ�䶨��ƥ��
        if (TimeTargeting != null)
        {
            totalScore += TimeTargeting.CalculateMatchScore(context.RequestTime);
            targetingCount++;
        }

        // ��Ϊ����ƥ��
        if (BehaviorTargeting != null)
        {
            totalScore += BehaviorTargeting.CalculateMatchScore(context.UserBehavior);
            targetingCount++;
        }

        // ���û�ж�������������Ĭ��ƥ���
        if (targetingCount == 0)
            return 1.0m;

        // ����ƽ��ƥ��Ȳ�Ӧ��Ȩ��
        return (totalScore / targetingCount) * Weight;
    }

    /// <summary>
    /// �Ƿ�ƥ��
    /// </summary>
    public bool IsMatch(TargetingContext context, decimal threshold = 0.5m)
    {
        return CalculateMatchScore(context) >= threshold;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GeoTargeting ?? new GeoTargeting();
        yield return DemographicTargeting ?? new DemographicTargeting();
        yield return DeviceTargeting ?? new DeviceTargeting();
        yield return TimeTargeting ?? new TimeTargeting();
        yield return BehaviorTargeting ?? new BehaviorTargeting();
        yield return Weight;
        yield return IsEnabled;
    }
}

/// <summary>
/// ����λ�ö���
/// </summary>
public class GeoTargeting : ValueObject
{
    /// <summary>
    /// Ŀ�����λ���б�
    /// </summary>
    public IReadOnlyList<GeoLocation> TargetLocations { get; private set; } = new List<GeoLocation>();

    /// <summary>
    /// �ų�����λ���б�
    /// </summary>
    public IReadOnlyList<GeoLocation> ExcludedLocations { get; private set; } = new List<GeoLocation>();

    /// <summary>
    /// ����뾶�����
    /// </summary>
    public double? RadiusKm { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public GeoTargeting(
        IList<GeoLocation>? targetLocations = null,
        IList<GeoLocation>? excludedLocations = null,
        double? radiusKm = null)
    {
        TargetLocations = targetLocations?.ToList() ?? new List<GeoLocation>();
        ExcludedLocations = excludedLocations?.ToList() ?? new List<GeoLocation>();
        RadiusKm = radiusKm;
    }

    /// <summary>
    /// �������λ��ƥ���
    /// </summary>
    public decimal CalculateMatchScore(GeoLocation? userLocation)
    {
        if (userLocation == null || !TargetLocations.Any())
            return 1.0m;

        // ����Ƿ����ų��б���
        if (ExcludedLocations.Any(excluded => IsLocationMatch(userLocation, excluded)))
            return 0m;

        // ����Ƿ���Ŀ���б���
        return TargetLocations.Any(target => IsLocationMatch(userLocation, target)) ? 1.0m : 0m;
    }

    /// <summary>
    /// �ж�λ���Ƿ�ƥ��
    /// </summary>
    private bool IsLocationMatch(GeoLocation userLocation, GeoLocation targetLocation)
    {
        // ��ȷ����ƥ��
        if (userLocation.HasCoordinates && targetLocation.HasCoordinates && RadiusKm.HasValue)
        {
            return userLocation.IsWithinRadius(targetLocation, RadiusKm.Value);
        }

        // ���м���ƥ��
        if (!string.IsNullOrWhiteSpace(targetLocation.CityName) && 
            !string.IsNullOrWhiteSpace(userLocation.CityName))
        {
            return string.Equals(userLocation.CityName, targetLocation.CityName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
        }

        // ���Ҽ���ƥ��
        return string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var location in TargetLocations)
            yield return location;
        foreach (var location in ExcludedLocations)
            yield return location;
        yield return RadiusKm ?? 0;
    }
}

/// <summary>
/// �˿����Զ���
/// </summary>
public class DemographicTargeting : ValueObject
{
    /// <summary>
    /// Ŀ���Ա�
    /// </summary>
    public IReadOnlyList<Gender> TargetGenders { get; private set; } = new List<Gender>();

    /// <summary>
    /// ��С����
    /// </summary>
    public int? MinAge { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public int? MaxAge { get; private set; }

    /// <summary>
    /// Ŀ��ؼ���
    /// </summary>
    public IReadOnlyList<string> TargetKeywords { get; private set; } = new List<string>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public DemographicTargeting(
        IList<Gender>? targetGenders = null,
        int? minAge = null,
        int? maxAge = null,
        IList<string>? targetKeywords = null)
    {
        TargetGenders = targetGenders?.ToList() ?? new List<Gender>();
        MinAge = minAge;
        MaxAge = maxAge;
        TargetKeywords = targetKeywords?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// �����˿�����ƥ���
    /// </summary>
    public decimal CalculateMatchScore(UserProfile? userProfile)
    {
        if (userProfile == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // �Ա�ƥ��
        if (TargetGenders.Any())
        {
            score *= TargetGenders.Contains(userProfile.Gender) ? 1.0m : 0m;
            criteriaCount++;
        }

        // ����ƥ��
        if (MinAge.HasValue || MaxAge.HasValue)
        {
            var ageMatch = true;
            if (MinAge.HasValue && userProfile.Age < MinAge.Value)
                ageMatch = false;
            if (MaxAge.HasValue && userProfile.Age > MaxAge.Value)
                ageMatch = false;

            score *= ageMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        // �ؼ���ƥ��
        if (TargetKeywords.Any())
        {
            var keywordMatch = TargetKeywords.Any(keyword => 
                userProfile.Keywords.Any(userKeyword => 
                    userKeyword.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            score *= keywordMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var gender in TargetGenders)
            yield return gender;
        yield return MinAge ?? 0;
        yield return MaxAge ?? 0;
        foreach (var keyword in TargetKeywords)
            yield return keyword;
    }
}

/// <summary>
/// �豸����
/// </summary>
public class DeviceTargeting : ValueObject
{
    /// <summary>
    /// Ŀ���豸����
    /// </summary>
    public IReadOnlyList<DeviceType> TargetDeviceTypes { get; private set; } = new List<DeviceType>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public DeviceTargeting(IList<DeviceType>? targetDeviceTypes = null)
    {
        TargetDeviceTypes = targetDeviceTypes?.ToList() ?? new List<DeviceType>();
    }

    /// <summary>
    /// �����豸ƥ���
    /// </summary>
    public decimal CalculateMatchScore(DeviceInfo? deviceInfo)
    {
        if (deviceInfo == null || !TargetDeviceTypes.Any())
            return 1.0m;

        return TargetDeviceTypes.Contains(deviceInfo.DeviceType) ? 1.0m : 0m;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var deviceType in TargetDeviceTypes)
            yield return deviceType;
    }
}

/// <summary>
/// ʱ�䶨��
/// </summary>
public class TimeTargeting : ValueObject
{
    /// <summary>
    /// Ŀ��Сʱ��Χ��0-23��
    /// </summary>
    public IReadOnlyList<int> TargetHours { get; private set; } = new List<int>();

    /// <summary>
    /// Ŀ�����ڷ�Χ��0=����, 1=��һ...6=������
    /// </summary>
    public IReadOnlyList<int> TargetDaysOfWeek { get; private set; } = new List<int>();

    /// <summary>
    /// ʱ��
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TimeTargeting(
        IList<int>? targetHours = null,
        IList<int>? targetDaysOfWeek = null,
        string? timeZone = null)
    {
        TargetHours = targetHours?.ToList() ?? new List<int>();
        TargetDaysOfWeek = targetDaysOfWeek?.ToList() ?? new List<int>();
        TimeZone = timeZone;
    }

    /// <summary>
    /// ����ʱ��ƥ���
    /// </summary>
    public decimal CalculateMatchScore(DateTime requestTime)
    {
        var targetTime = TimeZone != null ? 
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(requestTime, TimeZone) : 
            requestTime;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // Сʱƥ��
        if (TargetHours.Any())
        {
            score *= TargetHours.Contains(targetTime.Hour) ? 1.0m : 0m;
            criteriaCount++;
        }

        // ����ƥ��
        if (TargetDaysOfWeek.Any())
        {
            score *= TargetDaysOfWeek.Contains((int)targetTime.DayOfWeek) ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var hour in TargetHours)
            yield return hour;
        foreach (var day in TargetDaysOfWeek)
            yield return day;
        yield return TimeZone ?? string.Empty;
    }
}

/// <summary>
/// ��Ϊ����
/// </summary>
public class BehaviorTargeting : ValueObject
{
    /// <summary>
    /// Ŀ����Ȥ��ǩ
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

    /// <summary>
    /// Ŀ����Ϊ����
    /// </summary>
    public IReadOnlyList<string> BehaviorTypes { get; private set; } = new List<string>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public BehaviorTargeting(
        IList<string>? interestTags = null,
        IList<string>? behaviorTypes = null)
    {
        InterestTags = interestTags?.ToList() ?? new List<string>();
        BehaviorTypes = behaviorTypes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// ������Ϊƥ���
    /// </summary>
    public decimal CalculateMatchScore(UserBehavior? userBehavior)
    {
        if (userBehavior == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // ��Ȥ��ǩƥ��
        if (InterestTags.Any())
        {
            var interestMatch = InterestTags.Any(tag => 
                userBehavior.InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
            score *= interestMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        // ��Ϊ����ƥ��
        if (BehaviorTypes.Any())
        {
            var behaviorMatch = BehaviorTypes.Any(type => 
                userBehavior.BehaviorHistory.Any(b => 
                    string.Equals(b.BehaviorType, type, StringComparison.OrdinalIgnoreCase)));
            score *= behaviorMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var tag in InterestTags)
            yield return tag;
        foreach (var type in BehaviorTypes)
            yield return type;
    }
}

/// <summary>
/// ����������
/// </summary>
public class TargetingContext
{
    /// <summary>
    /// ����λ��
    /// </summary>
    public GeoLocation? GeoLocation { get; set; }

    /// <summary>
    /// �û�����
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// �豸��Ϣ
    /// </summary>
    public DeviceInfo? DeviceInfo { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// �û���Ϊ
    /// </summary>
    public UserBehavior? UserBehavior { get; set; }
}

/// <summary>
/// �û�����
/// </summary>
public class UserProfile
{
    /// <summary>
    /// �Ա�
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// �ؼ���
    /// </summary>
    public IList<string> Keywords { get; set; } = new List<string>();
}

/// <summary>
/// �豸��Ϣ
/// </summary>
public class DeviceInfo
{
    /// <summary>
    /// �豸����
    /// </summary>
    public DeviceType DeviceType { get; set; }
}

/// <summary>
/// �û���Ϊ
/// </summary>
public class UserBehavior
{
    /// <summary>
    /// ��Ȥ��ǩ
    /// </summary>
    public IList<string> InterestTags { get; set; } = new List<string>();

    /// <summary>
    /// ��Ϊ��ʷ
    /// </summary>
    public IList<BehaviorRecord> BehaviorHistory { get; set; } = new List<BehaviorRecord>();
}

/// <summary>
/// ��Ϊ��¼
/// </summary>
public class BehaviorRecord
{
    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public string BehaviorType { get; set; } = string.Empty;

    /// <summary>
    /// ��Ϊʱ��
    /// </summary>
    public DateTime Timestamp { get; set; }
}