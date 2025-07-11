using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���������ʵ��
/// ����������AdRequest��������Ϣ��ɵ��������������ģ������λ��֯��ѡ��漯��
/// �������ڣ����󴴽�������ʹ�á����٣�����������������б��ֲ���
/// </summary>
public class AdContext : EntityBase
{
    /// <summary>
    /// ����Ψһ��ʶ
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// �û���ʶ
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// �����λ����ĺ�ѡ��漯��
    /// KeyΪ���λID��ValueΪAdCandidate����
    /// </summary>
    private readonly Dictionary<string, List<AdCandidate>> _candidatesByPlacement = new();
    public IReadOnlyDictionary<string, IReadOnlyList<AdCandidate>> CandidatesByPlacement => 
        _candidatesByPlacement.ToDictionary(
            kv => kv.Key, 
            kv => (IReadOnlyList<AdCandidate>)kv.Value.AsReadOnly()
        ).AsReadOnly();

    /// <summary>
    /// ����������
    /// </summary>
    public TargetingContext TargetingContext { get; private set; }

    /// <summary>
    /// ������Ϣ
    /// ����ʱ�䡢��������Ȼ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; private set; }

    /// <summary>
    /// �豸��Ϣ����TargetingContext��ȡ�Ŀ�ݷ��ʣ�
    /// </summary>
    public DeviceInfo? Device => TargetingContext?.GetTargetingContext<DeviceInfo>("device") ?? TargetingContext?.GetTargetingContext<DeviceInfo>();

    /// <summary>
    /// ����λ����Ϣ����TargetingContext��ȡ�Ŀ�ݷ��ʣ�
    /// </summary>
    public GeoInfo? GeoLocation => TargetingContext?.GetTargetingContext<GeoInfo>("geo") ?? TargetingContext?.GetTargetingContext<GeoInfo>();

    /// <summary>
    /// �û�������Ϣ����TargetingContext��ȡ�Ŀ�ݷ��ʣ�
    /// </summary>
    public UserProfile? UserProfile => TargetingContext?.GetTargetingContext<UserProfile>("user") ?? TargetingContext?.GetTargetingContext<UserProfile>();

    /// <summary>
    /// ˽�й��캯��
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
    /// ������������Ķ���
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
    /// ���������Դ�����������ģ�����ԭ�нӿڣ�
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

        // ����������Ϣ
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

        // ��������������
        var targetingContexts = new Dictionary<string, ITargetingContext>();
        
        if (device != null)
        {
            targetingContexts["device"] = device;
        }
        
        if (geoLocation != null)
        {
            targetingContexts["geo"] = geoLocation;
        }

