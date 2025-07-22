using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 竞价上下文
    /// </summary>
    public class BiddingContext : ValueObject
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestId { get; private set; }

        /// <summary>
        /// 竞价轮次
        /// </summary>
        public int BiddingRound { get; private set; }

        /// <summary>
        /// 参与竞价的广告数量
        /// </summary>
        public int CompetingAdsCount { get; private set; }

        /// <summary>
        /// 底价（分）
        /// </summary>
        public decimal FloorPrice { get; private set; }

        /// <summary>
        /// 最高出价（分）
        /// </summary>
        public decimal? HighestBid { get; private set; }

        /// <summary>
        /// 平均出价（分）
        /// </summary>
        public decimal? AverageBid { get; private set; }

        /// <summary>
        /// 竞争激烈程度（0-1）
        /// </summary>
        public decimal CompetitionIntensity { get; private set; }

        /// <summary>
        /// 市场价格（分）
        /// </summary>
        public decimal? MarketPrice { get; private set; }

        /// <summary>
        /// 竞价截止时间
        /// </summary>
        public DateTime BiddingDeadline { get; private set; }

        /// <summary>
        /// 竞价环境数据
        /// </summary>
        public IReadOnlyList<ContextProperty> EnvironmentData { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private BiddingContext()
        {
            RequestId = string.Empty;
            EnvironmentData = new List<ContextProperty>().AsReadOnly();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BiddingContext(
            string requestId,
            int biddingRound,
            int competingAdsCount,
            decimal floorPrice,
            decimal competitionIntensity,
            DateTime biddingDeadline,
            decimal? highestBid = null,
            decimal? averageBid = null,
            decimal? marketPrice = null,
            IReadOnlyList<ContextProperty>? environmentData = null)
        {
            ValidateInput(requestId, biddingRound, competingAdsCount, floorPrice, competitionIntensity, biddingDeadline);

            RequestId = requestId;
            BiddingRound = biddingRound;
            CompetingAdsCount = competingAdsCount;
            FloorPrice = floorPrice;
            HighestBid = highestBid;
            AverageBid = averageBid;
            CompetitionIntensity = competitionIntensity;
            MarketPrice = marketPrice;
            BiddingDeadline = biddingDeadline;
            EnvironmentData = environmentData ?? new List<ContextProperty>().AsReadOnly();
        }

        /// <summary>
        /// 创建竞价上下文
        /// </summary>
        public static BiddingContext Create(
            string requestId,
            decimal floorPrice,
            DateTime biddingDeadline,
            int biddingRound = 1,
            int competingAdsCount = 0,
            decimal competitionIntensity = 0m)
        {
            return new BiddingContext(
                requestId,
                biddingRound,
                competingAdsCount,
                floorPrice,
                competitionIntensity,
                biddingDeadline);
        }

        /// <summary>
        /// 更新竞价信息
        /// </summary>
        public BiddingContext UpdateBiddingInfo(
            decimal? highestBid = null,
            decimal? averageBid = null,
            decimal? marketPrice = null,
            int? competingAdsCount = null,
            decimal? competitionIntensity = null)
        {
            return new BiddingContext(
                RequestId,
                BiddingRound,
                competingAdsCount ?? CompetingAdsCount,
                FloorPrice,
                competitionIntensity ?? CompetitionIntensity,
                BiddingDeadline,
                highestBid ?? HighestBid,
                averageBid ?? AverageBid,
                marketPrice ?? MarketPrice,
                EnvironmentData);
        }

        /// <summary>
        /// 添加环境数据
        /// </summary>
        public BiddingContext WithEnvironmentData(string key, object value)
        {
            var newEnvironmentData = EnvironmentData.ToList();

            // 移除已存在的相同key的属性
            newEnvironmentData.RemoveAll(p => p.PropertyKey == key);

            // 添加新的环境属性
            string propertyValue;
            string dataType;

            if (value is string stringValue)
            {
                propertyValue = stringValue;
                dataType = "String";
            }
            else if (value.GetType().IsPrimitive || value is decimal || value is DateTime)
            {
                propertyValue = value.ToString() ?? string.Empty;
                dataType = value.GetType().Name;
            }
            else
            {
                propertyValue = System.Text.Json.JsonSerializer.Serialize(value);
                dataType = "Json";
            }

            newEnvironmentData.Add(new ContextProperty(
                key,
                propertyValue,
                dataType,
                "EnvironmentData",
                false,
                1.0m,
                null,
                "BiddingContext"));

            return new BiddingContext(
                RequestId,
                BiddingRound,
                CompetingAdsCount,
                FloorPrice,
                CompetitionIntensity,
                BiddingDeadline,
                HighestBid,
                AverageBid,
                MarketPrice,
                newEnvironmentData.AsReadOnly());
        }

        /// <summary>
        /// 进入下一轮竞价
        /// </summary>
        public BiddingContext NextRound()
        {
            return new BiddingContext(
                RequestId,
                BiddingRound + 1,
                CompetingAdsCount,
                FloorPrice,
                CompetitionIntensity,
                BiddingDeadline,
                HighestBid,
                AverageBid,
                MarketPrice,
                EnvironmentData);
        }

        /// <summary>
        /// 获取环境数据值
        /// </summary>
        public T? GetEnvironmentData<T>(string key) where T : struct
        {
            var property = EnvironmentData.FirstOrDefault(p => p.PropertyKey == key);
            return property?.GetValue<T>();
        }

        /// <summary>
        /// 获取环境数据值（引用类型）
        /// </summary>
        public T? GetEnvironmentDataRef<T>(string key) where T : class
        {
            var property = EnvironmentData.FirstOrDefault(p => p.PropertyKey == key);
            return property?.GetValue<T>();
        }

        /// <summary>
        /// 获取所有环境数据作为字典（向后兼容）
        /// </summary>
        public Dictionary<string, object> GetEnvironmentDataAsDictionary()
        {
            var result = new Dictionary<string, object>();
            foreach (var property in EnvironmentData)
            {
                try
                {
                    var value = property.GetValue<object>();
                    if (value != null)
                    {
                        result[property.PropertyKey] = value;
                    }
                }
                catch
                {
                    // 如果转换失败，使用原始字符串值
                    result[property.PropertyKey] = property.PropertyValue;
                }
            }
            return result;
        }

        /// <summary>
        /// 检查是否包含特定的环境数据
        /// </summary>
        public bool HasEnvironmentData(string key)
        {
            return EnvironmentData.Any(p => p.PropertyKey == key);
        }

        /// <summary>
        /// 创建竞价上下文（从字典参数）
        /// </summary>
        public static BiddingContext CreateFromDictionary(
            string requestId,
            decimal floorPrice,
            DateTime biddingDeadline,
            int biddingRound = 1,
            int competingAdsCount = 0,
            decimal competitionIntensity = 0m,
            IDictionary<string, object>? environmentData = null)
        {
            var environmentProperties = environmentData?.Select(kvp =>
            {
                string propertyValue;
                string dataType;

                if (kvp.Value is string stringValue)
                {
                    propertyValue = stringValue;
                    dataType = "String";
                }
                else if (kvp.Value.GetType().IsPrimitive || kvp.Value is decimal || kvp.Value is DateTime)
                {
                    propertyValue = kvp.Value.ToString() ?? string.Empty;
                    dataType = kvp.Value.GetType().Name;
                }
                else
                {
                    propertyValue = System.Text.Json.JsonSerializer.Serialize(kvp.Value);
                    dataType = "Json";
                }

                return new ContextProperty(
                    kvp.Key,
                    propertyValue,
                    dataType,
                    "EnvironmentData",
                    false,
                    1.0m,
                    null,
                    "BiddingContext");
            }) ?? Enumerable.Empty<ContextProperty>();

            return new BiddingContext(
                requestId,
                biddingRound,
                competingAdsCount,
                floorPrice,
                competitionIntensity,
                biddingDeadline,
                environmentData: environmentProperties.ToList().AsReadOnly());
        }

        /// <summary>
        /// 是否竞价已超时
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > BiddingDeadline;

        /// <summary>
        /// 是否有效的竞价上下文
        /// </summary>
        public bool IsValid => !IsExpired && FloorPrice > 0;

        /// <summary>
        /// 获取竞价剩余时间
        /// </summary>
        public TimeSpan GetRemainingTime()
        {
            var remaining = BiddingDeadline - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// 计算竞价成功率
        /// </summary>
        public decimal CalculateWinRate()
        {
            if (CompetingAdsCount == 0)
                return 1m;

            // 基于竞争激烈程度计算成功率
            return Math.Max(0.1m, 1m - CompetitionIntensity);
        }

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RequestId;
            yield return BiddingRound;
            yield return CompetingAdsCount;
            yield return FloorPrice;
            yield return HighestBid ?? 0m;
            yield return AverageBid ?? 0m;
            yield return CompetitionIntensity;
            yield return MarketPrice ?? 0m;
            yield return BiddingDeadline;

            // 环境数据的比较
            foreach (var property in EnvironmentData.OrderBy(x => x.PropertyKey))
            {
                yield return property;
            }
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(
            string requestId,
            int biddingRound,
            int competingAdsCount,
            decimal floorPrice,
            decimal competitionIntensity,
            DateTime biddingDeadline)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                throw new ArgumentException("请求ID不能为空", nameof(requestId));

            if (biddingRound < 1)
                throw new ArgumentException("竞价轮次必须大于0", nameof(biddingRound));

            if (competingAdsCount < 0)
                throw new ArgumentException("参与竞价的广告数量不能为负数", nameof(competingAdsCount));

            if (floorPrice < 0)
                throw new ArgumentException("底价不能为负数", nameof(floorPrice));

            if (competitionIntensity < 0 || competitionIntensity > 1)
                throw new ArgumentException("竞争激烈程度必须在0-1之间", nameof(competitionIntensity));

            if (biddingDeadline <= DateTime.UtcNow)
                throw new ArgumentException("竞价截止时间必须在当前时间之后", nameof(biddingDeadline));
        }
    }
}
