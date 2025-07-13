using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 额外费用值对象
    /// </summary>
    public class AdditionalCost : ValueObject
    {
        /// <summary>
        /// 费用金额（分）
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// 费用描述
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// 费用时间
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdditionalCost(decimal amount, string description)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "费用金额不能为负数");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("费用描述不能为空", nameof(description));

            Amount = amount;
            Description = description;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Description;
            yield return Timestamp;
        }
    }
}
