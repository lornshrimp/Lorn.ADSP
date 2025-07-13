namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 账单信息值对象
    /// </summary>
    public record BillingInfo(
        string BillingAddress,
        string PaymentMethod,
        string BankAccount,
        string TaxInvoiceInfo,
        decimal CreditLimit);
}
