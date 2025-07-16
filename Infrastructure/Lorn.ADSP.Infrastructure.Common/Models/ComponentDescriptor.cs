namespace Lorn.ADSP.Infrastructure.Common.Models;

/// <summary>
/// 组件描述符，包含组件的元数据信息
/// </summary>
public class ComponentDescriptor
{
    /// <summary>
    /// 组件实现类型
    /// </summary>
    public Type ImplementationType { get; set; } = null!;
    
    /// <summary>
    /// 组件服务类型（通常是接口类型）
    /// </summary>
    public Type ServiceType { get; set; } = null!;
    
    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// 组件生命周期
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    
    /// <summary>
    /// 配置路径
    /// </summary>
    public string? ConfigurationPath { get; set; }
    
    /// <summary>
    /// 配置类型
    /// </summary>
    public Type? ConfigurationType { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// 扩展元数据
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 服务生命周期枚举
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// 瞬态：每次请求都创建新实例
    /// </summary>
    Transient,
    
    /// <summary>
    /// 作用域：在同一作用域内使用同一实例
    /// </summary>
    Scoped,
    
    /// <summary>
    /// 单例：整个应用程序生命周期内使用同一实例
    /// </summary>
    Singleton
}