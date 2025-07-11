namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// 用户行为定向上下文
/// 继承自TargetingContextBase，提供用户行为数据的定向上下文功能
/// 合并了原UserBehavior和UserBehaviorAnalysis的功能，专注于广告定向中的行为分析
/// </summary>
public class UserBehavior : TargetingContextBase
{
    /// <summary>
    /// 兴趣标签
    /// </summary>
    public IReadOnlyList<string> InterestTags => GetProperty<List<string>>("InterestTags") ?? new List<string>();

    /// <summary>
    /// 行为类型标签
    /// </summary>
    public IReadOnlyList<string> BehaviorTags => GetProperty<List<string>>("BehaviorTags") ?? new List<string>();

    /// <summary>
    /// 活跃天数
    /// </summary>
    public int ActiveDays => GetProperty("ActiveDays", 0);

    /// <summary>
    /// 总访问次数
    /// </summary>
    public int TotalSessions => GetProperty("TotalSessions", 0);

    /// <summary>
    /// 平均会话时长（分钟）
    /// </summary>
    public double AverageSessionDuration => GetProperty("AverageSessionDuration", 0.0);

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime => GetProperty("LastActiveTime", DateTime.UtcNow);

    /// <summary>
    /// 活跃时段偏好（24小时制）
    /// </summary>
    public IReadOnlyList<int> ActiveHours => GetProperty<List<int>>("ActiveHours") ?? new List<int>();

    /// <summary>
    /// 活跃度评分 (0-100)
    /// </summary>
    public int ActivityScore => GetProperty("ActivityScore", 0);

    /// <summary>
    /// 最近浏览的内容类别
    /// </summary>
    public IReadOnlyList<string> RecentContentCategories => GetProperty<List<string>>("RecentContentCategories") ?? new List<string>();

    /// <summary>
    /// 购买转化行为记录
    /// </summary>
    public IReadOnlyList<string> ConversionActions => GetProperty<List<string>>("ConversionActions") ?? new List<string>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserBehavior() : base("UserBehavior") { }

    /// <summary>
    /// 构造函数
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
    /// 创建属性字典
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
    /// 创建默认行为上下文
    /// </summary>
    public static UserBehavior CreateDefault(string? dataSource = null)
    {
        return new UserBehavior(dataSource: dataSource);
    }

    /// <summary>
    /// 创建活跃用户行为上下文
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
    /// 是否为活跃用户
    /// </summary>
    public bool IsActiveUser => ActivityScore >= 50;

    /// <summary>
    /// 是否为高度活跃用户
    /// </summary>
    public bool IsHighlyActiveUser => ActivityScore >= 80;

    /// <summary>
    /// 是否包含指定兴趣标签
    /// </summary>
    public bool HasInterestTag(string tag)
    {
        return InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否包含指定行为标签
    /// </summary>
    public bool HasBehaviorTag(string tag)
    {
        return BehaviorTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否在指定时段活跃
    /// </summary>
    public bool IsActiveInHour(int hour)
    {
        return ActiveHours.Contains(hour);
    }

    /// <summary>
    /// 获取与指定兴趣标签的匹配度
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
    /// 获取行为活跃度等级
    /// </summary>
    public string GetActivityLevel()
    {
        return ActivityScore switch
        {
            >= 80 => "高度活跃",
            >= 50 => "活跃",
            >= 20 => "一般",
            _ => "不活跃"
        };
    }

    /// <summary>
    /// 计算活跃度评分
    /// </summary>
    private static int CalculateActivityScore(int activeDays, int totalSessions, double averageSessionDuration, DateTime lastActiveTime)
    {
        var score = 0;

        // 基于活跃天数
        if (activeDays >= 30) score += 30;
        else if (activeDays >= 7) score += 20;
        else if (activeDays >= 3) score += 10;

        // 基于总会话数
        if (totalSessions >= 100) score += 30;
        else if (totalSessions >= 50) score += 20;
        else if (totalSessions >= 10) score += 10;

        // 基于平均会话时长
        if (averageSessionDuration >= 30) score += 20;
        else if (averageSessionDuration >= 10) score += 15;
        else if (averageSessionDuration >= 5) score += 10;

        // 基于最近活跃度
        var daysSinceLastActive = (DateTime.UtcNow - lastActiveTime).TotalDays;
        if (daysSinceLastActive <= 1) score += 20;
        else if (daysSinceLastActive <= 7) score += 10;
        else if (daysSinceLastActive > 30) score -= 20;

        return Math.Max(0, Math.Min(100, score));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var behaviorInfo = $"Activity:{GetActivityLevel()} Sessions:{TotalSessions} Interests:{InterestTags.Count} Behaviors:{BehaviorTags.Count}";
        return $"{baseInfo} | {behaviorInfo}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // 验证评分范围
        if (ActivityScore < 0 || ActivityScore > 100)
            return false;

        // 验证时段范围
        if (ActiveHours.Any(hour => hour < 0 || hour > 23))
            return false;

        return true;
    }
}