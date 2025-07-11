namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 置信度等级枚举
/// </summary>
public enum ConfidenceLevel
{
    /// <summary>
    /// 极低置信度（0-0.2）
    /// </summary>
    VeryLow = 1,
    
    /// <summary>
    /// 低置信度（0.2-0.4）
    /// </summary>
    Low = 2,
    
    /// <summary>
    /// 中等置信度（0.4-0.6）
    /// </summary>
    Medium = 3,
    
    /// <summary>
    /// 高置信度（0.6-0.8）
    /// </summary>
    High = 4,
    
    /// <summary>
    /// 极高置信度（0.8-1.0）
    /// </summary>
    VeryHigh = 5
}

/// <summary>
/// 可靠性评级枚举
/// </summary>
public enum ReliabilityRating
{
    /// <summary>
    /// 不可靠
    /// </summary>
    Unreliable = 1,
    
    /// <summary>
    /// 有限可靠
    /// </summary>
    LimitedReliability = 2,
    
    /// <summary>
    /// 基本可靠
    /// </summary>
    BasicReliability = 3,
    
    /// <summary>
    /// 高度可靠
    /// </summary>
    HighReliability = 4,
    
    /// <summary>
    /// 完全可靠
    /// </summary>
    FullReliability = 5
}