using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���ʵ�壨�ۺϸ���
/// </summary>
public class Advertisement : AggregateRoot
{
    /// <summary>
    /// �����ID
    /// </summary>
    public string AdvertiserId { get; private set; } = string.Empty;

    /// <summary>
    /// ���ID
    /// </summary>
    public string CampaignId { get; private set; } = string.Empty;

    /// <summary>
    /// �������
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// �������
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// ý������
    /// </summary>
    public MediaType MediaType { get; private set; }

    /// <summary>
    /// �����Ŀ�б�
    /// </summary>
    public IReadOnlyList<string> Categories { get; private set; } = new List<string>();

    /// <summary>
    /// ��������б�
    /// </summary>
    public IReadOnlyList<int> Attributes { get; private set; } = new List<int>();

    /// <summary>
    /// ����������б�
    /// </summary>
    public IReadOnlyList<string> AdvertiserDomains { get; private set; } = new List<string>();

    /// <summary>
    /// Ͷ�ſ�ʼʱ��
    /// </summary>
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// Ͷ�Ž���ʱ��
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// �����Ϣ
    /// </summary>
    public AuditInfo AuditInfo { get; private set; } = AuditInfo.CreatePending();

    /// <summary>
    /// �������
    /// </summary>
    public TargetingPolicy TargetingPolicy { get; private set; } = TargetingPolicy.CreateEmpty();

    /// <summary>
    /// Ͷ�Ų���
    /// </summary>
    public DeliveryPolicy DeliveryPolicy { get; private set; } = null!;

    /// <summary>
    /// ����ز���Ϣ
    /// </summary>
    public CreativeInfo CreativeInfo { get; private set; } = null!;

    /// <summary>
    /// ����ǩ
    /// </summary>
    public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

    /// <summary>
    /// �����÷�
    /// </summary>
    public decimal QualityScore { get; private set; } = DefaultValues.Advertisement.DefaultQualityScore;

    /// <summary>
    /// �Ƿ񼤻�
    /// </summary>
    public bool IsActive { get; private set; } = false;

    /// <summary>
    /// �ۼ�չʾ����
    /// </summary>
    public long TotalImpressions { get; private set; } = 0;

    /// <summary>
    /// �ۼƵ������
    /// </summary>
    public long TotalClicks { get; private set; } = 0;

    /// <summary>
    /// �ۼ����ѽ��֣�
    /// </summary>
    public decimal TotalSpent { get; private set; } = 0;

    /// <summary>
    /// ˽�й��캯��������ORM��
    /// </summary>
    private Advertisement() { }

    /// <summary>
    /// ���캯��
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

