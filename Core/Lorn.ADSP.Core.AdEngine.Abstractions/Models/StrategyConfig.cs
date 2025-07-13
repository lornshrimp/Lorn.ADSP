namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 策略配置
/// </summary>
public record StrategyConfig
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// 配置版本
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 执行优先级
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// 超时设置（毫秒）
    /// </summary>
    public int TimeoutMs { get; init; } = 10000;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// 配置参数
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 环境特定配置
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentConfig { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 权重配置
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Weights { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// 阈值配置
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Thresholds { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// 特性开关
    /// </summary>
    public IReadOnlyDictionary<string, bool> FeatureFlags { get; init; } = new Dictionary<string, bool>();

    /// <summary>
    /// A/B测试配置
    /// </summary>
    public ABTestConfig? ABTestConfig { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 配置创建者
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// 配置更新者
    /// </summary>
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// 配置描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 获取参数值
    /// </summary>
    public T GetParameter<T>(string key, T defaultValue = default!)
    {
        if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取权重值
    /// </summary>
    public decimal GetWeight(string key, decimal defaultValue = 1.0m)
    {
        return Weights.TryGetValue(key, out var weight) ? weight : defaultValue;
    }

    /// <summary>
    /// 获取阈值
    /// </summary>
    public decimal GetThreshold(string key, decimal defaultValue = 0.5m)
    {
        return Thresholds.TryGetValue(key, out var threshold) ? threshold : defaultValue;
    }

    /// <summary>
    /// 获取特性开关状态
    /// </summary>
    public bool GetFeatureFlag(string key, bool defaultValue = false)
    {
        return FeatureFlags.TryGetValue(key, out var flag) ? flag : defaultValue;
    }
}




