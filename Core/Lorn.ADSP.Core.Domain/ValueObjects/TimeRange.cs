namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间范围
    /// </summary>
    public record TimeRange
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public required DateTime StartTime { get; init; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public required DateTime EndTime { get; init; }

        /// <summary>
        /// 时间范围是否有效
        /// </summary>
        public bool IsValid => StartTime <= EndTime;

        /// <summary>
        /// 时间跨度
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// 创建最近N天的时间范围
        /// </summary>
        public static TimeRange LastDays(int days)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddDays(-days);
            return new TimeRange { StartTime = startTime, EndTime = endTime };
        }

        /// <summary>
        /// 创建最近N小时的时间范围
        /// </summary>
        public static TimeRange LastHours(int hours)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddHours(-hours);
            return new TimeRange { StartTime = startTime, EndTime = endTime };
        }

        /// <summary>
        /// 创建今天的时间范围
        /// </summary>
        public static TimeRange Today()
        {
            var today = DateTime.UtcNow.Date;
            return new TimeRange { StartTime = today, EndTime = today.AddDays(1) };
        }
    }
}
