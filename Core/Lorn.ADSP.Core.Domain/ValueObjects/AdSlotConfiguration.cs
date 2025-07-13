namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 广告位配置值对象
    /// </summary>
    public record AdSlotConfiguration(
        IReadOnlyList<AdSize> SupportedSizes,
        IReadOnlyList<string> SupportedFormats,
        decimal FloorPrice,
        decimal RevenueShareRate,
        int MaxAdsPerPage,
        bool AllowVideo,
        bool AllowAudio);
}
