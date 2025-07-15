using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 事务上下文
/// 用于传递事务相关的上下文信息
/// </summary>
public record TransactionContext
{
    /// <summary>
    /// 事务标识
    /// </summary>
    public string TransactionId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 事务类型
    /// </summary>
    public TransactionType TransactionType { get; init; } = TransactionType.Local;

    /// <summary>
    /// 事务范围
    /// </summary>
    public TransactionScope Scope { get; init; } = TransactionScope.Single;

    /// <summary>
    /// 一致性级别
    /// </summary>
    public ConsistencyLevel ConsistencyLevel { get; init; } = ConsistencyLevel.StrongConsistency;

    /// <summary>
    /// 参与的提供者列表
    /// </summary>
    public IReadOnlyList<string> ParticipatingProviders { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 事务元数据
    /// </summary>
    public IReadOnlyDictionary<string, object> TransactionMetadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 超时时间
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 隔离级别
    /// </summary>
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.ReadCommitted;

    /// <summary>
    /// 是否启用自动提交
    /// </summary>
    public bool AutoCommit { get; init; } = false;

    /// <summary>
    /// 请求标识
    /// </summary>
    public string RequestId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 用户标识
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// 会话标识
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// 追踪标识
    /// </summary>
    public string? TraceId { get; init; }
}



/// <summary>
/// 事务元数据
/// 描述事务管理器的能力和特性
/// </summary>
public record TransactionMetadata
{
    /// <summary>
    /// 技术类型
    /// </summary>
    public required TransactionTechnology Technology { get; init; }

    /// <summary>
    /// 支持的隔离级别
    /// </summary>
    public required IsolationLevel SupportedIsolationLevels { get; init; }

    /// <summary>
    /// 是否支持分布式事务
    /// </summary>
    public required bool SupportsDistributed { get; init; }

    /// <summary>
    /// 是否支持嵌套事务
    /// </summary>
    public required bool SupportsNested { get; init; }

    /// <summary>
    /// 是否支持保存点
    /// </summary>
    public required bool SupportsSavepoints { get; init; }

    /// <summary>
    /// 最大事务持续时间
    /// </summary>
    public required TimeSpan MaxTransactionDuration { get; init; }

