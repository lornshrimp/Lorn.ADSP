
namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 性能指标值对象
    /// </summary>
    public record PerformanceMetrics
    {
        /// <summary>
        /// 展示次数
        /// </summary>
        public long Impressions { get; init; }

        /// <summary>
        /// 点击次数
        /// </summary>
        public long Clicks { get; init; }

        /// <summary>
        /// 转化次数
        /// </summary>
        public long Conversions { get; init; }

        /// <summary>
        /// 总成本（分）
        /// </summary>
        public decimal Cost { get; init; }

        /// <summary>
        /// 点击率
        /// </summary>
        public decimal CTR => Impressions > 0 ? (decimal)Clicks / Impressions : 0m;

        /// <summary>
        /// 转化率
        /// </summary>
        public decimal CVR => Clicks > 0 ? (decimal)Conversions / Clicks : 0m;

        /// <summary>
        /// 每次点击成本
        /// </summary>
        public decimal CPC => Clicks > 0 ? Cost / Clicks : 0m;

        /// <summary>
        /// 千次展示成本
        /// </summary>
        public decimal CPM => Impressions > 0 ? Cost / Impressions * 1000 : 0m;

        /// <summary>
        /// 创建初始指标
        /// </summary>
        public static PerformanceMetrics Create()
        {
            return new PerformanceMetrics
            {
                Impressions = 0,
                Clicks = 0,
                Conversions = 0,
                Cost = 0
            };
        }

        /// <summary>
        /// 记录展示
        /// </summary>
        public PerformanceMetrics RecordImpression()
        {
            return this with { Impressions = Impressions + 1 };
        }

        /// <summary>
        /// 记录点击
        /// </summary>
        public PerformanceMetrics RecordClick()
        {
            return this with { Clicks = Clicks + 1 };
        }

        /// <summary>
        /// 记录转化
        /// </summary>
        public PerformanceMetrics RecordConversion()
        {
            return this with { Conversions = Conversions + 1 };
        }

        internal decimal GetOverallScore()
        {
            throw new NotImplementedException();
        }
    }
}
