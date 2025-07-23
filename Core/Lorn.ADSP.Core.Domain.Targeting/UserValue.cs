namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// �û���ֵ����������
/// �̳���TargetingContextBase���ṩ�û���ֵ�������ݵĶ��������Ĺ���
/// רע�ڻ����û���ֵ�Ĺ�涨�����
/// </summary>
public class UserValue : TargetingContextBase
{
    /// <summary>
    /// ������������
    /// </summary>
    public override string ContextName => "�û���ֵ������";

    /// <summary>
    /// ��������� (0-100)
    /// </summary>
    public int EngagementScore => GetPropertyValue("EngagementScore", 0);

    /// <summary>
    /// �ҳ϶����� (0-100)
    /// </summary>
    public int LoyaltyScore => GetPropertyValue("LoyaltyScore", 0);

    /// <summary>
    /// ���Ҽ�ֵ���� (0-100)
    /// </summary>
    public int MonetaryScore => GetPropertyValue("MonetaryScore", 0);

    /// <summary>
    /// Ǳ������ (0-100)
    /// </summary>
    public int PotentialScore => GetPropertyValue("PotentialScore", 0);

    /// <summary>
    /// �ۺ����� (0-100)
    /// </summary>
    public int OverallScore => GetPropertyValue("OverallScore", 0);

    /// <summary>
    /// �������ڼ�ֵ����
    /// </summary>
    public decimal EstimatedLTV => GetPropertyValue("EstimatedLTV", 0.0m);

    /// <summary>
    /// ���������ȼ�
    /// </summary>
    public SpendingLevel SpendingLevel => GetPropertyValue("SpendingLevel", SpendingLevel.Medium);

    /// <summary>
    /// �û���ֵ�ȼ�
    /// </summary>
    public ValueTier ValueTier => GetPropertyValue("ValueTier", ValueTier.Standard);

    /// <summary>
    /// ת������ (0.0-1.0)
    /// </summary>
    public decimal ConversionProbability => GetPropertyValue("ConversionProbability", 0.0m);

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserValue() : base("UserValue") { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public UserValue(
        int engagementScore = 50,
        int loyaltyScore = 50,
        int monetaryScore = 50,
        int potentialScore = 50,
        decimal estimatedLTV = 0.0m,
        SpendingLevel spendingLevel = SpendingLevel.Medium,
        ValueTier valueTier = ValueTier.Standard,
        decimal conversionProbability = 0.0m,
        string? dataSource = null)
        : base("UserValue", CreateProperties(engagementScore, loyaltyScore, monetaryScore, potentialScore, estimatedLTV, spendingLevel, valueTier, conversionProbability), dataSource)
    {
    }

    /// <summary>
    /// ���������ֵ�
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        int engagementScore,
        int loyaltyScore,
        int monetaryScore,
        int potentialScore,
        decimal estimatedLTV,
        SpendingLevel spendingLevel,
        ValueTier valueTier,
        decimal conversionProbability)
    {
        // �������ַ�Χ
        engagementScore = Math.Max(0, Math.Min(100, engagementScore));
        loyaltyScore = Math.Max(0, Math.Min(100, loyaltyScore));
        monetaryScore = Math.Max(0, Math.Min(100, monetaryScore));
        potentialScore = Math.Max(0, Math.Min(100, potentialScore));

        // �����ۺ����֣���Ȩƽ����
        var overallScore = (int)Math.Round(
            engagementScore * 0.3 +
            loyaltyScore * 0.3 +
            monetaryScore * 0.25 +
            potentialScore * 0.15);

        // ����ת�����ʷ�Χ
        conversionProbability = Math.Max(0.0m, Math.Min(1.0m, conversionProbability));

        return new Dictionary<string, object>
        {
            ["EngagementScore"] = engagementScore,
            ["LoyaltyScore"] = loyaltyScore,
            ["MonetaryScore"] = monetaryScore,
            ["PotentialScore"] = potentialScore,
            ["OverallScore"] = overallScore,
            ["EstimatedLTV"] = estimatedLTV,
            ["SpendingLevel"] = spendingLevel,
            ["ValueTier"] = valueTier,
            ["ConversionProbability"] = conversionProbability
        };
    }

    /// <summary>
    /// ����Ĭ�ϼ�ֵ������
    /// </summary>
    public static UserValue CreateDefault(string? dataSource = null)
    {
        return new UserValue(dataSource: dataSource);
    }

    /// <summary>
    /// �����߼�ֵ�û�������
    /// </summary>
    public static UserValue CreateHighValue(
        int engagementScore = 85,
        int loyaltyScore = 90,
        int monetaryScore = 80,
        int potentialScore = 75,
        decimal estimatedLTV = 1000.0m,
        string? dataSource = null)
    {
        return new UserValue(
            engagementScore: engagementScore,
            loyaltyScore: loyaltyScore,
            monetaryScore: monetaryScore,
            potentialScore: potentialScore,
            estimatedLTV: estimatedLTV,
            spendingLevel: SpendingLevel.High,
            valueTier: ValueTier.Premium,
            conversionProbability: 0.8m,
            dataSource: dataSource);
    }

