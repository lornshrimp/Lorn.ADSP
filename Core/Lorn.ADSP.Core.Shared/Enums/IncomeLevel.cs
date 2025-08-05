namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 收入水平枚举
/// </summary>
public enum IncomeLevel
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 低收入 (0-5万)
    /// </summary>
    Low = 1,

    /// <summary>
    /// 中低收入 (5-10万)
    /// </summary>
    LowerMiddle = 2,

    /// <summary>
    /// 中等收入 (10-20万)
    /// </summary>
    Middle = 3,

    /// <summary>
    /// 中高收入 (20-50万)
    /// </summary>
    UpperMiddle = 4,

    /// <summary>
    /// 高收入 (50万+)
    /// </summary>
    High = 5
}