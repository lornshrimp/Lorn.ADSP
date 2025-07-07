using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 频次状态值对象
/// </summary>
public record FrequencyStatus
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// 广告ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// 今日展示次数
    /// </summary>
    public int TodayImpressions { get; init; }

    /// <summary>
    /// 本小时展示次数
    /// </summary>
    public int HourlyImpressions { get; init; }

    /// <summary>
    /// 日频次上限
    /// </summary>
    public int DailyFrequencyCap { get; init; }

    /// <summary>
    /// 小时频次上限
    /// </summary>
    public int HourlyFrequencyCap { get; init; }

    /// <summary>
    /// 全局展示次数
    /// </summary>
    public int TotalImpressions { get; init; }

    /// <summary>
    /// 最后展示时间
    /// </summary>
    public DateTime? LastImpressionTime { get; init; }

    /// <summary>
    /// 是否超出频次限制
    /// </summary>
    public bool IsFrequencyCapExceeded =>
        TodayImpressions >= DailyFrequencyCap ||
        HourlyImpressions >= HourlyFrequencyCap;

    /// <summary>
    /// 可投放次数
    /// </summary>
    public int RemainingImpressions => Math.Min(
        Math.Max(0, DailyFrequencyCap - TodayImpressions),
        Math.Max(0, HourlyFrequencyCap - HourlyImpressions));
}