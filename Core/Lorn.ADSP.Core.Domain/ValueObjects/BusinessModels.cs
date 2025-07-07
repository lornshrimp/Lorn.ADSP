using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 圆形地理围栏
/// </summary>
public class CircularGeoFence : ValueObject
{
    /// <summary>
    /// 中心点纬度
    /// </summary>
    public decimal Latitude { get; private set; }

    /// <summary>
    /// 中心点经度
    /// </summary>
    public decimal Longitude { get; private set; }

    /// <summary>
    /// 半径（米）
    /// </summary>
    public int RadiusMeters { get; private set; }

    /// <summary>
    /// 围栏名称
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private CircularGeoFence()
    {
        Latitude = 0;
        Longitude = 0;
        RadiusMeters = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CircularGeoFence(decimal latitude, decimal longitude, int radiusMeters, string? name = null)
    {
        ValidateInput(latitude, longitude, radiusMeters);

        Latitude = latitude;
        Longitude = longitude;
        RadiusMeters = radiusMeters;
        Name = name;
    }

    /// <summary>
    /// 创建圆形地理围栏
    /// </summary>
    public static CircularGeoFence Create(decimal latitude, decimal longitude, int radiusMeters, string? name = null)
    {
        return new CircularGeoFence(latitude, longitude, radiusMeters, name);
    }

    /// <summary>
    /// 设置名称
    /// </summary>
    public CircularGeoFence WithName(string name)
    {
        return new CircularGeoFence(Latitude, Longitude, RadiusMeters, name);
    }

    /// <summary>
    /// 检查点是否在围栏内
    /// </summary>
    public bool Contains(decimal latitude, decimal longitude)
    {
        return CalculateDistance(Latitude, Longitude, latitude, longitude) <= RadiusMeters;
    }

    /// <summary>
    /// 检查点是否在围栏内
    /// </summary>
    public bool Contains(GeoPoint point)
    {
        return Contains(point.Latitude, point.Longitude);
    }

    /// <summary>
    /// 计算到某点的距离（米）
    /// </summary>
    public double CalculateDistanceTo(decimal latitude, decimal longitude)
    {
        return CalculateDistance(Latitude, Longitude, latitude, longitude);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
        yield return RadiusMeters;
        yield return Name ?? string.Empty;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(decimal latitude, decimal longitude, int radiusMeters)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("纬度必须在-90到90之间", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("经度必须在-180到180之间", nameof(longitude));

        if (radiusMeters <= 0)
            throw new ArgumentException("半径必须大于0", nameof(radiusMeters));
    }

    /// <summary>
    /// 计算两点间距离（米）
    /// </summary>
    private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadius = 6371000; // 地球半径（米）

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    /// <summary>
    /// 角度转弧度
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

/// <summary>
/// 多边形地理围栏
/// </summary>
public class PolygonGeoFence : ValueObject
{
    /// <summary>
    /// 多边形顶点（纬度、经度坐标）
    /// </summary>
    public IReadOnlyList<GeoPoint> Points { get; private set; }

    /// <summary>
    /// 围栏名称
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private PolygonGeoFence()
    {
        Points = Array.Empty<GeoPoint>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PolygonGeoFence(IReadOnlyList<GeoPoint> points, string? name = null)
    {
        ValidateInput(points);

        Points = points;
        Name = name;
    }

    /// <summary>
    /// 创建多边形地理围栏
    /// </summary>
    public static PolygonGeoFence Create(IReadOnlyList<GeoPoint> points, string? name = null)
    {
        return new PolygonGeoFence(points, name);
    }

    /// <summary>
    /// 设置名称
    /// </summary>
    public PolygonGeoFence WithName(string name)
    {
        return new PolygonGeoFence(Points, name);
    }

    /// <summary>
    /// 检查点是否在多边形内（射线法）
    /// </summary>
    public bool Contains(decimal latitude, decimal longitude)
    {
        if (Points.Count < 3)
            return false;

        var intersections = 0;
        for (int i = 0; i < Points.Count; i++)
        {
            var j = (i + 1) % Points.Count;
            var pi = Points[i];
            var pj = Points[j];

            if (((pi.Latitude > latitude) != (pj.Latitude > latitude)) &&
                (longitude < (pj.Longitude - pi.Longitude) * (latitude - pi.Latitude) / (pj.Latitude - pi.Latitude) + pi.Longitude))
            {
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    /// <summary>
    /// 检查点是否在围栏内
    /// </summary>
    public bool Contains(GeoPoint point)
    {
        return Contains(point.Latitude, point.Longitude);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name ?? string.Empty;
        foreach (var point in Points)
        {
            yield return point;
        }
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(IReadOnlyList<GeoPoint> points)
    {
        if (points == null || points.Count < 3)
            throw new ArgumentException("多边形至少需要3个点", nameof(points));
    }
}

/// <summary>
/// 地理坐标点
/// </summary>
public class GeoPoint : ValueObject
{
    /// <summary>
    /// 纬度
    /// </summary>
    public decimal Latitude { get; private set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal Longitude { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private GeoPoint()
    {
        Latitude = 0;
        Longitude = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public GeoPoint(decimal latitude, decimal longitude)
    {
        ValidateInput(latitude, longitude);

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// 创建地理坐标点
    /// </summary>
    public static GeoPoint Create(decimal latitude, decimal longitude)
    {
        return new GeoPoint(latitude, longitude);
    }

    /// <summary>
    /// 计算到另一个点的距离（米）
    /// </summary>
    public double CalculateDistanceTo(GeoPoint other)
    {
        const double earthRadius = 6371000; // 地球半径（米）

        var dLat = ToRadians((double)(other.Latitude - Latitude));
        var dLon = ToRadians((double)(other.Longitude - Longitude));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)Latitude)) * Math.Cos(ToRadians((double)other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("纬度必须在-90到90之间", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("经度必须在-180到180之间", nameof(longitude));
    }

    /// <summary>
    /// 角度转弧度
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

/// <summary>
/// 年龄范围
/// </summary>
public class AgeRange : ValueObject
{
    /// <summary>
    /// 最小年龄
    /// </summary>
    public int MinAge { get; private set; }

    /// <summary>
    /// 最大年龄
    /// </summary>
    public int MaxAge { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AgeRange()
    {
        MinAge = 0;
        MaxAge = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AgeRange(int minAge, int maxAge)
    {
        ValidateInput(minAge, maxAge);

        MinAge = minAge;
        MaxAge = maxAge;
    }

    /// <summary>
    /// 创建年龄范围
    /// </summary>
    public static AgeRange Create(int minAge, int maxAge)
    {
        return new AgeRange(minAge, maxAge);
    }

    /// <summary>
    /// 创建青少年年龄范围
    /// </summary>
    public static AgeRange CreateTeenager()
    {
        return new AgeRange(13, 19);
    }

    /// <summary>
    /// 创建成年人年龄范围
    /// </summary>
    public static AgeRange CreateAdult()
    {
        return new AgeRange(18, 65);
    }

    /// <summary>
    /// 检查年龄是否在范围内
    /// </summary>
    public bool Contains(int age)
    {
        return age >= MinAge && age <= MaxAge;
    }

    /// <summary>
    /// 扩展年龄范围
    /// </summary>
    public AgeRange Extend(int minExtension, int maxExtension)
    {
        return new AgeRange(
            Math.Max(0, MinAge - minExtension),
            MaxAge + maxExtension);
    }

    /// <summary>
    /// 获取年龄跨度
    /// </summary>
    public int Span => MaxAge - MinAge + 1;

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MinAge;
        yield return MaxAge;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(int minAge, int maxAge)
    {
        if (minAge < 0)
            throw new ArgumentException("最小年龄不能为负数", nameof(minAge));

        if (maxAge < 0)
            throw new ArgumentException("最大年龄不能为负数", nameof(maxAge));

        if (minAge > maxAge)
            throw new ArgumentException("最小年龄不能大于最大年龄");
    }
}

/// <summary>
/// 时间段
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
    private static void ValidateInput(TimeOnly startTime, TimeOnly endTime)
    {
        // TimeOnly类型本身已经保证了有效性，这里可以添加业务逻辑验证
        // 目前允许跨天的时间段，所以不需要额外验证
    }
}

/// <summary>
/// 日期范围
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
    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
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
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("开始日期不能大于结束日期");
    }
}



