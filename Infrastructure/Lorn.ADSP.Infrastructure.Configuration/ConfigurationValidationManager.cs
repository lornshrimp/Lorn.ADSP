using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.Reflection;

namespace Lorn.ADSP.Infrastructure.Configuration;

/// <summary>
/// 配置验证管理器
/// 提供统一的配置验证功能
/// </summary>
public class ConfigurationValidationManager
{
    private readonly IServiceCollection _services;
    private readonly HashSet<Type> _registeredValidators = new();

    public ConfigurationValidationManager(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// 自动扫描和注册所有验证器
    /// </summary>
    public void RegisterAllValidators()
    {
        var validatorTypes = FindValidatorTypes();
        
        foreach (var validatorType in validatorTypes)
        {
            RegisterValidatorType(validatorType);
        }
    }

    /// <summary>
    /// 为特定配置类型注册验证器
    /// </summary>
    /// <param name="optionsType">配置选项类型</param>
    public void RegisterValidatorsForOptionsType(Type optionsType)
    {
        if (optionsType == null) throw new ArgumentNullException(nameof(optionsType));

        var validatorTypes = FindValidatorTypesForOptionsType(optionsType);
        
        foreach (var validatorType in validatorTypes)
        {
            RegisterValidatorType(validatorType);
        }
    }

    /// <summary>
    /// 核心验证器注册逻辑 - 避免重复代码
    /// </summary>
    /// <param name="validatorType">验证器类型</param>
    private void RegisterValidatorType(Type validatorType)
    {
        if (_registeredValidators.Contains(validatorType))
        {
            return;
        }

        var validateInterface = validatorType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidateOptions<>));

        if (validateInterface != null)
        {
            var optionsType = validateInterface.GetGenericArguments()[0];
            var serviceType = typeof(IValidateOptions<>).MakeGenericType(optionsType);

            _services.AddSingleton(serviceType, validatorType);
            _registeredValidators.Add(validatorType);
        }
    }

    /// <summary>
    /// 查找所有验证器类型
    /// </summary>
    /// <returns>验证器类型列表</returns>
    private IEnumerable<Type> FindValidatorTypes()
    {
        var assemblies = GetRelevantAssemblies();
        var validatorTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(type => type.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidateOptions<>)));
                validatorTypes.AddRange(types);
            }
            catch (Exception)
            {
                // 忽略程序集加载错误
            }
        }

        return validatorTypes;
    }

    /// <summary>
    /// 查找特定配置类型的验证器
    /// </summary>
    /// <param name="optionsType">配置选项类型</param>
    /// <returns>验证器类型列表</returns>
    private IEnumerable<Type> FindValidatorTypesForOptionsType(Type optionsType)
    {
        var assemblies = GetRelevantAssemblies();
        var validatorTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(type => type.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidateOptions<>) &&
                        i.GetGenericArguments()[0] == optionsType));
                validatorTypes.AddRange(types);
            }
            catch (Exception)
            {
                // 忽略程序集加载错误
            }
        }

        return validatorTypes;
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
}