using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向配置值对象
/// </summary>
public class TargetingConfig : ValueObject
{
    /// <summary>
    /// 地理位置定向
    /// </summary>
    public GeoTargeting? GeoTargeting { get; private set; }

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
        GeoTargeting? geoTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m)
    {
        GeoTargeting = geoTargeting;
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
        GeoTargeting? geoTargeting = null,
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
            geoTargeting,
            demographicTargeting,
            deviceTargeting,
            timeTargeting,
            behaviorTargeting,
            keywords,
            interestTags,
            weight);
    }

    /// <summary>
    /// 计算匹配分数
    /// </summary>
    public double CalculateMatchScore(AdContext context)
    {
        if (context == null)
            return 0.0;

        double totalScore = 0.0;
        int factors = 0;

        // 地理位置匹配
        if (GeoTargeting != null)
        {
            totalScore += GeoTargeting.IsMatch(context.GeoLocation) ? 1.0 : 0.0;
            factors++;
        }

        // 设备匹配
        if (DeviceTargeting != null && context.Device != null)
        {
            totalScore += DeviceTargeting.IsMatch(context.Device) ? 1.0 : 0.0;
            factors++;
        }

        // 时间匹配
        if (TimeTargeting != null)
        {
            totalScore += TimeTargeting.IsActiveAt(context.RequestTime) ? 1.0 : 0.0;
            factors++;
        }

        // 人口属性匹配（需要用户画像）
        if (DemographicTargeting != null && context.UserProfile.Any())
        {
            // 简化的匹配逻辑，实际需要根据用户画像数据进行匹配
            totalScore += 0.5; // 假设部分匹配
            factors++;
        }

        return factors > 0 ? (totalScore / factors) * (double)Weight : 0.0;
    }

    /// <summary>
    /// 检查是否匹配
    /// </summary>
    public bool IsMatch(AdContext context)
    {
        if (context == null)
            return false;

        // 地理位置匹配检查
        if (GeoTargeting != null && !GeoTargeting.IsMatch(context.GeoLocation))
            return false;

        // 设备匹配检查
        if (DeviceTargeting != null && context.Device != null && !DeviceTargeting.IsMatch(context.Device))
            return false;

        // 时间匹配检查
        if (TimeTargeting != null && !TimeTargeting.IsActiveAt(context.RequestTime))
            return false;

        return true;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GeoTargeting ?? new object();
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



