using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Requests
{
    /// <summary>
    /// 特征请求
    /// </summary>
    public record FeatureRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; init; }

        /// <summary>
        /// 上下文ID
        /// </summary>
        public Guid? ContextId { get; init; }

        /// <summary>
        /// 特征类型
        /// </summary>
        public IReadOnlyList<string> FeatureTypes { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 时间窗口
        /// </summary>
        public TimeWindow? TimeWindow { get; init; }

        /// <summary>
        /// 聚合方式
        /// </summary>
        public string? AggregationMethod { get; init; }
    }
}
