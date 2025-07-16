namespace Lorn.ADSP.Infrastructure.Common.Models;

/// <summary>
/// 组件元数据，用于描述组件的附加信息
/// </summary>
public class ComponentMetadata
{
    /// <summary>
    /// 元数据键值对
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// 组件依赖的接口类型列表
    /// </summary>
    public List<Type> Dependencies { get; set; } = new();
    
    /// <summary>
    /// 组件提供的服务接口列表
    /// </summary>
    public List<Type> ProvidedServices { get; set; } = new();
    
    /// <summary>
    /// 组件标签
    /// </summary>
    public HashSet<string> Tags { get; set; } = new();
    
    /// <summary>
    /// 组件版本
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// 组件描述
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// 组件作者
    /// </summary>
    public string Author { get; set; } = "";
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}