namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums
{
    /// <summary>
    /// 执行状态
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 正在执行
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
        /// 已取消
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout = 5,

        /// <summary>
        /// 已跳过
        /// </summary>
        Skipped = 6
    }
}
