using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �û���Ϊ����ֵ����
/// </summary>
public class UserBehaviorAnalysis : ValueObject
{
    /// <summary>
    /// ��Ծ����
    /// </summary>
    public int ActiveDays { get; private set; }

    /// <summary>
    /// �ܷ��ʴ���
    /// </summary>
    public int TotalSessions { get; private set; }

    /// <summary>
    /// ƽ���Ựʱ�������ӣ�
    /// </summary>
    public double AverageSessionDuration { get; private set; }

    /// <summary>
    /// ����Ծʱ��
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// ��Ծʱ���ƫ��
    /// </summary>
    public IReadOnlyList<int> ActiveHours { get; private set; } = new List<int>();

    /// <summary>
    /// ��Ծ������
    /// </summary>
    public int ActivityScore { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserBehaviorAnalysis() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public UserBehaviorAnalysis(
        int activeDays = 0,
        int totalSessions = 0,
        double averageSessionDuration = 0,
        DateTime? lastActiveTime = null,
        IList<int>? activeHours = null,
        int activityScore = 0)
    {
        ActiveDays = activeDays;
        TotalSessions = totalSessions;
        AverageSessionDuration = averageSessionDuration;
        LastActiveTime = lastActiveTime ?? DateTime.UtcNow;
        ActiveHours = activeHours?.ToList() ?? new List<int>();
        ActivityScore = activityScore;
    }

    /// <summary>
    /// ����Ĭ����Ϊ����
    /// </summary>
    public static UserBehaviorAnalysis CreateDefault()
    {
        return new UserBehaviorAnalysis();
    }

    /// <summary>
    /// ��¼�
    /// </summary>
    public UserBehaviorAnalysis RecordActivity(DateTime activityTime)
    {
        var activeDays = ActiveDays;
        var totalSessions = TotalSessions + 1;
        var lastActiveTime = activityTime;
        
        // ������µ�һ�죬���ӻ�Ծ����
        if (LastActiveTime.Date != activityTime.Date)
        {
            activeDays++;
        }

        // ���»�Ծʱ���
        var activeHours = ActiveHours.ToList();
        var hour = activityTime.Hour;
        if (!activeHours.Contains(hour))
        {
            activeHours.Add(hour);
        }

        // ���¼����Ծ������
        var newActivityScore = CalculateActivityScore(activeDays, totalSessions, AverageSessionDuration);

        return new UserBehaviorAnalysis(
            activeDays: activeDays,
            totalSessions: totalSessions,
            averageSessionDuration: AverageSessionDuration,
            lastActiveTime: lastActiveTime,
            activeHours: activeHours,
            activityScore: newActivityScore);
    }

    /// <summary>
    /// �Ƿ��Ծ�û�
    /// </summary>
    public bool IsActive => ActivityScore >= 50;

    /// <summary>
    /// �Ƿ�߶Ȼ�Ծ�û�
    /// </summary>
    public bool IsHighlyActive => ActivityScore >= 80;

    /// <summary>
    /// �����Ծ������
    /// </summary>
    private static int CalculateActivityScore(int activeDays, int totalSessions, double averageSessionDuration)
    {
        var score = 0;

        // ���ڻ�Ծ����
        if (activeDays >= 30)
            score += 30;
        else if (activeDays >= 7)
            score += 20;
        else if (activeDays >= 3)
            score += 10;

        // �����ܻỰ��
        if (totalSessions >= 100)
            score += 30;
        else if (totalSessions >= 50)
            score += 20;
        else if (totalSessions >= 10)
            score += 10;

        // ����ƽ���Ựʱ��
        if (averageSessionDuration >= 30)
            score += 20;
        else if (averageSessionDuration >= 10)
            score += 15;
        else if (averageSessionDuration >= 5)
            score += 10;

        // ���������Ծ���
        var daysSinceLastActive = (DateTime.UtcNow - DateTime.UtcNow).TotalDays;
        if (daysSinceLastActive <= 1)
            score += 20;
        else if (daysSinceLastActive <= 7)
            score += 10;
        else if (daysSinceLastActive > 30)
            score -= 20;

        return Math.Max(0, Math.Min(100, score));
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ActiveDays;
        yield return TotalSessions;
        yield return AverageSessionDuration;
        yield return LastActiveTime;
        yield return string.Join(",", ActiveHours);
        yield return ActivityScore;
    }
}