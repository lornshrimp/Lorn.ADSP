using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 账单信息值对象
    /// </summary>
    public class BillingInfo : ValueObject
    {
        /// <summary>
        /// 账单地址
        /// </summary>
        public string BillingAddress { get; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentMethod { get; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; }

        /// <summary>
        /// 税务发票信息
        /// </summary>
        public string TaxInvoiceInfo { get; }

        /// <summary>
        /// 信用额度
        /// </summary>
        public decimal CreditLimit { get; }

        /// <summary>
        /// 私有构造函数（用于序列化）
        /// </summary>
        private BillingInfo()
        {
            BillingAddress = string.Empty;
            PaymentMethod = string.Empty;
            BankAccount = string.Empty;
            TaxInvoiceInfo = string.Empty;
            CreditLimit = 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="billingAddress">账单地址</param>
        /// <param name="paymentMethod">支付方式</param>
        /// <param name="bankAccount">银行账户</param>
        /// <param name="taxInvoiceInfo">税务发票信息</param>
        /// <param name="creditLimit">信用额度</param>
        public BillingInfo(
            string billingAddress,
            string paymentMethod,
            string bankAccount,
            string taxInvoiceInfo,
            decimal creditLimit)
        {
            // 验证输入参数
            if (string.IsNullOrWhiteSpace(billingAddress))
                throw new ArgumentException("账单地址不能为空", nameof(billingAddress));

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("支付方式不能为空", nameof(paymentMethod));

            if (string.IsNullOrWhiteSpace(bankAccount))
                throw new ArgumentException("银行账户不能为空", nameof(bankAccount));

            if (string.IsNullOrWhiteSpace(taxInvoiceInfo))
                throw new ArgumentException("税务发票信息不能为空", nameof(taxInvoiceInfo));

            if (creditLimit < 0)
                throw new ArgumentException("信用额度不能为负数", nameof(creditLimit));

            BillingAddress = billingAddress.Trim();
            PaymentMethod = paymentMethod.Trim();
            BankAccount = bankAccount.Trim();
            TaxInvoiceInfo = taxInvoiceInfo.Trim();
            CreditLimit = creditLimit;
        }

        /// <summary>
        /// 创建账单信息
        /// </summary>
        /// <param name="billingAddress">账单地址</param>
        /// <param name="paymentMethod">支付方式</param>
        /// <param name="bankAccount">银行账户</param>
        /// <param name="taxInvoiceInfo">税务发票信息</param>
        /// <param name="creditLimit">信用额度</param>
        /// <returns>账单信息实例</returns>
        public static BillingInfo Create(
            string billingAddress,
            string paymentMethod,
            string bankAccount,
            string taxInvoiceInfo,
            decimal creditLimit = 0)
        {
            return new BillingInfo(
                billingAddress,
                paymentMethod,
                bankAccount,
                taxInvoiceInfo,
                creditLimit);
        }

        /// <summary>
        /// 验证银行账户格式是否正确
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValidBankAccount()
        {
            // 简单的银行账户验证：长度在10-30位之间，只包含数字
            return !string.IsNullOrWhiteSpace(BankAccount) &&
                   BankAccount.Length >= 10 &&
                   BankAccount.Length <= 30 &&
                   BankAccount.All(char.IsDigit);
        }

        /// <summary>
        /// 检查是否有足够的信用额度
        /// </summary>
        /// <param name="amount">需要的金额</param>
        /// <returns>是否有足够额度</returns>
        public bool HasSufficientCredit(decimal amount)
        {
            return CreditLimit >= amount && amount >= 0;
        }

        /// <summary>
        /// 获取剩余信用额度
        /// </summary>
        /// <param name="usedAmount">已使用金额</param>
        /// <returns>剩余额度</returns>
        public decimal GetRemainingCredit(decimal usedAmount)
        {
            if (usedAmount < 0)
                throw new ArgumentException("已使用金额不能为负数", nameof(usedAmount));

            return Math.Max(0, CreditLimit - usedAmount);
        }

        /// <summary>
        /// 创建具有新信用额度的账单信息副本
        /// </summary>
        /// <param name="newCreditLimit">新的信用额度</param>
        /// <returns>新的账单信息实例</returns>
        public BillingInfo WithCreditLimit(decimal newCreditLimit)
        {
            return new BillingInfo(
                BillingAddress,
                PaymentMethod,
                BankAccount,
                TaxInvoiceInfo,
                newCreditLimit);
        }

        /// <summary>
        /// 创建具有新账单地址的账单信息副本
        /// </summary>
        /// <param name="newBillingAddress">新的账单地址</param>
        /// <returns>新的账单信息实例</returns>
        public BillingInfo WithBillingAddress(string newBillingAddress)
        {
            return new BillingInfo(
                newBillingAddress,
                PaymentMethod,
                BankAccount,
                TaxInvoiceInfo,
                CreditLimit);
        }

        /// <summary>
        /// 创建具有新支付方式的账单信息副本
        /// </summary>
        /// <param name="newPaymentMethod">新的支付方式</param>
        /// <returns>新的账单信息实例</returns>
        public BillingInfo WithPaymentMethod(string newPaymentMethod)
        {
            return new BillingInfo(
                BillingAddress,
                newPaymentMethod,
                BankAccount,
                TaxInvoiceInfo,
                CreditLimit);
        }

        /// <summary>
        /// 创建具有新银行账户的账单信息副本
        /// </summary>
        /// <param name="newBankAccount">新的银行账户</param>
        /// <returns>新的账单信息实例</returns>
        public BillingInfo WithBankAccount(string newBankAccount)
        {
            return new BillingInfo(
                BillingAddress,
                PaymentMethod,
                newBankAccount,
                TaxInvoiceInfo,
                CreditLimit);
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        /// <returns>用于相等性比较的组件</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BillingAddress;
            yield return PaymentMethod;
            yield return BankAccount;
            yield return TaxInvoiceInfo;
            yield return CreditLimit;
        }

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>账单信息描述</returns>
        public override string ToString()
        {
            var maskedBankAccount = BankAccount.Length > 4
                ? "****" + BankAccount.Substring(BankAccount.Length - 4)
                : "****";

            return $"账单信息: 地址={BillingAddress}, 支付方式={PaymentMethod}, 银行账户={maskedBankAccount}, 信用额度={CreditLimit:C}";
        }
    }
}
