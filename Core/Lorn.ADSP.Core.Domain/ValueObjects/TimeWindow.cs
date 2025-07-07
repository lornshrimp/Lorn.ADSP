using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间窗口
    /// </summary>
    public class TimeWindow : ValueObject
    {
        /// <summary>
        /// 窗口大小
        /// </summary>
        public TimeSpan Size { get; private set; }

        /// <summary>
        /// 滑动步长
        /// </summary>
        public TimeSpan? Step { get; private set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TimeWindow()
        {
            Size = TimeSpan.Zero;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeWindow(TimeSpan size, TimeSpan? step = null, DateTime? startTime = null)
        {
            ValidateInput(size, step);

            Size = size;
            Step = step;
            StartTime = startTime;
        }

        /// <summary>
        /// 创建时间窗口
        /// </summary>
        public static TimeWindow Create(TimeSpan size, TimeSpan? step = null, DateTime? startTime = null)
        {
            return new TimeWindow(size, step, startTime);
        }

        /// <summary>
        /// 创建滑动窗口
        /// </summary>
        public static TimeWindow CreateSliding(TimeSpan size, TimeSpan step, DateTime? startTime = null)
        {
            return new TimeWindow(size, step, startTime);
        }

        /// <summary>
        /// 创建固定窗口
        /// </summary>
        public static TimeWindow CreateFixed(TimeSpan size, DateTime? startTime = null)
        {
            return new TimeWindow(size, null, startTime);
        }

        /// <summary>
        /// 设置起始时间
        /// </summary>
        public TimeWindow WithStartTime(DateTime startTime)
        {
            return new TimeWindow(Size, Step, startTime);
        }

        /// <summary>
        /// 设置滑动步长
        /// </summary>
        public TimeWindow WithStep(TimeSpan step)
        {
            return new TimeWindow(Size, step, StartTime);
        }

        /// <summary>
        /// 获取窗口结束时间
        /// </summary>
        public DateTime? GetEndTime()
        {
            return StartTime?.Add(Size);
        }

        /// <summary>
        /// 检查时间是否在窗口内
        /// </summary>
        public bool ContainsTime(DateTime time)
        {
            if (!StartTime.HasValue)
                return false;

            return time >= StartTime.Value && time < StartTime.Value.Add(Size);
        }

        /// <summary>
        /// 获取下一个窗口
        /// </summary>
        public TimeWindow GetNext()
        {
            if (!StartTime.HasValue)
                throw new InvalidOperationException("无法获取下一个窗口，起始时间未设置");

            var nextStartTime = StartTime.Value.Add(Step ?? Size);
            return new TimeWindow(Size, Step, nextStartTime);
        }

        /// <summary>
        /// 是否为滑动窗口
        /// </summary>
        public bool IsSliding => Step.HasValue;

        /// <summary>
        /// 是否为固定窗口
        /// </summary>
        public bool IsFixed => !Step.HasValue;

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Size;
            yield return Step ?? TimeSpan.Zero;
            yield return StartTime ?? DateTime.MinValue;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(TimeSpan size, TimeSpan? step)
        {
            if (size <= TimeSpan.Zero)
                throw new ArgumentException("窗口大小必须大于0", nameof(size));

            if (step.HasValue && step.Value <= TimeSpan.Zero)
                throw new ArgumentException("滑动步长必须大于0", nameof(step));
        }
    }
}
