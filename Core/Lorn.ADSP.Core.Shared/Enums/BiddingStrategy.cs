namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 出价策略枚举
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
    /// 目标CPA出价
    /// </summary>
    TargetCPA = 3,

    /// <summary>
    /// 目标ROAS出价
    /// </summary>
    TargetROAS = 4,

    /// <summary>
    /// 最大化点击量
    /// </summary>
    MaximizeClicks = 5,

    /// <summary>
    /// 最大化转化量
    /// </summary>
    MaximizeConversions = 6
}