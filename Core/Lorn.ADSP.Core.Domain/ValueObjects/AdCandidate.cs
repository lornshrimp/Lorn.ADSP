using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 广告候选值对象
/// 在广告投放流程中的候选广告，是广告召回、过滤、排序阶段的核心处理对象
/// 设计原则：不可变、基于值相等、临时存在（召回→过滤→排序→投放）
/// </summary>
public class AdCandidate : ValueObject
{
    /// <summary>
    /// 广告唯一标识
    /// </summary>
    public Guid AdId { get; }

    /// <summary>
    /// 广告类型
    /// </summary>
    public AdType AdType { get; }

    /// <summary>
    /// 活动标识（通过此ID可以获取到Campaign，再获取TargetingConfig）
    /// </summary>
    public Guid CampaignId { get; }

    /// <summary>
    /// 广告位标识
    /// 目标投放的广告位ID，用于标识该候选广告要投放到哪个广告位
    /// </summary>
    public Guid PlacementId { get; }

    /// <summary>
    /// Campaign实体引用（聚合关系）
    /// AdCandidate通过Campaign来访问TargetingConfig，而不是直接持有TargetingConfig
    /// </summary>
    public Campaign Campaign { get; }

    /// <summary>
    /// 创意标识
    /// </summary>
    public Guid CreativeId { get; }

    /// <summary>
    /// 竞价价格
    /// </summary>
    public decimal BidPrice { get; }

    /// <summary>
    /// 创意信息
    /// </summary>
    public CreativeInfo Creative { get; }

    /// <summary>
    /// 竞价信息
    /// </summary>
    public BiddingInfo Bidding { get; }

    /// <summary>
    /// 质量得分
    /// </summary>
    public QualityScore QualityScore { get; }

    /// <summary>
    /// 预测点击率
    /// </summary>
    public double PredictedCtr { get; }

    /// <summary>
    /// 预测转化率
    /// </summary>
    public double PredictedCvr { get; }

    /// <summary>
    /// 权重分数
    /// </summary>
    public double WeightScore { get; }

    /// <summary>
    /// 定向匹配结果（可选）
    /// 可由定向策略计算器生成，包含详细的匹配信息和置信度
    /// </summary>
    public OverallMatchResult? MatchResult { get; }

    /// <summary>
    /// 请求上下文信息集合 - 集合导航属性
    /// 替代原有的Dictionary<string, object>结构
    /// 存储请求相关的临时数据
    /// </summary>
    public IReadOnlyList<RequestContextItem> RequestContextItems { get; }

    /// <summary>
    /// 候选状态
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// 私有构造函数，强制使用工厂方法创建
    /// </summary>
    private AdCandidate(
        Guid adId,
        AdType adType,
        Campaign campaign,
        Guid placementId,
        Guid creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr,
        double predictedCvr,
        double weightScore,
        OverallMatchResult? matchResult,
        IReadOnlyList<RequestContextItem> requestContextItems,
        string status)
    {
        AdId = adId;
        AdType = adType;
        Campaign = campaign;
        CampaignId = campaign.Id;
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
        RequestContextItems = requestContextItems;
        Status = status;
    }

