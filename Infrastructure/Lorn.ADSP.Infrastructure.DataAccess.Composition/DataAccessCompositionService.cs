using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Lorn.ADSP.Infrastructure.DataAccess.Core.Extensions;
using Lorn.ADSP.Infrastructure.DataAccess.Repository.Extensions;

namespace Lorn.ADSP.Infrastructure.DataAccess.Composition.Extensions;

/// <summary>
/// 数据访问组合服务扩展
/// 提供数据访问层所有组件的统一注册入口
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加完整的数据访问服务
    /// 包含核心服务、Repository模式支持等
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // 添加数据访问核心服务
        services.AddDataAccessCore(configuration);

        // 添加Repository模式支持
        services.AddRepositoryPattern();

        return services;
    }
}
