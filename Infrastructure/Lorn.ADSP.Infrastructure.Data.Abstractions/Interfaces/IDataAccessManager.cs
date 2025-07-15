using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 数据访问管理器接口
/// 基于统一抽象接口的数据访问协调器，替代传统的工作单元模式
/// 通过数据访问提供者注册表实现透明的数据访问路由
/// </summary>
public interface IDataAccessManager : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 获取数据访问提供者
    /// 根据上下文信息自动路由到合适的数据提供者
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据访问提供者</returns>
    Task<IDataAccessProvider> GetProviderAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取事务提供者
    /// 根据事务上下文路由到合适的事务管理器
    /// </summary>
    /// <param name="context">事务上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务提供者</returns>
    Task<ITransactionProvider> GetTransactionProviderAsync(TransactionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行数据操作
    /// 支持单一操作和事务操作的统一入口
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="operation">数据操作</param>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task<T> ExecuteAsync<T>(Func<IDataAccessProvider, CancellationToken, Task<T>> operation, DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 在事务中执行操作
    /// 自动管理事务的开始、提交和回滚
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="operation">事务操作</param>
    /// <param name="transactionContext">事务上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<IDataAccessProvider, ITransactionScope, CancellationToken, Task<T>> operation,
        TransactionContext transactionContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行批量操作
    /// 支持跨多个提供者的批量数据操作
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="operations">批量操作定义</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量操作结果</returns>
    Task<BatchOperationResult<T>> ExecuteBatchAsync<T>(
        IEnumerable<BatchOperationDefinition<T>> operations,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 获取健康状态
    /// 检查所有注册的数据提供者的健康状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态报告</returns>
    Task<HealthStatusReport> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取统计信息
    /// 聚合所有数据提供者的统计信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据访问统计信息</returns>
    Task<DataAccessStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 预加载数据
    /// 基于预加载策略批量预取常用数据到缓存
    /// </summary>
    /// <param name="preloadRequests">预加载请求列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预加载结果</returns>
    Task<PreloadResult> PreloadDataAsync(IEnumerable<DataPreloadRequest> preloadRequests, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理资源
    /// 清理缓存、连接池等资源
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>清理任务</returns>
    Task CleanupAsync(CancellationToken cancellationToken = default);
}















