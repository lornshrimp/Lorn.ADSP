using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向配置值对象
/// </summary>
public class TargetingConfig : ValueObject
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
    /// 关键词定向
    /// </summary>
    public IReadOnlyList<string> Keywords { get; private set; }

    /// <summary>
    /// 兴趣标签定向
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; }

    /// <summary>
    /// 定向权重
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingConfig(
        GeoTargeting? geoTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m)
    {
        GeoTargeting = geoTargeting;
        DemographicTargeting = demographicTargeting;
        DeviceTargeting = deviceTargeting;
        TimeTargeting = timeTargeting;
        BehaviorTargeting = behaviorTargeting;
        Keywords = keywords ?? Array.Empty<string>();
        InterestTags = interestTags ?? Array.Empty<string>();
        Weight = weight;
    }

    /// <summary>
    /// 创建定向配置
    /// </summary>
    public static TargetingConfig Create(
        GeoTargeting? geoTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m)
    {
        if (weight <= 0)
            throw new ArgumentException("定向权重必须大于0", nameof(weight));

        return new TargetingConfig(
            geoTargeting,
            demographicTargeting,
            deviceTargeting,
            timeTargeting,
            behaviorTargeting,
            keywords,
            interestTags,
            weight);
    }

    /// <summary>
    /// 计算匹配分数
    /// </summary>
    public double CalculateMatchScore(AdContext context)
    {
        if (context == null)
            return 0.0;

        double totalScore = 0.0;
        int factors = 0;

        // 地理位置匹配
        if (GeoTargeting != null)
        {
            totalScore += GeoTargeting.IsMatch(context.GeoLocation) ? 1.0 : 0.0;
            factors++;
        }

        // 设备匹配
        if (DeviceTargeting != null && context.Device != null)
        {
            totalScore += DeviceTargeting.IsMatch(context.Device) ? 1.0 : 0.0;
            factors++;
        }

        // 时间匹配
        if (TimeTargeting != null)
        {
            totalScore += TimeTargeting.IsActiveAt(context.RequestTime) ? 1.0 : 0.0;
            factors++;
        }

        // 人口属性匹配（需要用户画像）
        if (DemographicTargeting != null && context.UserProfile.Any())
        {
            // 简化的匹配逻辑，实际需要根据用户画像数据进行匹配
            totalScore += 0.5; // 假设部分匹配
            factors++;
        }

        return factors > 0 ? (totalScore / factors) * (double)Weight : 0.0;
    }

    /// <summary>
    /// 检查是否匹配
    /// </summary>
    public bool IsMatch(AdContext context)
    {
        if (context == null)
            return false;

        // 地理位置匹配检查
        if (GeoTargeting != null && !GeoTargeting.IsMatch(context.GeoLocation))
            return false;

        // 设备匹配检查
        if (DeviceTargeting != null && context.Device != null && !DeviceTargeting.IsMatch(context.Device))
            return false;

        // 时间匹配检查
        if (TimeTargeting != null && !TimeTargeting.IsActiveAt(context.RequestTime))
            return false;

        return true;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GeoTargeting ?? new object();
        yield return DemographicTargeting ?? new object();
        yield return DeviceTargeting ?? new object();
        yield return TimeTargeting ?? new object();
        yield return BehaviorTargeting ?? new object();
        yield return Weight;

        foreach (var keyword in Keywords.OrderBy(x => x))
        {
            yield return keyword;
        }

        foreach (var tag in InterestTags.OrderBy(x => x))
        {
            yield return tag;
        }
    }
}

/// <summary>
/// 设备定向
/// </summary>
public class DeviceTargeting : ValueObject
{
    /// <summary>
    /// 设备类型列表
    /// </summary>
    public IReadOnlyList<DeviceType> DeviceTypes { get; private set; }

    /// <summary>
    /// 操作系统列表
    /// </summary>
    public IReadOnlyList<string> OperatingSystems { get; private set; }

    /// <summary>
    /// 浏览器列表
    /// </summary>
    public IReadOnlyList<string> Browsers { get; private set; }

    private DeviceTargeting(
        IReadOnlyList<DeviceType> deviceTypes,
        IReadOnlyList<string> operatingSystems,
        IReadOnlyList<string> browsers)
    {
        DeviceTypes = deviceTypes;
        OperatingSystems = operatingSystems;
        Browsers = browsers;
    }

