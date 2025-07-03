using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Events;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public string EventId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// 事件类型
    /// </summary>
    public abstract string EventType { get; }
}

#region Advertisement Events

/// <summary>
/// 广告创建事件
/// </summary>
public class AdvertisementCreatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementCreated";

    public string AdvertisementId { get; }
    public string AdvertiserId { get; }
    public string CampaignId { get; }
    public string AdvertisementName { get; }

    public AdvertisementCreatedEvent(string advertisementId, string advertiserId, string campaignId, string advertisementName)
    {
        AdvertisementId = advertisementId;
        AdvertiserId = advertiserId;
        CampaignId = campaignId;
        AdvertisementName = advertisementName;
    }
}

/// <summary>
/// 广告更新事件
/// </summary>
public class AdvertisementUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementUpdated";

    public string AdvertisementId { get; }
    public string UpdateType { get; }

    public AdvertisementUpdatedEvent(string advertisementId, string updateType)
    {
        AdvertisementId = advertisementId;
        UpdateType = updateType;
    }
}

/// <summary>
/// 广告提交审核事件
/// </summary>
public class AdvertisementSubmittedForAuditEvent : DomainEventBase
{
    public override string EventType => "AdvertisementSubmittedForAudit";

    public string AdvertisementId { get; }
    public string AdvertiserId { get; }

    public AdvertisementSubmittedForAuditEvent(string advertisementId, string advertiserId)
    {
        AdvertisementId = advertisementId;
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// 广告审核开始事件
/// </summary>
public class AdvertisementAuditStartedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditStarted";

    public string AdvertisementId { get; }
    public string AuditorId { get; }

    public AdvertisementAuditStartedEvent(string advertisementId, string auditorId)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
    }
}

/// <summary>
/// 广告审核通过事件
/// </summary>
public class AdvertisementAuditApprovedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditApproved";

    public string AdvertisementId { get; }
    public string AuditorId { get; }

    public AdvertisementAuditApprovedEvent(string advertisementId, string auditorId)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
    }
}

/// <summary>
/// 广告审核拒绝事件
/// </summary>
public class AdvertisementAuditRejectedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditRejected";

    public string AdvertisementId { get; }
    public string AuditorId { get; }
    public string Feedback { get; }

    public AdvertisementAuditRejectedEvent(string advertisementId, string auditorId, string feedback)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
        Feedback = feedback;
    }
}

/// <summary>
/// 广告需要修改事件
/// </summary>
public class AdvertisementRequiresChangesEvent : DomainEventBase
{
    public override string EventType => "AdvertisementRequiresChanges";

    public string AdvertisementId { get; }
    public string AuditorId { get; }
    public string CorrectionSuggestion { get; }

    public AdvertisementRequiresChangesEvent(string advertisementId, string auditorId, string correctionSuggestion)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
        CorrectionSuggestion = correctionSuggestion;
    }
}

/// <summary>
/// 广告激活事件
/// </summary>
public class AdvertisementActivatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementActivated";

    public string AdvertisementId { get; }

    public AdvertisementActivatedEvent(string advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// 广告暂停事件
/// </summary>
public class AdvertisementPausedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementPaused";

    public string AdvertisementId { get; }

    public AdvertisementPausedEvent(string advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// 广告展示记录事件
/// </summary>
public class AdvertisementImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementImpressionRecorded";

    public string AdvertisementId { get; }
    public decimal Cost { get; }

    public AdvertisementImpressionRecordedEvent(string advertisementId, decimal cost)
    {
        AdvertisementId = advertisementId;
        Cost = cost;
    }
}

/// <summary>
/// 广告点击记录事件
/// </summary>
public class AdvertisementClickRecordedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementClickRecorded";

    public string AdvertisementId { get; }
    public decimal Cost { get; }

    public AdvertisementClickRecordedEvent(string advertisementId, decimal cost)
    {
        AdvertisementId = advertisementId;
        Cost = cost;
    }
}

/// <summary>
/// 广告质量得分更新事件
/// </summary>
public class AdvertisementQualityScoreUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementQualityScoreUpdated";

    public string AdvertisementId { get; }
    public decimal OldScore { get; }
    public decimal NewScore { get; }

    public AdvertisementQualityScoreUpdatedEvent(string advertisementId, decimal oldScore, decimal newScore)
    {
        AdvertisementId = advertisementId;
        OldScore = oldScore;
        NewScore = newScore;
    }
}

