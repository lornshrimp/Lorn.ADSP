namespace Lorn.ADSP.Core.Domain.ValueObjects;




/// <summary>
/// 圆形地理围栏
/// </summary>
public record CircularGeoFence
{
    /// <summary>
    /// 中心点纬度
    /// </summary>
    public required decimal Latitude { get; init; }

    /// <summary>
    /// 中心点经度
    /// </summary>
    public required decimal Longitude { get; init; }

    /// <summary>
    /// 半径（米）
    /// </summary>
    public required int RadiusMeters { get; init; }

    /// <summary>
    /// 围栏名称
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// 多边形地理围栏
/// </summary>
public record PolygonGeoFence
{
    /// <summary>
    /// 多边形顶点（纬度、经度坐标）
    /// </summary>
    public required IReadOnlyList<GeoPoint> Points { get; init; }

    /// <summary>
    /// 围栏名称
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// 地理坐标点
/// </summary>
public record GeoPoint
{
    /// <summary>
    /// 纬度
    /// </summary>
    public required decimal Latitude { get; init; }

    /// <summary>
    /// 经度
    /// </summary>
    public required decimal Longitude { get; init; }
}

/// <summary>
/// 年龄范围
/// </summary>
public record AgeRange
{
    /// <summary>
    /// 最小年龄
    /// </summary>
    public int MinAge { get; init; }

    /// <summary>
    /// 最大年龄
    /// </summary>
    public int MaxAge { get; init; }

    /// <summary>
    /// 检查年龄是否在范围内
    /// </summary>
    public bool Contains(int age) => age >= MinAge && age <= MaxAge;
}

/// <summary>
/// 时间段
/// </summary>
public record TimeSlot
{
    /// <summary>
    /// 开始时间（小时:分钟）
    /// </summary>
    public required TimeOnly StartTime { get; init; }

    /// <summary>
    /// 结束时间（小时:分钟）
    /// </summary>
    public required TimeOnly EndTime { get; init; }

    /// <summary>
    /// 检查时间是否在时间段内
    /// </summary>
    public bool Contains(TimeOnly time) => time >= StartTime && time <= EndTime;
}

/// <summary>
/// 日期范围
/// </summary>
public record DateRange
{
    /// <summary>
    /// 开始日期
    /// </summary>
    public required DateOnly StartDate { get; init; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public required DateOnly EndDate { get; init; }

    /// <summary>
    /// 检查日期是否在范围内
    /// </summary>
    public bool Contains(DateOnly date) => date >= StartDate && date <= EndDate;
}



/// <summary>
/// 定向类型
/// </summary>
public enum TargetingType
{
    /// <summary>
    /// 包含
    /// </summary>
    Include = 1,

    /// <summary>
    /// 排除
    /// </summary>
    Exclude = 2
}