using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Shared.Constants;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���ʵ�壨�ۺϸ���
/// </summary>
public class Campaign : AggregateRoot
{
    /// <summary>
    /// �������
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// �������
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// �����ID
    /// </summary>
    public string AdvertiserId { get; private set; } = string.Empty;

    /// <summary>
    /// �״̬
    /// </summary>
    public CampaignStatus Status { get; private set; } = CampaignStatus.Draft;

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// ������Ԥ�㣨�֣�
    /// </summary>
    public decimal SpentBudget { get; private set; } = 0;

    /// <summary>
    /// ���ʼ����
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// ���������
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// ʱ��
    /// </summary>
    public string TimeZone { get; private set; } = "UTC";

    /// <summary>
    /// Ĭ�϶������
    /// </summary>
    public TargetingPolicy DefaultTargeting { get; private set; } = TargetingPolicy.CreateEmpty();

    /// <summary>
    /// Ĭ��Ͷ�Ų���
    /// </summary>
    public DeliveryPolicy DefaultDelivery { get; private set; } = null!;

    /// <summary>
    /// ���ǩ
    /// </summary>
    public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

    /// <summary>
    /// �Ŀ��
    /// </summary>
    public CampaignObjective Objective { get; private set; } = null!;

    /// <summary>
    /// �ۼ�չʾ����
    /// </summary>
    public long TotalImpressions { get; private set; } = 0;

    /// <summary>
    /// �ۼƵ������
    /// </summary>
    public long TotalClicks { get; private set; } = 0;

    /// <summary>
    /// �ۼ�ת������
    /// </summary>
    public long TotalConversions { get; private set; } = 0;

    /// <summary>
    /// �����Ĺ���б�
    /// </summary>
    private readonly List<string> _advertisementIds = new();
    public IReadOnlyList<string> AdvertisementIds => _advertisementIds.AsReadOnly();

    /// <summary>
    /// ˽�й��캯��������ORM��
    /// </summary>
    private Campaign() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public Campaign(
        string name,
        string description,
        string advertiserId,
        decimal totalBudget,
        decimal dailyBudget,
        DateTime startDate,
        DateTime endDate,
        DeliveryPolicy defaultDelivery,
        CampaignObjective objective,
        string timeZone = "UTC",
        TargetingPolicy? defaultTargeting = null,
        IList<string>? tags = null)
    {
        ValidateInputs(name, advertiserId, totalBudget, dailyBudget, startDate, endDate);

        Name = name;
        Description = description;
        AdvertiserId = advertiserId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        StartDate = startDate;
        EndDate = endDate;
        DefaultDelivery = defaultDelivery;
        Objective = objective;
        TimeZone = timeZone;
        DefaultTargeting = defaultTargeting ?? TargetingPolicy.CreateEmpty();
        Tags = tags?.ToList() ?? new List<string>();

        // ����������¼�
        AddDomainEvent(new CampaignCreatedEvent(Id, advertiserId, name));
    }

