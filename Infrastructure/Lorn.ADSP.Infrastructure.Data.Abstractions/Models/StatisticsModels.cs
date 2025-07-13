using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ����ͳ����Ϣ
/// </summary>
public class TransactionStatistics
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public TimeSpan Duration => CompletedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// ��������
    /// </summary>
    public int OperationCount { get; set; }

    /// <summary>
    /// ��Ӱ��ļ�¼��
    /// </summary>
    public int AffectedRecords { get; set; }

    /// <summary>
    /// ��������Դ��
    /// </summary>
    public int LockedResources { get; set; }

    /// <summary>
    /// ʹ�õ��ڴ棨�ֽڣ�
    /// </summary>
    public long MemoryUsage { get; set; }

    /// <summary>
    /// ״̬
    /// </summary>
    public TransactionStatus Status { get; set; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// ������������
    /// </summary>
    public int CompensationCount { get; set; }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsCompleted => Status == TransactionStatus.Committed ||
                              Status == TransactionStatus.RolledBack ||
                              Status == TransactionStatus.Aborted;

    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccessful => Status == TransactionStatus.Committed;
}

/// <summary>
/// �ֲ�ʽ����ͳ����Ϣ
/// </summary>
public class DistributedTransactionStatistics
{
    /// <summary>
    /// ȫ������ID
    /// </summary>
    public string GlobalTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public TimeSpan Duration => CompletedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// ����ķ�Χ����
    /// </summary>
    public int ScopeCount { get; set; }

    /// <summary>
    /// �ɹ��ķ�Χ����
    /// </summary>
    public int SuccessfulScopes { get; set; }

    /// <summary>
    /// ʧ�ܵķ�Χ����
    /// </summary>
    public int FailedScopes { get; set; }

    /// <summary>
    /// �ܲ�������
    /// </summary>
    public int TotalOperations { get; set; }

    /// <summary>
    /// ����Ӱ���¼��
    /// </summary>
    public int TotalAffectedRecords { get; set; }

    /// <summary>
    /// ״̬
    /// </summary>
    public DistributedTransactionStatus Status { get; set; }

    /// <summary>
    /// Э������Ϣ
    /// </summary>
    public string CoordinatorInfo { get; set; } = string.Empty;

    /// <summary>
    /// ��Χͳ���б�
    /// </summary>
    public List<TransactionStatistics> ScopeStatistics { get; set; } = [];

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsCompleted => Status == DistributedTransactionStatus.Committed ||
                              Status == DistributedTransactionStatus.RolledBack ||
                              Status == DistributedTransactionStatus.Aborted;

    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccessful => Status == DistributedTransactionStatus.Committed;

    /// <summary>
    /// �ɹ���
    /// </summary>
    public double SuccessRate => ScopeCount > 0 ? (double)SuccessfulScopes / ScopeCount : 0;
}

/// <summary>
/// ������
/// </summary>
public class TransactionResult
{
    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// �쳣��Ϣ
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// ��Ӱ��ļ�¼��
    /// </summary>
    public int AffectedRecords { get; set; }

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// ����ͳ��
    /// </summary>
    public TransactionStatistics? Statistics { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = [];

    /// <summary>
    /// �����ɹ����
    /// </summary>
    /// <param name="affectedRecords">��Ӱ��ļ�¼��</param>
    /// <param name="executionTime">ִ��ʱ��</param>
    /// <returns>�ɹ����</returns>
    public static TransactionResult Success(int affectedRecords = 0, TimeSpan executionTime = default)
    {
        return new TransactionResult
        {
            IsSuccess = true,
            AffectedRecords = affectedRecords,
            ExecutionTime = executionTime
        };
    }

    /// <summary>
    /// ����ʧ�ܽ��
    /// </summary>
    /// <param name="errorMessage">������Ϣ</param>
    /// <param name="exception">�쳣��Ϣ</param>
    /// <param name="executionTime">ִ��ʱ��</param>
    /// <returns>ʧ�ܽ��</returns>
    public static TransactionResult Failure(string errorMessage, Exception? exception = null, TimeSpan executionTime = default)
    {
        return new TransactionResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Exception = exception,
            ExecutionTime = executionTime
        };
    }
}

/// <summary>
/// ���ͳ����Ϣ
/// </summary>
public class AdvertisementStatistics
{
    /// <summary>
    /// �ܹ������
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// ��Ծ�������
    /// </summary>
    public int ActiveCount { get; set; }

    /// <summary>
    /// ����˹������
    /// </summary>
    public int PendingReviewCount { get; set; }

    /// <summary>
    /// �Ѿܾ��������
    /// </summary>
    public int RejectedCount { get; set; }

    /// <summary>
    /// ��ͣ�Ĺ������
    /// </summary>
    public int PausedCount { get; set; }

    /// <summary>
    /// ���������������
    /// </summary>
    public int TodayCreatedCount { get; set; }

    /// <summary>
    /// ���������������
    /// </summary>
    public int WeekCreatedCount { get; set; }

    /// <summary>
    /// ���������������
    /// </summary>
    public int MonthCreatedCount { get; set; }

