using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 预算状态值对象
/// </summary>
public class BudgetStatus : ValueObject
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public string CampaignId { get; private set; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; private set; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// 今日已消费预算（分）
    /// </summary>
    public decimal TodaySpent { get; private set; }

    /// <summary>
    /// 今日剩余预算（分）
    /// </summary>
    public decimal TodayRemaining => DailyBudget - TodaySpent;

    /// <summary>
    /// 预算状态
    /// </summary>
    public BudgetStatusType Status { get; private set; }

    /// <summary>
    /// 消耗速度（分/小时）
    /// </summary>
    public decimal? BurnRate { get; private set; }

    /// <summary>
    /// 预计耗尽时间
    /// </summary>
    public DateTime? EstimatedExhaustionTime { get; private set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BudgetStatus()
    {
        CampaignId = string.Empty;
        TotalBudget = 0;
        DailyBudget = 0;
        SpentBudget = 0;
        TodaySpent = 0;
        Status = BudgetStatusType.Active;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BudgetStatus(
        string campaignId,
        decimal totalBudget,
        decimal dailyBudget,
        decimal spentBudget = 0,
        decimal todaySpent = 0,
        BudgetStatusType status = BudgetStatusType.Active,
        decimal? burnRate = null,
        DateTime? estimatedExhaustionTime = null,
        DateTime? lastUpdated = null)
    {
        ValidateInput(campaignId, totalBudget, dailyBudget, spentBudget, todaySpent);

        CampaignId = campaignId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        SpentBudget = spentBudget;
        TodaySpent = todaySpent;
        Status = status;
        BurnRate = burnRate;
        EstimatedExhaustionTime = estimatedExhaustionTime;
        LastUpdated = lastUpdated ?? DateTime.UtcNow;
    }

    /// <summary>
    /// 创建预算状态
    /// </summary>
    public static BudgetStatus Create(
        string campaignId,
        decimal totalBudget,
        decimal dailyBudget)
    {
        return new BudgetStatus(campaignId, totalBudget, dailyBudget);
    }

    /// <summary>
    /// 是否有足够预算
    /// </summary>
    public bool HasSufficientBudget(decimal amount)
    {
        return Status == BudgetStatusType.Active &&
               RemainingBudget >= amount &&
               TodayRemaining >= amount;
    }

    /// <summary>
    /// 消费预算
    /// </summary>
    public BudgetStatus SpendBudget(decimal amount, bool isToday = true)
    {
        if (amount < 0)
            throw new ArgumentException("消费金额不能为负数", nameof(amount));

        if (!HasSufficientBudget(amount))
            throw new InvalidOperationException("预算不足");

        var newSpentBudget = SpentBudget + amount;
        var newTodaySpent = isToday ? TodaySpent + amount : TodaySpent;

        // 自动更新状态
        var newStatus = Status;
        if (newSpentBudget >= TotalBudget)
            newStatus = BudgetStatusType.Exhausted;
        else if (newTodaySpent >= DailyBudget)
            newStatus = BudgetStatusType.OverLimit;

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            newSpentBudget,
            newTodaySpent,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 更新消耗速度
    /// </summary>
    public BudgetStatus UpdateBurnRate(decimal burnRate)
    {
        if (burnRate < 0)
            throw new ArgumentException("消耗速度不能为负数", nameof(burnRate));

        // 计算预计耗尽时间
        DateTime? estimatedExhaustionTime = null;
        if (burnRate > 0 && RemainingBudget > 0)
        {
            var hoursToExhaustion = (double)(RemainingBudget / burnRate);
            estimatedExhaustionTime = DateTime.UtcNow.AddHours(hoursToExhaustion);
        }

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            Status,
            burnRate,
            estimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 重置今日消费
    /// </summary>
    public BudgetStatus ResetDailySpent()
    {
        var newStatus = Status == BudgetStatusType.OverLimit ? BudgetStatusType.Active : Status;

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            0,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public BudgetStatus UpdateStatus(BudgetStatusType status)
    {
        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            status,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 增加预算
    /// </summary>
    public BudgetStatus AddBudget(decimal additionalBudget)
    {
        if (additionalBudget < 0)
            throw new ArgumentException("追加预算不能为负数", nameof(additionalBudget));

        var newTotalBudget = TotalBudget + additionalBudget;
        var newStatus = Status == BudgetStatusType.Exhausted ? BudgetStatusType.Active : Status;

        return new BudgetStatus(
            CampaignId,
            newTotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 预算使用率
    /// </summary>
    public decimal UsageRate => TotalBudget > 0 ? SpentBudget / TotalBudget : 0;

    /// <summary>
    /// 今日预算使用率
    /// </summary>
    public decimal DailyUsageRate => DailyBudget > 0 ? TodaySpent / DailyBudget : 0;

    /// <summary>
    /// 是否预算耗尽
    /// </summary>
    public bool IsExhausted => Status == BudgetStatusType.Exhausted;

    /// <summary>
    /// 是否今日预算耗尽
    /// </summary>
    public bool IsDailyExhausted => Status == BudgetStatusType.OverLimit;

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CampaignId;
        yield return TotalBudget;
        yield return DailyBudget;
        yield return SpentBudget;
        yield return TodaySpent;
        yield return Status;
        yield return BurnRate ?? 0m;
        yield return EstimatedExhaustionTime ?? DateTime.MinValue;
        yield return LastUpdated;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        string campaignId,
        decimal totalBudget,
        decimal dailyBudget,
        decimal spentBudget,
        decimal todaySpent)
    {
        if (string.IsNullOrWhiteSpace(campaignId))
            throw new ArgumentException("活动ID不能为空", nameof(campaignId));

        if (totalBudget < 0)
            throw new ArgumentException("总预算不能为负数", nameof(totalBudget));

        if (dailyBudget < 0)
            throw new ArgumentException("日预算不能为负数", nameof(dailyBudget));

        if (spentBudget < 0)
            throw new ArgumentException("已消费预算不能为负数", nameof(spentBudget));

        if (todaySpent < 0)
            throw new ArgumentException("今日已消费预算不能为负数", nameof(todaySpent));

        if (spentBudget > totalBudget)
            throw new ArgumentException("已消费预算不能超过总预算", nameof(spentBudget));

        if (todaySpent > dailyBudget)
            throw new ArgumentException("今日已消费预算不能超过日预算", nameof(todaySpent));
    }
}