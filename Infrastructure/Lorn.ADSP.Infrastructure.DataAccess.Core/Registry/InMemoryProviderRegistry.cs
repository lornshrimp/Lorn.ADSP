using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Concurrent;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Registry;

/// <summary>
/// 内存数据提供者注册表实现
/// 基于内存的线程安全注册表，支持高并发访问
/// </summary>
public class InMemoryProviderRegistry : IDataProviderRegistry
{
    private readonly ConcurrentDictionary<string, IDataAccessProvider> _providers = new();
    private readonly ConcurrentDictionary<string, DataProviderMetadata> _metadata = new();
    private readonly ILogger<InMemoryProviderRegistry> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public InMemoryProviderRegistry(ILogger<InMemoryProviderRegistry> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task RegisterProviderAsync(IDataAccessProvider provider, CancellationToken cancellationToken = default)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        var metadata = provider.GetMetadata();

        if (string.IsNullOrWhiteSpace(metadata.ProviderId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(provider));

        var success = _providers.TryAdd(metadata.ProviderId, provider);
        if (!success)
        {
            _logger.LogWarning("Provider with ID '{ProviderId}' is already registered", metadata.ProviderId);
            throw new InvalidOperationException($"Provider with ID '{metadata.ProviderId}' is already registered");
        }

        _metadata.TryAdd(metadata.ProviderId, metadata);

        _logger.LogInformation("Successfully registered provider '{ProviderName}' with ID '{ProviderId}'",
            metadata.ProviderName, metadata.ProviderId);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IDataAccessProvider?> GetProviderAsync(DataProviderQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var matchingProviders = FilterProviders(query).ToList();

        if (!matchingProviders.Any())
        {
            _logger.LogDebug("No providers found matching query criteria");
            return Task.FromResult<IDataAccessProvider?>(null);
        }

        // 按优先级排序并返回第一个
        var selectedProvider = matchingProviders
            .OrderByDescending(p => p.GetMetadata().Priority)
            .First();

        _logger.LogDebug("Selected provider '{ProviderName}' from {Count} matching providers",
            selectedProvider.GetMetadata().ProviderName, matchingProviders.Count);

        return Task.FromResult<IDataAccessProvider?>(selectedProvider);
    }

    /// <inheritdoc />
    public Task<IEnumerable<IDataAccessProvider>> GetProvidersAsync(DataProviderQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var matchingProviders = FilterProviders(query);

        if (query.OrderByPriority)
        {
            matchingProviders = matchingProviders.OrderByDescending(p => p.GetMetadata().Priority);
        }

        if (query.Limit.HasValue)
        {
            matchingProviders = matchingProviders.Take(query.Limit.Value);
        }

        var result = matchingProviders.ToList();

        _logger.LogDebug("Found {Count} providers matching query criteria", result.Count);

        return Task.FromResult<IEnumerable<IDataAccessProvider>>(result);
    }

    /// <inheritdoc />
    public Task<bool> UnregisterProviderAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(providerId));

        var providerRemoved = _providers.TryRemove(providerId, out var provider);
        var metadataRemoved = _metadata.TryRemove(providerId, out _);

        if (providerRemoved)
        {
            _logger.LogInformation("Successfully unregistered provider with ID '{ProviderId}'", providerId);
        }
        else
        {
            _logger.LogWarning("Provider with ID '{ProviderId}' was not found for unregistration", providerId);
        }

        return Task.FromResult(providerRemoved);
    }

