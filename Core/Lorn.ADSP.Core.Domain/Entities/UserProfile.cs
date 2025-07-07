using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// �û�����ʵ��
/// </summary>
public class UserProfile : AggregateRoot
{
    /// <summary>
    /// �û�ID���ⲿϵͳ�û���ʶ��
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// �û�״̬
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// �û�������Ϣ
    /// </summary>
    public UserBasicInfo BasicInfo { get; private set; }

    /// <summary>
    /// �˿�ͳ��ѧ��Ϣ
    /// </summary>
    public DemographicInfo? DemographicInfo { get; private set; }

    /// <summary>
    /// ����λ����Ϣ
    /// </summary>
    public GeoInfo? GeoInfo { get; private set; }

    /// <summary>
    /// �豸��Ϣ
    /// </summary>
    public DeviceInfo? DeviceInfo { get; private set; }

    /// <summary>
    /// �û�ƫ������
    /// </summary>
    public UserPreferences UserPreferences { get; private set; }

    /// <summary>
    /// ��˽����
    /// </summary>
    public PrivacySettings PrivacySettings { get; private set; }

    /// <summary>
    /// �û���Ϊ����
    /// </summary>
    public UserBehaviorAnalysis BehaviorAnalysis { get; private set; }

    /// <summary>
    /// �û���Ȥ��ǩ
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

    /// <summary>
    /// �û���Ϊ��ǩ
    /// </summary>
    public IReadOnlyList<string> BehaviorTags { get; private set; } = new List<string>();

    /// <summary>
    /// �û���ֵ����
    /// </summary>
    public UserValueScore ValueScore { get; private set; }

    /// <summary>
    /// ����Ծʱ��
    /// </summary>
    public DateTime LastActiveTime { get; private set; }

    /// <summary>
    /// ����������������
    /// </summary>
    public ProfileQualityScore QualityScore { get; private set; }

    /// <summary>
    /// �Զ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomAttributes { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// ˽�й��캯��������EF Core��
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
    /// ���캯��
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

