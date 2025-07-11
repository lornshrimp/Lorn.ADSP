using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 具体的时间范围（使用DateTime表示）
/// 用于表示具有明确开始和结束时间的时间范围，如活动时间、会议时间等
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
    /// 时间范围大小
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TimeRange() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public TimeRange(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("开始时间必须早于结束时间");

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
    /// 创建从现在开始的时间范围
    /// </summary>
    public static TimeRange CreateFromNow(TimeSpan duration)
    {
        var now = DateTime.UtcNow;
        return new TimeRange(now, now.Add(duration));
    }

    /// <summary>
    /// 创建指定时长的时间范围
    /// </summary>
    public static TimeRange CreateWithDuration(DateTime startTime, TimeSpan duration)
    {
        return new TimeRange(startTime, startTime.Add(duration));
    }

    /// <summary>
    /// 创建今天的时间范围
    /// </summary>
    public static TimeRange CreateToday()
    {
        var today = DateTime.Today;
        return new TimeRange(today, today.AddDays(1).AddTicks(-1));
    }

    /// <summary>
    /// 创建本周的时间范围
    /// </summary>
    public static TimeRange CreateThisWeek()
    {
        var today = DateTime.Today;
        var daysToSubtract = (int)today.DayOfWeek;
        var startOfWeek = today.AddDays(-daysToSubtract);
        var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
        return new TimeRange(startOfWeek, endOfWeek);
    }

    /// <summary>
    /// 检查指定时间是否在范围内
    /// </summary>
    public bool Contains(DateTime time)
    {
        return time >= StartTime && time <= EndTime;
    }

    /// <summary>
    /// 检查是否与另一个时间范围重叠
    /// </summary>
    public bool OverlapsWith(TimeRange other)
    {
        return StartTime <= other.EndTime && EndTime >= other.StartTime;
    }

    /// <summary>
    /// 获取与另一个时间范围的交集
    /// </summary>
    public TimeRange? GetIntersection(TimeRange other)
    {
        if (!OverlapsWith(other))
            return null;

        var intersectionStart = StartTime > other.StartTime ? StartTime : other.StartTime;
        var intersectionEnd = EndTime < other.EndTime ? EndTime : other.EndTime;

        return new TimeRange(intersectionStart, intersectionEnd);
    }

    /// <summary>
    /// 扩展时间范围
    /// </summary>
    public TimeRange Extend(TimeSpan beforeStart, TimeSpan afterEnd)
    {
        return new TimeRange(StartTime.Subtract(beforeStart), EndTime.Add(afterEnd));
    }

    /// <summary>
    /// 检查是否包含当前时间
    /// </summary>
    public bool ContainsNow()
    {
        return Contains(DateTime.UtcNow);
    }

    /// <summary>
    /// 检查是否已过期
    /// </summary>
    public bool IsExpired()
    {
        return EndTime < DateTime.UtcNow;
    }

    /// <summary>
    /// 检查是否尚未开始
    /// </summary>
    public bool IsNotStarted()
    {
        return StartTime > DateTime.UtcNow;
    }

    /// <summary>
    /// 检查是否正在进行中
    /// </summary>
    public bool IsActive()
    {
        return ContainsNow();
    }

    /// <summary>
    /// 分割时间范围为指定时长的段
    /// </summary>
    public IEnumerable<TimeRange> Split(TimeSpan segmentDuration)
    {
        if (segmentDuration <= TimeSpan.Zero)
            throw new ArgumentException("分段时长必须大于0", nameof(segmentDuration));

        var current = StartTime;
        while (current < EndTime)
        {
            var segmentEnd = current.Add(segmentDuration);
            if (segmentEnd > EndTime)
                segmentEnd = EndTime;

            yield return new TimeRange(current, segmentEnd);
            current = segmentEnd;
        }
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
    /// 字符串表示
    /// </summary>
    public override string ToString()
    {
        return $"{StartTime:yyyy-MM-dd HH:mm:ss} - {EndTime:yyyy-MM-dd HH:mm:ss} ({Duration})";
    }
}