using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告候选对象
/// </summary>
public record AdCandidate
{
    /// <summary>
    /// 广告ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// 广告类型
    /// </summary>
    public required AdType AdType { get; init; }

    /// <summary>
    /// 活动ID
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// 广告主ID
    /// </summary>
    public required string AdvertiserId { get; init; }

    /// <summary>
    /// 创意信息
    /// </summary>
    public required CreativeInfo Creative { get; init; }

    /// <summary>
    /// 定向配置
    /// </summary>
    public TargetingConfig? Targeting { get; init; }

    /// <summary>
    /// 竞价信息
    /// </summary>
    public required BiddingInfo Bidding { get; init; }

    /// <summary>
    /// 质量评分
    /// </summary>
    public QualityScore? QualityScore { get; init; }

    /// <summary>
    /// 广告状态
    /// </summary>
    public AdStatus Status { get; init; } = AdStatus.Active;

    /// <summary>
    /// 上下文数据
    /// </summary>
    public IReadOnlyDictionary<string, object> Context { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 权重分数（用于排序）
    /// </summary>
    public decimal WeightScore { get; init; } = 0m;

    /// <summary>
    /// 预估点击率
    /// </summary>
    public decimal? PredictedCtr { get; init; }

    /// <summary>
    /// 预估转化率
    /// </summary>
    public decimal? PredictedCvr { get; init; }

    /// <summary>
    /// eCPM值
    /// </summary>
    public decimal? ECpm { get; init; }
}

/// <summary>
/// 定向配置
/// </summary>
public record TargetingConfig
{
    /// <summary>
    /// 地理位置定向
    /// </summary>
    public GeoTargeting? GeoTargeting { get; init; }

    /// <summary>
    /// 人口属性定向
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; init; }

    /// <summary>
    /// 设备定向
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; init; }

    /// <summary>
    /// 时间定向
    /// </summary>
    public TimeTargeting? TimeTargeting { get; init; }

    /// <summary>
    /// 行为定向
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; init; }

    /// <summary>
    /// 关键词定向
    /// </summary>
    public IReadOnlyList<string> Keywords { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 兴趣标签定向
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 定向权重
    /// </summary>
    public decimal Weight { get; init; } = 1.0m;
}

/// <summary>
/// 竞价信息
/// </summary>
public record BiddingInfo
{
    /// <summary>
    /// 竞价策略
    /// </summary>
    public required BiddingStrategy Strategy { get; init; }

    /// <summary>
    /// 基础出价（分）
    /// </summary>
    public required decimal BaseBidPrice { get; init; }

    /// <summary>
    /// 最大出价（分）
    /// </summary>
    public required decimal MaxBidPrice { get; init; }

    /// <summary>
    /// 当前出价（分）
    /// </summary>
    public decimal CurrentBidPrice { get; init; }

    /// <summary>
    /// 出价调整因子
    /// </summary>
    public decimal BidAdjustmentFactor { get; init; } = 1.0m;

    /// <summary>
    /// 预算信息
    /// </summary>
    public BudgetInfo? Budget { get; init; }

    /// <summary>
    /// 竞价标签
    /// </summary>
    public IReadOnlyList<string> BidTags { get; init; } = Array.Empty<string>();
}

/// <summary>
/// 预算信息
/// </summary>
public record BudgetInfo
{
    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; init; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; init; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; init; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// 预算消耗速度（分/小时）
    /// </summary>
    public decimal? BurnRate { get; init; }
}