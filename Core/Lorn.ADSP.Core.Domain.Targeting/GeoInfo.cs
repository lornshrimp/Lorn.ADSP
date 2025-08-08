namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 地理位置信息定向上下文
    /// 继承自TargetingContextBase，提供地理位置数据的定向上下文功能
    /// </summary>
    public class GeoInfo : TargetingContextBase
    {
        /// <summary>
        /// 上下文名称
        /// </summary>
        public override string ContextName => "地理位置上下文";

        /// <summary>
        /// 国家代码
        /// </summary>
        public string? CountryCode => GetPropertyValue<string>("CountryCode");

        /// <summary>
        /// 国家名称
        /// </summary>
        public string? CountryName => GetPropertyValue<string>("CountryName");

        /// <summary>
        /// 省份代码
        /// </summary>
        public string? ProvinceCode => GetPropertyValue<string>("ProvinceCode");

        /// <summary>
        /// 省份名称
        /// </summary>
        public string? ProvinceName => GetPropertyValue<string>("ProvinceName");

        /// <summary>
        /// 城市名称
        /// </summary>
        public string? CityName => GetPropertyValue<string>("CityName");

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string? PostalCode => GetPropertyValue<string>("PostalCode");

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude => GetPropertyValue<decimal?>("Latitude");

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude => GetPropertyValue<decimal?>("Longitude");

        /// <summary>
        /// 精度（米）
        /// </summary>
        public int? Accuracy => GetPropertyValue<int?>("Accuracy");

        /// <summary>
        /// 时区
        /// </summary>
        public string? TimeZone => GetPropertyValue<string>("TimeZone");

        /// <summary>
        /// ISP信息
        /// </summary>
        public string? Isp => GetPropertyValue<string>("Isp");

        /// <summary>
        /// 地理位置来源
        /// </summary>
        public string? Source => GetPropertyValue<string>("Source");

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdated => GetPropertyValue<DateTime?>("LastUpdated");

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private GeoInfo() : base("Geo") { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GeoInfo(
            string? countryCode = null,
            string? countryName = null,
            string? provinceCode = null,
            string? provinceName = null,
            string? cityName = null,
            string? postalCode = null,
            decimal? latitude = null,
            decimal? longitude = null,
            int? accuracy = null,
            string? timeZone = null,
            string? isp = null,
            string? source = null,
            DateTime? lastUpdated = null,
            string? dataSource = null)
            : base("Geo", CreateProperties(countryCode, countryName, provinceCode, provinceName, cityName, postalCode, latitude, longitude, accuracy, timeZone, isp, source, lastUpdated), dataSource)
        {
            ValidateInput(latitude, longitude, accuracy);
        }

        /// <summary>
        /// 创建属性字典
        /// </summary>
        private static Dictionary<string, object> CreateProperties(
            string? countryCode,
            string? countryName,
            string? provinceCode,
            string? provinceName,
            string? cityName,
            string? postalCode,
            decimal? latitude,
            decimal? longitude,
            int? accuracy,
            string? timeZone,
            string? isp,
            string? source,
            DateTime? lastUpdated)
        {
            var properties = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(countryCode))
                properties["CountryCode"] = countryCode;

            if (!string.IsNullOrWhiteSpace(countryName))
                properties["CountryName"] = countryName;

            if (!string.IsNullOrWhiteSpace(provinceCode))
                properties["ProvinceCode"] = provinceCode;

            if (!string.IsNullOrWhiteSpace(provinceName))
                properties["ProvinceName"] = provinceName;

            if (!string.IsNullOrWhiteSpace(cityName))
                properties["CityName"] = cityName;

            if (!string.IsNullOrWhiteSpace(postalCode))
                properties["PostalCode"] = postalCode;

            if (latitude.HasValue)
                properties["Latitude"] = latitude;

            if (longitude.HasValue)
                properties["Longitude"] = longitude;

            if (accuracy.HasValue)
                properties["Accuracy"] = accuracy.Value;

            if (!string.IsNullOrWhiteSpace(timeZone))
                properties["TimeZone"] = timeZone;

            if (!string.IsNullOrWhiteSpace(isp))
                properties["Isp"] = isp;

            if (!string.IsNullOrWhiteSpace(source))
                properties["Source"] = source;

            if (lastUpdated.HasValue)
                properties["LastUpdated"] = lastUpdated.Value;

            return properties;
        }

        /// <summary>
        /// 创建地理位置信息
        /// </summary>
        public static GeoInfo Create(
            string? countryCode = null,
            string? countryName = null,
            string? cityName = null,
            decimal? latitude = null,
            decimal? longitude = null,
            string? dataSource = null)
        {
            return new GeoInfo(
                countryCode: countryCode,
                countryName: countryName,
                cityName: cityName,
                latitude: latitude,
                longitude: longitude,
                dataSource: dataSource);
        }

        /// <summary>
        /// 创建带坐标的地理位置信息
        /// </summary>
        public static GeoInfo CreateWithCoordinates(
            decimal latitude,
            decimal longitude,
            int? accuracy = null,
            string? source = null,
            string? dataSource = null)
        {
            return new GeoInfo(
                latitude: latitude,
                longitude: longitude,
                accuracy: accuracy,
                source: source,
                lastUpdated: DateTime.UtcNow,
                dataSource: dataSource);
        }

        /// <summary>
        /// 设置坐标
        /// </summary>
        public GeoInfo WithCoordinates(decimal latitude, decimal longitude, int? accuracy = null)
        {
            return new GeoInfo(
                CountryCode, CountryName, ProvinceCode, ProvinceName, CityName, PostalCode,
                latitude, longitude, accuracy, TimeZone, Isp, Source, DateTime.UtcNow, DataSource);
        }

        /// <summary>
        /// 设置地址信息
        /// </summary>
        public GeoInfo WithAddress(
            string? countryCode = null,
            string? countryName = null,
            string? provinceCode = null,
            string? provinceName = null,
            string? cityName = null,
            string? postalCode = null)
        {
            return new GeoInfo(
                countryCode ?? CountryCode,
                countryName ?? CountryName,
                provinceCode ?? ProvinceCode,
                provinceName ?? ProvinceName,
                cityName ?? CityName,
                postalCode ?? PostalCode,
                Latitude, Longitude, Accuracy, TimeZone, Isp, Source, DateTime.UtcNow, DataSource);
        }

        /// <summary>
        /// 是否有坐标信息
        /// </summary>
        public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;

        /// <summary>
        /// 是否有地址信息
        /// </summary>
        public bool HasAddress => !string.IsNullOrEmpty(CountryCode) || !string.IsNullOrEmpty(CityName);

        /// <summary>
        /// 计算与另一个地理位置的距离（公里）
        /// </summary>
        public double? CalculateDistanceTo(GeoInfo other)
        {
            if (!HasCoordinates || !other.HasCoordinates)
                return null;

            return CalculateDistance(
                Latitude!.Value, Longitude!.Value,
                other.Latitude!.Value, other.Longitude!.Value);
        }

        /// <summary>
        /// 获取地理位置描述
        /// </summary>
        public string GetLocationDescription()
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(CityName))
                parts.Add(CityName);

            if (!string.IsNullOrEmpty(ProvinceName))
                parts.Add(ProvinceName);

            if (!string.IsNullOrEmpty(CountryName))
                parts.Add(CountryName);

            return parts.Any() ? string.Join(", ", parts) : "Unknown Location";
        }

        /// <summary>
        /// 获取调试信息
        /// </summary>
        public override string GetDebugInfo()
        {
            var baseInfo = base.GetDebugInfo();
            var locationInfo = $"Location:{GetLocationDescription()} Coordinates:{(HasCoordinates ? $"{Latitude},{Longitude}" : "None")}";
            return $"{baseInfo} | {locationInfo}";
        }

        /// <summary>
        /// 验证上下文的有效性
        /// </summary>
        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            // 验证坐标范围
            if (Latitude.HasValue && (Latitude < -90 || Latitude > 90))
                return false;

            if (Longitude.HasValue && (Longitude < -180 || Longitude > 180))
                return false;

            if (Accuracy.HasValue && Accuracy < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(decimal? latitude, decimal? longitude, int? accuracy)
        {
            if (latitude.HasValue && (latitude < -90 || latitude > 90))
                throw new ArgumentException("纬度必须在-90到90之间", nameof(latitude));

            if (longitude.HasValue && (longitude < -180 || longitude > 180))
                throw new ArgumentException("经度必须在-180到180之间", nameof(longitude));

            if (accuracy.HasValue && accuracy < 0)
                throw new ArgumentException("精度不能为负数", nameof(accuracy));
        }

        /// <summary>
        /// 计算两点间距离（哈弗赛因公式）
        /// </summary>
        private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double earthRadius = 6371; // 地球半径（公里）

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
