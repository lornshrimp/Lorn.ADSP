namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 竞价策略枚举
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
    /// 目标CPA（每次获客成本）
    /// </summary>
    TargetCPA = 3,

    /// <summary>
    /// 目标ROAS（广告支出回报率）
    /// </summary>
    TargetROAS = 4,

    /// <summary>
    /// 最大化点击
    /// </summary>
    MaximizeClicks = 5,

    /// <summary>
    /// 最大化转化
    /// </summary>
    MaximizeConversions = 6,

    /// <summary>
    /// 最大化转化价值
    /// </summary>
    MaximizeConversionValue = 7
}