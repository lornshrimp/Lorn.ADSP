using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// �ʵ�壨�Ǿۺϸ���
/// </summary>
public class Campaign : EntityBase
{
    /// <summary>
    /// ���
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// �����
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// �״̬
    /// </summary>
    public CampaignStatus Status { get; private set; }

    /// <summary>
    /// �����ID
    /// </summary>
    public string AdvertisementId { get; private set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public TargetingConfig TargetingConfig { get; private set; } = null!;

    /// <summary>
    /// Ͷ�Ų���
    /// </summary>
    public DeliveryPolicy DeliveryPolicy { get; private set; } = null!;

    /// <summary>
    /// Ԥ����Ϣ
    /// </summary>
    public BudgetInfo Budget { get; private set; } = null!;

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime? StartDate { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// ���۲���
    /// </summary>
    public BiddingStrategy BiddingStrategy { get; private set; }

    /// <summary>
    /// ˽�й��캯��������ORM
    /// </summary>
    private Campaign() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public Campaign(
        string advertisementId,
        string name,
        string description,
        TargetingConfig targetingConfig,
        DeliveryPolicy deliveryPolicy,
        BudgetInfo budget,
        BiddingStrategy biddingStrategy = BiddingStrategy.AutoBid,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        ValidateInputs(advertisementId, name, targetingConfig, deliveryPolicy, budget);

        AdvertisementId = advertisementId;
        Name = name;
        Description = description;
        TargetingConfig = targetingConfig;
        DeliveryPolicy = deliveryPolicy;
        Budget = budget;
        BiddingStrategy = biddingStrategy;
        StartDate = startDate;
        EndDate = endDate;
        Status = CampaignStatus.Draft;
    }

    /// <summary>
    /// ��ʼ�
    /// </summary>
    public void Start()
    {
        if (Status != CampaignStatus.Draft && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("ֻ�вݸ����ͣ״̬�Ļ�ɿ�ʼ");

        if (!IsWithinScheduledTime())
            throw new InvalidOperationException("����Ԥ����ʱ�䷶Χ��");

        Status = CampaignStatus.Running;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ͣ�
    /// </summary>
    public void Pause()
    {
        if (Status != CampaignStatus.Running)
            throw new InvalidOperationException("ֻ�������еĻ����ͣ");

        Status = CampaignStatus.Paused;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �ָ��
    /// </summary>
    public void Resume()
    {
        if (Status != CampaignStatus.Paused)
            throw new InvalidOperationException("ֻ����ͣ�Ļ�ɻָ�");

        Status = CampaignStatus.Running;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ֹͣ�
    /// </summary>
    public void Stop()
    {
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Cancelled)
            throw new InvalidOperationException("�Ѿ���ɻ�ȡ��");

        Status = CampaignStatus.Cancelled;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ɻ
    /// </summary>
    public void Complete()
    {
        Status = CampaignStatus.Completed;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public void UpdateBudget(BudgetInfo budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        Budget = budget;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ���¶�������
    /// </summary>
    public void UpdateTargeting(TargetingConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        TargetingConfig = config;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �Ӳ���ģ�崴����������
    /// </summary>
    public TargetingConfig CreateTargetingFromPolicy(TargetingPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        var config = policy.CreateConfig(Id);
        TargetingConfig = config;
        UpdateLastModifiedTime();
        return config;
    }

    /// <summary>
    /// ���Ԥ�������
    /// </summary>
    public bool CheckBudgetAvailability()
    {
        if (Budget.IsExhausted())
        {
            // Ԥ���þ��Զ���ͣ�
            if (Status == CampaignStatus.Running)
            {
                Pause();
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// �Ƿ��Ծ
    /// </summary>
    public bool IsActive => Status == CampaignStatus.Running &&
                           CheckBudgetAvailability() &&
                           IsWithinScheduledTime() &&
                           !IsDeleted;

    public bool CanDeliver { get; internal set; }

    /// <summary>
    /// �Ƿ���Ԥ��ʱ�䷶Χ��
    /// </summary>
    public bool IsWithinScheduledTime()
    {
        var now = DateTime.UtcNow;

        if (StartDate.HasValue && now < StartDate.Value)
            return false;

        if (EndDate.HasValue && now > EndDate.Value)
        {
            // ��������ʱ��״̬Ϊ�����У��Զ����
            if (Status == CampaignStatus.Running)
            {
                Complete();
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string advertisementId, string name,
        TargetingConfig targetingConfig, DeliveryPolicy deliveryPolicy, BudgetInfo budget)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("���ID����Ϊ��", nameof(advertisementId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("�������Ϊ��", nameof(name));

        if (name.Length > ValidationConstants.StringLength.CampaignNameMaxLength)
            throw new ArgumentException($"������Ȳ��ܳ���{ValidationConstants.StringLength.CampaignNameMaxLength}���ַ�", nameof(name));

        ArgumentNullException.ThrowIfNull(targetingConfig, nameof(targetingConfig));
        ArgumentNullException.ThrowIfNull(deliveryPolicy, nameof(deliveryPolicy));
        ArgumentNullException.ThrowIfNull(budget, nameof(budget));
    }
}

