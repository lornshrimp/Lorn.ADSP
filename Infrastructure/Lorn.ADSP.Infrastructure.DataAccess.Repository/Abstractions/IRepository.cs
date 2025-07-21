using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.DataAccess.Repository.Abstractions;

/// <summary>
/// 通用仓储接口
/// 定义标准的CRUD操作和查询方法
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体实例</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查找实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体集合</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查找单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体实例</returns>
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体数量</returns>
    Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件删除实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的数量</returns>
    Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}

/// <summary>
/// 通用仓储接口（主键为string类型）
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, string> where TEntity : class
{
}

/// <summary>
/// 工作单元接口
/// 用于管理事务和批量操作
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事务对象</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取仓储实例
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <returns>仓储实例</returns>
    IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;

    /// <summary>
    /// 获取仓储实例（主键为string类型）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
}

/// <summary>
/// 事务接口
/// </summary>
public interface ITransaction : IDisposable
{
    /// <summary>
    /// 事务ID
    /// </summary>
    string TransactionId { get; }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
