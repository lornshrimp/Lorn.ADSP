using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// Ԥ��״ֵ̬����
/// </summary>
public record BudgetStatus
{
    /// <summary>
    /// �ID
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; init; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; init; }

    /// <summary>
    /// ������Ԥ�㣨�֣�
    /// </summary>
    public decimal SpentBudget { get; init; }

    /// <summary>
    /// ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// ����������Ԥ�㣨�֣�
    /// </summary>
    public decimal TodaySpent { get; init; }

    /// <summary>
    /// ����ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TodayRemaining => DailyBudget - TodaySpent;

    /// <summary>
    /// Ԥ��״̬
    /// </summary>
    public BudgetStatusType Status { get; init; } = BudgetStatusType.Active;

    /// <summary>
    /// �����ٶȣ���/Сʱ��
    /// </summary>
    public decimal? BurnRate { get; init; }

    /// <summary>
    /// Ԥ�ƺľ�ʱ��
    /// </summary>
    public DateTime? EstimatedExhaustionTime { get; init; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// �Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal amount)
    {
        return Status == BudgetStatusType.Active && 
               RemainingBudget >= amount && 
               TodayRemaining >= amount;
    }
}