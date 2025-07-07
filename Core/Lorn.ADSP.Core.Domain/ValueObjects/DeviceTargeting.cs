using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// �豸����
    /// </summary>
    public class DeviceTargeting : ValueObject
    {
        /// <summary>
        /// �豸�����б�
        /// </summary>
        public IReadOnlyList<DeviceType> DeviceTypes { get; private set; }

        /// <summary>
        /// ����ϵͳ�б�
        /// </summary>
        public IReadOnlyList<string> OperatingSystems { get; private set; }

        /// <summary>
        /// ������б�
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

            // �豸����ƥ��
            if (DeviceTypes.Any() && !DeviceTypes.Contains(deviceInfo.DeviceType))
                return false;

            // ����ϵͳƥ��
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
                return 0m; // ����豸��ϢΪ�գ�����0��
            }

            decimal score = 0m;

            // �����豸���ͼ������
            if (DeviceTypes.Contains(deviceInfo.DeviceType))
            {
                score += 0.5m; // �豸����ƥ���0.5��
            }

            // ���ݲ���ϵͳ�������
            if (OperatingSystems.Contains(deviceInfo.OperatingSystem))
            {
                score += 0.3m; // ����ϵͳƥ���0.3��
            }

            // ����������������
            if (Browsers.Contains(deviceInfo.Browser))
            {
                score += 0.2m; // �����ƥ���0.2��
            }

            return score; // ���ؼ�����ƥ�����
        }
    }
}