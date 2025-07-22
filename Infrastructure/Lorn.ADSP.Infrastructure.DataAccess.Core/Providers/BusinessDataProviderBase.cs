using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Providers;

/// <summary>
/// 业务数据提供者抽象基类
/// 专为业务数据提供者设计，提供基础的链式数据访问逻辑和缓存管理功能
/// 技术提供者（如Redis、SqlServer等）不应继承此类，而应直接实现IDataAccessProvider接口
/// </summary>
public abstract class BusinessDataProviderBase : IDataAccessProvider
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// 数据访问路由器
    /// </summary>
    protected readonly IDataAccessRouter _router;

    private readonly Lazy<DataProviderMetadata> _metadata;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="router">数据访问路由器</param>
    protected BusinessDataProviderBase(ILogger logger, IDataAccessRouter router)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _router = router ?? throw new ArgumentNullException(nameof(router));
        _metadata = new Lazy<DataProviderMetadata>(CreateMetadata);
    }

    #region IComponent 实现

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;

        await InitializeProviderInternalAsync(cancellationToken);
        IsInitialized = true;

        _logger.LogInformation("Data provider {ProviderName} initialized successfully", Name);
    }

    /// <inheritdoc />
    public virtual async Task DisposeAsync()
    {
        if (!IsInitialized)
            return;

        await DisposeProviderInternalAsync();
        IsInitialized = false;

        _logger.LogInformation("Data provider {ProviderName} disposed", Name);
    }

    #endregion

    #region IHealthCheckable 实现

    /// <inheritdoc />
    public virtual async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await HealthCheckDirectAsync(cancellationToken);
            return isHealthy ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for provider {ProviderName}", Name);
            return HealthStatus.Unhealthy;
        }
    }

    #endregion

    /// <inheritdoc />
    public DataProviderMetadata GetMetadata() => _metadata.Value;

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug("Getting data for request {RequestId} using provider {ProviderName}",
                context.RequestId, GetMetadata().ProviderName);

            // 如果是业务数据提供者，使用链式访问
            if (GetMetadata().ProviderType == DataProviderType.BusinessLogic)
            {
                return await GetWithProviderChainAsync<T>(context, cancellationToken);
            }

            // 技术提供者直接访问
            return await GetDirectAsync<T>(context, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get data for request {RequestId} using provider {ProviderName} after {Duration}ms",
                context.RequestId, GetMetadata().ProviderName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogDebug("Completed get operation for request {RequestId} in {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc />
    public virtual async Task SetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug("Setting data for request {RequestId} using provider {ProviderName}",
                context.RequestId, GetMetadata().ProviderName);

            // 如果是业务数据提供者，使用链式访问
            if (GetMetadata().ProviderType == DataProviderType.BusinessLogic)
            {
                await SetWithProviderChainAsync(context, value, cancellationToken);
                return;
            }

            // 技术提供者直接访问
            await SetDirectAsync(context, value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set data for request {RequestId} using provider {ProviderName} after {Duration}ms",
                context.RequestId, GetMetadata().ProviderName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogDebug("Completed set operation for request {RequestId} in {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        try
        {
            // 如果是业务数据提供者，使用链式访问
            if (GetMetadata().ProviderType == DataProviderType.BusinessLogic)
            {
                return await ExistsWithProviderChainAsync(context, cancellationToken);
            }

            // 技术提供者直接访问
            return await ExistsDirectAsync(context, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check existence for request {RequestId} using provider {ProviderName}",
                context.RequestId, GetMetadata().ProviderName);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task RemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        try
        {
            // 如果是业务数据提供者，使用链式访问
            if (GetMetadata().ProviderType == DataProviderType.BusinessLogic)
            {
                await RemoveWithProviderChainAsync(context, cancellationToken);
                return;
            }

            // 技术提供者直接访问
            await RemoveDirectAsync(context, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove data for request {RequestId} using provider {ProviderName}",
                context.RequestId, GetMetadata().ProviderName);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<DataProviderStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        return await GetStatisticsDirectAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        return await HealthCheckDirectAsync(cancellationToken);
    }

    /// <summary>
    /// 使用提供者链进行数据获取
    /// 实现缓存优先→数据库后备→缓存回写的逻辑
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取的数据</returns>
    protected virtual async Task<T?> GetWithProviderChainAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var chain = await _router.BuildProviderChainAsync(context, cancellationToken);
        var result = await chain.ExecuteGetAsync<T>(context, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Provider chain execution failed for request {RequestId}: {ErrorMessage}",
                context.RequestId, result.ErrorMessage);
            return default;
        }

        _logger.LogDebug("Provider chain executed successfully for request {RequestId}, data source: {ProviderName}",
            context.RequestId, result.SuccessfulProvider?.GetMetadata().ProviderName);

        return result.Data;
    }

    /// <summary>
    /// 使用提供者链进行数据设置
    /// 同时写入缓存和数据库
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="value">要设置的数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected virtual async Task SetWithProviderChainAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default)
    {
        var chain = await _router.BuildProviderChainAsync(context, cancellationToken);
        var result = await chain.ExecuteSetAsync(context, value, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Provider chain set operation failed for request {RequestId}: {ErrorMessage}",
                context.RequestId, result.ErrorMessage);
            throw new InvalidOperationException($"Failed to set data: {result.ErrorMessage}", result.Exception);
        }

        _logger.LogDebug("Provider chain set operation completed successfully for request {RequestId}",
            context.RequestId);
    }

    /// <summary>
    /// 使用提供者链检查数据是否存在
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    protected virtual async Task<bool> ExistsWithProviderChainAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var chain = await _router.BuildProviderChainAsync(context, cancellationToken);
        var result = await chain.ExecuteExistsAsync(context, cancellationToken);

        return result.IsSuccess && result.Data;
    }

    /// <summary>
    /// 使用提供者链进行数据删除
    /// 从所有提供者中删除数据
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected virtual async Task RemoveWithProviderChainAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var chain = await _router.BuildProviderChainAsync(context, cancellationToken);
        var result = await chain.ExecuteRemoveAsync(context, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Provider chain remove operation failed for request {RequestId}: {ErrorMessage}",
                context.RequestId, result.ErrorMessage);
            throw new InvalidOperationException($"Failed to remove data: {result.ErrorMessage}", result.Exception);
        }

        _logger.LogDebug("Provider chain remove operation completed successfully for request {RequestId}",
            context.RequestId);
    }

    #region 抽象方法 - 子类必须实现

    /// <summary>
    /// 创建提供者元数据
    /// 子类必须实现此方法以提供具体的元数据信息
    /// </summary>
    /// <returns>提供者元数据</returns>
    protected abstract DataProviderMetadata CreateMetadata();

    /// <summary>
    /// 直接进行数据获取（技术提供者实现）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>获取的数据</returns>
    protected abstract Task<T?> GetDirectAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接进行数据设置（技术提供者实现）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="value">要设置的数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task SetDirectAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接检查数据是否存在（技术提供者实现）
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    protected abstract Task<bool> ExistsDirectAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接进行数据删除（技术提供者实现）
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task RemoveDirectAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接获取统计信息（技术提供者实现）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>统计信息</returns>
    protected abstract Task<DataProviderStatistics> GetStatisticsDirectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 直接进行健康检查（技术提供者实现）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态</returns>
    protected abstract Task<bool> HealthCheckDirectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 初始化提供者内部逻辑（子类可选择性重写）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected virtual Task InitializeProviderInternalAsync(CancellationToken cancellationToken = default)
    {
        // 默认空实现，子类可以重写
        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放提供者内部资源（子类可选择性重写）
    /// </summary>
    /// <returns>任务</returns>
    protected virtual Task DisposeProviderInternalAsync()
    {
        // 默认空实现，子类可以重写
        return Task.CompletedTask;
    }

    #endregion

    #region 虚拟方法 - 子类可选择性重写

    /// <summary>
    /// 获取缓存键生成器
    /// 子类可以重写此方法以提供自定义的缓存键生成逻辑
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <returns>缓存键</returns>
    protected virtual string GenerateCacheKey(DataAccessContext context)
    {
        var entityType = context.EntityType ?? "Unknown";
        var operationType = context.OperationType ?? "Unknown";

        var keyParts = new List<string> { entityType, operationType };

        if (context.Parameters != null)
        {
            foreach (var param in context.Parameters.OrderBy(p => p.Key))
            {
                keyParts.Add($"{param.Key}:{param.Value}");
            }
        }

        var cacheKey = string.Join(":", keyParts);
        _logger.LogTrace("Generated cache key: {CacheKey} for request {RequestId}", cacheKey, context.RequestId);

        return cacheKey;
    }

    /// <summary>
    /// 验证数据访问上下文
    /// 子类可以重写此方法以提供自定义的验证逻辑
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    protected virtual void ValidateContext(DataAccessContext context)
    {
        if (string.IsNullOrWhiteSpace(context.EntityType))
        {
            throw new ArgumentException("EntityType cannot be null or empty", nameof(context));
        }

        if (string.IsNullOrWhiteSpace(context.OperationType))
        {
            throw new ArgumentException("OperationType cannot be null or empty", nameof(context));
        }
    }

    /// <summary>
    /// 处理数据访问异常
    /// 子类可以重写此方法以提供自定义的异常处理逻辑
    /// </summary>
    /// <param name="ex">异常</param>
    /// <param name="context">数据访问上下文</param>
    /// <param name="operation">操作类型</param>
    protected virtual void HandleException(Exception ex, DataAccessContext context, string operation)
    {
        _logger.LogError(ex, "Data access error in provider {ProviderName} for operation {Operation} with request {RequestId}",
            GetMetadata().ProviderName, operation, context.RequestId);
    }

    #endregion
}
