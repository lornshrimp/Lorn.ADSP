namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums;

/// <summary>
/// 策略类型
/// </summary>
public enum StrategyType
{
    /// <summary>
    /// 召回策略
    /// </summary>
    Recall = 1,

    /// <summary>
    /// 过滤策略
    /// </summary>
    Filter = 2,

    /// <summary>
    /// 排序策略
    /// </summary>
    Ranking = 3,

    /// <summary>
    /// 预处理策略
    /// </summary>
    Preprocessing = 4,

    /// <summary>
    /// 后处理策略
    /// </summary>
    Postprocessing = 5,

    /// <summary>
    /// 优化策略
    /// </summary>
    Optimization = 6,

    /// <summary>
    /// 验证策略
    /// </summary>
    Validation = 7,

    /// <summary>
    /// 监控策略
    /// </summary>
    Monitoring = 8,

    /// <summary>
    /// 自定义策略
    /// </summary>
    Custom = 99
}
