using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
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
/// Ԥ����Ϣֵ����
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