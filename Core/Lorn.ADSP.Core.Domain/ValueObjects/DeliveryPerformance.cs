using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放效果值对象
    /// </summary>
    public class DeliveryPerformance : ValueObject
    {
        /// <summary>
        /// 是否有点击
        /// </summary>
        public bool HasClick { get; private set; }

        /// <summary>
        /// 是否有转化
        /// </summary>
        public bool HasConversion { get; private set; }

        /// <summary>
        /// 总费用（分）
        /// </summary>
        public decimal TotalCost { get; private set; }

        /// <summary>
        /// 收入（分）
        /// </summary>
        public decimal Revenue { get; private set; }

        /// <summary>
        /// 质量得分
        /// </summary>
        public decimal QualityScore { get; private set; }

        /// <summary>
        /// 点击延迟（毫秒）
        /// </summary>
        public double? ClickLatency { get; private set; }

        /// <summary>
        /// 转化延迟（毫秒）
        /// </summary>
        public double? ConversionLatency { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeliveryPerformance(
            bool hasClick,
            bool hasConversion,
            decimal totalCost,
            decimal revenue,
            decimal qualityScore,
            double? clickLatency = null,
            double? conversionLatency = null)
        {
            HasClick = hasClick;
            HasConversion = hasConversion;
            TotalCost = totalCost;
            Revenue = revenue;
            QualityScore = qualityScore;
            ClickLatency = clickLatency;
            ConversionLatency = conversionLatency;
        }

        /// <summary>
        /// 计算ROI
        /// </summary>
        public decimal? CalculateROI()
        {
            if (TotalCost == 0)
                return null;

            return (Revenue - TotalCost) / TotalCost;
        }

        /// <summary>
        /// 计算ROAS
        /// </summary>
        public decimal? CalculateROAS()
        {
            if (TotalCost == 0)
                return null;

            return Revenue / TotalCost;
        }

        /// <summary>
        /// 获取效果等级
        /// </summary>
        public string GetPerformanceGrade()
        {
            var roas = CalculateROAS();
            if (!roas.HasValue)
                return "N/A";

            return roas.Value switch
            {
                >= 3.0m => "优秀",
                >= 2.0m => "良好",
                >= 1.0m => "一般",
                _ => "较差"
            };
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return HasClick;
            yield return HasConversion;
            yield return TotalCost;
            yield return Revenue;
            yield return QualityScore;
            yield return ClickLatency ?? 0;
            yield return ConversionLatency ?? 0;
        }
    }
}
