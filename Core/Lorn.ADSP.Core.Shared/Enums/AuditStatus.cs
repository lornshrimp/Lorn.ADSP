namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 审核状态枚举
/// </summary>
public enum AuditStatus
{
    /// <summary>
    /// 待审核
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 审核中
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// 审核通过
    /// </summary>
    Approved = 3,

    /// <summary>
    /// 审核拒绝
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// 需要修改
    /// </summary>
    RequiresChanges = 5,

    /// <summary>
    /// 已过期
    /// </summary>
    Expired = 6
}