namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;











/// <summary>
/// 仓储接口
/// 继承只读仓储接口，添加增删改操作和批量处理能力
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> : IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体集合</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新任务</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新任务</returns>
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除任务</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除任务</returns>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规格删除实体
    /// </summary>
    /// <param name="specification">删除规格</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的记录数</returns>
    Task<int> DeleteAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新
    /// </summary>
    /// <param name="specification">更新条件</param>
    /// <param name="updateValues">更新值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的记录数</returns>
    Task<int> BulkUpdateAsync(ISpecification<T> specification, object updateValues, CancellationToken cancellationToken = default);
}