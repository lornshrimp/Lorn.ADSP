using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
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
    /// 展示请求ID
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
    /// 结算价格（分）
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// 投放状态
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    /// 投放上下文
    /// </summary>
    public DeliveryContext Context { get; private set; } = null!;

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
    /// 结算费用明细
    /// </summary>
    public BillingDetails BillingDetails { get; private set; } = null!;

    /// <summary>
    /// 私有构造函数（用于ORM）
    /// </summary>
    private DeliveryRecord() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeliveryRecord(
        string adId,
        string impressionId,
        string requestId,
        decimal price,
        decimal bidPrice,
        DeliveryContext context,
        BillingDetails billingDetails,
        decimal qualityScore = 1.0m)
    {
        ValidateInputs(adId, impressionId, requestId, price, bidPrice, context, billingDetails);

        AdId = adId;
        ImpressionId = impressionId;
        RequestId = requestId;
        Price = price;
        BidPrice = bidPrice;
        Context = context;
        BillingDetails = billingDetails;
        QualityScore = qualityScore;
        Timestamp = DateTime.UtcNow;
        Status = DeliveryStatus.Success;

        // 发布投放记录创建事件
        AddDomainEvent(new DeliveryRecordCreatedEvent(Id, adId, impressionId, price));
    }

    /// <summary>
    /// 记录点击
    /// </summary>
    public void RecordClick(decimal? clickCost = null)
    {
        if (ClickTime.HasValue)
            throw new InvalidOperationException("该投放记录已记录过点击");

        ClickTime = DateTime.UtcNow;

        if (clickCost.HasValue)
        {
            Price += clickCost.Value;
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

        ConversionTime = DateTime.UtcNow;
        Revenue = revenue;

        if (conversionCost.HasValue)
        {
            Price += conversionCost.Value;
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
        AddDomainEvent(new DeliveryStatusUpdatedEvent(Id, AdId, oldStatus, status));
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
            totalCost: Price,
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
    /// 是否产生点击
    /// </summary>
    public bool HasClick => ClickTime.HasValue;

    /// <summary>
    /// 是否产生转化
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
        if (!Revenue.HasValue || Price == 0)
            return null;

        return (Revenue.Value - Price) / Price;
    }

    /// <summary>
    /// 计算ROAS（广告支出回报率）
    /// </summary>
    public decimal? CalculateROAS()
    {
        if (!Revenue.HasValue || Price == 0)
            return null;

        return Revenue.Value / Price;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string adId, string impressionId, string requestId, 
        decimal price, decimal bidPrice, DeliveryContext context, BillingDetails billingDetails)
    {
        if (string.IsNullOrWhiteSpace(adId))
            throw new ArgumentException("广告ID不能为空", nameof(adId));

        if (string.IsNullOrWhiteSpace(impressionId))
            throw new ArgumentException("展示ID不能为空", nameof(impressionId));

        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "价格不能为负数");

        if (bidPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(bidPrice), "竞价价格不能为负数");

        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(billingDetails);
    }
}