        // ������û�������ӵ�����������
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
    /// ��ȡָ�����λ�ĺ�ѡ����б�
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
    /// ��Ӻ�ѡ��浽ָ�����λ
    /// </summary>
    public void AddCandidateToPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        // ȷ����ѡ����PlacementId��Ŀ����λһ��
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
    /// ������Ӻ�ѡ��浽ָ�����λ
    /// </summary>
    public void AddCandidatesToPlacement(string placementId, IEnumerable<AdCandidate> candidates)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));

        foreach (var candidate in candidates)
        {
            AddCandidateToPlacement(placementId, candidate);
        }
    }

    /// <summary>
    /// �Ƴ�ָ�����λ�ĺ�ѡ���
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
    /// ���ָ�����λ�ĺ�ѡ���
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
    /// ��ȡ���й��λID�б�
    /// </summary>
    public IReadOnlyList<string> GetAllPlacements()
    {
        return _candidatesByPlacement.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// ��ȡ��ѡ�������
    /// </summary>
    public int GetTotalCandidatesCount()
    {
        return _candidatesByPlacement.Values.Sum(candidates => candidates.Count);
    }

    /// <summary>
    /// ��ȡ�û�ϸ�ֱ�ǩ
    /// </summary>
    public List<string> GetUserSegments()
    {
        var segments = new List<string>();

        if (UserProfile != null)
        {
            segments.AddRange(UserProfile.GetUserSegments());
        }

        // �Ӷ����������л�ȡ��ǩ
        var contextualTags = TargetingContext.GetMetadata<IEnumerable<string>>("contextualTags");
        if (contextualTags != null)
        {
            segments.AddRange(contextualTags);
        }

        return segments.Distinct().ToList();
    }

    /// <summary>
    /// �ж��Ƿ�����Ŀ�����
    /// </summary>
    public bool IsFromTargetRegion(GeoInfo targetGeoLocation)
    {
        if (targetGeoLocation == null || GeoLocation == null)
            return false;

        // ����ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("countryName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("countryName"), 
                          targetGeoLocation.GetProperty<string>("countryName"), 
                          StringComparison.OrdinalIgnoreCase))
            return false;

        // ʡ��ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("provinceName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("provinceName"), 
                          targetGeoLocation.GetProperty<string>("provinceName"), 
                          StringComparison.OrdinalIgnoreCase))
            return false;

        // ����ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.GetProperty<string>("cityName")) &&
            !string.Equals(GeoLocation.GetProperty<string>("cityName"), 
                          targetGeoLocation.GetProperty<string>("cityName"), 
                          StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    /// <summary>
    /// ��ȡʱ���
    /// </summary>
    public string GetTimeSlot()
    {
        var hour = RequestTime.Hour;
        return hour switch
        {
            >= 6 and < 12 => "����",
            >= 12 and < 18 => "����",
            >= 18 and < 24 => "����",
            _ => "�賿"
        };
    }

    /// <summary>
    /// ��ȡ���ڼ�
    /// </summary>
    public DayOfWeek GetDayOfWeek()
    {
        return RequestTime.DayOfWeek;
    }

    /// <summary>
    /// ��ȡ����Ԫ����
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

        // ����豸��Ϣ
        if (Device != null)
        {
            metadata["DeviceType"] = Device.DeviceType;
            metadata["OperatingSystem"] = Device.OperatingSystem ?? "Unknown";
            metadata["Browser"] = Device.Browser ?? "Unknown";
        }

        // ��ӵ���λ����Ϣ
        if (GeoLocation != null)
        {
            metadata["Country"] = GeoLocation.GetProperty<string>("countryName") ?? "Unknown";
            metadata["Province"] = GeoLocation.GetProperty<string>("provinceName") ?? "Unknown";
            metadata["City"] = GeoLocation.GetProperty<string>("cityName") ?? "Unknown";
        }

        // ��ӻ�����Ϣ
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
    /// �ж��Ƿ�Ϊ�ƶ��豸
    /// </summary>
    public bool IsMobileDevice()
    {
        return Device?.DeviceType == DeviceType.Smartphone || Device?.DeviceType == DeviceType.Tablet;
    }

    /// <summary>
    /// �ж��Ƿ�Ϊ�߼�ֵ�û�
    /// </summary>
    public bool IsHighValueUser()
    {
        if (UserProfile != null)
        {
            return UserProfile.IsHighValueUser;
        }

        // �Ӷ��������ĵ�ʵʱ�����л�ȡ
        return TargetingContext.GetMetadata<bool>("isHighValueUser");
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    public TargetingContext GetTargetingContext()
    {
        return TargetingContext;
    }

    /// <summary>
    /// ���»�����Ϣ
    /// </summary>
    public void UpdateEnvironmentInfo(IDictionary<string, object> environmentInfo)
    {
        if (environmentInfo == null)
            throw new ArgumentNullException(nameof(environmentInfo));

        EnvironmentInfo = environmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ӻ�����Ϣ
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("������Ϣ������Ϊ��", nameof(key));

        var envInfo = EnvironmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value);
        envInfo[key] = value;
        EnvironmentInfo = envInfo.AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ȡ������ժҪ��Ϣ
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
    /// ������֤
    /// </summary>
    private static void ValidateParameters(string requestId, TargetingContext targetingContext)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));
    }

    /// <summary>
    /// �ɰ������֤�������Է�����
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
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (device == null)
            throw new ArgumentNullException(nameof(device));

        if (geoLocation == null)
            throw new ArgumentNullException(nameof(geoLocation));

        if (string.IsNullOrEmpty(userAgent))
            throw new ArgumentException("�û�������Ϣ����Ϊ��", nameof(userAgent));

        if (timeWindow == null)
            throw new ArgumentNullException(nameof(timeWindow));

        if (adSize == null)
            throw new ArgumentNullException(nameof(adSize));
    }
}

















































