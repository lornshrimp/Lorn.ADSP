namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 定向匹配器元数据
/// 包含匹配器的描述信息、支持的功能、配置要求等
/// </summary>
public record TargetingMatcherMetadata
{
    /// <summary>
    /// 匹配器标识符
    /// </summary>
    public required string MatcherId { get; init; }

    /// <summary>
    /// 匹配器名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 匹配器描述
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// 匹配器版本
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// 匹配器类型
    /// </summary>
    public required string MatcherType { get; init; }

    /// <summary>
    /// 作者信息
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 支持的定向条件类型列表
    /// </summary>
    public IReadOnlyList<string> SupportedCriteriaTypes { get; init; } = [];

    /// <summary>
    /// 支持的匹配维度列表
    /// </summary>
    public IReadOnlyList<string> SupportedDimensions { get; init; } = [];

    /// <summary>
    /// 是否支持并行执行
    /// </summary>
    public bool SupportsParallelExecution { get; init; } = true;

    /// <summary>
    /// 是否支持缓存
    /// </summary>
    public bool SupportsCaching { get; init; } = true;

    /// <summary>
    /// 是否支持批量处理
    /// </summary>
    public bool SupportsBatchProcessing { get; init; } = false;

    /// <summary>
    /// 预期执行时间范围（毫秒）
    /// </summary>
    public TimeSpan ExpectedExecutionTime { get; init; } = TimeSpan.FromMilliseconds(10);

    /// <summary>
    /// 最大执行时间（毫秒）
    /// </summary>
    public TimeSpan MaxExecutionTime { get; init; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// 资源需求信息
    /// </summary>
    public ResourceRequirement ResourceRequirement { get; init; } = new();

    /// <summary>
    /// 配置参数定义
    /// </summary>
    public IReadOnlyDictionary<string, ConfigurationParameter> ConfigurationParameters { get; init; } =
        new Dictionary<string, ConfigurationParameter>();

    /// <summary>
    /// 依赖的回调接口类型
    /// </summary>
    public IReadOnlyList<string> RequiredCallbacks { get; init; } = [];

    /// <summary>
    /// 可选的回调接口类型
    /// </summary>
    public IReadOnlyList<string> OptionalCallbacks { get; init; } = [];

    /// <summary>
    /// 匹配器标签（用于分类和搜索）
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// 性能基准信息
    /// </summary>
    public PerformanceBenchmark? PerformanceBenchmark { get; init; }

    /// <summary>
    /// 兼容性信息
    /// </summary>
    public CompatibilityInfo? CompatibilityInfo { get; init; }
}

/// <summary>
/// 配置参数定义
/// </summary>
public record ConfigurationParameter
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 参数类型
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// 参数描述
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// 是否必需
    /// </summary>
    public bool IsRequired { get; init; } = false;

    /// <summary>
    /// 默认值
    /// </summary>
    public object? DefaultValue { get; init; }

    /// <summary>
    /// 取值范围或约束
    /// </summary>
    public string? Constraints { get; init; }

    /// <summary>
    /// 示例值
    /// </summary>
    public object? ExampleValue { get; init; }
}

/// <summary>
/// 性能基准信息
/// </summary>
public record PerformanceBenchmark
{
    /// <summary>
    /// 平均执行时间（毫秒）
    /// </summary>
    public double AverageExecutionTimeMs { get; init; }

    /// <summary>
    /// 95分位执行时间（毫秒）
    /// </summary>
    public double P95ExecutionTimeMs { get; init; }

    /// <summary>
    /// 99分位执行时间（毫秒）
    /// </summary>
    public double P99ExecutionTimeMs { get; init; }

    /// <summary>
    /// 吞吐量（每秒处理请求数）
    /// </summary>
    public double ThroughputPerSecond { get; init; }

    /// <summary>
    /// 内存使用量（MB）
    /// </summary>
    public double MemoryUsageMB { get; init; }

    /// <summary>
    /// CPU使用率（百分比）
    /// </summary>
    public double CpuUsagePercent { get; init; }

    /// <summary>
    /// 基准测试环境
    /// </summary>
    public string? BenchmarkEnvironment { get; init; }

    /// <summary>
    /// 基准测试时间
    /// </summary>
    public DateTime BenchmarkDate { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// 兼容性信息
/// </summary>
public record CompatibilityInfo
{
    /// <summary>
    /// 最低.NET版本要求
    /// </summary>
    public string? MinimumDotNetVersion { get; init; }

    /// <summary>
    /// 支持的操作系统
    /// </summary>
    public IReadOnlyList<string> SupportedOperatingSystems { get; init; } = [];

    /// <summary>
    /// 依赖的NuGet包
    /// </summary>
    public IReadOnlyList<string> Dependencies { get; init; } = [];

    /// <summary>
    /// 向后兼容的版本
    /// </summary>
    public IReadOnlyList<string> BackwardCompatibleVersions { get; init; } = [];

    /// <summary>
    /// 已知的不兼容版本
    /// </summary>
    public IReadOnlyList<string> IncompatibleVersions { get; init; } = [];
}