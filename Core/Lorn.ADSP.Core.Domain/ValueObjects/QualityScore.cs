using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 质量评分值对象
/// </summary>
public class QualityScore : ValueObject
{
    /// <summary>
    /// 广告ID
    /// </summary>
    public string AdId { get; private set; }

    /// <summary>
    /// 总体质量分（0-10）
    /// </summary>
    public decimal OverallScore { get; private set; }

    /// <summary>
    /// 相关性得分（0-10）
    /// </summary>
    public decimal RelevanceScore { get; private set; }

    /// <summary>
    /// 用户体验得分（0-10）
    /// </summary>
    public decimal UserExperienceScore { get; private set; }

    /// <summary>
    /// 预期点击率
    /// </summary>
    public decimal ExpectedCtr { get; private set; }

    /// <summary>
    /// 预期转化率
    /// </summary>
    public decimal ExpectedCvr { get; private set; }

    /// <summary>
    /// 历史表现得分
    /// </summary>
    public decimal HistoricalPerformanceScore { get; private set; }

    /// <summary>
    /// 创意质量得分
    /// </summary>
    public decimal CreativeQualityScore { get; private set; }

    /// <summary>
    /// 落地页质量得分
    /// </summary>
    public decimal LandingPageQualityScore { get; private set; }

    /// <summary>
    /// 评分计算时间
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// 评分版本
    /// </summary>
    public string? Version { get; private set; }

    /// <summary>
    /// 置信度
    /// </summary>
    public decimal Confidence { get; private set; }

    /// <summary>
    /// 质量等级
    /// </summary>
    public QualityLevel QualityLevel
    {
        get
        {
            return OverallScore switch
            {
                >= 8.5m => QualityLevel.Excellent,
                >= 7.0m => QualityLevel.Good,
                >= 5.0m => QualityLevel.Average,
                >= 3.0m => QualityLevel.Poor,
                _ => QualityLevel.VeryPoor
            };
        }
    }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private QualityScore()
    {
        AdId = string.Empty;
        OverallScore = 0;
        RelevanceScore = 0;
        UserExperienceScore = 0;
        ExpectedCtr = 0;
        ExpectedCvr = 0;
        HistoricalPerformanceScore = 0;
        CreativeQualityScore = 0;
        LandingPageQualityScore = 0;
        CalculatedAt = DateTime.UtcNow;
        Confidence = 1.0m;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public QualityScore(
        string adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore,
        string? version = null,
        decimal confidence = 1.0m,
        DateTime? calculatedAt = null)
    {
        ValidateInput(adId, relevanceScore, userExperienceScore, expectedCtr, expectedCvr,
                     historicalPerformanceScore, creativeQualityScore, landingPageQualityScore, confidence);

        AdId = adId;
        RelevanceScore = relevanceScore;
        UserExperienceScore = userExperienceScore;
        ExpectedCtr = expectedCtr;
        ExpectedCvr = expectedCvr;
        HistoricalPerformanceScore = historicalPerformanceScore;
        CreativeQualityScore = creativeQualityScore;
        LandingPageQualityScore = landingPageQualityScore;
        Version = version;
        Confidence = confidence;
        CalculatedAt = calculatedAt ?? DateTime.UtcNow;

        // 计算总体质量分（加权平均）
        OverallScore = CalculateOverallScore();
    }

    /// <summary>
    /// 创建质量评分
    /// </summary>
    public static QualityScore Create(
        string adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore)
    {
        return new QualityScore(
            adId,
            relevanceScore,
            userExperienceScore,
            expectedCtr,
            expectedCvr,
            historicalPerformanceScore,
            creativeQualityScore,
            landingPageQualityScore);
    }

    /// <summary>
    /// 创建默认质量评分
    /// </summary>
    public static QualityScore CreateDefault(string adId)
    {
        return new QualityScore(
            adId,
            5.0m, // 中等相关性
            5.0m, // 中等用户体验
            0.02m, // 2% CTR
            0.01m, // 1% CVR
            5.0m, // 中等历史表现
            5.0m, // 中等创意质量
            5.0m); // 中等落地页质量
    }

    /// <summary>
    /// 更新相关性得分
    /// </summary>
    public QualityScore UpdateRelevanceScore(decimal newScore)
    {
        return new QualityScore(
            AdId,
            newScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 更新预期表现
    /// </summary>
    public QualityScore UpdateExpectedPerformance(decimal newCtr, decimal newCvr)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            newCtr,
            newCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 更新历史表现得分
    /// </summary>
    public QualityScore UpdateHistoricalPerformance(decimal newScore)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            newScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 设置版本
    /// </summary>
    public QualityScore WithVersion(string version)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            version,
            Confidence,
            CalculatedAt);
    }

    /// <summary>
    /// 调整置信度
    /// </summary>
    public QualityScore WithConfidence(decimal confidence)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            confidence,
            CalculatedAt);
    }

    /// <summary>
    /// 是否为高质量广告
    /// </summary>
    public bool IsHighQuality => OverallScore >= 7.0m;

    /// <summary>
    /// 是否为低质量广告
    /// </summary>
    public bool IsLowQuality => OverallScore < 3.0m;

    /// <summary>
    /// 获取加权质量分
    /// </summary>
    public decimal GetWeightedScore(decimal confidenceWeight = 1.0m)
    {
        return OverallScore * Confidence * confidenceWeight;
    }

    /// <summary>
    /// 计算总体质量分
    /// </summary>
    private decimal CalculateOverallScore()
    {
        // 权重分配：相关性30%，用户体验25%，历史表现20%，创意质量15%，落地页质量10%
        var weightedScore =
            RelevanceScore * 0.30m +
            UserExperienceScore * 0.25m +
            HistoricalPerformanceScore * 0.20m +
            CreativeQualityScore * 0.15m +
            LandingPageQualityScore * 0.10m;

        return Math.Round(weightedScore, 2);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AdId;
        yield return OverallScore;
        yield return RelevanceScore;
        yield return UserExperienceScore;
        yield return ExpectedCtr;
        yield return ExpectedCvr;
        yield return HistoricalPerformanceScore;
        yield return CreativeQualityScore;
        yield return LandingPageQualityScore;
        yield return Version ?? string.Empty;
        yield return Confidence;
        yield return CalculatedAt;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        string adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore,
        decimal confidence)
    {
        if (string.IsNullOrWhiteSpace(adId))
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        ValidateScore(relevanceScore, nameof(relevanceScore));
        ValidateScore(userExperienceScore, nameof(userExperienceScore));
        ValidateScore(historicalPerformanceScore, nameof(historicalPerformanceScore));
        ValidateScore(creativeQualityScore, nameof(creativeQualityScore));
        ValidateScore(landingPageQualityScore, nameof(landingPageQualityScore));

        if (expectedCtr < 0 || expectedCtr > 1)
            throw new ArgumentException("预期点击率必须在0-1之间", nameof(expectedCtr));

        if (expectedCvr < 0 || expectedCvr > 1)
            throw new ArgumentException("预期转化率必须在0-1之间", nameof(expectedCvr));

        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("置信度必须在0-1之间", nameof(confidence));
    }

    /// <summary>
    /// 验证评分范围
    /// </summary>
    private static void ValidateScore(decimal score, string paramName)
    {
        if (score < 0 || score > 10)
            throw new ArgumentException($"{paramName}必须在0-10之间", paramName);
    }
}

