using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Entities;
using System.Text.Json;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向配置值对象
/// TargetingPolicy在AdCandidate中实例化为具体的运行时配置
/// 支持动态优化和个性化调整
/// 实现基于集合的可扩展定向条件管理，确保系统的灵活性和可扩展性
/// </summary>
public class TargetingConfig : ValueObject
{
    /// <summary>
    /// 关联的广告ID
    /// </summary>
    public Guid AdvertisementId { get; private set; }

    /// <summary>
    /// 来源策略ID（如果从TargetingPolicy创建）
    /// </summary>
    public Guid? SourcePolicyId { get; private set; }

    /// <summary>
    /// 定向条件集合
    /// 支持可扩展的定向条件类型管理
    /// </summary>
    public IReadOnlyList<ITargetingCriteria> Criteria { get; private set; }

    /// <summary>
    /// 动态参数集合
    /// 支持运行时优化和个性化调整的参数配置
    /// </summary>
    public IReadOnlyList<ContextProperty> DynamicParameterProperties { get; private set; }

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
        Guid advertisementId,
        Guid? sourcePolicyId = null,
        IEnumerable<ITargetingCriteria>? criteria = null,
        IDictionary<string, object>? dynamicParameters = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m,
        bool isEnabled = true,
        string createdFrom = "Manual")
    {
        AdvertisementId = advertisementId;
        SourcePolicyId = sourcePolicyId;
        Criteria = criteria?.ToList() ?? new List<ITargetingCriteria>();

        // 将动态参数字典转换为 ContextProperty 集合
        DynamicParameterProperties = dynamicParameters?.Select(kvp =>
            new ContextProperty(
                kvp.Key,
                JsonSerializer.Serialize(kvp.Value),
                kvp.Value?.GetType().Name ?? "object",
                "Dynamic",
                false,
                1.0m,
                null,
                "TargetingConfig")).ToList() ?? new List<ContextProperty>();

        Keywords = keywords ?? new List<string>();
        InterestTags = interestTags ?? new List<string>();
        Weight = weight;
        IsEnabled = isEnabled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedFrom = createdFrom;
    }

    /// <summary>
    /// 从TargetingPolicy创建运行时配置实例
    /// </summary>
    /// <param name="policy">定向策略</param>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="runtimeParameters">运行时参数</param>
    /// <returns>定向配置实例</returns>
    public static TargetingConfig CreateFromPolicy(
        TargetingPolicy policy,
        Guid advertisementId,
        IDictionary<string, object>? runtimeParameters = null)
    {
        if (policy == null)
            throw new ArgumentNullException(nameof(policy));
        if (advertisementId == Guid.Empty)
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        return new TargetingConfig(
            advertisementId: advertisementId,
            sourcePolicyId: policy.Id,
            criteria: policy.CriteriaTemplates,
            dynamicParameters: runtimeParameters,
            keywords: new List<string>(),
            interestTags: new List<string>(),
            weight: policy.Weight,
            isEnabled: policy.IsEnabled,
            createdFrom: "Policy");
    }

    /// <summary>
    /// 手工创建定向配置
    /// </summary>
    public static TargetingConfig Create(
        Guid advertisementId,
        IEnumerable<ITargetingCriteria>? criteria = null,
        IDictionary<string, object>? dynamicParameters = null,
        IReadOnlyList<string>? keywords = null,
        IReadOnlyList<string>? interestTags = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        return new TargetingConfig(
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
    /// 添加或更新定向条件
    /// </summary>
    /// <param name="criteria">定向条件</param>
    /// <returns>包含新条件的配置实例</returns>
    public TargetingConfig WithCriteria(ITargetingCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        var newCriteria = Criteria.Where(c => c.CriteriaType != criteria.CriteriaType)
                                 .Concat(new[] { criteria })
                                 .ToList();

        return new TargetingConfig(
            AdvertisementId,
            SourcePolicyId,
            newCriteria,
            DynamicParameterProperties.ToDictionary(p => p.PropertyKey, p => (object)p.PropertyValue),
            Keywords,
            InterestTags,
            Weight,
            IsEnabled,
            CreatedFrom);
    }

    /// <summary>
    /// 移除定向条件
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <returns>移除指定条件后的配置实例</returns>
    public TargetingConfig WithoutCriteria(string criteriaType)
    {
        if (string.IsNullOrEmpty(criteriaType))
            return this;

        var newCriteria = Criteria.Where(c => c.CriteriaType != criteriaType).ToList();

        return new TargetingConfig(
            AdvertisementId,
            SourcePolicyId,
            newCriteria,
            DynamicParameterProperties.ToDictionary(p => p.PropertyKey, p => (object)p.PropertyValue),
            Keywords,
            InterestTags,
            Weight,
            IsEnabled,
            CreatedFrom);
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

        return Criteria.FirstOrDefault(c => c.CriteriaType == criteriaType) as T;
    }

    /// <summary>
    /// 检查是否包含指定类型的定向条件
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <returns>是否包含</returns>
    public bool HasCriteria(string criteriaType)
    {
        return !string.IsNullOrEmpty(criteriaType) && Criteria.Any(c => c.CriteriaType == criteriaType);
    }

    /// <summary>
    /// 添加动态参数
    /// </summary>
    /// <param name="key">参数键</param>
    /// <param name="value">参数值</param>
    /// <returns>包含新参数的配置实例</returns>
    public TargetingConfig WithDynamicParameter(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("参数键不能为空", nameof(key));

        var newProperties = DynamicParameterProperties.Where(p => p.PropertyKey != key)
                                                     .Concat(new[]
                                                     {
                                                         new ContextProperty(
                                                             key,
                                                             JsonSerializer.Serialize(value),
                                                             value?.GetType().Name ?? "object",
                                                             "Dynamic",
                                                             false,
                                                             1.0m,
                                                             null,
                                                             "TargetingConfig")
                                                     }).ToList();

        return new TargetingConfig(
            AdvertisementId,
            SourcePolicyId,
            Criteria,
            newProperties.ToDictionary(p => p.PropertyKey, p => (object)p.PropertyValue),
            Keywords,
            InterestTags,
            Weight,
            IsEnabled,
            CreatedFrom);
    }

    /// <summary>
    /// 获取动态参数值
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="key">参数键</param>
    /// <returns>参数值，如果不存在则返回默认值</returns>
    public T? GetDynamicParameter<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default;

        var property = DynamicParameterProperties.FirstOrDefault(p => p.PropertyKey == key);
        if (property == null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(property.PropertyValue);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 检查运行时有效性
    /// </summary>
    /// <returns>验证结果</returns>
    public TargetingValidationResult ValidateRuntimeEffectiveness()
    {
        var validationErrors = new List<string>();
        var validationWarnings = new List<string>();

        // 检查是否启用
        if (!IsEnabled)
        {
            validationWarnings.Add("配置未启用");
        }

        // 检查是否有有效的定向条件
        var enabledCriteria = GetEnabledCriteria();
        if (!enabledCriteria.Any())
        {
            validationWarnings.Add("没有启用的定向条件");
        }

        // 检查权重合理性
        if (Weight <= 0)
        {
            validationErrors.Add("权重必须大于0");
        }

        return new TargetingValidationResult
        {
            IsValid = !validationErrors.Any(),
            Errors = validationErrors,
            Warnings = validationWarnings
        };
    }

    /// <summary>
    /// 获取启用的定向条件
    /// </summary>
    /// <returns>启用的定向条件集合</returns>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteria()
    {
        return Criteria.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// 获取所有定向条件类型
    /// </summary>
    /// <returns>条件类型列表</returns>
    public IReadOnlyList<string> GetCriteriaTypes()
    {
        return Criteria.Select(c => c.CriteriaType).ToList().AsReadOnly();
    }

    /// <summary>
    /// 计算定向强度
    /// 基于启用的条件数量和权重计算
    /// </summary>
    /// <returns>定向强度值</returns>
    public decimal CalculateTargetingStrength()
    {
        if (!IsEnabled)
            return 0;

        var enabledCriteria = GetEnabledCriteria();
        var criteriaCount = enabledCriteria.Count();

        if (criteriaCount == 0)
            return 0;

        // 基础强度 = 权重 * 启用条件数量 / 总条件数量
        var baseStrength = Weight * criteriaCount / Math.Max(Criteria.Count, 1);

        // 应用条件权重加成
        var weightedStrength = enabledCriteria.Sum(c => c.Weight) / criteriaCount;

        return baseStrength * weightedStrength;
    }

    /// <summary>
    /// 创建配置副本
    /// </summary>
    /// <param name="newConfigId">新的配置ID（不再使用）</param>
    /// <returns>配置副本</returns>
    public TargetingConfig Clone(string? newConfigId = null)
    {
        var clonedCriteria = Criteria.ToList();

        return new TargetingConfig(
            advertisementId: AdvertisementId,
            sourcePolicyId: SourcePolicyId,
            criteria: clonedCriteria,
            dynamicParameters: DynamicParameterProperties.ToDictionary(p => p.PropertyKey, p => (object)p.PropertyValue),
            keywords: Keywords,
            interestTags: InterestTags,
            weight: Weight,
            isEnabled: IsEnabled,
            createdFrom: $"Clone_{CreatedFrom}");
    }

    /// <summary>
    /// 生成配置摘要
    /// </summary>
    /// <returns>摘要字符串</returns>
    public string GenerateConfigSummary()
    {
        var summary = new System.Text.StringBuilder();
        summary.AppendLine($"配置ID: {Id}");
        summary.AppendLine($"广告ID: {AdvertisementId}");
        summary.AppendLine($"来源策略: {(SourcePolicyId.HasValue ? SourcePolicyId.ToString() : "手工创建")}");
        summary.AppendLine($"启用状态: {(IsEnabled ? "启用" : "禁用")}");
        summary.AppendLine($"权重: {Weight}");
        summary.AppendLine($"条件数量: {Criteria.Count}");
        summary.AppendLine($"启用条件: {GetEnabledCriteria().Count()}");

        foreach (var criteria in Criteria.OrderBy(c => c.CriteriaType))
        {
            summary.AppendLine($"  - {criteria.CriteriaType}: {(criteria.IsEnabled ? "启用" : "禁用")}");
        }

        return summary.ToString();
    }

    /// <summary>
    /// 根据优化建议应用调整
    /// </summary>
    /// <param name="recommendations">优化建议列表</param>
    /// <returns>优化后的配置实例</returns>
    public TargetingConfig ApplyOptimizationRecommendations(IEnumerable<OptimizationRecommendation> recommendations)
    {
        if (recommendations == null || !recommendations.Any())
            return this;

        var newConfig = this;

        foreach (var recommendation in recommendations)
        {
            switch (recommendation.Type)
            {
                case OptimizationType.EnableCriteria:
                    if (recommendation.CriteriaType != null)
                    {
                        var criteriaToEnable = Criteria.FirstOrDefault(c => c.CriteriaType == recommendation.CriteriaType);
                        if (criteriaToEnable != null && !criteriaToEnable.IsEnabled)
                        {
                            // 这里需要具体的Enable方法实现，假设存在
                            // newConfig = newConfig.WithCriteria(criteriaToEnable.Enable());
                        }
                    }
                    break;

                case OptimizationType.DisableCriteria:
                    if (recommendation.CriteriaType != null)
                    {
                        var criteriaToDisable = Criteria.FirstOrDefault(c => c.CriteriaType == recommendation.CriteriaType);
                        if (criteriaToDisable != null && criteriaToDisable.IsEnabled)
                        {
                            // 这里需要具体的Disable方法实现，假设存在
                            // newConfig = newConfig.WithCriteria(criteriaToDisable.Disable());
                        }
                    }
                    break;

                case OptimizationType.AdjustWeight:
                    // 调整权重的逻辑
                    break;

                case OptimizationType.SetDynamicParameter:
                    if (recommendation.ParameterKey != null && recommendation.ParameterValue != null)
                    {
                        newConfig = newConfig.WithDynamicParameter(recommendation.ParameterKey, recommendation.ParameterValue);
                    }
                    break;
            }
        }

        return newConfig;
    }

    /// <summary>
    /// 相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return AdvertisementId;
        yield return SourcePolicyId ?? Guid.Empty;
        yield return Weight;
        yield return IsEnabled;
        yield return CreatedFrom;

        foreach (var criteria in Criteria.OrderBy(c => c.CriteriaType))
        {
            yield return criteria.CriteriaType;
            yield return criteria.IsEnabled;
        }

        foreach (var property in DynamicParameterProperties.OrderBy(p => p.PropertyKey))
        {
            yield return property.PropertyKey;
            yield return property.PropertyValue;
        }
    }
}

/// <summary>
/// 定向验证结果
/// </summary>
public class TargetingValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
