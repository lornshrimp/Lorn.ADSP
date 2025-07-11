using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向策略值对象
/// </summary>
public class TargetingPolicy : ValueObject
{
    private readonly Dictionary<string, ITargetingCriteria> _criteria;

    /// <summary>
    /// 定向条件集合（只读）
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingCriteria> Criteria => _criteria.AsReadOnly();

    /// <summary>
    /// 策略权重
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// 是否启用定向
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// 策略创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 策略更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingPolicy()
    {
        _criteria = new Dictionary<string, ITargetingCriteria>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="criteria">定向条件集合</param>
    /// <param name="weight">策略权重</param>
    /// <param name="isEnabled">是否启用</param>
    public TargetingPolicy(
        IDictionary<string, ITargetingCriteria>? criteria = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        _criteria = criteria?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, ITargetingCriteria>();
        Weight = weight;
        IsEnabled = isEnabled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateWeight(weight);
    }

    /// <summary>
    /// 创建空的定向策略
    /// </summary>
    public static TargetingPolicy CreateEmpty()
    {
        return new TargetingPolicy();
    }

    /// <summary>
    /// 创建全开放无限制（不定向）
    /// </summary>
    public static TargetingPolicy CreateUnrestricted()
    {
        return new TargetingPolicy(isEnabled: false);
    }

    /// <summary>
    /// 添加或更新定向条件
    /// </summary>
    /// <param name="criteria">定向条件</param>
    public void AddOrUpdateCriteria(ITargetingCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        _criteria[criteria.CriteriaType] = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 移除定向条件
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveCriteria(string criteriaType)
    {
        if (string.IsNullOrEmpty(criteriaType))
            return false;

        var result = _criteria.Remove(criteriaType);
        if (result)
        {
            UpdatedAt = DateTime.UtcNow;
        }
        return result;
    }

    /// <summary>
    /// 获取指定类型的定向条件
    /// </summary>
    /// <typeparam name="T">条件类型</typeparam>
    /// <param name="criteriaType">条件类型标识</param>
    /// <returns>定向条件，如果不存在则返回null</returns>
    public T? GetCriteria<T>(string criteriaType) where T : class, ITargetingCriteria
    {
        if (string.IsNullOrEmpty(criteriaType))
            return null;

        return _criteria.TryGetValue(criteriaType, out var criteria) ? criteria as T : null;
    }

    /// <summary>
    /// 检查是否包含指定类型的定向条件
    /// </summary>
    /// <param name="criteriaType">条件类型标识</param>
    /// <returns>是否包含</returns>
    public bool HasCriteria(string criteriaType)
    {
        return !string.IsNullOrEmpty(criteriaType) && _criteria.ContainsKey(criteriaType);
    }

    /// <summary>
    /// 获取所有已启用的定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteria()
    {
        return _criteria.Values.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// 获取所有定向条件类型
    /// </summary>
    public IReadOnlyCollection<string> GetCriteriaTypes()
    {
        return _criteria.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 验证策略是否有效
    /// </summary>
    public bool IsValid()
    {
        if (!IsEnabled)
            return true;

        // 至少需要一个定向条件
        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return false;

        // 验证所有启用的条件都是有效的
        return enabledCriteria.All(criteria => criteria.IsValid());
    }

    /// <summary>
    /// 获取策略配置摘要
    /// </summary>
    public string GetConfigurationSummary()
    {
        if (!IsEnabled)
            return "Targeting: Disabled (Unrestricted)";

        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return "Targeting: No criteria defined";

        var criteriaTypes = enabledCriteria.Select(c => c.CriteriaType);
        return $"Targeting: {string.Join(", ", criteriaTypes)} (Weight: {Weight:F2})";
    }

    /// <summary>
    /// 更新策略权重
    /// </summary>
    /// <param name="newWeight">新权重</param>
    public void UpdateWeight(decimal newWeight)
    {
        ValidateWeight(newWeight);
        Weight = newWeight;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新策略启用状态
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public void UpdateEnabled(bool enabled)
    {
        IsEnabled = enabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Weight;
        yield return IsEnabled;

        // 按条件类型排序，确保一致的比较结果
        foreach (var criteria in _criteria.OrderBy(kv => kv.Key))
        {
            yield return criteria.Key;
            yield return criteria.Value;
        }
    }

    /// <summary>
    /// 验证权重值
    /// </summary>
    /// <param name="weight">权重值</param>
    private static void ValidateWeight(decimal weight)
    {
        if (weight < 0)
            throw new ArgumentException("权重不能为负数", nameof(weight));
    }

    // 便捷方法：为了保持向后兼容性，提供一些常用定向条件的快捷访问方法

    /// <summary>
    /// 获取行政区划地理定向条件
    /// </summary>
    public AdministrativeGeoTargeting? AdministrativeGeoTargeting => GetCriteria<AdministrativeGeoTargeting>("AdministrativeGeo");

    /// <summary>
    /// 获取圆形地理围栏定向条件
    /// </summary>
    public CircularGeoFenceTargeting? CircularGeoFenceTargeting => GetCriteria<CircularGeoFenceTargeting>("CircularGeoFence");

    /// <summary>
    /// 获取多边形地理围栏定向条件
    /// </summary>
    public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting => GetCriteria<PolygonGeoFenceTargeting>("PolygonGeoFence");

    /// <summary>
    /// 获取人口属性定向条件
    /// </summary>
    public DemographicTargeting? DemographicTargeting => GetCriteria<DemographicTargeting>("Demographic");

    /// <summary>
    /// 获取设备定向条件
    /// </summary>
    public DeviceTargeting? DeviceTargeting => GetCriteria<DeviceTargeting>("Device");

    /// <summary>
    /// 获取时间定向条件
    /// </summary>
    public TimeTargeting? TimeTargeting => GetCriteria<TimeTargeting>("Time");

    /// <summary>
    /// 获取行为定向条件
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting => GetCriteria<BehaviorTargeting>("Behavior");

    /// <summary>
    /// 获取所有地理定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetGeoTargetingCriteria()
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
        return GetGeoTargetingCriteria().Any();
    }
}









