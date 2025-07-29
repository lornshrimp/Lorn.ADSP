using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 行政地理定向条件类
    /// 实现基于国家、省份、城市等行政区划的地理定向功能
    /// 支持复杂的地理边界管控和投放，适用于特定区域、省份或国家的营销
    /// </summary>
    public class AdministrativeGeoTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "行政地理定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "AdministrativeGeo";

        /// <summary>
        /// 包含的地理位置列表
        /// 支持国家、省份、城市等不同层级的地理位置
        /// </summary>
        public IReadOnlyList<GeoInfo> IncludedLocations => GetRule<List<GeoInfo>>("IncludedLocations") ?? [];

        /// <summary>
        /// 排除的地理位置列表
        /// 用于在大范围内排除特定区域
        /// </summary>
        public IReadOnlyList<GeoInfo> ExcludedLocations => GetRule<List<GeoInfo>>("ExcludedLocations") ?? [];

        /// <summary>
        /// 地理定向模式
        /// Include: 仅投放指定区域
        /// Exclude: 排除指定区域，对其他区域投放
        /// </summary>
        public GeoTargetingMode Mode => GetRule<GeoTargetingMode>("Mode", GeoTargetingMode.Include);

        /// <summary>
        /// 最大扩展距离（公里）
        /// 在行政边界基础上扩展投放范围
        /// 例如：针对北京市，可以扩展到周边50公里范围
        /// </summary>
        public double? MaxDistance => GetRule<double?>("MaxDistance");

        /// <summary>
        /// 行政层级优先级
        /// 当用户位置匹配多个层级时的优先级规则
        /// Higher: 优先使用更高层级（省份优先于城市）
        /// Lower: 优先使用更低层级（城市优先于省份）
        /// Exact: 精确匹配指定层级
        /// </summary>
        public AdministrativeLevel? LevelPriority => GetRule<AdministrativeLevel?>("LevelPriority");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="includedLocations">包含的地理位置</param>
        /// <param name="excludedLocations">排除的地理位置</param>
        /// <param name="mode">定向模式</param>
        /// <param name="maxDistance">最大扩展距离（公里）</param>
        /// <param name="levelPriority">行政层级优先级</param>
        /// <param name="weight">权重</param>
        /// <param name="isEnabled">是否启用</param>
        public AdministrativeGeoTargeting(
            IList<GeoInfo>? includedLocations = null,
            IList<GeoInfo>? excludedLocations = null,
            GeoTargetingMode mode = GeoTargetingMode.Include,
            double? maxDistance = null,
            AdministrativeLevel? levelPriority = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(includedLocations, excludedLocations, mode, maxDistance, levelPriority), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static List<TargetingRule> CreateRules(
            IList<GeoInfo>? includedLocations,
            IList<GeoInfo>? excludedLocations,
            GeoTargetingMode mode,
            double? maxDistance,
            AdministrativeLevel? levelPriority)
        {
            var rules = new List<TargetingRule>();

            // 添加包含的位置列表
            var includedLocationsList = includedLocations?.ToList() ?? [];
            var includedLocationsRule = new TargetingRule("IncludedLocations", string.Empty, "Json").WithValue(includedLocationsList);
            rules.Add(includedLocationsRule);

            // 添加排除的位置列表
            var excludedLocationsList = excludedLocations?.ToList() ?? [];
            var excludedLocationsRule = new TargetingRule("ExcludedLocations", string.Empty, "Json").WithValue(excludedLocationsList);
            rules.Add(excludedLocationsRule);

            // 添加模式
            var modeRule = new TargetingRule("Mode", string.Empty, "Enum").WithValue(mode);
            rules.Add(modeRule);

            // 添加最大距离（如果存在）
            if (maxDistance.HasValue)
            {
                var maxDistanceRule = new TargetingRule("MaxDistance", string.Empty, "Double").WithValue(maxDistance.Value);
                rules.Add(maxDistanceRule);
            }

            // 添加级别优先级（如果存在）
            if (levelPriority.HasValue)
            {
                var levelPriorityRule = new TargetingRule("LevelPriority", string.Empty, "Enum").WithValue(levelPriority.Value);
                rules.Add(levelPriorityRule);
            }

            return rules;
        }

        /// <summary>
        /// 创建行政地理定向条件实例
        /// </summary>
        public static AdministrativeGeoTargeting Create(
            IList<GeoInfo>? includedLocations = null,
            IList<GeoInfo>? excludedLocations = null,
            GeoTargetingMode mode = GeoTargetingMode.Include,
            double? maxDistance = null,
            AdministrativeLevel? levelPriority = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new AdministrativeGeoTargeting(includedLocations, excludedLocations, mode, maxDistance, levelPriority, weight, isEnabled);
        }

        /// <summary>
        /// 添加包含的地理位置
        /// </summary>
        /// <param name="geoInfo">地理位置信息</param>
        public void AddIncludedLocation(GeoInfo geoInfo)
        {
            ArgumentNullException.ThrowIfNull(geoInfo);
            ValidateAdministrativeLocation(geoInfo);

            var currentList = IncludedLocations.ToList();
            currentList.Add(geoInfo);
            SetRule("IncludedLocations", currentList);
        }

        /// <summary>
        /// 添加排除的地理位置
        /// </summary>
        /// <param name="geoInfo">地理位置信息</param>
        public void AddExcludedLocation(GeoInfo geoInfo)
        {
            ArgumentNullException.ThrowIfNull(geoInfo);
            ValidateAdministrativeLocation(geoInfo);

            var currentList = ExcludedLocations.ToList();
            currentList.Add(geoInfo);
            SetRule("ExcludedLocations", currentList);
        }

        /// <summary>
        /// 批量添加国家定向（包含或排除）
        /// </summary>
        /// <param name="countryCodes">国家代码列表</param>
        /// <param name="include">是否为包含模式</param>
        public void AddCountries(IEnumerable<string> countryCodes, bool include = true)
        {
            foreach (var countryCode in countryCodes)
            {
                var geoInfo = new GeoInfo(countryCode: countryCode);
                if (include)
                    AddIncludedLocation(geoInfo);
                else
                    AddExcludedLocation(geoInfo);
            }
        }

        /// <summary>
        /// 批量添加城市定向（包含或排除）
        /// </summary>
        /// <param name="cityNames">城市名称列表</param>
        /// <param name="countryCode">所属国家代码</param>
        /// <param name="include">是否为包含模式</param>
        public void AddCities(IEnumerable<string> cityNames, string countryCode, bool include = true)
        {
            foreach (var cityName in cityNames)
            {
                var geoInfo = new GeoInfo(countryCode: countryCode, cityName: cityName);
                if (include)
                    AddIncludedLocation(geoInfo);
                else
                    AddExcludedLocation(geoInfo);
            }
        }

        /// <summary>
        /// 设置最大扩展距离
        /// </summary>
        /// <param name="maxDistance">最大扩展距离（公里）</param>
        public void SetMaxDistance(double? maxDistance)
        {
            if (maxDistance.HasValue)
            {
                if (maxDistance.Value <= 0)
                    throw new ArgumentException("最大扩展距离必须大于0", nameof(maxDistance));

                if (maxDistance.Value > 1000)
                    throw new ArgumentException("出于性能考虑，最大扩展距离不应超过1000公里", nameof(maxDistance));

                SetRule("MaxDistance", maxDistance.Value);
            }
            else
            {
                RemoveRule("MaxDistance");
            }
        }

        /// <summary>
        /// 设置定向模式
        /// </summary>
        /// <param name="mode">定向模式</param>
        public void SetMode(GeoTargetingMode mode)
        {
            SetRule("Mode", mode);
        }

        /// <summary>
        /// 设置行政层级优先级
        /// </summary>
        /// <param name="levelPriority">行政层级优先级</param>
        public void SetLevelPriority(AdministrativeLevel? levelPriority)
        {
            if (levelPriority.HasValue)
            {
                SetRule("LevelPriority", levelPriority.Value);
            }
            else
            {
                RemoveRule("LevelPriority");
            }
        }

        /// <summary>
        /// 检查用户位置是否匹配定向条件
        /// </summary>
        /// <param name="userLocation">用户当前位置信息</param>
        /// <returns>是否匹配</returns>
        public bool IsLocationMatched(GeoInfo userLocation)
        {
            if (userLocation == null)
                return false;

            var isIncluded = IsLocationInList(userLocation, IncludedLocations);
            var isExcluded = IsLocationInList(userLocation, ExcludedLocations);

            return Mode switch
            {
                GeoTargetingMode.Include => isIncluded && !isExcluded,
                GeoTargetingMode.Exclude => !isExcluded,
                _ => false
            };
        }

        /// <summary>
        /// 验证条件配置的特定规则有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证最大扩展距离
            if (MaxDistance.HasValue && (MaxDistance.Value <= 0 || MaxDistance.Value > 1000))
                return false;

            // 验证包含和排除列表不能都为空（在Include模式下）
            if (Mode == GeoTargetingMode.Include && !IncludedLocations.Any() && !ExcludedLocations.Any())
                return false;

            // 验证地理位置信息的有效性
            foreach (var location in IncludedLocations.Concat(ExcludedLocations))
            {
                if (!IsValidAdministrativeLocation(location))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"Type: Administrative, Mode: {Mode}, Included: {IncludedLocations.Count}, Excluded: {ExcludedLocations.Count}";

            if (MaxDistance.HasValue)
                details += $", Extension: {MaxDistance:F1}km";

            if (LevelPriority.HasValue)
                details += $", Priority: {LevelPriority}";

            return $"{summary} - {details}";
        }

        /// <summary>
        /// 验证行政区位置信息
        /// </summary>
        private static void ValidateAdministrativeLocation(GeoInfo geoInfo)
        {
            if (!IsValidAdministrativeLocation(geoInfo))
                throw new ArgumentException("行政地理定向需要有效位置信息，至少包含有效的地理标识符（国家代码、省份代码或城市名称）", nameof(geoInfo));
        }

        /// <summary>
        /// 检查是否为有效的行政区位置信息
        /// </summary>
        private static bool IsValidAdministrativeLocation(GeoInfo geoInfo)
        {
            if (geoInfo == null)
                return false;

            // 行政地理定向需要至少包含一个行政区标识符
            return !string.IsNullOrEmpty(geoInfo.CountryCode) ||
                   !string.IsNullOrEmpty(geoInfo.ProvinceCode) ||
                   !string.IsNullOrEmpty(geoInfo.CityName) ||
                   !string.IsNullOrEmpty(geoInfo.PostalCode);
        }

        /// <summary>
        /// 检查用户位置是否在指定的地理位置列表中
        /// </summary>
        private bool IsLocationInList(GeoInfo userLocation, IReadOnlyList<GeoInfo> locationList)
        {
            foreach (var targetLocation in locationList)
            {
                if (IsAdministrativeLocationMatched(userLocation, targetLocation))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 检查两个行政区位置是否匹配
        /// </summary>
        private bool IsAdministrativeLocationMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            // 根据层级优先级进行匹配
            return LevelPriority switch
            {
                AdministrativeLevel.Country => IsCountryMatched(userLocation, targetLocation),
                AdministrativeLevel.Province => IsProvinceMatched(userLocation, targetLocation),
                AdministrativeLevel.City => IsCityMatched(userLocation, targetLocation),
                _ => IsCityMatched(userLocation, targetLocation) ||
                     IsProvinceMatched(userLocation, targetLocation) ||
                     IsCountryMatched(userLocation, targetLocation)
            };
        }

        /// <summary>
        /// 检查国家是否匹配
        /// </summary>
        private static bool IsCountryMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.CountryCode) &&
                   !string.IsNullOrEmpty(userLocation.CountryCode) &&
                   string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查省份是否匹配
        /// </summary>
        private static bool IsProvinceMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.ProvinceCode) &&
                   !string.IsNullOrEmpty(userLocation.ProvinceCode) &&
                   string.Equals(userLocation.ProvinceCode, targetLocation.ProvinceCode, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查城市是否匹配
        /// </summary>
        private static bool IsCityMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.CityName) &&
                   !string.IsNullOrEmpty(userLocation.CityName) &&
                   string.Equals(userLocation.CityName, targetLocation.CityName, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// 行政区层级枚举
    /// </summary>
    public enum AdministrativeLevel
    {
        /// <summary>
        /// 国家级别
        /// </summary>
        Country = 1,

        /// <summary>
        /// 省份/州级别
        /// </summary>
        Province = 2,

        /// <summary>
        /// 城市级别
        /// </summary>
        City = 3
    }
}