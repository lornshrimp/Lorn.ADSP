using Microsoft.Extensions.DependencyInjection;
using Lorn.ADSP.Infrastructure.Common.Models;
using Lorn.ADSP.Infrastructure.Configuration;
using Lorn.ADSP.Infrastructure.DependencyInjection.Discovery;
using Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using Lorn.ADSP.Infrastructure.Common.Conventions;
using CommonServiceLifetime = Lorn.ADSP.Infrastructure.Common.Models.ServiceLifetime;
using MicrosoftServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Lorn.ADSP.Infrastructure.DependencyInjection;

/// <summary>
/// 组件注册管理器
/// 负责组件的自动发现和注册
/// </summary>
public class ComponentRegistrationManager
{
    private readonly IServiceCollection _services;
    private readonly AdSystemConfigurationManager _configManager;
    private readonly ConventionComponentDiscovery _componentDiscovery;
    private readonly AssemblyComponentScanner _scanner;
    private readonly HashSet<Type> _registeredTypes = new();

    public ComponentRegistrationManager(IServiceCollection services, AdSystemConfigurationManager configManager)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        _componentDiscovery = new ConventionComponentDiscovery();
        _scanner = new AssemblyComponentScanner();
    }

    /// <summary>
    /// 注册所有组件
    /// </summary>
    public void RegisterAllComponents()
    {
        // 配置扫描器
        ConfigureScanner();

        // 扫描组件类型
        var componentTypes = _scanner.ScanComponentTypes();

        // 发现和注册组件
        var descriptors = _componentDiscovery.DiscoverComponents(componentTypes);
        
        foreach (var descriptor in descriptors.Where(d => d.IsEnabled))
        {
            RegisterComponent(descriptor);
        }
    }

    /// <summary>
    /// 注册单个组件
    /// </summary>
    /// <param name="descriptor">组件描述符</param>
    public void RegisterComponent(ComponentDescriptor descriptor)
    {
        if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
        
        // 避免重复注册
        if (_registeredTypes.Contains(descriptor.ImplementationType))
            return;

        try
        {
            // 注册到 DI 容器
            RegisterServiceToContainer(descriptor);

            // 处理配置绑定
            ProcessConfigurationBinding(descriptor);

            // 记录已注册的类型
            _registeredTypes.Add(descriptor.ImplementationType);
        }
        catch (Exception ex)
        {
            // 记录注册失败的组件，但不影响其他组件的注册
            System.Diagnostics.Debug.WriteLine($"Failed to register component {descriptor.ImplementationType.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// 配置扫描器
    /// </summary>
    private void ConfigureScanner()
    {
        // 添加默认的程序集模式
        _scanner.AddAssemblyPattern("Lorn.ADSP.*");
        
        // 排除测试和基础设施程序集
        _scanner.ExcludeAssemblyPattern("*.Tests");
        _scanner.ExcludeAssemblyPattern("*.Test");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.Common");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.Configuration");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.DependencyInjection");
    }

    /// <summary>
    /// 注册服务到容器
    /// </summary>
    /// <param name="descriptor">组件描述符</param>
    private void RegisterServiceToContainer(ComponentDescriptor descriptor)
    {
        var serviceLifetime = ConvertToMicrosoftLifetime(descriptor.Lifetime);
        
        // 创建服务描述符
        var serviceDescriptor = new ServiceDescriptor(
            descriptor.ServiceType,
            descriptor.ImplementationType,
            serviceLifetime);

        _services.Add(serviceDescriptor);

        // 如果服务类型和实现类型不同，也注册实现类型本身
        if (descriptor.ServiceType != descriptor.ImplementationType)
        {
            var implementationDescriptor = new ServiceDescriptor(
                descriptor.ImplementationType,
                descriptor.ImplementationType,
                serviceLifetime);

            _services.Add(implementationDescriptor);
        }
    }

    /// <summary>
    /// 处理配置绑定
    /// </summary>
    /// <param name="descriptor">组件描述符</param>
    private void ProcessConfigurationBinding(ComponentDescriptor descriptor)
    {
        if (descriptor.ConfigurationType == null || string.IsNullOrEmpty(descriptor.ConfigurationPath))
            return;

        try
        {
            // 注册组件特定的配置选项
            _configManager.RegisterComponentOptions(
                descriptor.ImplementationType,
                descriptor.ConfigurationType,
                descriptor.ConfigurationPath);
        }
        catch (Exception ex)
        {
            // 配置绑定失败不影响组件注册
            System.Diagnostics.Debug.WriteLine($"Failed to bind configuration for {descriptor.ImplementationType.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// 转换生命周期枚举
    /// </summary>
    /// <param name="lifetime">自定义生命周期</param>
    /// <returns>微软DI生命周期</returns>
    private MicrosoftServiceLifetime ConvertToMicrosoftLifetime(CommonServiceLifetime lifetime)
    {
        return lifetime switch
        {
            CommonServiceLifetime.Singleton => MicrosoftServiceLifetime.Singleton,
            CommonServiceLifetime.Scoped => MicrosoftServiceLifetime.Scoped,
            CommonServiceLifetime.Transient => MicrosoftServiceLifetime.Transient,
            _ => MicrosoftServiceLifetime.Transient
        };
    }

    /// <summary>
    /// 获取已注册的组件类型
    /// </summary>
    /// <returns>已注册的类型集合</returns>
    public IReadOnlySet<Type> GetRegisteredTypes()
    {
        return _registeredTypes.ToHashSet();
    }

    /// <summary>
    /// 检查类型是否已注册
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否已注册</returns>
    public bool IsRegistered(Type type)
    {
        return _registeredTypes.Contains(type);
    }

    /// <summary>
    /// 手动注册组件类型
    /// </summary>
    /// <typeparam name="TImplementation">实现类型</typeparam>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="lifetime">生命周期</param>
    /// <param name="configurationPath">配置路径</param>
    public void RegisterComponent<TImplementation, TService>(
        CommonServiceLifetime lifetime = CommonServiceLifetime.Transient,
        string? configurationPath = null)
        where TImplementation : class, TService
        where TService : class
    {
        var descriptor = new ComponentDescriptor
        {
            ImplementationType = typeof(TImplementation),
            ServiceType = typeof(TService),
            Name = typeof(TImplementation).Name,
            Lifetime = lifetime,
            ConfigurationPath = configurationPath,
            IsEnabled = true,
            Priority = 0
        };

        // 尝试查找配置类型
        if (!string.IsNullOrEmpty(configurationPath))
        {
            var optionsTypeName = $"{typeof(TImplementation).Name}Options";
            descriptor.ConfigurationType = typeof(TImplementation).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == optionsTypeName);
        }

        RegisterComponent(descriptor);
    }

    /// <summary>
    /// 手动注册组件类型
    /// </summary>
    /// <typeparam name="TImplementation">实现类型</typeparam>
    /// <param name="lifetime">生命周期</param>
    /// <param name="configurationPath">配置路径</param>
    public void RegisterComponent<TImplementation>(
        CommonServiceLifetime lifetime = CommonServiceLifetime.Transient,
        string? configurationPath = null)
        where TImplementation : class
    {
        RegisterComponent<TImplementation, TImplementation>(lifetime, configurationPath);
    }
}