        // ������洴���¼�
        AddDomainEvent(new AdvertisementCreatedEvent(Id, advertiserId, campaignId, name));
    }

    /// <summary>
    /// ���¹�������Ϣ
    /// </summary>
    public void UpdateBasicInfo(string name, string description, IList<string>? tags = null)
    {
        ValidateName(name);

        Name = name;
        Description = description;
        Tags = tags?.ToList() ?? new List<string>();

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "������Ϣ"));
    }

    /// <summary>
    /// ����Ͷ��ʱ��
    /// </summary>
    public void UpdateDeliveryTime(DateTime? startTime, DateTime? endTime)
    {
        ValidateDeliveryTime(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "Ͷ��ʱ��"));
    }

    /// <summary>
    /// ���¶������
    /// </summary>
    public void UpdateTargetingPolicy(TargetingPolicy targetingPolicy)
    {
        ArgumentNullException.ThrowIfNull(targetingPolicy);

        TargetingPolicy = targetingPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "�������"));
    }

    /// <summary>
    /// ����Ͷ�Ų���
    /// </summary>
    public void UpdateDeliveryPolicy(DeliveryPolicy deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

        DeliveryPolicy = deliveryPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "Ͷ�Ų���"));
    }

    /// <summary>
    /// ���´�����Ϣ
    /// </summary>
    public void UpdateCreativeInfo(CreativeInfo creativeInfo)
    {
        ArgumentNullException.ThrowIfNull(creativeInfo);

        CreativeInfo = creativeInfo;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "������Ϣ"));

        // ������º���Ҫ�������
        if (AuditInfo.IsApproved)
        {
            SubmitForAudit();
        }
    }

    /// <summary>
    /// �ύ���
    /// </summary>
    public void SubmitForAudit()
    {
        if (AuditInfo.Status == AuditStatus.InProgress)
            throw new InvalidOperationException("�����������У��޷��ظ��ύ");

        AuditInfo = AuditInfo.CreatePending();
        IsActive = false; // �ύ���ʱ��ͣͶ��

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementSubmittedForAuditEvent(Id, AdvertiserId));
    }

    /// <summary>
    /// ��ʼ���
    /// </summary>
    public void StartAudit(string auditorId, string auditorName)
    {
        if (AuditInfo.Status != AuditStatus.Pending)
            throw new InvalidOperationException("ֻ�д����״̬�Ĺ����ܿ�ʼ���");

        AuditInfo = AuditInfo.CreateInProgress(auditorId, auditorName);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditStartedEvent(Id, auditorId));
    }

    /// <summary>
    /// ���ͨ��
    /// </summary>
    public void ApproveAudit(string auditorId, string auditorName, string? feedback = null)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("ֻ�������״̬�Ĺ��������ͨ��");

        AuditInfo = AuditInfo.CreateApproved(auditorId, auditorName, feedback);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditApprovedEvent(Id, auditorId));
    }

    /// <summary>
    /// ��˾ܾ�
    /// </summary>
    public void RejectAudit(string auditorId, string auditorName, string feedback)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("ֻ�������״̬�Ĺ�������˾ܾ�");

        if (string.IsNullOrWhiteSpace(feedback))
            throw new ArgumentException("��˾ܾ������ṩ������Ϣ");

        AuditInfo = AuditInfo.CreateRejected(feedback, auditorId, auditorName);
        IsActive = false; // ��˾ܾ�ʱֹͣͶ��

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditRejectedEvent(Id, auditorId, feedback));
    }

    /// <summary>
    /// ��Ҫ�޸�
    /// </summary>
    public void RequireChanges(string auditorId, string auditorName, string correctionSuggestion)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("ֻ�������״̬�Ĺ�����Ҫ���޸�");

        if (string.IsNullOrWhiteSpace(correctionSuggestion))
            throw new ArgumentException("Ҫ���޸ı����ṩ��������");

        AuditInfo = AuditInfo.CreateRequiresChanges(correctionSuggestion, auditorId, auditorName);
        IsActive = false; // ��Ҫ�޸�ʱֹͣͶ��

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementRequiresChangesEvent(Id, auditorId, correctionSuggestion));
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Activate()
    {
        if (!AuditInfo.CanDeliver)
            throw new InvalidOperationException("ֻ�����ͨ���Ĺ����ܼ���");

        if (IsExpired)
            throw new InvalidOperationException("�ѹ��ڵĹ���޷�����");

        IsActive = true;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementActivatedEvent(Id));
    }

    /// <summary>
    /// ��ͣ���
    /// </summary>
    public void Pause()
    {
        IsActive = false;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementPausedEvent(Id));
    }

    /// <summary>
    /// ��¼չʾ
    /// </summary>
    public void RecordImpression(decimal cost)
    {
        if (!CanDeliver)
            throw new InvalidOperationException("��浱ǰ״̬������Ͷ��");

        TotalImpressions++;
        TotalSpent += cost;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementImpressionRecordedEvent(Id, cost));
    }

    /// <summary>
    /// ��¼���
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
    /// ���������÷�
    /// </summary>
    public void UpdateQualityScore(decimal newScore)
    {
        if (newScore < 0 || newScore > 10)
            throw new ArgumentOutOfRangeException(nameof(newScore), "�����÷ֱ�����0-10֮��");

        var oldScore = QualityScore;
        QualityScore = newScore;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementQualityScoreUpdatedEvent(Id, oldScore, newScore));
    }

    /// <summary>
    /// �Ƿ����Ͷ��
    /// </summary>
    public bool CanDeliver => IsActive &&
                             AuditInfo.CanDeliver &&
                             !IsExpired &&
                             IsWithinDeliveryTime &&
                             !IsDeleted;

    /// <summary>
    /// �Ƿ��ѹ���
    /// </summary>
    public bool IsExpired => EndTime.HasValue && DateTime.UtcNow > EndTime.Value;

    /// <summary>
    /// �Ƿ���Ͷ��ʱ�䷶Χ��
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
    /// ��ȡ�����
    /// </summary>
    public decimal GetClickThroughRate()
    {
        return TotalImpressions > 0 ? (decimal)TotalClicks / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡƽ������չʾ�ɱ�
    /// </summary>
    public decimal GetAverageCostPerImpression()
    {
        return TotalImpressions > 0 ? TotalSpent / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡƽ�����ε���ɱ�
    /// </summary>
    public decimal GetAverageCostPerClick()
    {
        return TotalClicks > 0 ? TotalSpent / TotalClicks : 0m;
    }

    /// <summary>
    /// �������
    /// </summary>
    public decimal CalculateBid(decimal marketPrice = 0m)
    {
        return DeliveryPolicy.CalculateActualBid(QualityScore, marketPrice);
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string name, string advertiserId, string campaignId)
    {
        ValidateName(name);

        if (string.IsNullOrWhiteSpace(advertiserId))
            throw new ArgumentException("�����ID����Ϊ��", nameof(advertiserId));

        if (string.IsNullOrWhiteSpace(campaignId))
            throw new ArgumentException("���ID����Ϊ��", nameof(campaignId));
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("������Ʋ���Ϊ��", nameof(name));

        if (name.Length > ValidationConstants.StringLength.AdTitleMaxLength)
            throw new ArgumentException($"������Ƴ��Ȳ��ܳ���{ValidationConstants.StringLength.AdTitleMaxLength}���ַ�", nameof(name));
    }

    /// <summary>
    /// ��֤Ͷ��ʱ��
    /// </summary>
    private static void ValidateDeliveryTime(DateTime? startTime, DateTime? endTime)
    {
        if (startTime.HasValue && endTime.HasValue && startTime.Value >= endTime.Value)
            throw new ArgumentException("Ͷ�ſ�ʼʱ��������ڽ���ʱ��");
    }
}