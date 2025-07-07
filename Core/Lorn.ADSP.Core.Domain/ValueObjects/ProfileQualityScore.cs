using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 画像质量评分值对象
/// </summary>
public class ProfileQualityScore : ValueObject
{
    /// <summary>
    /// 完整性评分
    /// </summary>
    public int CompletenessScore { get; private set; }

    /// <summary>
    /// 准确性评分
    /// </summary>
    public int AccuracyScore { get; private set; }

    /// <summary>
    /// 时效性评分
    /// </summary>
    public int FreshnessScore { get; private set; }

    /// <summary>
    /// 综合评分
    /// </summary>
    public int OverallScore { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private ProfileQualityScore() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ProfileQualityScore(
        int completenessScore,
        int accuracyScore,
        int freshnessScore)
    {
        CompletenessScore = Math.Max(0, Math.Min(100, completenessScore));
        AccuracyScore = Math.Max(0, Math.Min(100, accuracyScore));
        FreshnessScore = Math.Max(0, Math.Min(100, freshnessScore));
        
        // 计算综合评分（加权平均）
        OverallScore = (int)Math.Round(
            (CompletenessScore * 0.4 + 
             AccuracyScore * 0.4 + 
             FreshnessScore * 0.2));
    }

    /// <summary>
    /// 创建默认质量评分
    /// </summary>
    public static ProfileQualityScore CreateDefault()
    {
        return new ProfileQualityScore(0, 100, 100);
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompletenessScore;
        yield return AccuracyScore;
        yield return FreshnessScore;
        yield return OverallScore;
    }
}