using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向策略模板值对象
/// 作为可复用的定向规则模template，支持创建TargetingConfig实例
/// 提供预配置的定向条件集合和版本管理功能
/// </summary>
public class TargetingPolicy : ValueObject
{
    private readonly List<string> _tags;

    /// <summary>
    /// 策略名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 策略描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 策略版本
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedBy { get; private set; }

    /// <summary>
    /// 定向条件模板集合
    /// </summary>
    public IReadOnlyList<ITargetingCriteria> CriteriaTemplates { get; private set; }

    /// <summary>
    /// 策略权重
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// 是否启用定向
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// 策略状态
    /// </summary>
    public PolicyStatus Status { get; private set; }

    /// <summary>
    /// 策略类别
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// 是否公开可用
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>
    /// 策略标签（只读）
    /// </summary>
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// 策略创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 策略更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">策略名称</param>
    /// <param name="createdBy">创建者</param>
    /// <param name="description">策略描述</param>
    /// <param name="version">版本号</param>
    /// <param name="criteriaTemplates">定向条件模板集合</param>
    /// <param name="category">策略类别</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="tags">标签列表</param>
    /// <param name="weight">策略权重</param>
    /// <param name="isEnabled">是否启用</param>
    /// <param name="status">策略状态</param>
    public TargetingPolicy(
        string name,
        string createdBy,
        string? description = null,
        int version = 1,
        IDictionary<string, ITargetingCriteria>? criteriaTemplates = null,
        string category = "General",
        bool isPublic = false,
        IList<string>? tags = null,
        decimal weight = 1.0m,
        bool isEnabled = true,
        PolicyStatus status = PolicyStatus.Draft)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("策略名称不能为空", nameof(name));
        if (string.IsNullOrEmpty(createdBy))
            throw new ArgumentException("创建者不能为空", nameof(createdBy));