    /// <summary>
    /// 创建新的广告候选
    /// </summary>
    public static AdCandidate Create(
        Guid adId,
        AdType adType,
        Campaign campaign,
        Guid placementId,
        Guid creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore,
        double predictedCtr = 0.0,
        double predictedCvr = 0.0,
        double weightScore = 0.0,
        IList<RequestContextItem>? requestContextItems = null)
    {
        ValidateParameters(adId, campaign, placementId, creativeId, bidPrice, creative, bidding, qualityScore);

        var readOnlyContextItems = (requestContextItems ?? new List<RequestContextItem>()).ToList().AsReadOnly();

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
            null, // matchResult 初始为空，稍后可由定向策略计算器设置
            readOnlyContextItems,
            "created");
    }

    /// <summary>
    /// 获取定向配置（通过Campaign间接访问）
    /// </summary>
    public TargetingConfig GetTargetingConfig()
    {
        return Campaign.TargetingConfig;
    }

    /// <summary>
    /// 设置匹配结果（返回新的AdCandidate实例）
    /// 可由定向策略计算器设置，含有定向匹配的详细信息
    /// </summary>
    public AdCandidate WithMatchResult(OverallMatchResult matchResult)
    {
        if (matchResult == null)
            throw new ArgumentNullException(nameof(matchResult));

        var newStatus = matchResult.IsOverallMatch ? "matched" : "filtered";

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
            matchResult,
            RequestContextItems,
            newStatus);
    }

    /// <summary>
    /// 清除匹配结果（返回新的AdCandidate实例）
    /// </summary>
    public AdCandidate WithoutMatchResult()
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
            null,
            RequestContextItems,
            "created");
    }

    /// <summary>
    /// 更新候选状态（返回新的AdCandidate实例）
    /// </summary>
    public AdCandidate WithStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("状态不能为空", nameof(status));

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
            MatchResult,
            RequestContextItems,
            status);
    }

    /// <summary>
    /// 更新竞价价格（返回新的AdCandidate实例）
    /// </summary>
    public AdCandidate WithBidPrice(decimal newBidPrice)
    {
        if (newBidPrice <= 0)
            throw new ArgumentException("竞价价格必须大于0", nameof(newBidPrice));

        return new AdCandidate(
            AdId,
            AdType,
            Campaign,
            PlacementId,
            CreativeId,
            newBidPrice,
            Creative,
            Bidding,
            QualityScore,
            PredictedCtr,
            PredictedCvr,
            WeightScore,
            MatchResult,
            RequestContextItems,
            Status);
    }

    /// <summary>
    /// 更新质量得分（返回新的AdCandidate实例）
    /// </summary>
    public AdCandidate WithQualityScore(QualityScore qualityScore)
    {
        if (qualityScore == null)
            throw new ArgumentNullException(nameof(qualityScore));

        return new AdCandidate(
            AdId,
            AdType,
            Campaign,
            PlacementId,
            CreativeId,
            BidPrice,
            Creative,
            Bidding,
            qualityScore,
            PredictedCtr,
            PredictedCvr,
            WeightScore,
            MatchResult,
            RequestContextItems,
            Status);
    }

    /// <summary>
    /// 添加请求上下文项（返回新的AdCandidate实例）
    /// </summary>
    public AdCandidate WithRequestContextItem(RequestContextItem contextItem)
    {
        if (contextItem == null)
            throw new ArgumentNullException(nameof(contextItem));

        var newContextItems = RequestContextItems.ToList();
        newContextItems.Add(contextItem);

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
            MatchResult,
            newContextItems.AsReadOnly(),
            Status);
    }

    /// <summary>
    /// 计算广告相关度分数
    /// 注意：实际的定向匹配计算应该由定向策略计算器处理
    /// 通过Campaign.TargetingConfig获取定向配置信息
    /// </summary>
    public double CalculateRelevanceScore(AdContext adContext)
    {
        if (adContext == null)
            throw new ArgumentNullException(nameof(adContext));

        // 基础质量分数
        double baseScore = (double)QualityScore.OverallScore;

        // 如果有匹配结果，直接使用匹配分数
        if (MatchResult != null)
        {
            return (double)MatchResult.OverallScore;
        }

        // 否则依赖外部定向策略计算器进行匹配
        // 这里返回基础分数，实际匹配逻辑由 Campaign.TargetingConfig 提供
        return baseScore;
    }

    /// <summary>
    /// 检查是否适合投放到指定的广告位
    /// 根据定向匹配结果和外部定向策略计算器判断使用Campaign.TargetingConfig
    /// </summary>
    public bool IsEligibleForPlacement(AdContext adContext)
    {
        if (adContext == null)
            return false;

        // 检查基本条件
        if (BidPrice <= 0)
            return false;

        // 检查质量得分
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

        // 否则需要外部定向策略计算器判断
        // 这里返回基础合格判断，实际匹配由定向策略处理
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
    public IReadOnlyDictionary<string, object> GetPerformanceMetrics()
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

        // 添加匹配相关指标
        if (MatchResult != null)
        {
            metrics["MatchScore"] = MatchResult.OverallScore;
            metrics["IsMatched"] = MatchResult.IsOverallMatch;
            metrics["MatchConfidence"] = MatchResult.Confidence?.ConfidenceScore ?? 0m;
            metrics["MatchReasonCode"] = MatchResult.ReasonCode;
            metrics["TotalMatchCriteria"] = MatchResult.IndividualResults.Count;
            metrics["MatchedCriteria"] = MatchResult.IndividualResults.Count(r => r.IsMatch);
        }

        return metrics;
    }

    /// <summary>
    /// 克隆广告候选
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
            MatchResult,
            RequestContextItems,
            Status);
    }

    /// <summary>
    /// 参数验证
    /// </summary>
    private static void ValidateParameters(
        Guid adId,
        Campaign campaign,
        Guid placementId,
        Guid creativeId,
        decimal bidPrice,
        CreativeInfo creative,
        BiddingInfo bidding,
        QualityScore qualityScore)
    {
        if (adId == Guid.Empty)
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (campaign == null)
            throw new ArgumentNullException(nameof(campaign));

        if (placementId == Guid.Empty)
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (creativeId == Guid.Empty)
            throw new ArgumentException("创意ID不能为空", nameof(creativeId));

        if (bidPrice <= 0)
            throw new ArgumentException("竞价价格必须大于0", nameof(bidPrice));

        if (creative == null)
            throw new ArgumentNullException(nameof(creative));

        if (bidding == null)
            throw new ArgumentNullException(nameof(bidding));

        if (qualityScore == null)
            throw new ArgumentNullException(nameof(qualityScore));
    }

    /// <summary>
    /// 实现值对象的相等性比较组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AdId;
        yield return AdType;
        yield return CampaignId;
        yield return PlacementId;
        yield return CreativeId;
        yield return BidPrice;
        yield return Creative;
        yield return Bidding;
        yield return QualityScore;
        yield return PredictedCtr;
        yield return PredictedCvr;
        yield return WeightScore;
        yield return MatchResult?.ToString() ?? string.Empty;
        yield return Status;

        // 请求上下文项按键排序
        foreach (var item in RequestContextItems.OrderBy(i => i.Key))
        {
            yield return item;
        }
    }
}
