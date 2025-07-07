using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告上下文实体
/// 封装广告请求的完整上下文信息，为广告投放决策提供环境数据
/// </summary>
public record AdContext
{
    /// <summary>
    /// 请求标识
    /// </summary>
    public required string RequestId { get; init; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// 广告位ID
    /// </summary>
    public required string PlacementId { get; init; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public required string DeviceType { get; init; }

    /// <summary>
    /// 地理位置信息
    /// </summary>
    public required GeoInfo GeoLocation { get; init; }

    /// <summary>
    /// 用户代理字符串
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 设备信息
    /// </summary>
    public DeviceInfo? DeviceInfo { get; init; }

    /// <summary>
    /// 用户画像信息
    /// </summary>
    public IReadOnlyDictionary<string, object> UserProfile { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 环境信息
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 请求元数据
    /// </summary>
    public IReadOnlyDictionary<string, object> RequestMetadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 媒体类型
    /// </summary>
    public MediaType? MediaType { get; init; }

    /// <summary>
    /// 广告类型
    /// </summary>
    public AdType? AdType { get; init; }

    /// <summary>
    /// 请求来源
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    /// 推荐信息
    /// </summary>
    public string? Referrer { get; init; }

    /// <summary>
    /// 获取用户分段信息
    /// </summary>
    /// <returns>用户分段列表</returns>
    public IReadOnlyList<string> GetUserSegments()
    {
        var segments = new List<string>();
        
        if (UserProfile.TryGetValue("segments", out var segmentData) && segmentData is IEnumerable<string> segmentList)
        {
            segments.AddRange(segmentList);
        }
        
        // 基于设备类型添加分段
        if (!string.IsNullOrEmpty(DeviceType))
        {
            segments.Add($"device_{DeviceType.ToLowerInvariant()}");
        }
        
        // 基于地理位置添加分段
        if (!string.IsNullOrEmpty(GeoLocation.CountryCode))
        {
            segments.Add($"geo_{GeoLocation.CountryCode.ToLowerInvariant()}");
        }
        
        return segments.AsReadOnly();
    }

    /// <summary>
    /// 判断是否来自目标地区
    /// </summary>
    /// <param name="targetCountryCode">目标国家代码</param>
    /// <returns>是否匹配</returns>
    public bool IsFromTargetRegion(string targetCountryCode)
    {
        return string.Equals(GeoLocation.CountryCode, targetCountryCode, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取时间段信息
    /// </summary>
    /// <returns>时间段枚举</returns>
    public TimeSlot GetTimeSlot()
    {
        var localTime = RequestTime;
        
        // 如果有时区信息，转换为本地时间
        if (!string.IsNullOrEmpty(GeoLocation.TimeZone))
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GeoLocation.TimeZone);
                localTime = TimeZoneInfo.ConvertTimeFromUtc(RequestTime, timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                // 如果找不到时区，使用UTC时间
                localTime = RequestTime;
            }
        }
        
        return localTime.Hour switch
        {
            >= 6 and < 12 => TimeSlot.Morning,
            >= 12 and < 18 => TimeSlot.Afternoon,
            >= 18 and < 22 => TimeSlot.Evening,
            _ => TimeSlot.Night
        };
    }

    /// <summary>
    /// 获取请求元数据
    /// </summary>
    /// <returns>请求元数据对象</returns>
    public RequestMetadataInfo GetRequestMetadata()
    {
        return new RequestMetadataInfo
        {
            RequestId = RequestId,
            RequestTime = RequestTime,
            Source = Source,
            UserAgent = UserAgent,
            ClientIp = ClientIp,
            Referrer = Referrer,
            SessionId = SessionId,
            AdditionalData = RequestMetadata
        };
    }

    /// <summary>
    /// 检查是否为有效的广告请求
    /// </summary>
    /// <returns>是否有效</returns>
    public bool IsValidAdRequest()
    {
        // 基本字段验证
        if (string.IsNullOrEmpty(RequestId) || string.IsNullOrEmpty(PlacementId) || string.IsNullOrEmpty(DeviceType))
        {
            return false;
        }

        // 时间验证（请求时间不能太旧）
        if (RequestTime < DateTime.UtcNow.AddHours(-1))
        {
            return false;
        }

        // 地理位置验证
        if (GeoLocation == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    /// <returns>调试信息字典</returns>
    public IReadOnlyDictionary<string, object> GetDebugInfo()
    {
        return new Dictionary<string, object>
        {
            ["RequestId"] = RequestId,
            ["PlacementId"] = PlacementId,
            ["DeviceType"] = DeviceType,
            ["GeoLocation"] = GeoLocation.CountryCode ?? "Unknown",
            ["RequestTime"] = RequestTime,
            ["TimeSlot"] = GetTimeSlot().ToString(),
            ["UserSegments"] = GetUserSegments(),
            ["IsValidRequest"] = IsValidAdRequest()
        };
    }
}

/// <summary>
/// 时间段枚举
/// </summary>
public enum TimeSlot
{
    /// <summary>
    /// 早晨 (6:00-12:00)
    /// </summary>
    Morning = 1,

    /// <summary>
    /// 下午 (12:00-18:00)
    /// </summary>
    Afternoon = 2,

    /// <summary>
    /// 晚上 (18:00-22:00)
    /// </summary>
    Evening = 3,

    /// <summary>
    /// 夜晚 (22:00-6:00)
    /// </summary>
    Night = 4
}

/// <summary>
/// 请求元数据信息
/// </summary>
public record RequestMetadataInfo
{
    /// <summary>
    /// 请求ID
    /// </summary>
    public required string RequestId { get; init; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; init; }

    /// <summary>
    /// 请求来源
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    /// 推荐信息
    /// </summary>
    public string? Referrer { get; init; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public IReadOnlyDictionary<string, object> AdditionalData { get; init; } = new Dictionary<string, object>();
}




