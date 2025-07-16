using Lorn.ADSP.Infrastructure.Common.Models;
using Lorn.ADSP.Infrastructure.Common.Conventions;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Discovery;

/// <summary>
/// 约定组件发现器
/// 基于约定规则发现和识别组件
/// </summary>
public class ConventionComponentDiscovery
{
    /// <summary>
    /// 发现组件描述符
    /// </summary>
    /// <param name="type">组件类型</param>
    /// <returns>组件描述符，如果不是组件则返回null</returns>
    public ComponentDescriptor? DiscoverComponent(Type type)
    {
        if (!IsValidComponentType(type))
            return null;

        var descriptor = new ComponentDescriptor
        {
            ImplementationType = type,
            ServiceType = DetermineServiceType(type),
            Name = type.Name,
            Lifetime = DetermineLifetime(type),
            ConfigurationPath = DetermineConfigurationPath(type),
            ConfigurationType = DetermineConfigurationType(type),
            IsEnabled = DetermineIsEnabled(type),
            Priority = DeterminePriority(type)
        };

        // 填充元数据
        PopulateMetadata(descriptor, type);

        return descriptor;
    }

    /// <summary>
    /// 批量发现组件描述符
    /// </summary>
    /// <param name="types">类型列表</param>
    /// <returns>组件描述符列表</returns>
    public IEnumerable<ComponentDescriptor> DiscoverComponents(IEnumerable<Type> types)
    {
        var descriptors = new List<ComponentDescriptor>();

        foreach (var type in types)
        {
            var descriptor = DiscoverComponent(type);
            if (descriptor != null)
            {
                descriptors.Add(descriptor);
            }
        }

        return descriptors;
    }

    /// <summary>
    /// 检查类型是否为有效的组件类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否有效</returns>
    private bool IsValidComponentType(Type type)
    {
        if (type == null || !type.IsConcreteClass())
            return false;

        // 检查是否有显式的组件标记
        if (type.GetCustomAttributeSafe<ComponentAttribute>() != null)
            return true;

        // 检查是否符合约定规则
        return ComponentConventions.IsComponent(type);
    }

    /// <summary>
    /// 确定服务类型
    /// </summary>
    /// <param name="type">实现类型</param>
    /// <returns>服务类型</returns>
    private Type DetermineServiceType(Type type)
    {
        // 优先查找对应的接口
        var interfaceName = $"I{type.Name}";
        var interfaceType = type.GetInterfaces()
            .FirstOrDefault(i => i.Name == interfaceName);

        if (interfaceType != null)
            return interfaceType;

        // 查找其他可能的服务接口
        var serviceInterfaces = type.GetInterfaces()
            .Where(i => !i.Name.StartsWith("System.") && 
                       !i.Name.StartsWith("Microsoft.") &&
                       i.IsPublic)
            .ToList();

        // 如果只有一个业务接口，使用它
        if (serviceInterfaces.Count == 1)
            return serviceInterfaces[0];

        // 否则使用实现类型本身
        return type;
    }

    /// <summary>
    /// 确定生命周期
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>生命周期</returns>
    private ServiceLifetime DetermineLifetime(Type type)
    {
        // 检查显式的生命周期特性
        var lifetimeAttribute = type.GetCustomAttributeSafe<ServiceLifetimeAttribute>();
        if (lifetimeAttribute != null)
            return lifetimeAttribute.Lifetime;

        // 使用约定规则
        return ComponentConventions.GetLifetimeByType(type);
    }

    /// <summary>
    /// 确定配置路径
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>配置路径</returns>
    private string? DetermineConfigurationPath(Type type)
    {
        // 检查显式的配置绑定特性
        var bindingAttribute = type.GetCustomAttributeSafe<ConfigurationBindingAttribute>();
        if (bindingAttribute != null)
            return bindingAttribute.ConfigurationPath;

        // 使用约定规则
        return ComponentConventions.GetConfigurationPath(type);
    }

    /// <summary>
    /// 确定配置类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>配置类型</returns>
    private Type? DetermineConfigurationType(Type type)
    {
        // 检查显式的配置绑定特性
        var bindingAttribute = type.GetCustomAttributeSafe<ConfigurationBindingAttribute>();
        if (bindingAttribute?.OptionsType != null)
            return bindingAttribute.OptionsType;

        // 根据约定查找配置类型
        var optionsTypeName = $"{type.Name}Options";
        var optionsType = type.Assembly.GetTypes()
            .FirstOrDefault(t => t.Name == optionsTypeName && t.IsClass);

        return optionsType;
    }

    /// <summary>
    /// 确定是否启用
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否启用</returns>
    private bool DetermineIsEnabled(Type type)
    {
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        return componentAttribute?.IsEnabled ?? true;
    }

    /// <summary>
    /// 确定优先级
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>优先级</returns>
    private int DeterminePriority(Type type)
    {
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        return componentAttribute?.Priority ?? 0;
    }

    /// <summary>
    /// 填充元数据
    /// </summary>
    /// <param name="descriptor">组件描述符</param>
    /// <param name="type">类型</param>
    private void PopulateMetadata(ComponentDescriptor descriptor, Type type)
    {
        // 添加基本元数据
        descriptor.Metadata["FullName"] = type.FullName ?? type.Name;
        descriptor.Metadata["AssemblyName"] = type.Assembly.GetName().Name ?? "";

        // 添加组件特性元数据
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        if (componentAttribute != null)
        {
            descriptor.Metadata["Description"] = componentAttribute.Description;
            descriptor.Metadata["Tags"] = componentAttribute.Tags;
        }

        // 添加依赖信息
        var dependencies = type.GetCustomAttributesSafe<DependencyAttribute>()
            .Select(d => new
            {
                ServiceType = d.ServiceType.Name,
                IsOptional = d.IsOptional,
                Description = d.Description
            })
            .ToList();

        if (dependencies.Any())
        {
            descriptor.Metadata["Dependencies"] = dependencies;
        }

        // 添加接口信息
        var interfaces = type.GetInterfaces()
            .Where(i => !i.Name.StartsWith("System.") && !i.Name.StartsWith("Microsoft."))
            .Select(i => i.Name)
            .ToList();

        if (interfaces.Any())
        {
            descriptor.Metadata["Interfaces"] = interfaces;
        }
    }
}