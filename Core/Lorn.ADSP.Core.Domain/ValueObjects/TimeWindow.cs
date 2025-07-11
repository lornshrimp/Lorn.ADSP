using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 时间窗口（用于数据处理和分析）
    /// 支持滑动窗口和固定窗口，主要用于数据聚合、监控等场景
    /// 与TimeRange的区别：TimeWindow侧重于数据处理的时间窗口概念，TimeRange侧重于具体的时间范围
    /// </summary>
    public class TimeWindow : ValueObject
    {
        /// <summary>
        /// 窗口大小
        /// </summary>
        public TimeSpan Size { get; private set; }

        /// <summary>
        /// 滑动步长（null表示固定窗口）
        /// </summary>
        public TimeSpan? Step { get; private set; }

        /// <summary>
        /// 起始时间（可选）
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
        /// 创建固定窗口（翻滚窗口）
        /// </summary>
        public static TimeWindow CreateFixed(TimeSpan size, DateTime? startTime = null)
        {
            return new TimeWindow(size, null, startTime);
        }

        /// <summary>
        /// 创建实时数据窗口（从当前时间开始）
        /// </summary>
        public static TimeWindow CreateRealTime(TimeSpan size, TimeSpan? step = null)
        {
            return new TimeWindow(size, step, DateTime.UtcNow);
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
        /// 获取前一个窗口
        /// </summary>
        public TimeWindow GetPrevious()
        {
            if (!StartTime.HasValue)
                throw new InvalidOperationException("无法获取前一个窗口，起始时间未设置");

            var previousStartTime = StartTime.Value.Subtract(Step ?? Size);
            return new TimeWindow(Size, Step, previousStartTime);
        }

        /// <summary>
        /// 生成指定数量的连续窗口
        /// </summary>
        public IEnumerable<TimeWindow> GenerateSequence(int count)
        {
            if (!StartTime.HasValue)
                throw new InvalidOperationException("无法生成窗口序列，起始时间未设置");

            if (count <= 0)
                throw new ArgumentException("窗口数量必须大于0", nameof(count));

            var current = this;
            for (int i = 0; i < count; i++)
            {
                yield return current;
                current = current.GetNext();
            }
        }

        /// <summary>
        /// 获取当前窗口的TimeRange表示
        /// </summary>
        public TimeRange? ToTimeRange()
        {
            if (!StartTime.HasValue)
                return null;

            return new TimeRange(StartTime.Value, StartTime.Value.Add(Size));
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
        /// 窗口类型描述
        /// </summary>
        public string WindowType => IsSliding ? "滑动窗口" : "固定窗口";

        /// <summary>
        /// 检查是否与另一个窗口重叠
        /// </summary>
        public bool OverlapsWith(TimeWindow other)
        {
            if (!StartTime.HasValue || !other.StartTime.HasValue)
                return false;

            var thisEnd = StartTime.Value.Add(Size);
            var otherEnd = other.StartTime.Value.Add(other.Size);

            return StartTime.Value < otherEnd && thisEnd > other.StartTime.Value;
        }

        /// <summary>
        /// 计算窗口覆盖率（相对于另一个窗口）
        /// </summary>
        public double CalculateCoverageRatio(TimeWindow other)
        {
            if (!OverlapsWith(other))
                return 0.0;

            var thisRange = ToTimeRange();
            var otherRange = other.ToTimeRange();

            if (thisRange == null || otherRange == null)
                return 0.0;

            var intersection = thisRange.GetIntersection(otherRange);
            if (intersection == null)
                return 0.0;

            return intersection.Duration.TotalMilliseconds / Size.TotalMilliseconds;
        }

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
        /// 字符串表示
        /// </summary>
        public override string ToString()
        {
            var stepInfo = Step.HasValue ? $", 步长: {Step.Value}" : "";
            var startInfo = StartTime.HasValue ? $", 开始: {StartTime.Value:yyyy-MM-dd HH:mm:ss}" : "";
            return $"{WindowType} (大小: {Size}{stepInfo}{startInfo})";
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

            if (step.HasValue && step.Value > size)
                throw new ArgumentException("滑动步长不应大于窗口大小", nameof(step));
        }
    }
}