    /// <inheritdoc />
    public Task<IEnumerable<DataProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken = default)
    {
        var metadata = _metadata.Values.ToList();
        _logger.LogDebug("Retrieved metadata for {Count} registered providers", metadata.Count);
        return Task.FromResult<IEnumerable<DataProviderMetadata>>(metadata);
    }

    /// <inheritdoc />
    public Task UpdateProviderHealthStatusAsync(string providerId, HealthStatus healthStatus, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(providerId));

        if (_metadata.TryGetValue(providerId, out var metadata))
        {
            // 直接更新可变属性
            metadata.HealthStatus = healthStatus;
            metadata.LastUpdatedAt = DateTimeOffset.UtcNow;

            _logger.LogDebug("Updated health status for provider '{ProviderId}' to '{HealthStatus}'",
                providerId, healthStatus);
        }
        else
        {
            _logger.LogWarning("Provider with ID '{ProviderId}' not found for health status update", providerId);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> GetProviderCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_providers.Count);
    }

    /// <inheritdoc />
    public Task<bool> IsProviderRegisteredAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            return Task.FromResult(false);

        return Task.FromResult(_providers.ContainsKey(providerId));
    }

    /// <summary>
    /// 根据查询条件过滤提供者
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>匹配的提供者</returns>
    private IEnumerable<IDataAccessProvider> FilterProviders(DataProviderQuery query)
    {
        var providers = _providers.Values.AsEnumerable();

        // 根据提供者ID过滤
        if (query.ProviderIds.Any())
        {
            providers = providers.Where(p => query.ProviderIds.Contains(p.GetMetadata().ProviderId));
        }

        // 根据提供者类型过滤
        if (query.ProviderType.HasValue)
        {
            providers = providers.Where(p => p.GetMetadata().ProviderType == query.ProviderType.Value);
        }

        // 根据业务实体过滤
        if (!string.IsNullOrWhiteSpace(query.BusinessEntity))
        {
            providers = providers.Where(p => string.Equals(p.GetMetadata().BusinessEntity, query.BusinessEntity, StringComparison.OrdinalIgnoreCase));
        }

        // 根据技术类型过滤
        if (!string.IsNullOrWhiteSpace(query.TechnologyType))
        {
            providers = providers.Where(p => string.Equals(p.GetMetadata().TechnologyType, query.TechnologyType, StringComparison.OrdinalIgnoreCase));
        }

        // 根据平台类型过滤
        if (!string.IsNullOrWhiteSpace(query.PlatformType))
        {
            providers = providers.Where(p => string.Equals(p.GetMetadata().PlatformType, query.PlatformType, StringComparison.OrdinalIgnoreCase));
        }

        // 根据标签过滤
        if (query.Tags.Any())
        {
            providers = providers.Where(p =>
                query.Tags.Any(tag => p.GetMetadata().Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
        }

        // 根据支持的操作过滤
        if (query.SupportedOperations.Any())
        {
            providers = providers.Where(p =>
                query.SupportedOperations.All(op => p.GetMetadata().SupportedOperations.Contains(op, StringComparer.OrdinalIgnoreCase)));
        }

        // 根据启用状态过滤
        if (query.OnlyEnabled)
        {
            providers = providers.Where(p => p.GetMetadata().IsEnabled);
        }

        // 根据健康状态过滤
        if (query.OnlyHealthy)
        {
            providers = providers.Where(p => p.GetMetadata().HealthStatus == HealthStatus.Healthy);
        }

        // 根据优先级范围过滤
        if (query.MinPriority.HasValue)
        {
            providers = providers.Where(p => p.GetMetadata().Priority >= query.MinPriority.Value);
        }

        if (query.MaxPriority.HasValue)
        {
            providers = providers.Where(p => p.GetMetadata().Priority <= query.MaxPriority.Value);
        }

        // 根据版本过滤
        if (!string.IsNullOrWhiteSpace(query.Version))
        {
            providers = providers.Where(p => string.Equals(p.GetMetadata().Version, query.Version, StringComparison.OrdinalIgnoreCase));
        }

        // 根据扩展元数据过滤
        if (query.ExtendedMetadataFilter.Any())
        {
            providers = providers.Where(p =>
                query.ExtendedMetadataFilter.All(kvp =>
                    p.GetMetadata().ExtendedMetadata.ContainsKey(kvp.Key) &&
                    Equals(p.GetMetadata().ExtendedMetadata[kvp.Key], kvp.Value)));
        }

        return providers;
    }
}
