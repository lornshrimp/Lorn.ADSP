namespace Lorn.ADSP.Core.AdEngine.Abstractions.Constants;

/// <summary>
/// 策略常量
/// </summary>
public static class StrategyConstants
{
    /// <summary>
    /// 默认策略优先级
    /// </summary>
    public const int DefaultPriority = 5;

    /// <summary>
    /// 最高优先级
    /// </summary>
    public const int HighestPriority = 1;

    /// <summary>
    /// 最低优先级
    /// </summary>
    public const int LowestPriority = 10;

    /// <summary>
    /// 默认超时时间（毫秒）
    /// </summary>
    public const int DefaultTimeoutMs = 10000;

    /// <summary>
    /// 最小超时时间（毫秒）
    /// </summary>
    public const int MinTimeoutMs = 100;

    /// <summary>
    /// 最大超时时间（毫秒）
    /// </summary>
    public const int MaxTimeoutMs = 60000;

    /// <summary>
    /// 默认重试次数
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public const int MaxRetries = 10;

    /// <summary>
    /// 默认批处理大小
    /// </summary>
    public const int DefaultBatchSize = 100;

    /// <summary>
    /// 最大批处理大小
    /// </summary>
    public const int MaxBatchSize = 10000;

    /// <summary>
    /// 策略版本格式
    /// </summary>
    public const string VersionFormat = @"^\d+\.\d+\.\d+$";

    /// <summary>
    /// 策略ID格式
    /// </summary>
    public const string StrategyIdFormat = @"^[a-zA-Z][a-zA-Z0-9_]*$";
}









