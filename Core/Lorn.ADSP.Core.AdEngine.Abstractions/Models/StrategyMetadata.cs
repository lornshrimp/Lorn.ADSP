using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 策略元数据
/// </summary>
public record StrategyMetadata
{
    /// <summary>
    /// 策略唯一标识符
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 策略名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 策略版本
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// 策略类型
    /// </summary>
    public required StrategyType Type { get; init; }

    /// <summary>
    /// 策略描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 策略作者
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// 执行优先级
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// 是否支持并行执行
    /// </summary>
    public bool CanRunInParallel { get; init; } = true;

    /// <summary>
    /// 预期执行时间
    /// </summary>
    public TimeSpan EstimatedExecutionTime { get; init; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// 资源需求
    /// </summary>
    public ResourceRequirement ResourceRequirement { get; init; } = new();

    /// <summary>
    /// 所需回调接口类型
    /// </summary>
    public IReadOnlyList<Type> RequiredCallbackTypes { get; init; } = Array.Empty<Type>();

    /// <summary>
    /// 所需回调名称
    /// </summary>
    public IReadOnlyList<string> RequiredCallbackNames { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 配置模式定义
    /// </summary>
    public IReadOnlyDictionary<string, object> ConfigurationSchema { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 支持的功能特性
    /// </summary>
    public IReadOnlyList<string> SupportedFeatures { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 策略标签
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 兼容的广告类型
    /// </summary>
    public IReadOnlyList<AdType> CompatibleAdTypes { get; init; } = Array.Empty<AdType>();

    /// <summary>
    /// 最小支持的数据量
    /// </summary>
    public int MinDataSize { get; init; } = 1;

    /// <summary>
    /// 最大支持的数据量
    /// </summary>
    public int MaxDataSize { get; init; } = int.MaxValue;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 资源需求
/// </summary>
public record ResourceRequirement
{
    /// <summary>
    /// CPU需求等级（1-10）
    /// </summary>
    public int CpuRequirement { get; init; } = 5;

    /// <summary>
    /// 内存需求（MB）
    /// </summary>
    public int MemoryRequirementMB { get; init; } = 100;

    /// <summary>
    /// 网络需求等级（1-10）
    /// </summary>
    public int NetworkRequirement { get; init; } = 5;

    /// <summary>
    /// 磁盘IO需求等级（1-10）
    /// </summary>
    public int DiskIORequirement { get; init; } = 3;

    /// <summary>
    /// 是否需要GPU
    /// </summary>
    public bool RequiresGpu { get; init; } = false;

    /// <summary>
    /// 最大并发数
    /// </summary>
    public int MaxConcurrency { get; init; } = 10;

    /// <summary>
    /// 超时时间
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 重试次数
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// 是否为轻量级策略
    /// </summary>
    public bool IsLightweight => CpuRequirement <= 3 && MemoryRequirementMB <= 50 && !RequiresGpu;

    /// <summary>
    /// 是否为重量级策略
    /// </summary>
    public bool IsHeavyweight => CpuRequirement >= 8 || MemoryRequirementMB >= 500 || RequiresGpu;
}