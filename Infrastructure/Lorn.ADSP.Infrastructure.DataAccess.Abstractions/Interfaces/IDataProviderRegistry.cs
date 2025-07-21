using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;

/// <summary>
/// 数据提供者注册表接口
/// 负责管理所有数据提供者的注册、查询和元数据管理
/// </summary>
public interface IDataProviderRegistry
{
    /// <summary>
    /// 异步注册数据提供者
    /// </summary>
    /// <param name="provider">要注册的数据提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task RegisterProviderAsync(IDataAccessProvider provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取单个数据提供者
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的数据提供者，如果不存在则返回null</returns>
    Task<IDataAccessProvider?> GetProviderAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取多个数据提供者
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的数据提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步注销数据提供者
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果成功注销则返回true，否则返回false</returns>
    Task<bool> UnregisterProviderAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取所有提供者的元数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>所有提供者的元数据集合</returns>
    Task<IEnumerable<DataProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步更新提供者健康状态
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="healthStatus">健康状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task UpdateProviderHealthStatusAsync(string providerId, HealthStatus healthStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取注册的提供者数量
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提供者数量</returns>
    Task<int> GetProviderCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步检查提供者是否已注册
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果已注册则返回true，否则返回false</returns>
    Task<bool> IsProviderRegisteredAsync(string providerId, CancellationToken cancellationToken = default);
}
