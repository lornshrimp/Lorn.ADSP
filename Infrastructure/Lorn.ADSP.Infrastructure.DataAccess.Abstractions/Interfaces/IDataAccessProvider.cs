using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;

/// <summary>
/// 数据访问提供者统一接口
/// 所有数据访问组件的基础接口，定义标准的数据访问方法
/// </summary>
public interface IDataAccessProvider : IComponent, IHealthCheckable
{
    /// <summary>
    /// 获取提供者元数据
    /// </summary>
    /// <returns>提供者元数据</returns>
    DataProviderMetadata GetMetadata();

    /// <summary>
    /// 异步获取数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取到的数据，如果不存在则返回default(T)</returns>
    Task<T?> GetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步设置数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="value">要设置的数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步检查数据是否存在
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果数据存在则返回true，否则返回false</returns>
    Task<bool> ExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步移除数据
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task RemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取提供者统计信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>统计信息</returns>
    Task<DataProviderStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
