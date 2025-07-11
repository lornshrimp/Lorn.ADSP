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
    /// 活动标识
    /// </summary>
    public string CampaignId { get; private set; }

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
    /// 定向策略
    /// </summary>
    public TargetingPolicy Targeting { get; private set; }

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
    /// 扩展上下文数据
    /// </summary>
    public Dictionary<string, object> Context { get; private set; }

    /// <summary>
    /// 私有构造函数
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
    /// 创建广告候选对象
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
    /// 添加上下文数据
    /// </summary>
    public void AddContext(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        Context[key] = value;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 移除上下文数据
    /// </summary>
    public void RemoveContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (Context.Remove(key))
        {
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 计算相关性分数
    /// 注意：实际的定向匹配计算应该由定向策略计算器来处理
    /// </summary>
    public double CalculateRelevanceScore(AdContext adContext)
    {
        if (adContext == null)
            throw new ArgumentNullException(nameof(adContext));

        // 基础质量分数
        double baseScore = (double)QualityScore.OverallScore;

        // 定向匹配分数由外部策略计算器计算
        // 这里返回基础分数，具体的定向匹配由 ITargetingMatcher 负责
        return baseScore;
    }

    /// <summary>
    /// 检查是否符合投放条件的基础检查
    /// 具体的定向匹配由外部策略计算器负责
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

        // 注意：具体的定向匹配检查应该由外部定向策略计算器执行
        // 这里只做基础的合格性检查
        return true;
    }

    /// <summary>
    /// 获取性能指标
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
    /// 参数验证
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
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (string.IsNullOrEmpty(campaignId))
            throw new ArgumentException("活动ID不能为空", nameof(campaignId));

        if (string.IsNullOrEmpty(creativeId))
            throw new ArgumentException("创意ID不能为空", nameof(creativeId));

        if (bidPrice < 0)
            throw new ArgumentException("竞价价格不能为负数", nameof(bidPrice));

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