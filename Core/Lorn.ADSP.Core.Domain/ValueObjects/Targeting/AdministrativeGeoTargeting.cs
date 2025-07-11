using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 行政区划地理定向条件
    /// 实现基于国家、省份、城市等行政区域的地理定向规则配置
    /// 适用场景：基于政治边界的广告投放，如针对特定城市、省份或国家的营销活动
    /// </summary>
    public class AdministrativeGeoTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "AdministrativeGeo";

        /// <summary>
        /// 包含的行政区域列表
        /// 支持国家、省份、城市等不同层级的行政区域定向
        /// </summary>
        public IReadOnlyList<GeoInfo> IncludedLocations => GetRule<List<GeoInfo>>("IncludedLocations") ?? new List<GeoInfo>();

        /// <summary>
        /// 排除的行政区域列表
        /// 用于在大范围定向中排除特定区域
        /// </summary>
        public IReadOnlyList<GeoInfo> ExcludedLocations => GetRule<List<GeoInfo>>("ExcludedLocations") ?? new List<GeoInfo>();

        /// <summary>
        /// 地理定向模式
        /// Include: 仅定向到指定区域
        /// Exclude: 排除指定区域，定向到其他所有区域
        /// </summary>
        public GeoTargetingMode Mode => GetRule<GeoTargetingMode>("Mode", GeoTargetingMode.Include);

        /// <summary>
        /// 最大扩展距离（公里）
        /// 用于在行政区域边界基础上扩展定向范围
        /// 例如：定向北京市，但扩展到周边50公里范围
        /// </summary>
        public double? MaxDistance => GetRule<double?>("MaxDistance");

        /// <summary>
        /// 区域层级优先级
        /// 当用户位置匹配多个行政区域时的优先级设置
        /// Higher: 优先使用更高层级（如省份优先于城市）
        /// Lower: 优先使用更低层级（如城市优先于省份）
        /// Exact: 精确匹配指定层级
        /// </summary>
        public AdministrativeLevel? LevelPriority => GetRule<AdministrativeLevel?>("LevelPriority");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="includedLocations">包含的行政区域</param>
        /// <param name="excludedLocations">排除的行政区域</param>
        /// <param name="mode">定向模式</param>
        /// <param name="maxDistance">最大扩展距离（公里）</param>
        /// <param name="levelPriority">区域层级优先级</param>
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
        private static Dictionary<string, object> CreateRules(
            IList<GeoInfo>? includedLocations,
            IList<GeoInfo>? excludedLocations,
            GeoTargetingMode mode,
            double? maxDistance,
            AdministrativeLevel? levelPriority)
        {
            var rules = new Dictionary<string, object>
            {
                ["IncludedLocations"] = includedLocations?.ToList() ?? new List<GeoInfo>(),
                ["ExcludedLocations"] = excludedLocations?.ToList() ?? new List<GeoInfo>(),
                ["Mode"] = mode
            };

            if (maxDistance.HasValue)
            {
                rules["MaxDistance"] = maxDistance.Value;
            }

            if (levelPriority.HasValue)
            {
                rules["LevelPriority"] = levelPriority.Value;
            }

            return rules;
        }

        /// <summary>
        /// 创建行政区划地理定向条件
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
        /// 添加包含的行政区域
        /// </summary>
        /// <param name="geoInfo">行政区域信息</param>
        public void AddIncludedLocation(GeoInfo geoInfo)
        {
            if (geoInfo == null)
                throw new ArgumentNullException(nameof(geoInfo));

            ValidateAdministrativeLocation(geoInfo);

            var currentList = IncludedLocations.ToList();
            currentList.Add(geoInfo);
            SetRule("IncludedLocations", currentList);
        }

        /// <summary>
        /// 添加排除的行政区域
        /// </summary>
        /// <param name="geoInfo">行政区域信息</param>
        public void AddExcludedLocation(GeoInfo geoInfo)
        {
            if (geoInfo == null)
                throw new ArgumentNullException(nameof(geoInfo));

            ValidateAdministrativeLocation(geoInfo);

            var currentList = ExcludedLocations.ToList();
            currentList.Add(geoInfo);
            SetRule("ExcludedLocations", currentList);
        }

        /// <summary>
        /// 批量添加行政区域（按国家）
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
        /// 批量添加行政区域（按城市）
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
                    throw new ArgumentException("行政区划定向的最大扩展距离不应超过1000公里", nameof(maxDistance));

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
        /// 设置区域层级优先级
        /// </summary>
        /// <param name="levelPriority">区域层级优先级</param>
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
        /// <param name="userLocation">用户地理位置信息</param>
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
        /// 验证行政区划地理定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证最大扩展距离
            if (MaxDistance.HasValue && (MaxDistance.Value <= 0 || MaxDistance.Value > 1000))
                return false;

            // 验证包含和排除列表不能都为空（在Include模式下）
            if (Mode == GeoTargetingMode.Include && !IncludedLocations.Any() && !ExcludedLocations.Any())
                return false;

            // 验证行政区域信息的有效性
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
        /// 验证行政区域位置信息
        /// </summary>
        private static void ValidateAdministrativeLocation(GeoInfo geoInfo)
        {
            if (!IsValidAdministrativeLocation(geoInfo))
                throw new ArgumentException("行政区划定向要求地理位置信息包含有效的行政区域标识（国家代码、省份代码或城市名称）", nameof(geoInfo));
        }

        /// <summary>
        /// 检查是否为有效的行政区域位置信息
        /// </summary>
        private static bool IsValidAdministrativeLocation(GeoInfo geoInfo)
        {
            if (geoInfo == null)
                return false;

            // 行政区划定向要求至少有一个行政区域标识
            return !string.IsNullOrEmpty(geoInfo.CountryCode) ||
                   !string.IsNullOrEmpty(geoInfo.ProvinceCode) ||
                   !string.IsNullOrEmpty(geoInfo.CityName) ||
                   !string.IsNullOrEmpty(geoInfo.PostalCode);
        }

        /// <summary>
        /// 检查用户位置是否在指定的行政区域列表中
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
        /// 检查两个行政区域位置是否匹配
        /// </summary>
        private bool IsAdministrativeLocationMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            // 按层级优先级进行匹配
            switch (LevelPriority)
            {
                case AdministrativeLevel.Country:
                    return IsCountryMatched(userLocation, targetLocation);
                
                case AdministrativeLevel.Province:
                    return IsProvinceMatched(userLocation, targetLocation);
                
                case AdministrativeLevel.City:
                    return IsCityMatched(userLocation, targetLocation);
                
                default:
                    // 默认按从细到粗的顺序匹配
                    return IsCityMatched(userLocation, targetLocation) ||
                           IsProvinceMatched(userLocation, targetLocation) ||
                           IsCountryMatched(userLocation, targetLocation);
            }
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
    /// 行政区域层级枚举
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