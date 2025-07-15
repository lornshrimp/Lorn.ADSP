namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 数据访问统计信息
    /// </summary>
    public record DataAccessStatistics
    {
        /// <summary>
        /// 总请求数
        /// </summary>
        public long TotalRequests { get; init; }

        /// <summary>
        /// 成功请求数
        /// </summary>
        public long SuccessfulRequests { get; init; }

        /// <summary>
        /// 失败请求数
        /// </summary>
        public long FailedRequests { get; init; }

        /// <summary>
        /// 平均响应时间
        /// </summary>
        public TimeSpan AverageResponseTime { get; init; }

        /// <summary>
        /// 缓存命中率
        /// </summary>
        public double CacheHitRate { get; init; }

        /// <summary>
        /// 提供者统计信息
        /// </summary>
        public IReadOnlyList<DataProviderStatistics> ProviderStatistics { get; init; } = Array.Empty<DataProviderStatistics>();

        /// <summary>
        /// 统计时间范围
        /// </summary>
        public TimeSpan StatisticsPeriod { get; init; }

        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime StatisticsEndTime { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests : 0;
    }
}
