using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo : ValueObject
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; private set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string? OperatingSystem { get; private set; }

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string? OSVersion { get; private set; }

        /// <summary>
        /// 浏览器
        /// </summary>
        public string? Browser { get; private set; }

        /// <summary>
        /// 浏览器版本
        /// </summary>
        public string? BrowserVersion { get; private set; }

        /// <summary>
        /// 设备品牌
        /// </summary>
        public string? Brand { get; private set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string? Model { get; private set; }

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int? ScreenWidth { get; private set; }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int? ScreenHeight { get; private set; }

        /// <summary>
        /// 设备像素比
        /// </summary>
        public decimal? DevicePixelRatio { get; private set; }

        /// <summary>
        /// 网络类型
        /// </summary>
        public string? NetworkType { get; private set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string? Carrier { get; private set; }

        /// <summary>
        /// 设备指纹
        /// </summary>
        public string? DeviceFingerprint { get; private set; }

        /// <summary>
        /// 是否支持JavaScript
        /// </summary>
        public bool? JavaScriptEnabled { get; private set; }

        /// <summary>
        /// 是否支持Cookie
        /// </summary>
        public bool? CookiesEnabled { get; private set; }

        /// <summary>
        /// Flash版本
        /// </summary>
        public string? FlashVersion { get; private set; }

        /// <summary>
        /// 是否移动设备
        /// </summary>
        public bool IsMobileDevice => DeviceType == DeviceType.Smartphone || DeviceType == DeviceType.Tablet;

        /// <summary>
        /// 是否桌面设备
        /// </summary>
        public bool IsDesktopDevice => DeviceType == DeviceType.PersonalComputer;

        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public string? ScreenResolution
        {
            get
            {
                if (ScreenWidth.HasValue && ScreenHeight.HasValue)
                    return $"{ScreenWidth}x{ScreenHeight}";
                return null;
            }
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private DeviceInfo()
        {
            DeviceType = DeviceType.PersonalComputer;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceInfo(
            DeviceType deviceType = DeviceType.PersonalComputer,
            string? operatingSystem = null,
            string? osVersion = null,
            string? browser = null,
            string? browserVersion = null,
            string? brand = null,
            string? model = null,
            int? screenWidth = null,
            int? screenHeight = null,
            decimal? devicePixelRatio = null,
            string? networkType = null,
            string? carrier = null,
            string? deviceFingerprint = null,
            bool? javaScriptEnabled = null,
            bool? cookiesEnabled = null,
            string? flashVersion = null)
        {
            ValidateInput(screenWidth, screenHeight, devicePixelRatio);

            DeviceType = deviceType;
            OperatingSystem = operatingSystem;
            OSVersion = osVersion;
            Browser = browser;
            BrowserVersion = browserVersion;
            Brand = brand;
            Model = model;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            DevicePixelRatio = devicePixelRatio;
            NetworkType = networkType;
            Carrier = carrier;
            DeviceFingerprint = deviceFingerprint;
            JavaScriptEnabled = javaScriptEnabled;
            CookiesEnabled = cookiesEnabled;
            FlashVersion = flashVersion;
        }

        /// <summary>
        /// 创建设备信息
        /// </summary>
        public static DeviceInfo Create(
            DeviceType deviceType,
            string? operatingSystem = null,
            string? browser = null)
        {
            return new DeviceInfo(deviceType, operatingSystem, browser: browser);
        }

        /// <summary>
        /// 创建移动设备信息
        /// </summary>
        public static DeviceInfo CreateMobile(
            string? operatingSystem = null,
            string? brand = null,
            string? model = null,
            int? screenWidth = null,
            int? screenHeight = null)
        {
            return new DeviceInfo(
                DeviceType.Smartphone,
                operatingSystem,
                brand: brand,
                model: model,
                screenWidth: screenWidth,
                screenHeight: screenHeight);
        }

        /// <summary>
        /// 创建桌面设备信息
        /// </summary>
        public static DeviceInfo CreateDesktop(
            string? operatingSystem = null,
            string? browser = null,
            string? browserVersion = null,
            int? screenWidth = null,
            int? screenHeight = null)
        {
            return new DeviceInfo(
                DeviceType.PersonalComputer,
                operatingSystem,
                browser: browser,
                browserVersion: browserVersion,
                screenWidth: screenWidth,
                screenHeight: screenHeight);
        }

        /// <summary>
        /// 设置屏幕信息
        /// </summary>
        public DeviceInfo WithScreenInfo(int screenWidth, int screenHeight, decimal? devicePixelRatio = null)
        {
            return new DeviceInfo(
                DeviceType,
                OperatingSystem,
                OSVersion,
                Browser,
                BrowserVersion,
                Brand,
                Model,
                screenWidth,
                screenHeight,
                devicePixelRatio,
                NetworkType,
                Carrier,
                DeviceFingerprint,
                JavaScriptEnabled,
                CookiesEnabled,
                FlashVersion);
        }

        /// <summary>
        /// 设置浏览器信息
        /// </summary>
        public DeviceInfo WithBrowserInfo(string browser, string? browserVersion = null)
        {
            return new DeviceInfo(
                DeviceType,
                OperatingSystem,
                OSVersion,
                browser,
                browserVersion,
                Brand,
                Model,
                ScreenWidth,
                ScreenHeight,
                DevicePixelRatio,
                NetworkType,
                Carrier,
                DeviceFingerprint,
                JavaScriptEnabled,
                CookiesEnabled,
                FlashVersion);
        }

        /// <summary>
        /// 设置网络信息
        /// </summary>
        public DeviceInfo WithNetworkInfo(string? networkType = null, string? carrier = null)
        {
            return new DeviceInfo(
                DeviceType,
                OperatingSystem,
                OSVersion,
                Browser,
                BrowserVersion,
                Brand,
                Model,
                ScreenWidth,
                ScreenHeight,
                DevicePixelRatio,
                networkType,
                carrier,
                DeviceFingerprint,
                JavaScriptEnabled,
                CookiesEnabled,
                FlashVersion);
        }

        /// <summary>
        /// 设置设备指纹
        /// </summary>
        public DeviceInfo WithFingerprint(string deviceFingerprint)
        {
            return new DeviceInfo(
                DeviceType,
                OperatingSystem,
                OSVersion,
                Browser,
                BrowserVersion,
                Brand,
                Model,
                ScreenWidth,
                ScreenHeight,
                DevicePixelRatio,
                NetworkType,
                Carrier,
                deviceFingerprint,
                JavaScriptEnabled,
                CookiesEnabled,
                FlashVersion);
        }

        /// <summary>
        /// 设置功能支持信息
        /// </summary>
        public DeviceInfo WithCapabilities(bool? javaScriptEnabled = null, bool? cookiesEnabled = null, string? flashVersion = null)
        {
            return new DeviceInfo(
                DeviceType,
                OperatingSystem,
                OSVersion,
                Browser,
                BrowserVersion,
                Brand,
                Model,
                ScreenWidth,
                ScreenHeight,
                DevicePixelRatio,
                NetworkType,
                Carrier,
                DeviceFingerprint,
                javaScriptEnabled,
                cookiesEnabled,
                flashVersion);
        }

        /// <summary>
        /// 是否为高分辨率设备
        /// </summary>
        public bool IsHighDensityDisplay => DevicePixelRatio >= 2.0m;

        /// <summary>
        /// 是否为大屏设备
        /// </summary>
        public bool IsLargeScreen
        {
            get
            {
                if (!ScreenWidth.HasValue || !ScreenHeight.HasValue)
                    return false;

                var minDimension = Math.Min(ScreenWidth.Value, ScreenHeight.Value);
                return minDimension >= 768; // 平板及以上尺寸
            }
        }

        /// <summary>
        /// 获取设备描述
        /// </summary>
        public string GetDeviceDescription()
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(Brand))
                parts.Add(Brand);

            if (!string.IsNullOrEmpty(Model))
                parts.Add(Model);

            if (!string.IsNullOrEmpty(OperatingSystem))
                parts.Add(OperatingSystem);

            if (!string.IsNullOrEmpty(OSVersion))
                parts.Add(OSVersion);

            return parts.Count > 0 ? string.Join(" ", parts) : DeviceType.ToString();
        }

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DeviceType;
            yield return OperatingSystem ?? string.Empty;
            yield return OSVersion ?? string.Empty;
            yield return Browser ?? string.Empty;
            yield return BrowserVersion ?? string.Empty;
            yield return Brand ?? string.Empty;
            yield return Model ?? string.Empty;
            yield return ScreenWidth ?? 0;
            yield return ScreenHeight ?? 0;
            yield return DevicePixelRatio ?? 0m;
            yield return NetworkType ?? string.Empty;
            yield return Carrier ?? string.Empty;
            yield return DeviceFingerprint ?? string.Empty;
            yield return JavaScriptEnabled ?? false;
            yield return CookiesEnabled ?? false;
            yield return FlashVersion ?? string.Empty;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(int? screenWidth, int? screenHeight, decimal? devicePixelRatio)
        {
            if (screenWidth.HasValue && screenWidth <= 0)
                throw new ArgumentException("屏幕宽度必须大于0", nameof(screenWidth));

            if (screenHeight.HasValue && screenHeight <= 0)
                throw new ArgumentException("屏幕高度必须大于0", nameof(screenHeight));

            if (devicePixelRatio.HasValue && devicePixelRatio <= 0)
                throw new ArgumentException("设备像素比必须大于0", nameof(devicePixelRatio));
        }
    }
}
