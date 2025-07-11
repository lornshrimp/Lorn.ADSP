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
    /// ���ʶ��ͨ����ID���Ի�ȡ��Campaign���ٻ�ȡTargetingConfig��
    /// </summary>
    public string CampaignId { get; private set; }

    /// <summary>
    /// ���λ��ʶ
    /// Ŀ��Ͷ�ŵĹ��λID�����ڱ�ʶ�ú�ѡ���ҪͶ�ŵ��ĸ����λ
    /// </summary>
    public string PlacementId { get; private set; }

    /// <summary>
    /// Campaignʵ�����ã��ۺϹ�ϵ��
    /// AdCandidateͨ��Campaign������TargetingConfig��������ֱ�ӳ���TargetingConfig
    /// </summary>
    public Campaign Campaign { get; private set; }

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
    /// ����ƥ��������ѡ��
    /// �ɶ�����Լ��������ɣ�������ϸ��ƥ����Ϣ�����Ŷ�
    /// </summary>
    public OverallMatchResult? MatchResult { get; private set; }

    /// <summary>
    /// ������������Ϣ
    /// �洢������ص���ʱ����
    /// </summary>
    public Dictionary<string, object> RequestContext { get; private set; }

    /// <summary>
    /// ��ѡ״̬
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private AdCandidate(
        string adId,
        AdType adType,
        Campaign campaign,
        string placementId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr = 0.0,
        double predictedCvr = 0.0,
        double weightScore = 0.0,
        OverallMatchResult? matchResult = null,
        Dictionary<string, object>? requestContext = null,
        string status = "created")
    {
        AdId = adId;
        AdType = adType;
        Campaign = campaign;
        CampaignId = campaign.Id.ToString(); // ��Campaign��ȡID
        PlacementId = placementId;
        CreativeId = creativeId;
        BidPrice = bidPrice;
        Creative = creative;
        Bidding = bidding;
        QualityScore = qualityScore;
        PredictedCtr = predictedCtr;
        PredictedCvr = predictedCvr;
        WeightScore = weightScore;
        MatchResult = matchResult;
        RequestContext = requestContext ?? new Dictionary<string, object>();
        Status = status;
    }

    /// <summary>
    /// ��������ѡ����
    /// </summary>
    public static AdCandidate Create(
        string adId,
        AdType adType,
        Campaign campaign,
        string placementId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr = 0.0,
        double predictedCvr = 0.0,
        double weightScore = 0.0,
        Dictionary<string, object>? requestContext = null)
    {
        ValidateParameters(adId, campaign, placementId, creativeId, bidPrice, creative, bidding, qualityScore);

        return new AdCandidate(
            adId,
            adType,
            campaign,
            placementId,
            creativeId,
            bidPrice,
            creative,
            bidding,
            qualityScore,
            predictedCtr,
            predictedCvr,
            weightScore,
            null, // matchResult ��ʼΪ�գ������ɶ�����Լ���������
            requestContext);
    }

    /// <summary>
    /// ���䵽���λ
    /// </summary>
    public void AssignToPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        PlacementId = placementId;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ȡ�������ã�ͨ��Campaign��ӷ��ʣ�
    /// </summary>
    public TargetingConfig GetTargetingConfig()
    {
        return Campaign.TargetingConfig;
    }

    /// <summary>
    /// ����ƥ����
    /// �ɶ�����Լ��������ã����ö���ƥ�����ϸ���
    /// </summary>
    public void SetMatchResult(OverallMatchResult matchResult)
    {
        MatchResult = matchResult ?? throw new ArgumentNullException(nameof(matchResult));
        Status = MatchResult.IsOverallMatch ? "matched" : "filtered";
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ���ƥ����
    /// </summary>
    public void ClearMatchResult()
    {
        MatchResult = null;
        Status = "created";
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ���º�ѡ״̬
    /// </summary>
    public void UpdateStatus(string status)
    {
        if (string.IsNullOrEmpty(status))
            throw new ArgumentException("״̬����Ϊ��", nameof(status));

        Status = status;
        UpdateLastModifiedTime();
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
    /// �����������������
    /// </summary>
    public void AddRequestContext(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�����ļ�����Ϊ��", nameof(key));

        RequestContext[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �Ƴ���������������
    /// </summary>
    public void RemoveRequestContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("�����ļ�����Ϊ��", nameof(key));

        if (RequestContext.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// ��������Է���
    /// ע�⣺ʵ�ʵĶ���ƥ�����Ӧ���ɶ�����Լ�����������
    /// ͨ��Campaign.TargetingConfig��ȡ����������Ϣ
    /// </summary>
    public double CalculateRelevanceScore(AdContext adContext)
    {
        if (adContext == null)
            throw new ArgumentNullException(nameof(adContext));

        // ������������
        double baseScore = (double)QualityScore.OverallScore;

        // �����ƥ������ʹ��ƥ�����
        if (MatchResult != null)
        {
            return (double)MatchResult.OverallScore;
        }

        // ����ƥ��������ⲿ���Լ���������
        // ��������ͨ�� Campaign.TargetingConfig ��ȡ
        // ���ﷵ�ػ�������������Ķ���ƥ���� ITargetingMatcher ����
        return baseScore;
    }

    /// <summary>
    /// ����Ƿ����Ͷ�������Ļ������
    /// ����Ķ���ƥ�����ⲿ���Լ���������ʹ��Campaign.TargetingConfig
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

        // ���Campaign�Ƿ����Ͷ��
        if (!Campaign.CanDeliver)
            return false;

        // �����ƥ����������Ƿ�����ƥ��
        if (MatchResult != null)
        {
            return MatchResult.IsOverallMatch;
        }

        // ע�⣺����Ķ���ƥ����Ӧ�����ⲿ������Լ�����ִ��
        // ��������ͨ�� Campaign.TargetingConfig ��ȡ
        // ����ֻ�������ĺϸ��Լ��
        return true;
    }

    /// <summary>
    /// ��ȡƥ�����
    /// </summary>
    public decimal GetMatchScore()
    {
        return MatchResult?.OverallScore ?? 0m;
    }

    /// <summary>
    /// ��ȡƥ�����Ŷ�
    /// </summary>
    public decimal? GetMatchConfidence()
    {
        return MatchResult?.Confidence?.ConfidenceScore;
    }

    /// <summary>
    /// �Ƿ�����Ч��ƥ����
    /// </summary>
    public bool HasValidMatchResult()
    {
        return MatchResult != null && MatchResult.IsValid();
    }

    /// <summary>
    /// ��ȡ����ָ��
    /// </summary>
    public Dictionary<string, object> GetPerformanceMetrics()
    {
        var metrics = new Dictionary<string, object>
        {
            ["AdId"] = AdId,
            ["CampaignId"] = CampaignId,
            ["PlacementId"] = PlacementId,
            ["BidPrice"] = BidPrice,
            ["PredictedCtr"] = PredictedCtr,
            ["PredictedCvr"] = PredictedCvr,
            ["WeightScore"] = WeightScore,
            ["QualityScore"] = QualityScore.OverallScore,
            ["ExpectedRevenue"] = (double)BidPrice * PredictedCtr * PredictedCvr,
            ["Status"] = Status
        };

        // ���ƥ�������ָ��
        if (MatchResult != null)
        {
            metrics["MatchScore"] = MatchResult.OverallScore;
            metrics["IsMatched"] = MatchResult.IsOverallMatch;
            metrics["MatchConfidence"] = MatchResult.Confidence.ConfidenceScore;
            metrics["MatchReasonCode"] = MatchResult.ReasonCode;
            metrics["TotalMatchCriteria"] = MatchResult.IndividualResults.Count;
            metrics["MatchedCriteria"] = MatchResult.IndividualResults.Count(r => r.IsMatch);
        }

        return metrics;
    }

    /// <summary>
    /// ��¡����ѡ����
    /// </summary>
    public AdCandidate Clone()
    {
        return new AdCandidate(
            AdId,
            AdType,
            Campaign,
            PlacementId,
            CreativeId,
            BidPrice,
            Creative,
            Bidding,
            QualityScore,
            PredictedCtr,
            PredictedCvr,
            WeightScore,
            MatchResult, // ƥ����Ҳ�ᱻ��¡��ǳ������
            new Dictionary<string, object>(RequestContext),
            Status
        );
    }

    /// <summary>
    /// ������֤
    /// </summary>
    private static void ValidateParameters(
        string adId,
        Campaign campaign,
        string placementId,
        string creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore)
    {
        if (string.IsNullOrEmpty(adId))
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        if (campaign == null)
            throw new ArgumentNullException(nameof(campaign));

        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (string.IsNullOrEmpty(creativeId))
            throw new ArgumentException("����ID����Ϊ��", nameof(creativeId));

        if (bidPrice < 0)
            throw new ArgumentException("���ۼ۸���Ϊ����", nameof(bidPrice));

        if (creative == null)
            throw new ArgumentNullException(nameof(creative));

        if (bidding == null)
            throw new ArgumentNullException(nameof(bidding));

        if (qualityScore == null)
            throw new ArgumentNullException(nameof(qualityScore));
    }
}