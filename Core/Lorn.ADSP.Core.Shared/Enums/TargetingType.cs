namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 定向类型枚举
/// </summary>
public enum TargetingType
{
    /// <summary>
    /// 地理位置定向
    /// </summary>
    Geographic = 1,

    /// <summary>
    /// 人口属性定向
    /// </summary>
    Demographic = 2,

    /// <summary>
    /// 行为定向
    /// </summary>
    Behavioral = 3,

    /// <summary>
    /// 上下文定向
    /// </summary>
    Contextual = 4,

    /// <summary>
    /// 重定向
    /// </summary>
    Retargeting = 5
}