namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 媒体资源状态枚举
    /// </summary>
    public enum MediaStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        PendingReview = 1,

        /// <summary>
        /// 已激活
        /// </summary>
        Active = 2,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 3,

        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled = 4,

        /// <summary>
        /// 维护中
        /// </summary>
        Maintenance = 5,
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 6,
        /// <summary>
        /// 已拒绝
        /// </summary>
        Rejected = 7
    }
}
