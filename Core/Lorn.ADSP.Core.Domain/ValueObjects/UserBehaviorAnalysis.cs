using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 用户行为分析值对象
/// </summary>
public class UserBehaviorAnalysis : ValueObject
{
    /// <summary>
    /// 活跃天数
    /// </summary>
    public int ActiveDays { get; private set; }

    /// <summary>
    /// 总访问次数
    /// </summary>
    public int TotalSessions { get; private set; }

    /// <summary>
    /// 平均会话时长（分钟）
    /// </summary>
    public double AverageSessionDuration { get; private set; }

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// 活跃时间段偏好
    /// </summary>
    public IReadOnlyList<int> ActiveHours { get; private set; } = new List<int>();

    /// <summary>
    /// 活跃度评分
    /// </summary>
    public int ActivityScore { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserBehaviorAnalysis() { }

    /// <summary>
    /// 构造函数
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
    /// 创建默认行为分析
    /// </summary>
    public static UserBehaviorAnalysis CreateDefault()
    {
        return new UserBehaviorAnalysis();
    }

    /// <summary>
    /// 记录活动
    /// </summary>
    public UserBehaviorAnalysis RecordActivity(DateTime activityTime)
    {
        var activeDays = ActiveDays;
        var totalSessions = TotalSessions + 1;
        var lastActiveTime = activityTime;
        
        // 如果是新的一天，增加活跃天数
        if (LastActiveTime.Date != activityTime.Date)
        {
            activeDays++;
        }

        // 更新活跃时间段
        var activeHours = ActiveHours.ToList();
        var hour = activityTime.Hour;
        if (!activeHours.Contains(hour))
        {
            activeHours.Add(hour);
        }

        // 重新计算活跃度评分
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
    /// 是否活跃用户
    /// </summary>
    public bool IsActive => ActivityScore >= 50;

    /// <summary>
    /// 是否高度活跃用户
    /// </summary>
    public bool IsHighlyActive => ActivityScore >= 80;

    /// <summary>
    /// 计算活跃度评分
    /// </summary>
    private static int CalculateActivityScore(int activeDays, int totalSessions, double averageSessionDuration)
    {
        var score = 0;

        // 基于活跃天数
        if (activeDays >= 30)
            score += 30;
        else if (activeDays >= 7)
            score += 20;
        else if (activeDays >= 3)
            score += 10;

        // 基于总会话数
        if (totalSessions >= 100)
            score += 30;
        else if (totalSessions >= 50)
            score += 20;
        else if (totalSessions >= 10)
            score += 10;

        // 基于平均会话时长
        if (averageSessionDuration >= 30)
            score += 20;
        else if (averageSessionDuration >= 10)
            score += 15;
        else if (averageSessionDuration >= 5)
            score += 10;

        // 基于最近活跃情况
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
    /// 获取相等性比较的组件
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