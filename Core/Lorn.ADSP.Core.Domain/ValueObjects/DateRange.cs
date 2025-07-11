using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 日期范围（使用DateOnly表示）
/// 用于表示日期范围，不包含具体时间信息
/// </summary>
public class DateRange : ValueObject
{
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private DateRange()
    {
        StartDate = DateOnly.MinValue;
        EndDate = DateOnly.MinValue;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        ValidateInput(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// 创建日期范围
    /// </summary>
    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// 创建今天的日期范围
    /// </summary>
    public static DateRange CreateToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return new DateRange(today, today);
    }

    /// <summary>
    /// 创建本周的日期范围
    /// </summary>
    public static DateRange CreateThisWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var daysToSubtract = (int)today.DayOfWeek;
        var startOfWeek = today.AddDays(-daysToSubtract);
        var endOfWeek = startOfWeek.AddDays(6);
        return new DateRange(startOfWeek, endOfWeek);
    }

    /// <summary>
    /// 创建本月的日期范围
    /// </summary>
    public static DateRange CreateThisMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        return new DateRange(startOfMonth, endOfMonth);
    }

    /// <summary>
    /// 创建本年的日期范围
    /// </summary>
    public static DateRange CreateThisYear()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startOfYear = new DateOnly(today.Year, 1, 1);
        var endOfYear = new DateOnly(today.Year, 12, 31);
        return new DateRange(startOfYear, endOfYear);
    }

    /// <summary>
    /// 检查日期是否在范围内
    /// </summary>
    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// 获取日期范围天数
    /// </summary>
    public int DaysCount => EndDate.DayNumber - StartDate.DayNumber + 1;

    /// <summary>
    /// 扩展日期范围
    /// </summary>
    public DateRange Extend(int startDays, int endDays)
    {
        return new DateRange(
            StartDate.AddDays(-startDays),
            EndDate.AddDays(endDays));
    }

    /// <summary>
    /// 检查是否与另一个日期范围重叠
    /// </summary>
    public bool OverlapsWith(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    /// <summary>
    /// 获取与另一个日期范围的交集
    /// </summary>
    public DateRange? GetIntersection(DateRange other)
    {
        if (!OverlapsWith(other))
            return null;

        var intersectionStart = StartDate > other.StartDate ? StartDate : other.StartDate;
        var intersectionEnd = EndDate < other.EndDate ? EndDate : other.EndDate;

        return new DateRange(intersectionStart, intersectionEnd);
    }

    /// <summary>
    /// 获取日期范围内的所有日期
    /// </summary>
    public IEnumerable<DateOnly> GetDates()
    {
        var current = StartDate;
        while (current <= EndDate)
        {
            yield return current;
            current = current.AddDays(1);
        }
    }

    /// <summary>
    /// 检查是否包含今天
    /// </summary>
    public bool ContainsToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return Contains(today);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    /// <summary>
    /// 字符串表示
    /// </summary>
    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} - {EndDate:yyyy-MM-dd} ({DaysCount} 天)";
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("开始日期不能大于结束日期");
    }
}