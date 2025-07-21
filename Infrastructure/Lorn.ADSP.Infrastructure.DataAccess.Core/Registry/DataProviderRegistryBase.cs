using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Registry;

/// <summary>
/// 数据提供者注册表基类实现
/// 提供通用的注册表功能和扩展点
/// </summary>
public abstract class DataProviderRegistryBase : IDataProviderRegistry
{
    protected readonly ILogger Logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    protected DataProviderRegistryBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public abstract Task RegisterProviderAsync(IDataAccessProvider provider, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IDataAccessProvider?> GetProviderAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<IDataAccessProvider>> GetProvidersAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> UnregisterProviderAsync(string providerId, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<DataProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task UpdateProviderHealthStatusAsync(string providerId, HealthStatus healthStatus, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<int> GetProviderCountAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> IsProviderRegisteredAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证提供者
    /// </summary>
    /// <param name="provider">数据提供者</param>
    /// <exception cref="ArgumentNullException">提供者为null时抛出</exception>
    /// <exception cref="ArgumentException">提供者元数据无效时抛出</exception>
    protected virtual void ValidateProvider(IDataAccessProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        var metadata = provider.GetMetadata();

        if (string.IsNullOrWhiteSpace(metadata.ProviderId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(provider));

        if (string.IsNullOrWhiteSpace(metadata.ProviderName))
            throw new ArgumentException("Provider name cannot be null or empty", nameof(provider));

        Logger.LogDebug("Provider validation passed for '{ProviderName}' (ID: {ProviderId})",
            metadata.ProviderName, metadata.ProviderId);
    }

    /// <summary>
    /// 验证提供者ID
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <exception cref="ArgumentException">提供者ID无效时抛出</exception>
    protected virtual void ValidateProviderId(string providerId)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(providerId));
    }

    /// <summary>
    /// 验证查询条件
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <exception cref="ArgumentNullException">查询条件为null时抛出</exception>
    protected virtual void ValidateQuery(DataProviderQuery query)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
    }

    /// <summary>
    /// 记录提供者注册事件
    /// </summary>
    /// <param name="metadata">提供者元数据</param>
    protected virtual void LogProviderRegistration(DataProviderMetadata metadata)
    {
        Logger.LogInformation("Provider registered: {ProviderName} (ID: {ProviderId}, Type: {ProviderType})",
            metadata.ProviderName, metadata.ProviderId, metadata.ProviderType);
    }

    /// <summary>
    /// 记录提供者注销事件
    /// </summary>
    /// <param name="providerId">提供者ID</param>
    /// <param name="success">是否成功注销</param>
    protected virtual void LogProviderUnregistration(string providerId, bool success)
    {
        if (success)
        {
            Logger.LogInformation("Provider unregistered: {ProviderId}", providerId);
        }
        else
        {
            Logger.LogWarning("Failed to unregister provider: {ProviderId} (not found)", providerId);
        }
    }
}
