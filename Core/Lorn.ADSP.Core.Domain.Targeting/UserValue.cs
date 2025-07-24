using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.Targeting;

/// <summary>
/// 用户价值定向上下文
/// 继承自TargetingContextBase，提供用户价值相关数据的定向上下文功能
/// 专注于基于用户价值的广告定向策略
/// </summary>
public class UserValue : TargetingContextBase
{
    /// <summary>
    /// 上下文类型名称
    /// </summary>
    public override string ContextName => "用户价值上下文";

    /// <summary>
    /// 参与度评分 (0-100)
    /// </summary>
    public int EngagementScore => GetPropertyValue("EngagementScore", 0);

    /// <summary>
    /// 忠诚度评分 (0-100)
    /// </summary>
    public int LoyaltyScore => GetPropertyValue("LoyaltyScore", 0);

    /// <summary>
    /// 消费价值评分 (0-100)
    /// </summary>
    public int MonetaryScore => GetPropertyValue("MonetaryScore", 0);

    /// <summary>
    /// 潜力评分 (0-100)
    /// </summary>
    public int PotentialScore => GetPropertyValue("PotentialScore", 0);

    /// <summary>
    /// 综合评分 (0-100)
    /// </summary>
    public int OverallScore => GetPropertyValue("OverallScore", 0);

    /// <summary>
    /// 预估生命周期价值（元）
    /// </summary>
    public decimal EstimatedLTV => GetPropertyValue("EstimatedLTV", 0.0m);

    /// <summary>
    /// 消费能力等级
    /// </summary>
    public SpendingLevel SpendingLevel => GetPropertyValue("SpendingLevel", SpendingLevel.Medium);

    /// <summary>
    /// 用户价值等级
    /// </summary>
    public ValueTier ValueTier => GetPropertyValue("ValueTier", ValueTier.Standard);

    /// <summary>
    /// 转化概率 (0.0-1.0)
    /// </summary>
    public decimal ConversionProbability => GetPropertyValue("ConversionProbability", 0.0m);

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserValue() : base("UserValue") { }

    /// <summary>
    /// 构造函数
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
    /// 创建属性字典
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
        // 限制评分范围
        engagementScore = Math.Max(0, Math.Min(100, engagementScore));
        loyaltyScore = Math.Max(0, Math.Min(100, loyaltyScore));
        monetaryScore = Math.Max(0, Math.Min(100, monetaryScore));
        potentialScore = Math.Max(0, Math.Min(100, potentialScore));

        // 计算综合评分（加权平均）
        var overallScore = (int)Math.Round(
            engagementScore * 0.3 +
            loyaltyScore * 0.3 +
            monetaryScore * 0.25 +
            potentialScore * 0.15);

        // 限制转化概率范围
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
    /// 创建默认价值上下文
    /// </summary>
    public static UserValue CreateDefault(string? dataSource = null)
    {
        return new UserValue(dataSource: dataSource);
    }

    /// <summary>
    /// 创建高价值用户上下文
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
    /// 创建潜在价值用户上下文
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
    /// 是否为高价值用户
    /// </summary>
    public bool IsHighValueUser => OverallScore >= 80 || ValueTier >= ValueTier.Premium;

    /// <summary>
    /// 是否为活跃用户
    /// </summary>
    public bool IsActiveUser => EngagementScore >= 60;

    /// <summary>
    /// 是否为忠诚用户
    /// </summary>
    public bool IsLoyalUser => LoyaltyScore >= 70;

    /// <summary>
    /// 是否具有消费潜力
    /// </summary>
    public bool HasSpendingPotential => MonetaryScore >= 50 || PotentialScore >= 70;

    /// <summary>
    /// 获取用户价值等级描述
    /// </summary>
    public string GetValueDescription()
    {
        return ValueTier switch
        {
            ValueTier.Basic => "基础用户",
            ValueTier.Standard => "标准用户",
            ValueTier.Growth => "成长用户",
            ValueTier.Premium => "优质用户",
            ValueTier.VIP => "VIP用户",
            _ => "未知类用户"
        };
    }

    /// <summary>
    /// 获取消费能力描述
    /// </summary>
    public string GetSpendingDescription()
    {
        return SpendingLevel switch
        {
            SpendingLevel.Low => "低消费",
            SpendingLevel.Medium => "中等消费",
            SpendingLevel.High => "高消费",
            _ => "未知消费水平"
        };
    }

    /// <summary>
    /// 计算价值匹配度
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
    /// 获取竞价调整系数
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
    /// 获取转化期望价值
    /// </summary>
    public decimal GetExpectedConversionValue()
    {
        return EstimatedLTV * ConversionProbability;
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var valueInfo = $"Overall:{OverallScore} Tier:{ValueTier} Spending:{SpendingLevel} LTV:{EstimatedLTV:C} CVR:{ConversionProbability:P1}";
        return $"{baseInfo} | {valueInfo}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // 验证评分范围
        var scores = new[] { EngagementScore, LoyaltyScore, MonetaryScore, PotentialScore, OverallScore };
        if (scores.Any(score => score < 0 || score > 100))
            return false;

        // 验证转化概率范围
        if (ConversionProbability < 0.0m || ConversionProbability > 1.0m)
            return false;

        // 验证LTV不为负数
        if (EstimatedLTV < 0.0m)
            return false;

        return true;
    }
}

/// <summary>
/// 消费能力等级枚举
/// </summary>
public enum SpendingLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}

/// <summary>
/// 用户价值等级枚举
/// </summary>
public enum ValueTier
{
    Basic = 1,
    Standard = 2,
    Growth = 3,
    Premium = 4,
    VIP = 5
}