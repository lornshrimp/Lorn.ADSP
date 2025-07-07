namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 竞价策略
/// </summary>
public enum BiddingStrategy
{
    /// <summary>
    /// 固定出价
    /// </summary>
    FixedBid = 1,

    /// <summary>
    /// 自动出价
    /// </summary>
    AutoBid = 2,

    /// <summary>
    /// 目标CPA
    /// </summary>
    TargetCpa = 3,

    /// <summary>
    /// 目标CPC
    /// </summary>
    TargetCpc = 4,

    /// <summary>
    /// 目标CPM
    /// </summary>
    TargetCpm = 5,

    /// <summary>
    /// 目标ROAS
    /// </summary>
    TargetRoas = 6,

    /// <summary>
    /// 最大化点击
    /// </summary>
    MaximizeClicks = 7,

    /// <summary>
    /// 最大化转化
    /// </summary>
    MaximizeConversions = 8,

    /// <summary>
    /// 最大化收入
    /// </summary>
    MaximizeRevenue = 9,
    /// <summary>
    /// 目标CPA
    /// </summary>
    TargetCPA = 10,
    /// <summary>
    /// 目标ROAS
    /// </summary>
    TargetROAS = 11
}