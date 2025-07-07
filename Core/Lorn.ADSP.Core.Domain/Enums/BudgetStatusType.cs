namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 预算状态类型
    /// </summary>
    public enum BudgetStatusType
    {
        /// <summary>
        /// 活跃
        /// </summary>
        Active = 1,

        /// <summary>
        /// 耗尽
        /// </summary>
        Exhausted = 2,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 3,

        /// <summary>
        /// 超限
        /// </summary>
        OverLimit = 4,

        /// <summary>
        /// 预警
        /// </summary>
        Warning = 5
    }
}
