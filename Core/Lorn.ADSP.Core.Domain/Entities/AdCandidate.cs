using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告候选实体
/// 在广告投放流程中的候选广告，是广告召回、过滤、排序阶段的核心处理对象
/// 生命周期：召回→过滤→排序→投放，每个阶段都可能修改候选对象的属性
/// </summary>
public class AdCandidate : EntityBase
{
    /// <summary>
    /// 广告唯一标识
    /// </summary>
    public string AdId { get; private set; }

    /// <summary>
    /// 广告类型
    /// </summary>
    public AdType AdType { get; private set; }

    /// <summary>
    /// 活动标识（通过此ID可以获取到Campaign，再获取TargetingConfig）
    /// </summary>
    public string CampaignId { get; private set; }

    /// <summary>
    /// 广告位标识
    /// 目标投放的广告位ID，用于标识该候选广告要投放到哪个广告位
    /// </summary>
    public string PlacementId { get; private set; }

    /// <summary>
    /// Campaign实体引用（聚合关系）
    /// AdCandidate通过Campaign来访问TargetingConfig，而不是直接持有TargetingConfig
    /// </summary>
    public Campaign Campaign { get; private set; }

    /// <summary>
    /// 创意标识
    /// </summary>
    public string CreativeId { get; private set; }

    /// <summary>
    /// 竞价价格
    /// </summary>
    public decimal BidPrice { get; private set; }

    /// <summary>
    /// 创意信息
    /// </summary>
    public CreativeInfo Creative { get; private set; }

    /// <summary>
    /// 竞价信息
    /// </summary>
    public BiddingInfo Bidding { get; private set; }

    /// <summary>
    /// 质量评分
    /// </summary>
    public QualityScore QualityScore { get; private set; }

    /// <summary>
    /// 预估点击率
    /// </summary>
    public double PredictedCtr { get; private set; }

    /// <summary>
    /// 预估转化率
    /// </summary>
    public double PredictedCvr { get; private set; }

    /// <summary>
    /// 权重分数
    /// </summary>
    public double WeightScore { get; private set; }

    /// <summary>
    /// 定向匹配结果（可选）
    /// 由定向策略计算器生成，包含详细的匹配信息和置信度
    /// </summary>
    public OverallMatchResult? MatchResult { get; private set; }

    /// <summary>
    /// 请求上下文信息
    /// 存储请求相关的临时数据
    /// </summary>
    public Dictionary<string, object> RequestContext { get; private set; }

