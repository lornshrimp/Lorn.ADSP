using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Extensions;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加广告引擎抽象层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAdEngineAbstractions(
        this IServiceCollection services,
        Action<AdEngineAbstractionsOptions>? configure = null)
    {
        var options = new AdEngineAbstractionsOptions();
        configure?.Invoke(options);

        // 注册核心抽象接口
        services.TryAddSingleton<ICallbackProvider, DefaultCallbackProvider>();

        // 注册策略相关服务
        if (options.RegisterStrategyServices)
        {
            services.AddStrategyServices();
        }

        // 注册回调服务
        if (options.RegisterCallbackServices)
        {
            services.AddCallbackServices();
        }

        // 注册监控服务
        if (options.RegisterMonitoringServices)
        {
            services.AddMonitoringServices();
        }

        return services;
    }

    /// <summary>
    /// 添加策略服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddStrategyServices(this IServiceCollection services)
    {
        // 策略服务将在具体实现项目中注册
        return services;
    }

    /// <summary>
    /// 添加回调服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddCallbackServices(this IServiceCollection services)
    {
        // 回调服务将在具体实现项目中注册
        return services;
    }

    /// <summary>
    /// 添加监控服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
    {
        // 监控服务将在具体实现项目中注册
        return services;
    }

    /// <summary>
    /// 注册广告处理策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAdProcessingStrategy<TStrategy>(
        this IServiceCollection services, 
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TStrategy : class, IAdProcessingStrategy
    {
        services.Add(new ServiceDescriptor(typeof(IAdProcessingStrategy), typeof(TStrategy), lifetime));
        services.Add(new ServiceDescriptor(typeof(TStrategy), typeof(TStrategy), lifetime));
        return services;
    }

    /// <summary>
    /// 注册广告引擎回调
    /// </summary>
    /// <typeparam name="TCallback">回调类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAdEngineCallback<TCallback>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCallback : class, IAdEngineCallback
    {
        services.Add(new ServiceDescriptor(typeof(TCallback), typeof(TCallback), lifetime));
        return services;
    }

    /// <summary>
    /// 注册命名的广告引擎回调
    /// </summary>
    /// <typeparam name="TCallback">回调类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="name">回调名称</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddNamedAdEngineCallback<TCallback>(
        this IServiceCollection services,
        string name,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCallback : class, IAdEngineCallback
    {
        services.Add(new ServiceDescriptor(typeof(TCallback), typeof(TCallback), lifetime));
        // 命名服务的注册将在CallbackProvider中处理
        return services;
    }
}

/// <summary>
/// 广告引擎抽象层选项
/// </summary>
public class AdEngineAbstractionsOptions
{
    /// <summary>
    /// 是否注册策略服务
    /// </summary>
    public bool RegisterStrategyServices { get; set; } = true;

    /// <summary>
    /// 是否注册回调服务
    /// </summary>
    public bool RegisterCallbackServices { get; set; } = true;

    /// <summary>
    /// 是否注册监控服务
    /// </summary>
    public bool RegisterMonitoringServices { get; set; } = true;

    /// <summary>
    /// 默认超时时间
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// 是否启用详细日志
    /// </summary>
    public bool EnableVerboseLogging { get; set; } = false;
}

/// <summary>
/// 默认回调提供者实现
/// </summary>
internal class DefaultCallbackProvider : ICallbackProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, IAdEngineCallback> _namedCallbacks = new();

    public DefaultCallbackProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T GetCallback<T>() where T : class, IAdEngineCallback
    {
        var callback = _serviceProvider.GetService<T>();
        if (callback == null)
        {
            throw new Exceptions.CallbackNotFoundException(typeof(T));
        }
        return callback;
    }

    public T GetCallback<T>(string name) where T : class, IAdEngineCallback
    {
        if (_namedCallbacks.TryGetValue(name, out var namedCallback) && namedCallback is T typedCallback)
        {
            return typedCallback;
        }

        throw new Exceptions.CallbackNotFoundException(name, typeof(T));
    }

    public bool HasCallback<T>() where T : class, IAdEngineCallback
    {
        return _serviceProvider.GetService<T>() != null;
    }

    public bool HasCallback(string callbackName)
    {
        return _namedCallbacks.ContainsKey(callbackName);
    }

    public IReadOnlyDictionary<string, IAdEngineCallback> GetAllCallbacks()
    {
        return _namedCallbacks.AsReadOnly();
    }

    public bool TryGetCallback<T>(out T? callback) where T : class, IAdEngineCallback
    {
        callback = _serviceProvider.GetService<T>();
        return callback != null;
    }

    public bool TryGetCallback<T>(string name, out T? callback) where T : class, IAdEngineCallback
    {
        callback = null;
        if (_namedCallbacks.TryGetValue(name, out var namedCallback) && namedCallback is T typedCallback)
        {
            callback = typedCallback;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 注册命名回调
    /// </summary>
    public void RegisterCallback(string name, IAdEngineCallback callback)
    {
        _namedCallbacks[name] = callback;
    }
}