using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 用户价值评分值对象
/// </summary>
public class UserValueScore : ValueObject
{
    /// <summary>
    /// 参与度评分
    /// </summary>
    public int EngagementScore { get; private set; }

    /// <summary>
    /// 忠诚度评分
    /// </summary>
    public int LoyaltyScore { get; private set; }

    /// <summary>
    /// 货币价值评分
    /// </summary>
    public int MonetaryScore { get; private set; }

    /// <summary>
    /// 潜力评分
    /// </summary>
    public int PotentialScore { get; private set; }

    /// <summary>
    /// 综合评分
    /// </summary>
    public int OverallScore { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserValueScore() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserValueScore(
        int engagementScore,
        int loyaltyScore,
        int monetaryScore,
        int potentialScore)
    {
        EngagementScore = Math.Max(0, Math.Min(100, engagementScore));
        LoyaltyScore = Math.Max(0, Math.Min(100, loyaltyScore));
        MonetaryScore = Math.Max(0, Math.Min(100, monetaryScore));
        PotentialScore = Math.Max(0, Math.Min(100, potentialScore));
        
        // 计算综合评分（加权平均）
        OverallScore = (int)Math.Round(
            (EngagementScore * 0.3 + 
             LoyaltyScore * 0.3 + 
             MonetaryScore * 0.25 + 
             PotentialScore * 0.15));
    }

    /// <summary>
    /// 创建默认价值评分
    /// </summary>
    public static UserValueScore CreateDefault()
    {
        return new UserValueScore(50, 50, 50, 50);
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EngagementScore;
        yield return LoyaltyScore;
        yield return MonetaryScore;
        yield return PotentialScore;
        yield return OverallScore;
    }
}