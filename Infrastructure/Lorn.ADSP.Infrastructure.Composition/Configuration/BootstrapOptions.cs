using Lorn.ADSP.Infrastructure.Common.Models;

namespace Lorn.ADSP.Infrastructure.Composition.Configuration;

/// <summary>
/// 基础设施启动选项
/// </summary>
public class BootstrapOptions
{
    /// <summary>
    /// 是否启用自动组件发现
    /// </summary>
    public bool EnableAutoComponentDiscovery { get; set; } = true;

    /// <summary>
    /// 是否启用自动配置绑定
    /// </summary>
    public bool EnableAutoConfigurationBinding { get; set; } = true;

    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// 是否启用配置验证
    /// </summary>
    public bool EnableConfigurationValidation { get; set; } = true;

    /// <summary>
    /// 程序集扫描模式
    /// </summary>
    public AssemblyScanMode AssemblyScanMode { get; set; } = AssemblyScanMode.LoadedAssemblies;

    /// <summary>
    /// 要扫描的程序集名称模式
    /// </summary>
    public List<string> AssemblyIncludePatterns { get; set; } = new() { "Lorn.ADSP.*" };

    /// <summary>
    /// 要排除的程序集名称模式
    /// </summary>
    public List<string> AssemblyExcludePatterns { get; set; } = new() { "*.Tests", "*.Test" };

    /// <summary>
    /// 组件注册超时时间（秒）
    /// </summary>
    public int ComponentRegistrationTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 配置验证超时时间（秒）
    /// </summary>
    public int ConfigurationValidationTimeoutSeconds { get; set; } = 15;

    /// <summary>
    /// 是否在启动时验证所有组件
    /// </summary>
    public bool ValidateComponentsOnStartup { get; set; } = true;

    /// <summary>
    /// 失败容忍模式
    /// </summary>
    public FailureToleranceMode FailureToleranceMode { get; set; } = FailureToleranceMode.ContinueOnError;
}

/// <summary>
/// 程序集扫描模式
/// </summary>
public enum AssemblyScanMode
{
    /// <summary>
    /// 仅扫描已加载的程序集
    /// </summary>
    LoadedAssemblies,

    /// <summary>
    /// 扫描应用程序目录下的所有程序集
    /// </summary>
    ApplicationDirectory,

    /// <summary>
    /// 手动指定程序集
    /// </summary>
    Manual
}

/// <summary>
/// 失败容忍模式
/// </summary>
public enum FailureToleranceMode
{
    /// <summary>
    /// 遇到错误时停止启动
    /// </summary>
    FailFast,

    /// <summary>
    /// 遇到错误时继续，但记录错误
    /// </summary>
    ContinueOnError,

    /// <summary>
    /// 忽略所有错误
    /// </summary>
    IgnoreErrors
}