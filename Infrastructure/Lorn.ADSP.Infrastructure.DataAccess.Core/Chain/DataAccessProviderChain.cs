using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Chain;

/// <summary>
/// 数据访问提供者链实现
/// 支持缓存优先、数据库后备、自动回写的链式数据访问
/// </summary>
public class DataAccessProviderChain : IDataAccessProviderChain
{
    private readonly ILogger<DataAccessProviderChain> _logger;

    /// <inheritdoc />
    public string ChainId { get; }

    /// <inheritdoc />
    public IReadOnlyList<DataProviderChainNode> Providers { get; }

    /// <inheritdoc />
    public CacheWriteBackStrategy WriteBackStrategy { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="chainId">链标识</param>
    /// <param name="providers">提供者节点列表</param>
    /// <param name="writeBackStrategy">缓存回写策略</param>
    /// <param name="logger">日志记录器</param>
    public DataAccessProviderChain(
        string chainId,
        IReadOnlyList<DataProviderChainNode> providers,
        CacheWriteBackStrategy writeBackStrategy,
        ILogger<DataAccessProviderChain> logger)
    {
        ChainId = chainId ?? throw new ArgumentNullException(nameof(chainId));
        Providers = providers ?? throw new ArgumentNullException(nameof(providers));
        WriteBackStrategy = writeBackStrategy;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!providers.Any())
        {
            throw new ArgumentException("Provider chain must contain at least one provider", nameof(providers));
        }
    }

    /// <inheritdoc />
    public async Task<DataChainExecutionResult<T>> ExecuteGetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionSteps = new List<ProviderExecutionStep>();
        IDataAccessProvider? successfulProvider = null;
        T? result = default;
        Exception? lastException = null;

        _logger.LogDebug("Starting chain execution for GET operation, request {RequestId}, chain {ChainId}",
            context.RequestId, ChainId);

