namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放资格
    /// </summary>
    public record DeliveryEligibility
    {
        /// <summary>
        /// 是否符合投放条件
        /// </summary>
        public bool IsEligible { get; init; }

        /// <summary>
        /// 不符合原因
        /// </summary>
        public IReadOnlyList<string> RejectReasons { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 资格评分（0-1）
        /// </summary>
        public decimal EligibilityScore { get; init; }

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime CheckedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiresAt { get; init; }
    }
}