    /// <summary>
    /// ���»�����Ϣ
    /// </summary>
    public void UpdateBasicInfo(string name, string description, IList<string>? tags = null)
    {
        if (Status == CampaignStatus.Deleted)
            throw new InvalidOperationException("��ɾ���Ļ�޷��޸�");

        ValidateName(name);

        Name = name;
        Description = description;
        Tags = tags?.ToList() ?? new List<string>();

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "������Ϣ"));
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public void UpdateBudget(decimal totalBudget, decimal dailyBudget)
    {
        if (Status == CampaignStatus.Deleted)
            throw new InvalidOperationException("��ɾ���Ļ�޷��޸�Ԥ��");

        ValidateBudget(totalBudget, dailyBudget);

        // ���ܽ�Ԥ����������������ѽ��
        if (totalBudget < SpentBudget)
            throw new InvalidOperationException("��Ԥ�㲻�ܵ��������ѽ��");

        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignBudgetUpdatedEvent(Id, totalBudget, dailyBudget));
    }

    /// <summary>
    /// ����Ͷ��ʱ��
    /// </summary>
    public void UpdateDeliveryTime(DateTime startDate, DateTime endDate)
    {
        if (Status == CampaignStatus.Active)
            throw new InvalidOperationException("��Ծ״̬�Ļ�޷��޸�Ͷ��ʱ��");

        ValidateDeliveryTime(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "Ͷ��ʱ��"));
    }

    /// <summary>
    /// ����Ĭ�϶������
    /// </summary>
    public void UpdateDefaultTargeting(TargetingPolicy targetingPolicy)
    {
        ArgumentNullException.ThrowIfNull(targetingPolicy);

        DefaultTargeting = targetingPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "Ĭ�϶������"));
    }

    /// <summary>
    /// ����Ĭ��Ͷ�Ų���
    /// </summary>
    public void UpdateDefaultDelivery(DeliveryPolicy deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

        DefaultDelivery = deliveryPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "Ĭ��Ͷ�Ų���"));
    }

    /// <summary>
    /// ���»Ŀ��
    /// </summary>
    public void UpdateObjective(CampaignObjective objective)
    {
        ArgumentNullException.ThrowIfNull(objective);

        Objective = objective;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "�Ŀ��"));
    }

    /// <summary>
    /// �ύ���
    /// </summary>
    public void SubmitForReview()
    {
        if (Status != CampaignStatus.Draft)
            throw new InvalidOperationException("ֻ�вݸ�״̬�Ļ�����ύ���");

        if (!_advertisementIds.Any())
            throw new InvalidOperationException("������������һ���������ύ���");

        Status = CampaignStatus.PendingReview;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignSubmittedForReviewEvent(Id, AdvertiserId));
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Activate()
    {
        if (Status != CampaignStatus.PendingReview && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("ֻ�д���˻���ͣ״̬�Ļ���Լ���");

        if (IsExpired)
            throw new InvalidOperationException("�ѹ��ڵĻ�޷�����");

        if (!HasSufficientBudget(0))
            throw new InvalidOperationException("Ԥ�㲻�㣬�޷�����");

        Status = CampaignStatus.Active;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignActivatedEvent(Id));
    }

    /// <summary>
    /// ��ͣ�
    /// </summary>
    public void Pause()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("ֻ�л�Ծ״̬�Ļ������ͣ");

        Status = CampaignStatus.Paused;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignPausedEvent(Id));
    }

    /// <summary>
    /// �����
    /// </summary>
    public void End()
    {
        if (Status == CampaignStatus.Ended || Status == CampaignStatus.Deleted)
            return;

        Status = CampaignStatus.Ended;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignEndedEvent(Id));
    }

    /// <summary>
    /// ɾ���
    /// </summary>
    public override void Delete()
    {
        if (Status == CampaignStatus.Active)
            throw new InvalidOperationException("��Ծ״̬�Ļ�޷�ɾ����������ͣ");

        Status = CampaignStatus.Deleted;
        base.Delete();

        AddDomainEvent(new CampaignDeletedEvent(Id));
    }

    /// <summary>
    /// ��ӹ��
    /// </summary>
    public void AddAdvertisement(string advertisementId)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("���ID����Ϊ��", nameof(advertisementId));

        if (_advertisementIds.Contains(advertisementId))
            throw new InvalidOperationException("����Ѵ����ڻ��");

        _advertisementIds.Add(advertisementId);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAddedToCampaignEvent(Id, advertisementId));
    }

    /// <summary>
    /// �Ƴ����
    /// </summary>
    public void RemoveAdvertisement(string advertisementId)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("���ID����Ϊ��", nameof(advertisementId));

        if (!_advertisementIds.Remove(advertisementId))
            throw new InvalidOperationException("��治�����ڻ��");

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementRemovedFromCampaignEvent(Id, advertisementId));
    }

    /// <summary>
    /// ��¼չʾ
    /// </summary>
    public void RecordImpression(decimal cost)
    {
        if (!CanDeliver)
            throw new InvalidOperationException("���ǰ״̬������Ͷ��");

        TotalImpressions++;
        SpentBudget += cost;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignImpressionRecordedEvent(Id, cost));

        // ���Ԥ���Ƿ�ľ�
        if (!HasSufficientBudget(0))
        {
            AddDomainEvent(new CampaignBudgetExhaustedEvent(Id, SpentBudget));
        }
    }

    /// <summary>
    /// ��¼���
    /// </summary>
    public void RecordClick(decimal cost = 0)
    {
        TotalClicks++;
        if (cost > 0)
        {
            SpentBudget += cost;
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignClickRecordedEvent(Id, cost));
    }

    /// <summary>
    /// ��¼ת��
    /// </summary>
    public void RecordConversion()
    {
        TotalConversions++;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignConversionRecordedEvent(Id));
    }

    /// <summary>
    /// �Ƿ����Ͷ��
    /// </summary>
    public bool CanDeliver => Status == CampaignStatus.Active && 
                             !IsExpired && 
                             IsWithinDeliveryTime &&
                             HasSufficientBudget(0) &&
                             !IsDeleted;

    /// <summary>
    /// �Ƿ��ѹ���
    /// </summary>
    public bool IsExpired => DateTime.UtcNow.Date > EndDate.Date;

    /// <summary>
    /// �Ƿ���Ͷ��ʱ�䷶Χ��
    /// </summary>
    public bool IsWithinDeliveryTime
    {
        get
        {
            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
            return now.Date >= StartDate.Date && now.Date <= EndDate.Date;
        }
    }

    /// <summary>
    /// �Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal additionalCost)
    {
        return (SpentBudget + additionalCost) <= TotalBudget;
    }

    /// <summary>
    /// ��ȡʣ��Ԥ��
    /// </summary>
    public decimal GetRemainingBudget()
    {
        return Math.Max(0, TotalBudget - SpentBudget);
    }

    /// <summary>
    /// ��ȡԤ��ʹ����
    /// </summary>
    public decimal GetBudgetUtilizationRate()
    {
        return TotalBudget > 0 ? SpentBudget / TotalBudget : 0m;
    }

    /// <summary>
    /// ��ȡ�����
    /// </summary>
    public decimal GetClickThroughRate()
    {
        return TotalImpressions > 0 ? (decimal)TotalClicks / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡת����
    /// </summary>
    public decimal GetConversionRate()
    {
        return TotalClicks > 0 ? (decimal)TotalConversions / TotalClicks : 0m;
    }

    /// <summary>
    /// ��ȡƽ������չʾ�ɱ�
    /// </summary>
    public decimal GetAverageCostPerImpression()
    {
        return TotalImpressions > 0 ? SpentBudget / TotalImpressions : 0m;
    }

    /// <summary>
    /// ��ȡƽ�����ε���ɱ�
    /// </summary>
    public decimal GetAverageCostPerClick()
    {
        return TotalClicks > 0 ? SpentBudget / TotalClicks : 0m;
    }

    /// <summary>
    /// ��ȡ�ʣ������
    /// </summary>
    public int GetRemainingDays()
    {
        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
        return Math.Max(0, (EndDate.Date - now.Date).Days);
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string name, string advertiserId, decimal totalBudget, 
        decimal dailyBudget, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);

        if (string.IsNullOrWhiteSpace(advertiserId))
            throw new ArgumentException("�����ID����Ϊ��", nameof(advertiserId));

        ValidateBudget(totalBudget, dailyBudget);
        ValidateDeliveryTime(startDate, endDate);
    }

    /// <summary>
    /// ��֤�����
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("����Ʋ���Ϊ��", nameof(name));

        if (name.Length > ValidationConstants.StringLength.AdTitleMaxLength)
            throw new ArgumentException($"����Ƴ��Ȳ��ܳ���{ValidationConstants.StringLength.AdTitleMaxLength}���ַ�", nameof(name));
    }

    /// <summary>
    /// ��֤Ԥ��
    /// </summary>
    private static void ValidateBudget(decimal totalBudget, decimal dailyBudget)
    {
        if (totalBudget <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalBudget), "��Ԥ��������0");

        if (dailyBudget <= 0)
            throw new ArgumentOutOfRangeException(nameof(dailyBudget), "��Ԥ��������0");

        if (dailyBudget > totalBudget)
            throw new ArgumentException("��Ԥ�㲻�ܳ�����Ԥ��");
    }

    /// <summary>
    /// ��֤Ͷ��ʱ��
    /// </summary>
    private static void ValidateDeliveryTime(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("��ʼ���ڱ������ڽ�������");

        if (endDate < DateTime.UtcNow.Date)
            throw new ArgumentException("�������ڲ������ڵ�ǰ����");
    }
}

