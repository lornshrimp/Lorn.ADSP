namespace Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;

/// <summary>
/// 定向匹配器配置选项
/// Targeting Matcher Configuration Options
/// </summary>
public class TargetingMatcherOptions
{
    /// <summary>
    /// 配置节名称
    /// Configuration section name
    /// </summary>
    public const string SectionName = "TargetingMatchers";

    /// <summary>
    /// 是否启用F#匹配器
    /// Whether to enable F# matchers
    /// </summary>
    public bool EnableFSharpMatchers { get; set; } = true;

    /// <summary>
    /// 匹配器配置字典
    /// Matcher configuration dictionary
    /// </summary>
    public Dictionary<string, MatcherConfiguration> Matchers { get; set; } = new();

    /// <summary>
    /// 默认服务生命周期
    /// Default service lifetime
    /// </summary>
    public string DefaultLifetime { get; set; } = "Scoped";

    /// <summary>
    /// 是否启用性能监控
    /// Whether to enable performance monitoring
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// 是否启用缓存
    /// Whether to enable caching
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// 默认超时时间（毫秒）
    /// Default timeout in milliseconds
    /// </summary>
    public int DefaultTimeoutMs { get; set; } = 5000;
}

/// <summary>
/// 单个匹配器的配置
/// Individual matcher configuration
/// </summary>
public class MatcherConfiguration
{
    /// <summary>
    /// 匹配器是否启用
    /// Whether the matcher is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 执行优先级（数值越小优先级越高）
    /// Execution priority (lower number = higher priority)
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// 服务生命周期
    /// Service lifetime
    /// </summary>
    public string Lifetime { get; set; } = "Scoped";

    /// <summary>
    /// 是否可以并行执行
    /// Whether can run in parallel
    /// </summary>
    public bool CanRunInParallel { get; set; } = true;

    /// <summary>
    /// 期望执行时间（毫秒）
    /// Expected execution time in milliseconds
    /// </summary>
    public int ExpectedExecutionTimeMs { get; set; } = 10;

    /// <summary>
    /// 超时时间（毫秒）
    /// Timeout in milliseconds
    /// </summary>
    public int TimeoutMs { get; set; } = 1000;

    /// <summary>
    /// 自定义配置参数
    /// Custom configuration parameters
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}
