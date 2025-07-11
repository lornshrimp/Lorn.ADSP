using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告上下文实体
/// 封装广告请求的完整上下文信息，为广告投放决策提供环境数据
/// 生命周期：请求创建→策略使用→销毁，在整个请求处理过程中保持不变
/// </summary>
public class AdContext : EntityBase
{
    /// <summary>
    /// 请求唯一标识
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// 用户标识
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// 广告位标识
    /// </summary>
    public string PlacementId { get; private set; }

    /// <summary>
    /// 设备信息
    /// </summary>
    public DeviceInfo Device { get; private set; }

    /// <summary>
    /// 地理位置信息
    /// </summary>
    public GeoInfo GeoLocation { get; private set; }

    /// <summary>
    /// 用户代理信息
    /// </summary>
    public string UserAgent { get; private set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// 时间窗口信息
    /// </summary>
    public TimeWindow TimeWindow { get; private set; }

    /// <summary>
    /// 用户画像信息
    /// </summary>
    public Dictionary<string, object> UserProfile { get; private set; }

    /// <summary>
    /// 环境上下文信息
    /// </summary>
    public Dictionary<string, object> EnvironmentInfo { get; private set; }

    /// <summary>
    /// 请求来源
    /// </summary>
    public RequestSource RequestSource { get; private set; }

    /// <summary>
    /// 广告位尺寸
    /// </summary>
    public AdSize AdSize { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AdContext(
        string requestId,
        string userId,
        string placementId,
        DeviceInfo device,
        GeoInfo geoLocation,
        string userAgent,
        DateTime requestTime,
        TimeWindow timeWindow,
        RequestSource requestSource,
        AdSize adSize,
        Dictionary<string, object>? userProfile = null,
        Dictionary<string, object>? environmentInfo = null)
    {
        RequestId = requestId;
        UserId = userId;
        PlacementId = placementId;
        Device = device;
        GeoLocation = geoLocation;
        UserAgent = userAgent;
        RequestTime = requestTime;
        TimeWindow = timeWindow;
        RequestSource = requestSource;
        AdSize = adSize;
        UserProfile = userProfile ?? new Dictionary<string, object>();
        EnvironmentInfo = environmentInfo ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// 创建广告上下文对象
    /// </summary>
    public static AdContext Create(
        string requestId,
        string userId,
        string placementId,
        DeviceInfo device,
        GeoInfo geoLocation,
        string userAgent,
        DateTime requestTime,
        TimeWindow timeWindow,
        RequestSource requestSource,
        AdSize adSize,
        Dictionary<string, object>? userProfile = null,
        Dictionary<string, object>? environmentInfo = null)
    {
        ValidateParameters(requestId, userId, placementId, device, geoLocation, userAgent, timeWindow, adSize);

        return new AdContext(
            requestId,
            userId,
            placementId,
            device,
            geoLocation,
            userAgent,
            requestTime,
            timeWindow,
            requestSource,
            adSize,
            userProfile,
            environmentInfo);
    }

    /// <summary>
    /// 更新用户画像信息
    /// </summary>
    public void UpdateUserProfile(Dictionary<string, object> userProfile)
    {
        UserProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加用户画像属性
    /// </summary>
    public void AddUserProfileAttribute(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("用户画像键不能为空", nameof(key));

        UserProfile[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 移除用户画像属性
    /// </summary>
    public void RemoveUserProfileAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("用户画像键不能为空", nameof(key));

        if (UserProfile.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 更新环境信息
    /// </summary>
    public void UpdateEnvironmentInfo(Dictionary<string, object> environmentInfo)
    {
        EnvironmentInfo = environmentInfo ?? throw new ArgumentNullException(nameof(environmentInfo));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加环境信息
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("环境信息键不能为空", nameof(key));

        EnvironmentInfo[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 移除环境信息
    /// </summary>
    public void RemoveEnvironmentInfo(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("环境信息键不能为空", nameof(key));

        if (EnvironmentInfo.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 获取用户细分标签
    /// </summary>
    public List<string> GetUserSegments()
    {
        var segments = new List<string>();

        if (UserProfile.ContainsKey("segments"))
        {
            if (UserProfile["segments"] is List<string> segmentList)
            {
                segments.AddRange(segmentList);
            }
        }

        return segments;
    }

    /// <summary>
    /// 检查是否来自目标区域
    /// </summary>
    public bool IsFromTargetRegion(GeoInfo targetGeoLocation)
    {
        if (targetGeoLocation == null)
            return false;

        // 检查国家匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.CountryName) &&
            !string.Equals(GeoLocation.CountryName, targetGeoLocation.CountryName, StringComparison.OrdinalIgnoreCase))
            return false;

        // 检查省份匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.ProvinceName) &&
            !string.Equals(GeoLocation.ProvinceName, targetGeoLocation.ProvinceName, StringComparison.OrdinalIgnoreCase))
            return false;

        // 检查城市匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.CityName) &&
            !string.Equals(GeoLocation.CityName, targetGeoLocation.CityName, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    /// <summary>
    /// 获取时间段
    /// </summary>
    public string GetTimeSlot()
    {
        var hour = RequestTime.Hour;
        return hour switch
        {
            >= 6 and < 12 => "上午",
            >= 12 and < 18 => "下午",
            >= 18 and < 24 => "晚上",
            _ => "凌晨"
        };
    }

    /// <summary>
    /// 获取星期几
    /// </summary>
    public DayOfWeek GetDayOfWeek()
    {
        return RequestTime.DayOfWeek;
    }

    /// <summary>
    /// 获取请求元数据
    /// </summary>
    public Dictionary<string, object> GetRequestMetadata()
    {
        return new Dictionary<string, object>
        {
            ["RequestId"] = RequestId,
            ["UserId"] = UserId,
            ["PlacementId"] = PlacementId,
            ["RequestTime"] = RequestTime,
            ["TimeSlot"] = GetTimeSlot(),
            ["DayOfWeek"] = GetDayOfWeek().ToString(),
            ["DeviceType"] = Device.DeviceType,
            ["Country"] = GeoLocation.CountryName,
            ["Province"] = GeoLocation.ProvinceName,
            ["City"] = GeoLocation.CityName,
            ["UserAgent"] = UserAgent,
            ["RequestSource"] = RequestSource.ToString(),
            ["AdSize"] = $"{AdSize.Width}x{AdSize.Height}"
        };
    }

    /// <summary>
    /// 检查是否为移动设备
    /// </summary>
    public bool IsMobileDevice()
    {
        return Device.DeviceType == DeviceType.Smartphone || Device.DeviceType == DeviceType.Tablet;
    }

    /// <summary>
    /// 检查是否为高价值用户
    /// </summary>
    public bool IsHighValueUser()
    {
        if (UserProfile.ContainsKey("value_score"))
        {
            if (UserProfile["value_score"] is double valueScore)
            {
                return valueScore > 0.8;
            }
        }

        return false;
    }

    /// <summary>
    /// 参数验证
    /// </summary>
    private static void ValidateParameters(
        string requestId,
        string userId,
        string placementId,
        DeviceInfo device,
        GeoInfo geoLocation,
        string userAgent,
        TimeWindow timeWindow,
        AdSize adSize)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("用户ID不能为空", nameof(userId));

        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (device == null)
            throw new ArgumentNullException(nameof(device));

        if (geoLocation == null)
            throw new ArgumentNullException(nameof(geoLocation));

        if (string.IsNullOrEmpty(userAgent))
            throw new ArgumentException("用户代理信息不能为空", nameof(userAgent));

        if (timeWindow == null)
            throw new ArgumentNullException(nameof(timeWindow));

        if (adSize == null)
            throw new ArgumentNullException(nameof(adSize));
    }
}




