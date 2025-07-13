namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 流量概况值对象
    /// </summary>
    public record TrafficProfile(
        int DailyInventory,
        int PeakHourInventory,
        IReadOnlyDictionary<string, decimal> AudienceSegments,
        IReadOnlyDictionary<string, string> GeographicDistribution,
        decimal AverageViewTime,
        decimal BounceRate);
}
