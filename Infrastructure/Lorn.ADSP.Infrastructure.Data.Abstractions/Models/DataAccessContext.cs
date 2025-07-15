using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 数据访问上下文
/// 封装数据访问请求的所有必要信息
/// 用于路由决策和数据访问执行
/// </summary>
public class DataAccessContext
{
    /// <summary>
    /// 操作类型
    /// 如 "Get", "Set", "Remove", "Query", "Batch"
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 实体类型
    /// 如 "Advertisement", "UserProfile", "Targeting"
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// 操作参数
    /// 存储操作相关的参数信息
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// 数据一致性级别
    /// </summary>
    public DataConsistencyLevel ConsistencyLevel { get; set; } = DataConsistencyLevel.Eventual;

    /// <summary>
    /// 操作超时时间
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// 是否绕过缓存
    /// </summary>
    public bool BypassCache { get; set; } = false;

    /// <summary>
    /// 请求标签
    /// 用于分类和监控
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 请求ID
    /// 用于请求追踪和调试
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 用户ID
    /// 用于用户相关的数据访问
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 租户ID
    /// 用于多租户数据隔离
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 优先级
    /// </summary>
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    /// <summary>
    /// 缓存策略
    /// </summary>
    public CachePolicy? CachePolicy { get; set; }

    /// <summary>
    /// 重试配置
    /// </summary>
    public RetryPolicy? RetryPolicy { get; set; }

    /// <summary>
    /// 获取参数值
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="key">参数键</param>
    /// <returns>参数值</returns>
    public T? GetParameter<T>(string key)
    {
        if (Parameters.TryGetValue(key, out var value))
        {
            return value is T typedValue ? typedValue : default;
        }
        return default;
    }

    /// <summary>
    /// 设置参数值
    /// </summary>
    /// <param name="key">参数键</param>
    /// <param name="value">参数值</param>
    public void SetParameter(string key, object value)
    {
        Parameters[key] = value;
    }

    /// <summary>
    /// 检查是否包含参数
    /// </summary>
    /// <param name="key">参数键</param>
    /// <returns>是否包含</returns>
    public bool HasParameter(string key)
    {
        return Parameters.ContainsKey(key);
    }

    /// <summary>
    /// 检查是否包含标签
    /// </summary>
    /// <param name="tag">标签</param>
    /// <returns>是否包含</returns>
    public bool HasTag(string tag)
    {
        return Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 添加标签
    /// </summary>
    /// <param name="tag">标签</param>
    public void AddTag(string tag)
    {
        if (!HasTag(tag))
        {
            Tags = Tags.Append(tag).ToArray();
        }
    }

    /// <summary>
    /// 创建副本
    /// </summary>
    /// <returns>上下文副本</returns>
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
/// 缓存策略
/// </summary>
public class CachePolicy
{
    /// <summary>
    /// 缓存持续时间
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// 是否启用缓存
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 缓存策略类型
    /// </summary>
    public CacheStrategy Strategy { get; set; } = CacheStrategy.WriteThrough;
}

/// <summary>
/// 重试策略
/// </summary>
public class RetryPolicy
{
    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// 重试间隔
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// 退避策略
    /// </summary>
    public BackoffStrategy BackoffStrategy { get; set; } = BackoffStrategy.Exponential;

    /// <summary>
    /// 可重试的异常类型
    /// </summary>
    public Type[] RetryableExceptions { get; set; } = Array.Empty<Type>();
}