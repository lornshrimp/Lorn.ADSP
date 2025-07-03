using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向策略值对象
/// </summary>
public class TargetingPolicy : ValueObject
{
    /// <summary>
    /// 地理位置定向
    /// </summary>
    public GeoTargeting? GeoTargeting { get; private set; }

    /// <summary>
    /// 人口属性定向
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; private set; }

    /// <summary>
    /// 设备定向
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; private set; }

    /// <summary>
    /// 时间定向
    /// </summary>
    public TimeTargeting? TimeTargeting { get; private set; }

    /// <summary>
    /// 行为定向
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; private set; }

    /// <summary>
    /// 定向权重
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// 是否启用定向
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingPolicy() { }

    /// <summary>
    /// 构造函数
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
    /// 创建空的定向策略
    /// </summary>
    public static TargetingPolicy CreateEmpty()
    {
        return new TargetingPolicy();
    }

    /// <summary>
    /// 创建全量定向策略（不限制）
    /// </summary>
    public static TargetingPolicy CreateUnrestricted()
    {
        return new TargetingPolicy(isEnabled: false);
    }

    /// <summary>
    /// 计算匹配度
    /// </summary>
    public decimal CalculateMatchScore(TargetingContext context)
    {
        if (!IsEnabled)
            return 1.0m; // 未启用定向时，匹配度为100%

        decimal totalScore = 0m;
        int targetingCount = 0;

        // 地理位置定向匹配
        if (GeoTargeting != null)
        {
            totalScore += GeoTargeting.CalculateMatchScore(context.GeoLocation);
            targetingCount++;
        }

        // 人口属性定向匹配
        if (DemographicTargeting != null)
        {
            totalScore += DemographicTargeting.CalculateMatchScore(context.UserProfile);
            targetingCount++;
        }

        // 设备定向匹配
        if (DeviceTargeting != null)
        {
            totalScore += DeviceTargeting.CalculateMatchScore(context.DeviceInfo);
            targetingCount++;
        }

        // 时间定向匹配
        if (TimeTargeting != null)
        {
            totalScore += TimeTargeting.CalculateMatchScore(context.RequestTime);
            targetingCount++;
        }

        // 行为定向匹配
        if (BehaviorTargeting != null)
        {
            totalScore += BehaviorTargeting.CalculateMatchScore(context.UserBehavior);
            targetingCount++;
        }

        // 如果没有定向条件，返回默认匹配度
        if (targetingCount == 0)
            return 1.0m;

        // 计算平均匹配度并应用权重
        return (totalScore / targetingCount) * Weight;
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    public bool IsMatch(TargetingContext context, decimal threshold = 0.5m)
    {
        return CalculateMatchScore(context) >= threshold;
    }

    /// <summary>
    /// 获取相等性比较的组件
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
/// 地理位置定向
/// </summary>
public class GeoTargeting : ValueObject
{
    /// <summary>
    /// 目标地理位置列表
    /// </summary>
    public IReadOnlyList<GeoLocation> TargetLocations { get; private set; } = new List<GeoLocation>();

    /// <summary>
    /// 排除地理位置列表
    /// </summary>
    public IReadOnlyList<GeoLocation> ExcludedLocations { get; private set; } = new List<GeoLocation>();

    /// <summary>
    /// 定向半径（公里）
    /// </summary>
    public double? RadiusKm { get; private set; }

    /// <summary>
    /// 构造函数
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
    /// 计算地理位置匹配度
    /// </summary>
    public decimal CalculateMatchScore(GeoLocation? userLocation)
    {
        if (userLocation == null || !TargetLocations.Any())
            return 1.0m;

        // 检查是否在排除列表中
        if (ExcludedLocations.Any(excluded => IsLocationMatch(userLocation, excluded)))
            return 0m;

        // 检查是否在目标列表中
        return TargetLocations.Any(target => IsLocationMatch(userLocation, target)) ? 1.0m : 0m;
    }

    /// <summary>
    /// 判断位置是否匹配
    /// </summary>
    private bool IsLocationMatch(GeoLocation userLocation, GeoLocation targetLocation)
    {
        // 精确坐标匹配
        if (userLocation.HasCoordinates && targetLocation.HasCoordinates && RadiusKm.HasValue)
        {
            return userLocation.IsWithinRadius(targetLocation, RadiusKm.Value);
        }

        // 城市级别匹配
        if (!string.IsNullOrWhiteSpace(targetLocation.CityName) && 
            !string.IsNullOrWhiteSpace(userLocation.CityName))
        {
            return string.Equals(userLocation.CityName, targetLocation.CityName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
        }

        // 国家级别匹配
        return string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取相等性比较的组件
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
/// 人口属性定向
/// </summary>
public class DemographicTargeting : ValueObject
{
    /// <summary>
    /// 目标性别
    /// </summary>
    public IReadOnlyList<Gender> TargetGenders { get; private set; } = new List<Gender>();

    /// <summary>
    /// 最小年龄
    /// </summary>
    public int? MinAge { get; private set; }

    /// <summary>
    /// 最大年龄
    /// </summary>
    public int? MaxAge { get; private set; }

    /// <summary>
    /// 目标关键词
    /// </summary>
    public IReadOnlyList<string> TargetKeywords { get; private set; } = new List<string>();

    /// <summary>
    /// 构造函数
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
    /// 计算人口属性匹配度
    /// </summary>
    public decimal CalculateMatchScore(UserProfile? userProfile)
    {
        if (userProfile == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // 性别匹配
        if (TargetGenders.Any())
        {
            score *= TargetGenders.Contains(userProfile.Gender) ? 1.0m : 0m;
            criteriaCount++;
        }

        // 年龄匹配
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

        // 关键词匹配
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
    /// 获取相等性比较的组件
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
/// 设备定向
/// </summary>
public class DeviceTargeting : ValueObject
{
    /// <summary>
    /// 目标设备类型
    /// </summary>
    public IReadOnlyList<DeviceType> TargetDeviceTypes { get; private set; } = new List<DeviceType>();

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeviceTargeting(IList<DeviceType>? targetDeviceTypes = null)
    {
        TargetDeviceTypes = targetDeviceTypes?.ToList() ?? new List<DeviceType>();
    }

    /// <summary>
    /// 计算设备匹配度
    /// </summary>
    public decimal CalculateMatchScore(DeviceInfo? deviceInfo)
    {
        if (deviceInfo == null || !TargetDeviceTypes.Any())
            return 1.0m;

        return TargetDeviceTypes.Contains(deviceInfo.DeviceType) ? 1.0m : 0m;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var deviceType in TargetDeviceTypes)
            yield return deviceType;
    }
}

/// <summary>
/// 时间定向
/// </summary>
public class TimeTargeting : ValueObject
{
    /// <summary>
    /// 目标小时范围（0-23）
    /// </summary>
    public IReadOnlyList<int> TargetHours { get; private set; } = new List<int>();

    /// <summary>
    /// 目标星期范围（0=周日, 1=周一...6=周六）
    /// </summary>
    public IReadOnlyList<int> TargetDaysOfWeek { get; private set; } = new List<int>();

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// 构造函数
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
    /// 计算时间匹配度
    /// </summary>
    public decimal CalculateMatchScore(DateTime requestTime)
    {
        var targetTime = TimeZone != null ? 
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(requestTime, TimeZone) : 
            requestTime;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // 小时匹配
        if (TargetHours.Any())
        {
            score *= TargetHours.Contains(targetTime.Hour) ? 1.0m : 0m;
            criteriaCount++;
        }

        // 星期匹配
        if (TargetDaysOfWeek.Any())
        {
            score *= TargetDaysOfWeek.Contains((int)targetTime.DayOfWeek) ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// 获取相等性比较的组件
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
/// 行为定向
/// </summary>
public class BehaviorTargeting : ValueObject
{
    /// <summary>
    /// 目标兴趣标签
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

    /// <summary>
    /// 目标行为类型
    /// </summary>
    public IReadOnlyList<string> BehaviorTypes { get; private set; } = new List<string>();

    /// <summary>
    /// 构造函数
    /// </summary>
    public BehaviorTargeting(
        IList<string>? interestTags = null,
        IList<string>? behaviorTypes = null)
    {
        InterestTags = interestTags?.ToList() ?? new List<string>();
        BehaviorTypes = behaviorTypes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// 计算行为匹配度
    /// </summary>
    public decimal CalculateMatchScore(UserBehavior? userBehavior)
    {
        if (userBehavior == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // 兴趣标签匹配
        if (InterestTags.Any())
        {
            var interestMatch = InterestTags.Any(tag => 
                userBehavior.InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
            score *= interestMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        // 行为类型匹配
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
    /// 获取相等性比较的组件
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
/// 定向上下文
/// </summary>
public class TargetingContext
{
    /// <summary>
    /// 地理位置
    /// </summary>
    public GeoLocation? GeoLocation { get; set; }

    /// <summary>
    /// 用户画像
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// 设备信息
    /// </summary>
    public DeviceInfo? DeviceInfo { get; set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 用户行为
    /// </summary>
    public UserBehavior? UserBehavior { get; set; }
}

/// <summary>
/// 用户画像
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 关键词
    /// </summary>
    public IList<string> Keywords { get; set; } = new List<string>();
}

/// <summary>
/// 设备信息
/// </summary>
public class DeviceInfo
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public DeviceType DeviceType { get; set; }
}

/// <summary>
/// 用户行为
/// </summary>
public class UserBehavior
{
    /// <summary>
    /// 兴趣标签
    /// </summary>
    public IList<string> InterestTags { get; set; } = new List<string>();

    /// <summary>
    /// 行为历史
    /// </summary>
    public IList<BehaviorRecord> BehaviorHistory { get; set; } = new List<BehaviorRecord>();
}

/// <summary>
/// 行为记录
/// </summary>
public class BehaviorRecord
{
    /// <summary>
    /// 行为类型
    /// </summary>
    public string BehaviorType { get; set; } = string.Empty;

    /// <summary>
    /// 行为时间
    /// </summary>
    public DateTime Timestamp { get; set; }
}