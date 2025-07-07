using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 用户画像实体
/// </summary>
public class UserProfile : AggregateRoot
{
    /// <summary>
    /// 用户ID（外部系统用户标识）
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// 用户基础信息
    /// </summary>
    public UserBasicInfo BasicInfo { get; private set; }

    /// <summary>
    /// 人口统计学信息
    /// </summary>
    public DemographicInfo? DemographicInfo { get; private set; }

    /// <summary>
    /// 地理位置信息
    /// </summary>
    public GeoInfo? GeoInfo { get; private set; }

    /// <summary>
    /// 设备信息
    /// </summary>
    public DeviceInfo? DeviceInfo { get; private set; }

    /// <summary>
    /// 用户偏好设置
    /// </summary>
    public UserPreferences UserPreferences { get; private set; }

    /// <summary>
    /// 隐私设置
    /// </summary>
    public PrivacySettings PrivacySettings { get; private set; }

    /// <summary>
    /// 用户行为分析
    /// </summary>
    public UserBehaviorAnalysis BehaviorAnalysis { get; private set; }

    /// <summary>
    /// 用户兴趣标签
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

    /// <summary>
    /// 用户行为标签
    /// </summary>
    public IReadOnlyList<string> BehaviorTags { get; private set; } = new List<string>();

    /// <summary>
    /// 用户价值评分
    /// </summary>
    public UserValueScore ValueScore { get; private set; }

    /// <summary>
    /// 最后活跃时间
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// 画像数据质量评分
    /// </summary>
    public ProfileQualityScore QualityScore { get; private set; }

    /// <summary>
    /// 自定义属性
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomAttributes { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private UserProfile()
    {
        UserId = string.Empty;
        Status = UserStatus.Active;
        BasicInfo = UserBasicInfo.CreateDefault();
        UserPreferences = UserPreferences.CreateDefault();
        PrivacySettings = PrivacySettings.CreateDefault();
        BehaviorAnalysis = UserBehaviorAnalysis.CreateDefault();
        ValueScore = UserValueScore.CreateDefault();
        LastActiveTime = DateTime.UtcNow;
        QualityScore = ProfileQualityScore.CreateDefault();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserProfile(
        string userId,
        UserBasicInfo basicInfo,
        UserPreferences? userPreferences = null,
        PrivacySettings? privacySettings = null) : this()
    {
        ValidateUserId(userId);
        ValidateBasicInfo(basicInfo);

        UserId = userId;
        BasicInfo = basicInfo;
        UserPreferences = userPreferences ?? UserPreferences.CreateDefault();
        PrivacySettings = privacySettings ?? PrivacySettings.CreateDefault();
        BehaviorAnalysis = UserBehaviorAnalysis.CreateDefault();
        ValueScore = UserValueScore.CreateDefault();
        LastActiveTime = DateTime.UtcNow;
        QualityScore = CalculateQualityScore();

        // 发布用户画像创建事件
        AddDomainEvent(new UserProfileCreatedEvent(Id, UserId));
    }

    /// <summary>
    /// 创建用户画像
    /// </summary>
    public static UserProfile Create(
        string userId,
        UserBasicInfo basicInfo,
        UserPreferences? userPreferences = null,
        PrivacySettings? privacySettings = null)
    {
        return new UserProfile(userId, basicInfo, userPreferences, privacySettings);
    }

    /// <summary>
    /// 创建基础用户画像
    /// </summary>
    public static UserProfile CreateBasic(
        string userId,
        string? displayName = null,
        Gender? gender = null,
        DateTime? dateOfBirth = null)
    {
        var basicInfo = UserBasicInfo.CreateBasic(displayName, gender, dateOfBirth);
        return new UserProfile(userId, basicInfo);
    }

    /// <summary>
    /// 更新基础信息
    /// </summary>
    public void UpdateBasicInfo(UserBasicInfo basicInfo)
    {
        ValidateBasicInfo(basicInfo);

        BasicInfo = basicInfo;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BasicInfo"));
    }

    /// <summary>
    /// 更新人口统计学信息
    /// </summary>
    public void UpdateDemographicInfo(DemographicInfo demographicInfo)
    {
        DemographicInfo = demographicInfo;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "DemographicInfo"));
    }

