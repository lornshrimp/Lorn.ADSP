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
    /// 事务类型
    /// </summary>
    TransactionType Type { get; }

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    IsolationLevel IsolationLevel { get; }

    /// <summary>
    /// 事务状态
    /// </summary>
    TransactionStatus Status { get; }

    /// <summary>
    /// 事务开始时间
    /// </summary>
    DateTime StartedAt { get; }

    /// <summary>
    /// 事务持续时间
    /// </summary>
    TimeSpan Duration { get; }

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
    /// 创建保存点
    /// </summary>
    /// <param name="name">保存点名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>保存点事务范围</returns>
    Task<ITransactionScope> CreateSavepointAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚到保存点
    /// </summary>
    /// <param name="name">保存点名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册补偿操作
    /// 在事务回滚时执行的操作
    /// </summary>
    /// <param name="compensation">补偿操作</param>
    void RegisterCompensation(Func<CancellationToken, Task> compensation);

    /// <summary>
    /// 获取事务统计信息
    /// </summary>
    /// <returns>事务统计</returns>
    Task<TransactionStatistics> GetStatisticsAsync();
}

/// <summary>
/// 分布式事务范围接口
/// 管理跨多个资源的分布式事务
/// </summary>
public interface IDistributedTransactionScope : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 全局事务标识
    /// </summary>
    string GlobalTransactionId { get; }

    /// <summary>
    /// 本地事务范围列表
    /// </summary>
    IReadOnlyList<ITransactionScope> LocalScopes { get; }

    /// <summary>
    /// 创建本地事务范围
    /// </summary>
    /// <param name="scopeName">范围名称</param>
    /// <param name="options">事务选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>本地事务范围</returns>
    Task<ITransactionScope> CreateLocalScopeAsync(string scopeName, TransactionOptions options, CancellationToken cancellationToken = default);

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
    /// 准备阶段
    /// 两阶段提交的第一阶段
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>准备结果</returns>
    Task<bool> PreparePhaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分布式事务结果
    /// </summary>
    /// <returns>事务结果</returns>
    Task<DistributedTransactionResult> GetResultAsync();
}