namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// �û���Ϊ����������
/// �̳���TargetingContextBase���ṩ�û���Ϊ���ݵĶ��������Ĺ���
/// �ϲ���ԭUserBehavior��UserBehaviorAnalysis�Ĺ��ܣ�רע�ڹ�涨���е���Ϊ����
/// </summary>
public class UserBehavior : TargetingContextBase
{
    /// <summary>
    /// ��Ȥ��ǩ
    /// </summary>
    public IReadOnlyList<string> InterestTags => GetPropertyValue<List<string>>("InterestTags") ?? new List<string>();

    /// <summary>
    /// ��Ϊ���ͱ�ǩ
    /// </summary>
    public IReadOnlyList<string> BehaviorTags => GetPropertyValue<List<string>>("BehaviorTags") ?? new List<string>();

    /// <summary>
    /// ��Ծ����
    /// </summary>
    public int ActiveDays => GetPropertyValue("ActiveDays", 0);

    /// <summary>
    /// �ܷ��ʴ���
    /// </summary>
    public int TotalSessions => GetPropertyValue("TotalSessions", 0);

    /// <summary>
    /// ƽ���Ựʱ�������ӣ�
    /// </summary>
    public double AverageSessionDuration => GetPropertyValue("AverageSessionDuration", 0.0);

    /// <summary>
    /// ����Ծʱ��
    /// </summary>
    public DateTime LastActiveTime => GetPropertyValue("LastActiveTime", DateTime.UtcNow);

    /// <summary>
    /// ��Ծʱ��ƫ�ã�24Сʱ�ƣ�
    /// </summary>
    public IReadOnlyList<int> ActiveHours => GetPropertyValue<List<int>>("ActiveHours") ?? new List<int>();

    /// <summary>
    /// ��Ծ������ (0-100)
    /// </summary>
    public int ActivityScore => GetPropertyValue("ActivityScore", 0);

    /// <summary>
    /// ���������������
    /// </summary>
    public IReadOnlyList<string> RecentContentCategories => GetPropertyValue<List<string>>("RecentContentCategories") ?? new List<string>();

    /// <summary>
    /// ����ת����Ϊ��¼
    /// </summary>
    public IReadOnlyList<string> ConversionActions => GetPropertyValue<List<string>>("ConversionActions") ?? new List<string>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserBehavior() : base("UserBehavior") { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public UserBehavior(
        IList<string>? interestTags = null,
        IList<string>? behaviorTags = null,
        int activeDays = 0,
        int totalSessions = 0,
        double averageSessionDuration = 0.0,
        DateTime? lastActiveTime = null,
        IList<int>? activeHours = null,
        int activityScore = 0,
        IList<string>? recentContentCategories = null,
        IList<string>? conversionActions = null,
        string? dataSource = null)
        : base("UserBehavior", CreateProperties(interestTags, behaviorTags, activeDays, totalSessions, averageSessionDuration, lastActiveTime, activeHours, activityScore, recentContentCategories, conversionActions), dataSource)
    {
    }

    /// <summary>
    /// ���������ֵ�
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        IList<string>? interestTags,
        IList<string>? behaviorTags,
        int activeDays,
        int totalSessions,
        double averageSessionDuration,
        DateTime? lastActiveTime,
        IList<int>? activeHours,
        int activityScore,
        IList<string>? recentContentCategories,
        IList<string>? conversionActions)
    {
        var properties = new Dictionary<string, object>
        {
            ["ActiveDays"] = activeDays,
            ["TotalSessions"] = totalSessions,
            ["AverageSessionDuration"] = averageSessionDuration,
            ["LastActiveTime"] = lastActiveTime ?? DateTime.UtcNow,
            ["ActivityScore"] = Math.Max(0, Math.Min(100, activityScore))
        };

        if (interestTags != null && interestTags.Any())
            properties["InterestTags"] = interestTags.ToList();

        if (behaviorTags != null && behaviorTags.Any())
            properties["BehaviorTags"] = behaviorTags.ToList();

        if (activeHours != null && activeHours.Any())
            properties["ActiveHours"] = activeHours.ToList();

        if (recentContentCategories != null && recentContentCategories.Any())
            properties["RecentContentCategories"] = recentContentCategories.ToList();

        if (conversionActions != null && conversionActions.Any())
            properties["ConversionActions"] = conversionActions.ToList();

        return properties;
    }

