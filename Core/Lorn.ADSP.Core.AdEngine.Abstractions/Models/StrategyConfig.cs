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

/// <summary>
/// A/B测试配置
/// </summary>
public record ABTestConfig
{
    /// <summary>
    /// 实验ID
    /// </summary>
    public required string ExperimentId { get; init; }

    /// <summary>
    /// 实验名称
    /// </summary>
    public required string ExperimentName { get; init; }

    /// <summary>
    /// 实验组配置
    /// </summary>
    public required IReadOnlyList<ExperimentGroup> Groups { get; init; }

    /// <summary>
    /// 流量分配比例
    /// </summary>
    public decimal TrafficAllocation { get; init; } = 1.0m;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// 分组策略
    /// </summary>
    public string GroupingStrategy { get; init; } = "hash";

    /// <summary>
    /// 用户分组
    /// </summary>
    public ExperimentGroup? GetUserGroup(string userId)
    {
        if (!IsEnabled || Groups.Count == 0)
            return null;

        var hash = Math.Abs(userId.GetHashCode());
        var totalWeight = Groups.Sum(g => g.Weight);
        var targetWeight = (hash % 100) * totalWeight / 100;

        decimal currentWeight = 0;
        foreach (var group in Groups)
        {
            currentWeight += group.Weight;
            if (targetWeight <= currentWeight)
                return group;
        }

        return Groups.Last();
    }
}

/// <summary>
/// 实验组
/// </summary>
public record ExperimentGroup
{
    /// <summary>
    /// 组ID
    /// </summary>
    public required string GroupId { get; init; }

    /// <summary>
    /// 组名称
    /// </summary>
    public required string GroupName { get; init; }

    /// <summary>
    /// 权重（百分比）
    /// </summary>
    public decimal Weight { get; init; } = 50.0m;

    /// <summary>
    /// 组配置参数
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 是否为对照组
    /// </summary>
    public bool IsControl { get; init; } = false;
}

/// <summary>
/// 验证结果
/// </summary>
public record ValidationResult
{
    /// <summary>
    /// 验证是否成功
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// 验证错误信息
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; init; } = Array.Empty<ValidationError>();

    /// <summary>
    /// 警告信息
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get; init; } = Array.Empty<ValidationWarning>();

    /// <summary>
    /// 验证时间
    /// </summary>
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 创建成功的验证结果
    /// </summary>
    public static ValidationResult Success(IReadOnlyList<ValidationWarning>? warnings = null)
    {
        return new ValidationResult
        {
            IsValid = true,
            Warnings = warnings ?? Array.Empty<ValidationWarning>()
        };
    }

    /// <summary>
    /// 创建失败的验证结果
    /// </summary>
    public static ValidationResult Failure(IReadOnlyList<ValidationError> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
    }

    /// <summary>
    /// 创建单个错误的验证结果
    /// </summary>
    public static ValidationResult Failure(string errorCode, string errorMessage)
    {
        return Failure(new[] { new ValidationError(errorCode, errorMessage) });
    }
}

/// <summary>
/// 验证错误
/// </summary>
public record ValidationError(string ErrorCode, string ErrorMessage)
{
    /// <summary>
    /// 错误字段
    /// </summary>
    public string? Field { get; init; }

    /// <summary>
    /// 错误值
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// 错误严重程度
    /// </summary>
    public ErrorSeverity Severity { get; init; } = ErrorSeverity.Error;
}

/// <summary>
/// 验证警告
/// </summary>
public record ValidationWarning(string WarningCode, string WarningMessage)
{
    /// <summary>
    /// 警告字段
    /// </summary>
    public string? Field { get; init; }

    /// <summary>
    /// 警告值
    /// </summary>
    public object? Value { get; init; }
}

/// <summary>
/// 错误严重程度
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// 信息
    /// </summary>
    Info = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warning = 2,

    /// <summary>
    /// 错误
    /// </summary>
    Error = 3,

    /// <summary>
    /// 致命错误
    /// </summary>
    Critical = 4
}