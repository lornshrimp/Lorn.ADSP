namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 地理位置信息
    /// </summary>
    public record GeoInfo
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        public string? CountryCode { get; init; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string? CountryName { get; init; }

        /// <summary>
        /// 地区代码
        /// </summary>
        public string? RegionCode { get; init; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string? RegionName { get; init; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string? CityName { get; init; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string? PostalCode { get; init; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; init; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; init; }

        /// <summary>
        /// 精度（米）
        /// </summary>
        public int? Accuracy { get; init; }

        /// <summary>
        /// 时区
        /// </summary>
        public string? TimeZone { get; init; }

        /// <summary>
        /// ISP信息
        /// </summary>
        public string? Isp { get; init; }

        /// <summary>
        /// 地理位置来源
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdated { get; init; }
    }
}
