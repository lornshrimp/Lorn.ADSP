using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 地理位置定向
/// </summary>
public class GeoTargeting : ValueObject
{
    /// <summary>
    /// 包含的地理位置列表
    /// </summary>
    public IReadOnlyList<GeoInfo> IncludedLocations { get; private set; }


    /// <summary>
    /// 获取排除的地理位置列表
    /// </summary>
    public IReadOnlyList<GeoInfo> ExcludedLocations { get; private set; }

    /// <summary>
    /// 地理定向模式
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

        // 检查排除列表
        if (ExcludedLocations.Any(loc => IsLocationMatch(userLocation, loc)))
            return false;

        // 检查包含列表
        if (IncludedLocations.Any())
        {
            return IncludedLocations.Any(loc => IsLocationMatch(userLocation, loc));
        }

        return true;
    }

    private bool IsLocationMatch(GeoInfo userLocation, GeoInfo targetLocation)
    {
        // 国家匹配
        if (!string.IsNullOrEmpty(targetLocation.CountryName) &&
            !string.Equals(userLocation.CountryName, targetLocation.CountryName, StringComparison.OrdinalIgnoreCase))
            return false;

        // 省份匹配
        if (!string.IsNullOrEmpty(targetLocation.ProvinceName) &&
            !string.Equals(userLocation.ProvinceName, targetLocation.ProvinceName, StringComparison.OrdinalIgnoreCase))
            return false;

        // 城市匹配
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
    /// 计算地理位置匹配度
    /// </summary>
    public decimal CalculateMatchScore(GeoInfo? userLocation)
    {
        if (userLocation == null)
            return 1.0m; // 如果用户地理信息为空，默认匹配度为100%

        // 检查用户地理位置是否匹配包含的地理位置
        if (IncludedLocations.Any(location => IsLocationMatch(userLocation, location)))
            return 1.0m;

        // 检查用户地理位置是否匹配排除的地理位置
        if (ExcludedLocations.Any(location => IsLocationMatch(userLocation, location)))
            return 0.0m;

        // 如果没有明确匹配或排除，返回默认匹配度
        return Mode == GeoTargetingMode.Include ? 0.0m : 1.0m;
    }
}
