using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 质量评分值对象
/// </summary>
public record QualityScore
{
    /// <summary>
    /// 广告ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// 总体质量分（0-10）
    /// </summary>
    public decimal OverallScore { get; init; }

    /// <summary>
    /// 相关性得分（0-10）
    /// </summary>
    public decimal RelevanceScore { get; init; }

    /// <summary>
    /// 用户体验得分（0-10）
    /// </summary>
    public decimal UserExperienceScore { get; init; }

    /// <summary>
    /// 预期点击率
    /// </summary>
    public decimal ExpectedCtr { get; init; }

    /// <summary>
    /// 预期转化率
    /// </summary>
    public decimal ExpectedCvr { get; init; }

    /// <summary>
    /// 历史表现得分
    /// </summary>
    public decimal HistoricalPerformanceScore { get; init; }

    /// <summary>
    /// 创意质量得分
    /// </summary>
    public decimal CreativeQualityScore { get; init; }

    /// <summary>
    /// 落地页质量得分
    /// </summary>
    public decimal LandingPageQualityScore { get; init; }

    /// <summary>
    /// 评分计算时间
    /// </summary>
    public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 评分版本
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// 置信度
    /// </summary>
    public decimal Confidence { get; init; } = 1.0m;
}