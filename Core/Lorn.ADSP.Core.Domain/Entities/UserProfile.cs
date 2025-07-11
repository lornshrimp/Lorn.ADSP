using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户画像实体
/// 作为用户相关定向上下文的聚合根，实现ITargetingContext接口
/// 通过定向上下文字典管理各种用户定向信息，而不是硬编码引用
/// </summary>
public class UserProfile : AggregateRoot, ITargetingContext
{
    private readonly Dictionary<string, ITargetingContext> _targetingContexts;

    /// <summary>
    /// 用户ID（外部系统用户标识）
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// 画像数据质量评分
    /// </summary>
    public ProfileQualityScore QualityScore { get; private set; }

    /// <summary>
    /// 自定义属性
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomAttributes { get; private set; } = new Dictionary<string, object>();

    #region ITargetingContext Implementation

    /// <summary>
    /// 定向上下文类型标识
    /// </summary>
    public string ContextType => "UserProfile";

    /// <summary>
    /// 定向上下文属性集合
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties => GetProfileProperties();

    /// <summary>
    /// 上下文的创建时间戳
    /// </summary>
    public DateTime Timestamp => CreateTime;

    /// <summary>
    /// 上下文的唯一标识
    /// </summary>
    public string ContextId => $"UserProfile_{UserId}";

    /// <summary>
    /// 上下文数据来源
    /// </summary>
    public string DataSource => "UserProfileAggregate";

    #endregion

    /// <summary>
    /// 定向上下文集合（只读）
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingContext> TargetingContexts => _targetingContexts.AsReadOnly();

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserProfile()
    {
        UserId = string.Empty;
        Status = UserStatus.Active;
        LastActiveTime = DateTime.UtcNow;
        QualityScore = ProfileQualityScore.CreateDefault();
        _targetingContexts = new Dictionary<string, ITargetingContext>();
    }





