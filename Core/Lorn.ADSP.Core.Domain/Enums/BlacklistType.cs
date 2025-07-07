namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 黑名单类型
    /// </summary>
    public enum BlacklistType
    {
        /// <summary>
        /// 用户黑名单
        /// </summary>
        User = 1,

        /// <summary>
        /// 设备黑名单
        /// </summary>
        Device = 2,

        /// <summary>
        /// IP黑名单
        /// </summary>
        IpAddress = 3,

        /// <summary>
        /// 广告黑名单
        /// </summary>
        Ad = 4,

        /// <summary>
        /// 广告主黑名单
        /// </summary>
        Advertiser = 5,

        /// <summary>
        /// 域名黑名单
        /// </summary>
        Domain = 6,

        /// <summary>
        /// 关键词黑名单
        /// </summary>
        Keyword = 7,

        /// <summary>
        /// 地理位置黑名单
        /// </summary>
        Geo = 8
    }
}
