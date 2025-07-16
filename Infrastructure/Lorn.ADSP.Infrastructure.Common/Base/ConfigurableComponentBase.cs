using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.Common.Base;

/// <summary>
/// 可配置组件基类
/// 提供配置管理的基础实现
/// </summary>
public abstract class ConfigurableComponentBase : IConfigurable
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public abstract Type ConfigurationType { get; }
    
    /// <summary>
    /// 配置组件
    /// </summary>
    /// <param name="configuration">配置对象</param>
    public virtual void Configure(object configuration)
    {
        if (configuration?.GetType() != ConfigurationType)
        {
            throw new ArgumentException($"Invalid configuration type. Expected: {ConfigurationType.Name}, Actual: {configuration?.GetType().Name}");
        }
        
        OnConfigurationChanged(configuration);
    }
    
    /// <summary>
    /// 配置变更时的处理逻辑，由子类实现
    /// </summary>
    /// <param name="configuration">新的配置对象</param>
    protected abstract void OnConfigurationChanged(object configuration);
}