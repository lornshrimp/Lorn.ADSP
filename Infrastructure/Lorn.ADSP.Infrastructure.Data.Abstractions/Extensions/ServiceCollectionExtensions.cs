using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Extensions;

/// <summary>
/// ���ݷ��ʷ���ע����չ
/// �ṩͳһ�����ݷ��ʳ�������ע�᷽��
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ������ݷ��ʳ�������
    /// ע�⣺�˷�����ע��ӿڣ�����ʵ��Ӧ����Ӧ��ʵ����Ŀ��ע��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddDataAccessAbstractions(this IServiceCollection services)
    {
        // �����ֻע��ӿڣ���ע�����ʵ��
        // ����ʵ��Ӧ���ڶ�Ӧ��ʵ����Ŀ��ע�ᣬ���磺
        // - Infrastructure.Data ��Ŀע�� IDataProviderRegistry��IDataAccessRouter ��ʵ��
        // - Infrastructure.Data ��Ŀע�� IDataAccessManager��ITransactionManager ��ʵ��
        
        // �������ע��һЩ������صķ���
        services.Configure<DataAccessOptions>(_ => { });
        
        return services;
    }

    /// <summary>
    /// ע�����ݷ����ṩ��ע���ʵ��
    /// �˷���Ӧ�ھ���ʵ����Ŀ�е���
    /// </summary>
    /// <typeparam name="TRegistry">ע���ʵ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddDataProviderRegistry<TRegistry>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TRegistry : class, IDataProviderRegistry
    {
        services.Add(new ServiceDescriptor(typeof(IDataProviderRegistry), typeof(TRegistry), lifetime));
        return services;
    }

    /// <summary>
    /// ע�����ݷ���·����ʵ��
    /// �˷���Ӧ�ھ���ʵ����Ŀ�е���
    /// </summary>
    /// <typeparam name="TRouter">·����ʵ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddDataAccessRouter<TRouter>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TRouter : class, IDataAccessRouter
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessRouter), typeof(TRouter), lifetime));
        return services;
    }

    /// <summary>
    /// ע�����ݷ��ʹ�����ʵ��
    /// �˷���Ӧ�ھ���ʵ����Ŀ�е���
    /// </summary>
    /// <typeparam name="TManager">������ʵ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddDataAccessManager<TManager>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TManager : class, IDataAccessManager
    {
        services.Add(new ServiceDescriptor(typeof(IDataAccessManager), typeof(TManager), lifetime));
        return services;
    }

    /// <summary>
    /// ע�����������ʵ��
    /// �˷���Ӧ�ھ���ʵ����Ŀ�е���
    /// </summary>
    /// <typeparam name="TTransactionManager">���������ʵ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddTransactionManager<TTransactionManager>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TTransactionManager : class, ITransactionManager
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionManager), typeof(TTransactionManager), lifetime));
        return services;
    }

    /// <summary>
    /// ע������Э����ʵ��
    /// �˷���Ӧ�ھ���ʵ����Ŀ�е���
    /// </summary>
    /// <typeparam name="TCoordinator">Э����ʵ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddTransactionCoordinator<TCoordinator>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCoordinator : class, ITransactionCoordinator
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionCoordinator), typeof(TCoordinator), lifetime));
        return services;
    }

    /// <summary>
    /// ע�����ݷ����ṩ��
    /// </summary>
    /// <typeparam name="TProvider">�ṩ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// ע�����ݷ����ṩ�ߣ�ʹ�ù���������
    /// </summary>
    /// <typeparam name="TProvider">�ṩ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="factory">�ṩ�߹�������</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// ע�������ṩ��
    /// </summary>
    /// <typeparam name="TProvider">�����ṩ������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// ����ע�����ݷ����ṩ��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="providerTypes">�ṩ�����ͼ���</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// �Զ�ɨ�貢ע�����ݷ����ṩ��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="assemblies">Ҫɨ��ĳ���</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// ע��·�ɲ���
    /// </summary>
    /// <typeparam name="TStrategy">��������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// �������ݷ���ѡ��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="configureOptions">ѡ������ί��</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection ConfigureDataAccess(
        this IServiceCollection services,
        Action<DataAccessOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}

/// <summary>
/// ���ݷ���ѡ������
/// </summary>
public class DataAccessOptions
{
    /// <summary>
    /// Ĭ�ϳ�ʱʱ��
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Ĭ�����Դ���
    /// </summary>
    public int DefaultRetryCount { get; set; } = 3;

    /// <summary>
    /// Ĭ������������С
    /// </summary>
    public int DefaultBatchSize { get; set; } = 100;

    /// <summary>
    /// �Ƿ��������ܼ��
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// �Ƿ����ý������
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// ���������
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// ·�ɻ������ʱ��
    /// </summary>
    public TimeSpan RouteCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// �Ƿ�������ϸ��־
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}