using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Lorn.ADSP.Infrastructure.DataAccess.Repository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lorn.ADSP.Infrastructure.DataAccess.Repository.Implementations;

/// <summary>
/// 工作单元基础实现
/// 管理仓储实例和事务
/// </summary>
public abstract class UnitOfWorkBase : IUnitOfWork
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IDataAccessRouter DataRouter;
    protected readonly ILogger Logger;
    private readonly Dictionary<Type, object> _repositories = new();
    private ITransaction? _currentTransaction;
    private bool _disposed = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="dataRouter">数据访问路由器</param>
    /// <param name="logger">日志记录器</param>
    protected UnitOfWorkBase(
        IServiceProvider serviceProvider,
        IDataAccessRouter dataRouter,
        ILogger logger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        DataRouter = dataRouter ?? throw new ArgumentNullException(nameof(dataRouter));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public abstract Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
    {
        var repositoryType = typeof(IRepository<TEntity, TKey>);

        if (_repositories.TryGetValue(repositoryType, out var existingRepository))
        {
            return (IRepository<TEntity, TKey>)existingRepository;
        }

        // 尝试从DI容器获取
        var repository = ServiceProvider.GetService<IRepository<TEntity, TKey>>();
        if (repository != null)
        {
            _repositories[repositoryType] = repository;
            return repository;
        }

        // 如果没有注册，创建默认实现
        repository = CreateRepository<TEntity, TKey>();
        _repositories[repositoryType] = repository;

        Logger.LogDebug("Created repository for entity type {EntityType}", typeof(TEntity).Name);

        return repository;
    }

    /// <inheritdoc />
    public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var repositoryType = typeof(IRepository<TEntity>);

        if (_repositories.TryGetValue(repositoryType, out var existingRepository))
        {
            return (IRepository<TEntity>)existingRepository;
        }

        // 尝试从DI容器获取
        var repository = ServiceProvider.GetService<IRepository<TEntity>>();
        if (repository != null)
        {
            _repositories[repositoryType] = repository;
            return repository;
        }

        // 如果没有注册，创建默认实现
        repository = CreateStringKeyRepository<TEntity>();
        _repositories[repositoryType] = repository;

        Logger.LogDebug("Created repository for entity type {EntityType}", typeof(TEntity).Name);

        return repository;
    }

    /// <summary>
    /// 创建仓储实例
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <returns>仓储实例</returns>
    protected abstract IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>() where TEntity : class;

    /// <summary>
    /// 创建字符串主键的仓储实例
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    protected abstract IRepository<TEntity> CreateStringKeyRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// 获取当前事务
    /// </summary>
    protected ITransaction? CurrentTransaction => _currentTransaction;

    /// <summary>
    /// 设置当前事务
    /// </summary>
    /// <param name="transaction">事务实例</param>
    protected void SetCurrentTransaction(ITransaction? transaction)
    {
        _currentTransaction = transaction;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                _currentTransaction?.Dispose();
                _repositories.Clear();

                Logger.LogDebug("UnitOfWork disposed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while disposing UnitOfWork");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 事务基础实现
/// </summary>
public abstract class TransactionBase : ITransaction
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger;
    private bool _disposed = false;
    private bool _committed = false;
    private bool _rolledBack = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="transactionId">事务ID</param>
    /// <param name="logger">日志记录器</param>
    protected TransactionBase(string transactionId, ILogger logger)
    {
        TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Logger.LogDebug("Transaction {TransactionId} created", TransactionId);
    }

    /// <inheritdoc />
    public string TransactionId { get; }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TransactionBase));

        if (_committed)
            throw new InvalidOperationException("Transaction has already been committed");

        if (_rolledBack)
            throw new InvalidOperationException("Transaction has been rolled back");

        try
        {
            await DoCommitAsync(cancellationToken);
            _committed = true;

            Logger.LogInformation("Transaction {TransactionId} committed successfully", TransactionId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to commit transaction {TransactionId}", TransactionId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TransactionBase));

        if (_committed)
            throw new InvalidOperationException("Transaction has already been committed");

        if (_rolledBack)
            return; // 已经回滚，直接返回

        try
        {
            await DoRollbackAsync(cancellationToken);
            _rolledBack = true;

            Logger.LogInformation("Transaction {TransactionId} rolled back successfully", TransactionId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to rollback transaction {TransactionId}", TransactionId);
            throw;
        }
    }

    /// <summary>
    /// 执行提交操作
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    protected abstract Task DoCommitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 执行回滚操作
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    protected abstract Task DoRollbackAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                if (!_committed && !_rolledBack)
                {
                    // 如果事务既没有提交也没有回滚，自动回滚
                    RollbackAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while disposing transaction {TransactionId}", TransactionId);
            }
            finally
            {
                _disposed = true;
                Logger.LogDebug("Transaction {TransactionId} disposed", TransactionId);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
