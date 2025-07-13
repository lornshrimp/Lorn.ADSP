using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 结算费用明细值对象
    /// </summary>
    public class BillingDetails : ValueObject
    {
        /// <summary>
        /// 基础费用（分）
        /// </summary>
        public decimal BaseCost { get; private set; }

        /// <summary>
        /// 额外费用列表
        /// </summary>
        public IReadOnlyList<AdditionalCost> AdditionalCosts { get; private set; } = new List<AdditionalCost>();

        /// <summary>
        /// 费用说明
        /// </summary>
        public IReadOnlyList<string> Notes { get; private set; } = new List<string>();

        /// <summary>
        /// 计费模式
        /// </summary>
        public string BillingMode { get; private set; } = string.Empty;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private BillingDetails() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BillingDetails(
            decimal baseCost,
            string billingMode,
            IList<AdditionalCost>? additionalCosts = null,
            IList<string>? notes = null)
        {
            if (baseCost < 0)
                throw new ArgumentOutOfRangeException(nameof(baseCost), "基础费用不能为负数");

            if (string.IsNullOrWhiteSpace(billingMode))
                throw new ArgumentException("计费模式不能为空", nameof(billingMode));

            BaseCost = baseCost;
            BillingMode = billingMode;
            AdditionalCosts = additionalCosts?.ToList() ?? new List<AdditionalCost>();
            Notes = notes?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// 创建CPM计费
        /// </summary>
        public static BillingDetails CreateCPM(decimal cost)
        {
            return new BillingDetails(cost, "CPM");
        }

        /// <summary>
        /// 创建CPC计费
        /// </summary>
        public static BillingDetails CreateCPC(decimal cost)
        {
            return new BillingDetails(cost, "CPC");
        }

        /// <summary>
        /// 创建CPA计费
        /// </summary>
        public static BillingDetails CreateCPA(decimal cost)
        {
            return new BillingDetails(cost, "CPA");
        }

        /// <summary>
        /// 添加额外费用
        /// </summary>
        public BillingDetails AddCost(decimal amount, string description)
        {
            var newAdditionalCosts = AdditionalCosts.ToList();
            newAdditionalCosts.Add(new AdditionalCost(amount, description));

            return new BillingDetails(BaseCost, BillingMode, newAdditionalCosts, Notes.ToList());
        }

        /// <summary>
        /// 添加说明
        /// </summary>
        public BillingDetails AddNote(string note)
        {
            var newNotes = Notes.ToList();
            newNotes.Add(note);

            return new BillingDetails(BaseCost, BillingMode, AdditionalCosts.ToList(), newNotes);
        }

        /// <summary>
        /// 获取总费用
        /// </summary>
        public decimal GetTotalCost()
        {
            return BaseCost + AdditionalCosts.Sum(c => c.Amount);
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BaseCost;
            yield return BillingMode;
            foreach (var cost in AdditionalCosts)
                yield return cost;
            foreach (var note in Notes)
                yield return note;
        }
    }
}
