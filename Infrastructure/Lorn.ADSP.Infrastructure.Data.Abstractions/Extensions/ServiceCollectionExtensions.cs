using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Extensions;

/// <summary>
/// 数据访问服务注册扩展
/// 提供统一的数据访问抽象层服务注册方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加数据访问抽象层服务
    /// 注意：此方法仅注册接口，具体实现应在相应的实现项目中注册
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessAbstractions(this IServiceCollection services)
    {
        // 抽象层只注册接口，不注册具体实现
        // 具体实现应该在对应的实现项目中注册，例如：
        // - Infrastructure.Data 项目注册 IDataProviderRegistry、IDataAccessRouter 的实现
        // - Infrastructure.Data 项目注册 IDataAccessManager、ITransactionManager 的实现
        
        // 这里可以注册一些配置相关的服务
        services.Configure<DataAccessOptions>(_ => { });
        
        return services;
    }

    /// <summary>
    /// 注册数据访问提供者注册表实现
    /// 此方法应在具体实现项目中调用
    /// </summary>
    /// <typeparam name="TRegistry">注册表实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataProviderRegistry<TRegistry>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TRegistry : class, IDataProviderRegistry
    {
        services.Add(new ServiceDescriptor(typeof(IDataProviderRegistry), typeof(TRegistry), lifetime));
        return services;
    }

    /// <summary>
    /// 注册数据访问路由器实现
    /// 此方法应在具体实现项目中调用
    /// </summary>
    /// <typeparam name="TRouter">路由器实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessRouter<TRouter>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TRouter : class, IDataAccessRouter
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessRouter), typeof(TRouter), lifetime));
        return services;
    }

    /// <summary>
    /// 注册数据访问管理器实现
    /// 此方法应在具体实现项目中调用
    /// </summary>
    /// <typeparam name="TManager">管理器实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessManager<TManager>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TManager : class, IDataAccessManager
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessManager), typeof(TManager), lifetime));
        return services;
    }

    /// <summary>
    /// 注册事务管理器实现
    /// 此方法应在具体实现项目中调用
    /// </summary>
    /// <typeparam name="TTransactionManager">事务管理器实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddTransactionManager<TTransactionManager>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TTransactionManager : class, ITransactionManager
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionManager), typeof(TTransactionManager), lifetime));
        return services;
    }

    /// <summary>
    /// 注册事务协调器实现
    /// 此方法应在具体实现项目中调用
    /// </summary>
    /// <typeparam name="TCoordinator">协调器实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddTransactionCoordinator<TCoordinator>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCoordinator : class, ITransactionCoordinator
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionCoordinator), typeof(TCoordinator), lifetime));
        return services;
    }

    /// <summary>
    /// 注册数据访问提供者
    /// </summary>
    /// <typeparam name="TProvider">提供者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessProvider<TProvider>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TProvider : class, IDataAccessProvider
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessProvider), typeof(TProvider), lifetime));
        services.Add(new ServiceDescriptor(typeof(TProvider), typeof(TProvider), lifetime));

        return services;
    }

    /// <summary>
    /// 注册数据访问提供者（使用工厂方法）
    /// </summary>
    /// <typeparam name="TProvider">提供者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="factory">提供者工厂方法</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessProvider<TProvider>(
        this IServiceCollection services,
        Func<IServiceProvider, TProvider> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TProvider : class, IDataAccessProvider
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessProvider), factory, lifetime));
        services.Add(new ServiceDescriptor(typeof(TProvider), factory, lifetime));

        return services;
    }

    /// <summary>
    /// 注册事务提供者
    /// </summary>
    /// <typeparam name="TProvider">事务提供者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddTransactionProvider<TProvider>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TProvider : class, ITransactionProvider
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionProvider), typeof(TProvider), lifetime));
        services.Add(new ServiceDescriptor(typeof(TProvider), typeof(TProvider), lifetime));

        return services;
    }

    /// <summary>
    /// 批量注册数据访问提供者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="providerTypes">提供者类型集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessProviders(
        this IServiceCollection services,
        IEnumerable<Type> providerTypes,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        foreach (var providerType in providerTypes)
        {
            if (providerType.IsClass && !providerType.IsAbstract &&
                typeof(IDataAccessProvider).IsAssignableFrom(providerType))
            {
                services.Add(new ServiceDescriptor(typeof(IDataAccessProvider), providerType, lifetime));
                services.Add(new ServiceDescriptor(providerType, providerType, lifetime));
            }
        }

        return services;
    }

    /// <summary>
    /// 自动扫描并注册数据访问提供者
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessProvidersFromAssemblies(
        this IServiceCollection services,
        IEnumerable<System.Reflection.Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var providerTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract &&
                          typeof(IDataAccessProvider).IsAssignableFrom(type));

        return services.AddDataAccessProviders(providerTypes, lifetime);
    }

    /// <summary>
    /// 注册路由策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRoutingStrategy<TStrategy>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TStrategy : class, IRoutingStrategy
    {
        services.Add(new ServiceDescriptor(typeof(IRoutingStrategy), typeof(TStrategy), lifetime));
        services.Add(new ServiceDescriptor(typeof(TStrategy), typeof(TStrategy), lifetime));

        return services;
    }

    /// <summary>
    /// 配置数据访问选项
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">选项配置委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ConfigureDataAccess(
        this IServiceCollection services,
        Action<DataAccessOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}

/// <summary>
/// 数据访问选项配置
/// </summary>
public class DataAccessOptions
{
    /// <summary>
    /// 默认超时时间
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 默认重试次数
    /// </summary>
    public int DefaultRetryCount { get; set; } = 3;

    /// <summary>
    /// 默认批量操作大小
    /// </summary>
    public int DefaultBatchSize { get; set; } = 100;

    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// 健康检查间隔
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// 路由缓存过期时间
    /// </summary>
    public TimeSpan RouteCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 是否启用详细日志
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}