    /// <summary>
    /// 更新地理位置信息
    /// </summary>
    public void UpdateGeoInfo(GeoInfo geoInfo)
    {
        GeoInfo = geoInfo;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "GeoInfo"));
    }

    /// <summary>
    /// 更新设备信息
    /// </summary>
    public void UpdateDeviceInfo(DeviceInfo deviceInfo)
    {
        DeviceInfo = deviceInfo;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "DeviceInfo"));
    }

    /// <summary>
    /// 更新用户偏好设置
    /// </summary>
    public void UpdateUserPreferences(UserPreferences userPreferences)
    {
        UserPreferences = userPreferences;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "UserPreferences"));
    }

    /// <summary>
    /// 更新隐私设置
    /// </summary>
    public void UpdatePrivacySettings(PrivacySettings privacySettings)
    {
        PrivacySettings = privacySettings;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "PrivacySettings"));
    }

    /// <summary>
    /// 更新用户行为分析
    /// </summary>
    public void UpdateBehaviorAnalysis(UserBehaviorAnalysis behaviorAnalysis)
    {
        BehaviorAnalysis = behaviorAnalysis;
        UpdateLastModifiedTime();
        RecalculateValueScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorAnalysis"));
    }

    /// <summary>
    /// 更新兴趣标签
    /// </summary>
    public void UpdateInterestTags(IList<string> interestTags)
    {
        InterestTags = interestTags.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList();
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "InterestTags"));
    }

    /// <summary>
    /// 添加兴趣标签
    /// </summary>
    public void AddInterestTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("兴趣标签不能为空", nameof(tag));

        var tags = InterestTags.ToList();
        if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            tags.Add(tag);
            InterestTags = tags;
            UpdateLastModifiedTime();
            RecalculateQualityScore();

            AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "InterestTags"));
        }
    }

    /// <summary>
    /// 移除兴趣标签
    /// </summary>
    public void RemoveInterestTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var tags = InterestTags.ToList();
        if (tags.Remove(tag))
        {
            InterestTags = tags;
            UpdateLastModifiedTime();
            RecalculateQualityScore();

            AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "InterestTags"));
        }
    }

    /// <summary>
    /// 更新行为标签
    /// </summary>
    public void UpdateBehaviorTags(IList<string> behaviorTags)
    {
        BehaviorTags = behaviorTags.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList();
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorTags"));
    }

    /// <summary>
    /// 添加行为标签
    /// </summary>
    public void AddBehaviorTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("行为标签不能为空", nameof(tag));

        var tags = BehaviorTags.ToList();
        if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            tags.Add(tag);
            BehaviorTags = tags;
            UpdateLastModifiedTime();
            RecalculateQualityScore();

            AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorTags"));
        }
    }

    /// <summary>
    /// 移除行为标签
    /// </summary>
    public void RemoveBehaviorTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var tags = BehaviorTags.ToList();
        if (tags.Remove(tag))
        {
            BehaviorTags = tags;
            UpdateLastModifiedTime();
            RecalculateQualityScore();

            AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorTags"));
        }
    }

    /// <summary>
    /// 更新自定义属性
    /// </summary>
    public void UpdateCustomAttributes(IDictionary<string, object> customAttributes)
    {
        CustomAttributes = customAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// 设置自定义属性
    /// </summary>
    public void SetCustomAttribute(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("自定义属性键不能为空", nameof(key));

        var attributes = CustomAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        attributes[key] = value;
        CustomAttributes = attributes;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// 获取自定义属性
    /// </summary>
    public T? GetCustomAttribute<T>(string key)
    {
        if (CustomAttributes.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// 记录用户活动
    /// </summary>
    public void RecordActivity(DateTime? activityTime = null)
    {
        var timestamp = activityTime ?? DateTime.UtcNow;
        LastActiveTime = timestamp;

        // 更新行为分析
        BehaviorAnalysis = BehaviorAnalysis.RecordActivity(timestamp);

        UpdateLastModifiedTime();

        AddDomainEvent(new UserActivityRecordedEvent(Id, UserId, timestamp));
    }

    /// <summary>
    /// 激活用户
    /// </summary>
    public void Activate()
    {
        if (Status != UserStatus.Active)
        {
            Status = UserStatus.Active;
            UpdateLastModifiedTime();

            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 停用用户
    /// </summary>
    public void Deactivate()
    {
        if (Status == UserStatus.Active)
        {
            Status = UserStatus.Inactive;
            UpdateLastModifiedTime();

            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 暂停用户
    /// </summary>
    public void Suspend()
    {
        if (Status != UserStatus.Suspended)
        {
            Status = UserStatus.Suspended;
            UpdateLastModifiedTime();

            AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
        }
    }

    /// <summary>
    /// 软删除用户
    /// </summary>
    public void SoftDelete()
    {
        Status = UserStatus.Deleted;
        Delete(); // 调用基类的软删除方法

        AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
    }

    /// <summary>
    /// 是否活跃用户
    /// </summary>
    public bool IsActive => Status == UserStatus.Active && !IsDeleted;

    /// <summary>
    /// 是否高价值用户
    /// </summary>
    public bool IsHighValueUser => ValueScore.OverallScore >= 80;

    /// <summary>
    /// 是否完整画像
    /// </summary>
    public bool IsCompleteProfile => QualityScore.CompletenessScore >= 80;

    /// <summary>
    /// 是否允许个性化广告
    /// </summary>
    public bool AllowPersonalizedAds => UserPreferences.AllowPersonalizedAds &&
                                       PrivacySettings.CanUseForMarketing();

    /// <summary>
    /// 是否允许数据共享
    /// </summary>
    public bool AllowDataSharing => PrivacySettings.CanShareData();

    public IReadOnlyList<string> Keywords { get; private set; } = new List<string>();

    /// <summary>
    /// 获取用户年龄
    /// </summary>
    public int? GetAge()
    {
        if (BasicInfo.DateOfBirth.HasValue)
        {
            var today = DateTime.Today;
            var age = today.Year - BasicInfo.DateOfBirth.Value.Year;
            if (BasicInfo.DateOfBirth.Value.Date > today.AddYears(-age))
                age--;
            return age;
        }
        return null;
    }

    /// <summary>
    /// 获取用户细分群体
    /// </summary>
    public IReadOnlyList<string> GetUserSegments()
    {
        var segments = new List<string>();

        // 基于年龄的细分
        var age = GetAge();
        if (age.HasValue)
        {
            segments.Add(age.Value switch
            {
                < 18 => "青少年",
                >= 18 and < 25 => "年轻成人",
                >= 25 and < 35 => "青年",
                >= 35 and < 50 => "中年",
                >= 50 and < 65 => "中老年",
                >= 65 => "老年"
            });
        }

        // 基于价值评分的细分
        if (ValueScore.OverallScore >= 80)
            segments.Add("高价值用户");
        else if (ValueScore.OverallScore >= 60)
            segments.Add("中价值用户");
        else
            segments.Add("低价值用户");

        // 基于活跃度的细分
        if (BehaviorAnalysis.IsHighlyActive)
            segments.Add("高活跃用户");
        else if (BehaviorAnalysis.IsActive)
            segments.Add("活跃用户");
        else
            segments.Add("非活跃用户");

        // 基于兴趣的细分
        if (InterestTags.Any())
            segments.Add("兴趣明确用户");

        return segments;
    }

    /// <summary>
    /// 计算与指定用户的相似度
    /// </summary>
    public decimal CalculateSimilarity(UserProfile other)
    {
        if (other == null)
            return 0;

        var similarity = 0m;
        var factors = 0;

        // 基础信息相似度
        if (BasicInfo.Gender == other.BasicInfo.Gender && BasicInfo.Gender != Gender.Unknown)
        {
            similarity += 0.1m;
            factors++;
        }

        // 年龄相似度
        var age = GetAge();
        var otherAge = other.GetAge();
        if (age.HasValue && otherAge.HasValue)
        {
            var ageDiff = Math.Abs(age.Value - otherAge.Value);
            similarity += Math.Max(0, 1 - ageDiff / 50m) * 0.2m;
            factors++;
        }

        // 兴趣相似度
        if (InterestTags.Any() && other.InterestTags.Any())
        {
            var commonInterests = InterestTags.Intersect(other.InterestTags, StringComparer.OrdinalIgnoreCase).Count();
            var totalInterests = InterestTags.Union(other.InterestTags, StringComparer.OrdinalIgnoreCase).Count();
            similarity += (decimal)commonInterests / totalInterests * 0.4m;
            factors++;
        }

        // 地理位置相似度
        if (GeoInfo != null && other.GeoInfo != null)
        {
            if (GeoInfo.CountryCode == other.GeoInfo.CountryCode)
            {
                similarity += 0.1m;
                if (GeoInfo.RegionCode == other.GeoInfo.RegionCode)
                {
                    similarity += 0.1m;
                    if (GeoInfo.CityName == other.GeoInfo.CityName)
                    {
                        similarity += 0.1m;
                    }
                }
            }
            factors++;
        }

        return factors > 0 ? similarity / factors : 0;
    }

    /// <summary>
    /// 重新计算质量评分
    /// </summary>
    private void RecalculateQualityScore()
    {
        QualityScore = CalculateQualityScore();

        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "QualityScore", QualityScore.OverallScore));
    }

    /// <summary>
    /// 重新计算价值评分
    /// </summary>
    private void RecalculateValueScore()
    {
        ValueScore = CalculateValueScore();

        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "ValueScore", ValueScore.OverallScore));
    }

    /// <summary>
    /// 计算质量评分
    /// </summary>
    private ProfileQualityScore CalculateQualityScore()
    {
        var completenessScore = CalculateCompletenessScore();
        var accuracyScore = CalculateAccuracyScore();
        var freshnessScore = CalculateFreshnessScore();

        return new ProfileQualityScore(completenessScore, accuracyScore, freshnessScore);
    }

    /// <summary>
    /// 计算完整性评分
    /// </summary>
    private int CalculateCompletenessScore()
    {
        var totalFields = 10;
        var filledFields = 0;

        // 基础信息
        if (!string.IsNullOrWhiteSpace(BasicInfo.DisplayName)) filledFields++;
        if (BasicInfo.Gender != Gender.Unknown) filledFields++;
        if (BasicInfo.DateOfBirth.HasValue) filledFields++;

        // 人口统计学信息
        if (DemographicInfo != null && DemographicInfo.GetCompletenessScore() > 0) filledFields++;

        // 地理位置信息
        if (GeoInfo != null && !string.IsNullOrWhiteSpace(GeoInfo.CountryCode)) filledFields++;

        // 设备信息
        if (DeviceInfo != null && DeviceInfo.DeviceType != DeviceType.Other) filledFields++;

        // 兴趣标签
        if (InterestTags.Any()) filledFields++;

        // 行为标签
        if (BehaviorTags.Any()) filledFields++;

        // 用户偏好
        if (UserPreferences != null) filledFields++;

        // 隐私设置
        if (PrivacySettings != null && PrivacySettings.HasValidConsent()) filledFields++;

        return (int)Math.Round((decimal)filledFields / totalFields * 100);
    }

    /// <summary>
    /// 计算准确性评分
    /// </summary>
    private int CalculateAccuracyScore()
    {
        // 基于数据一致性和验证状态计算
        var score = 100;

        // 年龄合理性检查
        var age = GetAge();
        if (age.HasValue && (age.Value < 0 || age.Value > 150))
            score -= 10;

        // 地理位置一致性检查
        if (GeoInfo != null)
        {
            if (string.IsNullOrWhiteSpace(GeoInfo.CountryCode) && !string.IsNullOrWhiteSpace(GeoInfo.RegionCode))
                score -= 5;
        }

        // 设备信息一致性检查
        if (DeviceInfo != null)
        {
            if (DeviceInfo.DeviceType == DeviceType.Smartphone &&
                !string.IsNullOrWhiteSpace(DeviceInfo.OperatingSystem) &&
                !DeviceInfo.OperatingSystem.Contains("iOS", StringComparison.OrdinalIgnoreCase) &&
                !DeviceInfo.OperatingSystem.Contains("Android", StringComparison.OrdinalIgnoreCase))
                score -= 5;
        }

        return Math.Max(0, score);
    }

    /// <summary>
    /// 计算时效性评分
    /// </summary>
    private int CalculateFreshnessScore()
    {
        var now = DateTime.UtcNow;
        var daysSinceUpdate = (now - LastModifiedTime).TotalDays;

        return daysSinceUpdate switch
        {
            <= 1 => 100,
            <= 7 => 90,
            <= 30 => 80,
            <= 90 => 70,
            <= 180 => 60,
            <= 365 => 50,
            _ => 30
        };
    }

    /// <summary>
    /// 计算价值评分
    /// </summary>
    private UserValueScore CalculateValueScore()
    {
        var engagementScore = CalculateEngagementScore();
        var loyaltyScore = CalculateLoyaltyScore();
        var monetaryScore = CalculateMonetaryScore();
        var potentialScore = CalculatePotentialScore();

        return new UserValueScore(engagementScore, loyaltyScore, monetaryScore, potentialScore);
    }

    /// <summary>
    /// 计算参与度评分
    /// </summary>
    private int CalculateEngagementScore()
    {
        if (BehaviorAnalysis.IsHighlyActive)
            return 90;
        if (BehaviorAnalysis.IsActive)
            return 70;
        return 40;
    }

    /// <summary>
    /// 计算忠诚度评分
    /// </summary>
    private int CalculateLoyaltyScore()
    {
        var daysSinceCreation = (DateTime.UtcNow - CreateTime).TotalDays;
        var daysSinceLastActive = (DateTime.UtcNow - LastActiveTime).TotalDays;

        var loyaltyScore = daysSinceCreation switch
        {
            >= 365 => 90,
            >= 180 => 80,
            >= 90 => 70,
            >= 30 => 60,
            _ => 50
        };

        // 根据最近活跃情况调整
        if (daysSinceLastActive <= 7)
            loyaltyScore += 10;
        else if (daysSinceLastActive > 30)
            loyaltyScore -= 20;

        return Math.Max(0, Math.Min(100, loyaltyScore));
    }

    /// <summary>
    /// 计算货币价值评分
    /// </summary>
    private int CalculateMonetaryScore()
    {
        // 这里可以基于用户的消费能力、收入水平等计算
        // 目前基于人口统计学信息进行简单估算
        if (DemographicInfo?.IncomeLevel?.Contains("高", StringComparison.OrdinalIgnoreCase) == true)
            return 80;
        if (DemographicInfo?.IncomeLevel?.Contains("中", StringComparison.OrdinalIgnoreCase) == true)
            return 60;
        if (DemographicInfo?.IncomeLevel?.Contains("低", StringComparison.OrdinalIgnoreCase) == true)
            return 40;

        return 50; // 默认值
    }

    /// <summary>
    /// 计算潜力评分
    /// </summary>
    private int CalculatePotentialScore()
    {
        var score = 50; // 基础分

        // 基于年龄的潜力评估
        var age = GetAge();
        if (age.HasValue)
        {
            score += age.Value switch
            {
                >= 18 and < 35 => 20, // 年轻用户潜力更大
                >= 35 and < 50 => 15,
                >= 50 and < 65 => 10,
                _ => 5
            };
        }

        // 基于兴趣多样性的潜力评估
        if (InterestTags.Count > 5)
            score += 15;
        else if (InterestTags.Count > 2)
            score += 10;

        // 基于设备类型的潜力评估
        if (DeviceInfo?.DeviceType == DeviceType.Smartphone)
            score += 10;

        return Math.Min(100, score);
    }

    /// <summary>
    /// 验证用户ID
    /// </summary>
    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("用户ID不能为空", nameof(userId));

        if (userId.Length > 255)
            throw new ArgumentException("用户ID长度不能超过255个字符", nameof(userId));
    }

    /// <summary>
    /// 验证基础信息
    /// </summary>
    private static void ValidateBasicInfo(UserBasicInfo basicInfo)
    {
        if (basicInfo == null)
            throw new ArgumentNullException(nameof(basicInfo));
    }
}