    /// <summary>
    /// �����ͷ����ͳ��
    /// </summary>
    public Dictionary<string, int> CountByType { get; set; } = [];

    /// <summary>
    /// ��״̬�����ͳ��
    /// </summary>
    public Dictionary<string, int> CountByStatus { get; set; } = [];

    /// <summary>
    /// ͳ��ʱ��
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// �����ͳ��
/// </summary>
public class CampaignPerformanceStatistics
{
    /// <summary>
    /// �ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// ͳ�ƿ�ʼʱ��
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// ͳ�ƽ���ʱ��
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// չʾ����
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// ת������
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// �ܷ���
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// ������
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// �����
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// ת����
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;

    /// <summary>
    /// ƽ���������
    /// </summary>
    public decimal AverageCostPerClick => Clicks > 0 ? TotalCost / Clicks : 0;

    /// <summary>
    /// ƽ��ת������
    /// </summary>
    public decimal AverageCostPerConversion => Conversions > 0 ? TotalCost / Conversions : 0;

    /// <summary>
    /// Ͷ�ʻر���
    /// </summary>
    public decimal ReturnOnInvestment => TotalCost > 0 ? (TotalRevenue - TotalCost) / TotalCost * 100 : 0;

    /// <summary>
    /// ͳ��ʱ��
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// �Ԥ��ʹ�����
/// </summary>
public class CampaignBudgetUsage
{
    /// <summary>
    /// �ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// ��Ԥ��
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// ��Ԥ��
    /// </summary>
    public decimal DailyBudget { get; set; }

    /// <summary>
    /// �������ܶ�
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// ����������
    /// </summary>
    public decimal TodaySpent { get; set; }

    /// <summary>
    /// ʣ��Ԥ��
    /// </summary>
    public decimal RemainingBudget => TotalBudget - TotalSpent;

    /// <summary>
    /// ����ʣ��Ԥ��
    /// </summary>
    public decimal TodayRemainingBudget => DailyBudget - TodaySpent;

    /// <summary>
    /// Ԥ��ʹ����
    /// </summary>
    public decimal BudgetUtilizationRate => TotalBudget > 0 ? TotalSpent / TotalBudget * 100 : 0;

    /// <summary>
    /// ����Ԥ��ʹ����
    /// </summary>
    public decimal TodayBudgetUtilizationRate => DailyBudget > 0 ? TodaySpent / DailyBudget * 100 : 0;

    /// <summary>
    /// �Ƿ�Ԥ��ľ�
    /// </summary>
    public bool IsBudgetExhausted => RemainingBudget <= 0;

    /// <summary>
    /// �Ƿ����Ԥ��ľ�
    /// </summary>
    public bool IsTodayBudgetExhausted => TodayRemainingBudget <= 0;

    /// <summary>
    /// Ԥ��״̬
    /// </summary>
    public string BudgetStatus
    {
        get
        {
            if (IsBudgetExhausted) return "Ԥ���Ѻľ�";
            if (IsTodayBudgetExhausted) return "����Ԥ���Ѻľ�";
            if (BudgetUtilizationRate >= 90) return "Ԥ�㼴���ľ�";
            if (TodayBudgetUtilizationRate >= 90) return "����Ԥ�㼴���ľ�";
            return "Ԥ������";
        }
    }

    /// <summary>
    /// ͳ��ʱ��
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Ͷ��ͳ����Ϣ
/// </summary>
public class DeliveryStatistics
{
    /// <summary>
    /// �ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// ͳ�ƿ�ʼʱ��
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// ͳ�ƽ���ʱ��
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Ͷ�ż�¼����
    /// </summary>
    public long TotalDeliveries { get; set; }

    /// <summary>
    /// �ɹ�Ͷ����
    /// </summary>
    public long SuccessfulDeliveries { get; set; }

    /// <summary>
    /// ʧ��Ͷ����
    /// </summary>
    public long FailedDeliveries { get; set; }

    /// <summary>
    /// չʾ����
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// ת������
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// �ܷ���
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// ƽ������
    /// </summary>
    public decimal AverageCost => TotalDeliveries > 0 ? TotalCost / TotalDeliveries : 0;

    /// <summary>
    /// �ɹ���
    /// </summary>
    public decimal SuccessRate => TotalDeliveries > 0 ? (decimal)SuccessfulDeliveries / TotalDeliveries * 100 : 0;

    /// <summary>
    /// �����
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// ת����
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;

    /// <summary>
    /// ͳ��ʱ��
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ��СʱͶ��ͳ��
/// </summary>
public class HourlyDeliveryStatistics
{
    /// <summary>
    /// ����
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Сʱ��0-23��
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// Ͷ������
    /// </summary>
    public long DeliveryCount { get; set; }

    /// <summary>
    /// չʾ����
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// �����
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;
}

/// <summary>
/// ������Ͷ��ͳ��
/// </summary>
public class RegionalDeliveryStatistics
{
    /// <summary>
    /// ���Ҵ���
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// ʡ��/�ݴ���
    /// </summary>
    public string StateCode { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// Ͷ������
    /// </summary>
    public long DeliveryCount { get; set; }

    /// <summary>
    /// չʾ����
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// ת������
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// �����
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// ת����
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;
}