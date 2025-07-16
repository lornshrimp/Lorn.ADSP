namespace Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// 组件标记特性
/// 用于显式标记需要自动注册的组件
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ComponentAttribute : Attribute
{
    /// <summary>
    /// 组件优先级
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 自定义配置节路径
    /// </summary>
    public string? ConfigurationSection { get; set; }

    /// <summary>
    /// 组件标签
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 组件描述
    /// </summary>
    public string Description { get; set; } = "";
}