        try
        {
            // 按顺序尝试每个提供者
            foreach (var node in Providers.OrderBy(p => p.Order))
            {
                var stepStopwatch = Stopwatch.StartNew();
                var step = new ProviderExecutionStep
                {
                    Provider = node.Provider,
                    StepOrder = node.Order,
                    OperationType = "GET"
                };

                try
                {
                    _logger.LogTrace("Trying provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);

                    result = await node.Provider.GetAsync<T>(context, cancellationToken);
                    stepStopwatch.Stop();

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = result != null,
                        ExecutionTime = stepStopwatch.Elapsed,
                        DataFound = result != null
                    };

                    executionSteps.Add(step);

                    if (result != null)
                    {
                        successfulProvider = node.Provider;
                        _logger.LogDebug("Data found in provider {ProviderName} for request {RequestId}",
                            node.Provider.GetMetadata().ProviderName, context.RequestId);
                        break;
                    }

                    _logger.LogTrace("No data found in provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);
                }
                catch (Exception ex)
                {
                    stepStopwatch.Stop();
                    lastException = ex;

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = false,
                        ExecutionTime = stepStopwatch.Elapsed,
                        ErrorMessage = ex.Message,
                        DataFound = false
                    };

                    executionSteps.Add(step);

                    _logger.LogWarning(ex, "Provider {ProviderName} failed for request {RequestId}: {ErrorMessage}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId, ex.Message);

                    if (!node.ContinueOnFailure)
                    {
                        _logger.LogWarning("Stopping chain execution due to ContinueOnFailure=false for provider {ProviderName}",
                            node.Provider.GetMetadata().ProviderName);
                        break;
                    }
                }
            }

            stopwatch.Stop();

            // 执行缓存回写逻辑
            bool cacheWriteBackTriggered = false;
            if (result != null && successfulProvider != null && WriteBackStrategy != CacheWriteBackStrategy.None)
            {
                cacheWriteBackTriggered = await TryWriteBackToCacheAsync(context, result, successfulProvider, executionSteps, cancellationToken);
            }

            var executionResult = new DataChainExecutionResult<T>
            {
                IsSuccess = result != null,
                Data = result,
                SuccessfulProvider = successfulProvider,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = cacheWriteBackTriggered,
                ErrorMessage = result == null ? "No data found in any provider" : null,
                Exception = lastException
            };

            _logger.LogDebug("Chain execution completed for request {RequestId}: Success={Success}, Duration={Duration}ms",
                context.RequestId, executionResult.IsSuccess, executionResult.TotalExecutionTime.TotalMilliseconds);

            return executionResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Chain execution failed for request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);

            return new DataChainExecutionResult<T>
            {
                IsSuccess = false,
                Data = default,
                SuccessfulProvider = null,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    /// <inheritdoc />
    public async Task<DataChainExecutionResult<bool>> ExecuteSetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionSteps = new List<ProviderExecutionStep>();
        var successfulProviders = new List<IDataAccessProvider>();
        var exceptions = new List<Exception>();

        _logger.LogDebug("Starting chain execution for SET operation, request {RequestId}, chain {ChainId}",
            context.RequestId, ChainId);

        try
        {
            // 同时写入所有提供者（并行或串行根据配置）
            foreach (var node in Providers.OrderBy(p => p.Order))
            {
                var stepStopwatch = Stopwatch.StartNew();
                var step = new ProviderExecutionStep
                {
                    Provider = node.Provider,
                    StepOrder = node.Order,
                    OperationType = "SET"
                };

                try
                {
                    _logger.LogTrace("Setting data in provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);

                    await node.Provider.SetAsync(context, value, cancellationToken);
                    stepStopwatch.Stop();

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = true,
                        ExecutionTime = stepStopwatch.Elapsed,
                        DataFound = true
                    };

                    executionSteps.Add(step);
                    successfulProviders.Add(node.Provider);

                    _logger.LogDebug("Data set successfully in provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);
                }
                catch (Exception ex)
                {
                    stepStopwatch.Stop();
                    exceptions.Add(ex);

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = false,
                        ExecutionTime = stepStopwatch.Elapsed,
                        ErrorMessage = ex.Message,
                        DataFound = false
                    };

                    executionSteps.Add(step);

                    _logger.LogError(ex, "Failed to set data in provider {ProviderName} for request {RequestId}: {ErrorMessage}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId, ex.Message);

                    if (!node.ContinueOnFailure)
                    {
                        _logger.LogWarning("Stopping chain execution due to ContinueOnFailure=false for provider {ProviderName}",
                            node.Provider.GetMetadata().ProviderName);
                        break;
                    }
                }
            }

            stopwatch.Stop();

            var isSuccess = successfulProviders.Any();
            var executionResult = new DataChainExecutionResult<bool>
            {
                IsSuccess = isSuccess,
                Data = isSuccess,
                SuccessfulProvider = successfulProviders.FirstOrDefault(),
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = !isSuccess ? "Failed to set data in any provider" : null,
                Exception = exceptions.FirstOrDefault()
            };

            _logger.LogDebug("Chain SET execution completed for request {RequestId}: Success={Success}, Duration={Duration}ms",
                context.RequestId, executionResult.IsSuccess, executionResult.TotalExecutionTime.TotalMilliseconds);

            return executionResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Chain SET execution failed for request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);

            return new DataChainExecutionResult<bool>
            {
                IsSuccess = false,
                Data = false,
                SuccessfulProvider = null,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    /// <inheritdoc />
    public async Task<DataChainExecutionResult<bool>> ExecuteRemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionSteps = new List<ProviderExecutionStep>();
        var successfulProviders = new List<IDataAccessProvider>();
        var exceptions = new List<Exception>();

        _logger.LogDebug("Starting chain execution for REMOVE operation, request {RequestId}, chain {ChainId}",
            context.RequestId, ChainId);

        try
        {
            // 从所有提供者中移除数据
            foreach (var node in Providers.OrderBy(p => p.Order))
            {
                var stepStopwatch = Stopwatch.StartNew();
                var step = new ProviderExecutionStep
                {
                    Provider = node.Provider,
                    StepOrder = node.Order,
                    OperationType = "REMOVE"
                };

                try
                {
                    _logger.LogTrace("Removing data from provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);

                    await node.Provider.RemoveAsync(context, cancellationToken);
                    stepStopwatch.Stop();

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = true,
                        ExecutionTime = stepStopwatch.Elapsed,
                        DataFound = true
                    };

                    executionSteps.Add(step);
                    successfulProviders.Add(node.Provider);

                    _logger.LogDebug("Data removed successfully from provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);
                }
                catch (Exception ex)
                {
                    stepStopwatch.Stop();
                    exceptions.Add(ex);

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = false,
                        ExecutionTime = stepStopwatch.Elapsed,
                        ErrorMessage = ex.Message,
                        DataFound = false
                    };

                    executionSteps.Add(step);

                    _logger.LogError(ex, "Failed to remove data from provider {ProviderName} for request {RequestId}: {ErrorMessage}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId, ex.Message);

                    if (!node.ContinueOnFailure)
                    {
                        _logger.LogWarning("Stopping chain execution due to ContinueOnFailure=false for provider {ProviderName}",
                            node.Provider.GetMetadata().ProviderName);
                        break;
                    }
                }
            }

            stopwatch.Stop();

            var isSuccess = successfulProviders.Any();
            var executionResult = new DataChainExecutionResult<bool>
            {
                IsSuccess = isSuccess,
                Data = isSuccess,
                SuccessfulProvider = successfulProviders.FirstOrDefault(),
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = !isSuccess ? "Failed to remove data from any provider" : null,
                Exception = exceptions.FirstOrDefault()
            };

            _logger.LogDebug("Chain REMOVE execution completed for request {RequestId}: Success={Success}, Duration={Duration}ms",
                context.RequestId, executionResult.IsSuccess, executionResult.TotalExecutionTime.TotalMilliseconds);

            return executionResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Chain REMOVE execution failed for request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);

            return new DataChainExecutionResult<bool>
            {
                IsSuccess = false,
                Data = false,
                SuccessfulProvider = null,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    /// <inheritdoc />
    public async Task<DataChainExecutionResult<bool>> ExecuteExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var executionSteps = new List<ProviderExecutionStep>();
        IDataAccessProvider? successfulProvider = null;
        bool exists = false;
        Exception? lastException = null;

        _logger.LogDebug("Starting chain execution for EXISTS operation, request {RequestId}, chain {ChainId}",
            context.RequestId, ChainId);

        try
        {
            // 按顺序检查每个提供者，直到找到或全部检查完毕
            foreach (var node in Providers.OrderBy(p => p.Order))
            {
                var stepStopwatch = Stopwatch.StartNew();
                var step = new ProviderExecutionStep
                {
                    Provider = node.Provider,
                    StepOrder = node.Order,
                    OperationType = "EXISTS"
                };

                try
                {
                    _logger.LogTrace("Checking existence in provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);

                    exists = await node.Provider.ExistsAsync(context, cancellationToken);
                    stepStopwatch.Stop();

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = true,
                        ExecutionTime = stepStopwatch.Elapsed,
                        DataFound = exists
                    };

                    executionSteps.Add(step);

                    if (exists)
                    {
                        successfulProvider = node.Provider;
                        _logger.LogDebug("Data exists in provider {ProviderName} for request {RequestId}",
                            node.Provider.GetMetadata().ProviderName, context.RequestId);
                        break;
                    }

                    _logger.LogTrace("Data does not exist in provider {ProviderName} for request {RequestId}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId);
                }
                catch (Exception ex)
                {
                    stepStopwatch.Stop();
                    lastException = ex;

                    step = new ProviderExecutionStep
                    {
                        Provider = step.Provider,
                        StepOrder = step.StepOrder,
                        OperationType = step.OperationType,
                        IsSuccess = false,
                        ExecutionTime = stepStopwatch.Elapsed,
                        ErrorMessage = ex.Message,
                        DataFound = false
                    };

                    executionSteps.Add(step);

                    _logger.LogWarning(ex, "Provider {ProviderName} failed for EXISTS check, request {RequestId}: {ErrorMessage}",
                        node.Provider.GetMetadata().ProviderName, context.RequestId, ex.Message);

                    if (!node.ContinueOnFailure)
                    {
                        _logger.LogWarning("Stopping chain execution due to ContinueOnFailure=false for provider {ProviderName}",
                            node.Provider.GetMetadata().ProviderName);
                        break;
                    }
                }
            }

            stopwatch.Stop();

            var executionResult = new DataChainExecutionResult<bool>
            {
                IsSuccess = true, // EXISTS操作本身成功（即使没有找到数据）
                Data = exists,
                SuccessfulProvider = successfulProvider,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = null,
                Exception = lastException
            };

            _logger.LogDebug("Chain EXISTS execution completed for request {RequestId}: Exists={Exists}, Duration={Duration}ms",
                context.RequestId, exists, executionResult.TotalExecutionTime.TotalMilliseconds);

            return executionResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Chain EXISTS execution failed for request {RequestId} after {Duration}ms",
                context.RequestId, stopwatch.ElapsedMilliseconds);

            return new DataChainExecutionResult<bool>
            {
                IsSuccess = false,
                Data = false,
                SuccessfulProvider = null,
                ExecutionPath = executionSteps,
                TotalExecutionTime = stopwatch.Elapsed,
                CacheWriteBackTriggered = false,
                ErrorMessage = ex.Message,
                Exception = ex
            };
        }
    }

    /// <summary>
    /// 尝试将数据回写到缓存提供者
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="context">数据访问上下文</param>
    /// <param name="data">要回写的数据</param>
    /// <param name="sourceProvider">数据来源提供者</param>
    /// <param name="executionSteps">执行步骤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功触发回写</returns>
    private async Task<bool> TryWriteBackToCacheAsync<T>(
        DataAccessContext context,
        T data,
        IDataAccessProvider sourceProvider,
        List<ProviderExecutionStep> executionSteps,
        CancellationToken cancellationToken)
    {
        try
        {
            // 找到需要回写的缓存提供者（在源提供者之前的缓存提供者）
            var sourceOrder = Providers.First(p => p.Provider == sourceProvider).Order;
            var cacheProvidersToWriteBack = Providers
                .Where(p => p.Role == ProviderNodeRole.Cache &&
                           p.Order < sourceOrder &&
                           p.EnableWriteBack)
                .ToList();

            if (!cacheProvidersToWriteBack.Any())
            {
                _logger.LogTrace("No cache providers found for write-back for request {RequestId}", context.RequestId);
                return false;
            }

            _logger.LogDebug("Writing back data to {Count} cache providers for request {RequestId}",
                cacheProvidersToWriteBack.Count, context.RequestId);

            var writeBackTasks = new List<Task>();

            foreach (var cacheNode in cacheProvidersToWriteBack)
            {
                var writeBackTask = WriteBackToCacheProviderAsync(cacheNode, context, data, cancellationToken);

                if (WriteBackStrategy == CacheWriteBackStrategy.Synchronous)
                {
                    await writeBackTask; // 同步回写
                }
                else
                {
                    writeBackTasks.Add(writeBackTask); // 异步回写
                }
            }

            if (WriteBackStrategy == CacheWriteBackStrategy.Asynchronous && writeBackTasks.Any())
            {
                // 异步执行回写任务（不等待完成）
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.WhenAll(writeBackTasks);
                        _logger.LogDebug("Asynchronous cache write-back completed for request {RequestId}", context.RequestId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Asynchronous cache write-back failed for request {RequestId}", context.RequestId);
                    }
                }, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to trigger cache write-back for request {RequestId}: {ErrorMessage}",
                context.RequestId, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 向指定的缓存提供者回写数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="cacheNode">缓存节点</param>
    /// <param name="context">数据访问上下文</param>
    /// <param name="data">要回写的数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task WriteBackToCacheProviderAsync<T>(
        DataProviderChainNode cacheNode,
        DataAccessContext context,
        T data,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogTrace("Writing back data to cache provider {ProviderName} for request {RequestId}",
                cacheNode.Provider.GetMetadata().ProviderName, context.RequestId);

            await cacheNode.Provider.SetAsync(context, data, cancellationToken);

            _logger.LogDebug("Successfully wrote back data to cache provider {ProviderName} for request {RequestId}",
                cacheNode.Provider.GetMetadata().ProviderName, context.RequestId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write back data to cache provider {ProviderName} for request {RequestId}: {ErrorMessage}",
                cacheNode.Provider.GetMetadata().ProviderName, context.RequestId, ex.Message);
            // 不重新抛出异常，避免影响主流程
        }
    }
}

