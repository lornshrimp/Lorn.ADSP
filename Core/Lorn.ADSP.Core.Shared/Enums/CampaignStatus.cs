namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 活动状态枚举
/// </summary>
public enum CampaignStatus
{
    /// <summary>
    /// 草稿状态
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 已计划
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// 运行中
    /// </summary>
    Running = 3,

    /// <summary>
    /// 活跃
    /// </summary>
    Active = 9,

    /// <summary>
    /// 暂停
    /// </summary>
    Paused = 4,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 5,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// 预算耗尽
    /// </summary>
    BudgetExhausted = 7,

    /// <summary>
    /// 已过期
    /// </summary>
    Expired = 8
}