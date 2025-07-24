namespace Lorn.ADSP.Core.Domain.Targeting;

/// <summary>
/// 用户行为上下文
/// 继承自TargetingContextBase，提供用户行为数据的定向上下文功能
/// 合并了原UserBehavior实体和UserBehaviorAnalysis的功能，专注于广告定向中的行为数据
/// </summary>
public class UserBehavior : TargetingContextBase
{
    /// <summary>
    /// 上下文名称
    /// </summary>
    public override string ContextName => "用户行为上下文";

    /// <summary>
    /// 兴趣标签
    /// </summary>
    public IReadOnlyList<string> InterestTags => GetPropertyValue<List<string>>("InterestTags") ?? new List<string>();

    /// <summary>
    /// 行为类型标签
    /// </summary>
    public IReadOnlyList<string> BehaviorTags => GetPropertyValue<List<string>>("BehaviorTags") ?? new List<string>();

    /// <summary>
    /// 活跃天数
    /// </summary>
    public int ActiveDays => GetPropertyValue("ActiveDays", 0);

    /// <summary>
    /// 总访问次数
    /// </summary>
    public int TotalSessions => GetPropertyValue("TotalSessions", 0);

    /// <summary>
    /// 平均会话时长（分钟）
    /// </summary>
    public double AverageSessionDuration => GetPropertyValue("AverageSessionDuration", 0.0);

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime => GetPropertyValue("LastActiveTime", DateTime.UtcNow);

    /// <summary>
    /// 活跃时间偏好（24小时制）
    /// </summary>
    public IReadOnlyList<int> ActiveHours => GetPropertyValue<List<int>>("ActiveHours") ?? new List<int>();

    /// <summary>
    /// 活跃评分 (0-100)
    /// </summary>
    public int ActivityScore => GetPropertyValue("ActivityScore", 0);

    /// <summary>
    /// 最近浏览内容类别
    /// </summary>
    public IReadOnlyList<string> RecentContentCategories => GetPropertyValue<List<string>>("RecentContentCategories") ?? new List<string>();

    /// <summary>
    /// 用户转化行为记录
    /// </summary>
    public IReadOnlyList<string> ConversionActions => GetPropertyValue<List<string>>("ConversionActions") ?? new List<string>();

    // 从原UserBehavior实体合并的属性

    /// <summary>
    /// 行为类型（如：Click, View, Purchase等）- 从原实体合并
    /// </summary>
    public string? BehaviorType => GetPropertyValue<string>("BehaviorType");

    /// <summary>
    /// 行为值或描述 - 从原实体合并
    /// </summary>
    public string? BehaviorValue => GetPropertyValue<string>("BehaviorValue");

    /// <summary>
    /// 行为发生时间戳 - 从原实体合并
    /// </summary>
    public DateTime? BehaviorTimestamp => GetPropertyValue<DateTime?>("BehaviorTimestamp");

    /// <summary>
    /// 行为频次 - 从原实体合并
    /// </summary>
    public int Frequency => GetPropertyValue("Frequency", 1);

    /// <summary>
    /// 行为权重 - 从原实体合并
    /// </summary>
    public decimal Weight => GetPropertyValue("Weight", 1.0m);

    /// <summary>
    /// 行为上下文信息（JSON格式）- 从原实体合并
    /// </summary>
    public string? BehaviorContext => GetPropertyValue<string>("BehaviorContext");

