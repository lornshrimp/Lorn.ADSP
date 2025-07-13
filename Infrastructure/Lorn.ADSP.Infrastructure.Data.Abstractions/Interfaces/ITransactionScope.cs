using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 事务范围接口
/// 管理单个事务的生命周期
/// </summary>
public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 事务标识
    /// </summary>
    string TransactionId { get; }

    /// <summary>
    /// 事务状态
    /// </summary>
    TransactionStatus Status { get; }

    /// <summary>
    /// 事务开始时间
    /// </summary>
    DateTime StartedAt { get; }

    /// <summary>
    /// 事务超时时间
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    IsolationLevel IsolationLevel { get; }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提交任务</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加补偿操作
    /// 在事务回滚时执行的操作
    /// </summary>
    /// <param name="compensation">补偿操作</param>
    void AddCompensation(Func<Task> compensation);

    /// <summary>
    /// 检查事务是否可以提交
    /// </summary>
    /// <returns>是否可以提交</returns>
    Task<bool> CanCommitAsync();

    /// <summary>
    /// 获取事务统计信息
    /// </summary>
    /// <returns>事务统计</returns>
    TransactionStatistics GetStatistics();

    /// <summary>
    /// 创建保存点
    /// </summary>
    /// <param name="savepointName">保存点名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建任务</returns>
    Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚到保存点
    /// </summary>
    /// <param name="savepointName">保存点名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default);
}

/// <summary>
/// 分布式事务接口
/// 管理跨多个资源的分布式事务
/// </summary>
public interface IDistributedTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 全局事务标识
    /// </summary>
    string GlobalTransactionId { get; }

    /// <summary>
    /// 事务范围列表
    /// </summary>
    IReadOnlyList<ITransactionScope> Scopes { get; }

    /// <summary>
    /// 分布式事务状态
    /// </summary>
    DistributedTransactionStatus Status { get; }

    /// <summary>
    /// 创建事务范围
    /// </summary>
    /// <param name="scopeName">范围名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务范围</returns>
    Task<ITransactionScope> CreateScopeAsync(string scopeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交所有事务范围
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提交任务</returns>
    Task CommitAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚所有事务范围
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行分布式事务操作
    /// </summary>
    /// <param name="operation">事务操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务结果</returns>
    Task<TransactionResult> ExecuteAsync(Func<ITransactionScope, Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分布式事务统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    DistributedTransactionStatistics GetStatistics();
}