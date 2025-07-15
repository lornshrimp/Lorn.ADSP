using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ���ݷ���������
/// ��װ���ݷ�����������б�Ҫ��Ϣ
/// ����·�ɾ��ߺ����ݷ���ִ��
/// </summary>
public class DataAccessContext
{
    /// <summary>
    /// ��������
    /// �� "Get", "Set", "Remove", "Query", "Batch"
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// ʵ������
    /// �� "Advertisement", "UserProfile", "Targeting"
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// �洢������صĲ�����Ϣ
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// ����һ���Լ���
    /// </summary>
    public DataConsistencyLevel ConsistencyLevel { get; set; } = DataConsistencyLevel.Eventual;

    /// <summary>
    /// ������ʱʱ��
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// �Ƿ��ƹ�����
    /// </summary>
    public bool BypassCache { get; set; } = false;

    /// <summary>
    /// �����ǩ
    /// ���ڷ���ͼ��
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// ����ID
    /// ��������׷�ٺ͵���
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// �û�ID
    /// �����û���ص����ݷ���
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// �⻧ID
    /// ���ڶ��⻧���ݸ���
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ���ȼ�
    /// </summary>
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    /// <summary>
    /// �������
    /// </summary>
    public CachePolicy? CachePolicy { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public RetryPolicy? RetryPolicy { get; set; }

    /// <summary>
    /// ��ȡ����ֵ
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="key">������</param>
    /// <returns>����ֵ</returns>
    public T? GetParameter<T>(string key)
    {
        if (Parameters.TryGetValue(key, out var value))
        {
            return value is T typedValue ? typedValue : default;
        }
        return default;
    }

    /// <summary>
    /// ���ò���ֵ
    /// </summary>
    /// <param name="key">������</param>
    /// <param name="value">����ֵ</param>
    public void SetParameter(string key, object value)
    {
        Parameters[key] = value;
    }

    /// <summary>
    /// ����Ƿ��������
    /// </summary>
    /// <param name="key">������</param>
    /// <returns>�Ƿ����</returns>
    public bool HasParameter(string key)
    {
        return Parameters.ContainsKey(key);
    }

    /// <summary>
    /// ����Ƿ������ǩ
    /// </summary>
    /// <param name="tag">��ǩ</param>
    /// <returns>�Ƿ����</returns>
    public bool HasTag(string tag)
    {
        return Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ��ӱ�ǩ
    /// </summary>
    /// <param name="tag">��ǩ</param>
    public void AddTag(string tag)
    {
        if (!HasTag(tag))
        {
            Tags = Tags.Append(tag).ToArray();
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <returns>�����ĸ���</returns>
    public DataAccessContext Clone()
    {
        return new DataAccessContext
        {
            OperationType = OperationType,
            EntityType = EntityType,
            Parameters = new Dictionary<string, object>(Parameters),
            ConsistencyLevel = ConsistencyLevel,
            Timeout = Timeout,
            BypassCache = BypassCache,
            Tags = Tags.ToArray(),
            RequestId = RequestId,
            UserId = UserId,
            TenantId = TenantId,
            CreatedAt = CreatedAt,
            Priority = Priority,
            CachePolicy = CachePolicy,
            RetryPolicy = RetryPolicy
        };
    }
}

/// <summary>
/// �������
/// </summary>
public class CachePolicy
{
    /// <summary>
    /// �������ʱ��
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// �����ǰ׺
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// �Ƿ����û���
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// �����������
    /// </summary>
    public CacheStrategy Strategy { get; set; } = CacheStrategy.WriteThrough;
}

/// <summary>
/// ���Բ���
/// </summary>
public class RetryPolicy
{
    /// <summary>
    /// ������Դ���
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// ���Լ��
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// �˱ܲ���
    /// </summary>
    public BackoffStrategy BackoffStrategy { get; set; } = BackoffStrategy.Exponential;

    /// <summary>
    /// �����Ե��쳣����
    /// </summary>
    public Type[] RetryableExceptions { get; set; } = Array.Empty<Type>();
}