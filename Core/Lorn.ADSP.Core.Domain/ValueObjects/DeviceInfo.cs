using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public record DeviceInfo
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; init; } = DeviceType.PersonalComputer;

        /// <summary>
        /// 操作系统
        /// </summary>
        public string? OperatingSystem { get; init; }

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string? OSVersion { get; init; }

        /// <summary>
        /// 浏览器
        /// </summary>
        public string? Browser { get; init; }

        /// <summary>
        /// 浏览器版本
        /// </summary>
        public string? BrowserVersion { get; init; }

        /// <summary>
        /// 设备品牌
        /// </summary>
        public string? Brand { get; init; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string? Model { get; init; }

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int? ScreenWidth { get; init; }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int? ScreenHeight { get; init; }

        /// <summary>
        /// 设备像素比
        /// </summary>
        public decimal? DevicePixelRatio { get; init; }

        /// <summary>
        /// 网络类型
        /// </summary>
        public string? NetworkType { get; init; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string? Carrier { get; init; }

        /// <summary>
        /// 设备指纹
        /// </summary>
        public string? DeviceFingerprint { get; init; }

        /// <summary>
        /// 是否支持JavaScript
        /// </summary>
        public bool? JavaScriptEnabled { get; init; }

        /// <summary>
        /// 是否支持Cookie
        /// </summary>
        public bool? CookiesEnabled { get; init; }

        /// <summary>
        /// Flash版本
        /// </summary>
        public string? FlashVersion { get; init; }
    }

}
