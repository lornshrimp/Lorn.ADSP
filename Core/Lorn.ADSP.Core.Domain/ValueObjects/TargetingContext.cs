using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 定向上下文
    /// </summary>
    public class TargetingContext
    {
        /// <summary>
        /// 地理位置
        /// </summary>
        public GeoInfo? GeoLocation { get; set; }

        /// <summary>
        /// 用户画像
        /// </summary>
        public UserProfile? UserProfile { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfo? DeviceInfo { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 用户行为
        /// </summary>
        public UserBehavior? UserBehavior { get; set; }
    }
}