#endregion

#region Campaign Events

/// <summary>
/// 活动创建事件
/// </summary>
public class CampaignCreatedEvent : DomainEventBase
{
    public override string EventType => "CampaignCreated";

    public string CampaignId { get; }
    public string AdvertiserId { get; }
    public string CampaignName { get; }

    public CampaignCreatedEvent(string campaignId, string advertiserId, string campaignName)
    {
        CampaignId = campaignId;
        AdvertiserId = advertiserId;
        CampaignName = campaignName;
    }
}

/// <summary>
/// 活动更新事件
/// </summary>
public class CampaignUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignUpdated";

    public string CampaignId { get; }
    public string UpdateType { get; }

    public CampaignUpdatedEvent(string campaignId, string updateType)
    {
        CampaignId = campaignId;
        UpdateType = updateType;
    }
}

/// <summary>
/// 活动预算更新事件
/// </summary>
public class CampaignBudgetUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignBudgetUpdated";

    public string CampaignId { get; }
    public decimal TotalBudget { get; }
    public decimal DailyBudget { get; }

    public CampaignBudgetUpdatedEvent(string campaignId, decimal totalBudget, decimal dailyBudget)
    {
        CampaignId = campaignId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
    }
}

/// <summary>
/// 活动提交审核事件
/// </summary>
public class CampaignSubmittedForReviewEvent : DomainEventBase
{
    public override string EventType => "CampaignSubmittedForReview";

    public string CampaignId { get; }
    public string AdvertiserId { get; }

    public CampaignSubmittedForReviewEvent(string campaignId, string advertiserId)
    {
        CampaignId = campaignId;
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// 活动激活事件
/// </summary>
public class CampaignActivatedEvent : DomainEventBase
{
    public override string EventType => "CampaignActivated";

    public string CampaignId { get; }

    public CampaignActivatedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动暂停事件
/// </summary>
public class CampaignPausedEvent : DomainEventBase
{
    public override string EventType => "CampaignPaused";

    public string CampaignId { get; }

    public CampaignPausedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动结束事件
/// </summary>
public class CampaignEndedEvent : DomainEventBase
{
    public override string EventType => "CampaignEnded";

    public string CampaignId { get; }

    public CampaignEndedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动删除事件
/// </summary>
public class CampaignDeletedEvent : DomainEventBase
{
    public override string EventType => "CampaignDeleted";

    public string CampaignId { get; }

    public CampaignDeletedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 广告添加到活动事件
/// </summary>
public class AdvertisementAddedToCampaignEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAddedToCampaign";

    public string CampaignId { get; }
    public string AdvertisementId { get; }

    public AdvertisementAddedToCampaignEvent(string campaignId, string advertisementId)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// 广告从活动移除事件
/// </summary>
public class AdvertisementRemovedFromCampaignEvent : DomainEventBase
{
    public override string EventType => "AdvertisementRemovedFromCampaign";

    public string CampaignId { get; }
    public string AdvertisementId { get; }

    public AdvertisementRemovedFromCampaignEvent(string campaignId, string advertisementId)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// 活动展示记录事件
/// </summary>
public class CampaignImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignImpressionRecorded";

    public string CampaignId { get; }
    public decimal Cost { get; }

    public CampaignImpressionRecordedEvent(string campaignId, decimal cost)
    {
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// 活动点击记录事件
/// </summary>
public class CampaignClickRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignClickRecorded";

    public string CampaignId { get; }
    public decimal Cost { get; }

    public CampaignClickRecordedEvent(string campaignId, decimal cost)
    {
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// 活动转化记录事件
/// </summary>
public class CampaignConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignConversionRecorded";

    public string CampaignId { get; }

    public CampaignConversionRecordedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// 活动预算耗尽事件
/// </summary>
public class CampaignBudgetExhaustedEvent : DomainEventBase
{
    public override string EventType => "CampaignBudgetExhausted";

    public string CampaignId { get; }
    public decimal SpentBudget { get; }

    public CampaignBudgetExhaustedEvent(string campaignId, decimal spentBudget)
    {
        CampaignId = campaignId;
        SpentBudget = spentBudget;
    }
}

#endregion