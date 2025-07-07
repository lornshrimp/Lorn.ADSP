namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 竞价上下文
    /// </summary>
    public record BiddingContext
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public required string RequestId { get; init; }

        /// <summary>
        /// 竞价轮次
        /// </summary>
        public int BiddingRound { get; init; }

        /// <summary>
        /// 参与竞价的广告数量
        /// </summary>
        public int CompetingAdsCount { get; init; }

        /// <summary>
        /// 底价（分）
        /// </summary>
        public decimal FloorPrice { get; init; }

        /// <summary>
        /// 最高出价（分）
        /// </summary>
        public decimal? HighestBid { get; init; }

        /// <summary>
        /// 平均出价（分）
        /// </summary>
        public decimal? AverageBid { get; init; }

        /// <summary>
        /// 竞争激烈程度（0-1）
        /// </summary>
        public decimal CompetitionIntensity { get; init; }

        /// <summary>
        /// 市场价格（分）
        /// </summary>
        public decimal? MarketPrice { get; init; }

        /// <summary>
        /// 竞价截止时间
        /// </summary>
        public DateTime BiddingDeadline { get; init; }

        /// <summary>
        /// 竞价环境数据
        /// </summary>
        public IReadOnlyDictionary<string, object> EnvironmentData { get; init; } =
            new Dictionary<string, object>();
    }
}
