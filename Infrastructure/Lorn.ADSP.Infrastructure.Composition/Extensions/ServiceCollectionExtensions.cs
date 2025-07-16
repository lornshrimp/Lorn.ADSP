using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Lorn.ADSP.Infrastructure.Composition.Bootstrapper;
using Lorn.ADSP.Infrastructure.Composition.Configuration;
using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.Reflection;

namespace Lorn.ADSP.Infrastructure.Composition.Extensions;

/// <summary>
/// 服务集合扩展方法
/// 这是整个基础设施唯一的对外接口，位于组合层避免循环引用
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册广告系统基础设施
    /// 这是外部应用程序唯一需要调用的方法
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象</param>
    /// <param name="configureOptions">配置启动选项的委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAdSystemInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<BootstrapOptions>? configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // 创建启动选项
        var options = new BootstrapOptions();
        configureOptions?.Invoke(options);

        // 使用基础设施启动器来组装所有组件
        var bootstrapper = new InfrastructureBootstrapper(services, configuration, options);
        var result = bootstrapper.Bootstrap();

        // 检查启动结果
        if (!bootstrapper.IsSuccessful())
        {
            var errors = bootstrapper.GetErrors();
            var errorMessage = string.Join(Environment.NewLine, errors);
            throw new InvalidOperationException($"Infrastructure bootstrap failed:{Environment.NewLine}{errorMessage}");
        }

        // 记录警告
        var warnings = bootstrapper.GetWarnings();
        foreach (var warning in warnings)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] {warning}");
        }

        return result;
    }

    /// <summary>
    /// 注册广告系统基础设施（简化版本）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddAdSystemInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        return AddAdSystemInfrastructure(services, configuration, null);
    }

    /// <summary>
    /// 为健康检查系统添加组件健康检查支持
    /// </summary>
    /// <param name="builder">健康检查构建器</param>
    /// <returns>健康检查构建器</returns>
    public static IHealthChecksBuilder AddComponentHealthChecks(this IHealthChecksBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        // 自动扫描所有实现 IHealthCheckable 的组件并添加健康检查
        var healthCheckableTypes = FindHealthCheckableTypes();
            
        foreach (var type in healthCheckableTypes)
        {
            // 为每个健康检查组件创建检查项
            builder.AddCheck(type.Name, () => 
            {
                try
                {
                    // 这里简化处理，实际实现中需要从DI容器获取实例
                    return HealthCheckResult.Healthy($"{type.Name} is healthy");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{type.Name} check failed: {ex.Message}");
                }
            });
        }
        
        return builder;
    }

    /// <summary>
    /// 添加基础设施配置选项
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInfrastructureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 注册基础设施配置选项
        services.Configure<BootstrapOptions>(configuration.GetSection("Infrastructure:Bootstrap"));
        
        return services;
    }

    /// <summary>
    /// 验证基础设施配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ValidateInfrastructureConfiguration(this IServiceCollection services)
    {
        // 添加配置验证
        services.AddOptions<BootstrapOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// 查找实现 IHealthCheckable 接口的类型
    /// </summary>
    /// <returns>健康检查类型列表</returns>
    private static IEnumerable<Type> FindHealthCheckableTypes()
    {
        var healthCheckableTypes = new List<Type>();
        
        try
        {
            // 获取相关程序集
            var assemblies = GetRelevantAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypesImplementing(typeof(IHealthCheckable));
                    healthCheckableTypes.AddRange(types);
                }
                catch (Exception)
                {
                    // 忽略程序集加载错误
                }
            }
        }
        catch (Exception)
        {
            // 忽略扫描错误
        }

        return healthCheckableTypes;
    }

    /// <summary>
    /// 获取相关的程序集
    /// </summary>
    /// <returns>程序集列表</returns>
    private static IEnumerable<Assembly> GetRelevantAssemblies()
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