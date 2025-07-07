using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���������ʵ��
/// ��װ��������������������Ϣ��Ϊ���Ͷ�ž����ṩ��������
/// </summary>
public record AdContext
{
    /// <summary>
    /// �����ʶ
    /// </summary>
    public required string RequestId { get; init; }

    /// <summary>
    /// �û�ID
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// ���λID
    /// </summary>
    public required string PlacementId { get; init; }

    /// <summary>
    /// �豸����
    /// </summary>
    public required string DeviceType { get; init; }

    /// <summary>
    /// ����λ����Ϣ
    /// </summary>
    public required GeoInfo GeoLocation { get; init; }

    /// <summary>
    /// �û������ַ���
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// �豸��Ϣ
    /// </summary>
    public DeviceInfo? DeviceInfo { get; init; }

    /// <summary>
    /// �û�������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> UserProfile { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ����Ԫ����
    /// </summary>
    public IReadOnlyDictionary<string, object> RequestMetadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ý������
    /// </summary>
    public MediaType? MediaType { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public AdType? AdType { get; init; }

    /// <summary>
    /// ������Դ
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// �ỰID
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// �ͻ���IP��ַ
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    /// �Ƽ���Ϣ
    /// </summary>
    public string? Referrer { get; init; }

    /// <summary>
    /// ��ȡ�û��ֶ���Ϣ
    /// </summary>
    /// <returns>�û��ֶ��б�</returns>
    public IReadOnlyList<string> GetUserSegments()
    {
        var segments = new List<string>();
        
        if (UserProfile.TryGetValue("segments", out var segmentData) && segmentData is IEnumerable<string> segmentList)
        {
            segments.AddRange(segmentList);
        }
        
        // �����豸������ӷֶ�
        if (!string.IsNullOrEmpty(DeviceType))
        {
            segments.Add($"device_{DeviceType.ToLowerInvariant()}");
        }
        
        // ���ڵ���λ����ӷֶ�
        if (!string.IsNullOrEmpty(GeoLocation.CountryCode))
        {
            segments.Add($"geo_{GeoLocation.CountryCode.ToLowerInvariant()}");
        }
        
        return segments.AsReadOnly();
    }

    /// <summary>
    /// �ж��Ƿ�����Ŀ�����
    /// </summary>
    /// <param name="targetCountryCode">Ŀ����Ҵ���</param>
    /// <returns>�Ƿ�ƥ��</returns>
    public bool IsFromTargetRegion(string targetCountryCode)
    {
        return string.Equals(GeoLocation.CountryCode, targetCountryCode, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ��ȡʱ�����Ϣ
    /// </summary>
    /// <returns>ʱ���ö��</returns>
    public TimeSlot GetTimeSlot()
    {
        var localTime = RequestTime;
        
        // �����ʱ����Ϣ��ת��Ϊ����ʱ��
        if (!string.IsNullOrEmpty(GeoLocation.TimeZone))
        {
            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(GeoLocation.TimeZone);
                localTime = TimeZoneInfo.ConvertTimeFromUtc(RequestTime, timeZoneInfo);
            }
            catch (TimeZoneNotFoundException)
            {
                // ����Ҳ���ʱ����ʹ��UTCʱ��
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
    /// ��ȡ����Ԫ����
    /// </summary>
    /// <returns>����Ԫ���ݶ���</returns>
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
    /// ����Ƿ�Ϊ��Ч�Ĺ������
    /// </summary>
    /// <returns>�Ƿ���Ч</returns>
    public bool IsValidAdRequest()
    {
        // �����ֶ���֤
        if (string.IsNullOrEmpty(RequestId) || string.IsNullOrEmpty(PlacementId) || string.IsNullOrEmpty(DeviceType))
        {
            return false;
        }

        // ʱ����֤������ʱ�䲻��̫�ɣ�
        if (RequestTime < DateTime.UtcNow.AddHours(-1))
        {
            return false;
        }

        // ����λ����֤
        if (GeoLocation == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    /// <returns>������Ϣ�ֵ�</returns>
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
/// ʱ���ö��
/// </summary>
public enum TimeSlot
{
    /// <summary>
    /// �糿 (6:00-12:00)
    /// </summary>
    Morning = 1,

    /// <summary>
    /// ���� (12:00-18:00)
    /// </summary>
    Afternoon = 2,

    /// <summary>
    /// ���� (18:00-22:00)
    /// </summary>
    Evening = 3,

    /// <summary>
    /// ҹ�� (22:00-6:00)
    /// </summary>
    Night = 4
}

/// <summary>
/// ����Ԫ������Ϣ
/// </summary>
public record RequestMetadataInfo
{
    /// <summary>
    /// ����ID
    /// </summary>
    public required string RequestId { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; init; }

    /// <summary>
    /// ������Դ
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// �û�����
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// �ͻ���IP
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    /// �Ƽ���Ϣ
    /// </summary>
    public string? Referrer { get; init; }

    /// <summary>
    /// �ỰID
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public IReadOnlyDictionary<string, object> AdditionalData { get; init; } = new Dictionary<string, object>();
}




