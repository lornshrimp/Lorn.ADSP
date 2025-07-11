using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���������ʵ��
/// ��װ��������������������Ϣ��Ϊ���Ͷ�ž����ṩ��������
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
    public string UserId { get; private set; }

    /// <summary>
    /// ���λ��ʶ
    /// </summary>
    public string PlacementId { get; private set; }

    /// <summary>
    /// �豸��Ϣ
    /// </summary>
    public DeviceInfo Device { get; private set; }

    /// <summary>
    /// ����λ����Ϣ
    /// </summary>
    public GeoInfo GeoLocation { get; private set; }

    /// <summary>
    /// �û�������Ϣ
    /// </summary>
    public string UserAgent { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// ʱ�䴰����Ϣ
    /// </summary>
    public TimeWindow TimeWindow { get; private set; }

    /// <summary>
    /// �û�������Ϣ
    /// </summary>
    public Dictionary<string, object> UserProfile { get; private set; }

    /// <summary>
    /// ������������Ϣ
    /// </summary>
    public Dictionary<string, object> EnvironmentInfo { get; private set; }

    /// <summary>
    /// ������Դ
    /// </summary>
    public RequestSource RequestSource { get; private set; }

    /// <summary>
    /// ���λ�ߴ�
    /// </summary>
    public AdSize AdSize { get; private set; }

    /// <summary>
    /// ˽�й��캯��
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
    /// ������������Ķ���
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
    /// �����û�������Ϣ
    /// </summary>
    public void UpdateUserProfile(Dictionary<string, object> userProfile)
    {
        UserProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����û���������
    /// </summary>
    public void AddUserProfileAttribute(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�û����������Ϊ��", nameof(key));

        UserProfile[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �Ƴ��û���������
    /// </summary>
    public void RemoveUserProfileAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�û����������Ϊ��", nameof(key));

        if (UserProfile.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// ���»�����Ϣ
    /// </summary>
    public void UpdateEnvironmentInfo(Dictionary<string, object> environmentInfo)
    {
        EnvironmentInfo = environmentInfo ?? throw new ArgumentNullException(nameof(environmentInfo));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ӻ�����Ϣ
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("������Ϣ������Ϊ��", nameof(key));

        EnvironmentInfo[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �Ƴ�������Ϣ
    /// </summary>
    public void RemoveEnvironmentInfo(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("������Ϣ������Ϊ��", nameof(key));

        if (EnvironmentInfo.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// ��ȡ�û�ϸ�ֱ�ǩ
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
    /// ����Ƿ�����Ŀ������
    /// </summary>
    public bool IsFromTargetRegion(GeoInfo targetGeoLocation)
    {
        if (targetGeoLocation == null)
            return false;

        // ������ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.CountryName) &&
            !string.Equals(GeoLocation.CountryName, targetGeoLocation.CountryName, StringComparison.OrdinalIgnoreCase))
            return false;

        // ���ʡ��ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.ProvinceName) &&
            !string.Equals(GeoLocation.ProvinceName, targetGeoLocation.ProvinceName, StringComparison.OrdinalIgnoreCase))
            return false;

        // ������ƥ��
        if (!string.IsNullOrEmpty(targetGeoLocation.CityName) &&
            !string.Equals(GeoLocation.CityName, targetGeoLocation.CityName, StringComparison.OrdinalIgnoreCase))
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
    /// ����Ƿ�Ϊ�ƶ��豸
    /// </summary>
    public bool IsMobileDevice()
    {
        return Device.DeviceType == DeviceType.Smartphone || Device.DeviceType == DeviceType.Tablet;
    }

    /// <summary>
    /// ����Ƿ�Ϊ�߼�ֵ�û�
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
    /// ������֤
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
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("�û�ID����Ϊ��", nameof(userId));

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




