using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Shared.Constants;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 预算信息值对象
/// </summary>
public class BudgetInfo : ValueObject
{
    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// 已消费金额（分）
    /// </summary>
    public decimal SpentAmount { get; private set; }

    /// <summary>
    /// 剩余预算（分）
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentAmount;

    /// <summary>
    /// 预算类型
    /// </summary>
    public BudgetType Type { get; private set; }

    /// <summary>
    /// 有效开始时间
    /// </summary>
    public DateTime ValidFrom { get; private set; }

    /// <summary>
    /// 有效结束时间
    /// </summary>
    public DateTime ValidTo { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BudgetInfo() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BudgetInfo(
        decimal totalBudget,
        decimal dailyBudget,
        BudgetType type = BudgetType.Standard,
        DateTime? validFrom = null,
        DateTime? validTo = null)
    {
        ValidateInputs(totalBudget, dailyBudget);

        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        SpentAmount = 0m;
        Type = type;
        ValidFrom = validFrom ?? DateTime.UtcNow;
        ValidTo = validTo ?? DateTime.MaxValue;
    }

    /// <summary>
    /// 创建标准预算
    /// </summary>
    public static BudgetInfo CreateStandard(decimal totalBudget, decimal dailyBudget)
    {
        return new BudgetInfo(totalBudget, dailyBudget, BudgetType.Standard);
    }

    /// <summary>
    /// 创建无限预算
    /// </summary>
    public static BudgetInfo CreateUnlimited(decimal dailyBudget)
    {
        return new BudgetInfo(decimal.MaxValue, dailyBudget, BudgetType.Unlimited);
    }

    /// <summary>
    /// 创建试验预算
    /// </summary>
    public static BudgetInfo CreateTrial(decimal totalBudget, DateTime validTo)
    {
        var dailyBudget = Math.Min(totalBudget, DefaultValues.Budget.DefaultDailyBudget * 100);
        return new BudgetInfo(totalBudget, dailyBudget, BudgetType.Trial, DateTime.UtcNow, validTo);
    }

    /// <summary>
    /// 获取剩余日预算
    /// </summary>
    public decimal GetRemainingDaily()
    {
        // 这里简化处理，实际应该根据当日已消费计算
        return Math.Min(DailyBudget, RemainingBudget);
    }

    /// <summary>
    /// 是否预算耗尽
    /// </summary>
    public bool IsExhausted()
    {
        return RemainingBudget <= 0 || DateTime.UtcNow > ValidTo;
    }

    /// <summary>
    /// 是否可以消费指定金额
    /// </summary>
    public bool CanSpend(decimal amount)
    {
        if (amount <= 0)
            return false;

        if (IsExhausted())
            return false;

        return RemainingBudget >= amount;
    }

    // Replace the 'with' expression with a method to create a new instance
    public BudgetInfo RecordSpending(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("消费金额必须大于0", nameof(amount));

        if (!CanSpend(amount))
            throw new InvalidOperationException("预算不足，无法记录消费");

        return new BudgetInfo(TotalBudget, DailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = SpentAmount + amount
        };
    }

    /// <summary>
    /// 获取消费率
    /// </summary>
    public double GetSpendingRate()
    {
        if (TotalBudget <= 0)
            return 0;

        return (double)(SpentAmount / TotalBudget);
    }

    /// <summary>
    /// 更新预算
    /// </summary>
    public BudgetInfo UpdateBudget(decimal newTotalBudget, decimal newDailyBudget)
    {
        ValidateInputs(newTotalBudget, newDailyBudget);

        if (newTotalBudget < SpentAmount)
            throw new ArgumentException("新的总预算不能小于已消费金额", nameof(newTotalBudget));

        return new BudgetInfo(newTotalBudget, newDailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = SpentAmount
        };
    }

    /// <summary>
    /// 重置消费
    /// </summary>
    public BudgetInfo ResetSpending()
    {
        return new BudgetInfo(TotalBudget, DailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = 0m
        };
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(decimal totalBudget, decimal dailyBudget)
    {
        if (totalBudget <= 0)
            throw new ArgumentException("总预算必须大于0", nameof(totalBudget));

        if (dailyBudget <= 0)
            throw new ArgumentException("日预算必须大于0", nameof(dailyBudget));

        if (dailyBudget > totalBudget && totalBudget != decimal.MaxValue)
            throw new ArgumentException("日预算不能超过总预算", nameof(dailyBudget));
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalBudget;
        yield return DailyBudget;
        yield return SpentAmount;
        yield return Type;
        yield return ValidFrom;
        yield return ValidTo;
    }
}

