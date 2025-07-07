using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 地理位置信息
    /// </summary>
    public class GeoInfo : ValueObject
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        public string? CountryCode { get; private set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string? CountryName { get; private set; }

        /// <summary>
        /// 省份代码
        /// </summary>
        public string? ProvinceCode { get; private set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string? ProvinceName { get; private set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string? CityName { get; private set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string? PostalCode { get; private set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; private set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; private set; }

        /// <summary>
        /// 精度（米）
        /// </summary>
        public int? Accuracy { get; private set; }

        /// <summary>
        /// 时区
        /// </summary>
        public string? TimeZone { get; private set; }

        /// <summary>
        /// ISP信息
        /// </summary>
        public string? Isp { get; private set; }

        /// <summary>
        /// 地理位置来源
        /// </summary>
        public string? Source { get; private set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdated { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private GeoInfo() { }

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
            DateTime? lastUpdated = null)
        {
            ValidateInput(latitude, longitude, accuracy);

            CountryCode = countryCode;
            CountryName = countryName;
            ProvinceCode = provinceCode;
            ProvinceName = provinceName;
            CityName = cityName;
            PostalCode = postalCode;
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
            TimeZone = timeZone;
            Isp = isp;
            Source = source;
            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// 创建地理位置信息
        /// </summary>
        public static GeoInfo Create(
            string? countryCode = null,
            string? countryName = null,
            string? cityName = null,
            decimal? latitude = null,
            decimal? longitude = null)
        {
            return new GeoInfo(
                countryCode: countryCode,
                countryName: countryName,
                cityName: cityName,
                latitude: latitude,
                longitude: longitude);
        }

        /// <summary>
        /// 创建带坐标的地理位置信息
        /// </summary>
        public static GeoInfo CreateWithCoordinates(
            decimal latitude,
            decimal longitude,
            int? accuracy = null,
            string? source = null)
        {
            return new GeoInfo(
                latitude: latitude,
                longitude: longitude,
                accuracy: accuracy,
                source: source,
                lastUpdated: DateTime.UtcNow);
        }

        /// <summary>
        /// 设置坐标
        /// </summary>
        public GeoInfo WithCoordinates(decimal latitude, decimal longitude, int? accuracy = null)
        {
            return new GeoInfo(
                CountryCode, CountryName, ProvinceCode, ProvinceName, CityName, PostalCode,
                latitude, longitude, accuracy, TimeZone, Isp, Source, DateTime.UtcNow);
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
                Latitude, Longitude, Accuracy, TimeZone, Isp, Source, DateTime.UtcNow);
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
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CountryCode ?? string.Empty;
            yield return CountryName ?? string.Empty;
            yield return ProvinceCode ?? string.Empty;
            yield return ProvinceName ?? string.Empty;
            yield return CityName ?? string.Empty;
            yield return PostalCode ?? string.Empty;
            yield return Latitude ?? 0m;
            yield return Longitude ?? 0m;
            yield return Accuracy ?? 0;
            yield return TimeZone ?? string.Empty;
            yield return Isp ?? string.Empty;
            yield return Source ?? string.Empty;
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
