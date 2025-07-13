using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 只读工作单元接口
/// 提供只读数据访问的仓储集合
/// </summary>
public interface IReadOnlyUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 广告只读仓储
    /// </summary>
    IReadOnlyRepository<Advertisement> Advertisements { get; }

    /// <summary>
    /// 活动只读仓储
    /// </summary>
    IReadOnlyRepository<Campaign> Campaigns { get; }

    /// <summary>
    /// 投放记录只读仓储
    /// </summary>
    IReadOnlyRepository<DeliveryRecord> DeliveryRecords { get; }

    /// <summary>
    /// 媒体资源只读仓储
    /// </summary>
    IReadOnlyRepository<MediaResource> MediaResources { get; }

    /// <summary>
    /// 广告主只读仓储
    /// </summary>
    IReadOnlyRepository<Advertiser> Advertisers { get; }

    /// <summary>
    /// 用户画像只读仓储
    /// </summary>
    IReadOnlyRepository<UserProfile> UserProfiles { get; }

    /// <summary>
    /// 定向配置只读仓储
    /// </summary>
    IReadOnlyRepository<TargetingConfig> TargetingConfigs { get; }

    /// <summary>
    /// 定向策略只读仓储
    /// </summary>
    IReadOnlyRepository<TargetingPolicy> TargetingPolicies { get; }

    /// <summary>
    /// 获取泛型只读仓储
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>只读仓储</returns>
    IReadOnlyRepository<T> GetReadOnlyRepository<T>() where T : class;
}

/// <summary>
/// 工作单元接口
/// 提供完整的数据访问操作和事务管理
/// </summary>
public interface IUnitOfWork : IReadOnlyUnitOfWork
{
    /// <summary>
    /// 广告仓储
    /// </summary>
    new IRepository<Advertisement> Advertisements { get; }

    /// <summary>
    /// 活动仓储
    /// </summary>
    new IRepository<Campaign> Campaigns { get; }

    /// <summary>
    /// 投放记录仓储
    /// </summary>
    new IRepository<DeliveryRecord> DeliveryRecords { get; }

    /// <summary>
    /// 媒体资源仓储
    /// </summary>
    new IRepository<MediaResource> MediaResources { get; }

    /// <summary>
    /// 广告主仓储
    /// </summary>
    new IRepository<Advertiser> Advertisers { get; }

    /// <summary>
    /// 用户画像仓储
    /// </summary>
    new IRepository<UserProfile> UserProfiles { get; }

    /// <summary>
    /// 定向配置仓储
    /// </summary>
    new IRepository<TargetingConfig> TargetingConfigs { get; }

    /// <summary>
    /// 定向策略仓储
    /// </summary>
    new IRepository<TargetingPolicy> TargetingPolicies { get; }

    /// <summary>
    /// 保存所有更改
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的记录数</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务任务</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提交任务</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>回滚任务</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取泛型仓储
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>仓储</returns>
    IRepository<T> GetRepository<T>() where T : class;

    /// <summary>
    /// 检查是否有未提交的更改
    /// </summary>
    /// <returns>是否有未提交的更改</returns>
    bool HasChanges();

    /// <summary>
    /// 清理上下文状态
    /// </summary>
    void ClearContext();
}