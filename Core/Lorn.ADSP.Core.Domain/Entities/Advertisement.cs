using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告实体（聚合根）
/// </summary>
public class Advertisement : AggregateRoot
{
    /// <summary>
    /// 广告主ID
    /// </summary>
    public string AdvertiserId { get; private set; } = string.Empty;

    /// <summary>
    /// 广告活动ID
    /// </summary>
    public string CampaignId { get; private set; } = string.Empty;

    /// <summary>
    /// 广告名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 广告描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体类型
    /// </summary>
    public MediaType MediaType { get; private set; }

    /// <summary>
    /// 广告类目列表
    /// </summary>
    public IReadOnlyList<string> Categories { get; private set; } = new List<string>();

    /// <summary>
    /// 广告属性列表
    /// </summary>
    public IReadOnlyList<int> Attributes { get; private set; } = new List<int>();

    /// <summary>
    /// 广告主域名列表
    /// </summary>
    public IReadOnlyList<string> AdvertiserDomains { get; private set; } = new List<string>();

    /// <summary>
    /// 投放开始时间
    /// </summary>
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// 投放结束时间
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// 审核信息
    /// </summary>
    public AuditInfo AuditInfo { get; private set; } = AuditInfo.CreatePending();

    /// <summary>
    /// 定向策略
    /// </summary>
    public TargetingPolicy TargetingPolicy { get; private set; } = TargetingPolicy.CreateEmpty();

    /// <summary>
    /// 投放策略
    /// </summary>
    public DeliveryPolicy DeliveryPolicy { get; private set; } = null!;

    /// <summary>
    /// 广告素材信息
    /// </summary>
    public CreativeInfo CreativeInfo { get; private set; } = null!;

    /// <summary>
    /// 广告标签
    /// </summary>
    public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

