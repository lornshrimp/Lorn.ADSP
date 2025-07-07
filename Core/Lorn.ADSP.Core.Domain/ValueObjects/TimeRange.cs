using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间范围
    /// </summary>
    public class TimeRange : ValueObject
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// 时间范围是否有效
        /// </summary>
        public bool IsValid => StartTime <= EndTime;

        /// <summary>
        /// 时间跨度
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TimeRange()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeRange(DateTime startTime, DateTime endTime)
        {
            ValidateInput(startTime, endTime);

            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 创建时间范围
        /// </summary>
        public static TimeRange Create(DateTime startTime, DateTime endTime)
        {
            return new TimeRange(startTime, endTime);
        }

        /// <summary>
        /// 创建最近N天的时间范围
        /// </summary>
        public static TimeRange LastDays(int days)
        {
            if (days < 0)
                throw new ArgumentException("天数不能为负数", nameof(days));

            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddDays(-days);
            return new TimeRange(startTime, endTime);
        }

        /// <summary>
        /// 创建最近N小时的时间范围
        /// </summary>
        public static TimeRange LastHours(int hours)
        {
            if (hours < 0)
                throw new ArgumentException("小时数不能为负数", nameof(hours));

            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddHours(-hours);
            return new TimeRange(startTime, endTime);
        }

        /// <summary>
        /// 创建今天的时间范围
        /// </summary>
        public static TimeRange Today()
        {
            var today = DateTime.UtcNow.Date;
            return new TimeRange(today, today.AddDays(1));
        }

        /// <summary>
        /// 创建本周的时间范围
        /// </summary>
        public static TimeRange ThisWeek()
        {
            var today = DateTime.UtcNow.Date;
            var daysToSubtract = (int)today.DayOfWeek;
            var startOfWeek = today.AddDays(-daysToSubtract);
            var endOfWeek = startOfWeek.AddDays(7);
            return new TimeRange(startOfWeek, endOfWeek);
        }

        /// <summary>
        /// 创建本月的时间范围
        /// </summary>
        public static TimeRange ThisMonth()
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            return new TimeRange(startOfMonth, endOfMonth);
        }

        /// <summary>
        /// 检查时间是否在范围内
        /// </summary>
        public bool Contains(DateTime time)
        {
            return time >= StartTime && time <= EndTime;
        }

        /// <summary>
        /// 检查是否与另一个时间范围重叠
        /// </summary>
        public bool Overlaps(TimeRange other)
        {
            return StartTime <= other.EndTime && EndTime >= other.StartTime;
        }

        /// <summary>
        /// 获取与另一个时间范围的交集
        /// </summary>
        public TimeRange? Intersect(TimeRange other)
        {
            if (!Overlaps(other))
                return null;

            var intersectionStart = StartTime > other.StartTime ? StartTime : other.StartTime;
            var intersectionEnd = EndTime < other.EndTime ? EndTime : other.EndTime;

            return new TimeRange(intersectionStart, intersectionEnd);
        }

        /// <summary>
        /// 扩展时间范围
        /// </summary>
        public TimeRange Extend(TimeSpan duration)
        {
            return new TimeRange(StartTime.Subtract(duration), EndTime.Add(duration));
        }

        /// <summary>
        /// 移动时间范围
        /// </summary>
        public TimeRange Shift(TimeSpan offset)
        {
            return new TimeRange(StartTime.Add(offset), EndTime.Add(offset));
        }

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartTime;
            yield return EndTime;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
                throw new ArgumentException("开始时间不能大于结束时间");
        }
    }
}
