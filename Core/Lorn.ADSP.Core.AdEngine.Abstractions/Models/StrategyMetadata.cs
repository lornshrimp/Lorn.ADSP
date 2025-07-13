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

