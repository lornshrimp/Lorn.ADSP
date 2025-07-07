using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ������״ֵ̬����
/// </summary>
public record BlacklistStatus
{
    /// <summary>
    /// �Ƿ��ں�������
    /// </summary>
    public bool IsBlacklisted { get; init; }

    /// <summary>
    /// ����������
    /// </summary>
    public IReadOnlyList<BlacklistType> BlacklistTypes { get; init; } = Array.Empty<BlacklistType>();

    /// <summary>
    /// ������ԭ��
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// ���������ʱ��
    /// </summary>
    public DateTime? BlacklistedAt { get; init; }

    /// <summary>
    /// ��������Ч��
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// �Ƿ����ú�����
    /// </summary>
    public bool IsPermanent => !ExpiresAt.HasValue;

    /// <summary>
    /// �Ƿ��ѹ���
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
}