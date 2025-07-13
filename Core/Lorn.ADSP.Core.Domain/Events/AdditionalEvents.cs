using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Events;

// 添加缺失的事件

/// <summary>
/// 广告停止事件
/// </summary>
public class AdvertisementStoppedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementStopped";

    public string AdvertisementId { get; }

    public AdvertisementStoppedEvent(string advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// 活动开始事件
/// </summary>
public class CampaignStartedEvent : DomainEventBase
{
    public override string EventType => "CampaignStarted";

    public string CampaignId { get; }
    public string AdvertisementId { get; }
    public decimal Budget { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public CampaignStartedEvent(string campaignId, string advertisementId, decimal budget, DateTime? startDate, DateTime? endDate)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
        Budget = budget;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// 活动恢复事件
/// </summary>
public class CampaignResumedEvent : DomainEventBase
{
    public override string EventType => "CampaignResumed";

    public string CampaignId { get; }

    public CampaignResumedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动停止事件
/// </summary>
public class CampaignStoppedEvent : DomainEventBase
{
    public override string EventType => "CampaignStopped";

    public string CampaignId { get; }

    public CampaignStoppedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动完成事件
/// </summary>
public class CampaignCompletedEvent : DomainEventBase
{
    public override string EventType => "CampaignCompleted";

    public string CampaignId { get; }

    public CampaignCompletedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动定向更新事件
/// </summary>
public class CampaignTargetingUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignTargetingUpdated";

    public string CampaignId { get; }

    public CampaignTargetingUpdatedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 预算耗尽事件
/// </summary>
public class BudgetExhaustedEvent : DomainEventBase
{
    public override string EventType => "BudgetExhausted";

    public string CampaignId { get; }
    public decimal BudgetLimit { get; }
    public decimal SpentAmount { get; }
    public DateTime ExhaustedAt { get; }

    public BudgetExhaustedEvent(string campaignId, decimal budgetLimit, decimal spentAmount, DateTime exhaustedAt)
    {
        CampaignId = campaignId;
        BudgetLimit = budgetLimit;
        SpentAmount = spentAmount;
        ExhaustedAt = exhaustedAt;
    }
}

/// <summary>
/// 投放记录事件
/// </summary>
public class DeliveryRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryRecorded";

    public string CampaignId { get; }
    public string DeliveryRecordId { get; }
    public decimal Cost { get; }

    public DeliveryRecordedEvent(string campaignId, string deliveryRecordId, decimal cost)
    {
        CampaignId = campaignId;
        DeliveryRecordId = deliveryRecordId;
        Cost = cost;
    }
}

#region Advertiser Events

/// <summary>
/// 广告主注册事件
/// </summary>
public class AdvertiserRegisteredEvent : DomainEventBase
{
    public override string EventType => "AdvertiserRegistered";

    public string AdvertiserId { get; }
    public string CompanyName { get; }
    public string Email { get; }

    public AdvertiserRegisteredEvent(string advertiserId, string companyName, string email)
    {
        AdvertiserId = advertiserId;
        CompanyName = companyName;
        Email = email;
    }
}

/// <summary>
/// 广告主资质提交事件
/// </summary>
public class AdvertiserQualificationSubmittedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserQualificationSubmitted";

    public string AdvertiserId { get; }
    public string BusinessLicense { get; }

    public AdvertiserQualificationSubmittedEvent(string advertiserId, string businessLicense)
    {
        AdvertiserId = advertiserId;
        BusinessLicense = businessLicense;
    }
}

/// <summary>
/// 广告主激活事件
/// </summary>
public class AdvertiserActivatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserActivated";

    public string AdvertiserId { get; }

    public AdvertiserActivatedEvent(string advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// 广告主暂停事件
/// </summary>
public class AdvertiserSuspendedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserSuspended";

    public string AdvertiserId { get; }
    public string Reason { get; }

    public AdvertiserSuspendedEvent(string advertiserId, string reason)
    {
        AdvertiserId = advertiserId;
        Reason = reason;
    }
}

/// <summary>
/// 广告主拒绝事件
/// </summary>
public class AdvertiserRejectedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserRejected";

    public string AdvertiserId { get; }
    public string Reason { get; }

    public AdvertiserRejectedEvent(string advertiserId, string reason)
    {
        AdvertiserId = advertiserId;
        Reason = reason;
    }
}

/// <summary>
/// 广告主恢复事件
/// </summary>
public class AdvertiserResumedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserResumed";

    public string AdvertiserId { get; }

    public AdvertiserResumedEvent(string advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// 广告主账单更新事件
/// </summary>
public class AdvertiserBillingUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserBillingUpdated";

    public string AdvertiserId { get; }

    public AdvertiserBillingUpdatedEvent(string advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// 广告主联系信息更新事件
/// </summary>
public class AdvertiserContactUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserContactUpdated";

    public string AdvertiserId { get; }
    public string Email { get; }

    public AdvertiserContactUpdatedEvent(string advertiserId, string email)
    {
        AdvertiserId = advertiserId;
        Email = email;
    }
}

/// <summary>
/// 广告主添加广告事件
/// </summary>
public class AdvertiserAdvertisementAddedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserAdvertisementAdded";

    public string AdvertiserId { get; }
    public string AdvertisementId { get; }

    public AdvertiserAdvertisementAddedEvent(string advertiserId, string advertisementId)
    {
        AdvertiserId = advertiserId;
        AdvertisementId = advertisementId;
    }
}

#endregion

#region MediaResource Events

/// <summary>
/// 媒体资源创建事件
/// </summary>
public class MediaResourceCreatedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceCreated";

    public string MediaResourceId { get; }
    public string Name { get; }
    public MediaType Type { get; }
    public string PublisherId { get; }

    public MediaResourceCreatedEvent(string mediaResourceId, string name, MediaType type, string publisherId)
    {
        MediaResourceId = mediaResourceId;
        Name = name;
        Type = type;
        PublisherId = publisherId;
    }
}

/// <summary>
/// 媒体资源配置事件
/// </summary>
public class MediaResourceConfiguredEvent : DomainEventBase
{
    public override string EventType => "MediaResourceConfigured";

    public string MediaResourceId { get; }

    public MediaResourceConfiguredEvent(string mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// 媒体资源流量更新事件
/// </summary>
public class MediaResourceTrafficUpdatedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceTrafficUpdated";

    public string MediaResourceId { get; }

    public MediaResourceTrafficUpdatedEvent(string mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// 媒体资源启用事件
/// </summary>
public class MediaResourceEnabledEvent : DomainEventBase
{
    public override string EventType => "MediaResourceEnabled";

    public string MediaResourceId { get; }

    public MediaResourceEnabledEvent(string mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// 媒体资源禁用事件
/// </summary>
public class MediaResourceDisabledEvent : DomainEventBase
{
    public override string EventType => "MediaResourceDisabled";

    public string MediaResourceId { get; }

    public MediaResourceDisabledEvent(string mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// 媒体资源审核通过事件
/// </summary>
public class MediaResourceApprovedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceApproved";

    public string MediaResourceId { get; }

    public MediaResourceApprovedEvent(string mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// 媒体资源审核拒绝事件
/// </summary>
public class MediaResourceRejectedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceRejected";

    public string MediaResourceId { get; }
    public string Reason { get; }

    public MediaResourceRejectedEvent(string mediaResourceId, string reason)
    {
        MediaResourceId = mediaResourceId;
        Reason = reason;
    }
}

/// <summary>
/// 媒体资源添加广告位事件
/// </summary>
public class MediaResourcePlacementAddedEvent : DomainEventBase
{
    public override string EventType => "MediaResourcePlacementAdded";

    public string MediaResourceId { get; }
    public string PlacementId { get; }

    public MediaResourcePlacementAddedEvent(string mediaResourceId, string placementId)
    {
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
    }
}

/// <summary>
/// 媒体资源移除广告位事件
/// </summary>
public class MediaResourcePlacementRemovedEvent : DomainEventBase
{
    public override string EventType => "MediaResourcePlacementRemoved";

    public string MediaResourceId { get; }
    public string PlacementId { get; }

    public MediaResourcePlacementRemovedEvent(string mediaResourceId, string placementId)
    {
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
    }
}

/// <summary>
/// 媒体资源投放记录事件
/// </summary>
public class MediaResourceDeliveryRecordedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceDeliveryRecorded";

    public string MediaResourceId { get; }
    public string DeliveryRecordId { get; }

    public MediaResourceDeliveryRecordedEvent(string mediaResourceId, string deliveryRecordId)
    {
        MediaResourceId = mediaResourceId;
        DeliveryRecordId = deliveryRecordId;
    }
}

#endregion

#region DeliveryRecord Events

/// <summary>
/// 投放记录创建事件
/// </summary>
public class DeliveryRecordCreatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryRecordCreated";

    public string DeliveryRecordId { get; }
    public string RequestId { get; }
    public string CampaignId { get; }
    public decimal Cost { get; }

    public DeliveryRecordCreatedEvent(string deliveryRecordId, string requestId, string campaignId, decimal cost)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// 展示记录事件
/// </summary>
public class ImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "ImpressionRecorded";

    public string DeliveryRecordId { get; }
    public string RequestId { get; }
    public string CampaignId { get; }

    public ImpressionRecordedEvent(string deliveryRecordId, string requestId, string campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// 点击记录事件
/// </summary>
public class ClickRecordedEvent : DomainEventBase
{
    public override string EventType => "ClickRecorded";

    public string DeliveryRecordId { get; }
    public string RequestId { get; }
    public string CampaignId { get; }

    public ClickRecordedEvent(string deliveryRecordId, string requestId, string campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// 转化记录事件
/// </summary>
public class ConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "ConversionRecorded";

    public string DeliveryRecordId { get; }
    public string RequestId { get; }
    public string CampaignId { get; }

    public ConversionRecordedEvent(string deliveryRecordId, string requestId, string campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// 投放状态更新事件
/// </summary>
public class DeliveryStatusUpdatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryStatusUpdated";

    public string DeliveryRecordId { get; }
    public DeliveryStatus Status { get; }

    public DeliveryStatusUpdatedEvent(string deliveryRecordId, DeliveryStatus status)
    {
        DeliveryRecordId = deliveryRecordId;
        Status = status;
    }
}

#endregion

#region Delivery Events


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