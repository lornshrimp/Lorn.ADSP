using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户标签实体
/// [需要存储] - 数据库存储
/// 用于替代UserProfile中的Tags列表
/// </summary>
public class UserTag : EntityBase
{
    /// <summary>
    /// 用户画像ID - 外键
    /// </summary>
    public Guid UserProfileId { get; set; }

    /// <summary>
    /// 用户画像 - 导航属性
    /// </summary>
    public UserProfile UserProfile { get; set; } = null!;

    /// <summary>
    /// 标签名称
    /// </summary>
    public string TagName { get; set; } = string.Empty;

    /// <summary>
    /// 标签类型（如：Interest, Behavior, Demographics等）
    /// </summary>
    public string TagType { get; set; } = string.Empty;

    /// <summary>
    /// 标签来源（算法、人工标注等）
    /// </summary>
    public string TagSource { get; set; } = string.Empty;

    /// <summary>
    /// 标签置信度（0-1之间的值）
    /// </summary>
    public decimal Confidence { get; set; } = 1.0m;

    /// <summary>
    /// 标签权重
    /// </summary>
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// 标签分配时间
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 标签过期时间（可选）
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserTag() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserTag(Guid userProfileId, string tagName, string tagType)
    {
        UserProfileId = userProfileId;
        TagName = tagName;
        TagType = tagType;
    }
}
