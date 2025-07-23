using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 设备信息定向上下文
    /// 继承自TargetingContextBase，提供设备信息数据的定向上下文功能
    /// </summary>
    public class DeviceInfo : TargetingContextBase
    {
        /// <summary>
        /// 上下文名称
        /// </summary>
        public override string ContextName => "设备信息上下文";

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType => GetPropertyValue("DeviceType", DeviceType.PersonalComputer);

        /// <summary>
        /// 操作系统
        /// </summary>
        public string? OperatingSystem => GetPropertyValue<string>("OperatingSystem");

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string? OSVersion => GetPropertyValue<string>("OSVersion");

        /// <summary>
        /// 浏览器
        /// </summary>
        public string? Browser => GetPropertyValue<string>("Browser");

        /// <summary>
        /// 浏览器版本
        /// </summary>
        public string? BrowserVersion => GetPropertyValue<string>("BrowserVersion");

        /// <summary>
        /// 设备品牌
        /// </summary>
        public string? Brand => GetPropertyValue<string>("Brand");

        /// <summary>
        /// 设备型号
        /// </summary>
        public string? Model => GetPropertyValue<string>("Model");

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int? ScreenWidth => GetPropertyValue<int?>("ScreenWidth");

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int? ScreenHeight => GetPropertyValue<int?>("ScreenHeight");

        /// <summary>
        /// 设备像素比
        /// </summary>
        public decimal? DevicePixelRatio => GetPropertyValue<decimal?>("DevicePixelRatio");

        /// <summary>
        /// 网络类型
        /// </summary>
        public string? NetworkType => GetPropertyValue<string>("NetworkType");

        /// <summary>
        /// 运营商
        /// </summary>
        public string? Carrier => GetPropertyValue<string>("Carrier");

        /// <summary>
        /// 设备指纹
        /// </summary>
        public string? DeviceFingerprint => GetPropertyValue<string>("DeviceFingerprint");

        /// <summary>
        /// 是否支持JavaScript
        /// </summary>
        public bool? JavaScriptEnabled => GetPropertyValue<bool?>("JavaScriptEnabled");

        /// <summary>
        /// 是否支持Cookie
        /// </summary>
        public bool? CookiesEnabled => GetPropertyValue<bool?>("CookiesEnabled");

        /// <summary>
        /// Flash版本
        /// </summary>
        public string? FlashVersion => GetPropertyValue<string>("FlashVersion");

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
        private DeviceInfo() : base("Device") { }

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
            string? flashVersion = null,
            string? dataSource = null)
            : base("Device", CreateProperties(deviceType, operatingSystem, osVersion, browser, browserVersion, brand, model, screenWidth, screenHeight, devicePixelRatio, networkType, carrier, deviceFingerprint, javaScriptEnabled, cookiesEnabled, flashVersion), dataSource)
        {
            ValidateInput(screenWidth, screenHeight, devicePixelRatio);
        }

        /// <summary>
        /// 创建属性字典
        /// </summary>
        private static Dictionary<string, object> CreateProperties(
            DeviceType deviceType,
            string? operatingSystem,
            string? osVersion,
            string? browser,
            string? browserVersion,
            string? brand,
            string? model,
            int? screenWidth,
            int? screenHeight,
            decimal? devicePixelRatio,
            string? networkType,
            string? carrier,
            string? deviceFingerprint,
            bool? javaScriptEnabled,
            bool? cookiesEnabled,
            string? flashVersion)
        {
            var properties = new Dictionary<string, object>
            {
                ["DeviceType"] = deviceType
            };

            if (!string.IsNullOrWhiteSpace(operatingSystem))
                properties["OperatingSystem"] = operatingSystem;

            if (!string.IsNullOrWhiteSpace(osVersion))
                properties["OSVersion"] = osVersion;

            if (!string.IsNullOrWhiteSpace(browser))
                properties["Browser"] = browser;

            if (!string.IsNullOrWhiteSpace(browserVersion))
                properties["BrowserVersion"] = browserVersion;

            if (!string.IsNullOrWhiteSpace(brand))
                properties["Brand"] = brand;

            if (!string.IsNullOrWhiteSpace(model))
                properties["Model"] = model;

            if (screenWidth.HasValue)
                properties["ScreenWidth"] = screenWidth.Value;

            if (screenHeight.HasValue)
                properties["ScreenHeight"] = screenHeight.Value;

            if (devicePixelRatio.HasValue)
                properties["DevicePixelRatio"] = devicePixelRatio.Value;

            if (!string.IsNullOrWhiteSpace(networkType))
                properties["NetworkType"] = networkType;

            if (!string.IsNullOrWhiteSpace(carrier))
                properties["Carrier"] = carrier;

            if (!string.IsNullOrWhiteSpace(deviceFingerprint))
                properties["DeviceFingerprint"] = deviceFingerprint;

            if (javaScriptEnabled.HasValue)
                properties["JavaScriptEnabled"] = javaScriptEnabled.Value;

            if (cookiesEnabled.HasValue)
                properties["CookiesEnabled"] = cookiesEnabled.Value;

            if (!string.IsNullOrWhiteSpace(flashVersion))
                properties["FlashVersion"] = flashVersion;

            return properties;
        }

        /// <summary>
        /// 创建设备信息
        /// </summary>
        public static DeviceInfo Create(
            DeviceType deviceType,
            string? operatingSystem = null,
            string? browser = null,
            string? dataSource = null)
        {
            return new DeviceInfo(deviceType, operatingSystem, browser: browser, dataSource: dataSource);
        }

        /// <summary>
        /// 创建移动设备信息
        /// </summary>
        public static DeviceInfo CreateMobile(
            string? operatingSystem = null,
            string? brand = null,
            string? model = null,
            int? screenWidth = null,
            int? screenHeight = null,
            string? dataSource = null)
        {
            return new DeviceInfo(
                DeviceType.Smartphone,
                operatingSystem,
                brand: brand,
                model: model,
                screenWidth: screenWidth,
                screenHeight: screenHeight,
                dataSource: dataSource);
        }

        /// <summary>
        /// 创建桌面设备信息
        /// </summary>
        public static DeviceInfo CreateDesktop(
            string? operatingSystem = null,
            string? browser = null,
            string? browserVersion = null,
            int? screenWidth = null,
            int? screenHeight = null,
            string? dataSource = null)
        {
            return new DeviceInfo(
                DeviceType.PersonalComputer,
                operatingSystem,
                browser: browser,
                browserVersion: browserVersion,
                screenWidth: screenWidth,
                screenHeight: screenHeight,
                dataSource: dataSource);
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
                FlashVersion,
                DataSource);
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
                FlashVersion,
                DataSource);
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
                FlashVersion,
                DataSource);
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
                FlashVersion,
                DataSource);
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
                flashVersion,
                DataSource);
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
        /// 获取调试信息
        /// </summary>
        public override string GetDebugInfo()
        {
            var baseInfo = base.GetDebugInfo();
            var deviceInfo = $"Type:{DeviceType} OS:{OperatingSystem} Browser:{Browser} Mobile:{IsMobileDevice} Resolution:{ScreenResolution}";
            return $"{baseInfo} | {deviceInfo}";
        }

        /// <summary>
        /// 验证上下文的有效性
        /// </summary>
        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            // 验证屏幕尺寸
            if (ScreenWidth.HasValue && ScreenWidth <= 0)
                return false;

            if (ScreenHeight.HasValue && ScreenHeight <= 0)
                return false;

            if (DevicePixelRatio.HasValue && DevicePixelRatio <= 0)
                return false;

            return true;
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
