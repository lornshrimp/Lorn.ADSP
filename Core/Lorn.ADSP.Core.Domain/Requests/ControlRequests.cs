using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Requests;

/// <summary>
/// �������������
/// </summary>
public record BlacklistCheckRequest
{
    /// <summary>
    /// �û�ID
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// �豸ID
    /// </summary>
    public string? DeviceId { get; init; }

    /// <summary>
    /// IP��ַ
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// ���ID
    /// </summary>
    public string? AdId { get; init; }

    /// <summary>
    /// �����ID
    /// </summary>
    public string? AdvertiserId { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public IReadOnlyList<BlacklistType> CheckTypes { get; init; } = Array.Empty<BlacklistType>();
}

/// <summary>
/// Ƶ�ο�������
/// </summary>
public record FrequencyControlRequest
{
    /// <summary>
    /// �û�ID
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ���ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; init; } = DateTime.UtcNow;
}