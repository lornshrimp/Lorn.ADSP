using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户行为实体
/// [需要存储] - 数据库存储
/// 用于替代UserProfile中的Behaviors字典
/// </summary>
public class UserBehavior : EntityBase
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
    /// 行为类型（如：Click, View, Purchase等）
    /// </summary>
    public string BehaviorType { get; set; } = string.Empty;

    /// <summary>
    /// 行为值或描述
    /// </summary>
    public string BehaviorValue { get; set; } = string.Empty;

    /// <summary>
    /// 行为发生时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 行为频次
    /// </summary>
    public int Frequency { get; set; } = 1;

    /// <summary>
    /// 行为权重
    /// </summary>
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// 行为上下文信息（JSON格式）
    /// </summary>
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// 数据来源
    /// </summary>
    public string DataSource { get; set; } = string.Empty;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserBehavior() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserBehavior(Guid userProfileId, string behaviorType, string behaviorValue)
    {
        UserProfileId = userProfileId;
        BehaviorType = behaviorType;
        BehaviorValue = behaviorValue;
    }
}
