using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 一天内的时间段（使用TimeOnly表示）
/// 用于表示每日的时间段，如工作时间、营业时间等
/// </summary>
public class TimeSlot : ValueObject
{
    /// <summary>
    /// 开始时间（小时:分钟）
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// 结束时间（小时:分钟）
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TimeSlot()
    {
        StartTime = TimeOnly.MinValue;
        EndTime = TimeOnly.MinValue;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        ValidateInput(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// 创建时间段
    /// </summary>
    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// 创建时间段（小时和分钟）
    /// </summary>
    public static TimeSlot Create(int startHour, int startMinute, int endHour, int endMinute)
    {
        var startTime = new TimeOnly(startHour, startMinute);
        var endTime = new TimeOnly(endHour, endMinute);
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// 创建工作时间段
    /// </summary>
    public static TimeSlot CreateWorkingHours()
    {
        return new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
    }

    /// <summary>
    /// 创建全天时间段
    /// </summary>
    public static TimeSlot CreateAllDay()
    {
        return new TimeSlot(TimeOnly.MinValue, TimeOnly.MaxValue);
    }

    /// <summary>
    /// 检查时间是否在时间段内
    /// </summary>
    public bool Contains(TimeOnly time)
    {
        if (StartTime <= EndTime)
        {
            // 不跨天
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            // 跨天
            return time >= StartTime || time <= EndTime;
        }
    }

    /// <summary>
    /// 获取时间段持续时间
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            if (StartTime <= EndTime)
            {
                return EndTime - StartTime;
            }
            else
            {
                // 跨天
                return (TimeOnly.MaxValue - StartTime) + (EndTime - TimeOnly.MinValue);
            }
        }
    }

    /// <summary>
    /// 是否跨天
    /// </summary>
    public bool IsCrossDay => StartTime > EndTime;

    /// <summary>
    /// 与另一个时间段是否重叠
    /// </summary>
    public bool OverlapsWith(TimeSlot other)
    {
        if (!IsCrossDay && !other.IsCrossDay)
        {
            // 都不跨天
            return StartTime <= other.EndTime && EndTime >= other.StartTime;
        }
        else if (IsCrossDay && other.IsCrossDay)
        {
            // 都跨天
            return true; // 跨天的时间段总是重叠
        }
        else
        {
            // 一个跨天，一个不跨天
            var crossDay = IsCrossDay ? this : other;
            var nonCrossDay = IsCrossDay ? other : this;
            
            return nonCrossDay.StartTime >= crossDay.StartTime || 
                   nonCrossDay.EndTime <= crossDay.EndTime ||
                   (nonCrossDay.StartTime <= crossDay.EndTime);
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
        return $"{StartTime:HH:mm} - {EndTime:HH:mm}{(IsCrossDay ? " (跨天)" : "")}";
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(TimeOnly startTime, TimeOnly endTime)
    {
        // TimeOnly类型本身已经保证了有效性，这里可以添加业务逻辑验证
        // 允许跨天的时间段，所以不需要额外验证
    }
}