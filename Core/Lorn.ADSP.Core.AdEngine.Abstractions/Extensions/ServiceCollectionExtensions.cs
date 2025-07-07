using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

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

/// <summary>
/// �����������ѡ��
/// </summary>
public class AdEngineAbstractionsOptions
{
    /// <summary>
    /// �Ƿ�ע����Է���
    /// </summary>
    public bool RegisterStrategyServices { get; set; } = true;

    /// <summary>
    /// �Ƿ�ע��ص�����
    /// </summary>
    public bool RegisterCallbackServices { get; set; } = true;

    /// <summary>
    /// �Ƿ�ע���ط���
    /// </summary>
    public bool RegisterMonitoringServices { get; set; } = true;

    /// <summary>
    /// Ĭ�ϳ�ʱʱ��
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// ������Դ���
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// �Ƿ��������ܼ��
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// �Ƿ�������ϸ��־
    /// </summary>
    public bool EnableVerboseLogging { get; set; } = false;
}

/// <summary>
/// Ĭ�ϻص��ṩ��ʵ��
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
    /// ע�������ص�
    /// </summary>
    public void RegisterCallback(string name, IAdEngineCallback callback)
    {
        _namedCallbacks[name] = callback;
    }
}