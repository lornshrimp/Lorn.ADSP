namespace Lorn.ADSP.Core.Shared.Entities
{
    /// <summary>
    /// 数据记录
    /// </summary>
    public record DataRecord
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// 指标值
        /// </summary>
        public IReadOnlyDictionary<string, object> Metrics { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// 维度值
        /// </summary>
        public IReadOnlyDictionary<string, string> Dimensions { get; init; } = new Dictionary<string, string>();
    }
}
