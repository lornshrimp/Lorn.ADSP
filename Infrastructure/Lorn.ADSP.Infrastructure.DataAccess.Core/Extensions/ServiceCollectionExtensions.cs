using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Core.Configuration;
using Lorn.ADSP.Infrastructure.DataAccess.Core.Registry;
using Lorn.ADSP.Infrastructure.DataAccess.Core.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Extensions;

/// <summary>
/// 服务集合扩展方法
/// 提供数据访问核心功能的注册方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加数据访问核心服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置服务</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // 注册数据访问特定配置
        services.Configure<DataAccessConfiguration>(
            configuration.GetSection(DataAccessConfiguration.SectionName));

        // 注册核心服务
        services.AddSingleton<IDataProviderRegistry, InMemoryProviderRegistry>();
        services.AddSingleton<IDataAccessRouter, DataAccessRouter>();

        // 注册路由相关服务
        services.AddSingleton<RoutingRuleEngine>();
        services.AddSingleton<DefaultRoutingStrategy>();

        // 注册配置管理服务
        services.AddSingleton<DataProviderOptionsManager>();

        return services;
    }

    /// <summary>
    /// 添加数据提供者
    /// </summary>
    /// <typeparam name="TProvider">数据提供者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IDataAccessProvider
    {
        services.AddSingleton<IDataAccessProvider, TProvider>();
        return services;
    }

    /// <summary>
    /// 添加数据提供者（带配置）
    /// </summary>
    /// <typeparam name="TProvider">数据提供者类型</typeparam>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置服务</param>
    /// <param name="configurationSection">配置节名称</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataProvider<TProvider, TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSection)
        where TProvider : class, IDataAccessProvider
        where TOptions : class
    {
        services.AddSingleton<IDataAccessProvider, TProvider>();
        services.Configure<TOptions>(configuration.GetSection(configurationSection));

        return services;
    }

    /// <summary>
    /// 配置数据访问选项
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ConfigureDataAccess(
        this IServiceCollection services,
        Action<DataAccessConfiguration> configureOptions)
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        return services;
    }
}
