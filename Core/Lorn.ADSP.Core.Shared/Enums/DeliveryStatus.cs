namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 投放状态枚举
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// 投放成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 投放失败
    /// </summary>
    Failed = 2,

    /// <summary>
    /// 部分成功
    /// </summary>
    PartialSuccess = 3,

    /// <summary>
    /// 暂停
    /// </summary>
    Paused = 4,

    /// <summary>
    /// 超时
    /// </summary>
    Timeout = 5
}