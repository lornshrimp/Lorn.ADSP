using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放资格
    /// </summary>
    public class DeliveryEligibility : ValueObject
    {
        /// <summary>
        /// 是否符合投放条件
        /// </summary>
        public bool IsEligible { get; private set; }

        /// <summary>
        /// 不符合原因
        /// </summary>
        public IReadOnlyList<string> RejectReasons { get; private set; }

        /// <summary>
        /// 资格评分（0-1）
        /// </summary>
        public decimal EligibilityScore { get; private set; }

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime CheckedAt { get; private set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiresAt { get; private set; }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

        /// <summary>
        /// 是否当前有效
        /// </summary>
        public bool IsCurrentlyValid => IsEligible && !IsExpired;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private DeliveryEligibility()
        {
            IsEligible = false;
            RejectReasons = Array.Empty<string>();
            EligibilityScore = 0;
            CheckedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeliveryEligibility(
            bool isEligible,
            decimal eligibilityScore,
            IReadOnlyList<string>? rejectReasons = null,
            DateTime? checkedAt = null,
            DateTime? expiresAt = null)
        {
            ValidateInput(eligibilityScore, rejectReasons, isEligible);

            IsEligible = isEligible;
            EligibilityScore = eligibilityScore;
            RejectReasons = rejectReasons ?? Array.Empty<string>();
            CheckedAt = checkedAt ?? DateTime.UtcNow;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// 创建符合条件的投放资格
        /// </summary>
        public static DeliveryEligibility CreateEligible(decimal eligibilityScore = 1.0m, DateTime? expiresAt = null)
        {
            return new DeliveryEligibility(true, eligibilityScore, null, null, expiresAt);
        }

        /// <summary>
        /// 创建不符合条件的投放资格
        /// </summary>
        public static DeliveryEligibility CreateIneligible(
            IReadOnlyList<string> rejectReasons,
            decimal eligibilityScore = 0m)
        {
            return new DeliveryEligibility(false, eligibilityScore, rejectReasons);
        }

        /// <summary>
        /// 创建不符合条件的投放资格（单个原因）
        /// </summary>
        public static DeliveryEligibility CreateIneligible(
            string rejectReason,
            decimal eligibilityScore = 0m)
        {
            return new DeliveryEligibility(false, eligibilityScore, new[] { rejectReason });
        }

        /// <summary>
        /// 添加拒绝原因
        /// </summary>
        public DeliveryEligibility AddRejectReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return this;

            var newReasons = RejectReasons.Concat(new[] { reason }).ToArray();
            return new DeliveryEligibility(
                false, // 添加拒绝原因意味着不符合条件
                EligibilityScore,
                newReasons,
                CheckedAt,
                ExpiresAt);
        }

        /// <summary>
        /// 更新资格评分
        /// </summary>
        public DeliveryEligibility UpdateScore(decimal newScore)
        {
            return new DeliveryEligibility(
                IsEligible,
                newScore,
                RejectReasons,
                DateTime.UtcNow,
                ExpiresAt);
        }

        /// <summary>
        /// 设置有效期
        /// </summary>
        public DeliveryEligibility WithExpiration(DateTime expiresAt)
        {
            if (expiresAt <= DateTime.UtcNow)
                throw new ArgumentException("有效期必须在当前时间之后", nameof(expiresAt));

            return new DeliveryEligibility(
                IsEligible,
                EligibilityScore,
                RejectReasons,
                CheckedAt,
                expiresAt);
        }

        /// <summary>
        /// 刷新检查时间
        /// </summary>
        public DeliveryEligibility RefreshCheckTime()
        {
            return new DeliveryEligibility(
                IsEligible,
                EligibilityScore,
                RejectReasons,
                DateTime.UtcNow,
                ExpiresAt);
        }

        /// <summary>
        /// 合并多个投放资格检查结果
        /// </summary>
        public static DeliveryEligibility Merge(params DeliveryEligibility[] eligibilities)
        {
            if (eligibilities == null || eligibilities.Length == 0)
                return CreateIneligible("没有可合并的资格检查结果");

            var allEligible = eligibilities.All(e => e.IsEligible);
            var allReasons = eligibilities.SelectMany(e => e.RejectReasons).Distinct().ToArray();
            var minScore = eligibilities.Min(e => e.EligibilityScore);
            var earliestExpiry = eligibilities
                .Where(e => e.ExpiresAt.HasValue)
                .Select(e => e.ExpiresAt!.Value)
                .DefaultIfEmpty()
                .Min();

            return new DeliveryEligibility(
                allEligible,
                minScore,
                allReasons,
                DateTime.UtcNow,
                earliestExpiry == DateTime.MinValue ? null : earliestExpiry);
        }

        /// <summary>
        /// 获取剩余有效时间
        /// </summary>
        public TimeSpan? GetRemainingValidTime()
        {
            if (!ExpiresAt.HasValue)
                return null;

            var remaining = ExpiresAt.Value - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return IsEligible;
            yield return EligibilityScore;
            yield return CheckedAt;
            yield return ExpiresAt ?? DateTime.MinValue;

            foreach (var reason in RejectReasons.OrderBy(r => r))
            {
                yield return reason;
            }
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(
            decimal eligibilityScore,
            IReadOnlyList<string>? rejectReasons,
            bool isEligible)
        {
            if (eligibilityScore < 0 || eligibilityScore > 1)
                throw new ArgumentException("资格评分必须在0-1之间", nameof(eligibilityScore));

            if (!isEligible && (rejectReasons == null || rejectReasons.Count == 0))
                throw new ArgumentException("不符合条件时必须提供拒绝原因", nameof(rejectReasons));

            if (isEligible && rejectReasons != null && rejectReasons.Count > 0)
                throw new ArgumentException("符合条件时不应该有拒绝原因", nameof(rejectReasons));
        }
    }
}
