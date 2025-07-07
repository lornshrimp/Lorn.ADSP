using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// ʱ�䶨��
    /// </summary>
    public class TimeTargeting : ValueObject
    {
        /// <summary>
        /// ���ڼ��б�
        /// </summary>
        public IReadOnlyList<DayOfWeek> Days { get; private set; }

        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        public TimeOnly StartTime { get; private set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public TimeOnly EndTime { get; private set; }

        /// <summary>
        /// ʱ��
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
            // ������ڼ�
            if (Days.Any() && !Days.Contains(dateTime.DayOfWeek))
                return false;

            // ���ʱ�䷶Χ
            var time = TimeOnly.FromDateTime(dateTime);
            if (StartTime <= EndTime)
            {
                return time >= StartTime && time <= EndTime;
            }
            else
            {
                // �����ʱ�䷶Χ
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
            // ��ȡ��ǰʱ��
            DateTime currentTime = DateTime.Now;

            // ����ʱ���
            TimeSpan timeDifference = requestTime - currentTime;

            // ����ʱ������ƥ�����
            // ����ʱ��Խ�ӽ�������Խ�ߣ�������ΧΪ0��1
            decimal score = 1 - (decimal)Math.Abs(timeDifference.TotalHours) / 24;

            // ȷ��������0��1֮��
            return Math.Max(0, Math.Min(1, score));
        }
    }

}