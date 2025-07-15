using System.Data;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 事务管理器接口
/// 提供统一的事务管理抽象，支持本地事务和分布式事务
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// 事务管理器元数据
    /// </summary>
    TransactionMetadata Metadata { get; }

    /// <summary>
    /// 开始本地事务
    /// </summary>
    /// <param name="options">事务选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务范围</returns>
    Task<ITransactionScope> BeginTransactionAsync(TransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 开始分布式事务
    /// </summary>
    /// <param name="options">分布式事务选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分布式事务范围</returns>
    Task<IDistributedTransactionScope> BeginDistributedTransactionAsync(DistributedTransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 在事务中执行操作
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="operation">事务操作</param>
    /// <param name="options">事务选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<ITransactionScope, CancellationToken, Task<T>> operation, TransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否支持指定的事务类型
    /// </summary>
    /// <param name="type">事务类型</param>
    /// <returns>是否支持</returns>
    Task<bool> SupportsTransactionType(TransactionType type);

    /// <summary>
    /// 获取事务能力信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务能力</returns>
    Task<TransactionCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 事务协调器接口
/// 管理分布式事务的协调和一致性
/// </summary>
public interface ITransactionCoordinator
{
    /// <summary>
    /// 创建分布式事务
    /// </summary>
    /// <param name="options">分布式事务选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分布式事务范围</returns>
    Task<IDistributedTransactionScope> CreateDistributedTransactionAsync(DistributedTransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册事务参与者
    /// </summary>
    /// <param name="participant">事务参与者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>注册结果</returns>
    Task<bool> RegisterParticipantAsync(ITransactionManager participant, CancellationToken cancellationToken = default);

    /// <summary>
    /// 协调提交
    /// </summary>
    /// <param name="globalTransactionId">全局事务标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>协调结果</returns>
    Task<CoordinationResult> CoordinateCommitAsync(string globalTransactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 协调回滚
    /// </summary>
    /// <param name="globalTransactionId">全局事务标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>协调结果</returns>
    Task<CoordinationResult> CoordinateRollbackAsync(string globalTransactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的事务参与者
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务参与者列表</returns>
    Task<IEnumerable<ITransactionManager>> GetActiveParticipantsAsync(CancellationToken cancellationToken = default);
}