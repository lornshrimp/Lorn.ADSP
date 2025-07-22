using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 投放报表值对象
    /// </summary>
    public class DeliveryReport : ValueObject
    {
        /// <summary>
        /// 投放记录ID
        /// </summary>
        public string DeliveryRecordId { get; }

        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// 广告活动ID
        /// </summary>
        public string CampaignId { get; }

        /// <summary>
        /// 媒体资源ID
        /// </summary>
        public string MediaResourceId { get; }

        /// <summary>
        /// 广告位ID
        /// </summary>
        public string PlacementId { get; }

        /// <summary>
        /// 投放时间
        /// </summary>
        public DateTime DeliveredAt { get; }

        /// <summary>
        /// 投放成本
        /// </summary>
        public decimal Cost { get; }

        /// <summary>
        /// 性能指标
        /// </summary>
        public PerformanceMetrics Metrics { get; }

        /// <summary>
        /// 用户细分
        /// </summary>
        public string UserSegment { get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; }

        /// <summary>
        /// 私有构造函数（用于序列化）
        /// </summary>
        private DeliveryReport()
        {
            DeliveryRecordId = string.Empty;
            RequestId = string.Empty;
            CampaignId = string.Empty;
            MediaResourceId = string.Empty;
            PlacementId = string.Empty;
            DeliveredAt = DateTime.MinValue;
            Cost = 0;
            Metrics = null!;
            UserSegment = string.Empty;
            DeviceType = string.Empty;
            Country = string.Empty;
            City = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deliveryRecordId">投放记录ID</param>
        /// <param name="requestId">请求ID</param>
        /// <param name="campaignId">广告活动ID</param>
        /// <param name="mediaResourceId">媒体资源ID</param>
        /// <param name="placementId">广告位ID</param>
        /// <param name="deliveredAt">投放时间</param>
        /// <param name="cost">投放成本</param>
        /// <param name="metrics">性能指标</param>
        /// <param name="userSegment">用户细分</param>
        /// <param name="deviceType">设备类型</param>
        /// <param name="country">国家</param>
        /// <param name="city">城市</param>
        public DeliveryReport(
            string deliveryRecordId,
            string requestId,
            string campaignId,
            string mediaResourceId,
            string placementId,
            DateTime deliveredAt,
            decimal cost,
            PerformanceMetrics metrics,
            string userSegment,
            string deviceType,
            string country,
            string city)
        {
            // 验证输入参数
            if (string.IsNullOrWhiteSpace(deliveryRecordId))
                throw new ArgumentException("投放记录ID不能为空", nameof(deliveryRecordId));

            if (string.IsNullOrWhiteSpace(requestId))
                throw new ArgumentException("请求ID不能为空", nameof(requestId));

            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentException("广告活动ID不能为空", nameof(campaignId));

            if (string.IsNullOrWhiteSpace(mediaResourceId))
                throw new ArgumentException("媒体资源ID不能为空", nameof(mediaResourceId));

            if (string.IsNullOrWhiteSpace(placementId))
                throw new ArgumentException("广告位ID不能为空", nameof(placementId));

            if (deliveredAt == default)
                throw new ArgumentException("投放时间不能为默认值", nameof(deliveredAt));

            if (cost < 0)
                throw new ArgumentException("投放成本不能为负数", nameof(cost));

            if (metrics == null)
                throw new ArgumentNullException(nameof(metrics), "性能指标不能为空");

            DeliveryRecordId = deliveryRecordId.Trim();
            RequestId = requestId.Trim();
            CampaignId = campaignId.Trim();
            MediaResourceId = mediaResourceId.Trim();
            PlacementId = placementId.Trim();
            DeliveredAt = deliveredAt;
            Cost = cost;
            Metrics = metrics;
            UserSegment = userSegment?.Trim() ?? string.Empty;
            DeviceType = deviceType?.Trim() ?? string.Empty;
            Country = country?.Trim() ?? string.Empty;
            City = city?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// 创建投放报表
        /// </summary>
        /// <param name="deliveryRecordId">投放记录ID</param>
        /// <param name="requestId">请求ID</param>
        /// <param name="campaignId">广告活动ID</param>
        /// <param name="mediaResourceId">媒体资源ID</param>
        /// <param name="placementId">广告位ID</param>
        /// <param name="deliveredAt">投放时间</param>
        /// <param name="cost">投放成本</param>
        /// <param name="metrics">性能指标</param>
        /// <param name="userSegment">用户细分</param>
        /// <param name="deviceType">设备类型</param>
        /// <param name="country">国家</param>
        /// <param name="city">城市</param>
        /// <returns>投放报表实例</returns>
        public static DeliveryReport Create(
            string deliveryRecordId,
            string requestId,
            string campaignId,
            string mediaResourceId,
            string placementId,
            DateTime deliveredAt,
            decimal cost,
            PerformanceMetrics metrics,
            string userSegment = "",
            string deviceType = "",
            string country = "",
            string city = "")
        {
            return new DeliveryReport(
                deliveryRecordId,
                requestId,
                campaignId,
                mediaResourceId,
                placementId,
                deliveredAt,
                cost,
                metrics,
                userSegment,
                deviceType,
                country,
                city);
        }

        /// <summary>
        /// 获取投放效率（ROI）
        /// </summary>
        /// <returns>投放效率</returns>
        public decimal GetReturnOnInvestment()
        {
            if (Cost == 0) return 0;

            // 假设收入是通过点击率和转换率计算的
            var estimatedRevenue = Metrics.Clicks * 0.1m; // 简化计算
            return estimatedRevenue / Cost;
        }

        /// <summary>
        /// 获取每千次展示成本（CPM）
        /// </summary>
        /// <returns>CPM</returns>
        public decimal GetCostPerMille()
        {
            if (Metrics.Impressions == 0) return 0;
            return (Cost / Metrics.Impressions) * 1000;
        }

        /// <summary>
        /// 获取每次点击成本（CPC）
        /// </summary>
        /// <returns>CPC</returns>
        public decimal GetCostPerClick()
        {
            if (Metrics.Clicks == 0) return 0;
            return Cost / Metrics.Clicks;
        }

        /// <summary>
        /// 检查是否为有效投放
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValidDelivery()
        {
            return Metrics.Impressions > 0 &&
                   Cost >= 0 &&
                   DeliveredAt <= DateTime.UtcNow;
        }

        /// <summary>
        /// 获取地理位置信息
        /// </summary>
        /// <returns>地理位置描述</returns>
        public string GetGeographicInfo()
        {
            if (string.IsNullOrEmpty(Country) && string.IsNullOrEmpty(City))
                return "未知位置";

            if (string.IsNullOrEmpty(City))
                return Country;

            if (string.IsNullOrEmpty(Country))
                return City;

            return $"{Country}, {City}";
        }

        /// <summary>
        /// 创建具有新成本的报表副本
        /// </summary>
        /// <param name="newCost">新的成本</param>
        /// <returns>新的报表实例</returns>
        public DeliveryReport WithCost(decimal newCost)
        {
            return new DeliveryReport(
                DeliveryRecordId,
                RequestId,
                CampaignId,
                MediaResourceId,
                PlacementId,
                DeliveredAt,
                newCost,
                Metrics,
                UserSegment,
                DeviceType,
                Country,
                City);
        }

        /// <summary>
        /// 创建具有新性能指标的报表副本
        /// </summary>
        /// <param name="newMetrics">新的性能指标</param>
        /// <returns>新的报表实例</returns>
        public DeliveryReport WithMetrics(PerformanceMetrics newMetrics)
        {
            return new DeliveryReport(
                DeliveryRecordId,
                RequestId,
                CampaignId,
                MediaResourceId,
                PlacementId,
                DeliveredAt,
                Cost,
                newMetrics,
                UserSegment,
                DeviceType,
                Country,
                City);
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        /// <returns>用于相等性比较的组件</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DeliveryRecordId;
            yield return RequestId;
            yield return CampaignId;
            yield return MediaResourceId;
            yield return PlacementId;
            yield return DeliveredAt;
            yield return Cost;
            yield return Metrics;
            yield return UserSegment;
            yield return DeviceType;
            yield return Country;
            yield return City;
        }

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>投放报表描述</returns>
        public override string ToString()
        {
            var location = GetGeographicInfo();
            return $"投放报表: {CampaignId} -> {PlacementId}, 成本:{Cost:C}, 展示:{Metrics.Impressions}, 点击:{Metrics.Clicks}, 位置:{location}, 时间:{DeliveredAt:yyyy-MM-dd HH:mm}";
        }
    }
}
