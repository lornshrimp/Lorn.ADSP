using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

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