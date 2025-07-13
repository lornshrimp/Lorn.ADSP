using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Aggregates;

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
    /// �������
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// �������
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// ���״̬
    /// </summary>
    public AdStatus Status { get; private set; } = AdStatus.Draft;

    /// <summary>
    /// ý������
    /// </summary>
    public MediaType MediaType { get; private set; }

    /// <summary>
    /// �������б�
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
    /// �����Ϣ
    /// </summary>
    public AuditInfo AuditInfo { get; private set; } = AuditInfo.CreatePending();

    /// <summary>
    /// ����������Ϣ
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
    /// �ۼƻ��ѣ��֣�
    /// </summary>
    public decimal TotalSpent { get; private set; } = 0;

    /// <summary>
    /// �������
    /// </summary>
    private readonly List<Campaign> _campaigns = new();
    public IReadOnlyList<Campaign> Campaigns => _campaigns.AsReadOnly();

    /// <summary>
    /// ˽�й��캯��������ORM
    /// </summary>
    private Advertisement() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public Advertisement(
        string advertiserId,
        string name,
        string description,
        MediaType mediaType,
        CreativeInfo creativeInfo,
        IList<string>? categories = null,
        IList<int>? attributes = null,
        IList<string>? advertiserDomains = null,
        IList<string>? tags = null)
    {
        ValidateInputs(name, advertiserId);

        AdvertiserId = advertiserId;
        Name = name;
        Description = description;
        MediaType = mediaType;
        CreativeInfo = creativeInfo;
        Categories = categories?.ToList() ?? new List<string>();
        Attributes = attributes?.ToList() ?? new List<int>();
        AdvertiserDomains = advertiserDomains?.ToList() ?? new List<string>();
        Tags = tags?.ToList() ?? new List<string>();

        // ������洴���¼�
        AddDomainEvent(new AdvertisementCreatedEvent(Id, advertiserId, string.Empty, name));
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
    /// ���´�����Ϣ
    /// </summary>
    public void UpdateCreativeInfo(CreativeInfo creativeInfo)
    {
        ArgumentNullException.ThrowIfNull(creativeInfo);

        CreativeInfo = creativeInfo;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "������Ϣ"));

        // ���´������Ҫ�������
        if (AuditInfo.IsApproved)
        {
            SubmitForAudit();
        }
    }

    /// <summary>
    /// ��ӻ
    /// </summary>
    public void AddCampaign(Campaign campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        if (campaign.AdvertisementId != Id)
            throw new ArgumentException("��������ID��ƥ��");

        _campaigns.Add(campaign);
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementUpdatedEvent(Id, "��ӻ"));
    }

    /// <summary>
    /// ��ȡ��Ծ�Ļ
    /// </summary>
    public IReadOnlyList<Campaign> GetActiveCampaigns()
    {
        return _campaigns.Where(c => c.IsActive).ToList().AsReadOnly();
    }

    /// <summary>
    /// �ύ���
    /// </summary>
    public void SubmitForAudit()
    {
        if (AuditInfo.Status == AuditStatus.InProgress)
            throw new InvalidOperationException("�����������У��޷��ظ��ύ");

        AuditInfo = AuditInfo.CreatePending();
        Status = AdStatus.PendingReview;
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
            throw new InvalidOperationException("ֻ�д����״̬�Ĺ���ܿ�ʼ���");

        AuditInfo = AuditInfo.CreateInProgress(auditorId, auditorName);
        Status = AdStatus.UnderReview;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditStartedEvent(Id, auditorId));
    }

    /// <summary>
    /// ���ͨ��
    /// </summary>
    public void ApproveAudit(string auditorId, string auditorName, string? feedback = null)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("ֻ�������״̬�Ĺ�������ͨ��");

        AuditInfo = AuditInfo.CreateApproved(auditorId, auditorName, feedback);
        Status = AdStatus.Approved;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAuditApprovedEvent(Id, auditorId));
    }

    /// <summary>
    /// ��˾ܾ�
    /// </summary>
    public void RejectAudit(string auditorId, string auditorName, string feedback)
    {
        if (AuditInfo.Status != AuditStatus.InProgress)
            throw new InvalidOperationException("ֻ�������״̬�Ĺ������˾ܾ�");

        if (string.IsNullOrWhiteSpace(feedback))
            throw new ArgumentException("��˾ܾ������ṩ������Ϣ");

        AuditInfo = AuditInfo.CreateRejected(feedback, auditorId, auditorName);
        Status = AdStatus.Rejected;
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
            throw new InvalidOperationException("ֻ�������״̬�Ĺ����Ҫ�޸�");

        if (string.IsNullOrWhiteSpace(correctionSuggestion))
            throw new ArgumentException("Ҫ���޸ı����ṩ������Ϣ");

        AuditInfo = AuditInfo.CreateRequiresChanges(correctionSuggestion, auditorId, auditorName);
        Status = AdStatus.Draft; // �ص��ݸ�״̬
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
            throw new InvalidOperationException("ֻ�����ͨ���Ĺ���ܼ���");

        IsActive = true;
        Status = AdStatus.Active;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementActivatedEvent(Id));
    }

    /// <summary>
    /// ��ͣ���
    /// </summary>
    public void Pause()
    {
        IsActive = false;
        Status = AdStatus.Paused;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementPausedEvent(Id));
    }

    /// <summary>
    /// ֹͣ���
    /// </summary>
    public void Stop()
    {
        IsActive = false;
        Status = AdStatus.Stopped;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementStoppedEvent(Id));
    }

    /// <summary>
    /// ��¼չʾ
    /// </summary>
    public void RecordImpression(decimal cost)
    {
        if (!CanDeliver)
            throw new InvalidOperationException("��ǰ״̬������Ͷ��");

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
                             Status == AdStatus.Active &&
                             !IsDeleted;

    /// <summary>
    /// ��ȡ�����
    /// </summary>
    public decimal GetClickThroughRate()
    {
        return TotalImpressions > 0 ? (decimal)TotalClicks / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡƽ��ÿ��չʾ�ɱ�
    /// </summary>
    public decimal GetAverageCostPerImpression()
    {
        return TotalImpressions > 0 ? TotalSpent / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡƽ��ÿ�ε���ɱ�
    /// </summary>
    public decimal GetAverageCostPerClick()
    {
        return TotalClicks > 0 ? TotalSpent / TotalClicks : 0m;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string name, string advertiserId)
    {
        ValidateName(name);

        if (string.IsNullOrWhiteSpace(advertiserId))
            throw new ArgumentException("�����ID����Ϊ��", nameof(advertiserId));
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
}