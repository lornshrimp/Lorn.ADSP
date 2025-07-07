using Lorn.ADSP.Core.Domain.Common;

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