    /// <summary>
    /// ����Ǳ�ڼ�ֵ�û�������
    /// </summary>
    public static UserValue CreatePotential(
        int engagementScore = 70,
        int loyaltyScore = 40,
        int monetaryScore = 30,
        int potentialScore = 85,
        string? dataSource = null)
    {
        return new UserValue(
            engagementScore: engagementScore,
            loyaltyScore: loyaltyScore,
            monetaryScore: monetaryScore,
            potentialScore: potentialScore,
            spendingLevel: SpendingLevel.Medium,
            valueTier: ValueTier.Growth,
            conversionProbability: 0.4m,
            dataSource: dataSource);
    }

    /// <summary>
    /// �Ƿ�Ϊ�߼�ֵ�û�
    /// </summary>
    public bool IsHighValueUser => OverallScore >= 80 || ValueTier >= ValueTier.Premium;

    /// <summary>
    /// �Ƿ�Ϊ��Ծ�û�
    /// </summary>
    public bool IsActiveUser => EngagementScore >= 60;

    /// <summary>
    /// �Ƿ�Ϊ�ҳ��û�
    /// </summary>
    public bool IsLoyalUser => LoyaltyScore >= 70;

    /// <summary>
    /// �Ƿ��������Ǳ��
    /// </summary>
    public bool HasSpendingPotential => MonetaryScore >= 50 || PotentialScore >= 70;

    /// <summary>
    /// ��ȡ�û���ֵ�ȼ�����
    /// </summary>
    public string GetValueDescription()
    {
        return ValueTier switch
        {
            ValueTier.Basic => "�����û�",
            ValueTier.Standard => "��׼�û�",
            ValueTier.Growth => "�ɳ��û�",
            ValueTier.Premium => "�����û�",
            ValueTier.VIP => "VIP�û�",
            _ => "δ�����û�"
        };
    }

    /// <summary>
    /// ��ȡ������������
    /// </summary>
    public string GetSpendingDescription()
    {
        return SpendingLevel switch
        {
            SpendingLevel.Low => "������",
            SpendingLevel.Medium => "�е�����",
            SpendingLevel.High => "������",
            _ => "δ֪��������"
        };
    }

    /// <summary>
    /// �����ֵƥ���
    /// </summary>
    public decimal CalculateValueMatchScore(ValueTier targetTier, SpendingLevel? targetSpendingLevel = null)
    {
        var tierScore = ValueTier >= targetTier ? 1.0m : 0.0m;

        if (targetSpendingLevel.HasValue)
        {
            var spendingScore = SpendingLevel >= targetSpendingLevel.Value ? 1.0m : 0.0m;
            return (tierScore + spendingScore) / 2.0m;
        }

        return tierScore;
    }

    /// <summary>
    /// ��ȡ�����۽���ϵ��
    /// </summary>
    public decimal GetBidMultiplier()
    {
        return ValueTier switch
        {
            ValueTier.VIP => 2.0m,
            ValueTier.Premium => 1.5m,
            ValueTier.Growth => 1.2m,
            ValueTier.Standard => 1.0m,
            ValueTier.Basic => 0.8m,
            _ => 1.0m
        };
    }

    /// <summary>
    /// ��ȡת������ֵ
    /// </summary>
    public decimal GetExpectedConversionValue()
    {
        return EstimatedLTV * ConversionProbability;
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var valueInfo = $"Overall:{OverallScore} Tier:{ValueTier} Spending:{SpendingLevel} LTV:{EstimatedLTV:C} CVR:{ConversionProbability:P1}";
        return $"{baseInfo} | {valueInfo}";
    }

    /// <summary>
    /// ��֤�����ĵ���Ч��
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // ��֤���ַ�Χ
        var scores = new[] { EngagementScore, LoyaltyScore, MonetaryScore, PotentialScore, OverallScore };
        if (scores.Any(score => score < 0 || score > 100))
            return false;

        // ��֤ת�����ʷ�Χ
        if (ConversionProbability < 0.0m || ConversionProbability > 1.0m)
            return false;

        // ��֤LTV��Ϊ����
        if (EstimatedLTV < 0.0m)
            return false;

        return true;
    }
}

/// <summary>
/// ���������ȼ�ö��
/// </summary>
public enum SpendingLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}

/// <summary>
/// �û���ֵ�ȼ�ö��
/// </summary>
public enum ValueTier
{
    Basic = 1,
    Standard = 2,
    Growth = 3,
    Premium = 4,
    VIP = 5
}