    /// <summary>
    /// 最大并发事务数
    /// </summary>
    public required int MaxConcurrentTransactions { get; init; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 事务选项
/// 用于配置事务的行为
/// </summary>
public record TransactionOptions
{
    /// <summary>
    /// 隔离级别
    /// </summary>
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.ReadCommitted;

    /// <summary>
    /// 超时时间
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 是否启用自动重试
    /// </summary>
    public bool EnableAutoRetry { get; init; } = false;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// 事务优先级
    /// </summary>
    public TransactionPriority Priority { get; init; } = TransactionPriority.Normal;

    /// <summary>
    /// 自定义属性
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 分布式事务选项
/// 用于配置分布式事务的行为
/// </summary>
public record DistributedTransactionOptions
{
    /// <summary>
    /// 全局超时时间
    /// </summary>
    public TimeSpan GlobalTimeout { get; init; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// 一致性级别
    /// </summary>
    public ConsistencyLevel ConsistencyLevel { get; init; } = ConsistencyLevel.StrongConsistency;

    /// <summary>
    /// 是否启用两阶段提交
    /// </summary>
    public bool EnableTwoPhaseCommit { get; init; } = true;

    /// <summary>
    /// 参与者超时时间
    /// </summary>
    public TimeSpan ParticipantTimeout { get; init; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// 最大参与者数量
    /// </summary>
    public int MaxParticipants { get; init; } = 10;

    /// <summary>
    /// 协调策略
    /// </summary>
    public CoordinationStrategy CoordinationStrategy { get; init; } = CoordinationStrategy.Centralized;

    /// <summary>
    /// 自定义属性
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 事务统计信息
/// </summary>
public record TransactionStatistics
{
    /// <summary>
    /// 事务标识
    /// </summary>
    public required string TransactionId { get; init; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndedAt { get; init; }

    /// <summary>
    /// 持续时间
    /// </summary>
    public TimeSpan Duration => EndedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// 操作次数
    /// </summary>
    public int OperationCount { get; init; }

    /// <summary>
    /// 读取次数
    /// </summary>
    public int ReadCount { get; init; }

    /// <summary>
    /// 写入次数
    /// </summary>
    public int WriteCount { get; init; }

    /// <summary>
    /// 锁等待时间
    /// </summary>
    public TimeSpan LockWaitTime { get; init; }

    /// <summary>
    /// 扩展统计信息
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedStatistics { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 分布式事务统计信息
/// </summary>
public record DistributedTransactionStatistics
{
    /// <summary>
    /// 全局事务标识
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndedAt { get; init; }

    /// <summary>
    /// 总持续时间
    /// </summary>
    public TimeSpan TotalDuration => EndedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// 参与者数量
    /// </summary>
    public int ParticipantCount { get; init; }

    /// <summary>
    /// 本地事务统计列表
    /// </summary>
    public IReadOnlyList<TransactionStatistics> LocalTransactionStatistics { get; init; } = Array.Empty<TransactionStatistics>();

    /// <summary>
    /// 准备阶段持续时间
    /// </summary>
    public TimeSpan PreparePhaseTime { get; init; }

    /// <summary>
    /// 提交阶段持续时间
    /// </summary>
    public TimeSpan CommitPhaseTime { get; init; }

    /// <summary>
    /// 协调开销时间
    /// </summary>
    public TimeSpan CoordinationOverhead { get; init; }
}

/// <summary>
/// 事务能力
/// </summary>
public record TransactionCapabilities
{
    /// <summary>
    /// 支持的事务类型
    /// </summary>
    public required TransactionType[] SupportedTypes { get; init; }

    /// <summary>
    /// 支持的隔离级别
    /// </summary>
    public required IsolationLevel[] SupportedIsolationLevels { get; init; }

    /// <summary>
    /// 最大嵌套深度
    /// </summary>
    public int MaxNestingDepth { get; init; }

    /// <summary>
    /// 是否支持只读事务
    /// </summary>
    public bool SupportsReadOnlyTransactions { get; init; }

    /// <summary>
    /// 是否支持跨数据库事务
    /// </summary>
    public bool SupportsCrossDatabaseTransactions { get; init; }

    /// <summary>
    /// 性能特性
    /// </summary>
    public PerformanceCharacteristics Performance { get; init; } = new();
}

/// <summary>
/// 性能特性
/// </summary>
public record PerformanceCharacteristics
{
    /// <summary>
    /// 平均提交时间
    /// </summary>
    public TimeSpan AverageCommitTime { get; init; }

    /// <summary>
    /// 平均回滚时间
    /// </summary>
    public TimeSpan AverageRollbackTime { get; init; }

    /// <summary>
    /// 最大吞吐量（每秒事务数）
    /// </summary>
    public int MaxThroughput { get; init; }

    /// <summary>
    /// 锁争用概率
    /// </summary>
    public double LockContentionProbability { get; init; }
}

/// <summary>
/// 协调结果
/// </summary>
public record CoordinationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 全局事务标识
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// 参与者结果
    /// </summary>
    public IReadOnlyList<ParticipantResult> ParticipantResults { get; init; } = Array.Empty<ParticipantResult>();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 协调持续时间
    /// </summary>
    public TimeSpan CoordinationTime { get; init; }
}

/// <summary>
/// 参与者结果
/// </summary>
public record ParticipantResult
{
    /// <summary>
    /// 参与者标识
    /// </summary>
    public required string ParticipantId { get; init; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public TimeSpan ProcessingTime { get; init; }
}

/// <summary>
/// 分布式事务结果
/// </summary>
public record DistributedTransactionResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 全局事务标识
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// 最终状态
    /// </summary>
    public required DistributedTransactionStatus FinalStatus { get; init; }

    /// <summary>
    /// 本地事务结果
    /// </summary>
    public IReadOnlyList<TransactionResult> LocalResults { get; init; } = Array.Empty<TransactionResult>();

    /// <summary>
    /// 统计信息
    /// </summary>
    public DistributedTransactionStatistics? Statistics { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// 事务结果
/// </summary>
public record TransactionResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 事务标识
    /// </summary>
    public required string TransactionId { get; init; }

    /// <summary>
    /// 最终状态
    /// </summary>
    public required TransactionStatus FinalStatus { get; init; }

    /// <summary>
    /// 影响的记录数
    /// </summary>
    public int AffectedRows { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 统计信息
    /// </summary>
    public TransactionStatistics? Statistics { get; init; }
}