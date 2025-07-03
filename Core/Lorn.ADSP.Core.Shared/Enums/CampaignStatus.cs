namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 广告活动状态枚举
/// </summary>
public enum CampaignStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 待审核
    /// </summary>
    PendingReview = 2,

    /// <summary>
    /// 已激活
    /// </summary>
    Active = 3,

    /// <summary>
    /// 已暂停
    /// </summary>
    Paused = 4,

    /// <summary>
    /// 已结束
    /// </summary>
    Ended = 5,

    /// <summary>
    /// 已删除
    /// </summary>
    Deleted = 6
}