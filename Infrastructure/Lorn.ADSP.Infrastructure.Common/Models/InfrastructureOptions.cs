namespace Lorn.ADSP.Infrastructure.Common.Models;

/// <summary>
/// 基础设施配置选项
/// </summary>
public class InfrastructureOptions
{
    /// <summary>
    /// 是否启用自动组件发现
    /// </summary>
    public bool EnableAutoDiscovery { get; set; } = true;
    
    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;
    
    /// <summary>
    /// 是否启用性能监控
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;
    
    /// <summary>
    /// 是否启用配置热重载
    /// </summary>
    public bool EnableConfigurationHotReload { get; set; } = true;
    
    /// <summary>
    /// 组件扫描的程序集名称模式
    /// </summary>
    public List<string> AssemblyPatterns { get; set; } = new() { "Lorn.ADSP.*" };
    
    /// <summary>
    /// 排除的程序集名称模式
    /// </summary>
    public List<string> ExcludedAssemblyPatterns { get; set; } = new();
    
    /// <summary>
    /// 配置验证超时时间（秒）
    /// </summary>
    public int ConfigurationValidationTimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// 组件初始化超时时间（秒）
    /// </summary>
    public int ComponentInitializationTimeoutSeconds { get; set; } = 60;
}