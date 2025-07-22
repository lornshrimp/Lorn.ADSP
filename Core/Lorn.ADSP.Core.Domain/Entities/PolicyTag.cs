using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 策略标签实体
/// [需要存储] - 数据库存储
/// 用于TargetingPolicy中的标签管理
/// </summary>
public class PolicyTag : EntityBase
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
    /// 标签名称
    /// </summary>
    public string TagName { get; set; } = string.Empty;

    /// <summary>
    /// 标签类型
    /// </summary>
    public string TagType { get; set; } = string.Empty;

    /// <summary>
    /// 标签值
    /// </summary>
    public string TagValue { get; set; } = string.Empty;

    /// <summary>
    /// 标签描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private PolicyTag() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PolicyTag(Guid targetingPolicyId, string tagName, string tagType)
    {
        TargetingPolicyId = targetingPolicyId;
        TagName = tagName;
        TagType = tagType;
    }
}
