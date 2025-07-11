using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向配置值对象
/// </summary>
public class TargetingConfig : ValueObject
{
    /// <summary>
    /// 行政区划地理定向
    /// </summary>
    public AdministrativeGeoTargeting? AdministrativeGeoTargeting { get; private set; }

    /// <summary>
    /// 圆形地理围栏定向
    /// </summary>
    public CircularGeoFenceTargeting? CircularGeoFenceTargeting { get; private set; }

    /// <summary>
    /// 多边形地理围栏定向
    /// </summary>
    public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting { get; private set; }

    /// <summary>
    /// 人口属性定向
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; private set; }

    /// <summary>
    /// 设备定向
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; private set; }

    /// <summary>
    /// 时间定向
    /// </summary>
    public TimeTargeting? TimeTargeting { get; private set; }

    /// <summary>
    /// 行为定向
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; private set; }

    /// <summary>
    /// 关键词定向
    /// </summary>
    public IReadOnlyList<string> Keywords { get; private set; }

    /// <summary>
    /// 兴趣标签定向
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; }

    /// <summary>
    /// 定向权重
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingConfig(
        AdministrativeGeoTargeting? administrativeGeoTargeting = null,
        CircularGeoFenceTargeting? circularGeoFenceTargeting = null,
        PolygonGeoFenceTargeting? polygonGeoFenceTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m)
    {
        AdministrativeGeoTargeting = administrativeGeoTargeting;
        CircularGeoFenceTargeting = circularGeoFenceTargeting;
        PolygonGeoFenceTargeting = polygonGeoFenceTargeting;
        DemographicTargeting = demographicTargeting;
        DeviceTargeting = deviceTargeting;
        TimeTargeting = timeTargeting;
        BehaviorTargeting = behaviorTargeting;
        Keywords = keywords ?? Array.Empty<string>();
        InterestTags = interestTags ?? Array.Empty<string>();
        Weight = weight;
    }

    /// <summary>
    /// 创建定向配置
    /// </summary>
    public static TargetingConfig Create(
        AdministrativeGeoTargeting? administrativeGeoTargeting = null,
        CircularGeoFenceTargeting? circularGeoFenceTargeting = null,
        PolygonGeoFenceTargeting? polygonGeoFenceTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m)
    {
        if (weight <= 0)
            throw new ArgumentException("定向权重必须大于0", nameof(weight));

        return new TargetingConfig(
            administrativeGeoTargeting,
            circularGeoFenceTargeting,
            polygonGeoFenceTargeting,
            demographicTargeting,
            deviceTargeting,
            timeTargeting,
            behaviorTargeting,
            keywords,
            interestTags,
            weight);
    }

    /// <summary>
    /// 获取所有已启用的定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteria()
    {
        if (AdministrativeGeoTargeting != null && AdministrativeGeoTargeting.IsEnabled)
            yield return AdministrativeGeoTargeting;

        if (CircularGeoFenceTargeting != null && CircularGeoFenceTargeting.IsEnabled)
            yield return CircularGeoFenceTargeting;

        if (PolygonGeoFenceTargeting != null && PolygonGeoFenceTargeting.IsEnabled)
            yield return PolygonGeoFenceTargeting;

        if (DemographicTargeting != null && DemographicTargeting.IsEnabled)
            yield return DemographicTargeting;

        if (DeviceTargeting != null && DeviceTargeting.IsEnabled)
            yield return DeviceTargeting;

        if (TimeTargeting != null && TimeTargeting.IsEnabled)
            yield return TimeTargeting;

        if (BehaviorTargeting != null && BehaviorTargeting.IsEnabled)
            yield return BehaviorTargeting;
    }

    /// <summary>
    /// 获取所有已启用的地理定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledGeoTargetingCriteria()
    {
        if (AdministrativeGeoTargeting != null && AdministrativeGeoTargeting.IsEnabled)
            yield return AdministrativeGeoTargeting;

        if (CircularGeoFenceTargeting != null && CircularGeoFenceTargeting.IsEnabled)
            yield return CircularGeoFenceTargeting;

        if (PolygonGeoFenceTargeting != null && PolygonGeoFenceTargeting.IsEnabled)
            yield return PolygonGeoFenceTargeting;
    }

    /// <summary>
    /// 检查是否有任何地理定向条件
    /// </summary>
    public bool HasGeoTargeting()
    {
        return GetEnabledGeoTargetingCriteria().Any();
    }

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    public bool IsValid()
    {
        // 至少需要一个定向条件
        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return false;

        // 验证所有启用的条件都是有效的
        return enabledCriteria.All(criteria => criteria.IsValid());
    }

    /// <summary>
    /// 获取配置摘要
    /// </summary>
    public string GetConfigurationSummary()
    {
        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return "TargetingConfig: No criteria defined";

        var criteriaTypes = enabledCriteria.Select(c => c.CriteriaType);
        var keywordInfo = Keywords.Any() ? $", Keywords: {Keywords.Count}" : "";
        var tagInfo = InterestTags.Any() ? $", Tags: {InterestTags.Count}" : "";

        return $"TargetingConfig: {string.Join(", ", criteriaTypes)} (Weight: {Weight:F2}){keywordInfo}{tagInfo}";
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AdministrativeGeoTargeting ?? new object();
        yield return CircularGeoFenceTargeting ?? new object();
        yield return PolygonGeoFenceTargeting ?? new object();
        yield return DemographicTargeting ?? new object();
        yield return DeviceTargeting ?? new object();
        yield return TimeTargeting ?? new object();
        yield return BehaviorTargeting ?? new object();
        yield return Weight;

        foreach (var keyword in Keywords.OrderBy(x => x))
        {
            yield return keyword;
        }

        foreach (var tag in InterestTags.OrderBy(x => x))
        {
            yield return tag;
        }
    }
}





