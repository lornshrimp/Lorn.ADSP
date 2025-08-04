namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 错误级别枚举
/// 定义错误的严重程度级别
/// </summary>
public enum ErrorLevel
{
    /// <summary>
    /// 调试信息
    /// </summary>
    Debug = 0,

    /// <summary>
    /// 一般信息
    /// </summary>
    Information = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warning = 2,

    /// <summary>
    /// 错误
    /// </summary>
    Error = 3,

    /// <summary>
    /// 严重错误
    /// </summary>
    Critical = 4,

    /// <summary>
    /// 致命错误
    /// </summary>
    Fatal = 5
}