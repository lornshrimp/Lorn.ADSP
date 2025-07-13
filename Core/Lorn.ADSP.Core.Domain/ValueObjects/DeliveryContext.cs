using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放上下文值对象
    /// </summary>
    public class DeliveryContext : ValueObject
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string? UserId { get; private set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string? DeviceId { get; private set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string? IpAddress { get; private set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; private set; }

        /// <summary>
        /// 引用页面
        /// </summary>
        public string? Referrer { get; private set; }

        /// <summary>
        /// 页面URL
        /// </summary>
        public string? PageUrl { get; private set; }

        /// <summary>
        /// 地理位置
        /// </summary>
        public string? GeoLocation { get; private set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string? Language { get; private set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType? DeviceType { get; private set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public MediaType MediaType { get; private set; }

        /// <summary>
        /// 广告位ID
        /// </summary>
        public string? PlacementId { get; private set; }

        /// <summary>
        /// 广告位尺寸
        /// </summary>
        public string? AdSize { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private DeliveryContext() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeliveryContext(
            MediaType mediaType,
            string? userId = null,
            string? deviceId = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? referrer = null,
            string? pageUrl = null,
            string? geoLocation = null,
            string? language = null,
            DeviceType? deviceType = null,
            string? placementId = null,
            string? adSize = null)
        {
            MediaType = mediaType;
            UserId = userId;
            DeviceId = deviceId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Referrer = referrer;
            PageUrl = pageUrl;
            GeoLocation = geoLocation;
            Language = language;
            DeviceType = deviceType;
            PlacementId = placementId;
            AdSize = adSize;
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId ?? string.Empty;
            yield return DeviceId ?? string.Empty;
            yield return IpAddress ?? string.Empty;
            yield return UserAgent ?? string.Empty;
            yield return Referrer ?? string.Empty;
            yield return PageUrl ?? string.Empty;
            yield return GeoLocation ?? string.Empty;
            yield return Language ?? string.Empty;
            yield return DeviceType ?? Shared.Enums.DeviceType.PersonalComputer;
            yield return MediaType;
            yield return PlacementId ?? string.Empty;
            yield return AdSize ?? string.Empty;
        }
    }
}
