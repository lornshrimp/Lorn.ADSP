using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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



