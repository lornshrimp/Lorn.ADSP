namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums;

/// <summary>
/// 执行状态枚举
/// 表示策略或匹配器的执行状态
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// 执行中
    /// </summary>
    Running = 1,

    /// <summary>
    /// 执行成功
    /// </summary>
    Succeeded = 2,

    /// <summary>
    /// 执行失败
    /// </summary>
    Failed = 3,

    /// <summary>
    /// 执行超时
    /// </summary>
    Timeout = 4,

    /// <summary>
    /// 执行被取消
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// 跳过执行
    /// </summary>
    Skipped = 6,

    /// <summary>
    /// 部分成功
    /// </summary>
    PartialSuccess = 7,

    /// <summary>
    /// 执行中断
    /// </summary>
    Interrupted = 8,

    /// <summary>
    /// 等待重试
    /// </summary>
    PendingRetry = 9,

    /// <summary>
    /// 降级执行
    /// </summary>
    Degraded = 10
}