using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;

/// <summary>
/// 数据访问提供者链接口
/// 表示一个有序的数据提供者执行链，支持链式调用和缓存回写
/// </summary>
public interface IDataAccessProviderChain
{
    /// <summary>
    /// 提供者链的唯一标识
    /// </summary>
    string ChainId { get; }

    /// <summary>
    /// 链中的提供者列表（按执行顺序排列）
    /// </summary>
    IReadOnlyList<DataProviderChainNode> Providers { get; }

    /// <summary>
    /// 缓存回写策略
    /// </summary>
    CacheWriteBackStrategy WriteBackStrategy { get; }

    /// <summary>
    /// 异步执行数据获取操作
    /// 按顺序尝试每个提供者，直到获取到数据或全部失败
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    Task<DataChainExecutionResult<T>> ExecuteGetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步执行数据设置操作
    /// 写入所有适用的提供者（通常是缓存和数据库）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="value">要设置的数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    Task<DataChainExecutionResult<bool>> ExecuteSetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步执行数据删除操作
    /// 从所有提供者中删除数据
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    Task<DataChainExecutionResult<bool>> ExecuteRemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步检查数据是否存在
    /// 按顺序检查每个提供者，直到找到或全部检查完毕
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    Task<DataChainExecutionResult<bool>> ExecuteExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// 数据提供者链节点
/// 表示链中的一个提供者及其执行配置
/// </summary>
public sealed class DataProviderChainNode
{
    /// <summary>
    /// 数据提供者
    /// </summary>
    public required IDataAccessProvider Provider { get; init; }

    /// <summary>
    /// 在链中的执行顺序（数值越小越先执行）
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// 节点角色类型
    /// </summary>
    public ProviderNodeRole Role { get; init; }

    /// <summary>
    /// 是否在读取成功时触发缓存回写
    /// </summary>
    public bool EnableWriteBack { get; init; }

    /// <summary>
    /// 失败时是否继续执行下一个节点
    /// </summary>
    public bool ContinueOnFailure { get; init; } = true;

    /// <summary>
    /// 超时设置（可选，覆盖默认超时）
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// 节点级别的扩展元数据
    /// </summary>
    public Dictionary<string, object>? ExtendedMetadata { get; init; }
}

/// <summary>
/// 提供者节点角色类型
/// </summary>
public enum ProviderNodeRole
{
    /// <summary>
    /// 缓存提供者（优先读取，支持回写）
    /// </summary>
    Cache,

    /// <summary>
    /// 数据库提供者（主要数据源）
    /// </summary>
    Database,

    /// <summary>
    /// 外部服务提供者（第三方API等）
    /// </summary>
    ExternalService,

    /// <summary>
    /// 备用提供者（故障转移时使用）
    /// </summary>
    Fallback
}

/// <summary>
/// 缓存回写策略
/// </summary>
public enum CacheWriteBackStrategy
{
    /// <summary>
    /// 不进行回写
    /// </summary>
    None,

    /// <summary>
    /// 同步回写（等待回写完成）
    /// </summary>
    Synchronous,

    /// <summary>
    /// 异步回写（不等待回写完成）
    /// </summary>
    Asynchronous,

    /// <summary>
    /// 延迟回写（批量处理）
    /// </summary>
    Delayed
}

/// <summary>
/// 数据链执行结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public sealed class DataChainExecutionResult<T>
{
    /// <summary>
    /// 执行是否成功
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 返回的数据
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// 成功执行的提供者
    /// </summary>
    public IDataAccessProvider? SuccessfulProvider { get; init; }

    /// <summary>
    /// 执行路径（所有尝试过的提供者）
    /// </summary>
    public required IReadOnlyList<ProviderExecutionStep> ExecutionPath { get; init; }

    /// <summary>
    /// 总执行时间
    /// </summary>
    public TimeSpan TotalExecutionTime { get; init; }

    /// <summary>
    /// 是否触发了缓存回写
    /// </summary>
    public bool CacheWriteBackTriggered { get; init; }

    /// <summary>
    /// 错误信息（如果失败）
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 异常信息（如果有）
    /// </summary>
    public Exception? Exception { get; init; }
}

/// <summary>
/// 提供者执行步骤
/// </summary>
public sealed class ProviderExecutionStep
{
    /// <summary>
    /// 执行的提供者
    /// </summary>
    public required IDataAccessProvider Provider { get; init; }

    /// <summary>
    /// 执行顺序
    /// </summary>
    public int StepOrder { get; init; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string OperationType { get; init; } = string.Empty;

    /// <summary>
    /// 错误信息（如果失败）
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 是否命中数据
    /// </summary>
    public bool DataFound { get; init; }
}
