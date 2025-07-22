using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户兴趣实体
/// [需要存储] - 数据库存储
/// 用于替代UserProfile中的Interests字典
/// </summary>
public class UserInterest : EntityBase
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
    /// 兴趣类别（如：Sports, Technology, Fashion等）
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 兴趣分数（0-1之间的值）
    /// </summary>
    public decimal Score { get; set; } = 0.0m;

    /// <summary>
    /// 兴趣权重
    /// </summary>
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// 数据来源
    /// </summary>
    public string DataSource { get; set; } = string.Empty;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserInterest() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserInterest(Guid userProfileId, string category, decimal score)
    {
        UserProfileId = userProfileId;
        Category = category;
        Score = score;
    }
}
