namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 投放模式枚举
/// </summary>
public enum DeliveryMode
{
    /// <summary>
    /// 保量投放
    /// </summary>
    Guaranteed = 1,

    /// <summary>
    /// 竞价投放
    /// </summary>
    Bidding = 2,

    /// <summary>
    /// 混合投放
    /// </summary>
    Mixed = 3,

    /// <summary>
    /// 优先级投放
    /// </summary>
    Priority = 4
}