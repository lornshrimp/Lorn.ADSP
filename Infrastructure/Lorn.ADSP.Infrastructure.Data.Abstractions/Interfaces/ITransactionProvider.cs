using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 事务数据访问提供者接口
/// 基于统一抽象接口实现的事务管理提供者
/// 通过元数据标识事务技术类型和能力
/// </summary>
public interface ITransactionProvider : IDataAccessProvider
{
    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="context">事务上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务范围</returns>
    Task<ITransactionScope> BeginTransactionAsync(TransactionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="transaction">事务范围</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提交任务</returns>
    Task CommitAsync(ITransactionScope transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="transaction">事务范围</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackAsync(ITransactionScope transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取事务状态
    /// </summary>
    /// <param name="transactionId">事务标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务状态</returns>
    Task<TransactionStatus> GetStatusAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 在事务中执行操作
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="operation">事务操作</param>
    /// <param name="context">事务上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<ITransactionScope, CancellationToken, Task<T>> operation,
        TransactionContext context,
        CancellationToken cancellationToken = default);
}