    /// <summary>
    /// 质量得分
    /// </summary>
    public decimal QualityScore { get; private set; } = DefaultValues.Advertisement.DefaultQualityScore;

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; private set; } = false;

    /// <summary>
    /// 累计展示次数
    /// </summary>
    public long TotalImpressions { get; private set; } = 0;

    /// <summary>
    /// 累计点击次数
    /// </summary>
    public long TotalClicks { get; private set; } = 0;

    /// <summary>
    /// 累计消费金额（分）
    /// </summary>
    public decimal TotalSpent { get; private set; } = 0;

    /// <summary>
    /// 私有构造函数（用于ORM）
    /// </summary>
    private Advertisement() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Advertisement(
        string advertiserId,
        string campaignId,
        string name,
        string description,
        MediaType mediaType,
        DeliveryPolicy deliveryPolicy,
        CreativeInfo creativeInfo,
        TargetingPolicy? targetingPolicy = null,
        IList<string>? categories = null,
        IList<int>? attributes = null,
        IList<string>? advertiserDomains = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        IList<string>? tags = null)
    {
        ValidateInputs(name, advertiserId, campaignId);

        AdvertiserId = advertiserId;
        CampaignId = campaignId;
        Name = name;
        Description = description;
        MediaType = mediaType;
        DeliveryPolicy = deliveryPolicy;
        CreativeInfo = creativeInfo;
        TargetingPolicy = targetingPolicy ?? TargetingPolicy.CreateEmpty();
        Categories = categories?.ToList() ?? new List<string>();
        Attributes = attributes?.ToList() ?? new List<int>();
        AdvertiserDomains = advertiserDomains?.ToList() ?? new List<string>();
        StartTime = startTime;
        EndTime = endTime;
        Tags = tags?.ToList() ?? new List<string>();

        // 发布广告创建事件
        AddDomainEvent(new AdvertisementCreatedEvent(Id, advertiserId, campaignId, name));
    }

    /// <summary>
    /// 更新广告基本信息
    /// </summary>
    public void UpdateBasicInfo(string name, string description, IList<string>? tags = null)
    {
        ValidateName(name);

        Name = name;
        Description = description;
        Tags = tags?.ToList() ?? new List<string>();

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "基本信息"));
    }

    /// <summary>
    /// 更新投放时间
    /// </summary>
    public void UpdateDeliveryTime(DateTime? startTime, DateTime? endTime)
    {
        ValidateDeliveryTime(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "投放时间"));
    }

    /// <summary>
    /// 更新定向策略
    /// </summary>
    public void UpdateTargetingPolicy(TargetingPolicy targetingPolicy)
    {
        ArgumentNullException.ThrowIfNull(targetingPolicy);

        TargetingPolicy = targetingPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "定向策略"));
    }

    /// <summary>
    /// 更新投放策略
    /// </summary>
    public void UpdateDeliveryPolicy(DeliveryPolicy deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

        DeliveryPolicy = deliveryPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "投放策略"));
    }

    /// <summary>
    /// 更新创意信息
    /// </summary>
    public void UpdateCreativeInfo(CreativeInfo creativeInfo)
    {
        ArgumentNullException.ThrowIfNull(creativeInfo);

        CreativeInfo = creativeInfo;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "创意信息"));

        // 创意更新后需要重新审核
        if (AuditInfo.IsApproved)
        {
            SubmitForAudit();
        }
    }

    /// <summary>
    /// 提交审核
    /// </summary>
    public void SubmitForAudit()
    {
        if (AuditInfo.Status == AuditStatus.InProgress)
            throw new InvalidOperationException("广告正在审核中，无法重复提交");

        AuditInfo = AuditInfo.CreatePending();
        IsActive = false; // 提交审核时暂停投放

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementSubmittedForAuditEvent(Id, AdvertiserId));
    }

    /// <summary>
    /// 开始审核
    /// </summary>
    public void StartAudit(string auditorId, string auditorName)
    {
        if (AuditInfo.Status != AuditStatus.Pending)
            throw new InvalidOperationException("只有待审核状态的广告才能开始审核");

        AuditInfo = AuditInfo.CreateInProgress(auditorId, auditorName);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditStartedEvent(Id, auditorId));
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public void ApproveAudit(string auditorId, string auditorName, string? feedback = null)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("只有审核中状态的广告才能审核通过");

        AuditInfo = AuditInfo.CreateApproved(auditorId, auditorName, feedback);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditApprovedEvent(Id, auditorId));
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public void RejectAudit(string auditorId, string auditorName, string feedback)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("只有审核中状态的广告才能审核拒绝");

        if (string.IsNullOrWhiteSpace(feedback))
            throw new ArgumentException("审核拒绝必须提供反馈信息");

        AuditInfo = AuditInfo.CreateRejected(feedback, auditorId, auditorName);
        IsActive = false; // 审核拒绝时停止投放

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditRejectedEvent(Id, auditorId, feedback));
    }

    /// <summary>
    /// 需要修改
    /// </summary>
    public void RequireChanges(string auditorId, string auditorName, string correctionSuggestion)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("只有审核中状态的广告才能要求修改");

        if (string.IsNullOrWhiteSpace(correctionSuggestion))
            throw new ArgumentException("要求修改必须提供修正建议");

        AuditInfo = AuditInfo.CreateRequiresChanges(correctionSuggestion, auditorId, auditorName);
        IsActive = false; // 需要修改时停止投放

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementRequiresChangesEvent(Id, auditorId, correctionSuggestion));
    }

    /// <summary>
    /// 激活广告
    /// </summary>
    public void Activate()
    {
        if (!AuditInfo.CanDeliver)
            throw new InvalidOperationException("只有审核通过的广告才能激活");

        if (IsExpired)
            throw new InvalidOperationException("已过期的广告无法激活");

        IsActive = true;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementActivatedEvent(Id));
    }

    /// <summary>
    /// 暂停广告
    /// </summary>
    public void Pause()
    {
        IsActive = false;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementPausedEvent(Id));
    }

    /// <summary>
    /// 记录展示
    /// </summary>
    public void RecordImpression(decimal cost)
    {
        if (!CanDeliver)
            throw new InvalidOperationException("广告当前状态不允许投放");

        TotalImpressions++;
        TotalSpent += cost;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementImpressionRecordedEvent(Id, cost));
    }

    /// <summary>
    /// 记录点击
    /// </summary>
    public void RecordClick(decimal cost)
    {
        TotalClicks++;
        if (cost > 0)
        {
            TotalSpent += cost;
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementClickRecordedEvent(Id, cost));
    }

    /// <summary>
    /// 更新质量得分
    /// </summary>
    public void UpdateQualityScore(decimal newScore)
    {
        if (newScore < 0 || newScore > 10)
            throw new ArgumentOutOfRangeException(nameof(newScore), "质量得分必须在0-10之间");

        var oldScore = QualityScore;
        QualityScore = newScore;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementQualityScoreUpdatedEvent(Id, oldScore, newScore));
    }

    /// <summary>
    /// 是否可以投放
    /// </summary>
    public bool CanDeliver => IsActive &&
                             AuditInfo.CanDeliver &&
                             !IsExpired &&
                             IsWithinDeliveryTime &&
                             !IsDeleted;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => EndTime.HasValue && DateTime.UtcNow > EndTime.Value;

    /// <summary>
    /// 是否在投放时间范围内
    /// </summary>
    public bool IsWithinDeliveryTime
    {
        get
        {
            var now = DateTime.UtcNow;
            if (StartTime.HasValue && now < StartTime.Value)
                return false;
            if (EndTime.HasValue && now > EndTime.Value)
                return false;
            return true;
        }
    }

    /// <summary>
    /// 获取点击率
    /// </summary>
    public decimal GetClickThroughRate()
    {
        return TotalImpressions > 0 ? (decimal)TotalClicks / TotalImpressions : 0m;
    }

    /// <summary>
    /// 获取平均单次展示成本
    /// </summary>
    public decimal GetAverageCostPerImpression()
    {
        return TotalImpressions > 0 ? TotalSpent / TotalImpressions : 0m;
    }

    /// <summary>
    /// 获取平均单次点击成本
    /// </summary>
    public decimal GetAverageCostPerClick()
    {
        return TotalClicks > 0 ? TotalSpent / TotalClicks : 0m;
    }

    /// <summary>
    /// 计算出价
    /// </summary>
    public decimal CalculateBid(decimal marketPrice = 0m)
    {
        return DeliveryPolicy.CalculateActualBid(QualityScore, marketPrice);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string name, string advertiserId, string campaignId)
    {
        ValidateName(name);

        if (string.IsNullOrWhiteSpace(advertiserId))
            throw new ArgumentException("广告主ID不能为空", nameof(advertiserId));

        if (string.IsNullOrWhiteSpace(campaignId))
            throw new ArgumentException("广告活动ID不能为空", nameof(campaignId));
    }

    /// <summary>
    /// 验证广告名称
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("广告名称不能为空", nameof(name));

        if (name.Length > ValidationConstants.StringLength.AdTitleMaxLength)
            throw new ArgumentException($"广告名称长度不能超过{ValidationConstants.StringLength.AdTitleMaxLength}个字符", nameof(name));
    }

    /// <summary>
    /// 验证投放时间
    /// </summary>
    private static void ValidateDeliveryTime(DateTime? startTime, DateTime? endTime)
    {
        if (startTime.HasValue && endTime.HasValue && startTime.Value >= endTime.Value)
            throw new ArgumentException("投放开始时间必须早于结束时间");
    }
}