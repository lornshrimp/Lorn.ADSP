using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Aggregates;

/// <summary>
/// Ͷ�ż�¼�ۺϸ�
/// </summary>
public class DeliveryRecord : AggregateRoot
{
    /// <summary>
    /// ���ID
    /// </summary>
    public string AdId { get; private set; } = string.Empty;

    /// <summary>
    /// չʾ����ID
    /// </summary>
    public string ImpressionId { get; private set; } = string.Empty;

    /// <summary>
    /// ����ID
    /// </summary>
    public string RequestId { get; private set; } = string.Empty;

    /// <summary>
    /// Ͷ��ʱ���
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// ����۸񣨷֣�
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Ͷ��״̬
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    /// Ͷ��������
    /// </summary>
    public DeliveryContext Context { get; private set; } = null!;

    /// <summary>
    /// ���ʱ��
    /// </summary>
    public DateTime? ClickTime { get; private set; }

    /// <summary>
    /// ת��ʱ��
    /// </summary>
    public DateTime? ConversionTime { get; private set; }

    /// <summary>
    /// ת�����루�֣�
    /// </summary>
    public decimal? Revenue { get; private set; }

    /// <summary>
    /// �����÷�
    /// </summary>
    public decimal QualityScore { get; private set; } = 1.0m;

    /// <summary>
    /// ���ۼ۸񣨷֣�
    /// </summary>
    public decimal BidPrice { get; private set; }

    /// <summary>
    /// ���������ϸ
    /// </summary>
    public BillingDetails BillingDetails { get; private set; } = null!;

    /// <summary>
    /// ˽�й��캯��������ORM��
    /// </summary>
    private DeliveryRecord() { }

    /// <summary>
    /// ���캯��
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

