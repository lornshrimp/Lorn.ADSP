using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �������ֵ����
/// </summary>
public class TargetingPolicy : ValueObject
{
    /// <summary>
    /// ����λ�ö���
    /// </summary>
    public GeoTargeting? GeoTargeting { get; private set; }

    /// <summary>
    /// �˿����Զ���
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; private set; }

    /// <summary>
    /// �豸����
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; private set; }

    /// <summary>
    /// ʱ�䶨��
    /// </summary>
    public TimeTargeting? TimeTargeting { get; private set; }

    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; private set; }

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// �Ƿ����ö���
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TargetingPolicy() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TargetingPolicy(
        GeoTargeting? geoTargeting = null,
        DemographicTargeting? demographicTargeting = null,
        DeviceTargeting? deviceTargeting = null,
        TimeTargeting? timeTargeting = null,
        BehaviorTargeting? behaviorTargeting = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        GeoTargeting = geoTargeting;
        DemographicTargeting = demographicTargeting;
        DeviceTargeting = deviceTargeting;
        TimeTargeting = timeTargeting;
        BehaviorTargeting = behaviorTargeting;
        Weight = weight;
        IsEnabled = isEnabled;
    }

    /// <summary>
    /// �����յĶ������
    /// </summary>
    public static TargetingPolicy CreateEmpty()
    {
        return new TargetingPolicy();
    }

    /// <summary>
    /// ����ȫ��������ԣ������ƣ�
    /// </summary>
    public static TargetingPolicy CreateUnrestricted()
    {
        return new TargetingPolicy(isEnabled: false);
    }

    /// <summary>
    /// ����ƥ���
    /// </summary>
    public decimal CalculateMatchScore(TargetingContext context)
    {
        if (!IsEnabled)
            return 1.0m; // δ���ö���ʱ��ƥ���Ϊ100%

        decimal totalScore = 0m;
        int targetingCount = 0;

        // ����λ�ö���ƥ��
        if (GeoTargeting != null)
        {
            totalScore += GeoTargeting.CalculateMatchScore(context.GeoLocation);
            targetingCount++;
        }

        // �˿����Զ���ƥ��
        if (DemographicTargeting != null)
        {
            totalScore += DemographicTargeting.CalculateMatchScore(context.UserProfile);
            targetingCount++;
        }

        // �豸����ƥ��
        if (DeviceTargeting != null)
        {
            totalScore += DeviceTargeting.CalculateMatchScore(context.DeviceInfo); // Fix: Removed extra dot and ensured correct parameter type
            targetingCount++;
        }

        // ʱ�䶨��ƥ��
        if (TimeTargeting != null)
        {
            totalScore += TimeTargeting.CalculateMatchScore(context.RequestTime);
            targetingCount++;
        }

        // ��Ϊ����ƥ��
        if (BehaviorTargeting != null)
        {
            totalScore += BehaviorTargeting.CalculateMatchScore(context.UserBehavior);
            targetingCount++;
        }

        // ���û�ж�������������Ĭ��ƥ���
        if (targetingCount == 0)
            return 1.0m;

        // ����ƽ��ƥ��Ȳ�Ӧ��Ȩ��
        return (totalScore / targetingCount) * Weight;
    }

    /// <summary>
    /// �Ƿ�ƥ��
    /// </summary>
    public bool IsMatch(TargetingContext context, decimal threshold = 0.5m)
    {
        return CalculateMatchScore(context) >= threshold;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GeoTargeting ?? GeoTargeting.Create(new List<GeoInfo>()); // Fix: Use the static Create method to instantiate GeoTargeting
        yield return DemographicTargeting ?? new DemographicTargeting();
        yield return DeviceTargeting ?? DeviceTargeting.Create(); // Fix: Use the static Create method to instantiate DeviceTargeting
        yield return TimeTargeting ?? TimeTargeting.Create();
        yield return BehaviorTargeting ?? new BehaviorTargeting();
        yield return Weight;
        yield return IsEnabled;
    }
}

/// <summary>
/// �˿����Զ���
/// </summary>
public class DemographicTargeting : ValueObject
{
    /// <summary>
    /// Ŀ���Ա�
    /// </summary>
    public IReadOnlyList<Gender> TargetGenders { get; private set; } = new List<Gender>();

