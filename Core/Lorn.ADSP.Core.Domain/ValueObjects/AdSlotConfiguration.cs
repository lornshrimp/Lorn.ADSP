using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 广告位配置值对象
    /// </summary>
    public class AdSlotConfiguration : ValueObject
    {
        /// <summary>
        /// 支持的广告尺寸列表
        /// </summary>
        public IReadOnlyList<AdSize> SupportedSizes { get; }

        /// <summary>
        /// 支持的广告格式列表
        /// </summary>
        public IReadOnlyList<string> SupportedFormats { get; }

        /// <summary>
        /// 底价
        /// </summary>
        public decimal FloorPrice { get; }

        /// <summary>
        /// 收益分成比率
        /// </summary>
        public decimal RevenueShareRate { get; }

        /// <summary>
        /// 每页最大广告数量
        /// </summary>
        public int MaxAdsPerPage { get; }

        /// <summary>
        /// 是否允许视频广告
        /// </summary>
        public bool AllowVideo { get; }

        /// <summary>
        /// 是否允许音频广告
        /// </summary>
        public bool AllowAudio { get; }

        /// <summary>
        /// 私有构造函数（用于序列化）
        /// </summary>
        private AdSlotConfiguration()
        {
            SupportedSizes = new List<AdSize>();
            SupportedFormats = new List<string>();
            FloorPrice = 0;
            RevenueShareRate = 0;
            MaxAdsPerPage = 1;
            AllowVideo = false;
            AllowAudio = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="supportedSizes">支持的广告尺寸列表</param>
        /// <param name="supportedFormats">支持的广告格式列表</param>
        /// <param name="floorPrice">底价</param>
        /// <param name="revenueShareRate">收益分成比率</param>
        /// <param name="maxAdsPerPage">每页最大广告数量</param>
        /// <param name="allowVideo">是否允许视频广告</param>
        /// <param name="allowAudio">是否允许音频广告</param>
        public AdSlotConfiguration(
            IReadOnlyList<AdSize> supportedSizes,
            IReadOnlyList<string> supportedFormats,
            decimal floorPrice,
            decimal revenueShareRate,
            int maxAdsPerPage,
            bool allowVideo,
            bool allowAudio)
        {
            // 验证输入参数
            if (supportedSizes == null || !supportedSizes.Any())
                throw new ArgumentException("广告位必须至少支持一种广告尺寸", nameof(supportedSizes));

            if (supportedFormats == null || !supportedFormats.Any())
                throw new ArgumentException("广告位必须至少支持一种广告格式", nameof(supportedFormats));

            if (floorPrice < 0)
                throw new ArgumentException("底价不能为负数", nameof(floorPrice));

            if (revenueShareRate < 0 || revenueShareRate > 1)
                throw new ArgumentException("收益分成比率必须在0到1之间", nameof(revenueShareRate));

            if (maxAdsPerPage <= 0)
                throw new ArgumentException("每页最大广告数量必须大于0", nameof(maxAdsPerPage));

            SupportedSizes = supportedSizes.ToList().AsReadOnly();
            SupportedFormats = supportedFormats.ToList().AsReadOnly();
            FloorPrice = floorPrice;
            RevenueShareRate = revenueShareRate;
            MaxAdsPerPage = maxAdsPerPage;
            AllowVideo = allowVideo;
            AllowAudio = allowAudio;
        }

        /// <summary>
        /// 创建广告位配置
        /// </summary>
        /// <param name="supportedSizes">支持的广告尺寸列表</param>
        /// <param name="supportedFormats">支持的广告格式列表</param>
        /// <param name="floorPrice">底价</param>
        /// <param name="revenueShareRate">收益分成比率</param>
        /// <param name="maxAdsPerPage">每页最大广告数量</param>
        /// <param name="allowVideo">是否允许视频广告</param>
        /// <param name="allowAudio">是否允许音频广告</param>
        /// <returns>广告位配置实例</returns>
        public static AdSlotConfiguration Create(
            IReadOnlyList<AdSize> supportedSizes,
            IReadOnlyList<string> supportedFormats,
            decimal floorPrice = 0.01m,
            decimal revenueShareRate = 0.7m,
            int maxAdsPerPage = 3,
            bool allowVideo = true,
            bool allowAudio = false)
        {
            return new AdSlotConfiguration(
                supportedSizes,
                supportedFormats,
                floorPrice,
                revenueShareRate,
                maxAdsPerPage,
                allowVideo,
                allowAudio);
        }

        /// <summary>
        /// 检查是否支持指定的广告尺寸
        /// </summary>
        /// <param name="adSize">广告尺寸</param>
        /// <returns>是否支持</returns>
        public bool IsSizeSupported(AdSize adSize)
        {
            return SupportedSizes.Contains(adSize);
        }

        /// <summary>
        /// 检查是否支持指定的广告格式
        /// </summary>
        /// <param name="format">广告格式</param>
        /// <returns>是否支持</returns>
        public bool IsFormatSupported(string format)
        {
            return SupportedFormats.Contains(format, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否支持视频内容
        /// </summary>
        /// <returns>是否支持视频</returns>
        public bool SupportsVideo()
        {
            return AllowVideo;
        }

        /// <summary>
        /// 检查是否支持音频内容
        /// </summary>
        /// <returns>是否支持音频</returns>
        public bool SupportsAudio()
        {
            return AllowAudio;
        }

        /// <summary>
        /// 创建具有新底价的配置副本
        /// </summary>
        /// <param name="newFloorPrice">新的底价</param>
        /// <returns>新的配置实例</returns>
        public AdSlotConfiguration WithFloorPrice(decimal newFloorPrice)
        {
            return new AdSlotConfiguration(
                SupportedSizes,
                SupportedFormats,
                newFloorPrice,
                RevenueShareRate,
                MaxAdsPerPage,
                AllowVideo,
                AllowAudio);
        }

        /// <summary>
        /// 创建具有新收益分成比率的配置副本
        /// </summary>
        /// <param name="newRevenueShareRate">新的收益分成比率</param>
        /// <returns>新的配置实例</returns>
        public AdSlotConfiguration WithRevenueShareRate(decimal newRevenueShareRate)
        {
            return new AdSlotConfiguration(
                SupportedSizes,
                SupportedFormats,
                FloorPrice,
                newRevenueShareRate,
                MaxAdsPerPage,
                AllowVideo,
                AllowAudio);
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        /// <returns>用于相等性比较的组件</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var size in SupportedSizes)
                yield return size;

            foreach (var format in SupportedFormats.OrderBy(f => f))
                yield return format;

            yield return FloorPrice;
            yield return RevenueShareRate;
            yield return MaxAdsPerPage;
            yield return AllowVideo;
            yield return AllowAudio;
        }

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>配置描述</returns>
        public override string ToString()
        {
            var sizeCount = SupportedSizes.Count;
            var formatCount = SupportedFormats.Count;
            var videoSupport = AllowVideo ? "支持视频" : "不支持视频";
            var audioSupport = AllowAudio ? "支持音频" : "不支持音频";

            return $"广告位配置: {sizeCount}种尺寸, {formatCount}种格式, 底价:{FloorPrice:C}, 分成:{RevenueShareRate:P}, 最大数量:{MaxAdsPerPage}, {videoSupport}, {audioSupport}";
        }
    }
}
