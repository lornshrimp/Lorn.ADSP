namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 投放模式枚举
/// </summary>
public enum DeliveryMode
{
    /// <summary>
    /// 保量投放（Guaranteed）
    /// </summary>
    Guaranteed = 1,

    /// <summary>
    /// 优先级投放（Priority）
    /// </summary>
    Priority = 2,

    /// <summary>
    /// 竞价投放（Bidding/RTB）
    /// </summary>
    Bidding = 3,

    /// <summary>
    /// 混合投放（Mixed）
    /// </summary>
    Mixed = 4
}