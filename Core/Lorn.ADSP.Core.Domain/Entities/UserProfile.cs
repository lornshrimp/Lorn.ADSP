using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// �û�����ʵ��
/// ��Ϊ�û���ض��������ĵľۺϸ���ʵ��ITargetingContext�ӿ�
/// ͨ�������������ֵ��������û�������Ϣ��������Ӳ��������
/// </summary>
public class UserProfile : AggregateRoot, ITargetingContext
{
    private readonly Dictionary<string, ITargetingContext> _targetingContexts;

    /// <summary>
    /// �û�ID���ⲿϵͳ�û���ʶ��
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// �û�״̬
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// ����Ծʱ��
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// ����������������
    /// </summary>
    public ProfileQualityScore QualityScore { get; private set; }

    /// <summary>
    /// �Զ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomAttributes { get; private set; } = new Dictionary<string, object>();

    #region ITargetingContext Implementation

    /// <summary>
    /// �������������ͱ�ʶ
    /// </summary>
    public string ContextType => "UserProfile";

    /// <summary>
    /// �������������Լ���
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties => GetProfileProperties();

    /// <summary>
    /// �����ĵĴ���ʱ���
    /// </summary>
    public DateTime Timestamp => CreateTime;

    /// <summary>
    /// �����ĵ�Ψһ��ʶ
    /// </summary>
    public string ContextId => $"UserProfile_{UserId}";

    /// <summary>
    /// ������������Դ
    /// </summary>
    public string DataSource => "UserProfileAggregate";

    #endregion

    /// <summary>
    /// ���������ļ��ϣ�ֻ����
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingContext> TargetingContexts => _targetingContexts.AsReadOnly();