    /// <summary>
    /// ����Ĭ����Ϊ������
    /// </summary>
    public static UserBehavior CreateDefault(string? dataSource = null)
    {
        return new UserBehavior(dataSource: dataSource);
    }

    /// <summary>
    /// ������Ծ�û���Ϊ������
    /// </summary>
    public static UserBehavior CreateActiveUser(
        IList<string>? interestTags = null,
        int activeDays = 30,
        int totalSessions = 50,
        double averageSessionDuration = 10.0,
        string? dataSource = null)
    {
        var activityScore = CalculateActivityScore(activeDays, totalSessions, averageSessionDuration, DateTime.UtcNow);
        return new UserBehavior(
            interestTags: interestTags,
            activeDays: activeDays,
            totalSessions: totalSessions,
            averageSessionDuration: averageSessionDuration,
            activityScore: activityScore,
            dataSource: dataSource);
    }

    /// <summary>
    /// �Ƿ�Ϊ��Ծ�û�
    /// </summary>
    public bool IsActiveUser => ActivityScore >= 50;

    /// <summary>
    /// �Ƿ�Ϊ�߶Ȼ�Ծ�û�
    /// </summary>
    public bool IsHighlyActiveUser => ActivityScore >= 80;

    /// <summary>
    /// �Ƿ����ָ����Ȥ��ǩ
    /// </summary>
    public bool HasInterestTag(string tag)
    {
        return InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ����ָ����Ϊ��ǩ
    /// </summary>
    public bool HasBehaviorTag(string tag)
    {
        return BehaviorTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ���ָ��ʱ�λ�Ծ
    /// </summary>
    public bool IsActiveInHour(int hour)
    {
        return ActiveHours.Contains(hour);
    }

    /// <summary>
    /// ��ȡ��ָ����Ȥ��ǩ��ƥ���
    /// </summary>
    public decimal GetInterestMatchScore(IEnumerable<string> targetInterests)
    {
        if (!targetInterests.Any() || !InterestTags.Any())
            return 0.0m;

        var matchCount = targetInterests.Count(interest =>
            InterestTags.Contains(interest, StringComparer.OrdinalIgnoreCase));

        return (decimal)matchCount / targetInterests.Count();
    }

    /// <summary>
    /// ��ȡ��Ϊ��Ծ�ȵȼ�
    /// </summary>
    public string GetActivityLevel()
    {
        return ActivityScore switch
        {
            >= 80 => "�߶Ȼ�Ծ",
            >= 50 => "��Ծ",
            >= 20 => "һ��",
            _ => "����Ծ"
        };
    }

    /// <summary>
    /// �����Ծ������
    /// </summary>
    private static int CalculateActivityScore(int activeDays, int totalSessions, double averageSessionDuration, DateTime lastActiveTime)
    {
        var score = 0;

        // ���ڻ�Ծ����
        if (activeDays >= 30) score += 30;
        else if (activeDays >= 7) score += 20;
        else if (activeDays >= 3) score += 10;

        // �����ܻỰ��
        if (totalSessions >= 100) score += 30;
        else if (totalSessions >= 50) score += 20;
        else if (totalSessions >= 10) score += 10;

        // ����ƽ���Ựʱ��
        if (averageSessionDuration >= 30) score += 20;
        else if (averageSessionDuration >= 10) score += 15;
        else if (averageSessionDuration >= 5) score += 10;

        // ���������Ծ��
        var daysSinceLastActive = (DateTime.UtcNow - lastActiveTime).TotalDays;
        if (daysSinceLastActive <= 1) score += 20;
        else if (daysSinceLastActive <= 7) score += 10;
        else if (daysSinceLastActive > 30) score -= 20;

        return Math.Max(0, Math.Min(100, score));
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var behaviorInfo = $"Activity:{GetActivityLevel()} Sessions:{TotalSessions} Interests:{InterestTags.Count} Behaviors:{BehaviorTags.Count}";
        return $"{baseInfo} | {behaviorInfo}";
    }

    /// <summary>
    /// ��֤�����ĵ���Ч��
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // ��֤���ַ�Χ
        if (ActivityScore < 0 || ActivityScore > 100)
            return false;

        // ��֤ʱ�η�Χ
        if (ActiveHours.Any(hour => hour < 0 || hour > 23))
            return false;

        return true;
    }
}
