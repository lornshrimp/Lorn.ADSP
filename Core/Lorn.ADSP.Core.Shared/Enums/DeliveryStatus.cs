namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// Ͷ��״̬ö��
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Ͷ�ųɹ�
    /// </summary>
    Success = 1,

    /// <summary>
    /// Ͷ��ʧ��
    /// </summary>
    Failed = 2,

    /// <summary>
    /// ���ֳɹ�
    /// </summary>
    PartialSuccess = 3,

    /// <summary>
    /// ��ͣ
    /// </summary>
    Paused = 4,

    /// <summary>
    /// ��ʱ
    /// </summary>
    Timeout = 5
}