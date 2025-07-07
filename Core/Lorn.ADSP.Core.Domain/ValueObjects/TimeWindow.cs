namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间窗口
    /// </summary>
    public record TimeWindow
    {
        /// <summary>
        /// 窗口大小
        /// </summary>
        public required TimeSpan Size { get; init; }

        /// <summary>
        /// 滑动步长
        /// </summary>
        public TimeSpan? Step { get; init; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartTime { get; init; }
    }
}