    /// <summary>
    /// 候选状态
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// 私有构造函数
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
        CampaignId = campaign.Id.ToString(); // 从Campaign获取ID
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
    /// 创建广告候选对象
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
            null, // matchResult 初始为空，后续由定向策略计算器设置
            requestContext);
    }

    /// <summary>
    /// 分配到广告位
    /// </summary>
    public void AssignToPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        PlacementId = placementId;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 获取定向配置（通过Campaign间接访问）
    /// </summary>
    public TargetingConfig GetTargetingConfig()
    {
        return Campaign.TargetingConfig;
    }

    /// <summary>
    /// 设置匹配结果
    /// 由定向策略计算器调用，设置定向匹配的详细结果
    /// </summary>
    public void SetMatchResult(OverallMatchResult matchResult)
    {
        MatchResult = matchResult ?? throw new ArgumentNullException(nameof(matchResult));
        Status = MatchResult.IsOverallMatch ? "matched" : "filtered";
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 清除匹配结果
    /// </summary>
    public void ClearMatchResult()
    {
        MatchResult = null;
        Status = "created";
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新候选状态
    /// </summary>
    public void UpdateStatus(string status)
    {
        if (string.IsNullOrEmpty(status))
            throw new ArgumentException("状态不能为空", nameof(status));

        Status = status;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新竞价价格
    /// </summary>
    public void UpdateBidPrice(decimal newBidPrice)
    {
        if (newBidPrice < 0)
            throw new ArgumentException("竞价价格不能为负数", nameof(newBidPrice));

        BidPrice = newBidPrice;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新预估点击率
    /// </summary>
    public void UpdatePredictedCtr(double predictedCtr)
    {
        if (predictedCtr < 0 || predictedCtr > 1)
            throw new ArgumentException("预估点击率必须在0-1之间", nameof(predictedCtr));

        PredictedCtr = predictedCtr;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新预估转化率
    /// </summary>
    public void UpdatePredictedCvr(double predictedCvr)
    {
        if (predictedCvr < 0 || predictedCvr > 1)
            throw new ArgumentException("预估转化率必须在0-1之间", nameof(predictedCvr));

        PredictedCvr = predictedCvr;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新权重分数
    /// </summary>
    public void UpdateWeightScore(double weightScore)
    {
        if (weightScore < 0)
            throw new ArgumentException("权重分数不能为负数", nameof(weightScore));

        WeightScore = weightScore;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新质量评分
    /// </summary>
    public void UpdateQualityScore(QualityScore qualityScore)
    {
        QualityScore = qualityScore ?? throw new ArgumentNullException(nameof(qualityScore));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加请求上下文数据
    /// </summary>
    public void AddRequestContext(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        RequestContext[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 移除请求上下文数据
    /// </summary>
    public void RemoveRequestContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (RequestContext.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 计算相关性分数
    /// 注意：实际的定向匹配计算应该由定向策略计算器来处理
    /// 通过Campaign.TargetingConfig获取定向配置信息
    /// </summary>
    public double CalculateRelevanceScore(AdContext adContext)
    {
        if (adContext == null)
            throw new ArgumentNullException(nameof(adContext));

        // 基础质量分数
        double baseScore = (double)QualityScore.OverallScore;

        // 如果有匹配结果，使用匹配分数
        if (MatchResult != null)
        {
            return (double)MatchResult.OverallScore;
        }

        // 定向匹配分数由外部策略计算器计算
        // 定向配置通过 Campaign.TargetingConfig 获取
        // 这里返回基础分数，具体的定向匹配由 ITargetingMatcher 负责
        return baseScore;
    }

    /// <summary>
    /// 检查是否符合投放条件的基础检查
    /// 具体的定向匹配由外部策略计算器负责，使用Campaign.TargetingConfig
    /// </summary>
    public bool IsEligibleForPlacement(AdContext adContext)
    {
        if (adContext == null)
            return false;

        // 基础条件检查
        if (BidPrice <= 0)
            return false;

        // 检查质量分数
        if (QualityScore.OverallScore < 0.1m)
            return false;

        // 检查Campaign是否可以投放
        if (!Campaign.CanDeliver)
            return false;

        // 如果有匹配结果，检查是否总体匹配
        if (MatchResult != null)
        {
            return MatchResult.IsOverallMatch;
        }

        // 注意：具体的定向匹配检查应该由外部定向策略计算器执行
        // 定向配置通过 Campaign.TargetingConfig 获取
        // 这里只做基础的合格性检查
        return true;
    }

    /// <summary>
    /// 获取匹配分数
    /// </summary>
    public decimal GetMatchScore()
    {
        return MatchResult?.OverallScore ?? 0m;
    }

    /// <summary>
    /// 获取匹配置信度
    /// </summary>
    public decimal? GetMatchConfidence()
    {
        return MatchResult?.Confidence?.ConfidenceScore;
    }

    /// <summary>
    /// 是否有有效的匹配结果
    /// </summary>
    public bool HasValidMatchResult()
    {
        return MatchResult != null && MatchResult.IsValid();
    }

    /// <summary>
    /// 获取性能指标
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

        // 添加匹配结果相关指标
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
    /// 克隆广告候选对象
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
            MatchResult, // 匹配结果也会被克隆（浅拷贝）
            new Dictionary<string, object>(RequestContext),
            Status
        );
    }

    /// <summary>
    /// 参数验证
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
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (campaign == null)
            throw new ArgumentNullException(nameof(campaign));

        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (string.IsNullOrEmpty(creativeId))
            throw new ArgumentException("创意ID不能为空", nameof(creativeId));

        if (bidPrice < 0)
            throw new ArgumentException("竞价价格不能为负数", nameof(bidPrice));

        if (creative == null)
            throw new ArgumentNullException(nameof(creative));

        if (bidding == null)
            throw new ArgumentNullException(nameof(bidding));

        if (qualityScore == null)
            throw new ArgumentNullException(nameof(qualityScore));
    }
}