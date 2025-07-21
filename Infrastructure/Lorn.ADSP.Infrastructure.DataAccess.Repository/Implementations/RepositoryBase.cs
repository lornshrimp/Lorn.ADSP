using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.DataAccess.Repository.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.DataAccess.Repository.Implementations;

/// <summary>
/// 基础仓储实现
/// 提供通用的仓储操作基础实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    protected readonly IDataAccessRouter DataRouter;
    protected readonly ILogger Logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dataRouter">数据访问路由器</param>
    /// <param name="logger">日志记录器</param>
    protected RepositoryBase(IDataAccessRouter dataRouter, ILogger logger)
    {
        DataRouter = dataRouter ?? throw new ArgumentNullException(nameof(dataRouter));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取实体名称
    /// </summary>
    protected virtual string EntityName => typeof(TEntity).Name;

    /// <summary>
    /// 获取上下文信息
    /// </summary>
    /// <returns>数据访问上下文</returns>
    protected virtual DataAccessContext CreateContext()
    {
        return new DataAccessContext
        {
            EntityType = typeof(TEntity).Name,
            OperationType = "Query",
            RequestId = Guid.NewGuid().ToString(),
            Parameters = new Dictionary<string, object>()
        };
    }

    /// <inheritdoc />
    public abstract Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证实体
    /// </summary>
    /// <param name="entity">实体实例</param>
    /// <param name="operation">操作类型</param>
    /// <returns>验证结果</returns>
    protected virtual bool ValidateEntity(TEntity entity, string operation)
    {
        if (entity == null)
            return false;

        // 可以在派生类中重写以添加自定义验证逻辑
        return true;
    }

    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="operation">操作类型</param>
    /// <param name="parameters">操作参数</param>
    /// <param name="duration">操作耗时</param>
    protected virtual void LogOperation(string operation, object? parameters, TimeSpan duration)
    {
        Logger.LogInformation("Repository operation completed: {Operation} on {Entity} took {Duration}ms",
            operation, EntityName, duration.TotalMilliseconds);

        if (duration.TotalSeconds > 5) // 记录慢查询
        {
            Logger.LogWarning("Slow repository operation detected: {Operation} on {Entity} took {Duration}ms",
                operation, EntityName, duration.TotalMilliseconds);
        }
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="ex">异常实例</param>
    /// <param name="operation">操作类型</param>
    /// <param name="parameters">操作参数</param>
    protected virtual void HandleException(Exception ex, string operation, object? parameters)
    {
        Logger.LogError(ex, "Repository operation failed: {Operation} on {Entity} with parameters {Parameters}",
            operation, EntityName, parameters);
    }
}

/// <summary>
/// 基础仓储实现（主键为string类型）
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public abstract class RepositoryBase<TEntity> : RepositoryBase<TEntity, string>, IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dataRouter">数据访问路由器</param>
    /// <param name="logger">日志记录器</param>
    protected RepositoryBase(IDataAccessRouter dataRouter, ILogger logger) : base(dataRouter, logger)
    {
    }
}
