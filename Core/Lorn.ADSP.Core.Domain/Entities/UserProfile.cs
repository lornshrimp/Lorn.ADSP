using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户画像实体
/// [需要存储] - 数据库存储
/// 作为用户相关定向上下文的聚合根，实现ITargetingContext接口
/// </summary>
public class UserProfile : AggregateRoot, ITargetingContext
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
    public Guid SegmentId { get; set; } = Guid.Empty;

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
    /// 上下文属性集合 - 集合导航属性
    /// 替代原有的Demographics、Interests、Behaviors、Tags等多个集合
    /// 统一使用ContextProperty存储所有类型的用户属性信息
    /// </summary>
    public List<ContextProperty> Properties { get; set; } = new();

    /// <summary>
    /// 嵌套的定向上下文集合 - 集合导航属性
    /// 用于存储复杂的定向上下文信息，如地理位置、设备信息等
    /// </summary>
    public List<ITargetingContext> NestedContexts { get; set; } = new();

    #region ITargetingContext 接口实现

    /// <summary>
    /// 上下文名称
    /// </summary>
    public string ContextName => $"UserProfile-{UserId}";

    /// <summary>
    /// 上下文类型标识
    /// </summary>
    public string ContextType => "UserProfile";

    /// <summary>
    /// 上下文属性集合（只读访问）
    /// </summary>
    IReadOnlyList<ContextProperty> ITargetingContext.Properties => Properties.AsReadOnly();

    /// <summary>
    /// 上下文创建时间戳
    /// </summary>
    public DateTime Timestamp => CreatedAt;

    /// <summary>
    /// 上下文的唯一标识
    /// </summary>
    public Guid ContextId => Id;

    #endregion

    #region ITargetingContext 接口方法实现

    /// <summary>
    /// 获取指定键的属性实体
    /// </summary>
    public ContextProperty? GetProperty(string propertyKey)
    {
        return Properties.FirstOrDefault(p => p.PropertyKey == propertyKey);
    }

    /// <summary>
    /// 获取指定类型的属性值
    /// </summary>
    public T? GetPropertyValue<T>(string propertyKey)
    {
        var property = GetProperty(propertyKey);
        if (property == null) return default;

        try
        {
            if (typeof(T) == typeof(string))
                return (T)(object)property.PropertyValue;

            return System.Text.Json.JsonSerializer.Deserialize<T>(property.PropertyValue);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 获取指定类型的属性值，如果不存在则返回默认值
    /// </summary>
    public T GetPropertyValue<T>(string propertyKey, T defaultValue)
    {
        var value = GetPropertyValue<T>(propertyKey);
        return value ?? defaultValue;
    }

    /// <summary>
    /// 获取属性值的字符串表示
    /// </summary>
    public string GetPropertyAsString(string propertyKey)
    {
        var property = GetProperty(propertyKey);
        return property?.PropertyValue ?? string.Empty;
    }

    /// <summary>
    /// 检查是否包含指定的属性
    /// </summary>
    public bool HasProperty(string propertyKey)
    {
        return Properties.Any(p => p.PropertyKey == propertyKey);
    }

    /// <summary>
    /// 获取所有属性键
    /// </summary>
    public IReadOnlyCollection<string> GetPropertyKeys()
    {
        return Properties.Select(p => p.PropertyKey).ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取指定分类的属性
    /// </summary>
    public IReadOnlyList<ContextProperty> GetPropertiesByCategory(string category)
    {
        return Properties.Where(p => p.Category == category).ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取未过期的属性
    /// </summary>
    public IReadOnlyList<ContextProperty> GetActiveProperties()
    {
        var now = DateTime.UtcNow;
        return Properties.Where(p => !p.ExpiresAt.HasValue || p.ExpiresAt.Value > now).ToList().AsReadOnly();
    }

    /// <summary>
    /// 检查上下文数据是否有效
    /// </summary>
    public bool IsValid()
    {
        return Status == UserStatus.Active && ProfileStatus == ProfileStatus.Active;
    }

    /// <summary>
    /// 检查上下文数据是否已过期
    /// </summary>
    public bool IsExpired(TimeSpan maxAge)
    {
        return DateTime.UtcNow - LastActiveTime > maxAge;
    }

    /// <summary>
    /// 获取上下文的元数据信息
    /// </summary>
    public IReadOnlyList<ContextProperty> GetMetadata()
    {
        var metadata = new List<ContextProperty>
        {
            new ContextProperty("UserId", UserId, "String", "Metadata", false, 1.0m, null, DataSource),
            new ContextProperty("SegmentId", SegmentId.ToString(), "String", "Metadata", false, 1.0m, null, DataSource),
            new ContextProperty("LastActiveTime", LastActiveTime.ToString("O"), "DateTime", "Metadata", false, 1.0m, null, DataSource),
            new ContextProperty("Status", Status.ToString(), "Enum", "Metadata", false, 1.0m, null, DataSource),
            new ContextProperty("ProfileStatus", ProfileStatus.ToString(), "Enum", "Metadata", false, 1.0m, null, DataSource)
        };
        return metadata.AsReadOnly();
    }

    /// <summary>
    /// 获取上下文的调试信息
    /// </summary>
    public string GetDebugInfo()
    {
        return $"UserProfile: Id={Id}, UserId={UserId}, Properties={Properties.Count}, Status={Status}, LastActive={LastActiveTime:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// 创建上下文的轻量级副本
    /// </summary>
    public ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys)
    {
        var keySet = includeKeys.ToHashSet();
        var filteredProperties = Properties.Where(p => keySet.Contains(p.PropertyKey)).ToList();

        var copy = new UserProfile(UserId, SegmentId)
        {
            Properties = filteredProperties,
            DataSource = DataSource,
            Status = Status,
            ProfileStatus = ProfileStatus,
            LastActiveTime = LastActiveTime
        };

        return copy;
    }

    /// <summary>
    /// 创建上下文的分类副本
    /// </summary>
    public ITargetingContext CreateCategorizedCopy(IEnumerable<string> categories)
    {
        var categorySet = categories.ToHashSet();
        var filteredProperties = Properties.Where(p => categorySet.Contains(p.Category)).ToList();

        var copy = new UserProfile(UserId, SegmentId)
        {
            Properties = filteredProperties,
            DataSource = DataSource,
            Status = Status,
            ProfileStatus = ProfileStatus,
            LastActiveTime = LastActiveTime
        };

        return copy;
    }

    /// <summary>
    /// 合并另一个上下文的属性
    /// </summary>
    public ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false)
    {
        var mergedProperties = new List<ContextProperty>(Properties);

        foreach (var otherProperty in other.Properties)
        {
            var existing = mergedProperties.FirstOrDefault(p => p.PropertyKey == otherProperty.PropertyKey);
            if (existing == null)
            {
                mergedProperties.Add(otherProperty);
            }
            else if (overwriteExisting)
            {
                mergedProperties.Remove(existing);
                mergedProperties.Add(otherProperty);
            }
        }

        var merged = new UserProfile(UserId, SegmentId)
        {
            Properties = mergedProperties,
            DataSource = DataSource,
            Status = Status,
            ProfileStatus = ProfileStatus,
            LastActiveTime = DateTime.UtcNow
        };

        return merged;
    }

    #endregion

    /// <summary>
    /// 私有构造函数，用于EF Core
    /// </summary>
    private UserProfile() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserProfile(string userId, Guid segmentId = default)
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
        return Properties.Where(p => p.Category == "Segment")
                        .Select(p => p.PropertyValue)
                        .ToList();
    }

    /// <summary>
    /// 检查是否有指定标签
    /// </summary>
    public bool HasTag(string tag)
    {
        // 临时计算方法 - 运行时使用，不存储
        return Properties.Any(p => p.Category == "Tag" && p.PropertyValue == tag);
    }

    /// <summary>
    /// 获取兴趣分数
    /// </summary>
    public decimal GetInterestScore(string category)
    {
        // 临时计算方法 - 运行时使用，不存储
        var interest = Properties.FirstOrDefault(p => p.Category == "Interest" && p.PropertyKey == category);
        return interest != null && decimal.TryParse(interest.PropertyValue, out var score) ? score : 0m;
    }

    /// <summary>
    /// 获取行为模式
    /// </summary>
    public object GetBehaviorPattern(string pattern)
    {
        // 临时计算方法 - 运行时使用，不存储
        var behaviors = Properties.Where(p => p.Category == "Behavior" && p.PropertyKey.StartsWith(pattern)).ToList();
        var frequency = behaviors.Sum(b => decimal.TryParse(b.PropertyValue, out var freq) ? freq : 0);
        var latestOccurrence = behaviors.Select(b => DateTime.TryParse(b.PropertyValue, out var dt) ? dt : DateTime.MinValue).Max();
        return new { Frequency = frequency, LatestOccurrence = latestOccurrence };
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
    public void UpdateProfile(List<ContextProperty> newProperties)
    {
        foreach (var newProperty in newProperties)
        {
            // 更新或添加属性信息
            var existingProperty = Properties.FirstOrDefault(p => p.PropertyKey == newProperty.PropertyKey && p.Category == newProperty.Category);
            if (existingProperty != null)
            {
                // 移除旧属性，添加新属性（因为ContextProperty是值对象）
                Properties.Remove(existingProperty);
            }
            Properties.Add(newProperty);
        }

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加单个属性信息
    /// </summary>
    public void AddProperty(string propertyKey, string propertyValue, string category = "Demographics", string dataType = "String", decimal weight = 1.0m)
    {
        if (string.IsNullOrEmpty(propertyKey))
            throw new ArgumentException("属性键不能为空", nameof(propertyKey));

        var existingProperty = Properties.FirstOrDefault(p => p.PropertyKey == propertyKey && p.Category == category);
        if (existingProperty != null)
        {
            Properties.Remove(existingProperty);
        }

        var newProperty = new ContextProperty(propertyKey, propertyValue ?? string.Empty, dataType, category, false, weight, null, DataSource);
        Properties.Add(newProperty);

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }    /// <summary>
         /// 合并用户画像
         /// </summary>
    public void MergeProfile(UserProfile other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        // 合并属性信息
        foreach (var otherProperty in other.Properties)
        {
            var existing = Properties.FirstOrDefault(p => p.PropertyKey == otherProperty.PropertyKey && p.Category == otherProperty.Category);
            if (existing == null)
            {
                Properties.Add(new ContextProperty(
                    otherProperty.PropertyKey,
                    otherProperty.PropertyValue,
                    otherProperty.DataType,
                    otherProperty.Category,
                    otherProperty.IsSensitive,
                    otherProperty.Weight,
                    otherProperty.ExpiresAt,
                    otherProperty.DataSource));
            }
            else
            {
                // 对于兴趣分数等数值类型，取最大值
                if (otherProperty.Category == "Interest" && decimal.TryParse(existing.PropertyValue, out var existingScore) && decimal.TryParse(otherProperty.PropertyValue, out var otherScore))
                {
                    if (otherScore > existingScore)
                    {
                        Properties.Remove(existing);
                        Properties.Add(otherProperty);
                    }
                }
            }
        }

        // 合并嵌套上下文
        foreach (var otherContext in other.NestedContexts)
        {
            var existing = NestedContexts.FirstOrDefault(c => c.ContextType == otherContext.ContextType && c.ContextName == otherContext.ContextName);
            if (existing == null)
            {
                NestedContexts.Add(otherContext);
            }
        }

        LastActiveTime = DateTime.UtcNow;
        UpdateLastModifiedTime();
    }

    #region 便利方法 - 为了方便使用和迁移

    /// <summary>
    /// 添加人口统计学信息
    /// </summary>
    public void AddDemographic(string propertyName, string propertyValue, string dataType = "String", decimal weight = 1.0m)
    {
        AddProperty(propertyName, propertyValue, "Demographics", dataType, weight);
    }

    /// <summary>
    /// 添加兴趣信息
    /// </summary>
    public void AddInterest(string category, decimal score, decimal weight = 1.0m)
    {
        AddProperty(category, score.ToString(), "Interest", "Decimal", weight);
    }

    /// <summary>
    /// 添加行为信息
    /// </summary>
    public void AddBehavior(string behaviorType, object behaviorData, decimal weight = 1.0m)
    {
        var serializedData = System.Text.Json.JsonSerializer.Serialize(behaviorData);
        AddProperty(behaviorType, serializedData, "Behavior", "Json", weight);
    }

    /// <summary>
    /// 添加标签
    /// </summary>
    public void AddTag(string tagName, string tagType = "General", decimal weight = 1.0m)
    {
        AddProperty(tagName, tagType, "Tag", "String", weight);
    }

    /// <summary>
    /// 获取所有人口统计学信息
    /// </summary>
    public IReadOnlyList<ContextProperty> GetDemographics()
    {
        return GetPropertiesByCategory("Demographics");
    }

    /// <summary>
    /// 获取所有兴趣信息
    /// </summary>
    public IReadOnlyList<ContextProperty> GetInterests()
    {
        return GetPropertiesByCategory("Interest");
    }

    /// <summary>
    /// 获取所有行为信息
    /// </summary>
    public IReadOnlyList<ContextProperty> GetBehaviors()
    {
        return GetPropertiesByCategory("Behavior");
    }

    /// <summary>
    /// 获取所有标签信息
    /// </summary>
    public IReadOnlyList<ContextProperty> GetTags()
    {
        return GetPropertiesByCategory("Tag");
    }

    /// <summary>
    /// 添加嵌套的定向上下文
    /// </summary>
    public void AddNestedContext(ITargetingContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        // 避免重复添加相同的上下文
        var existing = NestedContexts.FirstOrDefault(c => c.ContextType == context.ContextType && c.ContextId == context.ContextId);
        if (existing == null)
        {
            NestedContexts.Add(context);
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 移除嵌套的定向上下文
    /// </summary>
    public void RemoveNestedContext(string contextType, Guid contextId)
    {
        var context = NestedContexts.FirstOrDefault(c => c.ContextType == contextType && c.ContextId == contextId);
        if (context != null)
        {
            NestedContexts.Remove(context);
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 获取指定类型的嵌套上下文
    /// </summary>
    public IReadOnlyList<ITargetingContext> GetNestedContextsByType(string contextType)
    {
        return NestedContexts.Where(c => c.ContextType == contextType).ToList().AsReadOnly();
    }

    #endregion
}