    /// <summary>
    /// 行为记录集合 - 存储多个行为记录
    /// </summary>
    public IReadOnlyList<BehaviorRecord> BehaviorRecords =>
        GetPropertyValue<List<BehaviorRecord>>("BehaviorRecords") ?? new List<BehaviorRecord>();

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
        string? behaviorType = null,
        string? behaviorValue = null,
        DateTime? behaviorTimestamp = null,
        int frequency = 1,
        decimal weight = 1.0m,
        string? behaviorContext = null,
        IList<BehaviorRecord>? behaviorRecords = null,
        string? dataSource = null)
        : base("UserBehavior", CreateProperties(interestTags, behaviorTags, activeDays, totalSessions, averageSessionDuration, lastActiveTime, activeHours, activityScore, recentContentCategories, conversionActions, behaviorType, behaviorValue, behaviorTimestamp, frequency, weight, behaviorContext, behaviorRecords), dataSource)
    {
        ValidateInput(activeDays, totalSessions, averageSessionDuration, activityScore, frequency, weight);
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
        IList<string>? conversionActions,
        string? behaviorType,
        string? behaviorValue,
        DateTime? behaviorTimestamp,
        int frequency,
        decimal weight,
        string? behaviorContext,
        IList<BehaviorRecord>? behaviorRecords)
    {
        var properties = new Dictionary<string, object>
        {
            ["ActiveDays"] = Math.Max(0, activeDays),
            ["TotalSessions"] = Math.Max(0, totalSessions),
            ["AverageSessionDuration"] = Math.Max(0, averageSessionDuration),
            ["LastActiveTime"] = lastActiveTime ?? DateTime.UtcNow,
            ["ActivityScore"] = Math.Max(0, Math.Min(100, activityScore)),
            ["Frequency"] = Math.Max(1, frequency),
            ["Weight"] = Math.Max(0, weight)
        };

        if (interestTags != null && interestTags.Any())
            properties["InterestTags"] = interestTags.ToList();

        if (behaviorTags != null && behaviorTags.Any())
            properties["BehaviorTags"] = behaviorTags.ToList();

        if (activeHours != null && activeHours.Any())
            properties["ActiveHours"] = activeHours.Where(h => h >= 0 && h <= 23).ToList();

        if (recentContentCategories != null && recentContentCategories.Any())
            properties["RecentContentCategories"] = recentContentCategories.ToList();

        if (conversionActions != null && conversionActions.Any())
            properties["ConversionActions"] = conversionActions.ToList();

        if (!string.IsNullOrWhiteSpace(behaviorType))
            properties["BehaviorType"] = behaviorType;

        if (!string.IsNullOrWhiteSpace(behaviorValue))
            properties["BehaviorValue"] = behaviorValue;

        if (behaviorTimestamp.HasValue)
            properties["BehaviorTimestamp"] = behaviorTimestamp.Value;

        if (!string.IsNullOrWhiteSpace(behaviorContext))
            properties["BehaviorContext"] = behaviorContext;

        if (behaviorRecords != null && behaviorRecords.Any())
            properties["BehaviorRecords"] = behaviorRecords.ToList();

        return properties;
    }

    /// <summary>
    /// 创建默认用户行为
    /// </summary>
    public static UserBehavior CreateDefault(string? dataSource = null)
    {
        return new UserBehavior(
            activeDays: 1,
            totalSessions: 1,
            averageSessionDuration: 5.0,
            activityScore: 30,
            dataSource: dataSource);
    }

    /// <summary>
    /// 创建活跃用户行为
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
    /// 从原UserBehavior实体数据创建（兼容方法）
    /// </summary>
    public static UserBehavior FromUserBehaviorEntity(
        string behaviorType,
        string behaviorValue,
        DateTime timestamp,
        int frequency = 1,
        decimal weight = 1.0m,
        string? context = null,
        string? dataSource = null)
    {
        var behaviorRecord = new BehaviorRecord(behaviorType, behaviorValue, timestamp, frequency, weight, context);
        return new UserBehavior(
            behaviorType: behaviorType,
            behaviorValue: behaviorValue,
            behaviorTimestamp: timestamp,
            frequency: frequency,
            weight: weight,
            behaviorContext: context,
            behaviorRecords: new List<BehaviorRecord> { behaviorRecord },
            dataSource: dataSource);
    }

    /// <summary>
    /// 添加行为记录
    /// </summary>
    public UserBehavior AddBehaviorRecord(string behaviorType, string behaviorValue, DateTime? timestamp = null, int frequency = 1, decimal weight = 1.0m, string? context = null)
    {
        var newRecord = new BehaviorRecord(behaviorType, behaviorValue, timestamp ?? DateTime.UtcNow, frequency, weight, context);
        var currentRecords = BehaviorRecords.ToList();
        currentRecords.Add(newRecord);

        // 更新统计信息
        var newTotalSessions = TotalSessions + frequency;
        var newActivityScore = Math.Min(100, ActivityScore + (frequency * 2));

        return new UserBehavior(
            interestTags: InterestTags.ToList(),
            behaviorTags: BehaviorTags.ToList(),
            activeDays: ActiveDays,
            totalSessions: newTotalSessions,
            averageSessionDuration: AverageSessionDuration,
            lastActiveTime: timestamp ?? DateTime.UtcNow,
            activeHours: ActiveHours.ToList(),
            activityScore: newActivityScore,
            recentContentCategories: RecentContentCategories.ToList(),
            conversionActions: ConversionActions.ToList(),
            behaviorRecords: currentRecords,
            dataSource: DataSource);
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
        return hour >= 0 && hour <= 23 && ActiveHours.Contains(hour);
    }

    /// <summary>
    /// 是否有指定类型的行为记录
    /// </summary>
    public bool HasBehaviorType(string behaviorType)
    {
        return BehaviorRecords.Any(r => string.Equals(r.BehaviorType, behaviorType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取指定兴趣标签的匹配度
    /// </summary>
    public decimal GetInterestMatchScore(IEnumerable<string> targetInterests)
    {
        if (!targetInterests.Any() || !InterestTags.Any())
            return 0m;

        var matchingTags = InterestTags.Intersect(targetInterests, StringComparer.OrdinalIgnoreCase).Count();
        return (decimal)matchingTags / targetInterests.Count();
    }

    /// <summary>
    /// 获取行为活跃度等级
    /// </summary>
    public string GetActivityLevel()
    {
        return ActivityScore switch
        {
            >= 80 => "Very Active",
            >= 60 => "Active",
            >= 40 => "Moderate",
            >= 20 => "Low",
            _ => "Inactive"
        };
    }

    /// <summary>
    /// 获取指定类型的行为记录
    /// </summary>
    public IReadOnlyList<BehaviorRecord> GetBehaviorsByType(string behaviorType)
    {
        return BehaviorRecords.Where(r => string.Equals(r.BehaviorType, behaviorType, StringComparison.OrdinalIgnoreCase))
                             .ToList()
                             .AsReadOnly();
    }

    /// <summary>
    /// 获取最近的行为记录
    /// </summary>
    public IReadOnlyList<BehaviorRecord> GetRecentBehaviors(int count = 10)
    {
        return BehaviorRecords.OrderByDescending(r => r.Timestamp)
                             .Take(count)
                             .ToList()
                             .AsReadOnly();
    }

    /// <summary>
    /// 计算活跃度评分
    /// </summary>
    private static int CalculateActivityScore(int activeDays, int totalSessions, double averageSessionDuration, DateTime lastActiveTime)
    {
        var score = 0;

        // 活跃天数评分 (30%)
        if (activeDays >= 30) score += 30;
        else if (activeDays >= 7) score += 20;
        else if (activeDays >= 3) score += 10;

        // 会话次数评分 (30%)
        if (totalSessions >= 100) score += 30;
        else if (totalSessions >= 50) score += 20;
        else if (totalSessions >= 10) score += 10;

        // 平均会话时长评分 (20%)
        if (averageSessionDuration >= 30) score += 20;
        else if (averageSessionDuration >= 10) score += 15;
        else if (averageSessionDuration >= 5) score += 10;

        // 最近活跃度评分 (20%)
        var daysSinceLastActive = (DateTime.UtcNow - lastActiveTime).Days;
        if (daysSinceLastActive <= 1) score += 20;
        else if (daysSinceLastActive <= 7) score += 15;
        else if (daysSinceLastActive > 30) score -= 10;

        return Math.Max(0, Math.Min(100, score));
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(int activeDays, int totalSessions, double averageSessionDuration, int activityScore, int frequency, decimal weight)
    {
        if (activeDays < 0)
            throw new ArgumentException("活跃天数不能为负数", nameof(activeDays));

        if (totalSessions < 0)
            throw new ArgumentException("总会话数不能为负数", nameof(totalSessions));

        if (averageSessionDuration < 0)
            throw new ArgumentException("平均会话时长不能为负数", nameof(averageSessionDuration));

        if (activityScore < 0 || activityScore > 100)
            throw new ArgumentException("活跃度评分必须在0-100之间", nameof(activityScore));

        if (frequency < 1)
            throw new ArgumentException("频次必须大于0", nameof(frequency));

        if (weight < 0)
            throw new ArgumentException("权重不能为负数", nameof(weight));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var level = GetActivityLevel();
        return $"UserBehavior: ActivityScore={ActivityScore} ({level}), Sessions={TotalSessions}, ActiveDays={ActiveDays}, BehaviorRecords={BehaviorRecords.Count}, InterestTags={InterestTags.Count}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        if (ActivityScore < 0 || ActivityScore > 100)
            return false;

        if (ActiveHours.Any(hour => hour < 0 || hour > 23))
            return false;

        if (TotalSessions < 0 || ActiveDays < 0 || AverageSessionDuration < 0)
            return false;

        if (Frequency < 1 || Weight < 0)
            return false;

        return true;
    }
}

/// <summary>
/// 行为记录值对象（从原UserBehavior实体合并）
/// </summary>
public record BehaviorRecord(
    string BehaviorType,
    string BehaviorValue,
    DateTime Timestamp,
    int Frequency = 1,
    decimal Weight = 1.0m,
    string? Context = null)
{
    /// <summary>
    /// 是否为转化行为
    /// </summary>
    public bool IsConversionBehavior => BehaviorType.ToLower().Contains("conversion") ||
                                       BehaviorType.ToLower().Contains("purchase") ||
                                       BehaviorType.ToLower().Contains("signup");

    /// <summary>
    /// 是否为最近行为（7天内）
    /// </summary>
    public bool IsRecentBehavior => (DateTime.UtcNow - Timestamp).TotalDays <= 7;

    /// <summary>
    /// 获取行为重要性评分
    /// </summary>
    public decimal GetImportanceScore()
    {
        var baseScore = Weight * Frequency;

        if (IsConversionBehavior)
            baseScore *= 2;

        if (IsRecentBehavior)
            baseScore *= 1.5m;

        return Math.Min(10m, baseScore);
    }
}
