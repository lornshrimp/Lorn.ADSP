using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户画像实体
/// [需要存储] - 数据库存储
/// 作为用户相关定向上下文的聚合根
/// </summary>
public class UserProfile : AggregateRoot
{
    /// <summary>
    /// 用户ID（外部系统用户标识）
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// 细分ID
    /// </summary>
    public string SegmentId { get; set; } = string.Empty;

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime { get; set; }

    /// <summary>
    /// 数据来源
    /// </summary>
    public string DataSource { get; set; } = string.Empty;

    /// <summary>
    /// 画像状态
    /// </summary>
    public ProfileStatus ProfileStatus { get; set; } = ProfileStatus.Active;

    /// <summary>
    /// 人口统计学信息集合 - 集合导航属性
    /// 替代原有的Demographics字典
    /// </summary>
    public List<UserDemographic> Demographics { get; set; } = new();

    /// <summary>
    /// 兴趣信息集合 - 集合导航属性
    /// 替代原有的Interests字典
    /// </summary>
    public List<UserInterest> Interests { get; set; } = new();

    /// <summary>
    /// 行为数据集合 - 集合导航属性
    /// 替代原有的Behaviors字典
    /// </summary>
    public List<UserBehavior> Behaviors { get; set; } = new();

    /// <summary>
    /// 标签集合 - 集合导航属性
    /// 替代原有的Tags列表
    /// </summary>
    public List<UserTag> Tags { get; set; } = new();

    /// <summary>
    /// 私有构造函数，用于EF Core
    /// </summary>
    private UserProfile() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserProfile(string userId, string segmentId = "")
    {
        UserId = userId;
        SegmentId = segmentId;
        LastActiveTime = DateTime.UtcNow;
        Status = UserStatus.Active;
    }

    /// <summary>
    /// 获取用户分群
    /// </summary>
    public List<string> GetUserSegments()
    {
        // 临时计算方法 - 运行时使用，不存储
        return Tags.Where(t => t.TagType == "Segment")
                  .Select(t => t.TagName)
                  .ToList();
    }

    /// <summary>
    /// 检查是否有指定标签
    /// </summary>
    public bool HasTag(string tag)
    {
        // 临时计算方法 - 运行时使用，不存储
        return Tags.Any(t => t.TagName == tag);
    }

    /// <summary>
    /// 获取兴趣分数
    /// </summary>
    public decimal GetInterestScore(string category)
    {
        // 临时计算方法 - 运行时使用，不存储
        var interest = Interests.FirstOrDefault(i => i.Category == category);
        return interest?.Score ?? 0m;
    }

    /// <summary>
    /// 获取行为模式
    /// </summary>
    public object GetBehaviorPattern(string pattern)
    {
        // 临时计算方法 - 运行时使用，不存储
        var behaviors = Behaviors.Where(b => b.BehaviorType == pattern).ToList();
        return new { Frequency = behaviors.Sum(b => b.Frequency), LatestOccurrence = behaviors.Max(b => b.Timestamp) };
    }

    /// <summary>
    /// 是否目标受众
    /// </summary>
    public bool IsTargetAudience(TargetingConfig config)
    {
        // 临时计算方法 - 运行时使用，不存储
        // 这里需要根据具体的定向配置来判断
        return true; // 简化实现
    }

    /// <summary>
    /// 更新用户画像
    /// </summary>
    public void UpdateProfile(List<UserDemographic> newDemographics)
    {
        foreach (var newDemo in newDemographics)
        {
            // 更新或添加人口统计学信息
            var existingDemo = Demographics.FirstOrDefault(d => d.PropertyName == newDemo.PropertyName);
            if (existingDemo != null)
            {
                existingDemo.PropertyValue = newDemo.PropertyValue;
                existingDemo.DataType = newDemo.DataType;
            }
            else
            {
                Demographics.Add(new UserDemographic(Id, newDemo.PropertyName, newDemo.PropertyValue, newDemo.DataType));
            }
        }

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加单个人口统计学信息
    /// </summary>
    public void AddDemographic(string propertyName, string propertyValue, string dataType = "String")
    {
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentException("属性名称不能为空", nameof(propertyName));

        var existingDemo = Demographics.FirstOrDefault(d => d.PropertyName == propertyName);
        if (existingDemo != null)
        {
            existingDemo.PropertyValue = propertyValue ?? string.Empty;
            existingDemo.DataType = dataType;
        }
        else
        {
            Demographics.Add(new UserDemographic(Id, propertyName, propertyValue ?? string.Empty, dataType));
        }

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }    /// <summary>
         /// 合并用户画像
         /// </summary>
    public void MergeProfile(UserProfile other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        // 合并人口统计学信息
        foreach (var otherDemo in other.Demographics)
        {
            var existing = Demographics.FirstOrDefault(d => d.PropertyName == otherDemo.PropertyName);
            if (existing == null)
            {
                Demographics.Add(new UserDemographic(Id, otherDemo.PropertyName, otherDemo.PropertyValue, otherDemo.DataType));
            }
        }

        // 合并兴趣信息
        foreach (var otherInterest in other.Interests)
        {
            var existing = Interests.FirstOrDefault(i => i.Category == otherInterest.Category);
            if (existing == null)
            {
                Interests.Add(new UserInterest(Id, otherInterest.Category, otherInterest.Score));
            }
            else
            {
                // 取平均值或最大值
                existing.Score = Math.Max(existing.Score, otherInterest.Score);
            }
        }

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }
}
