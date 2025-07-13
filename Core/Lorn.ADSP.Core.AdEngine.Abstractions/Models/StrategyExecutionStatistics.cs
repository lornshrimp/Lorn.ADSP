namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models
{
    /// <summary>
    /// 策略执行统计信息
    /// </summary>
    public record StrategyExecutionStatistics
    {
        /// <summary>
        /// 输入候选数量
        /// </summary>
        public int InputCount { get; init; }

        /// <summary>
        /// 输出候选数量
        /// </summary>
        public int OutputCount { get; init; }

        /// <summary>
        /// 处理时间（毫秒）
        /// </summary>
        public double ProcessingTimeMs { get; init; }

        /// <summary>
        /// 内存使用量（字节）
        /// </summary>
        public long MemoryUsage { get; init; }

        /// <summary>
        /// CPU使用时间（毫秒）
        /// </summary>
        public double CpuTimeMs { get; init; }

        /// <summary>
        /// 缓存命中次数
        /// </summary>
        public int CacheHits { get; init; }

        /// <summary>
        /// 缓存未命中次数
        /// </summary>
        public int CacheMisses { get; init; }

        /// <summary>
        /// 数据库查询次数
        /// </summary>
        public int DatabaseQueries { get; init; }

        /// <summary>
        /// 外部调用次数
        /// </summary>
        public int ExternalCalls { get; init; }

        /// <summary>
        /// 处理率（候选/秒）
        /// </summary>
        public double ProcessingRate => ProcessingTimeMs > 0 ? (InputCount * 1000.0) / ProcessingTimeMs : 0;

        /// <summary>
        /// 缓存命中率
        /// </summary>
        public double CacheHitRate => (CacheHits + CacheMisses) > 0 ? (double)CacheHits / (CacheHits + CacheMisses) : 0;

        /// <summary>
        /// 过滤率
        /// </summary>
        public double FilterRate => InputCount > 0 ? (double)(InputCount - OutputCount) / InputCount : 0;

        /// <summary>
        /// 性能指标
        /// </summary>
        public IReadOnlyDictionary<string, object> PerformanceMetrics { get; init; } = new Dictionary<string, object>();
    }
}
