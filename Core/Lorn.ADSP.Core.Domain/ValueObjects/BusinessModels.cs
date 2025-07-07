using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// Բ�ε���Χ��
/// </summary>
public class CircularGeoFence : ValueObject
{
    /// <summary>
    /// ���ĵ�γ��
    /// </summary>
    public decimal Latitude { get; private set; }

    /// <summary>
    /// ���ĵ㾭��
    /// </summary>
    public decimal Longitude { get; private set; }

    /// <summary>
    /// �뾶���ף�
    /// </summary>
    public int RadiusMeters { get; private set; }

    /// <summary>
    /// Χ������
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private CircularGeoFence()
    {
        Latitude = 0;
        Longitude = 0;
        RadiusMeters = 0;
    }

    /// <summary>
    /// ���캯��
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
    /// ����Բ�ε���Χ��
    /// </summary>
    public static CircularGeoFence Create(decimal latitude, decimal longitude, int radiusMeters, string? name = null)
    {
        return new CircularGeoFence(latitude, longitude, radiusMeters, name);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public CircularGeoFence WithName(string name)
    {
        return new CircularGeoFence(Latitude, Longitude, RadiusMeters, name);
    }

    /// <summary>
    /// �����Ƿ���Χ����
    /// </summary>
    public bool Contains(decimal latitude, decimal longitude)
    {
        return CalculateDistance(Latitude, Longitude, latitude, longitude) <= RadiusMeters;
    }

    /// <summary>
    /// �����Ƿ���Χ����
    /// </summary>
    public bool Contains(GeoPoint point)
    {
        return Contains(point.Latitude, point.Longitude);
    }

    /// <summary>
    /// ���㵽ĳ��ľ��루�ף�
    /// </summary>
    public double CalculateDistanceTo(decimal latitude, decimal longitude)
    {
        return CalculateDistance(Latitude, Longitude, latitude, longitude);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
        yield return RadiusMeters;
        yield return Name ?? string.Empty;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(decimal latitude, decimal longitude, int radiusMeters)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("γ�ȱ�����-90��90֮��", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("���ȱ�����-180��180֮��", nameof(longitude));

        if (radiusMeters <= 0)
            throw new ArgumentException("�뾶�������0", nameof(radiusMeters));
    }

    /// <summary>
    /// �����������루�ף�
    /// </summary>
    private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadius = 6371000; // ����뾶���ף�

        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    /// <summary>
    /// �Ƕ�ת����
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

/// <summary>
/// ����ε���Χ��
/// </summary>
public class PolygonGeoFence : ValueObject
{
    /// <summary>
    /// ����ζ��㣨γ�ȡ��������꣩
    /// </summary>
    public IReadOnlyList<GeoPoint> Points { get; private set; }

    /// <summary>
    /// Χ������
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private PolygonGeoFence()
    {
        Points = Array.Empty<GeoPoint>();
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public PolygonGeoFence(IReadOnlyList<GeoPoint> points, string? name = null)
    {
        ValidateInput(points);

        Points = points;
        Name = name;
    }

    /// <summary>
    /// ��������ε���Χ��
    /// </summary>
    public static PolygonGeoFence Create(IReadOnlyList<GeoPoint> points, string? name = null)
    {
        return new PolygonGeoFence(points, name);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public PolygonGeoFence WithName(string name)
    {
        return new PolygonGeoFence(Points, name);
    }

    /// <summary>
    /// �����Ƿ��ڶ�����ڣ����߷���
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
    /// �����Ƿ���Χ����
    /// </summary>
    public bool Contains(GeoPoint point)
    {
        return Contains(point.Latitude, point.Longitude);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
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
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(IReadOnlyList<GeoPoint> points)
    {
        if (points == null || points.Count < 3)
            throw new ArgumentException("�����������Ҫ3����", nameof(points));
    }
}

/// <summary>
/// ���������
/// </summary>
public class GeoPoint : ValueObject
{
    /// <summary>
    /// γ��
    /// </summary>
    public decimal Latitude { get; private set; }

    /// <summary>
    /// ����
    /// </summary>
    public decimal Longitude { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private GeoPoint()
    {
        Latitude = 0;
        Longitude = 0;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public GeoPoint(decimal latitude, decimal longitude)
    {
        ValidateInput(latitude, longitude);

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// �������������
    /// </summary>
    public static GeoPoint Create(decimal latitude, decimal longitude)
    {
        return new GeoPoint(latitude, longitude);
    }

    /// <summary>
    /// ���㵽��һ����ľ��루�ף�
    /// </summary>
    public double CalculateDistanceTo(GeoPoint other)
    {
        const double earthRadius = 6371000; // ����뾶���ף�

        var dLat = ToRadians((double)(other.Latitude - Latitude));
        var dLon = ToRadians((double)(other.Longitude - Longitude));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)Latitude)) * Math.Cos(ToRadians((double)other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("γ�ȱ�����-90��90֮��", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("���ȱ�����-180��180֮��", nameof(longitude));
    }

    /// <summary>
    /// �Ƕ�ת����
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

/// <summary>
/// ���䷶Χ
/// </summary>
public class AgeRange : ValueObject
{
    /// <summary>
    /// ��С����
    /// </summary>
    public int MinAge { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public int MaxAge { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private AgeRange()
    {
        MinAge = 0;
        MaxAge = 0;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public AgeRange(int minAge, int maxAge)
    {
        ValidateInput(minAge, maxAge);

        MinAge = minAge;
        MaxAge = maxAge;
    }

    /// <summary>
    /// �������䷶Χ
    /// </summary>
    public static AgeRange Create(int minAge, int maxAge)
    {
        return new AgeRange(minAge, maxAge);
    }

    /// <summary>
    /// �������������䷶Χ
    /// </summary>
    public static AgeRange CreateTeenager()
    {
        return new AgeRange(13, 19);
    }

    /// <summary>
    /// �������������䷶Χ
    /// </summary>
    public static AgeRange CreateAdult()
    {
        return new AgeRange(18, 65);
    }

    /// <summary>
    /// ��������Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(int age)
    {
        return age >= MinAge && age <= MaxAge;
    }

    /// <summary>
    /// ��չ���䷶Χ
    /// </summary>
    public AgeRange Extend(int minExtension, int maxExtension)
    {
        return new AgeRange(
            Math.Max(0, MinAge - minExtension),
            MaxAge + maxExtension);
    }

    /// <summary>
    /// ��ȡ������
    /// </summary>
    public int Span => MaxAge - MinAge + 1;

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MinAge;
        yield return MaxAge;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(int minAge, int maxAge)
    {
        if (minAge < 0)
            throw new ArgumentException("��С���䲻��Ϊ����", nameof(minAge));

        if (maxAge < 0)
            throw new ArgumentException("������䲻��Ϊ����", nameof(maxAge));

        if (minAge > maxAge)
            throw new ArgumentException("��С���䲻�ܴ����������");
    }
}

/// <summary>
/// ʱ���
/// </summary>
public class TimeSlot : ValueObject
{
    /// <summary>
    /// ��ʼʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// ����ʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TimeSlot()
    {
        StartTime = TimeOnly.MinValue;
        EndTime = TimeOnly.MinValue;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        ValidateInput(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// ����ʱ���
    /// </summary>
    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// ����ʱ��Σ�Сʱ�ͷ��ӣ�
    /// </summary>
    public static TimeSlot Create(int startHour, int startMinute, int endHour, int endMinute)
    {
        var startTime = new TimeOnly(startHour, startMinute);
        var endTime = new TimeOnly(endHour, endMinute);
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// ��������ʱ���
    /// </summary>
    public static TimeSlot CreateWorkingHours()
    {
        return new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
    }

    /// <summary>
    /// ����ȫ��ʱ���
    /// </summary>
    public static TimeSlot CreateAllDay()
    {
        return new TimeSlot(TimeOnly.MinValue, TimeOnly.MaxValue);
    }

    /// <summary>
    /// ���ʱ���Ƿ���ʱ�����
    /// </summary>
    public bool Contains(TimeOnly time)
    {
        if (StartTime <= EndTime)
        {
            // ������
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            // ����
            return time >= StartTime || time <= EndTime;
        }
    }

    /// <summary>
    /// ��ȡʱ��γ���ʱ��
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
                // ����
                return (TimeOnly.MaxValue - StartTime) + (EndTime - TimeOnly.MinValue);
            }
        }
    }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsCrossDay => StartTime > EndTime;

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(TimeOnly startTime, TimeOnly endTime)
    {
        // TimeOnly���ͱ����Ѿ���֤����Ч�ԣ�����������ҵ���߼���֤
        // Ŀǰ��������ʱ��Σ����Բ���Ҫ������֤
    }
}

/// <summary>
/// ���ڷ�Χ
/// </summary>
public class DateRange : ValueObject
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DateRange()
    {
        StartDate = DateOnly.MinValue;
        EndDate = DateOnly.MinValue;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        ValidateInput(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// �������ڷ�Χ
    /// </summary>
    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// ������������ڷ�Χ
    /// </summary>
    public static DateRange CreateToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return new DateRange(today, today);
    }

    /// <summary>
    /// �������ܵ����ڷ�Χ
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
    /// �������µ����ڷ�Χ
    /// </summary>
    public static DateRange CreateThisMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        return new DateRange(startOfMonth, endOfMonth);
    }

    /// <summary>
    /// ��������Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// ��ȡ���ڷ�Χ����
    /// </summary>
    public int DaysCount => EndDate.DayNumber - StartDate.DayNumber + 1;

    /// <summary>
    /// ��չ���ڷ�Χ
    /// </summary>
    public DateRange Extend(int startDays, int endDays)
    {
        return new DateRange(
            StartDate.AddDays(-startDays),
            EndDate.AddDays(endDays));
    }

    /// <summary>
    /// ����Ƿ�����һ�����ڷ�Χ�ص�
    /// </summary>
    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("��ʼ���ڲ��ܴ��ڽ�������");
    }
}



