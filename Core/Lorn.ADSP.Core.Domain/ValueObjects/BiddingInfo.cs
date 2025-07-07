using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 竞价信息值对象
/// </summary>
public class BiddingInfo : ValueObject
{
    /// <summary>
    /// 竞价策略
    /// </summary>
    public BiddingStrategy Strategy { get; private set; }

    /// <summary>
    /// 基础出价（分）
    /// </summary>
    public decimal BaseBidPrice { get; private set; }

    /// <summary>
    /// 最大出价（分）
    /// </summary>
    public decimal MaxBidPrice { get; private set; }

    /// <summary>
    /// 当前出价（分）
    /// </summary>
    public decimal CurrentBidPrice { get; private set; }

    /// <summary>
    /// 出价调整因子
    /// </summary>
    public decimal BidAdjustmentFactor { get; private set; }

    /// <summary>
    /// 预算信息
    /// </summary>
    public BudgetInfo? Budget { get; private set; }

    /// <summary>
    /// 竞价标签
    /// </summary>
    public IReadOnlyList<string> BidTags { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BiddingInfo()
    {
        Strategy = BiddingStrategy.FixedBid;
        BaseBidPrice = 0;
        MaxBidPrice = 0;
        CurrentBidPrice = 0;
        BidAdjustmentFactor = 1.0m;
        BidTags = Array.Empty<string>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BiddingInfo(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal currentBidPrice = 0,
        decimal bidAdjustmentFactor = 1.0m,
        BudgetInfo? budget = null,
        IReadOnlyList<string>? bidTags = null)
    {
        ValidateInput(strategy, baseBidPrice, maxBidPrice, bidAdjustmentFactor);

        Strategy = strategy;
        BaseBidPrice = baseBidPrice;
        MaxBidPrice = maxBidPrice;
        CurrentBidPrice = currentBidPrice;
        BidAdjustmentFactor = bidAdjustmentFactor;
        Budget = budget;
        BidTags = bidTags ?? Array.Empty<string>();
    }

    /// <summary>
    /// 创建竞价信息
    /// </summary>
    public static BiddingInfo Create(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        BudgetInfo? budget = null)
    {
        return new BiddingInfo(strategy, baseBidPrice, maxBidPrice, budget: budget);
    }

    /// <summary>
    /// 更新当前出价
    /// </summary>
    public BiddingInfo UpdateCurrentBidPrice(decimal currentBidPrice)
    {
        if (currentBidPrice < 0)
            throw new ArgumentException("当前出价不能为负数", nameof(currentBidPrice));

        if (currentBidPrice > MaxBidPrice)
            throw new ArgumentException("当前出价不能超过最大出价", nameof(currentBidPrice));

        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            currentBidPrice,
            BidAdjustmentFactor,
            Budget,
            BidTags);
    }

    /// <summary>
    /// 调整出价因子
    /// </summary>
    public BiddingInfo WithBidAdjustmentFactor(decimal adjustmentFactor)
    {
        if (adjustmentFactor <= 0)
            throw new ArgumentException("出价调整因子必须大于0", nameof(adjustmentFactor));

        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            adjustmentFactor,
            Budget,
            BidTags);
    }

    /// <summary>
    /// 更新预算信息
    /// </summary>
    public BiddingInfo WithBudget(BudgetInfo budget)
    {
        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            BidAdjustmentFactor,
            budget,
            BidTags);
    }

    /// <summary>
    /// 添加竞价标签
    /// </summary>
    public BiddingInfo WithBidTags(params string[] tags)
    {
        var newTags = BidTags.Concat(tags).Distinct().ToArray();
        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            BidAdjustmentFactor,
            Budget,
            newTags);
    }

    /// <summary>
    /// 是否有足够预算
    /// </summary>
    public bool HasSufficientBudget(decimal requiredAmount)
    {
        return Budget?.RemainingBudget >= requiredAmount;
    }

    /// <summary>
    /// 计算调整后的出价
    /// </summary>
    public decimal CalculateAdjustedBidPrice()
    {
        var adjustedPrice = BaseBidPrice * BidAdjustmentFactor;
        return Math.Min(adjustedPrice, MaxBidPrice);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Strategy;
        yield return BaseBidPrice;
        yield return MaxBidPrice;
        yield return CurrentBidPrice;
        yield return BidAdjustmentFactor;
        yield return Budget ?? new object();

        foreach (var tag in BidTags.OrderBy(x => x))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal bidAdjustmentFactor)
    {
        if (baseBidPrice < 0)
            throw new ArgumentException("基础出价不能为负数", nameof(baseBidPrice));

        if (maxBidPrice < 0)
            throw new ArgumentException("最大出价不能为负数", nameof(maxBidPrice));

        if (maxBidPrice < baseBidPrice)
            throw new ArgumentException("最大出价不能小于基础出价", nameof(maxBidPrice));

        if (bidAdjustmentFactor <= 0)
            throw new ArgumentException("出价调整因子必须大于0", nameof(bidAdjustmentFactor));
    }
}

