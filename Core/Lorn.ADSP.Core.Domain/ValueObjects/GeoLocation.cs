using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ����λ��ֵ����
/// </summary>
public class GeoLocation : ValueObject
{
    /// <summary>
    /// ���Ҵ��� (ISO 3166-1 alpha-2)
    /// </summary>
    public string? CountryCode { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? CountryName { get; private set; }

    /// <summary>
    /// ʡ��/�ݴ���
    /// </summary>
    public string? StateCode { get; private set; }

    /// <summary>
    /// ʡ��/������
    /// </summary>
    public string? StateName { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? CityName { get; private set; }

    /// <summary>
    /// γ��
    /// </summary>
    public decimal? Latitude { get; private set; }

    /// <summary>
    /// ����
    /// </summary>
    public decimal? Longitude { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// ʱ��
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// ����λ�þ��ȣ��ף�
    /// </summary>
    public int? Accuracy { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private GeoLocation() { }

    /// <summary>
    /// ���캯��
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
    /// �������ڹ��ҵĵ���λ��
    /// </summary>
    public static GeoLocation FromCountry(string countryCode, string? countryName = null)
    {
        return new GeoLocation(countryCode: countryCode, countryName: countryName);
    }

    /// <summary>
    /// �������ڳ��еĵ���λ��
    /// </summary>
    public static GeoLocation FromCity(string countryCode, string? stateName, string cityName)
    {
        return new GeoLocation(countryCode: countryCode, stateName: stateName, cityName: cityName);
    }

    /// <summary>
    /// ������������ĵ���λ��
    /// </summary>
    public static GeoLocation FromCoordinates(decimal latitude, decimal longitude, int? accuracy = null)
    {
        return new GeoLocation(latitude: latitude, longitude: longitude, accuracy: accuracy);
    }

    /// <summary>
    /// �Ƿ���������Ϣ
    /// </summary>
    public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;

    /// <summary>
    /// �Ƿ�����ϸ��ַ��Ϣ
    /// </summary>
    public bool HasDetailedAddress => !string.IsNullOrWhiteSpace(CountryCode) && 
                                      !string.IsNullOrWhiteSpace(CityName);

    /// <summary>
    /// ��������һ������λ�õľ��루���
    /// </summary>
    public double? CalculateDistanceKm(GeoLocation other)
    {
        if (!HasCoordinates || !other.HasCoordinates)
            return null;

        const double earthRadius = 6371.0; // ����뾶�����

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
    /// �ж��Ƿ���ָ���뾶��Χ��
    /// </summary>
    public bool IsWithinRadius(GeoLocation center, double radiusKm)
    {
        var distance = CalculateDistanceKm(center);
        return distance.HasValue && distance.Value <= radiusKm;
    }

    /// <summary>
    /// ��֤������Ч��
    /// </summary>
    private void ValidateCoordinates()
    {
        if (Latitude.HasValue && (Latitude < -90 || Latitude > 90))
            throw new ArgumentOutOfRangeException(nameof(Latitude), "γ�ȱ�����-90��90֮��");

        if (Longitude.HasValue && (Longitude < -180 || Longitude > 180))
            throw new ArgumentOutOfRangeException(nameof(Longitude), "���ȱ�����-180��180֮��");
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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