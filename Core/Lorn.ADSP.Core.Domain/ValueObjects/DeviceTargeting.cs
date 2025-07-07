using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 设备定向
    /// </summary>
    public class DeviceTargeting : ValueObject
    {
        /// <summary>
        /// 设备类型列表
        /// </summary>
        public IReadOnlyList<DeviceType> DeviceTypes { get; private set; }

        /// <summary>
        /// 操作系统列表
        /// </summary>
        public IReadOnlyList<string> OperatingSystems { get; private set; }

        /// <summary>
        /// 浏览器列表
        /// </summary>
        public IReadOnlyList<string> Browsers { get; private set; }

        private DeviceTargeting(
            IReadOnlyList<DeviceType> deviceTypes,
            IReadOnlyList<string> operatingSystems,
            IReadOnlyList<string> browsers)
        {
            DeviceTypes = deviceTypes;
            OperatingSystems = operatingSystems;
            Browsers = browsers;
        }

        public static DeviceTargeting Create(
            IReadOnlyList<DeviceType>? deviceTypes = null,
            IReadOnlyList<string>? operatingSystems = null,
            IReadOnlyList<string>? browsers = null)
        {
            return new DeviceTargeting(
                deviceTypes ?? Array.Empty<DeviceType>(),
                operatingSystems ?? Array.Empty<string>(),
                browsers ?? Array.Empty<string>());
        }

        public bool IsMatch(DeviceInfo deviceInfo)
        {
            if (deviceInfo == null)
                return false;

            // 设备类型匹配
            if (DeviceTypes.Any() && !DeviceTypes.Contains(deviceInfo.DeviceType))
                return false;

            // 操作系统匹配
            if (OperatingSystems.Any() && !string.IsNullOrEmpty(deviceInfo.OperatingSystem))
            {
                if (!OperatingSystems.Any(os =>
                    deviceInfo.OperatingSystem.Contains(os, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }

            return true;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var deviceType in DeviceTypes.OrderBy(x => x))
            {
                yield return deviceType;
            }

            foreach (var os in OperatingSystems.OrderBy(x => x))
            {
                yield return os;
            }

            foreach (var browser in Browsers.OrderBy(x => x))
            {
                yield return browser;
            }
        }

        internal decimal CalculateMatchScore(DeviceInfo? deviceInfo)
        {
            if (deviceInfo == null)
            {
                return 0m; // 如果设备信息为空，返回0分
            }

            decimal score = 0m;

            // 根据设备类型计算分数
            if (DeviceTypes.Contains(deviceInfo.DeviceType))
            {
                score += 0.5m; // 设备类型匹配加0.5分
            }

            // 根据操作系统计算分数
            if (OperatingSystems.Contains(deviceInfo.OperatingSystem))
            {
                score += 0.3m; // 操作系统匹配加0.3分
            }

            // 根据浏览器计算分数
            if (Browsers.Contains(deviceInfo.Browser))
            {
                score += 0.2m; // 浏览器匹配加0.2分
            }

            return score; // 返回计算后的匹配分数
        }
    }
}