    /// <summary>
    /// ��С����
    /// </summary>
    public int? MinAge { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public int? MaxAge { get; private set; }

    /// <summary>
    /// Ŀ��ؼ���
    /// </summary>
    public IReadOnlyList<string> TargetKeywords { get; private set; } = new List<string>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public DemographicTargeting(
        IList<Gender>? targetGenders = null,
        int? minAge = null,
        int? maxAge = null,
        IList<string>? targetKeywords = null)
    {
        TargetGenders = targetGenders?.ToList() ?? new List<Gender>();
        MinAge = minAge;
        MaxAge = maxAge;
        TargetKeywords = targetKeywords?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// �����˿�����ƥ���
    /// </summary>
    public decimal CalculateMatchScore(UserProfile? userProfile)
    {
        if (userProfile == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // �Ա�ƥ��
        if (TargetGenders.Any())
        {
            score *= TargetGenders.Contains(userProfile.BasicInfo.Gender) ? 1.0m : 0m;
            criteriaCount++;
        }

        // ����ƥ��
        if (MinAge.HasValue || MaxAge.HasValue)
        {
            var ageMatch = true;
            if (MinAge.HasValue && userProfile.GetAge() < MinAge.Value)
                ageMatch = false;
            if (MaxAge.HasValue && userProfile.GetAge() > MaxAge.Value)
                ageMatch = false;

            score *= ageMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        // �ؼ���ƥ��
        if (TargetKeywords.Any())
        {
            var keywordMatch = TargetKeywords.Any(keyword =>
                userProfile.Keywords.Any(userKeyword =>
                    userKeyword.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            score *= keywordMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var gender in TargetGenders)
            yield return gender;
        yield return MinAge ?? 0;
        yield return MaxAge ?? 0;
        foreach (var keyword in TargetKeywords)
            yield return keyword;
    }
}

/// <summary>
/// ��Ϊ����
/// </summary>
public class BehaviorTargeting : ValueObject
{
    /// <summary>
    /// Ŀ����Ȥ��ǩ
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

    /// <summary>
    /// Ŀ����Ϊ����
    /// </summary>
    public IReadOnlyList<string> BehaviorTypes { get; private set; } = new List<string>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public BehaviorTargeting(
        IList<string>? interestTags = null,
        IList<string>? behaviorTypes = null)
    {
        InterestTags = interestTags?.ToList() ?? new List<string>();
        BehaviorTypes = behaviorTypes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// ������Ϊƥ���
    /// </summary>
    public decimal CalculateMatchScore(UserBehavior? userBehavior)
    {
        if (userBehavior == null)
            return 1.0m;

        decimal score = 1.0m;
        int criteriaCount = 0;

        // ��Ȥ��ǩƥ��
        if (InterestTags.Any())
        {
            var interestMatch = InterestTags.Any(tag =>
                userBehavior.InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
            score *= interestMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        // ��Ϊ����ƥ��
        if (BehaviorTypes.Any())
        {
            var behaviorMatch = BehaviorTypes.Any(type =>
                userBehavior.BehaviorHistory.Any(b =>
                    string.Equals(b.BehaviorType, type, StringComparison.OrdinalIgnoreCase)));
            score *= behaviorMatch ? 1.0m : 0m;
            criteriaCount++;
        }

        return criteriaCount == 0 ? 1.0m : score;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var tag in InterestTags)
            yield return tag;
        foreach (var type in BehaviorTypes)
            yield return type;
    }
}

/// <summary>
/// ����������
/// </summary>
public class TargetingContext
{
    /// <summary>
    /// ����λ��
    /// </summary>
    public GeoInfo? GeoLocation { get; set; }

    /// <summary>
    /// �û�����
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// �豸��Ϣ
    /// </summary>
    public DeviceInfo? DeviceInfo { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// �û���Ϊ
    /// </summary>
    public UserBehavior? UserBehavior { get; set; }
}

/// <summary>
/// �û���Ϊ
/// </summary>
public class UserBehavior
{
    /// <summary>
    /// ��Ȥ��ǩ
    /// </summary>
    public IList<string> InterestTags { get; set; } = new List<string>();

    /// <summary>
    /// ��Ϊ��ʷ
    /// </summary>
    public IList<BehaviorRecord> BehaviorHistory { get; set; } = new List<BehaviorRecord>();
}

/// <summary>
/// ��Ϊ��¼
/// </summary>
public class BehaviorRecord
{
    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public string BehaviorType { get; set; } = string.Empty;

    /// <summary>
    /// ��Ϊʱ��
    /// </summary>
    public DateTime Timestamp { get; set; }
}