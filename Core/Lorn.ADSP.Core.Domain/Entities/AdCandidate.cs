using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ����ѡʵ��
/// �ڹ��Ͷ�������еĺ�ѡ��棬�ǹ���ٻء����ˡ�����׶εĺ��Ĵ������
/// �������ڣ��ٻء����ˡ������Ͷ�ţ�ÿ���׶ζ������޸ĺ�ѡ���������
/// </summary>
public class AdCandidate : EntityBase
{
    /// <summary>
    /// ���Ψһ��ʶ
    /// </summary>
    public string AdId { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public AdType AdType { get; private set; }

    /// <summary>
    /// ���ʶ
    /// </summary>
    public string CampaignId { get; private set; }

    /// <summary>
    /// �����ʶ
    /// </summary>
    public string CreativeId { get; private set; }

    /// <summary>
    /// ���ۼ۸�
    /// </summary>
    public decimal BidPrice { get; private set; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public CreativeInfo Creative { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public TargetingPolicy Targeting { get; private set; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public BiddingInfo Bidding { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public QualityScore QualityScore { get; private set; }

    /// <summary>
    /// Ԥ�������
    /// </summary>
    public double PredictedCtr { get; private set; }

    /// <summary>
    /// Ԥ��ת����
    /// </summary>
    public double PredictedCvr { get; private set; }

    /// <summary>
    /// Ȩ�ط���
    /// </summary>
    public double WeightScore { get; private set; }

    /// <summary>
    /// ��չ����������
    /// </summary>
    public Dictionary<string, object> Context { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private AdCandidate(
        string adId,
        AdType adType,
        string campaignId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        TargetingPolicy targeting,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr = 0.0,
        double predictedCvr = 0.0,
        double weightScore = 0.0,
        Dictionary<string, object>? context = null)
    {
        AdId = adId;
        AdType = adType;
        CampaignId = campaignId;
        CreativeId = creativeId;
        BidPrice = bidPrice;
        Creative = creative;
        Targeting = targeting;
        Bidding = bidding;
        QualityScore = qualityScore;
        PredictedCtr = predictedCtr;
        PredictedCvr = predictedCvr;
        WeightScore = weightScore;
        Context = context ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// ��������ѡ����
    /// </summary>
    public static AdCandidate Create(
        string adId,
        AdType adType,
        string campaignId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        TargetingPolicy targeting,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr = 0.0,
        double predictedCvr = 0.0,
        double weightScore = 0.0,
        Dictionary<string, object>? context = null)
    {
        ValidateParameters(adId, campaignId, creativeId, bidPrice, creative, targeting, bidding, qualityScore);

        return new AdCandidate(
            adId,
            adType,
            campaignId,
            creativeId,
            bidPrice,
            creative,
            targeting,
            bidding,
            qualityScore,
            predictedCtr,
            predictedCvr,
            weightScore,
            context);
    }

    /// <summary>
    /// ���¾��ۼ۸�
    /// </summary>
    public void UpdateBidPrice(decimal newBidPrice)
    {
        if (newBidPrice < 0)
            throw new ArgumentException("���ۼ۸���Ϊ����", nameof(newBidPrice));

        BidPrice = newBidPrice;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����Ԥ�������
    /// </summary>
    public void UpdatePredictedCtr(double predictedCtr)
    {
        if (predictedCtr < 0 || predictedCtr > 1)
            throw new ArgumentException("Ԥ������ʱ�����0-1֮��", nameof(predictedCtr));

        PredictedCtr = predictedCtr;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����Ԥ��ת����
    /// </summary>
    public void UpdatePredictedCvr(double predictedCvr)
    {
        if (predictedCvr < 0 || predictedCvr > 1)
            throw new ArgumentException("Ԥ��ת���ʱ�����0-1֮��", nameof(predictedCvr));

        PredictedCvr = predictedCvr;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����Ȩ�ط���
    /// </summary>
    public void UpdateWeightScore(double weightScore)
    {
        if (weightScore < 0)
            throw new ArgumentException("Ȩ�ط�������Ϊ����", nameof(weightScore));

        WeightScore = weightScore;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ������������
    /// </summary>
    public void UpdateQualityScore(QualityScore qualityScore)
    {
        QualityScore = qualityScore ?? throw new ArgumentNullException(nameof(qualityScore));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �������������
    /// </summary>
    public void AddContext(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�����ļ�����Ϊ��", nameof(key));

        Context[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �Ƴ�����������
    /// </summary>
    public void RemoveContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�����ļ�����Ϊ��", nameof(key));

        if (Context.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// ��������Է���
    /// ע�⣺ʵ�ʵĶ���ƥ�����Ӧ���ɶ�����Լ�����������
    /// </summary>
    public double CalculateRelevanceScore(AdContext adContext)
    {
        if (adContext == null)
            throw new ArgumentNullException(nameof(adContext));

        // ������������
        double baseScore = (double)QualityScore.OverallScore;

        // ����ƥ��������ⲿ���Լ���������
        // ���ﷵ�ػ�������������Ķ���ƥ���� ITargetingMatcher ����
        return baseScore;
    }

    /// <summary>
    /// ����Ƿ����Ͷ�������Ļ������
    /// ����Ķ���ƥ�����ⲿ���Լ���������
    /// </summary>
    public bool IsEligibleForPlacement(AdContext adContext)
    {
        if (adContext == null)
            return false;

        // �����������
        if (BidPrice <= 0)
            return false;

        // �����������
        if (QualityScore.OverallScore < 0.1m)
            return false;

        // ע�⣺����Ķ���ƥ����Ӧ�����ⲿ������Լ�����ִ��
        // ����ֻ�������ĺϸ��Լ��
        return true;
    }

    /// <summary>
    /// ��ȡ����ָ��
    /// </summary>
    public Dictionary<string, object> GetPerformanceMetrics()
    {
        return new Dictionary<string, object>
        {
            ["AdId"] = AdId,
            ["BidPrice"] = BidPrice,
            ["PredictedCtr"] = PredictedCtr,
            ["PredictedCvr"] = PredictedCvr,
            ["WeightScore"] = WeightScore,
            ["QualityScore"] = QualityScore.OverallScore,
            ["ExpectedRevenue"] = (double)BidPrice * PredictedCtr * PredictedCvr
        };
    }

    /// <summary>
    /// ������֤
    /// </summary>
    private static void ValidateParameters(
        string adId,
        string campaignId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        TargetingPolicy targeting,
        BiddingInfo bidding,
        QualityScore qualityScore)
    {
        if (string.IsNullOrEmpty(adId))
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        if (string.IsNullOrEmpty(campaignId))
            throw new ArgumentException("�ID����Ϊ��", nameof(campaignId));

        if (string.IsNullOrEmpty(creativeId))
            throw new ArgumentException("����ID����Ϊ��", nameof(creativeId));

        if (bidPrice < 0)
            throw new ArgumentException("���ۼ۸���Ϊ����", nameof(bidPrice));

        if (creative == null)
            throw new ArgumentNullException(nameof(creative));

        if (targeting == null)
            throw new ArgumentNullException(nameof(targeting));

        if (bidding == null)
            throw new ArgumentNullException(nameof(bidding));

        if (qualityScore == null)
            throw new ArgumentNullException(nameof(qualityScore));
    }
}