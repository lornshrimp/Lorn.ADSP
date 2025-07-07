namespace Lorn.ADSP.Core.Shared.Entities
{
    /// <summary>
    /// 历史数据
    /// </summary>
    public record HistoricalData
    {
        /// <summary>
        /// 数据记录
        /// </summary>
        public required IReadOnlyList<DataRecord> Records { get; init; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// 数据质量评分
        /// </summary>
        public decimal Quality { get; init; } = 1.0m;

        /// <summary>
        /// 数据来源
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// 查询时间
        /// </summary>
        public DateTime QueryTime { get; init; } = DateTime.UtcNow;
    }
}