        // �����û����񴴽��¼�
        AddDomainEvent(new UserProfileCreatedEvent(Id, UserId));
    }

    /// <summary>
    /// �����û�����
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
    /// ���������û�����
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
    /// ���»�����Ϣ
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
    /// �����˿�ͳ��ѧ��Ϣ
    /// </summary>
    public void UpdateDemographicInfo(DemographicInfo demographicInfo)
    {
        DemographicInfo = demographicInfo;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "DemographicInfo"));
    }

    /// <summary>
    /// ���µ���λ����Ϣ
    /// </summary>
    public void UpdateGeoInfo(GeoInfo geoInfo)
    {
        GeoInfo = geoInfo;
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "GeoInfo"));
    }

    /// <summary>
    /// �����豸��Ϣ
    /// </summary>
    public void UpdateDeviceInfo(DeviceInfo deviceInfo)
    {
        DeviceInfo = deviceInfo;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "DeviceInfo"));
    }

    /// <summary>
    /// �����û�ƫ������
    /// </summary>
    public void UpdateUserPreferences(UserPreferences userPreferences)
    {
        UserPreferences = userPreferences;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "UserPreferences"));
    }

    /// <summary>
    /// ������˽����
    /// </summary>
    public void UpdatePrivacySettings(PrivacySettings privacySettings)
    {
        PrivacySettings = privacySettings;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "PrivacySettings"));
    }

    /// <summary>
    /// �����û���Ϊ����
    /// </summary>
    public void UpdateBehaviorAnalysis(UserBehaviorAnalysis behaviorAnalysis)
    {
        BehaviorAnalysis = behaviorAnalysis;
        UpdateLastModifiedTime();
        RecalculateValueScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorAnalysis"));
    }

    /// <summary>
    /// ������Ȥ��ǩ
    /// </summary>
    public void UpdateInterestTags(IList<string> interestTags)
    {
        InterestTags = interestTags.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList();
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "InterestTags"));
    }

    /// <summary>
    /// �����Ȥ��ǩ
    /// </summary>
    public void AddInterestTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("��Ȥ��ǩ����Ϊ��", nameof(tag));

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
    /// �Ƴ���Ȥ��ǩ
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
    /// ������Ϊ��ǩ
    /// </summary>
    public void UpdateBehaviorTags(IList<string> behaviorTags)
    {
        BehaviorTags = behaviorTags.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList();
        UpdateLastModifiedTime();
        RecalculateQualityScore();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "BehaviorTags"));
    }

    /// <summary>
    /// �����Ϊ��ǩ
    /// </summary>
    public void AddBehaviorTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("��Ϊ��ǩ����Ϊ��", nameof(tag));

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
    /// �Ƴ���Ϊ��ǩ
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
    /// �����Զ�������
    /// </summary>
    public void UpdateCustomAttributes(IDictionary<string, object> customAttributes)
    {
        CustomAttributes = customAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// �����Զ�������
    /// </summary>
    public void SetCustomAttribute(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("�Զ������Լ�����Ϊ��", nameof(key));

        var attributes = CustomAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        attributes[key] = value;
        CustomAttributes = attributes;
        UpdateLastModifiedTime();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId, "CustomAttributes"));
    }

    /// <summary>
    /// ��ȡ�Զ�������
    /// </summary>
    public T? GetCustomAttribute<T>(string key)
    {
        if (CustomAttributes.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// ��¼�û��
    /// </summary>
    public void RecordActivity(DateTime? activityTime = null)
    {
        var timestamp = activityTime ?? DateTime.UtcNow;
        LastActiveTime = timestamp;

        // ������Ϊ����
        BehaviorAnalysis = BehaviorAnalysis.RecordActivity(timestamp);

        UpdateLastModifiedTime();

        AddDomainEvent(new UserActivityRecordedEvent(Id, UserId, timestamp));
    }

    /// <summary>
    /// �����û�
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
    /// ͣ���û�
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
    /// ��ͣ�û�
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
    /// ��ɾ���û�
    /// </summary>
    public void SoftDelete()
    {
        Status = UserStatus.Deleted;
        Delete(); // ���û������ɾ������

        AddDomainEvent(new UserProfileStatusChangedEvent(Id, UserId, Status));
    }

    /// <summary>
    /// �Ƿ��Ծ�û�
    /// </summary>
    public bool IsActive => Status == UserStatus.Active && !IsDeleted;

    /// <summary>
    /// �Ƿ�߼�ֵ�û�
    /// </summary>
    public bool IsHighValueUser => ValueScore.OverallScore >= 80;

    /// <summary>
    /// �Ƿ���������
    /// </summary>
    public bool IsCompleteProfile => QualityScore.CompletenessScore >= 80;

    /// <summary>
    /// �Ƿ�������Ի����
    /// </summary>
    public bool AllowPersonalizedAds => UserPreferences.AllowPersonalizedAds &&
                                       PrivacySettings.CanUseForMarketing();

    /// <summary>
    /// �Ƿ��������ݹ���
    /// </summary>
    public bool AllowDataSharing => PrivacySettings.CanShareData();

    public IReadOnlyList<string> Keywords { get; private set; } = new List<string>();

    /// <summary>
    /// ��ȡ�û�����
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
    /// ��ȡ�û�ϸ��Ⱥ��
    /// </summary>
    public IReadOnlyList<string> GetUserSegments()
    {
        var segments = new List<string>();

        // ���������ϸ��
        var age = GetAge();
        if (age.HasValue)
        {
            segments.Add(age.Value switch
            {
                < 18 => "������",
                >= 18 and < 25 => "�������",
                >= 25 and < 35 => "����",
                >= 35 and < 50 => "����",
                >= 50 and < 65 => "������",
                >= 65 => "����"
            });
        }

        // ���ڼ�ֵ���ֵ�ϸ��
        if (ValueScore.OverallScore >= 80)
            segments.Add("�߼�ֵ�û�");
        else if (ValueScore.OverallScore >= 60)
            segments.Add("�м�ֵ�û�");
        else
            segments.Add("�ͼ�ֵ�û�");

        // ���ڻ�Ծ�ȵ�ϸ��
        if (BehaviorAnalysis.IsHighlyActive)
            segments.Add("�߻�Ծ�û�");
        else if (BehaviorAnalysis.IsActive)
            segments.Add("��Ծ�û�");
        else
            segments.Add("�ǻ�Ծ�û�");

        // ������Ȥ��ϸ��
        if (InterestTags.Any())
            segments.Add("��Ȥ��ȷ�û�");

        return segments;
    }

    /// <summary>
    /// ������ָ���û������ƶ�
    /// </summary>
    public decimal CalculateSimilarity(UserProfile other)
    {
        if (other == null)
            return 0;

        var similarity = 0m;
        var factors = 0;

        // ������Ϣ���ƶ�
        if (BasicInfo.Gender == other.BasicInfo.Gender && BasicInfo.Gender != Gender.Unknown)
        {
            similarity += 0.1m;
            factors++;
        }

        // �������ƶ�
        var age = GetAge();
        var otherAge = other.GetAge();
        if (age.HasValue && otherAge.HasValue)
        {
            var ageDiff = Math.Abs(age.Value - otherAge.Value);
            similarity += Math.Max(0, 1 - ageDiff / 50m) * 0.2m;
            factors++;
        }

        // ��Ȥ���ƶ�
        if (InterestTags.Any() && other.InterestTags.Any())
        {
            var commonInterests = InterestTags.Intersect(other.InterestTags, StringComparer.OrdinalIgnoreCase).Count();
            var totalInterests = InterestTags.Union(other.InterestTags, StringComparer.OrdinalIgnoreCase).Count();
            similarity += (decimal)commonInterests / totalInterests * 0.4m;
            factors++;
        }

        // ����λ�����ƶ�
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
    /// ���¼�����������
    /// </summary>
    private void RecalculateQualityScore()
    {
        QualityScore = CalculateQualityScore();

        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "QualityScore", QualityScore.OverallScore));
    }

    /// <summary>
    /// ���¼����ֵ����
    /// </summary>
    private void RecalculateValueScore()
    {
        ValueScore = CalculateValueScore();

        AddDomainEvent(new UserProfileScoreUpdatedEvent(Id, UserId, "ValueScore", ValueScore.OverallScore));
    }

    /// <summary>
    /// ������������
    /// </summary>
    private ProfileQualityScore CalculateQualityScore()
    {
        var completenessScore = CalculateCompletenessScore();
        var accuracyScore = CalculateAccuracyScore();
        var freshnessScore = CalculateFreshnessScore();

        return new ProfileQualityScore(completenessScore, accuracyScore, freshnessScore);
    }

    /// <summary>
    /// ��������������
    /// </summary>
    private int CalculateCompletenessScore()
    {
        var totalFields = 10;
        var filledFields = 0;

        // ������Ϣ
        if (!string.IsNullOrWhiteSpace(BasicInfo.DisplayName)) filledFields++;
        if (BasicInfo.Gender != Gender.Unknown) filledFields++;
        if (BasicInfo.DateOfBirth.HasValue) filledFields++;

        // �˿�ͳ��ѧ��Ϣ
        if (DemographicInfo != null && DemographicInfo.GetCompletenessScore() > 0) filledFields++;

        // ����λ����Ϣ
        if (GeoInfo != null && !string.IsNullOrWhiteSpace(GeoInfo.CountryCode)) filledFields++;

        // �豸��Ϣ
        if (DeviceInfo != null && DeviceInfo.DeviceType != DeviceType.Other) filledFields++;

        // ��Ȥ��ǩ
        if (InterestTags.Any()) filledFields++;

        // ��Ϊ��ǩ
        if (BehaviorTags.Any()) filledFields++;

        // �û�ƫ��
        if (UserPreferences != null) filledFields++;

        // ��˽����
        if (PrivacySettings != null && PrivacySettings.HasValidConsent()) filledFields++;

        return (int)Math.Round((decimal)filledFields / totalFields * 100);
    }

    /// <summary>
    /// ����׼ȷ������
    /// </summary>
    private int CalculateAccuracyScore()
    {
        // ��������һ���Ժ���֤״̬����
        var score = 100;

        // ��������Լ��
        var age = GetAge();
        if (age.HasValue && (age.Value < 0 || age.Value > 150))
            score -= 10;

        // ����λ��һ���Լ��
        if (GeoInfo != null)
        {
            if (string.IsNullOrWhiteSpace(GeoInfo.CountryCode) && !string.IsNullOrWhiteSpace(GeoInfo.RegionCode))
                score -= 5;
        }

        // �豸��Ϣһ���Լ��
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
    /// ����ʱЧ������
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
    /// �����ֵ����
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
    /// ������������
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
    /// �����ҳ϶�����
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

        // ���������Ծ�������
        if (daysSinceLastActive <= 7)
            loyaltyScore += 10;
        else if (daysSinceLastActive > 30)
            loyaltyScore -= 20;

        return Math.Max(0, Math.Min(100, loyaltyScore));
    }

    /// <summary>
    /// ������Ҽ�ֵ����
    /// </summary>
    private int CalculateMonetaryScore()
    {
        // ������Ի����û�����������������ˮƽ�ȼ���
        // Ŀǰ�����˿�ͳ��ѧ��Ϣ���м򵥹���
        if (DemographicInfo?.IncomeLevel?.Contains("��", StringComparison.OrdinalIgnoreCase) == true)
            return 80;
        if (DemographicInfo?.IncomeLevel?.Contains("��", StringComparison.OrdinalIgnoreCase) == true)
            return 60;
        if (DemographicInfo?.IncomeLevel?.Contains("��", StringComparison.OrdinalIgnoreCase) == true)
            return 40;

        return 50; // Ĭ��ֵ
    }

    /// <summary>
    /// ����Ǳ������
    /// </summary>
    private int CalculatePotentialScore()
    {
        var score = 50; // ������

        // ���������Ǳ������
        var age = GetAge();
        if (age.HasValue)
        {
            score += age.Value switch
            {
                >= 18 and < 35 => 20, // �����û�Ǳ������
                >= 35 and < 50 => 15,
                >= 50 and < 65 => 10,
                _ => 5
            };
        }

        // ������Ȥ�����Ե�Ǳ������
        if (InterestTags.Count > 5)
            score += 15;
        else if (InterestTags.Count > 2)
            score += 10;

        // �����豸���͵�Ǳ������
        if (DeviceInfo?.DeviceType == DeviceType.Smartphone)
            score += 10;

        return Math.Min(100, score);
    }

    /// <summary>
    /// ��֤�û�ID
    /// </summary>
    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("�û�ID����Ϊ��", nameof(userId));

        if (userId.Length > 255)
            throw new ArgumentException("�û�ID���Ȳ��ܳ���255���ַ�", nameof(userId));
    }

    /// <summary>
    /// ��֤������Ϣ
    /// </summary>
    private static void ValidateBasicInfo(UserBasicInfo basicInfo)
    {
        if (basicInfo == null)
            throw new ArgumentNullException(nameof(basicInfo));
    }
}