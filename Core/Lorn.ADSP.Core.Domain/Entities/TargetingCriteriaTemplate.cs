using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 定向条件模板实体
/// [需要存储] - 数据库存储
/// 用于TargetingPolicy中的条件模板管理
/// </summary>
public class TargetingCriteriaTemplate : EntityBase
{
    /// <summary>
    /// 定向策略ID - 外键
    /// </summary>
    public Guid TargetingPolicyId { get; set; }

    /// <summary>
    /// 定向策略 - 导航属性
    /// </summary>
    public TargetingPolicy TargetingPolicy { get; set; } = null!;

    /// <summary>
    /// 模板名称
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 条件类型
    /// </summary>
    public string CriteriaType { get; set; } = string.Empty;

    /// <summary>
    /// 模板配置（JSON格式）
    /// </summary>
    public string TemplateConfig { get; set; } = string.Empty;

    /// <summary>
    /// 默认权重
    /// </summary>
    public decimal DefaultWeight { get; set; } = 1.0m;

    /// <summary>
    /// 是否为必需条件
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// 是否默认启用
    /// </summary>
    public bool IsEnabledByDefault { get; set; } = true;

    /// <summary>
    /// 模板描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private TargetingCriteriaTemplate() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public TargetingCriteriaTemplate(Guid targetingPolicyId, string templateName, string criteriaType)
    {
        TargetingPolicyId = targetingPolicyId;
        TemplateName = templateName;
        CriteriaType = criteriaType;
    }
}