/// <summary>
/// 投放上下文值对象
/// </summary>
public class DeliveryContext : ValueObject
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public string? DeviceId { get; private set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// 引用页面
    /// </summary>
    public string? Referrer { get; private set; }

    /// <summary>
    /// 页面URL
    /// </summary>
    public string? PageUrl { get; private set; }

    /// <summary>
    /// 地理位置
    /// </summary>
    public string? GeoLocation { get; private set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string? Language { get; private set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public DeviceType? DeviceType { get; private set; }

    /// <summary>
    /// 媒体类型
    /// </summary>
    public MediaType MediaType { get; private set; }

    /// <summary>
    /// 广告位ID
    /// </summary>
    public string? PlacementId { get; private set; }

    /// <summary>
    /// 广告位尺寸
    /// </summary>
    public string? AdSize { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private DeliveryContext() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeliveryContext(
        MediaType mediaType,
        string? userId = null,
        string? deviceId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? referrer = null,
        string? pageUrl = null,
        string? geoLocation = null,
        string? language = null,
        DeviceType? deviceType = null,
        string? placementId = null,
        string? adSize = null)
    {
        MediaType = mediaType;
        UserId = userId;
        DeviceId = deviceId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Referrer = referrer;
        PageUrl = pageUrl;
        GeoLocation = geoLocation;
        Language = language;
        DeviceType = deviceType;
        PlacementId = placementId;
        AdSize = adSize;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId ?? string.Empty;
        yield return DeviceId ?? string.Empty;
        yield return IpAddress ?? string.Empty;
        yield return UserAgent ?? string.Empty;
        yield return Referrer ?? string.Empty;
        yield return PageUrl ?? string.Empty;
        yield return GeoLocation ?? string.Empty;
        yield return Language ?? string.Empty;
        yield return DeviceType ?? Shared.Enums.DeviceType.PersonalComputer;
        yield return MediaType;
        yield return PlacementId ?? string.Empty;
        yield return AdSize ?? string.Empty;
    }
}

/// <summary>
/// 结算费用明细值对象
/// </summary>
public class BillingDetails : ValueObject
{
    /// <summary>
    /// 基础费用（分）
    /// </summary>
    public decimal BaseCost { get; private set; }

    /// <summary>
    /// 额外费用列表
    /// </summary>
    public IReadOnlyList<AdditionalCost> AdditionalCosts { get; private set; } = new List<AdditionalCost>();

    /// <summary>
    /// 费用说明
    /// </summary>
    public IReadOnlyList<string> Notes { get; private set; } = new List<string>();

    /// <summary>
    /// 计费模式
    /// </summary>
    public string BillingMode { get; private set; } = string.Empty;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BillingDetails() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BillingDetails(
        decimal baseCost,
        string billingMode,
        IList<AdditionalCost>? additionalCosts = null,
        IList<string>? notes = null)
    {
        if (baseCost < 0)
            throw new ArgumentOutOfRangeException(nameof(baseCost), "基础费用不能为负数");

        if (string.IsNullOrWhiteSpace(billingMode))
            throw new ArgumentException("计费模式不能为空", nameof(billingMode));

        BaseCost = baseCost;
        BillingMode = billingMode;
        AdditionalCosts = additionalCosts?.ToList() ?? new List<AdditionalCost>();
        Notes = notes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// 创建CPM计费
    /// </summary>
    public static BillingDetails CreateCPM(decimal cost)
    {
        return new BillingDetails(cost, "CPM");
    }

    /// <summary>
    /// 创建CPC计费
    /// </summary>
    public static BillingDetails CreateCPC(decimal cost)
    {
        return new BillingDetails(cost, "CPC");
    }

    /// <summary>
    /// 创建CPA计费
    /// </summary>
    public static BillingDetails CreateCPA(decimal cost)
    {
        return new BillingDetails(cost, "CPA");
    }

    /// <summary>
    /// 添加额外费用
    /// </summary>
    public BillingDetails AddCost(decimal amount, string description)
    {
        var newAdditionalCosts = AdditionalCosts.ToList();
        newAdditionalCosts.Add(new AdditionalCost(amount, description));

        return new BillingDetails(BaseCost, BillingMode, newAdditionalCosts, Notes.ToList());
    }

    /// <summary>
    /// 添加说明
    /// </summary>
    public BillingDetails AddNote(string note)
    {
        var newNotes = Notes.ToList();
        newNotes.Add(note);

        return new BillingDetails(BaseCost, BillingMode, AdditionalCosts.ToList(), newNotes);
    }

    /// <summary>
    /// 获取总费用
    /// </summary>
    public decimal GetTotalCost()
    {
        return BaseCost + AdditionalCosts.Sum(c => c.Amount);
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BaseCost;
        yield return BillingMode;
        foreach (var cost in AdditionalCosts)
            yield return cost;
        foreach (var note in Notes)
            yield return note;
    }
}

/// <summary>
/// 额外费用值对象
/// </summary>
public class AdditionalCost : ValueObject
{
    /// <summary>
    /// 费用金额（分）
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// 费用描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 费用时间
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AdditionalCost(decimal amount, string description)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "费用金额不能为负数");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("费用描述不能为空", nameof(description));

        Amount = amount;
        Description = description;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Description;
        yield return Timestamp;
    }
}

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

#region Delivery Events

/// <summary>
/// 投放记录创建事件
/// </summary>
public class DeliveryRecordCreatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryRecordCreated";

    public string DeliveryRecordId { get; }
    public string AdId { get; }
    public string ImpressionId { get; }
    public decimal Price { get; }

    public DeliveryRecordCreatedEvent(string deliveryRecordId, string adId, string impressionId, decimal price)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        ImpressionId = impressionId;
        Price = price;
    }
}

/// <summary>
/// 投放点击记录事件
/// </summary>
public class DeliveryClickRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryClickRecorded";

    public string DeliveryRecordId { get; }
    public string AdId { get; }
    public decimal ClickCost { get; }

    public DeliveryClickRecordedEvent(string deliveryRecordId, string adId, decimal clickCost)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        ClickCost = clickCost;
    }
}

/// <summary>
/// 投放转化记录事件
/// </summary>
public class DeliveryConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryConversionRecorded";

    public string DeliveryRecordId { get; }
    public string AdId { get; }
    public decimal Revenue { get; }

    public DeliveryConversionRecordedEvent(string deliveryRecordId, string adId, decimal revenue)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        Revenue = revenue;
    }
}

/// <summary>
/// 投放状态更新事件
/// </summary>
public class DeliveryStatusUpdatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryStatusUpdated";

    public string DeliveryRecordId { get; }
    public string AdId { get; }
    public DeliveryStatus OldStatus { get; }
    public DeliveryStatus NewStatus { get; }

    public DeliveryStatusUpdatedEvent(string deliveryRecordId, string adId, DeliveryStatus oldStatus, DeliveryStatus newStatus)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

/// <summary>
/// 投放失败事件
/// </summary>
public class DeliveryFailedEvent : DomainEventBase
{
    public override string EventType => "DeliveryFailed";

    public string DeliveryRecordId { get; }
    public string AdId { get; }
    public string Reason { get; }

    public DeliveryFailedEvent(string deliveryRecordId, string adId, string reason)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        Reason = reason;
    }
}

/// <summary>
/// 投放超时事件
/// </summary>
public class DeliveryTimeoutEvent : DomainEventBase
{
    public override string EventType => "DeliveryTimeout";

    public string DeliveryRecordId { get; }
    public string AdId { get; }

    public DeliveryTimeoutEvent(string deliveryRecordId, string adId)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
    }
}

#endregion