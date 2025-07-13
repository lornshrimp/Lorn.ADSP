using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Extensions;

/// <summary>
/// ���񼯺���չ����
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ��ӹ�������������
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="configure">����ί��</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddAdEngineAbstractions(
        this IServiceCollection services,
        Action<AdEngineAbstractionsOptions>? configure = null)
    {
        var options = new AdEngineAbstractionsOptions();
        configure?.Invoke(options);

        // ע����ĳ���ӿ�
        services.TryAddSingleton<ICallbackProvider, DefaultCallbackProvider>();

        // ע�������ط���
        if (options.RegisterStrategyServices)
        {
            services.AddStrategyServices();
        }

        // ע��ص�����
        if (options.RegisterCallbackServices)
        {
            services.AddCallbackServices();
        }

        // ע���ط���
        if (options.RegisterMonitoringServices)
        {
            services.AddMonitoringServices();
        }

        return services;
    }

    /// <summary>
    /// ��Ӳ��Է���
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddStrategyServices(this IServiceCollection services)
    {
        // ���Է����ھ���ʵ����Ŀ��ע��
        return services;
    }

    /// <summary>
    /// ��ӻص�����
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddCallbackServices(this IServiceCollection services)
    {
        // �ص������ھ���ʵ����Ŀ��ע��
        return services;
    }

    /// <summary>
    /// ��Ӽ�ط���
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
    {
        // ��ط����ھ���ʵ����Ŀ��ע��
        return services;
    }

    /// <summary>
    /// ע���洦�����
    /// </summary>
    /// <typeparam name="TStrategy">��������</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
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
    /// ע��������ص�
    /// </summary>
    /// <typeparam name="TCallback">�ص�����</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddAdEngineCallback<TCallback>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCallback : class, IAdEngineCallback
    {
        services.Add(new ServiceDescriptor(typeof(TCallback), typeof(TCallback), lifetime));
        return services;
    }

    /// <summary>
    /// ע�������Ĺ������ص�
    /// </summary>
    /// <typeparam name="TCallback">�ص�����</typeparam>
    /// <param name="services">���񼯺�</param>
    /// <param name="name">�ص�����</param>
    /// <param name="lifetime">������������</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddNamedAdEngineCallback<TCallback>(
        this IServiceCollection services,
        string name,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TCallback : class, IAdEngineCallback
    {
        services.Add(new ServiceDescriptor(typeof(TCallback), typeof(TCallback), lifetime));
        // ���������ע�Ὣ��CallbackProvider�д���
        return services;
    }
}



