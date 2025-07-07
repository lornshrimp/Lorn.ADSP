using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ����ѡ����
/// </summary>
public record AdCandidate
{
    /// <summary>
    /// ���ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public required AdType AdType { get; init; }

    /// <summary>
    /// �ID
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// �����ID
    /// </summary>
    public required string AdvertiserId { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public required CreativeInfo Creative { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public TargetingConfig? Targeting { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public required BiddingInfo Bidding { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public QualityScore? QualityScore { get; init; }

    /// <summary>
    /// ���״̬
    /// </summary>
    public AdStatus Status { get; init; } = AdStatus.Active;

    /// <summary>
    /// ����������
    /// </summary>
    public IReadOnlyDictionary<string, object> Context { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Ȩ�ط�������������
    /// </summary>
    public decimal WeightScore { get; init; } = 0m;

    /// <summary>
    /// Ԥ�������
    /// </summary>
    public decimal? PredictedCtr { get; init; }

    /// <summary>
    /// Ԥ��ת����
    /// </summary>
    public decimal? PredictedCvr { get; init; }

    /// <summary>
    /// eCPMֵ
    /// </summary>
    public decimal? ECpm { get; init; }
}

/// <summary>
/// ��������
/// </summary>
public record TargetingConfig
{
    /// <summary>
    /// ����λ�ö���
    /// </summary>
    public GeoTargeting? GeoTargeting { get; init; }

    /// <summary>
    /// �˿����Զ���
    /// </summary>
    public DemographicTargeting? DemographicTargeting { get; init; }

    /// <summary>
    /// �豸����
    /// </summary>
    public DeviceTargeting? DeviceTargeting { get; init; }

    /// <summary>
    /// ʱ�䶨��
    /// </summary>
    public TimeTargeting? TimeTargeting { get; init; }

    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting { get; init; }

    /// <summary>
    /// �ؼ��ʶ���
    /// </summary>
    public IReadOnlyList<string> Keywords { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ��Ȥ��ǩ����
    /// </summary>
    public IReadOnlyList<string> InterestTags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public decimal Weight { get; init; } = 1.0m;
}

/// <summary>
/// ������Ϣ
/// </summary>
public record BiddingInfo
{
    /// <summary>
    /// ���۲���
    /// </summary>
    public required BiddingStrategy Strategy { get; init; }

    /// <summary>
    /// �������ۣ��֣�
    /// </summary>
    public required decimal BaseBidPrice { get; init; }

    /// <summary>
    /// �����ۣ��֣�
    /// </summary>
    public required decimal MaxBidPrice { get; init; }

    /// <summary>
    /// ��ǰ���ۣ��֣�
    /// </summary>
    public decimal CurrentBidPrice { get; init; }

    /// <summary>
    /// ���۵�������
    /// </summary>
    public decimal BidAdjustmentFactor { get; init; } = 1.0m;

    /// <summary>
    /// Ԥ����Ϣ
    /// </summary>
    public BudgetInfo? Budget { get; init; }

    /// <summary>
    /// ���۱�ǩ
    /// </summary>
    public IReadOnlyList<string> BidTags { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Ԥ����Ϣ
/// </summary>
public record BudgetInfo
{
    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; init; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; init; }

    /// <summary>
    /// ������Ԥ�㣨�֣�
    /// </summary>
    public decimal SpentBudget { get; init; }

    /// <summary>
    /// ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// Ԥ�������ٶȣ���/Сʱ��
    /// </summary>
    public decimal? BurnRate { get; init; }
}