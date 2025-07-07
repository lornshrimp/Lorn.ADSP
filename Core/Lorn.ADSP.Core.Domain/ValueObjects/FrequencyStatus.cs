using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// Ƶ��״ֵ̬����
/// </summary>
public record FrequencyStatus
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
    /// ����չʾ����
    /// </summary>
    public int TodayImpressions { get; init; }

    /// <summary>
    /// ��Сʱչʾ����
    /// </summary>
    public int HourlyImpressions { get; init; }

    /// <summary>
    /// ��Ƶ������
    /// </summary>
    public int DailyFrequencyCap { get; init; }

    /// <summary>
    /// СʱƵ������
    /// </summary>
    public int HourlyFrequencyCap { get; init; }

    /// <summary>
    /// ȫ��չʾ����
    /// </summary>
    public int TotalImpressions { get; init; }

    /// <summary>
    /// ���չʾʱ��
    /// </summary>
    public DateTime? LastImpressionTime { get; init; }

    /// <summary>
    /// �Ƿ񳬳�Ƶ������
    /// </summary>
    public bool IsFrequencyCapExceeded =>
        TodayImpressions >= DailyFrequencyCap ||
        HourlyImpressions >= HourlyFrequencyCap;

    /// <summary>
    /// ��Ͷ�Ŵ���
    /// </summary>
    public int RemainingImpressions => Math.Min(
        Math.Max(0, DailyFrequencyCap - TodayImpressions),
        Math.Max(0, HourlyFrequencyCap - HourlyImpressions));
}