/// <summary>
/// 预算信息值对象
/// </summary>
public class BudgetInfo : ValueObject
{
    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; private set; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// 预算消耗速度（分/小时）
    /// </summary>
    public decimal? BurnRate { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BudgetInfo()
    {
        DailyBudget = 0;
        TotalBudget = 0;
        SpentBudget = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BudgetInfo(
        decimal dailyBudget,
        decimal totalBudget,
        decimal spentBudget = 0,
        decimal? burnRate = null)
    {
        ValidateInput(dailyBudget, totalBudget, spentBudget);

        DailyBudget = dailyBudget;
        TotalBudget = totalBudget;
        SpentBudget = spentBudget;
        BurnRate = burnRate;
    }

    /// <summary>
    /// 创建预算信息
    /// </summary>
    public static BudgetInfo Create(decimal dailyBudget, decimal totalBudget)
    {
        return new BudgetInfo(dailyBudget, totalBudget);
    }

    /// <summary>
    /// 更新已消费预算
    /// </summary>
    public BudgetInfo UpdateSpentBudget(decimal spentBudget)
    {
        if (spentBudget < 0)
            throw new ArgumentException("已消费预算不能为负数", nameof(spentBudget));

        if (spentBudget > TotalBudget)
            throw new ArgumentException("已消费预算不能超过总预算", nameof(spentBudget));

        return new BudgetInfo(DailyBudget, TotalBudget, spentBudget, BurnRate);
    }

    /// <summary>
    /// 消费预算
    /// </summary>
    public BudgetInfo SpendBudget(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("消费金额不能为负数", nameof(amount));

        var newSpentBudget = SpentBudget + amount;
        if (newSpentBudget > TotalBudget)
            throw new InvalidOperationException("预算不足");

        return new BudgetInfo(DailyBudget, TotalBudget, newSpentBudget, BurnRate);
    }

    /// <summary>
    /// 更新消耗速度
    /// </summary>
    public BudgetInfo WithBurnRate(decimal burnRate)
    {
        if (burnRate < 0)
            throw new ArgumentException("消耗速度不能为负数", nameof(burnRate));

        return new BudgetInfo(DailyBudget, TotalBudget, SpentBudget, burnRate);
    }

    /// <summary>
    /// 是否预算充足
    /// </summary>
    public bool IsSufficient(decimal requiredAmount)
    {
        return RemainingBudget >= requiredAmount;
    }

    /// <summary>
    /// 是否预算耗尽
    /// </summary>
    public bool IsExhausted => RemainingBudget <= 0;

    /// <summary>
    /// 预算使用率
    /// </summary>
    public decimal UsageRate => TotalBudget > 0 ? SpentBudget / TotalBudget : 0;

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DailyBudget;
        yield return TotalBudget;
        yield return SpentBudget;
        yield return BurnRate ?? 0m;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(decimal dailyBudget, decimal totalBudget, decimal spentBudget)
    {
        if (dailyBudget < 0)
            throw new ArgumentException("日预算不能为负数", nameof(dailyBudget));

        if (totalBudget < 0)
            throw new ArgumentException("总预算不能为负数", nameof(totalBudget));

        if (spentBudget < 0)
            throw new ArgumentException("已消费预算不能为负数", nameof(spentBudget));

        if (spentBudget > totalBudget)
            throw new ArgumentException("已消费预算不能超过总预算", nameof(spentBudget));
    }
}