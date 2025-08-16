using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;
using Lorn.ADSP.Infrastructure.DependencyInjection.Validation;
using Lorn.ADSP.Infrastructure.DependencyInjection.Services;
using Lorn.ADSP.Infrastructure.DependencyInjection.HealthChecks;
using System.Reflection;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Extensions;

/// <summary>
/// F#定向匹配器服务注册扩展方法
/// F# Targeting Matcher Service Registration Extensions
/// </summary>
public static class TargetingMatcherServiceExtensions
{
    /// <summary>
    /// 注册F#定向匹配器服务
    /// Register F# targeting matcher services
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <param name="configuration">配置对象 / Configuration object</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddFSharpTargetingMatchers(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置选项绑定
        // Configure options binding
        services.Configure<TargetingMatcherOptions>(
            configuration.GetSection(TargetingMatcherOptions.SectionName));

        // 注册配置验证
        // Register configuration validation
        services.AddSingleton<IValidateOptions<TargetingMatcherOptions>, TargetingMatcherOptionsValidator>();

        // 获取配置选项
        // Get configuration options
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<TargetingMatcherOptions>>()?.Value
                     ?? new TargetingMatcherOptions();

        // 如果禁用F#匹配器，则跳过注册
        // Skip registration if F# matchers are disabled
        if (!options.EnableFSharpMatchers)
        {
            return services;
        }

        // 注册具体的F#匹配器实现
        // Register concrete F# matcher implementations
        RegisterFSharpMatchers(services, options);

        // 注册匹配器管理器
        // Register matcher manager
        services.AddScoped<ITargetingMatcherManager, TargetingMatcherManager>();

        return services;
    }

    /// <summary>
    /// 注册具体的F#匹配器
    /// Register concrete F# matchers
    /// </summary>
    private static void RegisterFSharpMatchers(IServiceCollection services, TargetingMatcherOptions options)
    {
        // 获取F#匹配器程序集
        // Get F# matcher assembly
        var fsharpAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Lorn.ADSP.Strategies.Targeting");

        if (fsharpAssembly == null)
        {
            // 如果程序集未加载，尝试通过反射加载
            // If assembly is not loaded, try to load it via reflection
            try
            {
                fsharpAssembly = Assembly.LoadFrom("Lorn.ADSP.Strategies.Targeting.dll");
            }
            catch
            {
                // 程序集加载失败，跳过F#匹配器注册
                // Assembly loading failed, skip F# matcher registration
                return;
            }
        }

        // 定义F#匹配器类型映射
        // Define F# matcher type mappings
        var matcherTypeNames = new Dictionary<string, string>
        {
            { "demographic", "Lorn.ADSP.Strategies.Targeting.Matchers.DemographicTargetingMatcher" },
            { "geolocation", "Lorn.ADSP.Strategies.Targeting.Matchers.GeoLocationTargetingMatcher" },
            { "time", "Lorn.ADSP.Strategies.Targeting.Matchers.TimeTargetingMatcher" },
            { "device", "Lorn.ADSP.Strategies.Targeting.Matchers.DeviceTargetingMatcher" },
            { "user-interest", "Lorn.ADSP.Strategies.Targeting.Matchers.UserInterestTargetingMatcher" },
            { "user-behavior", "Lorn.ADSP.Strategies.Targeting.Matchers.UserBehaviorTargetingMatcher" },
            { "user-preference", "Lorn.ADSP.Strategies.Targeting.Matchers.UserPreferenceTargetingMatcher" },
            { "user-tag", "Lorn.ADSP.Strategies.Targeting.Matchers.UserTagTargetingMatcher" },
            { "user-value", "Lorn.ADSP.Strategies.Targeting.Matchers.UserValueTargetingMatcher" }
        };

        // 遍历每种匹配器类型进行注册
        // Iterate through each matcher type for registration
        foreach (var (key, typeName) in matcherTypeNames)
        {
            var config = options.Matchers.GetValueOrDefault(key, new MatcherConfiguration());

            // 检查是否启用该匹配器
            // Check if the matcher is enabled
            if (!config.Enabled)
            {
                continue;
            }

            // 通过反射获取类型
            // Get type through reflection
            var matcherType = fsharpAssembly.GetType(typeName);
            if (matcherType == null)
            {
                continue;
            }

            // 根据配置的生命周期注册服务
            // Register service according to configured lifetime
            var lifetime = ParseServiceLifetime(config.Lifetime);

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(ITargetingMatcher), matcherType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(ITargetingMatcher), matcherType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(ITargetingMatcher), matcherType);
                    break;
            }
        }
    }

    /// <summary>
    /// 解析服务生命周期
    /// Parse service lifetime
    /// </summary>
    private static ServiceLifetime ParseServiceLifetime(string lifetime)
    {
        return lifetime.ToLowerInvariant() switch
        {
            "singleton" => ServiceLifetime.Singleton,
            "scoped" => ServiceLifetime.Scoped,
            "transient" => ServiceLifetime.Transient,
            _ => ServiceLifetime.Scoped
        };
    }

    /// <summary>
    /// 添加F#匹配器的健康检查
    /// Add health checks for F# matchers
    /// </summary>
    /// <param name="services">服务集合 / Service collection</param>
    /// <returns>服务集合 / Service collection</returns>
    public static IServiceCollection AddFSharpMatcherHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<FSharpMatcherHealthCheck>("fsharp-matchers");

        return services;
    }
}
