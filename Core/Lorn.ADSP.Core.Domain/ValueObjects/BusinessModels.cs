namespace Lorn.ADSP.Core.Domain.ValueObjects;




/// <summary>
/// Բ�ε���Χ��
/// </summary>
public record CircularGeoFence
{
    /// <summary>
    /// ���ĵ�γ��
    /// </summary>
    public required decimal Latitude { get; init; }

    /// <summary>
    /// ���ĵ㾭��
    /// </summary>
    public required decimal Longitude { get; init; }

    /// <summary>
    /// �뾶���ף�
    /// </summary>
    public required int RadiusMeters { get; init; }

    /// <summary>
    /// Χ������
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// ����ε���Χ��
/// </summary>
public record PolygonGeoFence
{
    /// <summary>
    /// ����ζ��㣨γ�ȡ��������꣩
    /// </summary>
    public required IReadOnlyList<GeoPoint> Points { get; init; }

    /// <summary>
    /// Χ������
    /// </summary>
    public string? Name { get; init; }
}

/// <summary>
/// ���������
/// </summary>
public record GeoPoint
{
    /// <summary>
    /// γ��
    /// </summary>
    public required decimal Latitude { get; init; }

    /// <summary>
    /// ����
    /// </summary>
    public required decimal Longitude { get; init; }
}

/// <summary>
/// ���䷶Χ
/// </summary>
public record AgeRange
{
    /// <summary>
    /// ��С����
    /// </summary>
    public int MinAge { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public int MaxAge { get; init; }

    /// <summary>
    /// ��������Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(int age) => age >= MinAge && age <= MaxAge;
}

/// <summary>
/// ʱ���
/// </summary>
public record TimeSlot
{
    /// <summary>
    /// ��ʼʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public required TimeOnly StartTime { get; init; }

    /// <summary>
    /// ����ʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public required TimeOnly EndTime { get; init; }

    /// <summary>
    /// ���ʱ���Ƿ���ʱ�����
    /// </summary>
    public bool Contains(TimeOnly time) => time >= StartTime && time <= EndTime;
}

/// <summary>
/// ���ڷ�Χ
/// </summary>
public record DateRange
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    public required DateOnly StartDate { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public required DateOnly EndDate { get; init; }

    /// <summary>
    /// ��������Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(DateOnly date) => date >= StartDate && date <= EndDate;
}



/// <summary>
/// ��������
/// </summary>
public enum TargetingType
{
    /// <summary>
    /// ����
    /// </summary>
    Include = 1,

    /// <summary>
    /// �ų�
    /// </summary>
    Exclude = 2
}