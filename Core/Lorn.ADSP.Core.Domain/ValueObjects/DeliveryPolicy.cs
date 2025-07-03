using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Shared.Constants;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 投放策略值对象
/// </summary>
public class DeliveryPolicy : ValueObject
{
    /// <summary>
    /// 投放模式
    /// </summary>
    public DeliveryMode DeliveryMode { get; private set; }

    /// <summary>
    /// 出价策略
    /// </summary>
    public BiddingStrategy BiddingStrategy { get; private set; }

    /// <summary>
    /// 基础出价价格（分）
    /// </summary>
    public decimal BaseBidPrice { get; private set; }

    /// <summary>
    /// 最大出价价格（分）
    /// </summary>
    public decimal MaxBidPrice { get; private set; }

    /// <summary>
    /// 每日频次上限
    /// </summary>
    public int FrequencyCapDaily { get; private set; }

    /// <summary>
    /// 每小时频次上限
    /// </summary>
    public int FrequencyCapHourly { get; private set; }

    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// 投放开始时间
    /// </summary>
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// 投放结束时间
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// 出价调整因子
    /// </summary>
    public decimal BidAdjustmentFactor { get; private set; } = 1.0m;

    /// <summary>
    /// 是否启用智能出价
    /// </summary>
    public bool EnableSmartBidding { get; private set; } = false;

    /// <summary>
    /// 投放优先级（1-10，数字越大优先级越高）
    /// </summary>
    public int Priority { get; private set; } = 5;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private DeliveryPolicy() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeliveryPolicy(
        DeliveryMode deliveryMode,
        BiddingStrategy biddingStrategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        int frequencyCapDaily = DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
        int frequencyCapHourly = DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
        decimal dailyBudget = DefaultValues.Budget.DefaultDailyBudget * 100, // 转换为分
        decimal totalBudget = DefaultValues.Budget.DefaultTotalBudget * 100, // 转换为分
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? timeZone = null,
        decimal bidAdjustmentFactor = 1.0m,
        bool enableSmartBidding = false,
        int priority = 5)
    {
        ValidateInputs(baseBidPrice, maxBidPrice, frequencyCapDaily, frequencyCapHourly, 
                      dailyBudget, totalBudget, priority);

        DeliveryMode = deliveryMode;
        BiddingStrategy = biddingStrategy;
        BaseBidPrice = baseBidPrice;
        MaxBidPrice = maxBidPrice;
        FrequencyCapDaily = frequencyCapDaily;
        FrequencyCapHourly = frequencyCapHourly;
        DailyBudget = dailyBudget;
        TotalBudget = totalBudget;
        StartTime = startTime;
        EndTime = endTime;
        TimeZone = timeZone;
        BidAdjustmentFactor = bidAdjustmentFactor;
        EnableSmartBidding = enableSmartBidding;
        Priority = priority;
    }

    /// <summary>
    /// 创建保量投放策略
    /// </summary>
    public static DeliveryPolicy CreateGuaranteed(
        decimal baseBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        DateTime? startTime = null,
        DateTime? endTime = null)
    {
        return new DeliveryPolicy(
            DeliveryMode.Guaranteed,
            BiddingStrategy.FixedBid,
            baseBidPrice,
            baseBidPrice, // 保量投放时最大出价等于基础出价
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            startTime,
            endTime,
            priority: 8 // 保量投放优先级较高
        );
    }

    /// <summary>
    /// 创建竞价投放策略
    /// </summary>
    public static DeliveryPolicy CreateBidding(
        BiddingStrategy biddingStrategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        bool enableSmartBidding = true)
    {
        return new DeliveryPolicy(
            DeliveryMode.Bidding,
            biddingStrategy,
            baseBidPrice,
            maxBidPrice,
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            enableSmartBidding: enableSmartBidding,
            priority: 5 // 竞价投放默认优先级
        );
    }

    /// <summary>
    /// 创建混合投放策略
    /// </summary>
    public static DeliveryPolicy CreateMixed(
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        int guaranteedPriority = 7)
    {
        return new DeliveryPolicy(
            DeliveryMode.Mixed,
            BiddingStrategy.AutoBid,
            baseBidPrice,
            maxBidPrice,
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            enableSmartBidding: true,
            priority: guaranteedPriority
        );
    }

    /// <summary>
    /// 计算实际出价
    /// </summary>
    public decimal CalculateActualBid(decimal qualityScore = 1.0m, decimal marketPrice = 0m)
    {
        decimal adjustedBaseBid = BaseBidPrice * BidAdjustmentFactor * qualityScore;

        return BiddingStrategy switch
        {
            BiddingStrategy.FixedBid => Math.Min(adjustedBaseBid, MaxBidPrice),
            BiddingStrategy.AutoBid => CalculateAutoBid(adjustedBaseBid, marketPrice),
            BiddingStrategy.TargetCPA => CalculateTargetCPABid(adjustedBaseBid),
            BiddingStrategy.TargetROAS => CalculateTargetROASBid(adjustedBaseBid),
            BiddingStrategy.MaximizeClicks => CalculateMaximizeClicksBid(adjustedBaseBid),
            BiddingStrategy.MaximizeConversions => CalculateMaximizeConversionsBid(adjustedBaseBid),
            _ => Math.Min(adjustedBaseBid, MaxBidPrice)
        };
    }

