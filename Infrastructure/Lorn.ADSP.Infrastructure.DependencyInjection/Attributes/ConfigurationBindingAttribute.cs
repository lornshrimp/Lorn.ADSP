namespace Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// 配置绑定特性
/// 用于显式指定组件的配置绑定信息
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConfigurationBindingAttribute : Attribute
{
    /// <summary>
    /// 配置路径
    /// </summary>
    public string ConfigurationPath { get; }

    /// <summary>
    /// 配置选项类型
    /// </summary>
    public Type? OptionsType { get; set; }

    /// <summary>
    /// 是否使用命名选项
    /// </summary>
    public bool UseNamedOptions { get; set; } = false;

    /// <summary>
    /// 命名选项的名称
    /// </summary>
    public string? OptionsName { get; set; }

    /// <summary>
    /// 初始化配置绑定特性
    /// </summary>
    /// <param name="configurationPath">配置路径</param>
    public ConfigurationBindingAttribute(string configurationPath)
    {
        ConfigurationPath = configurationPath ?? throw new ArgumentNullException(nameof(configurationPath));
    }
}