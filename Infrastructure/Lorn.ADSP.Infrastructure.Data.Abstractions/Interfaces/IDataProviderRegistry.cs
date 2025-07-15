using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 数据提供者注册表接口
/// 负责管理所有数据访问提供者的注册、查询和生命周期
/// 通过元数据标识实现智能路由和动态发现
/// </summary>
public interface IDataProviderRegistry
{
    /// <summary>
    /// 异步注册数据访问提供者
    /// 通过元数据自动分类和索引提供者
    /// </summary>
    /// <param name="provider">数据访问提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>注册任务</returns>
    Task RegisterProviderAsync(IDataAccessProvider provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取数据访问提供者
    /// 根据查询条件返回最佳匹配的提供者
    /// </summary>
    /// <param name="query">数据提供者查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的数据访问提供者</returns>
    Task<IDataAccessProvider?> GetProviderAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取多个数据访问提供者
    /// 根据查询条件返回所有匹配的提供者列表
    /// </summary>
    /// <param name="query">数据提供者查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的数据访问提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步注销数据访问提供者
    /// 从注册表中移除指定的提供者
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功注销</returns>
    Task<bool> UnregisterProviderAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取所有提供者元数据
    /// 返回注册表中所有提供者的元数据信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>所有提供者元数据集合</returns>
    Task<IEnumerable<DataProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取支持指定业务实体的提供者
    /// 按优先级排序返回
    /// </summary>
    /// <param name="businessEntity">业务实体类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>支持该业务实体的提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByBusinessEntityAsync(string businessEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取支持指定技术类型的提供者
    /// 如Redis、SqlServer等
    /// </summary>
    /// <param name="technologyType">技术类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>支持该技术类型的提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByTechnologyAsync(string technologyType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取支持指定云平台的提供者
    /// 如AlibabaCloud、Azure等
    /// </summary>
    /// <param name="platformType">平台类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>支持该平台类型的提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByPlatformAsync(string platformType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查提供者是否已注册
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否已注册</returns>
    Task<bool> IsProviderRegisteredAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取注册表统计信息
    /// 提供注册表的健康状态和使用情况
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>注册表统计信息</returns>
    Task<RegistryStatistics> GetRegistryStatisticsAsync(CancellationToken cancellationToken = default);
}