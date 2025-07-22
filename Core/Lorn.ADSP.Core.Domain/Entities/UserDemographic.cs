using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户人口统计学信息实体
/// [需要存储] - 数据库存储
/// 用于替代UserProfile中的Demographics字典
/// </summary>
public class UserDemographic : EntityBase
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
    /// 属性名称（如：Age, Gender, Income等）
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// 属性值
    /// </summary>
    public string PropertyValue { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型（String, Number, Boolean等）
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 置信度（0-1之间的值）
    /// </summary>
    public decimal Confidence { get; set; } = 1.0m;

    /// <summary>
    /// 数据来源
    /// </summary>
    public string DataSource { get; set; } = string.Empty;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserDemographic() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserDemographic(Guid userProfileId, string propertyName, string propertyValue, string dataType)
    {
        UserProfileId = userProfileId;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
        DataType = dataType;
    }
}
