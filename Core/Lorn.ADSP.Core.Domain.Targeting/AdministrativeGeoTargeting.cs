using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ��������������������
    /// ʵ�ֻ��ڹ��ҡ�ʡ�ݡ����е���������ĵ��������������
    /// ���ó������������α߽�Ĺ��Ͷ�ţ�������ض����С�ʡ�ݻ���ҵ�Ӫ���
    /// </summary>
    public class AdministrativeGeoTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "行政地理定向";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "AdministrativeGeo";

        /// <summary>
        /// ���������������б�
        /// ֧�ֹ��ҡ�ʡ�ݡ����еȲ�ͬ�㼶������������
        /// </summary>
        public IReadOnlyList<GeoInfo> IncludedLocations => GetRule<List<GeoInfo>>("IncludedLocations") ?? new List<GeoInfo>();

        /// <summary>
        /// �ų������������б�
        /// �����ڴ�Χ�������ų��ض�����
        /// </summary>
        public IReadOnlyList<GeoInfo> ExcludedLocations => GetRule<List<GeoInfo>>("ExcludedLocations") ?? new List<GeoInfo>();

        /// <summary>
        /// ��������ģʽ
        /// Include: ������ָ������
        /// Exclude: �ų�ָ�����򣬶���������������
        /// </summary>
        public GeoTargetingMode Mode => GetRule<GeoTargetingMode>("Mode", GeoTargetingMode.Include);

        /// <summary>
        /// �����չ���루���
        /// ��������������߽��������չ����Χ
        /// ���磺���򱱾��У�����չ���ܱ�50���ﷶΧ
        /// </summary>
        public double? MaxDistance => GetRule<double?>("MaxDistance");

        /// <summary>
        /// ����㼶���ȼ�
        /// ���û�λ��ƥ������������ʱ�����ȼ�����
        /// Higher: ����ʹ�ø��߲㼶����ʡ�������ڳ��У�
        /// Lower: ����ʹ�ø��Ͳ㼶�������������ʡ�ݣ�
        /// Exact: ��ȷƥ��ָ���㼶
        /// </summary>
        public AdministrativeLevel? LevelPriority => GetRule<AdministrativeLevel?>("LevelPriority");

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="includedLocations">��������������</param>
        /// <param name="excludedLocations">�ų�����������</param>
        /// <param name="mode">����ģʽ</param>
        /// <param name="maxDistance">�����չ���루���</param>
        /// <param name="levelPriority">����㼶���ȼ�</param>
        /// <param name="weight">Ȩ��</param>
        /// <param name="isEnabled">�Ƿ�����</param>
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
        /// ���������ֵ�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<GeoInfo>? includedLocations,
            IList<GeoInfo>? excludedLocations,
            GeoTargetingMode mode,
            double? maxDistance,
            AdministrativeLevel? levelPriority)
        {
            var rules = new List<TargetingRule>();

            // 添加包含的位置列表
            var includedLocationsList = includedLocations?.ToList() ?? new List<GeoInfo>();
            var includedLocationsRule = new TargetingRule("IncludedLocations", string.Empty, "Json").WithValue(includedLocationsList);
            rules.Add(includedLocationsRule);

            // 添加排除的位置列表
            var excludedLocationsList = excludedLocations?.ToList() ?? new List<GeoInfo>();
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
        /// ������������������������
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
        /// ���Ӱ�������������
        /// </summary>
        /// <param name="geoInfo">����������Ϣ</param>
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
        /// �����ų�����������
        /// </summary>
        /// <param name="geoInfo">����������Ϣ</param>
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
        /// ���������������򣨰����ң�
        /// </summary>
        /// <param name="countryCodes">���Ҵ����б�</param>
        /// <param name="include">�Ƿ�Ϊ����ģʽ</param>
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
        /// ���������������򣨰����У�
        /// </summary>
        /// <param name="cityNames">���������б�</param>
        /// <param name="countryCode">�������Ҵ���</param>
        /// <param name="include">�Ƿ�Ϊ����ģʽ</param>
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
        /// ���������չ����
        /// </summary>
        /// <param name="maxDistance">�����չ���루���</param>
        public void SetMaxDistance(double? maxDistance)
        {
            if (maxDistance.HasValue)
            {
                if (maxDistance.Value <= 0)
                    throw new ArgumentException("�����չ����������0", nameof(maxDistance));

                if (maxDistance.Value > 1000)
                    throw new ArgumentException("������������������չ���벻Ӧ����1000����", nameof(maxDistance));

                SetRule("MaxDistance", maxDistance.Value);
            }
            else
            {
                RemoveRule("MaxDistance");
            }
        }

        /// <summary>
        /// ���ö���ģʽ
        /// </summary>
        /// <param name="mode">����ģʽ</param>
        public void SetMode(GeoTargetingMode mode)
        {
            SetRule("Mode", mode);
        }

        /// <summary>
        /// ��������㼶���ȼ�
        /// </summary>
        /// <param name="levelPriority">����㼶���ȼ�</param>
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
        /// ����û�λ���Ƿ�ƥ�䶨������
        /// </summary>
        /// <param name="userLocation">�û�����λ����Ϣ</param>
        /// <returns>�Ƿ�ƥ��</returns>
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
        /// ��֤�����������������ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ��֤�����չ����
            if (MaxDistance.HasValue && (MaxDistance.Value <= 0 || MaxDistance.Value > 1000))
                return false;

            // ��֤�������ų��б����ܶ�Ϊ�գ���Includeģʽ�£�
            if (Mode == GeoTargetingMode.Include && !IncludedLocations.Any() && !ExcludedLocations.Any())
                return false;

            // ��֤����������Ϣ����Ч��
            foreach (var location in IncludedLocations.Concat(ExcludedLocations))
            {
                if (!IsValidAdministrativeLocation(location))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
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
        /// ��֤��������λ����Ϣ
        /// </summary>
        private static void ValidateAdministrativeLocation(GeoInfo geoInfo)
        {
            if (!IsValidAdministrativeLocation(geoInfo))
                throw new ArgumentException("������������Ҫ�����λ����Ϣ������Ч�����������ʶ�����Ҵ��롢ʡ�ݴ����������ƣ�", nameof(geoInfo));
        }

        /// <summary>
        /// ����Ƿ�Ϊ��Ч����������λ����Ϣ
        /// </summary>
        private static bool IsValidAdministrativeLocation(GeoInfo geoInfo)
        {
            if (geoInfo == null)
                return false;

            // ������������Ҫ��������һ�����������ʶ
            return !string.IsNullOrEmpty(geoInfo.CountryCode) ||
                   !string.IsNullOrEmpty(geoInfo.ProvinceCode) ||
                   !string.IsNullOrEmpty(geoInfo.CityName) ||
                   !string.IsNullOrEmpty(geoInfo.PostalCode);
        }

        /// <summary>
        /// ����û�λ���Ƿ���ָ�������������б���
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
        /// ���������������λ���Ƿ�ƥ��
        /// </summary>
        private bool IsAdministrativeLocationMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            // ���㼶���ȼ�����ƥ��
            switch (LevelPriority)
            {
                case AdministrativeLevel.Country:
                    return IsCountryMatched(userLocation, targetLocation);

                case AdministrativeLevel.Province:
                    return IsProvinceMatched(userLocation, targetLocation);

                case AdministrativeLevel.City:
                    return IsCityMatched(userLocation, targetLocation);

                default:
                    // Ĭ�ϰ���ϸ���ֵ�˳��ƥ��
                    return IsCityMatched(userLocation, targetLocation) ||
                           IsProvinceMatched(userLocation, targetLocation) ||
                           IsCountryMatched(userLocation, targetLocation);
            }
        }

        /// <summary>
        /// �������Ƿ�ƥ��
        /// </summary>
        private static bool IsCountryMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.CountryCode) &&
                   !string.IsNullOrEmpty(userLocation.CountryCode) &&
                   string.Equals(userLocation.CountryCode, targetLocation.CountryCode, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ���ʡ���Ƿ�ƥ��
        /// </summary>
        private static bool IsProvinceMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.ProvinceCode) &&
                   !string.IsNullOrEmpty(userLocation.ProvinceCode) &&
                   string.Equals(userLocation.ProvinceCode, targetLocation.ProvinceCode, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// �������Ƿ�ƥ��
        /// </summary>
        private static bool IsCityMatched(GeoInfo userLocation, GeoInfo targetLocation)
        {
            return !string.IsNullOrEmpty(targetLocation.CityName) &&
                   !string.IsNullOrEmpty(userLocation.CityName) &&
                   string.Equals(userLocation.CityName, targetLocation.CityName, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// ��������㼶ö��
    /// </summary>
    public enum AdministrativeLevel
    {
        /// <summary>
        /// ���Ҽ���
        /// </summary>
        Country = 1,

        /// <summary>
        /// ʡ��/�ݼ���
        /// </summary>
        Province = 2,

        /// <summary>
        /// ���м���
        /// </summary>
        City = 3
    }
}
