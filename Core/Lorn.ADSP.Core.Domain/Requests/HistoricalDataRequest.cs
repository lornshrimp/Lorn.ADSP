using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.Requests
{
    /// <summary>
    /// 历史数据请求
    /// </summary>
    public record HistoricalDataRequest
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public required string DataType { get; init; }

        /// <summary>
        /// 实体ID
        /// </summary>
        public string? EntityId { get; init; }

        /// <summary>
        /// 时间范围
        /// </summary>
        public required TimeRange TimeRange { get; init; }

        /// <summary>
        /// 聚合粒度
        /// </summary>
        public string? Granularity { get; init; }

        /// <summary>
        /// 指标列表
        /// </summary>
        public IReadOnlyList<string> Metrics { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 维度列表
        /// </summary>
        public IReadOnlyList<string> Dimensions { get; init; } = Array.Empty<string>();
    }
}
