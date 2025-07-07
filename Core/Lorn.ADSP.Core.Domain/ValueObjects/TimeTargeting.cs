using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间定向
    /// </summary>
    public class TimeTargeting : ValueObject
    {
        /// <summary>
        /// 星期几列表
        /// </summary>
        public IReadOnlyList<DayOfWeek> Days { get; private set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public TimeOnly EndTime { get; private set; }

        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZoneId { get; private set; } // Renamed from TimeZone to TimeZoneId

        private TimeTargeting(
            IReadOnlyList<DayOfWeek> days,
            TimeOnly startTime,
            TimeOnly endTime,
            string timeZoneId) // Updated parameter name
        {
            Days = days;
            StartTime = startTime;
            EndTime = endTime;
            TimeZoneId = timeZoneId; // Updated property assignment
        }

        public static TimeTargeting Create(
            IReadOnlyList<DayOfWeek>? days = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null,
            string timeZoneId = "UTC") // Updated parameter name
        {
            return new TimeTargeting(
                days ?? Enum.GetValues<DayOfWeek>(),
                startTime ?? TimeOnly.MinValue,
                endTime ?? TimeOnly.MaxValue,
                timeZoneId); // Updated argument
        }

        public bool IsActiveAt(DateTime dateTime)
        {
            // 检查星期几
            if (Days.Any() && !Days.Contains(dateTime.DayOfWeek))
                return false;

            // 检查时间范围
            var time = TimeOnly.FromDateTime(dateTime);
            if (StartTime <= EndTime)
            {
                return time >= StartTime && time <= EndTime;
            }
            else
            {
                // 跨天的时间范围
                return time >= StartTime || time <= EndTime;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartTime;
            yield return EndTime;
            yield return TimeZoneId; // Updated property name

            foreach (var day in Days.OrderBy(x => x))
            {
                yield return day;
            }
        }

        internal decimal CalculateMatchScore(DateTime requestTime)
        {
            // 获取当前时间
            DateTime currentTime = DateTime.Now;

            // 计算时间差
            TimeSpan timeDifference = requestTime - currentTime;

            // 根据时间差计算匹配分数
            // 假设时间越接近，分数越高，分数范围为0到1
            decimal score = 1 - (decimal)Math.Abs(timeDifference.TotalHours) / 24;

            // 确保分数在0到1之间
            return Math.Max(0, Math.Min(1, score));
        }
    }

}