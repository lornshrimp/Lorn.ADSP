using Lorn.ADSP.Infrastructure.DataAccess.Repository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Lorn.ADSP.Infrastructure.DataAccess.Repository.Extensions;

/// <summary>
/// 服务集合扩展方法
/// 提供仓储模式相关的注册方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加仓储模式支持
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRepositoryPattern(this IServiceCollection services)
    {
        // 仓储模式的核心服务在具体实现中注册
        // 这里只提供基础的扩展点

        return services;
    }

    /// <summary>
    /// 添加仓储实现
    /// </summary>
    /// <typeparam name="TInterface">仓储接口类型</typeparam>
    /// <typeparam name="TImplementation">仓储实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRepository<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TInterface, TImplementation>();
        return services;
    }

    /// <summary>
    /// 添加工作单元实现
    /// </summary>
    /// <typeparam name="TImplementation">工作单元实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddUnitOfWork<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IUnitOfWork
    {
        services.AddScoped<IUnitOfWork, TImplementation>();
        return services;
    }

    /// <summary>
    /// 添加特定实体的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TRepository">仓储实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddEntityRepository<TEntity, TKey, TRepository>(this IServiceCollection services)
        where TEntity : class
        where TRepository : class, IRepository<TEntity, TKey>
    {
        services.AddScoped<IRepository<TEntity, TKey>, TRepository>();
        return services;
    }

    /// <summary>
    /// 添加特定实体的仓储（主键为string类型）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TRepository">仓储实现类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddEntityRepository<TEntity, TRepository>(this IServiceCollection services)
        where TEntity : class
        where TRepository : class, IRepository<TEntity>
    {
        services.AddScoped<IRepository<TEntity>, TRepository>();
        return services;
    }
}
