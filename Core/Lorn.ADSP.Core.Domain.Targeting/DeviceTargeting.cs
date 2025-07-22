using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 设备定向条件
    /// 实现基于设备属性的定向规则配置
    /// </summary>
    public class DeviceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Device";

        /// <summary>
        /// 设备类型列表
        /// </summary>
        public IReadOnlyList<DeviceType> DeviceTypes => GetRule<List<DeviceType>>("DeviceTypes") ?? new List<DeviceType>();

        /// <summary>
        /// 操作系统列表
        /// </summary>
        public IReadOnlyList<string> OperatingSystems => GetRule<List<string>>("OperatingSystems") ?? new List<string>();

        /// <summary>
        /// 浏览器列表
        /// </summary>
        public IReadOnlyList<string> Browsers => GetRule<List<string>>("Browsers") ?? new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceTargeting(
            IList<DeviceType>? deviceTypes = null,
            IList<string>? operatingSystems = null,
            IList<string>? browsers = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(deviceTypes, operatingSystems, browsers), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<DeviceType>? deviceTypes,
            IList<string>? operatingSystems,
            IList<string>? browsers)
        {
            var rules = new List<TargetingRule>();

            // 添加设备类型列表
            var deviceTypesList = deviceTypes?.ToList() ?? new List<DeviceType>();
            var deviceTypesRule = new TargetingRule("DeviceTypes", string.Empty, "Json").WithValue(deviceTypesList);
            rules.Add(deviceTypesRule);

            // 添加操作系统列表
            var operatingSystemsList = operatingSystems?.ToList() ?? new List<string>();
            var operatingSystemsRule = new TargetingRule("OperatingSystems", string.Empty, "Json").WithValue(operatingSystemsList);
            rules.Add(operatingSystemsRule);

            // 添加浏览器列表
            var browsersList = browsers?.ToList() ?? new List<string>();
            var browsersRule = new TargetingRule("Browsers", string.Empty, "Json").WithValue(browsersList);
            rules.Add(browsersRule);

            return rules;
        }

        /// <summary>
        /// 创建设备定向条件
        /// </summary>
        public static DeviceTargeting Create(
            IList<DeviceType>? deviceTypes = null,
            IList<string>? operatingSystems = null,
            IList<string>? browsers = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new DeviceTargeting(deviceTypes, operatingSystems, browsers, weight, isEnabled);
        }

        /// <summary>
        /// 添加设备类型
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        public void AddDeviceType(DeviceType deviceType)
        {
            var currentList = DeviceTypes.ToList();
            if (!currentList.Contains(deviceType))
            {
                currentList.Add(deviceType);
                SetRule("DeviceTypes", currentList);
            }
        }

        /// <summary>
        /// 添加操作系统
        /// </summary>
        /// <param name="operatingSystem">操作系统</param>
        public void AddOperatingSystem(string operatingSystem)
        {
            if (string.IsNullOrEmpty(operatingSystem))
                throw new ArgumentException("操作系统不能为空", nameof(operatingSystem));

            var currentList = OperatingSystems.ToList();
            if (!currentList.Contains(operatingSystem))
            {
                currentList.Add(operatingSystem);
                SetRule("OperatingSystems", currentList);
            }
        }

        /// <summary>
        /// 添加浏览器
        /// </summary>
        /// <param name="browser">浏览器</param>
        public void AddBrowser(string browser)
        {
            if (string.IsNullOrEmpty(browser))
                throw new ArgumentException("浏览器不能为空", nameof(browser));

            var currentList = Browsers.ToList();
            if (!currentList.Contains(browser))
            {
                currentList.Add(browser);
                SetRule("Browsers", currentList);
            }
        }

        /// <summary>
        /// 验证设备定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 至少需要配置一个设备条件
            if (!DeviceTypes.Any() && !OperatingSystems.Any() && !Browsers.Any())
                return false;

            // 验证操作系统和浏览器名称不能为空字符串
            if (OperatingSystems.Any(string.IsNullOrWhiteSpace) || Browsers.Any(string.IsNullOrWhiteSpace))
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"DeviceTypes: {DeviceTypes.Count}, OS: {OperatingSystems.Count}, Browsers: {Browsers.Count}";
            return $"{summary} - {details}";
        }
    }
}