/// <summary>
/// �Ŀ��ֵ����
/// </summary>
public class CampaignObjective : ValueObject
{
    /// <summary>
    /// Ŀ������
    /// </summary>
    public string ObjectiveType { get; private set; } = string.Empty;

    /// <summary>
    /// Ŀ��չʾ����
    /// </summary>
    public long? TargetImpressions { get; private set; }

    /// <summary>
    /// Ŀ��������
    /// </summary>
    public long? TargetClicks { get; private set; }

    /// <summary>
    /// Ŀ��ת������
    /// </summary>
    public long? TargetConversions { get; private set; }

    /// <summary>
    /// Ŀ������
    /// </summary>
    public decimal? TargetClickThroughRate { get; private set; }

    /// <summary>
    /// Ŀ��ת����
    /// </summary>
    public decimal? TargetConversionRate { get; private set; }

    /// <summary>
    /// Ŀ��ÿ�ε���ɱ�
    /// </summary>
    public decimal? TargetCostPerClick { get; private set; }

    /// <summary>
    /// Ŀ��ÿ��ת���ɱ�
    /// </summary>
    public decimal? TargetCostPerConversion { get; private set; }

    /// <summary>
    /// Ŀ����֧���ر���
    /// </summary>
    public decimal? TargetReturnOnAdSpend { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private CampaignObjective() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CampaignObjective(
        string objectiveType,
        long? targetImpressions = null,
        long? targetClicks = null,
        long? targetConversions = null,
        decimal? targetClickThroughRate = null,
        decimal? targetConversionRate = null,
        decimal? targetCostPerClick = null,
        decimal? targetCostPerConversion = null,
        decimal? targetReturnOnAdSpend = null)
    {
        if (string.IsNullOrWhiteSpace(objectiveType))
            throw new ArgumentException("Ŀ�����Ͳ���Ϊ��", nameof(objectiveType));

        ObjectiveType = objectiveType;
        TargetImpressions = targetImpressions;
        TargetClicks = targetClicks;
        TargetConversions = targetConversions;
        TargetClickThroughRate = targetClickThroughRate;
        TargetConversionRate = targetConversionRate;
        TargetCostPerClick = targetCostPerClick;
        TargetCostPerConversion = targetCostPerConversion;
        TargetReturnOnAdSpend = targetReturnOnAdSpend;
    }

    /// <summary>
    /// ����Ʒ��֪����Ŀ��
    /// </summary>
    public static CampaignObjective CreateBrandAwareness(long targetImpressions)
    {
        return new CampaignObjective("BrandAwareness", targetImpressions: targetImpressions);
    }

    /// <summary>
    /// ��������Ŀ��
    /// </summary>
    public static CampaignObjective CreateTraffic(long targetClicks, decimal? targetCostPerClick = null)
    {
        return new CampaignObjective("Traffic", targetClicks: targetClicks, targetCostPerClick: targetCostPerClick);
    }

    /// <summary>
    /// ����ת��Ŀ��
    /// </summary>
    public static CampaignObjective CreateConversions(long targetConversions, decimal? targetCostPerConversion = null)
    {
        return new CampaignObjective("Conversions", targetConversions: targetConversions, targetCostPerConversion: targetCostPerConversion);
    }

    /// <summary>
    /// ��������Ŀ��
    /// </summary>
    public static CampaignObjective CreateSales(decimal targetReturnOnAdSpend)
    {
        return new CampaignObjective("Sales", targetReturnOnAdSpend: targetReturnOnAdSpend);
    }

    /// <summary>
    /// ����Ŀ�������
    /// </summary>
    public decimal CalculateCompletionRate(long actualImpressions, long actualClicks, long actualConversions, decimal actualSpent)
    {
        return ObjectiveType switch
        {
            "BrandAwareness" => TargetImpressions.HasValue ? (decimal)actualImpressions / TargetImpressions.Value : 0m,
            "Traffic" => TargetClicks.HasValue ? (decimal)actualClicks / TargetClicks.Value : 0m,
            "Conversions" => TargetConversions.HasValue ? (decimal)actualConversions / TargetConversions.Value : 0m,
            _ => 0m
        };
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ObjectiveType;
        yield return TargetImpressions ?? 0;
        yield return TargetClicks ?? 0;
        yield return TargetConversions ?? 0;
        yield return TargetClickThroughRate ?? 0m;
        yield return TargetConversionRate ?? 0m;
        yield return TargetCostPerClick ?? 0m;
        yield return TargetCostPerConversion ?? 0m;
        yield return TargetReturnOnAdSpend ?? 0m;
    }
}