    /// <summary>
    /// ˽�й��캯��������EF Core��
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
    /// ���ö���������
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
    /// ��ȡָ�����͵Ķ���������
    /// </summary>
    public T? GetTargetingContext<T>() where T : class, ITargetingContext
    {
        var contextType = typeof(T).Name;
        // ֧�ּ򻯵�����������
        if (contextType.EndsWith("Info") || contextType.EndsWith("Context"))
        {
            var simpleName = contextType.Replace("Info", "").Replace("Context", "");
            if (_targetingContexts.TryGetValue(simpleName, out var context))
                return context as T;
        }

        return _targetingContexts.Values.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// ��ȡָ���������Ķ���������
    /// </summary>
    public ITargetingContext? GetTargetingContext(string contextType)
    {
        return _targetingContexts.TryGetValue(contextType, out var context) ? context : null;
    }

    /// <summary>
    /// �Ƴ�ָ�����͵Ķ���������
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
    /// ����Ƿ����ָ�����͵Ķ���������
    /// </summary>
    public bool HasTargetingContext(string contextType)
    {
        return _targetingContexts.ContainsKey(contextType);
    }



    /// <summary>
    /// �����û�
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
    /// ͣ���û�
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
    /// ��ͣ�û�
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
    /// ��ɾ���û�
    /// </summary>
    public void SoftDelete()
    {
        Status = UserStatus.Deleted;
        Delete(); // ���û������ɾ������
        AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
    }

    /// <summary>
    /// �����Զ�������
    /// </summary>
    public void SetCustomAttribute(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("�Զ������Լ�����Ϊ��", nameof(key));

        var attributes = CustomAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        attributes[key] = value;
        CustomAttributes = attributes;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// ��ȡ�Զ�������
    /// </summary>
    public T? GetCustomAttribute<T>(string key)
    {
        if (CustomAttributes.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    #region ITargetingContext Methods

    /// <summary>
    /// ��ȡָ�����͵�����ֵ
    /// </summary>
    public T? GetProperty<T>(string propertyKey)
    {
        if (string.IsNullOrEmpty(propertyKey))
            return default;

        // ���ȴ��Զ��������в���
        if (CustomAttributes.TryGetValue(propertyKey, out var customValue) && customValue is T typedCustomValue)
            return typedCustomValue;

        // Ȼ��Ӹ��������������в���
        foreach (var context in _targetingContexts.Values)
        {
            var value = context.GetProperty<T>(propertyKey);
            if (value != null)
                return value;
        }

        return default;
    }

    /// <summary>
    /// ��ȡָ�����͵�����ֵ������������򷵻�Ĭ��ֵ
    /// </summary>
    public T GetProperty<T>(string propertyKey, T defaultValue)
    {
        var result = GetProperty<T>(propertyKey);
        return result != null ? result : defaultValue;
    }

    /// <summary>
    /// ��ȡ����ֵ���ַ�����ʾ
    /// </summary>
    public string GetPropertyAsString(string propertyKey)
    {
        var value = GetProperty<object>(propertyKey);
        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// ����Ƿ����ָ������
    /// </summary>
    public bool HasProperty(string propertyKey)
    {
        if (CustomAttributes.ContainsKey(propertyKey))
            return true;

        return _targetingContexts.Values.Any(context => context.HasProperty(propertyKey));
    }

    /// <summary>
    /// ��ȡ�������Լ�
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
    /// ��֤�����ĵ���Ч��
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(UserId))
            return false;

        if (Timestamp == default)
            return false;

        // ��֤���ж��������ĵ���Ч��
        return _targetingContexts.Values.All(context => context.IsValid());
    }

    /// <summary>
    /// ����������Ƿ��ѹ���
    /// </summary>
    public bool IsExpired(TimeSpan maxAge)
    {
        return DateTime.UtcNow - Timestamp > maxAge;
    }

    /// <summary>
    /// ��ȡ�����ĵ�Ԫ������Ϣ
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
    /// ��ȡ�����ĵĵ�����Ϣ
    /// </summary>
    public string GetDebugInfo()
    {
        var contextTypes = string.Join(", ", _targetingContexts.Keys);
        return $"UserProfile[{UserId}] Status:{Status} Contexts:[{contextTypes}] " +
               $"Quality:{QualityScore.OverallScore} LastActive:{LastActiveTime:yyyy-MM-dd HH:mm:ss} " +
               $"Age:{DateTime.UtcNow - Timestamp:hh\\:mm\\:ss}";
    }

    /// <summary>
    /// ���������ĵ�����������
    /// </summary>
    public ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys)
    {
        // �����򻯵��û����񸱱���ֻ����ָ��������
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
    /// �ϲ���һ�������ĵ�����
    /// </summary>
    public ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        // ���ﷵ��һ���ϲ�����������ģ��������޸ĵ�ǰʵ��
        var mergedProperties = new Dictionary<string, object>();

        // ��ӵ�ǰ�����ĵ�����
        foreach (var property in Properties)
        {
            mergedProperties[property.Key] = property.Value;
        }

        // ��ӻ򸲸����������ĵ�����
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
    /// �Ƿ��Ծ�û�
    /// </summary>
    public bool IsActive => Status == UserStatus.Active && !IsDeleted;



    /// <summary>
    /// �Ƿ���������
    /// </summary>
    public bool IsCompleteProfile => QualityScore.CompletenessScore >= 80;





    #endregion

    #region Private Methods

    /// <summary>
    /// ��ȡ���������ֵ�
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

        // ����Զ�������
        foreach (var attr in CustomAttributes)
        {
            properties[attr.Key] = attr.Value;
        }

        // ��Ӹ������������ĵ�����
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
    /// ���¼�����������
    /// </summary>
    private void RecalculateQualityScore()
    {
        QualityScore = CalculateQualityScore();
        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "QualityScore", QualityScore.OverallScore));
    }

    /// <summary>
    /// ������������
    /// </summary>
    private ProfileQualityScore CalculateQualityScore()
    {
        var completenessScore = CalculateCompletenessScore();
        var accuracyScore = CalculateAccuracyScore();
        var freshnessScore = CalculateFreshnessScore();

        return new ProfileQualityScore(completenessScore, accuracyScore, freshnessScore);
    }

    /// <summary>
    /// ��������������
    /// </summary>
    private int CalculateCompletenessScore()
    {
        var totalContextTypes = 6; // Ԥ�ڵĶ�����������������
        var filledContexts = _targetingContexts.Count;

        // ���ڶ�����������������������������
        var contextScore = (decimal)filledContexts / totalContextTypes * 100;

        // ���ڸ��������ĵ������Խ�һ������
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
    /// ����׼ȷ������
    /// </summary>
    private int CalculateAccuracyScore()
    {
        var score = 100;

        // ��֤�������������ĵ���Ч��
        foreach (var context in _targetingContexts.Values)
        {
            if (!context.IsValid())
                score -= 10;
        }

        return Math.Max(0, score);
    }

    /// <summary>
    /// ����ʱЧ������
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
    /// ��֤�û�ID
    /// </summary>
    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("�û�ID����Ϊ��", nameof(userId));

        if (userId.Length > 255)
            throw new ArgumentException("�û�ID���Ȳ��ܳ���255���ַ�", nameof(userId));
    }

    #endregion
}