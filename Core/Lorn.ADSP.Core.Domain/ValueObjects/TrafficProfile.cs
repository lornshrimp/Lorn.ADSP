using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 流量概况值对象
    /// </summary>
    public class TrafficProfile : ValueObject
    {
        /// <summary>
        /// 日库存量
        /// </summary>
        public int DailyInventory { get; private set; }

        /// <summary>
        /// 峰值小时库存量
        /// </summary>
        public int PeakHourInventory { get; private set; }

        /// <summary>
        /// 受众细分数据
        /// </summary>
        public IReadOnlyList<ContextProperty> AudienceSegments { get; private set; }

        /// <summary>
        /// 地理分布数据
        /// </summary>
        public IReadOnlyList<ContextProperty> GeographicDistribution { get; private set; }

        /// <summary>
        /// 平均观看时间
        /// </summary>
        public decimal AverageViewTime { get; private set; }

        /// <summary>
        /// 跳出率
        /// </summary>
        public decimal BounceRate { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private TrafficProfile(
            int dailyInventory,
            int peakHourInventory,
            IReadOnlyList<ContextProperty> audienceSegments,
            IReadOnlyList<ContextProperty> geographicDistribution,
            decimal averageViewTime,
            decimal bounceRate)
        {
            DailyInventory = dailyInventory;
            PeakHourInventory = peakHourInventory;
            AudienceSegments = audienceSegments;
            GeographicDistribution = geographicDistribution;
            AverageViewTime = averageViewTime;
            BounceRate = bounceRate;
        }

        /// <summary>
        /// 创建流量概况
        /// </summary>
        public static TrafficProfile Create(
            int dailyInventory,
            int peakHourInventory,
            decimal averageViewTime,
            decimal bounceRate,
            IEnumerable<ContextProperty>? audienceSegments = null,
            IEnumerable<ContextProperty>? geographicDistribution = null)
        {
            ValidateInputs(dailyInventory, peakHourInventory, averageViewTime, bounceRate);

            var audience = audienceSegments?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();
            var geographic = geographicDistribution?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

            return new TrafficProfile(
                dailyInventory,
                peakHourInventory,
                audience,
                geographic,
                averageViewTime,
                bounceRate);
        }

        /// <summary>
        /// 从字典创建流量概况（向后兼容）
        /// </summary>
        public static TrafficProfile CreateFromDictionaries(
            int dailyInventory,
            int peakHourInventory,
            decimal averageViewTime,
            decimal bounceRate,
            IReadOnlyDictionary<string, decimal>? audienceSegments = null,
            IReadOnlyDictionary<string, string>? geographicDistribution = null)
        {
            var audienceProperties = audienceSegments?.Select(kvp => new ContextProperty(
                kvp.Key,
                kvp.Value.ToString(),
                "Decimal",
                "AudienceSegment",
                false,
                1.0m,
                null,
                "TrafficProfile")) ?? Enumerable.Empty<ContextProperty>();

            var geographicProperties = geographicDistribution?.Select(kvp => new ContextProperty(
                kvp.Key,
                kvp.Value,
                "String",
                "GeographicDistribution",
                false,
                1.0m,
                null,
                "TrafficProfile")) ?? Enumerable.Empty<ContextProperty>();

            return Create(
                dailyInventory,
                peakHourInventory,
                averageViewTime,
                bounceRate,
                audienceProperties,
                geographicProperties);
        }

        /// <summary>
        /// 获取受众细分值
        /// </summary>
        public decimal? GetAudienceSegment(string segmentKey)
        {
            var segment = AudienceSegments.FirstOrDefault(p => p.PropertyKey == segmentKey);
            return segment?.GetValue<decimal>();
        }

        /// <summary>
        /// 获取地理分布值
        /// </summary>
        public string? GetGeographicDistribution(string regionKey)
        {
            var distribution = GeographicDistribution.FirstOrDefault(p => p.PropertyKey == regionKey);
            return distribution?.GetValue<string>();
        }

        /// <summary>
        /// 获取受众细分作为字典（向后兼容）
        /// </summary>
        public Dictionary<string, decimal> GetAudienceSegmentsAsDictionary()
        {
            var result = new Dictionary<string, decimal>();
            foreach (var segment in AudienceSegments)
            {
                if (decimal.TryParse(segment.PropertyValue, out decimal value))
                {
                    result[segment.PropertyKey] = value;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取地理分布作为字典（向后兼容）
        /// </summary>
        public Dictionary<string, string> GetGeographicDistributionAsDictionary()
        {
            var result = new Dictionary<string, string>();
            foreach (var distribution in GeographicDistribution)
            {
                result[distribution.PropertyKey] = distribution.PropertyValue;
            }
            return result;
        }

        /// <summary>
        /// 计算流量质量分数
        /// </summary>
        public decimal CalculateQualityScore()
        {
            // 基于平均观看时间和跳出率计算质量分数
            var viewTimeScore = Math.Min(AverageViewTime / 300m, 1m); // 假设300秒为最佳观看时间
            var bounceScore = Math.Max(0m, 1m - BounceRate); // 跳出率越低越好

            return (viewTimeScore + bounceScore) / 2m;
        }

        /// <summary>
        /// 计算库存利用率
        /// </summary>
        public decimal CalculateInventoryUtilization()
        {
            if (DailyInventory == 0) return 0m;
            return (decimal)PeakHourInventory / DailyInventory;
        }

        /// <summary>
        /// 是否为高流量时段
        /// </summary>
        public bool IsHighTrafficPeriod()
        {
            return CalculateInventoryUtilization() > 0.1m; // 峰值超过日均10%认为是高流量
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DailyInventory;
            yield return PeakHourInventory;
            yield return AverageViewTime;
            yield return BounceRate;

            foreach (var segment in AudienceSegments.OrderBy(s => s.PropertyKey))
            {
                yield return segment;
            }

            foreach (var distribution in GeographicDistribution.OrderBy(d => d.PropertyKey))
            {
                yield return distribution;
            }
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInputs(int dailyInventory, int peakHourInventory, decimal averageViewTime, decimal bounceRate)
        {
            if (dailyInventory < 0)
                throw new ArgumentException("日库存量不能为负数", nameof(dailyInventory));

            if (peakHourInventory < 0)
                throw new ArgumentException("峰值小时库存量不能为负数", nameof(peakHourInventory));

            if (averageViewTime < 0)
                throw new ArgumentException("平均观看时间不能为负数", nameof(averageViewTime));

            if (bounceRate < 0 || bounceRate > 1)
                throw new ArgumentException("跳出率必须在0-1之间", nameof(bounceRate));
        }
    }
}