    /// <summary>
    /// 设置定向上下文
    /// </summary>
    public void SetTargetingContext(ITargetingContext targetingContext)
    {
        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));

        _targetingContexts[targetingContext.ContextType] = targetingContext;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, targetingContext.ContextType));
    }

    /// <summary>
    /// 获取指定类型的定向上下文
    /// </summary>
    public T? GetTargetingContext<T>() where T : class, ITargetingContext
    {
        var contextType = typeof(T).Name;
        // 支持简化的类型名查找
        if (contextType.EndsWith("Info") || contextType.EndsWith("Context"))
        {
            var simpleName = contextType.Replace("Info", "").Replace("Context", "");
            if (_targetingContexts.TryGetValue(simpleName, out var context))
                return context as T;
        }

        return _targetingContexts.Values.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// 获取指定类型名的定向上下文
    /// </summary>
    public ITargetingContext? GetTargetingContext(string contextType)
    {
        return _targetingContexts.TryGetValue(contextType, out var context) ? context : null;
    }

    /// <summary>
    /// 移除指定类型的定向上下文
    /// </summary>
    public bool RemoveTargetingContext(string contextType)
    {
        if (_targetingContexts.Remove(contextType))
        {
            UpdateLastModifiedTime();
            RecalculateQualityScore();
            AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, $"Removed_{contextType}"));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检查是否包含指定类型的定向上下文
    /// </summary>
    public bool HasTargetingContext(string contextType)
    {
        return _targetingContexts.ContainsKey(contextType);
    }



    /// <summary>
    /// 激活用户
    /// </summary>
    public void Activate()
    {
        if (Status != UserStatus.Active)
        {
            Status = UserStatus.Active;
            UpdateLastModifiedTime();
            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 停用用户
    /// </summary>
    public void Deactivate()
    {
        if (Status == UserStatus.Active)
        {
            Status = UserStatus.Inactive;
            UpdateLastModifiedTime();
            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 暂停用户
    /// </summary>
    public void Suspend()
    {
        if (Status != UserStatus.Suspended)
        {
            Status = UserStatus.Suspended;
            UpdateLastModifiedTime();
            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 软删除用户
    /// </summary>
    public void SoftDelete()
    {
        Status = UserStatus.Deleted;
        Delete(); // 调用基类的软删除方法
        AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
    }

    /// <summary>
    /// 设置自定义属性
    /// </summary>
    public void SetCustomAttribute(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("自定义属性键不能为空", nameof(key));

        var attributes = CustomAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        attributes[key] = value;
        CustomAttributes = attributes;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// 获取自定义属性
    /// </summary>
    public T? GetCustomAttribute<T>(string key)
    {
        if (CustomAttributes.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    #region ITargetingContext Methods

    /// <summary>
    /// 获取指定类型的属性值
    /// </summary>
    public T? GetProperty<T>(string propertyKey)
    {
        if (string.IsNullOrEmpty(propertyKey))
            return default;

        // 首先从自定义属性中查找
        if (CustomAttributes.TryGetValue(propertyKey, out var customValue) && customValue is T typedCustomValue)
            return typedCustomValue;

        // 然后从各个定向上下文中查找
        foreach (var context in _targetingContexts.Values)
        {
            var value = context.GetProperty<T>(propertyKey);
            if (value != null)
                return value;
        }

        return default;
    }

    /// <summary>
    /// 获取指定类型的属性值，如果不存在则返回默认值
    /// </summary>
    public T GetProperty<T>(string propertyKey, T defaultValue)
    {
        var result = GetProperty<T>(propertyKey);
        return result != null ? result : defaultValue;
    }

    /// <summary>
    /// 获取属性值的字符串表示
    /// </summary>
    public string GetPropertyAsString(string propertyKey)
    {
        var value = GetProperty<object>(propertyKey);
        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// 检查是否包含指定属性
    /// </summary>
    public bool HasProperty(string propertyKey)
    {
        if (CustomAttributes.ContainsKey(propertyKey))
            return true;

        return _targetingContexts.Values.Any(context => context.HasProperty(propertyKey));
    }

    /// <summary>
    /// 获取所有属性键
    /// </summary>
    public IReadOnlyCollection<string> GetPropertyKeys()
    {
        var keys = new HashSet<string>(CustomAttributes.Keys);

        foreach (var context in _targetingContexts.Values)
        {
            foreach (var key in context.GetPropertyKeys())
            {
                keys.Add(key);
            }
        }

        return keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(UserId))
            return false;

        if (Timestamp == default)
            return false;

        // 验证所有定向上下文的有效性
        return _targetingContexts.Values.All(context => context.IsValid());
    }

    /// <summary>
    /// 检查上下文是否已过期
    /// </summary>
    public bool IsExpired(TimeSpan maxAge)
    {
        return DateTime.UtcNow - Timestamp > maxAge;
    }

    /// <summary>
    /// 获取上下文的元数据信息
    /// </summary>
    public IReadOnlyDictionary<string, object> GetMetadata()
    {
        return new Dictionary<string, object>
        {
            ["ContextType"] = ContextType,
            ["ContextId"] = ContextId,
            ["DataSource"] = DataSource,
            ["Timestamp"] = Timestamp,
            ["UserId"] = UserId,
            ["Status"] = Status.ToString(),
            ["LastActiveTime"] = LastActiveTime,
            ["TargetingContextCount"] = _targetingContexts.Count,
            ["TargetingContextTypes"] = string.Join(",", _targetingContexts.Keys),
            ["QualityScore"] = QualityScore.OverallScore,
            ["Age"] = DateTime.UtcNow - Timestamp
        }.AsReadOnly();
    }

    /// <summary>
    /// 获取上下文的调试信息
    /// </summary>
    public string GetDebugInfo()
    {
        var contextTypes = string.Join(", ", _targetingContexts.Keys);
        return $"UserProfile[{UserId}] Status:{Status} Contexts:[{contextTypes}] " +
               $"Quality:{QualityScore.OverallScore} LastActive:{LastActiveTime:yyyy-MM-dd HH:mm:ss} " +
               $"Age:{DateTime.UtcNow - Timestamp:hh\\:mm\\:ss}";
    }

    /// <summary>
    /// 创建上下文的轻量级副本
    /// </summary>
    public ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys)
    {
        // 创建简化的用户画像副本，只包含指定的属性
        var filteredProperties = new Dictionary<string, object>();

        foreach (var key in includeKeys)
        {
            var value = GetProperty<object>(key);
            if (value != null)
            {
                filteredProperties[key] = value;
            }
        }

        return new TargetingContextBase(
            ContextType + "_Lightweight",
            filteredProperties,
            DataSource,
            ContextId + "_Copy");
    }

    /// <summary>
    /// 合并另一个上下文的属性
    /// </summary>
    public ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        // 这里返回一个合并后的新上下文，而不是修改当前实体
        var mergedProperties = new Dictionary<string, object>();

        // 添加当前上下文的属性
        foreach (var property in Properties)
        {
            mergedProperties[property.Key] = property.Value;
        }

        // 添加或覆盖其他上下文的属性
        foreach (var property in other.Properties)
        {
            if (overwriteExisting || !mergedProperties.ContainsKey(property.Key))
            {
                mergedProperties[property.Key] = property.Value;
            }
        }

        return new TargetingContextBase(
            $"{ContextType}_Merged_{other.ContextType}",
            mergedProperties,
            $"{DataSource},{other.DataSource}",
            $"{ContextId}_Merged_{other.ContextId}");
    }

    #endregion

    #region Business Logic Helpers

    /// <summary>
    /// 是否活跃用户
    /// </summary>
    public bool IsActive => Status == UserStatus.Active && !IsDeleted;



    /// <summary>
    /// 是否完整画像
    /// </summary>
    public bool IsCompleteProfile => QualityScore.CompletenessScore >= 80;





    #endregion

    #region Private Methods

    /// <summary>
    /// 获取画像属性字典
    /// </summary>
    private IReadOnlyDictionary<string, object> GetProfileProperties()
    {
        var properties = new Dictionary<string, object>
        {
            ["UserId"] = UserId,
            ["Status"] = Status.ToString(),
            ["LastActiveTime"] = LastActiveTime,
            ["QualityScore"] = QualityScore.OverallScore
        };

        // 添加自定义属性
        foreach (var attr in CustomAttributes)
        {
            properties[attr.Key] = attr.Value;
        }

        // 添加各个定向上下文的属性
        foreach (var context in _targetingContexts.Values)
        {
            foreach (var prop in context.Properties)
            {
                properties[$"{context.ContextType}_{prop.Key}"] = prop.Value;
            }
        }

        return properties.AsReadOnly();
    }

    /// <summary>
    /// 重新计算质量评分
    /// </summary>
    private void RecalculateQualityScore()
    {
        QualityScore = CalculateQualityScore();
        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "QualityScore", QualityScore.OverallScore));
    }

    /// <summary>
    /// 计算质量评分
    /// </summary>
    private ProfileQualityScore CalculateQualityScore()
    {
        var completenessScore = CalculateCompletenessScore();
        var accuracyScore = CalculateAccuracyScore();
        var freshnessScore = CalculateFreshnessScore();

        return new ProfileQualityScore(completenessScore, accuracyScore, freshnessScore);
    }

    /// <summary>
    /// 计算完整性评分
    /// </summary>
    private int CalculateCompletenessScore()
    {
        var totalContextTypes = 6; // 预期的定向上下文类型数量
        var filledContexts = _targetingContexts.Count;

        // 基于定向上下文数量和质量计算完整性
        var contextScore = (decimal)filledContexts / totalContextTypes * 100;

        // 基于各个上下文的完整性进一步调整
        var contextQualitySum = 0m;
        var validContexts = 0;

        foreach (var context in _targetingContexts.Values)
        {
            if (context.Properties.Count > 0)
            {
                contextQualitySum += context.Properties.Count;
                validContexts++;
            }
        }

        var qualityBonus = validContexts > 0 ? (contextQualitySum / validContexts) * 2 : 0;

        return (int)Math.Min(100, contextScore + qualityBonus);
    }

    /// <summary>
    /// 计算准确性评分
    /// </summary>
    private int CalculateAccuracyScore()
    {
        var score = 100;

        // 验证各个定向上下文的有效性
        foreach (var context in _targetingContexts.Values)
        {
            if (!context.IsValid())
                score -= 10;
        }

        return Math.Max(0, score);
    }

    /// <summary>
    /// 计算时效性评分
    /// </summary>
    private int CalculateFreshnessScore()
    {
        var now = DateTime.UtcNow;
        var daysSinceUpdate = (now - LastModifiedTime).TotalDays;

        return daysSinceUpdate switch
        {
            <= 1 => 100,
            <= 7 => 90,
            <= 30 => 80,
            <= 90 => 70,
            <= 180 => 60,
            <= 365 => 50,
            _ => 30
        };
    }

    /// <summary>
    /// 验证用户ID
    /// </summary>
    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("用户ID不能为空", nameof(userId));

        if (userId.Length > 255)
            throw new ArgumentException("用户ID长度不能超过255个字符", nameof(userId));
    }

    #endregion
}