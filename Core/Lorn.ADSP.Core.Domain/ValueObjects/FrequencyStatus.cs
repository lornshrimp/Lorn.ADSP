using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 频次状态值对象
/// </summary>
public class FrequencyStatus : ValueObject
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// 广告ID
    /// </summary>
    public string AdId { get; private set; }

    /// <summary>
    /// 今日展示次数
    /// </summary>
    public int TodayImpressions { get; private set; }

    /// <summary>
    /// 本小时展示次数
    /// </summary>
    public int HourlyImpressions { get; private set; }

    /// <summary>
    /// 日频次上限
    /// </summary>
    public int DailyFrequencyCap { get; private set; }

    /// <summary>
    /// 小时频次上限
    /// </summary>
    public int HourlyFrequencyCap { get; private set; }

    /// <summary>
    /// 全局展示次数
    /// </summary>
    public int TotalImpressions { get; private set; }

    /// <summary>
    /// 最后展示时间
    /// </summary>
    public DateTime? LastImpressionTime { get; private set; }

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

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private FrequencyStatus()
    {
        UserId = string.Empty;
        AdId = string.Empty;
        TodayImpressions = 0;
        HourlyImpressions = 0;
        DailyFrequencyCap = 0;
        HourlyFrequencyCap = 0;
        TotalImpressions = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public FrequencyStatus(
        string userId,
        string adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap,
        int todayImpressions = 0,
        int hourlyImpressions = 0,
        int totalImpressions = 0,
        DateTime? lastImpressionTime = null)
    {
        ValidateInput(userId, adId, dailyFrequencyCap, hourlyFrequencyCap, todayImpressions, hourlyImpressions, totalImpressions);

        UserId = userId;
        AdId = adId;
        DailyFrequencyCap = dailyFrequencyCap;
        HourlyFrequencyCap = hourlyFrequencyCap;
        TodayImpressions = todayImpressions;
        HourlyImpressions = hourlyImpressions;
        TotalImpressions = totalImpressions;
        LastImpressionTime = lastImpressionTime;
    }

    /// <summary>
    /// 创建频次状态
    /// </summary>
    public static FrequencyStatus Create(
        string userId,
        string adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap)
    {
        return new FrequencyStatus(userId, adId, dailyFrequencyCap, hourlyFrequencyCap);
    }

    /// <summary>
    /// 记录展示
    /// </summary>
    public FrequencyStatus RecordImpression(DateTime? impressionTime = null)
    {
        var currentTime = impressionTime ?? DateTime.UtcNow;
        var lastTime = LastImpressionTime ?? DateTime.MinValue;

        // 判断是否需要重置计数器
        var newTodayImpressions = TodayImpressions;
        var newHourlyImpressions = HourlyImpressions;

        if (lastTime.Date != currentTime.Date)
        {
            // 跨天重置日计数
            newTodayImpressions = 0;
            newHourlyImpressions = 0;
        }
        else if (lastTime.Hour != currentTime.Hour)
        {
            // 跨小时重置小时计数
            newHourlyImpressions = 0;
        }

        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            newTodayImpressions + 1,
            newHourlyImpressions + 1,
            TotalImpressions + 1,
            currentTime);
    }

    /// <summary>
    /// 更新频次限制
    /// </summary>
    public FrequencyStatus UpdateFrequencyCaps(int dailyFrequencyCap, int hourlyFrequencyCap)
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            dailyFrequencyCap,
            hourlyFrequencyCap,
            TodayImpressions,
            HourlyImpressions,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// 重置今日计数
    /// </summary>
    public FrequencyStatus ResetDailyCount()
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            0,
            0,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// 重置小时计数
    /// </summary>
    public FrequencyStatus ResetHourlyCount()
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            TodayImpressions,
            0,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// 是否可以投放
    /// </summary>
    public bool CanServe => !IsFrequencyCapExceeded;

    /// <summary>
    /// 日频次使用率
    /// </summary>
    public decimal DailyUsageRate => DailyFrequencyCap > 0 ? (decimal)TodayImpressions / DailyFrequencyCap : 0;

    /// <summary>
    /// 小时频次使用率
    /// </summary>
    public decimal HourlyUsageRate => HourlyFrequencyCap > 0 ? (decimal)HourlyImpressions / HourlyFrequencyCap : 0;

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return AdId;
        yield return TodayImpressions;
        yield return HourlyImpressions;
        yield return DailyFrequencyCap;
        yield return HourlyFrequencyCap;
        yield return TotalImpressions;
        yield return LastImpressionTime ?? DateTime.MinValue;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        string userId,
        string adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap,
        int todayImpressions,
        int hourlyImpressions,
        int totalImpressions)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("用户ID不能为空", nameof(userId));

        if (string.IsNullOrWhiteSpace(adId))
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (dailyFrequencyCap < 0)
            throw new ArgumentException("日频次上限不能为负数", nameof(dailyFrequencyCap));

        if (hourlyFrequencyCap < 0)
            throw new ArgumentException("小时频次上限不能为负数", nameof(hourlyFrequencyCap));

        if (todayImpressions < 0)
            throw new ArgumentException("今日展示次数不能为负数", nameof(todayImpressions));

        if (hourlyImpressions < 0)
            throw new ArgumentException("本小时展示次数不能为负数", nameof(hourlyImpressions));

        if (totalImpressions < 0)
            throw new ArgumentException("全局展示次数不能为负数", nameof(totalImpressions));

        if (hourlyFrequencyCap > dailyFrequencyCap && dailyFrequencyCap > 0)
            throw new ArgumentException("小时频次上限不能大于日频次上限");
    }
}