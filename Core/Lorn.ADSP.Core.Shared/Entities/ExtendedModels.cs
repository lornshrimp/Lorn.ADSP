namespace Lorn.ADSP.Core.Shared.Entities
{

    /// <summary>
    /// 特征向量
    /// </summary>
    public record FeatureVector
    {
        /// <summary>
        /// 特征数据
        /// </summary>
        public required IReadOnlyDictionary<string, object> Features { get; init; }

        /// <summary>
        /// 特征版本
        /// </summary>
        public string? Version { get; init; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiresAt { get; init; }

        /// <summary>
        /// 置信度
        /// </summary>
        public decimal Confidence { get; init; } = 1.0m;
    }
}








