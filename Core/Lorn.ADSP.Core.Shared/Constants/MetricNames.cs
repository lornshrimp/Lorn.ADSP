namespace Lorn.ADSP.Core.Shared.Constants
{
    /// <summary>
    /// 指标名称常量
    /// </summary>
    public static class MetricNames
    {
        // 性能指标
        public const string ExecutionTime = "execution_time";
        public const string ResponseTime = "response_time";
        public const string Throughput = "throughput";
        public const string ErrorRate = "error_rate";
        public const string SuccessRate = "success_rate";

        // 资源指标
        public const string CpuUsage = "cpu_usage";
        public const string MemoryUsage = "memory_usage";
        public const string DiskUsage = "disk_usage";
        public const string NetworkUsage = "network_usage";

        // 业务指标
        public const string AdImpressions = "ad_impressions";
        public const string AdClicks = "ad_clicks";
        public const string AdConversions = "ad_conversions";
        public const string FillRate = "fill_rate";
        public const string QualityScore = "quality_score";

        // 缓存指标
        public const string CacheHits = "cache_hits";
        public const string CacheMisses = "cache_misses";
        public const string CacheHitRate = "cache_hit_rate";

        // 队列指标
        public const string QueueLength = "queue_length";
        public const string QueueProcessingTime = "queue_processing_time";
        public const string QueueThroughput = "queue_throughput";
    }
}