        Name = name;
        Description = description;
        Version = version;
        CreatedBy = createdBy;
        CriteriaTemplates = criteriaTemplates?.Values.ToList() ?? new List<ITargetingCriteria>();
        Category = category;
        IsPublic = isPublic;
        _tags = tags?.ToList() ?? new List<string>();
        Weight = weight;
        IsEnabled = isEnabled;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateWeight(weight);
    }

    /// <summary>
    /// 创建空的定向策略
    /// </summary>
    /// <param name="name">策略名称</param>
    /// <param name="createdBy">创建者</param>
    /// <param name="category">策略类别</param>
    /// <returns>空策略实例</returns>
    public static TargetingPolicy CreateEmpty(string name, string createdBy, string category = "General")
    {
        return new TargetingPolicy(name, createdBy, category: category);
    }

    /// <summary>
    /// 创建全开放无限制策略（不定向）
    /// </summary>
    /// <param name="name">策略名称</param>
    /// <param name="createdBy">创建者</param>
    /// <returns>无限制策略实例</returns>
    public static TargetingPolicy CreateUnrestricted(string name, string createdBy)
    {
        return new TargetingPolicy(
            name: name,
            createdBy: createdBy,
            description: "无定向限制策略",
            category: "Unrestricted",
            isEnabled: false);
    }

    /// <summary>
    /// 从现有策略创建TargetingConfig实例
    /// </summary>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="runtimeParameters">运行时参数（可选）</param>
    /// <returns>配置实例</returns>
    public TargetingConfig CreateConfig(Guid advertisementId, IDictionary<string, object>? runtimeParameters = null)
    {
        if (advertisementId == Guid.Empty)
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        return TargetingConfig.CreateFromPolicy(this, advertisementId, runtimeParameters);
    }

    /// <summary>
    /// 添加或更新定向条件模板（返回新实例）
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <param name="criteria">定向条件</param>
    /// <returns>包含新条件的策略实例</returns>
    public TargetingPolicy WithCriteriaTemplate(string criteriaType, ITargetingCriteria criteria)
    {
        if (string.IsNullOrEmpty(criteriaType))
            throw new ArgumentException("条件类型不能为空", nameof(criteriaType));
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        var existingCriteria = CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType);
        var criteriaList = CriteriaTemplates.ToList();

        if (existingCriteria != null)
        {
            var index = criteriaList.IndexOf(existingCriteria);
            criteriaList[index] = criteria;
        }
        else
        {
            criteriaList.Add(criteria);
        }

        // 转换为字典以使用现有构造函数
        var criteriaDict = criteriaList.ToDictionary(c => c.CriteriaType, c => c);

        return new TargetingPolicy(
            Name, CreatedBy, Description, Version,
            criteriaDict, Category, IsPublic, _tags,
            Weight, IsEnabled, Status);
    }

    /// <summary>
    /// 移除定向条件模板（返回新实例）
    /// </summary>
    /// <param name="criteriaType">条件类型</param>
    /// <returns>移除指定条件后的策略实例</returns>
    public TargetingPolicy WithoutCriteriaTemplate(string criteriaType)
    {
        if (string.IsNullOrEmpty(criteriaType))
            return this;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        var criteriaToRemove = CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType);
        if (criteriaToRemove == null)
            return this;

        var criteriaList = CriteriaTemplates.Where(c => c.CriteriaType != criteriaType).ToList();
        var criteriaDict = criteriaList.ToDictionary(c => c.CriteriaType, c => c);

        return new TargetingPolicy(
            Name, CreatedBy, Description, Version,
            criteriaDict, Category, IsPublic, _tags,
            Weight, IsEnabled, Status);
    }

    /// <summary>
    /// 获取指定类型的定向条件模板
    /// </summary>
    /// <typeparam name="T">条件类型</typeparam>
    /// <param name="criteriaType">条件类型标识</param>
    /// <returns>定向条件模板，如果不存在则返回null</returns>
    public T? GetCriteriaTemplate<T>(string criteriaType) where T : class, ITargetingCriteria
    {
        if (string.IsNullOrEmpty(criteriaType))
            return null;

        return CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType) as T;
    }

    /// <summary>
    /// 检查是否包含指定类型的定向条件模板
    /// </summary>
    /// <param name="criteriaType">条件类型标识</param>
    /// <returns>是否包含</returns>
    public bool HasCriteriaTemplate(string criteriaType)
    {
        return !string.IsNullOrEmpty(criteriaType) && CriteriaTemplates.Any(c => c.CriteriaType == criteriaType);
    }

    /// <summary>
    /// 获取所有已启用的定向条件模板
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteriaTemplates()
    {
        return CriteriaTemplates.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// 获取所有定向条件类型
    /// </summary>
    public IReadOnlyCollection<string> GetCriteriaTypes()
    {
        return CriteriaTemplates.Select(c => c.CriteriaType).ToList().AsReadOnly();
    }

    /// <summary>
    /// 添加标签
    /// </summary>
    /// <param name="tag">标签</param>
    public void AddTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            _tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 移除标签
    /// </summary>
    /// <param name="tag">标签</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return false;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        var result = _tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase)) > 0;
        if (result)
        {
            UpdatedAt = DateTime.UtcNow;
        }
        return result;
    }

    /// <summary>
    /// 验证策略是否有效
    /// </summary>
    public ValidationResult Validate()
    {
        var validationResult = new ValidationResult(string.Empty);

        // 验证基本属性
        if (Id == Guid.Empty)
            validationResult.AddError("策略ID不能为空");

        if (string.IsNullOrEmpty(Name))
            validationResult.AddError("策略名称不能为空");

        if (string.IsNullOrEmpty(CreatedBy))
            validationResult.AddError("创建者不能为空");

        if (Weight <= 0)
            validationResult.AddError("权重必须大于0");

        // 如果启用，至少需要一个定向条件模板
        if (IsEnabled)
        {
            var enabledTemplates = GetEnabledCriteriaTemplates().ToList();
            if (!enabledTemplates.Any())
                validationResult.AddWarning("启用状态下建议至少包含一个定向条件模板");

            // 验证所有启用的条件模板都是有效的
            foreach (var template in enabledTemplates)
            {
                if (!template.IsValid())
                    validationResult.AddError($"定向条件模板 {template.CriteriaType} 无效");
            }
        }

        return validationResult;
    }

    /// <summary>
    /// 检查策略是否有效
    /// </summary>
    public bool IsValid()
    {
        return Validate().IsValid;
    }

    /// <summary>
    /// 发布策略
    /// </summary>
    public void Publish()
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能发布");

        var validationResult = Validate();
        if (!validationResult.IsValid)
            throw new InvalidOperationException($"策略验证失败：{string.Join(", ", validationResult.Errors)}");

        Status = PolicyStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 归档策略
    /// </summary>
    public void Archive()
    {
        Status = PolicyStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 克隆策略
    /// </summary>
    /// <param name="newName">新策略名称</param>
    /// <param name="createdBy">创建者</param>
    /// <returns>克隆的策略</returns>
    public TargetingPolicy Clone(string newName, string createdBy)
    {
        if (string.IsNullOrEmpty(newName))
            throw new ArgumentException("新策略名称不能为空", nameof(newName));
        if (string.IsNullOrEmpty(createdBy))
            throw new ArgumentException("创建者不能为空", nameof(createdBy));

        var newPolicyId = Guid.NewGuid().ToString();
        var clonedCriteriaTemplates = new Dictionary<string, ITargetingCriteria>();

        foreach (var template in CriteriaTemplates)
        {
            // 假设ITargetingCriteria实现了深拷贝方法
            clonedCriteriaTemplates[template.CriteriaType] = template; // 这里需要实现深拷贝
        }

        return new TargetingPolicy(
            name: newName,
            createdBy: createdBy,
            description: $"克隆自：{Name}",
            version: 1,
            criteriaTemplates: clonedCriteriaTemplates,
            category: Category,
            isPublic: false, // 克隆的策略默认不公开
            tags: _tags.ToList(),
            weight: Weight,
            isEnabled: IsEnabled,
            status: PolicyStatus.Draft);
    }

    /// <summary>
    /// 获取策略使用统计
    /// </summary>
    /// <returns>使用统计</returns>
    public PolicyUsageStats GetUsageStatistics()
    {
        // 这里应该通过仓储或服务获取实际使用统计
        // 为演示目的，返回模拟数据
        return PolicyUsageStats.Create(
            Id,
            0, // 需要从数据库查询
            0, // 需要从数据库查询
            null, // 需要从数据库查询
            0 // 需要从性能数据计算
        );
    }

    /// <summary>
    /// 获取策略配置摘要
    /// </summary>
    public string GetConfigurationSummary()
    {
        if (!IsEnabled)
            return $"TargetingPolicy[{Id}]: {Name} - Disabled (Unrestricted)";

        var enabledTemplates = GetEnabledCriteriaTemplates().ToList();
        if (!enabledTemplates.Any())
            return $"TargetingPolicy[{Id}]: {Name} - No criteria templates defined";

        var criteriaTypes = enabledTemplates.Select(c => c.CriteriaType);
        var tagInfo = _tags.Any() ? $", Tags: [{string.Join(", ", _tags)}]" : "";

        return $"TargetingPolicy[{Id}]: {Name} - {string.Join(", ", criteriaTypes)} (Weight: {Weight:F2}, Version: {Version}, Status: {Status}){tagInfo}";
    }

    /// <summary>
    /// 更新策略权重
    /// </summary>
    /// <param name="newWeight">新权重</param>
    public void UpdateWeight(decimal newWeight)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

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
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        IsEnabled = enabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新策略描述
    /// </summary>
    /// <param name="description">新描述</param>
    public void UpdateDescription(string? description)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新策略可见性
    /// </summary>
    /// <param name="isPublic">是否公开</param>
    public void UpdateVisibility(bool isPublic)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("已归档的策略不能修改");

        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Name;
        yield return Version;
        yield return Weight;
        yield return IsEnabled;
        yield return Status;
        yield return Category;
        yield return IsPublic;

        // 按条件类型排序，确保一致的比较结果
        foreach (var template in CriteriaTemplates.OrderBy(c => c.CriteriaType))
        {
            yield return template.CriteriaType;
            yield return template;
        }

        // 按标签排序，确保一致的比较结果
        foreach (var tag in _tags.OrderBy(t => t))
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

}













