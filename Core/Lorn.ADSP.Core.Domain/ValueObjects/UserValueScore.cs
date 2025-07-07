using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �û���ֵ����ֵ����
/// </summary>
public class UserValueScore : ValueObject
{
    /// <summary>
    /// ���������
    /// </summary>
    public int EngagementScore { get; private set; }

    /// <summary>
    /// �ҳ϶�����
    /// </summary>
    public int LoyaltyScore { get; private set; }

    /// <summary>
    /// ���Ҽ�ֵ����
    /// </summary>
    public int MonetaryScore { get; private set; }

    /// <summary>
    /// Ǳ������
    /// </summary>
    public int PotentialScore { get; private set; }

    /// <summary>
    /// �ۺ�����
    /// </summary>
    public int OverallScore { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserValueScore() { }

    /// <summary>
    /// ���캯��
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
        
        // �����ۺ����֣���Ȩƽ����
        OverallScore = (int)Math.Round(
            (EngagementScore * 0.3 + 
             LoyaltyScore * 0.3 + 
             MonetaryScore * 0.25 + 
             PotentialScore * 0.15));
    }

    /// <summary>
    /// ����Ĭ�ϼ�ֵ����
    /// </summary>
    public static UserValueScore CreateDefault()
    {
        return new UserValueScore(50, 50, 50, 50);
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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