    public static DeviceTargeting Create(
        IReadOnlyList<DeviceType>? deviceTypes = null,
        IReadOnlyList<string>? operatingSystems = null,
        IReadOnlyList<string>? browsers = null)
    {
        return new DeviceTargeting(
            deviceTypes ?? Array.Empty<DeviceType>(),
            operatingSystems ?? Array.Empty<string>(),
            browsers ?? Array.Empty<string>());
    }

    public bool IsMatch(DeviceInfo deviceInfo)
    {
        if (deviceInfo == null)
            return false;

        // 设备类型匹配
        if (DeviceTypes.Any() && !DeviceTypes.Contains(deviceInfo.DeviceType))
            return false;

        // 操作系统匹配
        if (OperatingSystems.Any() && !string.IsNullOrEmpty(deviceInfo.OperatingSystem))
        {
            if (!OperatingSystems.Any(os =>
                deviceInfo.OperatingSystem.Contains(os, StringComparison.OrdinalIgnoreCase)))
                return false;
        }

        return true;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var deviceType in DeviceTypes.OrderBy(x => x))
        {
            yield return deviceType;
        }

        foreach (var os in OperatingSystems.OrderBy(x => x))
        {
            yield return os;
        }

        foreach (var browser in Browsers.OrderBy(x => x))
        {
            yield return browser;
        }
    }

    internal decimal CalculateMatchScore(DeviceInfo? deviceInfo)
    {
        if (deviceInfo == null)
        {
            return 0m; // 如果设备信息为空，返回0分
        }

        decimal score = 0m;

        // 根据设备类型计算分数
        if (DeviceTypes.Contains(deviceInfo.DeviceType))
        {
            score += 0.5m; // 设备类型匹配加0.5分
        }

        // 根据操作系统计算分数
        if (OperatingSystems.Contains(deviceInfo.OperatingSystem))
        {
            score += 0.3m; // 操作系统匹配加0.3分
        }

        // 根据浏览器计算分数
        if (Browsers.Contains(deviceInfo.Browser))
        {
            score += 0.2m; // 浏览器匹配加0.2分
        }

        return score; // 返回计算后的匹配分数
    }
}

/// <summary>
/// 时间定向
/// </summary>
public class TimeTargeting : ValueObject
{
    /// <summary>
    /// 星期几列表
    /// </summary>
    public IReadOnlyList<DayOfWeek> Days { get; private set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string TimeZoneId { get; private set; } // Renamed from TimeZone to TimeZoneId

    private TimeTargeting(
        IReadOnlyList<DayOfWeek> days,
        TimeOnly startTime,
        TimeOnly endTime,
        string timeZoneId) // Updated parameter name
    {
        Days = days;
        StartTime = startTime;
        EndTime = endTime;
        TimeZoneId = timeZoneId; // Updated property assignment
    }

    public static TimeTargeting Create(
        IReadOnlyList<DayOfWeek>? days = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        string timeZoneId = "UTC") // Updated parameter name
    {
        return new TimeTargeting(
            days ?? Enum.GetValues<DayOfWeek>(),
            startTime ?? TimeOnly.MinValue,
            endTime ?? TimeOnly.MaxValue,
            timeZoneId); // Updated argument
    }

    public bool IsActiveAt(DateTime dateTime)
    {
        // 检查星期几
        if (Days.Any() && !Days.Contains(dateTime.DayOfWeek))
            return false;

        // 检查时间范围
        var time = TimeOnly.FromDateTime(dateTime);
        if (StartTime <= EndTime)
        {
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            // 跨天的时间范围
            return time >= StartTime || time <= EndTime;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
        yield return TimeZoneId; // Updated property name

        foreach (var day in Days.OrderBy(x => x))
        {
            yield return day;
        }
    }

    internal decimal CalculateMatchScore(DateTime requestTime)
    {
        // 获取当前时间
        DateTime currentTime = DateTime.Now;

        // 计算时间差
        TimeSpan timeDifference = requestTime - currentTime;

        // 根据时间差计算匹配分数
        // 假设时间越接近，分数越高，分数范围为0到1
        decimal score = 1 - (decimal)Math.Abs(timeDifference.TotalHours) / 24;

        // 确保分数在0到1之间
        return Math.Max(0, Math.Min(1, score));
    }
}
