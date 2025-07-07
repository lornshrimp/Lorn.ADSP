using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
/// </summary>
public class BiddingInfo : ValueObject
{
    /// <summary>
    /// ���۲���
    /// </summary>
    public BiddingStrategy Strategy { get; private set; }

    /// <summary>
    /// �������ۣ��֣�
    /// </summary>
    public decimal BaseBidPrice { get; private set; }

    /// <summary>
    /// �����ۣ��֣�
    /// </summary>
    public decimal MaxBidPrice { get; private set; }

    /// <summary>
    /// ��ǰ���ۣ��֣�
    /// </summary>
    public decimal CurrentBidPrice { get; private set; }

    /// <summary>
    /// ���۵�������
    /// </summary>
    public decimal BidAdjustmentFactor { get; private set; }

    /// <summary>
    /// Ԥ����Ϣ
    /// </summary>
    public BudgetInfo? Budget { get; private set; }

    /// <summary>
    /// ���۱�ǩ
    /// </summary>
    public IReadOnlyList<string> BidTags { get; private set; }

    /// <summary>
    /// ˽�й��캯��
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
    /// ���캯��
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
    /// ����������Ϣ
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
    /// ���µ�ǰ����
    /// </summary>
    public BiddingInfo UpdateCurrentBidPrice(decimal currentBidPrice)
    {
        if (currentBidPrice < 0)
            throw new ArgumentException("��ǰ���۲���Ϊ����", nameof(currentBidPrice));

        if (currentBidPrice > MaxBidPrice)
            throw new ArgumentException("��ǰ���۲��ܳ���������", nameof(currentBidPrice));

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
    /// ������������
    /// </summary>
    public BiddingInfo WithBidAdjustmentFactor(decimal adjustmentFactor)
    {
        if (adjustmentFactor <= 0)
            throw new ArgumentException("���۵������ӱ������0", nameof(adjustmentFactor));

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
    /// ����Ԥ����Ϣ
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
    /// ��Ӿ��۱�ǩ
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
    /// �Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal requiredAmount)
    {
        return Budget?.RemainingBudget >= requiredAmount;
    }

    /// <summary>
    /// ���������ĳ���
    /// </summary>
    public decimal CalculateAdjustedBidPrice()
    {
        var adjustedPrice = BaseBidPrice * BidAdjustmentFactor;
        return Math.Min(adjustedPrice, MaxBidPrice);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
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
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal bidAdjustmentFactor)
    {
        if (baseBidPrice < 0)
            throw new ArgumentException("�������۲���Ϊ����", nameof(baseBidPrice));

        if (maxBidPrice < 0)
            throw new ArgumentException("�����۲���Ϊ����", nameof(maxBidPrice));

        if (maxBidPrice < baseBidPrice)
            throw new ArgumentException("�����۲���С�ڻ�������", nameof(maxBidPrice));

        if (bidAdjustmentFactor <= 0)
            throw new ArgumentException("���۵������ӱ������0", nameof(bidAdjustmentFactor));
    }
}

/// <summary>
/// Ԥ����Ϣֵ����
/// </summary>
public class BudgetInfo : ValueObject
{
    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// ������Ԥ�㣨�֣�
    /// </summary>
    public decimal SpentBudget { get; private set; }

    /// <summary>
    /// ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// Ԥ�������ٶȣ���/Сʱ��
    /// </summary>
    public decimal? BurnRate { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BudgetInfo()
    {
        DailyBudget = 0;
        TotalBudget = 0;
        SpentBudget = 0;
    }

    /// <summary>
    /// ���캯��
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
    /// ����Ԥ����Ϣ
    /// </summary>
    public static BudgetInfo Create(decimal dailyBudget, decimal totalBudget)
    {
        return new BudgetInfo(dailyBudget, totalBudget);
    }

    /// <summary>
    /// ����������Ԥ��
    /// </summary>
    public BudgetInfo UpdateSpentBudget(decimal spentBudget)
    {
        if (spentBudget < 0)
            throw new ArgumentException("������Ԥ�㲻��Ϊ����", nameof(spentBudget));

        if (spentBudget > TotalBudget)
            throw new ArgumentException("������Ԥ�㲻�ܳ�����Ԥ��", nameof(spentBudget));

        return new BudgetInfo(DailyBudget, TotalBudget, spentBudget, BurnRate);
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public BudgetInfo SpendBudget(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("���ѽ���Ϊ����", nameof(amount));

        var newSpentBudget = SpentBudget + amount;
        if (newSpentBudget > TotalBudget)
            throw new InvalidOperationException("Ԥ�㲻��");

        return new BudgetInfo(DailyBudget, TotalBudget, newSpentBudget, BurnRate);
    }

    /// <summary>
    /// ���������ٶ�
    /// </summary>
    public BudgetInfo WithBurnRate(decimal burnRate)
    {
        if (burnRate < 0)
            throw new ArgumentException("�����ٶȲ���Ϊ����", nameof(burnRate));

        return new BudgetInfo(DailyBudget, TotalBudget, SpentBudget, burnRate);
    }

    /// <summary>
    /// �Ƿ�Ԥ�����
    /// </summary>
    public bool IsSufficient(decimal requiredAmount)
    {
        return RemainingBudget >= requiredAmount;
    }

    /// <summary>
    /// �Ƿ�Ԥ��ľ�
    /// </summary>
    public bool IsExhausted => RemainingBudget <= 0;

    /// <summary>
    /// Ԥ��ʹ����
    /// </summary>
    public decimal UsageRate => TotalBudget > 0 ? SpentBudget / TotalBudget : 0;

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DailyBudget;
        yield return TotalBudget;
        yield return SpentBudget;
        yield return BurnRate ?? 0m;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(decimal dailyBudget, decimal totalBudget, decimal spentBudget)
    {
        if (dailyBudget < 0)
            throw new ArgumentException("��Ԥ�㲻��Ϊ����", nameof(dailyBudget));

        if (totalBudget < 0)
            throw new ArgumentException("��Ԥ�㲻��Ϊ����", nameof(totalBudget));

        if (spentBudget < 0)
            throw new ArgumentException("������Ԥ�㲻��Ϊ����", nameof(spentBudget));

        if (spentBudget > totalBudget)
            throw new ArgumentException("������Ԥ�㲻�ܳ�����Ԥ��", nameof(spentBudget));
    }
}