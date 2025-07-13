using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 事务统计信息
/// </summary>
public class TransactionStatistics
{
    /// <summary>
    /// 事务ID
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 持续时间
    /// </summary>
    public TimeSpan Duration => CompletedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// 操作数量
    /// </summary>
    public int OperationCount { get; set; }

    /// <summary>
    /// 受影响的记录数
    /// </summary>
    public int AffectedRecords { get; set; }

    /// <summary>
    /// 锁定的资源数
    /// </summary>
    public int LockedResources { get; set; }

    /// <summary>
    /// 使用的内存（字节）
    /// </summary>
    public long MemoryUsage { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public TransactionStatus Status { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 补偿操作数量
    /// </summary>
    public int CompensationCount { get; set; }

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool IsCompleted => Status == TransactionStatus.Committed ||
                              Status == TransactionStatus.RolledBack ||
                              Status == TransactionStatus.Aborted;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccessful => Status == TransactionStatus.Committed;
}

/// <summary>
/// 分布式事务统计信息
/// </summary>
public class DistributedTransactionStatistics
{
    /// <summary>
    /// 全局事务ID
    /// </summary>
    public string GlobalTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 持续时间
    /// </summary>
    public TimeSpan Duration => CompletedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// 参与的范围数量
    /// </summary>
    public int ScopeCount { get; set; }

    /// <summary>
    /// 成功的范围数量
    /// </summary>
    public int SuccessfulScopes { get; set; }

    /// <summary>
    /// 失败的范围数量
    /// </summary>
    public int FailedScopes { get; set; }

    /// <summary>
    /// 总操作数量
    /// </summary>
    public int TotalOperations { get; set; }

    /// <summary>
    /// 总受影响记录数
    /// </summary>
    public int TotalAffectedRecords { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public DistributedTransactionStatus Status { get; set; }

    /// <summary>
    /// 协调器信息
    /// </summary>
    public string CoordinatorInfo { get; set; } = string.Empty;

    /// <summary>
    /// 范围统计列表
    /// </summary>
    public List<TransactionStatistics> ScopeStatistics { get; set; } = [];

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool IsCompleted => Status == DistributedTransactionStatus.Committed ||
                              Status == DistributedTransactionStatus.RolledBack ||
                              Status == DistributedTransactionStatus.Aborted;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccessful => Status == DistributedTransactionStatus.Committed;

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate => ScopeCount > 0 ? (double)SuccessfulScopes / ScopeCount : 0;
}

/// <summary>
/// 事务结果
/// </summary>
public class TransactionResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// 受影响的记录数
    /// </summary>
    public int AffectedRecords { get; set; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// 事务统计
    /// </summary>
    public TransactionStatistics? Statistics { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = [];

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <param name="affectedRecords">受影响的记录数</param>
    /// <param name="executionTime">执行时间</param>
    /// <returns>成功结果</returns>
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
    /// 创建失败结果
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="exception">异常信息</param>
    /// <param name="executionTime">执行时间</param>
    /// <returns>失败结果</returns>
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
/// 广告统计信息
/// </summary>
public class AdvertisementStatistics
{
    /// <summary>
    /// 总广告数量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 活跃广告数量
    /// </summary>
    public int ActiveCount { get; set; }

    /// <summary>
    /// 待审核广告数量
    /// </summary>
    public int PendingReviewCount { get; set; }

    /// <summary>
    /// 已拒绝广告数量
    /// </summary>
    public int RejectedCount { get; set; }

    /// <summary>
    /// 暂停的广告数量
    /// </summary>
    public int PausedCount { get; set; }

    /// <summary>
    /// 今日新增广告数量
    /// </summary>
    public int TodayCreatedCount { get; set; }

    /// <summary>
    /// 本周新增广告数量
    /// </summary>
    public int WeekCreatedCount { get; set; }

    /// <summary>
    /// 本月新增广告数量
    /// </summary>
    public int MonthCreatedCount { get; set; }

    /// <summary>
    /// 按类型分组的统计
    /// </summary>
    public Dictionary<string, int> CountByType { get; set; } = [];

    /// <summary>
    /// 按状态分组的统计
    /// </summary>
    public Dictionary<string, int> CountByStatus { get; set; } = [];

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 活动性能统计
/// </summary>
public class CampaignPerformanceStatistics
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 统计结束时间
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 展示次数
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// 点击次数
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// 转化次数
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// 总费用
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// 总收入
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// 点击率
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// 转化率
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;

    /// <summary>
    /// 平均点击费用
    /// </summary>
    public decimal AverageCostPerClick => Clicks > 0 ? TotalCost / Clicks : 0;

    /// <summary>
    /// 平均转化费用
    /// </summary>
    public decimal AverageCostPerConversion => Conversions > 0 ? TotalCost / Conversions : 0;

    /// <summary>
    /// 投资回报率
    /// </summary>
    public decimal ReturnOnInvestment => TotalCost > 0 ? (TotalRevenue - TotalCost) / TotalCost * 100 : 0;

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 活动预算使用情况
/// </summary>
public class CampaignBudgetUsage
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// 总预算
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// 日预算
    /// </summary>
    public decimal DailyBudget { get; set; }

    /// <summary>
    /// 已消费总额
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// 今日已消费
    /// </summary>
    public decimal TodaySpent { get; set; }

    /// <summary>
    /// 剩余预算
    /// </summary>
    public decimal RemainingBudget => TotalBudget - TotalSpent;

    /// <summary>
    /// 今日剩余预算
    /// </summary>
    public decimal TodayRemainingBudget => DailyBudget - TodaySpent;

    /// <summary>
    /// 预算使用率
    /// </summary>
    public decimal BudgetUtilizationRate => TotalBudget > 0 ? TotalSpent / TotalBudget * 100 : 0;

    /// <summary>
    /// 今日预算使用率
    /// </summary>
    public decimal TodayBudgetUtilizationRate => DailyBudget > 0 ? TodaySpent / DailyBudget * 100 : 0;

    /// <summary>
    /// 是否预算耗尽
    /// </summary>
    public bool IsBudgetExhausted => RemainingBudget <= 0;

    /// <summary>
    /// 是否今日预算耗尽
    /// </summary>
    public bool IsTodayBudgetExhausted => TodayRemainingBudget <= 0;

    /// <summary>
    /// 预算状态
    /// </summary>
    public string BudgetStatus
    {
        get
        {
            if (IsBudgetExhausted) return "预算已耗尽";
            if (IsTodayBudgetExhausted) return "今日预算已耗尽";
            if (BudgetUtilizationRate >= 90) return "预算即将耗尽";
            if (TodayBudgetUtilizationRate >= 90) return "今日预算即将耗尽";
            return "预算正常";
        }
    }

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 投放统计信息
/// </summary>
public class DeliveryStatistics
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public int CampaignId { get; set; }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 统计结束时间
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 投放记录总数
    /// </summary>
    public long TotalDeliveries { get; set; }

    /// <summary>
    /// 成功投放数
    /// </summary>
    public long SuccessfulDeliveries { get; set; }

    /// <summary>
    /// 失败投放数
    /// </summary>
    public long FailedDeliveries { get; set; }

    /// <summary>
    /// 展示次数
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// 点击次数
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// 转化次数
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// 总费用
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// 平均费用
    /// </summary>
    public decimal AverageCost => TotalDeliveries > 0 ? TotalCost / TotalDeliveries : 0;

    /// <summary>
    /// 成功率
    /// </summary>
    public decimal SuccessRate => TotalDeliveries > 0 ? (decimal)SuccessfulDeliveries / TotalDeliveries * 100 : 0;

    /// <summary>
    /// 点击率
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// 转化率
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime StatisticsTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 按小时投放统计
/// </summary>
public class HourlyDeliveryStatistics
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 小时（0-23）
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// 投放数量
    /// </summary>
    public long DeliveryCount { get; set; }

    /// <summary>
    /// 展示次数
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// 点击次数
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// 费用
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// 点击率
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;
}

/// <summary>
/// 按地域投放统计
/// </summary>
public class RegionalDeliveryStatistics
{
    /// <summary>
    /// 国家代码
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// 省份/州代码
    /// </summary>
    public string StateCode { get; set; } = string.Empty;

    /// <summary>
    /// 城市名称
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// 投放数量
    /// </summary>
    public long DeliveryCount { get; set; }

    /// <summary>
    /// 展示次数
    /// </summary>
    public long Impressions { get; set; }

    /// <summary>
    /// 点击次数
    /// </summary>
    public long Clicks { get; set; }

    /// <summary>
    /// 转化次数
    /// </summary>
    public long Conversions { get; set; }

    /// <summary>
    /// 费用
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// 点击率
    /// </summary>
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;

    /// <summary>
    /// 转化率
    /// </summary>
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;
}