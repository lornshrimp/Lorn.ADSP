using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 竞价信息值对象
/// </summary>
public record BiddingInfo
{
    /// <summary>
    /// 竞价策略
    /// </summary>
    public required BiddingStrategy Strategy { get; init; }

    /// <summary>
    /// 基础出价（分）
    /// </summary>
    public required decimal BaseBidPrice { get; init; }

    /// <summary>
    /// 最大出价（分）
    /// </summary>
    public required decimal MaxBidPrice { get; init; }

    /// <summary>
    /// 当前出价（分）
    /// </summary>
    public decimal CurrentBidPrice { get; init; }

    /// <summary>
    /// 出价调整因子
    /// </summary>
    public decimal BidAdjustmentFactor { get; init; } = 1.0m;

    /// <summary>
    /// 预算信息
    /// </summary>
    public BudgetInfo? Budget { get; init; }

    /// <summary>
    /// 竞价标签
    /// </summary>
    public IReadOnlyList<string> BidTags { get; init; } = Array.Empty<string>();
}

/// <summary>
/// 预算信息值对象
/// </summary>
public record BudgetInfo
{
    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; init; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; init; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; init; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// 预算消耗速度（分/小时）
    /// </summary>
    public decimal? BurnRate { get; init; }
}