using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 预算状态值对象
/// </summary>
public record BudgetStatus
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; init; }

    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; init; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; init; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// 今日已消费预算（分）
    /// </summary>
    public decimal TodaySpent { get; init; }

    /// <summary>
    /// 今日剩余预算（分）
    /// </summary>
    public decimal TodayRemaining => DailyBudget - TodaySpent;

    /// <summary>
    /// 预算状态
    /// </summary>
    public BudgetStatusType Status { get; init; } = BudgetStatusType.Active;

    /// <summary>
    /// 消耗速度（分/小时）
    /// </summary>
    public decimal? BurnRate { get; init; }

    /// <summary>
    /// 预计耗尽时间
    /// </summary>
    public DateTime? EstimatedExhaustionTime { get; init; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 是否有足够预算
    /// </summary>
    public bool HasSufficientBudget(decimal amount)
    {
        return Status == BudgetStatusType.Active && 
               RemainingBudget >= amount && 
               TodayRemaining >= amount;
    }
}