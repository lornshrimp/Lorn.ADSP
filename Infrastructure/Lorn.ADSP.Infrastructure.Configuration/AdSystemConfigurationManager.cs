using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.Reflection;

namespace Lorn.ADSP.Infrastructure.Configuration;

/// <summary>
/// 广告系统配置管理器
/// 提供统一的配置管理和自动绑定功能
/// </summary>
public class AdSystemConfigurationManager
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly ConfigurationValidationManager _validationManager;
    private readonly HashSet<Type> _registeredOptionsTypes = new();
    private readonly Dictionary<string, Type> _namedOptionsRegistry = new();

    public AdSystemConfigurationManager(IConfiguration configuration, IServiceCollection services)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _validationManager = new ConfigurationValidationManager(services);
    }

    /// <summary>
    /// 自动注册所有 *Options 类型的配置
    /// </summary>
    public void RegisterAllOptions()
    {
        var optionsTypes = FindOptionsTypes();
        
        foreach (var optionsType in optionsTypes)
        {
            RegisterConventionOptions(optionsType);
        }
    }

    /// <summary>
    /// 为组件注册特定配置（支持命名选项）
    /// </summary>
    /// <param name="componentType">组件类型</param>
    /// <param name="optionsType">配置选项类型</param>
    /// <param name="configurationPath">配置路径</param>
    public void RegisterComponentOptions(Type componentType, Type optionsType, string configurationPath)
    {
        if (componentType == null) throw new ArgumentNullException(nameof(componentType));
        if (optionsType == null) throw new ArgumentNullException(nameof(optionsType));
        if (string.IsNullOrEmpty(configurationPath)) throw new ArgumentException("Configuration path cannot be null or empty", nameof(configurationPath));

        var componentName = componentType.Name;
        var registryKey = $"{optionsType.FullName}:{componentName}";

        // 避免重复注册相同的命名配置
        if (_namedOptionsRegistry.ContainsKey(registryKey))
        {
            return;
        }

        // 查找支持命名选项的 Configure 方法
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == "Configure" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 3 &&
                m.GetParameters()[1].ParameterType == typeof(string) &&
                m.GetParameters()[2].ParameterType == typeof(IConfiguration));

        if (configureMethod != null)
        {
            var genericMethod = configureMethod.MakeGenericMethod(optionsType);
            var configSection = _configuration.GetSection(configurationPath);

            // 使用组件名作为命名选项的名称，支持多组件共享配置选项类型
            genericMethod.Invoke(null, new object[]
            {
                _services,
                componentName, // 组件名作为命名选项名称
                configSection
            });

            // 记录已注册的命名配置
            _namedOptionsRegistry[registryKey] = optionsType;
        }

        // 使用统一的验证管理器
        _validationManager.RegisterValidatorsForOptionsType(optionsType);
    }

    /// <summary>
    /// 根据约定自动绑定配置（仅处理传统的无命名配置）
    /// </summary>
    /// <param name="optionsType">配置选项类型</param>
    private void RegisterConventionOptions(Type optionsType)
    {
        // 避免重复注册
        if (_registeredOptionsTypes.Contains(optionsType))
        {
            return;
        }

        // 约定：AdEngineOptions -> "AdEngine" 配置节
        var sectionName = GetConfigurationSectionName(optionsType);

        // 使用 Microsoft.Extensions.Options 进行强类型绑定
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethods()
            .FirstOrDefault(m => 
                m.Name == "Configure" && 
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == typeof(IServiceCollection) &&
                m.GetParameters()[1].ParameterType == typeof(IConfiguration));

        if (configureMethod != null)
        {
            var genericMethod = configureMethod.MakeGenericMethod(optionsType);
            genericMethod.Invoke(null, new object[]
            {
                _services,
                _configuration.GetSection(sectionName)
            });
        }

        // 记录已注册的配置类型
        _registeredOptionsTypes.Add(optionsType);

        // 使用统一的验证管理器
        _validationManager.RegisterValidatorsForOptionsType(optionsType);
    }

    /// <summary>
    /// 查找所有 Options 类型
    /// </summary>
    /// <returns>Options 类型列表</returns>
    private IEnumerable<Type> FindOptionsTypes()
    {
        var assemblies = GetRelevantAssemblies();
        var optionsTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(t => t.Name.EndsWith("Options") && t.IsClass && !t.IsAbstract);
                optionsTypes.AddRange(types);
            }
            catch (Exception)
            {
                // 忽略程序集加载错误
            }
        }

        return optionsTypes;
    }

    /// <summary>
    /// 获取相关的程序集
    /// </summary>
    /// <returns>程序集列表</returns>
    private IEnumerable<Assembly> GetRelevantAssemblies()
    {
        var assemblies = new List<Assembly>();
        
        // 添加当前程序集
        assemblies.Add(Assembly.GetExecutingAssembly());
        
        // 添加调用程序集
        var callingAssembly = Assembly.GetCallingAssembly();
        if (callingAssembly != null)
        {
            assemblies.Add(callingAssembly);
        }
        
        // 添加入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            assemblies.Add(entryAssembly);
        }

        // 添加已加载的相关程序集
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.StartsWith("Lorn.ADSP") == true);
        assemblies.AddRange(loadedAssemblies);

        return assemblies.Distinct();
    }

    /// <summary>
    /// 根据配置类型名称获取配置节名称
    /// </summary>
    /// <param name="optionsType">配置类型</param>
    /// <returns>配置节名称</returns>
    private string GetConfigurationSectionName(Type optionsType)
    {
        var typeName = optionsType.Name;
        
        if (typeName.EndsWith("Options"))
        {
            return typeName.Substring(0, typeName.Length - "Options".Length);
        }
        
        if (typeName.EndsWith("Settings"))
        {
            return typeName.Substring(0, typeName.Length - "Settings".Length);
        }
        
        if (typeName.EndsWith("Configuration"))
        {
            return typeName.Substring(0, typeName.Length - "Configuration".Length);
        }
        
        return typeName;
    }
}