        // ����Ͷ�ż�¼�����¼�
        AddDomainEvent(new DeliveryRecordCreatedEvent(Id, adId, impressionId, price));
    }

    /// <summary>
    /// ��¼���
    /// </summary>
    public void RecordClick(decimal? clickCost = null)
    {
        if (ClickTime.HasValue)
            throw new InvalidOperationException("��Ͷ�ż�¼�Ѽ�¼�����");

        ClickTime = DateTime.UtcNow;

        if (clickCost.HasValue)
        {
            Price += clickCost.Value;
            BillingDetails = BillingDetails.AddCost(clickCost.Value, "�������");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryClickRecordedEvent(Id, AdId, clickCost ?? 0));
    }

    /// <summary>
    /// ��¼ת��
    /// </summary>
    public void RecordConversion(decimal? revenue = null, decimal? conversionCost = null)
    {
        if (ConversionTime.HasValue)
            throw new InvalidOperationException("��Ͷ�ż�¼�Ѽ�¼��ת��");

        ConversionTime = DateTime.UtcNow;
        Revenue = revenue;

        if (conversionCost.HasValue)
        {
            Price += conversionCost.Value;
            BillingDetails = BillingDetails.AddCost(conversionCost.Value, "ת������");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryConversionRecordedEvent(Id, AdId, revenue ?? 0));
    }

    /// <summary>
    /// ����Ͷ��״̬
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
    /// ���Ϊʧ��
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        Status = DeliveryStatus.Failed;
        BillingDetails = BillingDetails.AddNote($"ʧ��ԭ��: {reason}");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryFailedEvent(Id, AdId, reason));
    }

    /// <summary>
    /// ���Ϊ��ʱ
    /// </summary>
    public void MarkAsTimeout()
    {
        Status = DeliveryStatus.Timeout;
        BillingDetails = BillingDetails.AddNote("Ͷ�ų�ʱ");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryTimeoutEvent(Id, AdId));
    }

    /// <summary>
    /// ��ȡͶ��Ч��
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
    /// �Ƿ���ЧͶ��
    /// </summary>
    public bool IsSuccessfulDelivery => Status == DeliveryStatus.Success;

    /// <summary>
    /// �Ƿ�������
    /// </summary>
    public bool HasClick => ClickTime.HasValue;

    /// <summary>
    /// �Ƿ����ת��
    /// </summary>
    public bool HasConversion => ConversionTime.HasValue;

    /// <summary>
    /// ��ȡͶ��ʱ�������룩
    /// </summary>
    public double GetDeliveryDurationMs()
    {
        return (DateTime.UtcNow - Timestamp).TotalMilliseconds;
    }

    /// <summary>
    /// ��ȡ���ʱ�������룩
    /// </summary>
    public double? GetClickDurationMs()
    {
        return ClickTime.HasValue ? (ClickTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// ��ȡת��ʱ�������룩
    /// </summary>
    public double? GetConversionDurationMs()
    {
        return ConversionTime.HasValue ? (ConversionTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// ����ROI��Ͷ�ʻر��ʣ�
    /// </summary>
    public decimal? CalculateROI()
    {
        if (!Revenue.HasValue || Price == 0)
            return null;

        return (Revenue.Value - Price) / Price;
    }

    /// <summary>
    /// ����ROAS�����֧���ر��ʣ�
    /// </summary>
    public decimal? CalculateROAS()
    {
        if (!Revenue.HasValue || Price == 0)
            return null;

        return Revenue.Value / Price;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string adId, string impressionId, string requestId, 
        decimal price, decimal bidPrice, DeliveryContext context, BillingDetails billingDetails)
    {
        if (string.IsNullOrWhiteSpace(adId))
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        if (string.IsNullOrWhiteSpace(impressionId))
            throw new ArgumentException("չʾID����Ϊ��", nameof(impressionId));

        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "�۸���Ϊ����");

        if (bidPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(bidPrice), "���ۼ۸���Ϊ����");

        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(billingDetails);
    }
}

/// <summary>
/// Ͷ��������ֵ����
/// </summary>
public class DeliveryContext : ValueObject
{
    /// <summary>
    /// �û�ID
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// �豸ID
    /// </summary>
    public string? DeviceId { get; private set; }

    /// <summary>
    /// IP��ַ
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// �û�����
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// ����ҳ��
    /// </summary>
    public string? Referrer { get; private set; }

    /// <summary>
    /// ҳ��URL
    /// </summary>
    public string? PageUrl { get; private set; }

    /// <summary>
    /// ����λ��
    /// </summary>
    public string? GeoLocation { get; private set; }

    /// <summary>
    /// ����
    /// </summary>
    public string? Language { get; private set; }

    /// <summary>
    /// �豸����
    /// </summary>
    public DeviceType? DeviceType { get; private set; }

    /// <summary>
    /// ý������
    /// </summary>
    public MediaType MediaType { get; private set; }

    /// <summary>
    /// ���λID
    /// </summary>
    public string? PlacementId { get; private set; }

    /// <summary>
    /// ���λ�ߴ�
    /// </summary>
    public string? AdSize { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DeliveryContext() { }

    /// <summary>
    /// ���캯��
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
    /// ��ȡ����ԱȽϵ����
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
/// ���������ϸֵ����
/// </summary>
public class BillingDetails : ValueObject
{
    /// <summary>
    /// �������ã��֣�
    /// </summary>
    public decimal BaseCost { get; private set; }

    /// <summary>
    /// ��������б�
    /// </summary>
    public IReadOnlyList<AdditionalCost> AdditionalCosts { get; private set; } = new List<AdditionalCost>();

    /// <summary>
    /// ����˵��
    /// </summary>
    public IReadOnlyList<string> Notes { get; private set; } = new List<string>();

    /// <summary>
    /// �Ʒ�ģʽ
    /// </summary>
    public string BillingMode { get; private set; } = string.Empty;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BillingDetails() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public BillingDetails(
        decimal baseCost,
        string billingMode,
        IList<AdditionalCost>? additionalCosts = null,
        IList<string>? notes = null)
    {
        if (baseCost < 0)
            throw new ArgumentOutOfRangeException(nameof(baseCost), "�������ò���Ϊ����");

        if (string.IsNullOrWhiteSpace(billingMode))
            throw new ArgumentException("�Ʒ�ģʽ����Ϊ��", nameof(billingMode));

        BaseCost = baseCost;
        BillingMode = billingMode;
        AdditionalCosts = additionalCosts?.ToList() ?? new List<AdditionalCost>();
        Notes = notes?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// ����CPM�Ʒ�
    /// </summary>
    public static BillingDetails CreateCPM(decimal cost)
    {
        return new BillingDetails(cost, "CPM");
    }

    /// <summary>
    /// ����CPC�Ʒ�
    /// </summary>
    public static BillingDetails CreateCPC(decimal cost)
    {
        return new BillingDetails(cost, "CPC");
    }

    /// <summary>
    /// ����CPA�Ʒ�
    /// </summary>
    public static BillingDetails CreateCPA(decimal cost)
    {
        return new BillingDetails(cost, "CPA");
    }

    /// <summary>
    /// ��Ӷ������
    /// </summary>
    public BillingDetails AddCost(decimal amount, string description)
    {
        var newAdditionalCosts = AdditionalCosts.ToList();
        newAdditionalCosts.Add(new AdditionalCost(amount, description));

        return new BillingDetails(BaseCost, BillingMode, newAdditionalCosts, Notes.ToList());
    }

    /// <summary>
    /// ���˵��
    /// </summary>
    public BillingDetails AddNote(string note)
    {
        var newNotes = Notes.ToList();
        newNotes.Add(note);

        return new BillingDetails(BaseCost, BillingMode, AdditionalCosts.ToList(), newNotes);
    }

    /// <summary>
    /// ��ȡ�ܷ���
    /// </summary>
    public decimal GetTotalCost()
    {
        return BaseCost + AdditionalCosts.Sum(c => c.Amount);
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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
/// �������ֵ����
/// </summary>
public class AdditionalCost : ValueObject
{
    /// <summary>
    /// ���ý��֣�
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public AdditionalCost(decimal amount, string description)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "���ý���Ϊ����");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("������������Ϊ��", nameof(description));

        Amount = amount;
        Description = description;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Description;
        yield return Timestamp;
    }
}

/// <summary>
/// Ͷ��Ч��ֵ����
/// </summary>
public class DeliveryPerformance : ValueObject
{
    /// <summary>
    /// �Ƿ��е��
    /// </summary>
    public bool HasClick { get; private set; }

    /// <summary>
    /// �Ƿ���ת��
    /// </summary>
    public bool HasConversion { get; private set; }

    /// <summary>
    /// �ܷ��ã��֣�
    /// </summary>
    public decimal TotalCost { get; private set; }

    /// <summary>
    /// ���루�֣�
    /// </summary>
    public decimal Revenue { get; private set; }

    /// <summary>
    /// �����÷�
    /// </summary>
    public decimal QualityScore { get; private set; }

    /// <summary>
    /// ����ӳ٣����룩
    /// </summary>
    public double? ClickLatency { get; private set; }

    /// <summary>
    /// ת���ӳ٣����룩
    /// </summary>
    public double? ConversionLatency { get; private set; }

    /// <summary>
    /// ���캯��
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
    /// ����ROI
    /// </summary>
    public decimal? CalculateROI()
    {
        if (TotalCost == 0)
            return null;

        return (Revenue - TotalCost) / TotalCost;
    }

    /// <summary>
    /// ����ROAS
    /// </summary>
    public decimal? CalculateROAS()
    {
        if (TotalCost == 0)
            return null;

        return Revenue / TotalCost;
    }

    /// <summary>
    /// ��ȡЧ���ȼ�
    /// </summary>
    public string GetPerformanceGrade()
    {
        var roas = CalculateROAS();
        if (!roas.HasValue)
            return "N/A";

        return roas.Value switch
        {
            >= 3.0m => "����",
            >= 2.0m => "����",
            >= 1.0m => "һ��",
            _ => "�ϲ�"
        };
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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
/// Ͷ�ż�¼�����¼�
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
/// Ͷ�ŵ����¼�¼�
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
/// Ͷ��ת����¼�¼�
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
/// Ͷ��״̬�����¼�
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
/// Ͷ��ʧ���¼�
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
/// Ͷ�ų�ʱ�¼�
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