using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 地理位置值对象
/// </summary>
public class GeoLocation : ValueObject
{
    /// <summary>
    /// 国家代码 (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; private set; }

    /// <summary>
    /// 国家名称
    /// </summary>
    public string? CountryName { get; private set; }

    /// <summary>
    /// 省份/州代码
    /// </summary>
    public string? StateCode { get; private set; }

    /// <summary>
    /// 省份/州名称
    /// </summary>
    public string? StateName { get; private set; }

    /// <summary>
    /// 城市名称
    /// </summary>
    public string? CityName { get; private set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public decimal? Latitude { get; private set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal? Longitude { get; private set; }

    /// <summary>
    /// 邮政编码
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// 地理位置精度（米）
    /// </summary>
    public int? Accuracy { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private GeoLocation() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public GeoLocation(
        string? countryCode = null,
        string? countryName = null,
        string? stateCode = null,
        string? stateName = null,
        string? cityName = null,
        decimal? latitude = null,
        decimal? longitude = null,
        string? postalCode = null,
        string? timeZone = null,
        int? accuracy = null)
    {
        CountryCode = countryCode;
        CountryName = countryName;
        StateCode = stateCode;
        StateName = stateName;
        CityName = cityName;
        Latitude = latitude;
        Longitude = longitude;
        PostalCode = postalCode;
        TimeZone = timeZone;
        Accuracy = accuracy;

        ValidateCoordinates();
    }

    /// <summary>
    /// 创建基于国家的地理位置
    /// </summary>
    public static GeoLocation FromCountry(string countryCode, string? countryName = null)
    {
        return new GeoLocation(countryCode: countryCode, countryName: countryName);
    }

    /// <summary>
    /// 创建基于城市的地理位置
    /// </summary>
    public static GeoLocation FromCity(string countryCode, string? stateName, string cityName)
    {
        return new GeoLocation(countryCode: countryCode, stateName: stateName, cityName: cityName);
    }

    /// <summary>
    /// 创建基于坐标的地理位置
    /// </summary>
    public static GeoLocation FromCoordinates(decimal latitude, decimal longitude, int? accuracy = null)
    {
        return new GeoLocation(latitude: latitude, longitude: longitude, accuracy: accuracy);
    }

    /// <summary>
    /// 是否有坐标信息
    /// </summary>
    public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;

    /// <summary>
    /// 是否有详细地址信息
    /// </summary>
    public bool HasDetailedAddress => !string.IsNullOrWhiteSpace(CountryCode) && 
                                      !string.IsNullOrWhiteSpace(CityName);

    /// <summary>
    /// 计算与另一个地理位置的距离（公里）
    /// </summary>
    public double? CalculateDistanceKm(GeoLocation other)
    {
        if (!HasCoordinates || !other.HasCoordinates)
            return null;

        const double earthRadius = 6371.0; // 地球半径（公里）

        var lat1Rad = (double)(Latitude!.Value * (decimal)Math.PI / 180);
        var lat2Rad = (double)(other.Latitude!.Value * (decimal)Math.PI / 180);
        var deltaLatRad = (double)((other.Latitude.Value - Latitude.Value) * (decimal)Math.PI / 180);
        var deltaLonRad = (double)((other.Longitude!.Value - Longitude!.Value) * (decimal)Math.PI / 180);

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadius * c;
    }

    /// <summary>
    /// 判断是否在指定半径范围内
    /// </summary>
    public bool IsWithinRadius(GeoLocation center, double radiusKm)
    {
        var distance = CalculateDistanceKm(center);
        return distance.HasValue && distance.Value <= radiusKm;
    }

    /// <summary>
    /// 验证坐标有效性
    /// </summary>
    private void ValidateCoordinates()
    {
        if (Latitude.HasValue && (Latitude < -90 || Latitude > 90))
            throw new ArgumentOutOfRangeException(nameof(Latitude), "纬度必须在-90到90之间");

        if (Longitude.HasValue && (Longitude < -180 || Longitude > 180))
            throw new ArgumentOutOfRangeException(nameof(Longitude), "经度必须在-180到180之间");
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode ?? string.Empty;
        yield return CountryName ?? string.Empty;
        yield return StateCode ?? string.Empty;
        yield return StateName ?? string.Empty;
        yield return CityName ?? string.Empty;
        yield return Latitude ?? 0m;
        yield return Longitude ?? 0m;
        yield return PostalCode ?? string.Empty;
        yield return TimeZone ?? string.Empty;
        yield return Accuracy ?? 0;
    }
}