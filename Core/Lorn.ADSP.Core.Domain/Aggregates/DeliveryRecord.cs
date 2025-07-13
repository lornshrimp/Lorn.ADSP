using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Aggregates;

/// <summary>
/// 投放记录聚合根
/// </summary>
public class DeliveryRecord : AggregateRoot
{
    /// <summary>
    /// 广告ID
    /// </summary>
    public string AdId { get; private set; } = string.Empty;

    /// <summary>
    /// 活动ID
    /// </summary>
    public string CampaignId { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体资源ID
    /// </summary>
    public string MediaResourceId { get; private set; } = string.Empty;

    /// <summary>
    /// 广告位ID
    /// </summary>
    public string PlacementId { get; private set; } = string.Empty;

    /// <summary>
    /// 展示唯一ID
    /// </summary>
    public string ImpressionId { get; private set; } = string.Empty;

    /// <summary>
    /// 请求ID
    /// </summary>
    public string RequestId { get; private set; } = string.Empty;

    /// <summary>
    /// 投放时间戳
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// 投放时间（用于业务查询和统计）
    /// </summary>
    public DateTime DeliveredAt { get; private set; }

    /// <summary>
    /// 实际价格（分）
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// 投放成本（分） - 用于媒体资源收益计算
    /// </summary>
    public decimal Cost { get; private set; }

    /// <summary>
    /// 投放状态
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    /// 性能指标
    /// </summary>
    public PerformanceMetrics Metrics { get; private set; } = null!;

    /// <summary>
    /// 投放上下文集合
    /// 使用ITargetingContext接口提供可扩展的上下文信息存储
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingContext> DeliveryContexts { get; private set; }

    /// <summary>
    /// 点击时间
    /// </summary>
    public DateTime? ClickTime { get; private set; }

    /// <summary>
    /// 转化时间
    /// </summary>
    public DateTime? ConversionTime { get; private set; }

    /// <summary>
    /// 转化收入（分）
    /// </summary>
    public decimal? Revenue { get; private set; }

    /// <summary>
    /// 质量得分
    /// </summary>
    public decimal QualityScore { get; private set; } = 1.0m;

    /// <summary>
    /// 竞价价格（分）
    /// </summary>
    public decimal BidPrice { get; private set; }

    /// <summary>
    /// 计费详情明细
    /// </summary>
    public BillingDetails BillingDetails { get; private set; } = null!;

    /// <summary>
    /// 私有构造函数，用于ORM
    /// </summary>
    private DeliveryRecord() 
    {
        DeliveryContexts = new Dictionary<string, ITargetingContext>();
        Metrics = PerformanceMetrics.Create();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeliveryRecord(
        string adId,
        string campaignId,
        string mediaResourceId,
        string placementId,
        string impressionId,
        string requestId,
        decimal price,
        decimal cost,
        decimal bidPrice,
        IReadOnlyDictionary<string, ITargetingContext> deliveryContexts,
        BillingDetails billingDetails,
        decimal qualityScore = 1.0m)
    {
        ValidateInputs(adId, campaignId, mediaResourceId, placementId, impressionId, requestId, price, cost, bidPrice, deliveryContexts, billingDetails);

        AdId = adId;
        CampaignId = campaignId;
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
        ImpressionId = impressionId;
        RequestId = requestId;
        Price = price;
        Cost = cost;
        BidPrice = bidPrice;
        DeliveryContexts = deliveryContexts;
        BillingDetails = billingDetails;
        QualityScore = qualityScore;
        Timestamp = DateTime.UtcNow;
        DeliveredAt = DateTime.UtcNow;
        Status = DeliveryStatus.Success;
        Metrics = PerformanceMetrics.Create();

        // 触发投放记录创建事件
        AddDomainEvent(new DeliveryRecordCreatedEvent(Id, adId, impressionId, price));
    }

    /// <summary>
    /// 记录展示
    /// </summary>
    public void RecordImpression()
    {
        if (Status != DeliveryStatus.Success)
            throw new InvalidOperationException("只有成功投放状态的记录可以记录展示");

        Metrics = Metrics.RecordImpression();
        UpdateLastModifiedTime();
        AddDomainEvent(new ImpressionRecordedEvent(Id, RequestId, CampaignId));
    }

    /// <summary>
    /// 记录点击
    /// </summary>
    public void RecordClick(decimal? clickCost = null)
    {
        if (ClickTime.HasValue)
            throw new InvalidOperationException("该投放记录已记录过点击");

        if (Metrics.Impressions == 0)
            throw new InvalidOperationException("必须先有展示才能记录点击");

        ClickTime = DateTime.UtcNow;
        Metrics = Metrics.RecordClick();

        if (clickCost.HasValue)
        {
            Price += clickCost.Value;
            Cost += clickCost.Value;
            BillingDetails = BillingDetails.AddCost(clickCost.Value, "点击费用");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryClickRecordedEvent(Id, AdId, clickCost ?? 0));
    }

    /// <summary>
    /// 记录转化
    /// </summary>
    public void RecordConversion(decimal? revenue = null, decimal? conversionCost = null)
    {
        if (ConversionTime.HasValue)
            throw new InvalidOperationException("该投放记录已记录过转化");

        if (Metrics.Clicks == 0)
            throw new InvalidOperationException("必须先有点击才能记录转化");

        ConversionTime = DateTime.UtcNow;
        Revenue = revenue;
        Metrics = Metrics.RecordConversion();

        if (conversionCost.HasValue)
        {
            Price += conversionCost.Value;
            Cost += conversionCost.Value;
            BillingDetails = BillingDetails.AddCost(conversionCost.Value, "转化费用");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryConversionRecordedEvent(Id, AdId, revenue ?? 0));
    }

    /// <summary>
    /// 更新投放状态
    /// </summary>
    public void UpdateStatus(DeliveryStatus status)
    {
        if (Status == status)
            return;

        var oldStatus = Status;
        Status = status;

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryStatusUpdatedEvent(Id, status));
    }

    /// <summary>
    /// 标记为失败
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        Status = DeliveryStatus.Failed;
        BillingDetails = BillingDetails.AddNote($"失败原因: {reason}");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryFailedEvent(Id, AdId, reason));
    }

    /// <summary>
    /// 标记为超时
    /// </summary>
    public void MarkAsTimeout()
    {
        Status = DeliveryStatus.Timeout;
        BillingDetails = BillingDetails.AddNote("投放超时");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryTimeoutEvent(Id, AdId));
    }

    /// <summary>
    /// 获取投放效果
    /// </summary>
    public DeliveryPerformance GetPerformance()
    {
        return new DeliveryPerformance(
            hasClick: ClickTime.HasValue,
            hasConversion: ConversionTime.HasValue,
            totalCost: Cost,
            revenue: Revenue ?? 0,
            qualityScore: QualityScore,
            clickLatency: ClickTime.HasValue ? (ClickTime.Value - Timestamp).TotalMilliseconds : null,
            conversionLatency: ConversionTime.HasValue ? (ConversionTime.Value - Timestamp).TotalMilliseconds : null
        );
    }

    /// <summary>
    /// 是否有效投放
    /// </summary>
    public bool IsSuccessfulDelivery => Status == DeliveryStatus.Success;

    /// <summary>
    /// 是否有点击
    /// </summary>
    public bool HasClick => ClickTime.HasValue;

    /// <summary>
    /// 是否有转化
    /// </summary>
    public bool HasConversion => ConversionTime.HasValue;

    /// <summary>
    /// 获取投放时长（毫秒）
    /// </summary>
    public double GetDeliveryDurationMs()
    {
        return (DateTime.UtcNow - Timestamp).TotalMilliseconds;
    }

    /// <summary>
    /// 获取点击时长（毫秒）
    /// </summary>
    public double? GetClickDurationMs()
    {
        return ClickTime.HasValue ? (ClickTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// 获取转化时长（毫秒）
    /// </summary>
    public double? GetConversionDurationMs()
    {
        return ConversionTime.HasValue ? (ConversionTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// 计算ROI（投资回报率）
    /// </summary>
    public decimal? CalculateROI()
    {
        if (!Revenue.HasValue || Cost == 0)
            return null;

        return (Revenue.Value - Cost) / Cost;
    }

    /// <summary>
    /// 计算ROAS（广告支出回报率）
    /// </summary>
    public decimal? CalculateROAS()
    {
        if (!Revenue.HasValue || Cost == 0)
            return null;

        return Revenue.Value / Cost;
    }

    /// <summary>
    /// 获取特定类型的上下文信息
    /// </summary>
    /// <typeparam name="T">上下文类型</typeparam>
    /// <param name="contextType">上下文类型标识</param>
    /// <returns>指定类型的上下文信息，如果不存在则返回null</returns>
    public T? GetContext<T>(string contextType) where T : class, ITargetingContext
    {
        return DeliveryContexts.TryGetValue(contextType, out var context) ? context as T : null;
    }

    /// <summary>
    /// 检查是否包含特定类型的上下文
    /// </summary>
    /// <param name="contextType">上下文类型标识</param>
    /// <returns>是否包含指定类型的上下文</returns>
    public bool HasContext(string contextType)
    {
        return DeliveryContexts.ContainsKey(contextType);
    }

    /// <summary>
    /// 获取所有上下文类型
    /// </summary>
    /// <returns>所有上下文类型的集合</returns>
    public IReadOnlyCollection<string> GetContextTypes()
    {
        return DeliveryContexts.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 从上下文中获取用户标识符
    /// </summary>
    /// <returns>用户标识符，如果不存在则返回null</returns>
    public string? GetUserId()
    {
        var userContext = GetContext<ITargetingContext>("User");
        return userContext?.GetProperty<string>("UserId");
    }

    /// <summary>
    /// 从上下文中获取设备类型
    /// </summary>
    /// <returns>设备类型，如果不存在则返回null</returns>
    public string? GetDeviceType()
    {
        var deviceContext = GetContext<ITargetingContext>("Device");
        return deviceContext?.GetProperty<string>("DeviceType");
    }

    /// <summary>
    /// 从上下文中获取地理位置信息
    /// </summary>
    /// <returns>地理位置信息，如果不存在则返回null</returns>
    public string? GetGeoLocation()
    {
        var geoContext = GetContext<ITargetingContext>("Geo");
        return geoContext?.GetProperty<string>("Country") ?? geoContext?.GetProperty<string>("Region");
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string adId, string campaignId, string mediaResourceId, string placementId, 
        string impressionId, string requestId, decimal price, decimal cost, decimal bidPrice, 
        IReadOnlyDictionary<string, ITargetingContext> deliveryContexts, BillingDetails billingDetails)
    {
        if (string.IsNullOrWhiteSpace(adId))
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (string.IsNullOrWhiteSpace(campaignId))
            throw new ArgumentException("活动ID不能为空", nameof(campaignId));

        if (string.IsNullOrWhiteSpace(mediaResourceId))
            throw new ArgumentException("媒体资源ID不能为空", nameof(mediaResourceId));

        if (string.IsNullOrWhiteSpace(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (string.IsNullOrWhiteSpace(impressionId))
            throw new ArgumentException("展示ID不能为空", nameof(impressionId));

        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "价格不能为负数");

        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "成本不能为负数");

        if (bidPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(bidPrice), "竞价价格不能为负数");

        ArgumentNullException.ThrowIfNull(deliveryContexts);
        ArgumentNullException.ThrowIfNull(billingDetails);
    }
}

