using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 性能指标值对象
    /// </summary>
    public class PerformanceMetrics : ValueObject
    {
        /// <summary>
        /// 展示次数
        /// </summary>
        public long Impressions { get; private set; }

        /// <summary>
        /// 点击次数
        /// </summary>
        public long Clicks { get; private set; }

        /// <summary>
        /// 转化次数
        /// </summary>
        public long Conversions { get; private set; }

        /// <summary>
        /// 总成本（分）
        /// </summary>
        public decimal Cost { get; private set; }

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
        /// 私有构造函数
        /// </summary>
        private PerformanceMetrics(long impressions, long clicks, long conversions, decimal cost)
        {
            Impressions = impressions;
            Clicks = clicks;
            Conversions = conversions;
            Cost = cost;
        }

        /// <summary>
        /// 创建性能指标
        /// </summary>
        public static PerformanceMetrics Create(long impressions = 0, long clicks = 0, long conversions = 0, decimal cost = 0)
        {
            ValidateInputs(impressions, clicks, conversions, cost);
            return new PerformanceMetrics(impressions, clicks, conversions, cost);
        }

        /// <summary>
        /// 创建初始指标
        /// </summary>
        public static PerformanceMetrics CreateEmpty()
        {
            return new PerformanceMetrics(0, 0, 0, 0);
        }

        /// <summary>
        /// 记录展示
        /// </summary>
        public PerformanceMetrics RecordImpression()
        {
            return new PerformanceMetrics(Impressions + 1, Clicks, Conversions, Cost);
        }

        /// <summary>
        /// 记录点击
        /// </summary>
        public PerformanceMetrics RecordClick()
        {
            return new PerformanceMetrics(Impressions, Clicks + 1, Conversions, Cost);
        }

        /// <summary>
        /// 记录转化
        /// </summary>
        public PerformanceMetrics RecordConversion()
        {
            return new PerformanceMetrics(Impressions, Clicks, Conversions + 1, Cost);
        }

        /// <summary>
        /// 增加成本
        /// </summary>
        public PerformanceMetrics AddCost(decimal additionalCost)
        {
            if (additionalCost < 0)
                throw new ArgumentException("成本不能为负数", nameof(additionalCost));

            return new PerformanceMetrics(Impressions, Clicks, Conversions, Cost + additionalCost);
        }

        /// <summary>
        /// 合并两个性能指标
        /// </summary>
        public PerformanceMetrics Merge(PerformanceMetrics other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return new PerformanceMetrics(
                Impressions + other.Impressions,
                Clicks + other.Clicks,
                Conversions + other.Conversions,
                Cost + other.Cost);
        }

        /// <summary>
        /// 获取总体评分
        /// </summary>
        public decimal GetOverallScore()
        {
            // 基于CTR、CVR的综合评分 (0-1)
            var ctrScore = Math.Min(CTR * 100m, 10m) / 10m; // CTR转换为0-1分数
            var cvrScore = Math.Min(CVR * 100m, 10m) / 10m; // CVR转换为0-1分数

            return (ctrScore + cvrScore) / 2m;
        }

        /// <summary>
        /// 是否为高性能
        /// </summary>
        public bool IsHighPerformance()
        {
            return GetOverallScore() > 0.7m && CTR > 0.01m && CVR > 0.001m;
        }

        /// <summary>
        /// 获取投资回报率
        /// </summary>
        public decimal GetReturnOnInvestment(decimal revenue)
        {
            if (Cost <= 0) return 0m;
            return (revenue - Cost) / Cost;
        }

        /// <summary>
        /// 获取性能摘要
        /// </summary>
        public Dictionary<string, object> GetPerformanceSummary()
        {
            return new Dictionary<string, object>
            {
                ["Impressions"] = Impressions,
                ["Clicks"] = Clicks,
                ["Conversions"] = Conversions,
                ["Cost"] = Cost,
                ["CTR"] = CTR,
                ["CVR"] = CVR,
                ["CPC"] = CPC,
                ["CPM"] = CPM,
                ["OverallScore"] = GetOverallScore(),
                ["IsHighPerformance"] = IsHighPerformance()
            };
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Impressions;
            yield return Clicks;
            yield return Conversions;
            yield return Cost;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInputs(long impressions, long clicks, long conversions, decimal cost)
        {
            if (impressions < 0)
                throw new ArgumentException("展示次数不能为负数", nameof(impressions));

            if (clicks < 0)
                throw new ArgumentException("点击次数不能为负数", nameof(clicks));

            if (clicks > impressions)
                throw new ArgumentException("点击次数不能超过展示次数", nameof(clicks));

            if (conversions < 0)
                throw new ArgumentException("转化次数不能为负数", nameof(conversions));

            if (conversions > clicks)
                throw new ArgumentException("转化次数不能超过点击次数", nameof(conversions));

            if (cost < 0)
                throw new ArgumentException("成本不能为负数", nameof(cost));
        }
    }
}
