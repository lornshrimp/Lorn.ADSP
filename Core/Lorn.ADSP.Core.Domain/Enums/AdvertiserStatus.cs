namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 广告主状态枚举
    /// </summary>
    public enum AdvertiserStatus
    {
        /// <summary>
        /// 待验证
        /// </summary>
        PendingVerification = 1,

        /// <summary>
        /// 已激活
        /// </summary>
        Active = 2,

        /// <summary>
        /// 暂停
        /// </summary>
        Suspended = 3,

        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled = 4,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 5,
        /// <summary>
        /// 等待审核
        /// </summary>
        Pending = 6,
        /// <summary>
        /// 拒绝
        /// </summary>
        Rejected = 7,
        /// <summary>
        /// 在审核中
        /// </summary>
        UnderReview = 8
    }
}
