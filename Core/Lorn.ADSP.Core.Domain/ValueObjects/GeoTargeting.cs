using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ����λ�ö���
/// </summary>
public class GeoTargeting : ValueObject
{
    /// <summary>
    /// �����ĵ���λ���б�
    /// </summary>
    public IReadOnlyList<GeoInfo> IncludedLocations { get; private set; }


    /// <summary>
    /// ��ȡ�ų��ĵ���λ���б�
    /// </summary>
    public IReadOnlyList<GeoInfo> ExcludedLocations { get; private set; }

    /// <summary>
    /// ������ģʽ
    /// </summary>
    public GeoTargetingMode Mode { get; private set; }

    private GeoTargeting(
        IReadOnlyList<GeoInfo> includedLocations,
        IReadOnlyList<GeoInfo> excludedLocations,
        GeoTargetingMode mode)
    {
        IncludedLocations = includedLocations;
        ExcludedLocations = excludedLocations;
        Mode = mode;
    }

    public static GeoTargeting Create(
        IReadOnlyList<GeoInfo> includedLocations,
        IReadOnlyList<GeoInfo>? excludedLocations = null,
        GeoTargetingMode mode = GeoTargetingMode.Include)
    {
        return new GeoTargeting(
            includedLocations ?? Array.Empty<GeoInfo>(),
            excludedLocations ?? Array.Empty<GeoInfo>(),
            mode);
    }

    public bool IsMatch(GeoInfo userLocation)
    {
        if (userLocation == null)
            return false;

        // ����ų��б�
        if (ExcludedLocations.Any(loc => IsLocationMatch(userLocation, loc)))
            return false;

        // �������б�
        if (IncludedLocations.Any())
        {
            return IncludedLocations.Any(loc => IsLocationMatch(userLocation, loc));
        }

        return true;
    }

    private bool IsLocationMatch(GeoInfo userLocation, GeoInfo targetLocation)
    {
        // ����ƥ��
        if (!string.IsNullOrEmpty(targetLocation.CountryName) &&
            !string.Equals(userLocation.CountryName, targetLocation.CountryName, StringComparison.OrdinalIgnoreCase))
            return false;

        // ʡ��ƥ��
        if (!string.IsNullOrEmpty(targetLocation.ProvinceName) &&
            !string.Equals(userLocation.ProvinceName, targetLocation.ProvinceName, StringComparison.OrdinalIgnoreCase))
            return false;

        // ����ƥ��
        if (!string.IsNullOrEmpty(targetLocation.CityName) &&
            !string.Equals(userLocation.CityName, targetLocation.CityName, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Mode;

        foreach (var location in IncludedLocations.OrderBy(x => x.CountryName))
        {
            yield return location;
        }

        foreach (var excludedLocation in ExcludedLocations.OrderBy(x => x.CountryName))
        {
            yield return excludedLocation;
        }
    }
    /// <summary>
    /// �������λ��ƥ���
    /// </summary>
    public decimal CalculateMatchScore(GeoInfo? userLocation)
    {
        if (userLocation == null)
            return 1.0m; // ����û�������ϢΪ�գ�Ĭ��ƥ���Ϊ100%

        // ����û�����λ���Ƿ�ƥ������ĵ���λ��
        if (IncludedLocations.Any(location => IsLocationMatch(userLocation, location)))
            return 1.0m;

        // ����û�����λ���Ƿ�ƥ���ų��ĵ���λ��
        if (ExcludedLocations.Any(location => IsLocationMatch(userLocation, location)))
            return 0.0m;

        // ���û����ȷƥ����ų�������Ĭ��ƥ���
        return Mode == GeoTargetingMode.Include ? 0.0m : 1.0m;
    }
}
