using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告上下文实体
/// 广告引擎根据AdRequest和其他信息组成的完整请求上下文，按广告位组织候选广告集合
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
    public string? UserId { get; private set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// 按广告位分组的候选广告集合
    /// Key为广告位ID，Value为AdCandidate数组
    /// </summary>
    private readonly Dictionary<string, List<AdCandidate>> _candidatesByPlacement = new();
    public IReadOnlyDictionary<string, IReadOnlyList<AdCandidate>> CandidatesByPlacement => 
        _candidatesByPlacement.ToDictionary(
            kv => kv.Key, 
            kv => (IReadOnlyList<AdCandidate>)kv.Value.AsReadOnly()
        ).AsReadOnly();

    /// <summary>
    /// 定向上下文
    /// </summary>
    public TargetingContext TargetingContext { get; private set; }

    /// <summary>
    /// 环境信息
    /// 包含时间、竞争情况等环境数据
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; private set; }

    /// <summary>
    /// 设备信息（从TargetingContext获取的快捷访问）
    /// </summary>
    public DeviceInfo? Device => TargetingContext?.GetTargetingContext<DeviceInfo>("device") ?? TargetingContext?.GetTargetingContext<DeviceInfo>();

    /// <summary>
    /// 地理位置信息（从TargetingContext获取的快捷访问）
    /// </summary>
    public GeoInfo? GeoLocation => TargetingContext?.GetTargetingContext<GeoInfo>("geo") ?? TargetingContext?.GetTargetingContext<GeoInfo>();

    /// <summary>
    /// 用户画像信息（从TargetingContext获取的快捷访问）
    /// </summary>
    public UserProfile? UserProfile => TargetingContext?.GetTargetingContext<UserProfile>("user") ?? TargetingContext?.GetTargetingContext<UserProfile>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AdContext(
        string requestId,
        string? userId,
        DateTime requestTime,
        TargetingContext targetingContext,
        IDictionary<string, object>? environmentInfo = null)
    {
        RequestId = requestId;
        UserId = userId;
        RequestTime = requestTime;
        TargetingContext = targetingContext;
        EnvironmentInfo = environmentInfo?.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly() ?? 
                          new Dictionary<string, object>().AsReadOnly();
    }

    /// <summary>
    /// 创建广告上下文对象
    /// </summary>
    public static AdContext Create(
        string requestId,
        string? userId,
        DateTime requestTime,
        TargetingContext targetingContext,
        IDictionary<string, object>? environmentInfo = null)
    {
        ValidateParameters(requestId, targetingContext);

        return new AdContext(
            requestId,
            userId,
            requestTime,
            targetingContext,
            environmentInfo);
    }

    /// <summary>
    /// 从现有属性创建广告上下文（兼容原有接口）
    /// </summary>
    public static AdContext CreateFromLegacyData(
        string requestId,
        string? userId,
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
        ValidateLegacyParameters(requestId, userId, placementId, device, geoLocation, userAgent, timeWindow, adSize);

        // 构建环境信息
        var envInfo = new Dictionary<string, object>
        {
            ["userAgent"] = userAgent,
            ["timeWindow"] = timeWindow,
            ["requestSource"] = requestSource,
            ["adSize"] = adSize
        };

        if (environmentInfo != null)
        {
            foreach (var kv in environmentInfo)
            {
                envInfo[kv.Key] = kv.Value;
            }
        }

        // 创建定向上下文
        var targetingContexts = new Dictionary<string, ITargetingContext>();
        
        if (device != null)
        {
            targetingContexts["device"] = device;
        }
        
        if (geoLocation != null)
        {
            targetingContexts["geo"] = geoLocation;
        }

        // 如果有用户画像，添加到定向上下文
        if (userProfile?.ContainsKey("userProfile") == true && 
            userProfile["userProfile"] is UserProfile profile)
        {
            targetingContexts["user"] = profile;
        }

        var targetingContext = TargetingContext.Create(
            requestId: requestId,
            targetingContexts: targetingContexts,
            contextMetadata: new Dictionary<string, object>
            {
                ["placementId"] = placementId,
                ["userAgent"] = userAgent,
                ["requestSource"] = requestSource.ToString(),
                ["adSize"] = $"{adSize.Width}x{adSize.Height}",
                ["requestTime"] = requestTime,
                ["isMobileDevice"] = device.DeviceType == DeviceType.Smartphone || device.DeviceType == DeviceType.Tablet
            }
        );

        return new AdContext(
            requestId,
            userId,
            requestTime,
            targetingContext,
            envInfo);
    }

    /// <summary>
    /// 获取指定广告位的候选广告列表
    /// </summary>
    public IReadOnlyList<AdCandidate> GetCandidatesForPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            return new List<AdCandidate>().AsReadOnly();

        return _candidatesByPlacement.TryGetValue(placementId, out var candidates) 
            ? candidates.AsReadOnly() 
            : new List<AdCandidate>().AsReadOnly();
    }

    /// <summary>
    /// 添加候选广告到指定广告位
    /// </summary>
    public void AddCandidateToPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        // 确保候选广告的PlacementId与目标广告位一致
        if (candidate.PlacementId != placementId)
        {
            candidate.AssignToPlacement(placementId);
        }

        if (!_candidatesByPlacement.ContainsKey(placementId))
        {
            _candidatesByPlacement[placementId] = new List<AdCandidate>();
        }

        _candidatesByPlacement[placementId].Add(candidate);
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 批量添加候选广告到指定广告位
    /// </summary>
    public void AddCandidatesToPlacement(string placementId, IEnumerable<AdCandidate> candidates)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));

        foreach (var candidate in candidates)
        {
            AddCandidateToPlacement(placementId, candidate);
        }
    }

    /// <summary>
    /// 移除指定广告位的候选广告
    /// </summary>
    public bool RemoveCandidateFromPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId) || candidate == null)
            return false;

        if (_candidatesByPlacement.TryGetValue(placementId, out var candidates))
        {
            var removed = candidates.Remove(candidate);
            if (removed)
            {
                UpdateLastModifiedTime();
            }
            return removed;
        }

        return false;
    }

    /// <summary>
    /// 清空指定广告位的候选广告
    /// </summary>
    public void ClearCandidatesForPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            return;

        if (_candidatesByPlacement.TryGetValue(placementId, out var candidates))
        {
            candidates.Clear();
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 获取所有广告位ID列表
    /// </summary>
    public IReadOnlyList<string> GetAllPlacements()
    {
        return _candidatesByPlacement.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取候选广告总数
    /// </summary>
    public int GetTotalCandidatesCount()
    {
        return _candidatesByPlacement.Values.Sum(candidates => candidates.Count);
    }

    /// <summary>
    /// 获取用户细分标签
    /// </summary>
    public List<string> GetUserSegments()
    {
        var segments = new List<string>();

        if (UserProfile != null)
        {
            segments.AddRange(UserProfile.GetUserSegments());
        }

        // 从定向上下文中获取标签
        var contextualTags = TargetingContext.GetMetadata<IEnumerable<string>>("contextualTags");
        if (contextualTags != null)
        {
            segments.AddRange(contextualTags);
        }

        return segments.Distinct().ToList();
    }

    /// <summary>
    /// 判断是否来自目标地区
    /// </summary>
    public bool IsFromTargetRegion(GeoInfo targetGeoLocation)
    {
        if (targetGeoLocation == null || GeoLocation == null)
            return false;

        // 国家匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("countryName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("countryName"), 
                          targetGeoLocation.GetProperty<string>("countryName"), 
                          StringComparison.OrdinalIgnoreCase))
            return false;

        // 省份匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("provinceName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("provinceName"), 
                          targetGeoLocation.GetProperty<string>("provinceName"), 
                          StringComparison.OrdinalIgnoreCase))
            return false;

        // 城市匹配
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("cityName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("cityName"), 
                          targetGeoLocation.GetProperty<string>("cityName"), 
                          StringComparison.OrdinalIgnoreCase))
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
        var metadata = new Dictionary<string, object>
        {
            ["RequestId"] = RequestId,
            ["UserId"] = UserId ?? "Anonymous",
            ["RequestTime"] = RequestTime,
            ["TimeSlot"] = GetTimeSlot(),
            ["DayOfWeek"] = GetDayOfWeek().ToString(),
            ["TotalCandidates"] = GetTotalCandidatesCount(),
            ["PlacementCount"] = GetAllPlacements().Count
        };

        // 添加设备信息
        if (Device != null)
        {
            metadata["DeviceType"] = Device.DeviceType;
            metadata["OperatingSystem"] = Device.OperatingSystem ?? "Unknown";
            metadata["Browser"] = Device.Browser ?? "Unknown";
        }

        // 添加地理位置信息
        if (GeoLocation != null)
        {
            metadata["Country"] = GeoLocation.GetProperty<string>("countryName") ?? "Unknown";
            metadata["Province"] = GeoLocation.GetProperty<string>("provinceName") ?? "Unknown";
            metadata["City"] = GeoLocation.GetProperty<string>("cityName") ?? "Unknown";
        }

        // 添加环境信息
        foreach (var env in EnvironmentInfo)
        {
            if (!metadata.ContainsKey(env.Key))
            {
                metadata[env.Key] = env.Value;
            }
        }

        return metadata;
    }

    /// <summary>
    /// 判断是否为移动设备
    /// </summary>
    public bool IsMobileDevice()
    {
        return Device?.DeviceType == DeviceType.Smartphone || Device?.DeviceType == DeviceType.Tablet;
    }

    /// <summary>
    /// 判断是否为高价值用户
    /// </summary>
    public bool IsHighValueUser()
    {
        if (UserProfile != null)
        {
            return UserProfile.IsHighValueUser;
        }

        // 从定向上下文的实时因子中获取
        return TargetingContext.GetMetadata<bool>("isHighValueUser");
    }

    /// <summary>
    /// 获取定向上下文
    /// </summary>
    public TargetingContext GetTargetingContext()
    {
        return TargetingContext;
    }

    /// <summary>
    /// 更新环境信息
    /// </summary>
    public void UpdateEnvironmentInfo(IDictionary<string, object> environmentInfo)
    {
        if (environmentInfo == null)
            throw new ArgumentNullException(nameof(environmentInfo));

        EnvironmentInfo = environmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加环境信息
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("环境信息键不能为空", nameof(key));

        var envInfo = EnvironmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value);
        envInfo[key] = value;
        EnvironmentInfo = envInfo.AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 获取上下文摘要信息
    /// </summary>
    public Dictionary<string, object> GetContextSummary()
    {
        return new Dictionary<string, object>
        {
            ["RequestId"] = RequestId,
            ["UserId"] = UserId ?? "Anonymous",
            ["RequestTime"] = RequestTime,
            ["TotalPlacements"] = GetAllPlacements().Count,
            ["TotalCandidates"] = GetTotalCandidatesCount(),
            ["HasUserProfile"] = UserProfile != null,
            ["HasDeviceInfo"] = Device != null,
            ["HasLocationInfo"] = GeoLocation != null,
            ["IsMobileDevice"] = IsMobileDevice(),
            ["IsHighValueUser"] = IsHighValueUser(),
            ["TimeSlot"] = GetTimeSlot(),
            ["TargetingContextSummary"] = TargetingContext.GetSummary()
        };
    }

    /// <summary>
    /// 参数验证
    /// </summary>
    private static void ValidateParameters(string requestId, TargetingContext targetingContext)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));
    }

    /// <summary>
    /// 旧版参数验证（兼容性方法）
    /// </summary>
    private static void ValidateLegacyParameters(
        string requestId,
        string? userId,
        string placementId,
        DeviceInfo device,
        GeoInfo geoLocation,
        string userAgent,
        TimeWindow timeWindow,
        AdSize adSize)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

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

















































