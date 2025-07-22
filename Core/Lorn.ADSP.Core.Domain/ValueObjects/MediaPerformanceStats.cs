using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 媒体性能统计值对象
    /// </summary>
    public class MediaPerformanceStats : ValueObject
    {
        /// <summary>
        /// 总展示次数
        /// </summary>
        public long TotalImpressions { get; private set; }

        /// <summary>
        /// 总点击次数
        /// </summary>
        public long TotalClicks { get; private set; }

        /// <summary>
        /// 总收入
        /// </summary>
        public decimal TotalRevenue { get; private set; }

        /// <summary>
        /// 点击率
        /// </summary>
        public decimal ClickThroughRate { get; private set; }

        /// <summary>
        /// 千次展示收入
        /// </summary>
        public decimal RevenuePerThousandImpressions { get; private set; }

        /// <summary>
        /// 填充率
        /// </summary>
        public decimal FillRate { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private MediaPerformanceStats(
            long totalImpressions,
            long totalClicks,
            decimal totalRevenue,
            decimal clickThroughRate,
            decimal revenuePerThousandImpressions,
            decimal fillRate)
        {
            TotalImpressions = totalImpressions;
            TotalClicks = totalClicks;
            TotalRevenue = totalRevenue;
            ClickThroughRate = clickThroughRate;
            RevenuePerThousandImpressions = revenuePerThousandImpressions;
            FillRate = fillRate;
        }

        /// <summary>
        /// 创建媒体性能统计
        /// </summary>
        public static MediaPerformanceStats Create(
            long totalImpressions,
            long totalClicks,
            decimal totalRevenue,
            decimal fillRate)
        {
            ValidateInputs(totalImpressions, totalClicks, totalRevenue, fillRate);

            var clickThroughRate = totalImpressions > 0 ? (decimal)totalClicks / totalImpressions : 0m;
            var revenuePerThousandImpressions = totalImpressions > 0 ? (totalRevenue / totalImpressions) * 1000m : 0m;

            return new MediaPerformanceStats(
                totalImpressions,
                totalClicks,
                totalRevenue,
                clickThroughRate,
                revenuePerThousandImpressions,
                fillRate);
        }

        /// <summary>
        /// 创建空的统计对象
        /// </summary>
        public static MediaPerformanceStats CreateEmpty()
        {
            return new MediaPerformanceStats(0, 0, 0m, 0m, 0m, 0m);
        }

        /// <summary>
        /// 增加展示次数
        /// </summary>
        public MediaPerformanceStats AddImpressions(long impressions)
        {
            if (impressions < 0)
                throw new ArgumentException("展示次数不能为负数", nameof(impressions));

            return Create(
                TotalImpressions + impressions,
                TotalClicks,
                TotalRevenue,
                FillRate);
        }

        /// <summary>
        /// 增加点击次数
        /// </summary>
        public MediaPerformanceStats AddClicks(long clicks)
        {
            if (clicks < 0)
                throw new ArgumentException("点击次数不能为负数", nameof(clicks));

            return Create(
                TotalImpressions,
                TotalClicks + clicks,
                TotalRevenue,
                FillRate);
        }

        /// <summary>
        /// 增加收入
        /// </summary>
        public MediaPerformanceStats AddRevenue(decimal revenue)
        {
            if (revenue < 0)
                throw new ArgumentException("收入不能为负数", nameof(revenue));

            return Create(
                TotalImpressions,
                TotalClicks,
                TotalRevenue + revenue,
                FillRate);
        }

        /// <summary>
        /// 计算整体性能分数
        /// </summary>
        public decimal CalculatePerformanceScore()
        {
            // 基于CTR、RPM和填充率的综合评分
            var ctrScore = Math.Min(ClickThroughRate * 100m, 10m) / 10m; // CTR转换为0-1分数
            var rpmScore = Math.Min(RevenuePerThousandImpressions / 10m, 1m); // RPM转换为0-1分数
            var fillScore = FillRate; // 填充率本身就是0-1

            return (ctrScore + rpmScore + fillScore) / 3m;
        }

        /// <summary>
        /// 是否为高性能媒体
        /// </summary>
        public bool IsHighPerformance()
        {
            return CalculatePerformanceScore() > 0.7m;
        }

        /// <summary>
        /// 获取统计摘要
        /// </summary>
        public Dictionary<string, object> GetStatsSummary()
        {
            return new Dictionary<string, object>
            {
                ["TotalImpressions"] = TotalImpressions,
                ["TotalClicks"] = TotalClicks,
                ["TotalRevenue"] = TotalRevenue,
                ["ClickThroughRate"] = ClickThroughRate,
                ["RevenuePerThousandImpressions"] = RevenuePerThousandImpressions,
                ["FillRate"] = FillRate,
                ["PerformanceScore"] = CalculatePerformanceScore(),
                ["IsHighPerformance"] = IsHighPerformance()
            };
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalImpressions;
            yield return TotalClicks;
            yield return TotalRevenue;
            yield return ClickThroughRate;
            yield return RevenuePerThousandImpressions;
            yield return FillRate;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInputs(long totalImpressions, long totalClicks, decimal totalRevenue, decimal fillRate)
        {
            if (totalImpressions < 0)
                throw new ArgumentException("总展示次数不能为负数", nameof(totalImpressions));

            if (totalClicks < 0)
                throw new ArgumentException("总点击次数不能为负数", nameof(totalClicks));

            if (totalClicks > totalImpressions)
                throw new ArgumentException("点击次数不能超过展示次数", nameof(totalClicks));

            if (totalRevenue < 0)
                throw new ArgumentException("总收入不能为负数", nameof(totalRevenue));

            if (fillRate < 0 || fillRate > 1)
                throw new ArgumentException("填充率必须在0-1之间", nameof(fillRate));
        }
    }
}
