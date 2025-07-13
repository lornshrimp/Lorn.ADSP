namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放报表
    /// </summary>
    public record DeliveryReport
    {
        public string DeliveryRecordId { get; init; } = string.Empty;
        public string RequestId { get; init; } = string.Empty;
        public string CampaignId { get; init; } = string.Empty;
        public string MediaResourceId { get; init; } = string.Empty;
        public string PlacementId { get; init; } = string.Empty;
        public DateTime DeliveredAt { get; init; }
        public decimal Cost { get; init; }
        public PerformanceMetrics Metrics { get; init; } = null!;
        public string UserSegment { get; init; } = string.Empty;
        public string DeviceType { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
    }
}