    /// <summary>
    /// 判断是否在投放时间范围内
    /// </summary>
    public bool IsWithinDeliveryTime(DateTime currentTime)
    {
        var targetTime = TimeZone != null ? 
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTime, TimeZone) : 
            currentTime;

        if (StartTime.HasValue && targetTime < StartTime.Value)
            return false;

        if (EndTime.HasValue && targetTime > EndTime.Value)
            return false;

        return true;
    }

    /// <summary>
    /// 判断是否超出频次限制
    /// </summary>
    public bool IsFrequencyCapExceeded(int dailyImpressions, int hourlyImpressions)
    {
        return dailyImpressions >= FrequencyCapDaily || hourlyImpressions >= FrequencyCapHourly;
    }

    /// <summary>
    /// 判断是否有足够预算
    /// </summary>
    public bool HasSufficientBudget(decimal spentDailyBudget, decimal spentTotalBudget, decimal bidPrice)
    {
        return (spentDailyBudget + bidPrice) <= DailyBudget && 
               (spentTotalBudget + bidPrice) <= TotalBudget;
    }

    /// <summary>
    /// 获取投放权重（基于优先级和模式）
    /// </summary>
    public decimal GetDeliveryWeight()
    {
        var basePriority = Priority / 10.0m;
        
        return DeliveryMode switch
        {
            DeliveryMode.Guaranteed => basePriority * 1.5m, // 保量投放权重更高
            DeliveryMode.Priority => basePriority * 1.3m,   // 优先级投放权重较高
            DeliveryMode.Bidding => basePriority * 1.0m,    // 竞价投放标准权重
            DeliveryMode.Mixed => basePriority * 1.2m,      // 混合投放权重适中
            _ => basePriority
        };
    }

    /// <summary>
    /// 自动出价计算
    /// </summary>
    private decimal CalculateAutoBid(decimal baseBid, decimal marketPrice)
    {
        if (marketPrice > 0)
        {
            // 基于市场价格的动态调整
            var adjustedBid = Math.Min(baseBid, marketPrice * 1.1m);
            return Math.Min(adjustedBid, MaxBidPrice);
        }
        
        return Math.Min(baseBid, MaxBidPrice);
    }

    /// <summary>
    /// 目标CPA出价计算
    /// </summary>
    private decimal CalculateTargetCPABid(decimal baseBid)
    {
        // 简化的目标CPA计算，实际实现需要考虑历史转化数据
        return Math.Min(baseBid * 0.8m, MaxBidPrice);
    }

    /// <summary>
    /// 目标ROAS出价计算
    /// </summary>
    private decimal CalculateTargetROASBid(decimal baseBid)
    {
        // 简化的目标ROAS计算，实际实现需要考虑收入目标
        return Math.Min(baseBid * 0.9m, MaxBidPrice);
    }

    /// <summary>
    /// 最大化点击出价计算
    /// </summary>
    private decimal CalculateMaximizeClicksBid(decimal baseBid)
    {
        // 在预算约束下最大化点击量
        return Math.Min(baseBid * 1.1m, MaxBidPrice);
    }

    /// <summary>
    /// 最大化转化出价计算
    /// </summary>
    private decimal CalculateMaximizeConversionsBid(decimal baseBid)
    {
        // 在预算约束下最大化转化量
        return Math.Min(baseBid * 1.2m, MaxBidPrice);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(decimal baseBidPrice, decimal maxBidPrice, 
        int frequencyCapDaily, int frequencyCapHourly, decimal dailyBudget, 
        decimal totalBudget, int priority)
    {
        if (baseBidPrice < ValidationConstants.NumberRange.MinBidPrice * 100)
            throw new ArgumentOutOfRangeException(nameof(baseBidPrice), "基础出价不能低于最小值");

        if (maxBidPrice < baseBidPrice)
            throw new ArgumentException("最大出价不能低于基础出价");

        if (maxBidPrice > ValidationConstants.NumberRange.MaxBidPrice * 100)
            throw new ArgumentOutOfRangeException(nameof(maxBidPrice), "最大出价不能超过最大值");

        if (frequencyCapDaily <= 0 || frequencyCapHourly <= 0)
            throw new ArgumentOutOfRangeException("频次上限必须大于0");

        if (frequencyCapHourly > frequencyCapDaily)
            throw new ArgumentException("每小时频次上限不能超过每日频次上限");

        if (dailyBudget <= 0 || totalBudget <= 0)
            throw new ArgumentOutOfRangeException("预算必须大于0");

        if (dailyBudget > totalBudget)
            throw new ArgumentException("日预算不能超过总预算");

        if (priority < 1 || priority > 10)
            throw new ArgumentOutOfRangeException(nameof(priority), "优先级必须在1-10之间");
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeliveryMode;
        yield return BiddingStrategy;
        yield return BaseBidPrice;
        yield return MaxBidPrice;
        yield return FrequencyCapDaily;
        yield return FrequencyCapHourly;
        yield return DailyBudget;
        yield return TotalBudget;
        yield return StartTime ?? DateTime.MinValue;
        yield return EndTime ?? DateTime.MaxValue;
        yield return TimeZone ?? string.Empty;
        yield return BidAdjustmentFactor;
        yield return EnableSmartBidding;
        yield return Priority;
    }
}