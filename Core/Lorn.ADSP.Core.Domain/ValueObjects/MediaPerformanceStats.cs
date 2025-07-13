namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 媒体性能统计
    /// </summary>
    public record MediaPerformanceStats
    {
        public long TotalImpressions { get; init; }
        public long TotalClicks { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal ClickThroughRate { get; init; }
        public decimal RevenuePerThousandImpressions { get; init; }
        public decimal FillRate { get; init; }
    }
}
