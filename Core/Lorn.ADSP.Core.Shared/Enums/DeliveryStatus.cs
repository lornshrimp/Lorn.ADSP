namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 投放状态枚举
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// 待投放
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已投放
    /// </summary>
    Delivered = 2,

    /// <summary>
    /// 投放成功
    /// </summary>
    Success = 3,

    /// <summary>
    /// 投放失败
    /// </summary>
    Failed = 4,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// 已过期
    /// </summary>
    Expired = 6,

    /// <summary>
    /// 超时
    /// </summary>
    Timeout = 7
}