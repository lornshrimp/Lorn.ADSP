using Lorn.ADSP.Infrastructure.Common.Models;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// 服务生命周期特性
/// 用于显式指定组件的生命周期
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    /// <summary>
    /// 服务生命周期
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 初始化服务生命周期特性
    /// </summary>
    /// <param name="lifetime">服务生命周期</param>
    public ServiceLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}

/// <summary>
/// 单例服务特性
/// </summary>
public class SingletonAttribute : ServiceLifetimeAttribute
{
    public SingletonAttribute() : base(ServiceLifetime.Singleton) { }
}

/// <summary>
/// 作用域服务特性
/// </summary>
public class ScopedAttribute : ServiceLifetimeAttribute
{
    public ScopedAttribute() : base(ServiceLifetime.Scoped) { }
}

/// <summary>
/// 瞬态服务特性
/// </summary>
public class TransientAttribute : ServiceLifetimeAttribute
{
    public TransientAttribute() : base(ServiceLifetime.Transient) { }
}