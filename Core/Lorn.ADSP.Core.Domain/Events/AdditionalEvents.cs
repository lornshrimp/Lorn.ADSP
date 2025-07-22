using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Events;

// ����ȱʧ���¼�

/// <summary>
/// ���ֹͣ�¼�
/// </summary>
public class AdvertisementStoppedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementStopped";

    public Guid AdvertisementId { get; }

    public AdvertisementStoppedEvent(Guid advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// ���ʼ�¼�
/// </summary>
public class CampaignStartedEvent : DomainEventBase
{
    public override string EventType => "CampaignStarted";

    public Guid CampaignId { get; }
    public Guid AdvertisementId { get; }
    public decimal Budget { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public CampaignStartedEvent(Guid campaignId, Guid advertisementId, decimal budget, DateTime? startDate, DateTime? endDate)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
        Budget = budget;
        StartDate = startDate;
        EndDate = endDate;
    }
}

/// <summary>
/// ��ָ��¼�
/// </summary>
public class CampaignResumedEvent : DomainEventBase
{
    public override string EventType => "CampaignResumed";

    public Guid CampaignId { get; }

    public CampaignResumedEvent(Guid campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// �ֹͣ�¼�
/// </summary>
public class CampaignStoppedEvent : DomainEventBase
{
    public override string EventType => "CampaignStopped";

    public Guid CampaignId { get; }

    public CampaignStoppedEvent(Guid campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// �����¼�
/// </summary>
public class CampaignCompletedEvent : DomainEventBase
{
    public override string EventType => "CampaignCompleted";

    public Guid CampaignId { get; }

    public CampaignCompletedEvent(Guid campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// ���������¼�
/// </summary>
public class CampaignTargetingUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignTargetingUpdated";

    public Guid CampaignId { get; }

    public CampaignTargetingUpdatedEvent(Guid campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// Ԥ��ľ��¼�
/// </summary>
public class BudgetExhaustedEvent : DomainEventBase
{
    public override string EventType => "BudgetExhausted";

    public Guid CampaignId { get; }
    public decimal BudgetLimit { get; }
    public decimal SpentAmount { get; }
    public DateTime ExhaustedAt { get; }

    public BudgetExhaustedEvent(Guid campaignId, decimal budgetLimit, decimal spentAmount, DateTime exhaustedAt)
    {
        CampaignId = campaignId;
        BudgetLimit = budgetLimit;
        SpentAmount = spentAmount;
        ExhaustedAt = exhaustedAt;
    }
}

/// <summary>
/// Ͷ�ż�¼�¼�
/// </summary>
public class DeliveryRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryRecorded";

    public Guid CampaignId { get; }
    public Guid DeliveryRecordId { get; }
    public decimal Cost { get; }

    public DeliveryRecordedEvent(Guid campaignId, Guid deliveryRecordId, decimal cost)
    {
        CampaignId = campaignId;
        DeliveryRecordId = deliveryRecordId;
        Cost = cost;
    }
}

#region Advertiser Events

/// <summary>
/// �����ע���¼�
/// </summary>
public class AdvertiserRegisteredEvent : DomainEventBase
{
    public override string EventType => "AdvertiserRegistered";

    public Guid AdvertiserId { get; }
    public string CompanyName { get; }
    public string Email { get; }

    public AdvertiserRegisteredEvent(Guid advertiserId, string companyName, string email)
    {
        AdvertiserId = advertiserId;
        CompanyName = companyName;
        Email = email;
    }
}

/// <summary>
/// ����������ύ�¼�
/// </summary>
public class AdvertiserQualificationSubmittedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserQualificationSubmitted";

    public Guid AdvertiserId { get; }
    public string BusinessLicense { get; }

    public AdvertiserQualificationSubmittedEvent(Guid advertiserId, string businessLicense)
    {
        AdvertiserId = advertiserId;
        BusinessLicense = businessLicense;
    }
}

/// <summary>
/// ����������¼�
/// </summary>
public class AdvertiserActivatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserActivated";

    public Guid AdvertiserId { get; }

    public AdvertiserActivatedEvent(Guid advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// �������ͣ�¼�
/// </summary>
public class AdvertiserSuspendedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserSuspended";

    public Guid AdvertiserId { get; }
    public string Reason { get; }

    public AdvertiserSuspendedEvent(Guid advertiserId, string reason)
    {
        AdvertiserId = advertiserId;
        Reason = reason;
    }
}

/// <summary>
/// ������ܾ��¼�
/// </summary>
public class AdvertiserRejectedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserRejected";

    public Guid AdvertiserId { get; }
    public string Reason { get; }

    public AdvertiserRejectedEvent(Guid advertiserId, string reason)
    {
        AdvertiserId = advertiserId;
        Reason = reason;
    }
}

/// <summary>
/// ������ָ��¼�
/// </summary>
public class AdvertiserResumedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserResumed";

    public Guid AdvertiserId { get; }

    public AdvertiserResumedEvent(Guid advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// ������˵������¼�
/// </summary>
public class AdvertiserBillingUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserBillingUpdated";

    public Guid AdvertiserId { get; }

    public AdvertiserBillingUpdatedEvent(Guid advertiserId)
    {
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// �������ϵ��Ϣ�����¼�
/// </summary>
public class AdvertiserContactUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserContactUpdated";

    public Guid AdvertiserId { get; }
    public string Email { get; }

    public AdvertiserContactUpdatedEvent(Guid advertiserId, string email)
    {
        AdvertiserId = advertiserId;
        Email = email;
    }
}

/// <summary>
/// ��������ӹ���¼�
/// </summary>
public class AdvertiserAdvertisementAddedEvent : DomainEventBase
{
    public override string EventType => "AdvertiserAdvertisementAdded";

    public Guid AdvertiserId { get; }
    public Guid AdvertisementId { get; }

    public AdvertiserAdvertisementAddedEvent(Guid advertiserId, Guid advertisementId)
    {
        AdvertiserId = advertiserId;
        AdvertisementId = advertisementId;
    }
}

#endregion

#region MediaResource Events

/// <summary>
/// ý����Դ�����¼�
/// </summary>
public class MediaResourceCreatedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceCreated";

    public Guid MediaResourceId { get; }
    public string Name { get; }
    public MediaType Type { get; }
    public Guid PublisherId { get; }

    public MediaResourceCreatedEvent(Guid mediaResourceId, string name, MediaType type, Guid publisherId)
    {
        MediaResourceId = mediaResourceId;
        Name = name;
        Type = type;
        PublisherId = publisherId;
    }
}

/// <summary>
/// ý����Դ�����¼�
/// </summary>
public class MediaResourceConfiguredEvent : DomainEventBase
{
    public override string EventType => "MediaResourceConfigured";

    public Guid MediaResourceId { get; }

    public MediaResourceConfiguredEvent(Guid mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// ý����Դ���������¼�
/// </summary>
public class MediaResourceTrafficUpdatedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceTrafficUpdated";

    public Guid MediaResourceId { get; }

    public MediaResourceTrafficUpdatedEvent(Guid mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// ý����Դ�����¼�
/// </summary>
public class MediaResourceEnabledEvent : DomainEventBase
{
    public override string EventType => "MediaResourceEnabled";

    public Guid MediaResourceId { get; }

    public MediaResourceEnabledEvent(Guid mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// ý����Դ�����¼�
/// </summary>
public class MediaResourceDisabledEvent : DomainEventBase
{
    public override string EventType => "MediaResourceDisabled";

    public Guid MediaResourceId { get; }

    public MediaResourceDisabledEvent(Guid mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// ý����Դ���ͨ���¼�
/// </summary>
public class MediaResourceApprovedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceApproved";

    public Guid MediaResourceId { get; }

    public MediaResourceApprovedEvent(Guid mediaResourceId)
    {
        MediaResourceId = mediaResourceId;
    }
}

/// <summary>
/// ý����Դ��˾ܾ��¼�
/// </summary>
public class MediaResourceRejectedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceRejected";

    public Guid MediaResourceId { get; }
    public string Reason { get; }

    public MediaResourceRejectedEvent(Guid mediaResourceId, string reason)
    {
        MediaResourceId = mediaResourceId;
        Reason = reason;
    }
}

/// <summary>
/// ý����Դ���ӹ��λ�¼�
/// </summary>
public class MediaResourcePlacementAddedEvent : DomainEventBase
{
    public override string EventType => "MediaResourcePlacementAdded";

    public Guid MediaResourceId { get; }
    public Guid PlacementId { get; }

    public MediaResourcePlacementAddedEvent(Guid mediaResourceId, Guid placementId)
    {
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
    }
}

/// <summary>
/// ý����Դ�Ƴ����λ�¼�
/// </summary>
public class MediaResourcePlacementRemovedEvent : DomainEventBase
{
    public override string EventType => "MediaResourcePlacementRemoved";

    public Guid MediaResourceId { get; }
    public Guid PlacementId { get; }

    public MediaResourcePlacementRemovedEvent(Guid mediaResourceId, Guid placementId)
    {
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
    }
}

/// <summary>
/// ý����ԴͶ�ż�¼�¼�
/// </summary>
public class MediaResourceDeliveryRecordedEvent : DomainEventBase
{
    public override string EventType => "MediaResourceDeliveryRecorded";

    public Guid MediaResourceId { get; }
    public Guid DeliveryRecordId { get; }

    public MediaResourceDeliveryRecordedEvent(Guid mediaResourceId, Guid deliveryRecordId)
    {
        MediaResourceId = mediaResourceId;
        DeliveryRecordId = deliveryRecordId;
    }
}

#endregion

#region DeliveryRecord Events

/// <summary>
/// Ͷ�ż�¼�����¼�
/// </summary>
public class DeliveryRecordCreatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryRecordCreated";

    public Guid DeliveryRecordId { get; }
    public Guid RequestId { get; }
    public Guid CampaignId { get; }
    public decimal Cost { get; }

    public DeliveryRecordCreatedEvent(Guid deliveryRecordId, Guid requestId, Guid campaignId, decimal cost)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// չʾ��¼�¼�
/// </summary>
public class ImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "ImpressionRecorded";

    public Guid DeliveryRecordId { get; }
    public Guid RequestId { get; }
    public Guid CampaignId { get; }

    public ImpressionRecordedEvent(Guid deliveryRecordId, Guid requestId, Guid campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// �����¼�¼�
/// </summary>
public class ClickRecordedEvent : DomainEventBase
{
    public override string EventType => "ClickRecorded";

    public Guid DeliveryRecordId { get; }
    public Guid RequestId { get; }
    public Guid CampaignId { get; }

    public ClickRecordedEvent(Guid deliveryRecordId, Guid requestId, Guid campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// ת����¼�¼�
/// </summary>
public class ConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "ConversionRecorded";

    public Guid DeliveryRecordId { get; }
    public Guid RequestId { get; }
    public Guid CampaignId { get; }

    public ConversionRecordedEvent(Guid deliveryRecordId, Guid requestId, Guid campaignId)
    {
        DeliveryRecordId = deliveryRecordId;
        RequestId = requestId;
        CampaignId = campaignId;
    }
}

/// <summary>
/// Ͷ��״̬�����¼�
/// </summary>
public class DeliveryStatusUpdatedEvent : DomainEventBase
{
    public override string EventType => "DeliveryStatusUpdated";

    public Guid DeliveryRecordId { get; }
    public DeliveryStatus Status { get; }

    public DeliveryStatusUpdatedEvent(Guid deliveryRecordId, DeliveryStatus status)
    {
        DeliveryRecordId = deliveryRecordId;
        Status = status;
    }
}

#endregion

#region Delivery Events


/// <summary>
/// Ͷ�ŵ����¼�¼�
/// </summary>
public class DeliveryClickRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryClickRecorded";

    public Guid DeliveryRecordId { get; }
    public Guid AdId { get; }
    public decimal ClickCost { get; }

    public DeliveryClickRecordedEvent(Guid deliveryRecordId, Guid adId, decimal clickCost)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        ClickCost = clickCost;
    }
}

/// <summary>
/// Ͷ��ת����¼�¼�
/// </summary>
public class DeliveryConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "DeliveryConversionRecorded";

    public Guid DeliveryRecordId { get; }
    public Guid AdId { get; }
    public decimal Revenue { get; }

    public DeliveryConversionRecordedEvent(Guid deliveryRecordId, Guid adId, decimal revenue)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        Revenue = revenue;
    }
}

/// <summary>
/// Ͷ��ʧ���¼�
/// </summary>
public class DeliveryFailedEvent : DomainEventBase
{
    public override string EventType => "DeliveryFailed";

    public Guid DeliveryRecordId { get; }
    public Guid AdId { get; }
    public string Reason { get; }

    public DeliveryFailedEvent(Guid deliveryRecordId, Guid adId, string reason)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
        Reason = reason;
    }
}

/// <summary>
/// Ͷ�ų�ʱ�¼�
/// </summary>
public class DeliveryTimeoutEvent : DomainEventBase
{
    public override string EventType => "DeliveryTimeout";

    public Guid DeliveryRecordId { get; }
    public Guid AdId { get; }

    public DeliveryTimeoutEvent(Guid deliveryRecordId, Guid adId)
    {
        DeliveryRecordId = deliveryRecordId;
        AdId = adId;
    }
}

#endregion
