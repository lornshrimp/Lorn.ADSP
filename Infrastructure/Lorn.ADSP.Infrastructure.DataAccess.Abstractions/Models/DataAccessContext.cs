using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 数据访问上下文
/// 封装数据访问请求的所有上下文信息
/// </summary>
public sealed class DataAccessContext
{
    /// <summary>
    /// 操作类型（如：Get、Set、Remove、Exists等）
    /// </summary>
    public required string OperationType { get; init; }

    /// <summary>
    /// 实体类型（如：Advertisement、UserProfile等）
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// 操作参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; } = new();

    /// <summary>
    /// 数据一致性级别
    /// </summary>
    public DataConsistencyLevel ConsistencyLevel { get; init; } = DataConsistencyLevel.Eventual;

    /// <summary>
    /// 操作超时时间
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// 是否绕过缓存
    /// </summary>
    public bool BypassCache { get; init; } = false;

    /// <summary>
    /// 标签集合，用于路由和过滤
    /// </summary>
    public string[] Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 请求唯一标识符
    /// </summary>
    public string RequestId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 用户标识
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// 租户标识
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 扩展属性
    /// </summary>
    public Dictionary<string, object> ExtendedProperties { get; init; } = new();

    /// <summary>
    /// 是否为调试模式
    /// </summary>
    public bool IsDebugMode { get; init; } = false;

    /// <summary>
    /// 优先级（用于资源调度）
    /// </summary>
    public int Priority { get; init; } = 0;
}
