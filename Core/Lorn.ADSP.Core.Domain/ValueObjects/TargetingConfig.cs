using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向配置值对象
/// TargetingPolicy在AdCandidate中的运行时配置实例，支持动态优化和个性化调整
/// 实现基于字典的可扩展定向条件管理，确保系统的灵活性和可扩展性
/// </summary>
public class TargetingConfig : ValueObject
{
    private readonly Dictionary<string, ITargetingCriteria> _criteria;
    private readonly Dictionary<string, object> _dynamicParameters;

    /// <summary>
    /// 配置标识
    /// </summary>
    public string ConfigId { get; private set; }

    /// <summary>
    /// 关联的广告ID
    /// </summary>
    public string AdvertisementId { get; private set; }

    /// <summary>
    /// 来源策略ID（如果从TargetingPolicy创建）
    /// </summary>
    public string? SourcePolicyId { get; private set; }

    /// <summary>
    /// 定向条件集合（只读）
    /// 使用字典结构支持可扩展的定向条件类型管理
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingCriteria> Criteria => _criteria.AsReadOnly();

    /// <summary>
    /// 动态参数集合（只读）
    /// 支持运行时优化和个性化调整的参数配置
    /// </summary>
    public IReadOnlyDictionary<string, object> DynamicParameters => _dynamicParameters.AsReadOnly();

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
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// 创建来源类型
    /// </summary>
    public string CreatedFrom { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingConfig(
        string configId,
        string advertisementId,
        string? sourcePolicyId = null,
        IDictionary<string, ITargetingCriteria>? criteria = null,
        IDictionary<string, object>? dynamicParameters = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m,
        bool isEnabled = true,
        string createdFrom = "Manual")
    {
        ConfigId = configId;
        AdvertisementId = advertisementId;
        SourcePolicyId = sourcePolicyId;
        _criteria = criteria?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, ITargetingCriteria>();
        _dynamicParameters = dynamicParameters?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, object>();
        Keywords = keywords ?? Array.Empty<string>();
        InterestTags = interestTags ?? Array.Empty<string>();
        Weight = weight;
        IsEnabled = isEnabled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedFrom = createdFrom;

        ValidateWeight(weight);
    }

    /// <summary>
    /// 从TargetingPolicy创建TargetingConfig实例
    /// </summary>
    /// <param name="policy">源策略</param>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="configId">配置ID（可选，如为空则自动生成）</param>
    /// <returns>配置实例</returns>
    public static TargetingConfig CreateFromPolicy(TargetingPolicy policy, string advertisementId, string? configId = null)
    {
        if (policy == null)
            throw new ArgumentNullException(nameof(policy));
        if (string.IsNullOrEmpty(advertisementId))
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        var finalConfigId = configId ?? Guid.NewGuid().ToString();

        // 从策略复制定向条件
        var criteriaDict = new Dictionary<string, ITargetingCriteria>();
        foreach (var criteriaKvp in policy.CriteriaTemplates)
        {
            criteriaDict[criteriaKvp.Key] = criteriaKvp.Value;
        }

        return new TargetingConfig(
            configId: finalConfigId,
            advertisementId: advertisementId,
            sourcePolicyId: policy.PolicyId,
            criteria: criteriaDict,
            weight: policy.Weight,
            isEnabled: policy.IsEnabled,
            createdFrom: "Policy");
    }

    /// <summary>
    /// 从头创建TargetingConfig实例
    /// </summary>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="configId">配置ID（可选，如为空则自动生成）</param>
    /// <param name="criteria">定向条件</param>
    /// <param name="dynamicParameters">动态参数</param>
    /// <param name="keywords">关键词</param>
    /// <param name="interestTags">兴趣标签</param>
    /// <param name="weight">权重</param>
    /// <param name="isEnabled">是否启用</param>
    /// <returns>配置实例</returns>
    public static TargetingConfig CreateFromScratch(
        string advertisementId,
        string? configId = null,
        IDictionary<string, ITargetingCriteria>? criteria = null,
        IDictionary<string, object>? dynamicParameters = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        if (string.IsNullOrEmpty(advertisementId))
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        var finalConfigId = configId ?? Guid.NewGuid().ToString();

        return new TargetingConfig(
            configId: finalConfigId,
            advertisementId: advertisementId,
            criteria: criteria,
            dynamicParameters: dynamicParameters,
            keywords: keywords,
            interestTags: interestTags,
            weight: weight,
            isEnabled: isEnabled,
            createdFrom: "Manual");
    }

    /// <summary>
    /// 添加定向条件
    /// </summary>
    /// <param name="criteria">定向条件</param>
    public void AddCriteria(ITargetingCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        _criteria[criteria.CriteriaType] = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新定向条件
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <param name="criteria">定向条件</param>
    public void UpdateCriteria(string criteriaType, ITargetingCriteria criteria)
    {
        if (string.IsNullOrEmpty(criteriaType))
            throw new ArgumentException("条件类型不能为空", nameof(criteriaType));
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));
        if (criteria.CriteriaType != criteriaType)
            throw new ArgumentException("条件类型不匹配", nameof(criteriaType));

        _criteria[criteriaType] = criteria;
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
    /// 设置动态参数
    /// </summary>
    /// <param name="parameterKey">参数键</param>
    /// <param name="parameterValue">参数值</param>
    public void SetDynamicParameter(string parameterKey, object parameterValue)
    {
        if (string.IsNullOrEmpty(parameterKey))
            throw new ArgumentException("参数键不能为空", nameof(parameterKey));

        _dynamicParameters[parameterKey] = parameterValue;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取动态参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="parameterKey">参数键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>参数值</returns>
    public T GetDynamicParameter<T>(string parameterKey, T defaultValue = default!)
    {
        if (string.IsNullOrEmpty(parameterKey))
            return defaultValue;

        return _dynamicParameters.TryGetValue(parameterKey, out var value) && value is T typedValue
            ? typedValue
            : defaultValue;
    }

    /// <summary>
    /// 应用动态优化
    /// </summary>
    /// <param name="context">优化上下文</param>
    public void ApplyDynamicOptimization(OptimizationContext context)
    {
        // 实现动态优化逻辑，如调整权重、启用/禁用特定条件等
        if (context == null)
            return;

        // 示例：根据历史表现调整权重
        if (context.PerformanceMetrics != null)
        {
            var performanceScore = context.PerformanceMetrics.GetOverallScore();
            if (performanceScore > 0.8m)
            {
                Weight = Math.Min(Weight * 1.1m, 2.0m); // 提升权重，但不超过2.0
            }
            else if (performanceScore < 0.3m)
            {
                Weight = Math.Max(Weight * 0.9m, 0.1m); // 降低权重，但不低于0.1
            }
        }

        // 根据优化建议调整定向条件
        if (context.OptimizationRecommendations != null)
        {
            foreach (var recommendation in context.OptimizationRecommendations)
            {
                ApplyRecommendation(recommendation);
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取所有已启用的定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteria()
    {
        return _criteria.Values.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// 获取所有已启用的地理定向条件
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledGeoTargetingCriteria()
    {
        return GetEnabledCriteria().Where(c => IsGeoTargetingType(c.CriteriaType));
    }

    /// <summary>
    /// 检查是否有任何地理定向条件
    /// </summary>
    public bool HasGeoTargeting()
    {
        return GetEnabledGeoTargetingCriteria().Any();
    }

    /// <summary>
    /// 获取所有定向条件类型
    /// </summary>
    public IReadOnlyCollection<string> GetCriteriaTypes()
    {
        return _criteria.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    public ValidationResult ValidateConfig()
    {
        var validationResult = new ValidationResult();

        // 验证基本属性
        if (string.IsNullOrEmpty(ConfigId))
            validationResult.AddError("配置ID不能为空");

        if (string.IsNullOrEmpty(AdvertisementId))
            validationResult.AddError("广告ID不能为空");

        if (Weight <= 0)
            validationResult.AddError("权重必须大于0");

        // 如果启用，至少需要一个定向条件
        if (IsEnabled)
        {
            var enabledCriteria = GetEnabledCriteria().ToList();
            if (!enabledCriteria.Any() && !Keywords.Any() && !InterestTags.Any())
                validationResult.AddError("启用状态下至少需要一个定向条件");

            // 验证所有启用的条件都是有效的
            foreach (var criteria in enabledCriteria)
            {
                if (!criteria.IsValid())
                    validationResult.AddError($"定向条件 {criteria.CriteriaType} 无效");
            }
        }

        return validationResult;
    }

    /// <summary>
    /// 检查配置是否有效
    /// </summary>
    public bool IsValid()
    {
        return ValidateConfig().IsValid;
    }

    /// <summary>
    /// 克隆配置
    /// </summary>
    /// <param name="newAdvertisementId">新的广告ID（可选）</param>
    /// <returns>克隆的配置</returns>
    public TargetingConfig Clone(string? newAdvertisementId = null)
    {
        var targetAdId = newAdvertisementId ?? AdvertisementId;
        var newConfigId = Guid.NewGuid().ToString();

        var clonedCriteria = new Dictionary<string, ITargetingCriteria>();
        foreach (var criteriaKvp in _criteria)
        {
            // 假设ITargetingCriteria实现了深拷贝方法
            clonedCriteria[criteriaKvp.Key] = criteriaKvp.Value; // 这里需要实现深拷贝
        }

        var clonedDynamicParams = new Dictionary<string, object>(_dynamicParameters);

        return new TargetingConfig(
            configId: newConfigId,
            advertisementId: targetAdId,
            sourcePolicyId: SourcePolicyId,
            criteria: clonedCriteria,
            dynamicParameters: clonedDynamicParams,
            keywords: Keywords.ToList(),
            interestTags: InterestTags.ToList(),
            weight: Weight,
            isEnabled: IsEnabled,
            createdFrom: "Cloned");
    }

    /// <summary>
    /// 获取配置摘要
    /// </summary>
    public string GetConfigurationSummary()
    {
        var enabledCriteria = GetEnabledCriteria().ToList();
        var criteriaInfo = enabledCriteria.Any()
            ? string.Join(", ", enabledCriteria.Select(c => c.CriteriaType))
            : "No criteria";

        var keywordInfo = Keywords.Any() ? $", Keywords: {Keywords.Count}" : "";
        var tagInfo = InterestTags.Any() ? $", Tags: {InterestTags.Count}" : "";
        var dynamicInfo = _dynamicParameters.Any() ? $", DynamicParams: {_dynamicParameters.Count}" : "";

        return $"TargetingConfig[{ConfigId}]: {criteriaInfo} (Weight: {Weight:F2}, Source: {CreatedFrom}){keywordInfo}{tagInfo}{dynamicInfo}";
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ConfigId;
        yield return AdvertisementId;
        yield return SourcePolicyId ?? string.Empty;
        yield return Weight;
        yield return IsEnabled;
        yield return CreatedFrom;

        // 按条件类型排序，确保一致的比较结果
        foreach (var criteria in _criteria.OrderBy(kv => kv.Key))
        {
            yield return criteria.Key;
            yield return criteria.Value;
        }

        // 按参数键排序，确保一致的比较结果
        foreach (var param in _dynamicParameters.OrderBy(kv => kv.Key))
        {
            yield return param.Key;
            yield return param.Value;
        }

        foreach (var keyword in Keywords.OrderBy(x => x))
        {
            yield return keyword;
        }

        foreach (var tag in InterestTags.OrderBy(x => x))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// 验证权重值
    /// </summary>
    /// <param name="weight">权重值</param>
    private static void ValidateWeight(decimal weight)
    {
        if (weight <= 0)
            throw new ArgumentException("权重必须大于0", nameof(weight));
    }

    /// <summary>
    /// 判断是否为地理定向类型
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <returns>是否为地理定向</returns>
    private static bool IsGeoTargetingType(string criteriaType)
    {
        return criteriaType switch
        {
            "AdministrativeGeo" or "CircularGeoFence" or "PolygonGeoFence" => true,
            _ => false
        };
    }

    /// <summary>
    /// 应用优化建议
    /// </summary>
    /// <param name="recommendation">优化建议</param>
    private void ApplyRecommendation(OptimizationRecommendation recommendation)
    {
        switch (recommendation.Type)
        {
            case OptimizationType.AdjustWeight:
                if (recommendation.NewWeight.HasValue)
                {
                    Weight = Math.Max(0.1m, Math.Min(2.0m, recommendation.NewWeight.Value));
                }
                break;

            case OptimizationType.EnableCriteria:
                if (!string.IsNullOrEmpty(recommendation.CriteriaType) &&
                    _criteria.TryGetValue(recommendation.CriteriaType, out var criteria))
                {
                    // 需要在ITargetingCriteria接口中添加Enable/Disable方法
                    // criteria.Enable();
                }
                break;

            case OptimizationType.DisableCriteria:
                if (!string.IsNullOrEmpty(recommendation.CriteriaType) &&
                    _criteria.TryGetValue(recommendation.CriteriaType, out var criteriaToDisable))
                {
                    // criteriaToDisable.Disable();
                }
                break;

            case OptimizationType.SetDynamicParameter:
                if (!string.IsNullOrEmpty(recommendation.ParameterKey) &&
                    recommendation.ParameterValue != null)
                {
                    SetDynamicParameter(recommendation.ParameterKey, recommendation.ParameterValue);
                }
                break;
        }
    }
}

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    public bool IsValid => !_errors.Any();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();
    public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();

    public void AddError(string error)
    {
        _errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        _warnings.Add(warning);
    }
}

/// <summary>
/// 优化上下文
/// </summary>
public class OptimizationContext
{
    public PerformanceMetrics? PerformanceMetrics { get; set; }
    public List<OptimizationRecommendation>? OptimizationRecommendations { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}

/// <summary>
/// 性能指标
/// </summary>
public class PerformanceMetrics
{
    public decimal CTR { get; set; }
    public decimal CVR { get; set; }
    public decimal CPC { get; set; }
    public decimal ROI { get; set; }

    public decimal GetOverallScore()
    {
        // 简化的综合评分计算
        return (CTR * 0.3m + CVR * 0.4m + ROI * 0.3m);
    }
}

/// <summary>
/// 优化建议
/// </summary>
public class OptimizationRecommendation
{
    public OptimizationType Type { get; set; }
    public string? CriteriaType { get; set; }
    public decimal? NewWeight { get; set; }
    public string? ParameterKey { get; set; }
    public object? ParameterValue { get; set; }
    public string? Reason { get; set; }
}











