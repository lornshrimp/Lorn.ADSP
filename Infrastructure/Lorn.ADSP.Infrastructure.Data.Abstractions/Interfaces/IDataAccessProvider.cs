using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 统一数据访问提供者接口
/// 所有数据访问实现（业务实体、缓存、数据库、云平台）都实现此统一接口
/// 通过元数据驱动实现高度可扩展的数据访问架构
/// </summary>
public interface IDataAccessProvider
{
    /// <summary>
    /// 获取提供者元数据
    /// 包含业务实体类型、技术类型、平台类型等标识信息
    /// </summary>
    /// <returns>数据提供者元数据</returns>
    DataProviderMetadata GetMetadata();

    /// <summary>
    /// 异步获取数据
    /// 支持泛型返回类型，提供类型安全的数据访问
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定类型的数据结果</returns>
    Task<T?> GetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 异步设置数据
    /// 支持数据的创建、更新操作
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="value">要设置的数据值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设置操作任务</returns>
    Task SetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 异步检查数据是否存在
    /// 提供数据存在性检查能力
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据是否存在</returns>
    Task<bool> ExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步删除数据
    /// 提供数据删除能力
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除操作任务</returns>
    Task RemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量数据操作
    /// 支持批量获取、设置、删除等高性能操作
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="operation">批量操作类型</param>
    /// <param name="contexts">数据访问上下文集合</param>
    /// <param name="values">数据值集合（用于批量设置）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量操作结果</returns>
    Task<BatchOperationResult<T>> BatchOperationAsync<T>(
        BatchOperationType operation,
        IEnumerable<DataAccessContext> contexts,
        IEnumerable<T>? values = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 执行查询操作
    /// 支持复杂查询条件、分页、排序等高级查询功能
    /// </summary>
    /// <typeparam name="T">查询结果类型</typeparam>
    /// <param name="query">查询规格</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>查询结果</returns>
    Task<QueryResult<T>> QueryAsync<T>(IQuerySpecification<T> query, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// 获取提供者统计信息
    /// 提供性能监控和健康检查信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提供者统计信息</returns>
    Task<DataProviderStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 健康检查
    /// 检查提供者是否可用
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否健康</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}