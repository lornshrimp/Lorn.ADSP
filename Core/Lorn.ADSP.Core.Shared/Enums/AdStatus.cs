namespace Lorn.ADSP.Core.Shared.Enums
{
    /// <summary>
    /// 广告状态
    /// </summary>
    public enum AdStatus
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
        /// 审核中
        /// </summary>
        UnderReview = 3,

        /// <summary>
        /// 已批准
        /// </summary>
        Approved = 4,

        /// <summary>
        /// 已拒绝
        /// </summary>
        Rejected = 5,

        /// <summary>
        /// 活跃
        /// </summary>
        Active = 6,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 7,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped = 8,

        /// <summary>
        /// 已过期
        /// </summary>
        Expired = 9,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 10,

        /// <summary>
        /// 已归档
        /// </summary>
        Archived = 11
    }
}
