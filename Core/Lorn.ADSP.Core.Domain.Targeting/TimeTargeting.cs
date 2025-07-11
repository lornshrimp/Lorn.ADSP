namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 时间定向条件
    /// 实现基于时间范围的定向规则配置
    /// </summary>
    public class TimeTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Time";

        /// <summary>
        /// 星期几列表
        /// </summary>
        public IReadOnlyList<DayOfWeek> Days => GetRule<List<DayOfWeek>>("Days") ?? new List<DayOfWeek>();

        /// <summary>
        /// 开始时间
        /// </summary>
        public TimeOnly StartTime => GetRule("StartTime", TimeOnly.MinValue);

        /// <summary>
        /// 结束时间
        /// </summary>
        public TimeOnly EndTime => GetRule("EndTime", TimeOnly.MaxValue);

        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZoneId => GetRule("TimeZoneId", "UTC");

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeTargeting(
            IList<DayOfWeek>? days = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null,
            string timeZoneId = "UTC",
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(days, startTime, endTime, timeZoneId), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static Dictionary<string, object> CreateRules(
            IList<DayOfWeek>? days,
            TimeOnly? startTime,
            TimeOnly? endTime,
            string timeZoneId)
        {
            var rules = new Dictionary<string, object>
            {
                ["Days"] = days?.ToList() ?? Enum.GetValues<DayOfWeek>().ToList(),
                ["StartTime"] = startTime ?? TimeOnly.MinValue,
                ["EndTime"] = endTime ?? TimeOnly.MaxValue,
                ["TimeZoneId"] = timeZoneId ?? "UTC"
            };

            return rules;
        }

        /// <summary>
        /// 创建时间定向条件
        /// </summary>
        public static TimeTargeting Create(
            IList<DayOfWeek>? days = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null,
            string timeZoneId = "UTC",
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new TimeTargeting(days, startTime, endTime, timeZoneId, weight, isEnabled);
        }

        /// <summary>
        /// 设置时间范围
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void SetTimeRange(TimeOnly startTime, TimeOnly endTime)
        {
            SetRule("StartTime", startTime);
            SetRule("EndTime", endTime);
        }

        /// <summary>
        /// 设置星期几
        /// </summary>
        /// <param name="days">星期几列表</param>
        public void SetDays(IList<DayOfWeek> days)
        {
            if (days == null)
                throw new ArgumentNullException(nameof(days));

            SetRule("Days", days.ToList());
        }

        /// <summary>
        /// 添加星期几
        /// </summary>
        /// <param name="dayOfWeek">星期几</param>
        public void AddDay(DayOfWeek dayOfWeek)
        {
            var currentList = Days.ToList();
            if (!currentList.Contains(dayOfWeek))
            {
                currentList.Add(dayOfWeek);
                SetRule("Days", currentList);
            }
        }

        /// <summary>
        /// 设置时区
        /// </summary>
        /// <param name="timeZoneId">时区标识</param>
        public void SetTimeZone(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
                throw new ArgumentException("时区标识不能为空", nameof(timeZoneId));

            SetRule("TimeZoneId", timeZoneId);
        }

        /// <summary>
        /// 检查是否在指定时间激活
        /// </summary>
        /// <param name="dateTime">检查时间</param>
        /// <returns>是否激活</returns>
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
                // 跨日时间范围
                return time >= StartTime || time <= EndTime;
            }
        }

        /// <summary>
        /// 验证时间定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证时区标识
            if (string.IsNullOrWhiteSpace(TimeZoneId))
                return false;

            // 验证至少有一天被选中
            if (!Days.Any())
                return false;

            // 验证时区标识格式（简单验证）
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var timeRange = StartTime == TimeOnly.MinValue && EndTime == TimeOnly.MaxValue
                ? "All Day"
                : $"{StartTime:HH:mm}-{EndTime:HH:mm}";
            var details = $"Days: {Days.Count}, Time: {timeRange}, TimeZone: {TimeZoneId}";
            return $"{summary} - {details}";
        }
    }
}