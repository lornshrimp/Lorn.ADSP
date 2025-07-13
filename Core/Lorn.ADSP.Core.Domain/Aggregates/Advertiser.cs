using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;

namespace Lorn.ADSP.Core.Domain.Aggregates;

/// <summary>
/// 广告主实体（聚合根）
/// </summary>
public class Advertiser : AggregateRoot
{
    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; private set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string ContactName { get; private set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// 广告主状态
    /// </summary>
    public AdvertiserStatus Status { get; private set; }

    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// 资质信息
    /// </summary>
    public QualificationInfo Qualification { get; private set; } = null!;

    /// <summary>
    /// 账单信息
    /// </summary>
    public BillingInfo Billing { get; private set; } = null!;

    /// <summary>
    /// 广告集合
    /// </summary>
    private readonly List<Advertisement> _advertisements = new();
    public IReadOnlyList<Advertisement> Advertisements => _advertisements.AsReadOnly();

    /// <summary>
    /// 私有构造函数，用于ORM
    /// </summary>
    private Advertiser() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Advertiser(
        string companyName,
        string contactName,
        string email,
        string phone,
        QualificationInfo qualification,
        BillingInfo billing)
    {
        ValidateInputs(companyName, contactName, email, phone);

        CompanyName = companyName;
        ContactName = contactName;
        Email = email;
        Phone = phone;
        Qualification = qualification;
        Billing = billing;
        Status = AdvertiserStatus.Pending;

        // 触发广告主注册事件
        AddDomainEvent(new AdvertiserRegisteredEvent(Id, companyName, email));
    }

    /// <summary>
    /// 注册广告主
    /// </summary>
    public static Advertiser Register(RegistrationInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        var advertiser = new Advertiser(
            info.CompanyName,
            info.ContactName,
            info.Email,
            info.Phone,
            info.Qualification,
            info.Billing);

        return advertiser;
    }

    /// <summary>
    /// 提交资质认证
    /// </summary>
    public void SubmitQualification(QualificationInfo qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);

        if (Status != AdvertiserStatus.Pending && Status != AdvertiserStatus.Rejected)
            throw new InvalidOperationException("只有待审核或被拒绝状态的广告主可以提交资质");

        Qualification = qualification;
        Status = AdvertiserStatus.UnderReview;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserQualificationSubmittedEvent(Id, qualification.BusinessLicense));
    }

    /// <summary>
    /// 激活广告主
    /// </summary>
    public void Activate()
    {
        if (Status != AdvertiserStatus.UnderReview)
            throw new InvalidOperationException("只有审核中的广告主可以激活");

        Status = AdvertiserStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserActivatedEvent(Id));
    }

    /// <summary>
    /// 暂停广告主
    /// </summary>
    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("暂停必须提供原因", nameof(reason));

        if (Status != AdvertiserStatus.Active)
            throw new InvalidOperationException("只有激活状态的广告主可以暂停");

        Status = AdvertiserStatus.Suspended;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserSuspendedEvent(Id, reason));
    }

    /// <summary>
    /// 拒绝广告主
    /// </summary>
    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("拒绝必须提供原因", nameof(reason));

        if (Status != AdvertiserStatus.UnderReview)
            throw new InvalidOperationException("只有审核中的广告主可以拒绝");

        Status = AdvertiserStatus.Rejected;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserRejectedEvent(Id, reason));
    }

    /// <summary>
    /// 恢复广告主
    /// </summary>
    public void Resume()
    {
        if (Status != AdvertiserStatus.Suspended)
            throw new InvalidOperationException("只有暂停状态的广告主可以恢复");

        Status = AdvertiserStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserResumedEvent(Id));
    }

    /// <summary>
    /// 更新账单信息
    /// </summary>
    public void UpdateBilling(BillingInfo billing)
    {
        ArgumentNullException.ThrowIfNull(billing);

        Billing = billing;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserBillingUpdatedEvent(Id));
    }

    /// <summary>
    /// 更新联系信息
    /// </summary>
    public void UpdateContactInfo(string contactName, string email, string phone)
    {
        ValidateContactInfo(contactName, email, phone);

        ContactName = contactName;
        Email = email;
        Phone = phone;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserContactUpdatedEvent(Id, email));
    }

    /// <summary>
    /// 添加广告
    /// </summary>
    public void AddAdvertisement(Advertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        if (advertisement.AdvertiserId != Id)
            throw new ArgumentException("广告所属广告主ID不匹配");

        if (Status != AdvertiserStatus.Active)
            throw new InvalidOperationException("只有激活状态的广告主可以创建广告");

        _advertisements.Add(advertisement);
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserAdvertisementAddedEvent(Id, advertisement.Id));
    }

    /// <summary>
    /// 获取活跃广告
    /// </summary>
    public IReadOnlyList<Advertisement> GetActiveAdvertisements()
    {
        return _advertisements.Where(a => a.IsActive && !a.IsDeleted).ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取总消费金额
    /// </summary>
    public decimal GetTotalSpent()
    {
        return _advertisements.Sum(a => a.TotalSpent);
    }

    /// <summary>
    /// 获取总展示次数
    /// </summary>
    public long GetTotalImpressions()
    {
        return _advertisements.Sum(a => a.TotalImpressions);
    }

    /// <summary>
    /// 获取总点击次数
    /// </summary>
    public long GetTotalClicks()
    {
        return _advertisements.Sum(a => a.TotalClicks);
    }

    /// <summary>
    /// 获取平均点击率
    /// </summary>
    public decimal GetAverageClickThroughRate()
    {
        var totalImpressions = GetTotalImpressions();
        var totalClicks = GetTotalClicks();
        return totalImpressions > 0 ? (decimal)totalClicks / totalImpressions : 0m;
    }

    /// <summary>
    /// 是否可以创建广告
    /// </summary>
    public bool CanCreateAdvertisement => Status == AdvertiserStatus.Active && !IsDeleted;

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string companyName, string contactName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("公司名称不能为空", nameof(companyName));

        if (companyName.Length > ValidationConstants.StringLength.CompanyNameMaxLength)
            throw new ArgumentException($"公司名称长度不能超过{ValidationConstants.StringLength.CompanyNameMaxLength}个字符", nameof(companyName));

        ValidateContactInfo(contactName, email, phone);
    }

    /// <summary>
    /// 验证联系信息
    /// </summary>
    private static void ValidateContactInfo(string contactName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new ArgumentException("联系人姓名不能为空", nameof(contactName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("邮箱不能为空", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("邮箱格式不正确", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("电话不能为空", nameof(phone));
    }

    /// <summary>
    /// 验证邮箱格式
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// 注册信息
/// </summary>
public record RegistrationInfo(
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    QualificationInfo Qualification,
    BillingInfo Billing);

/// <summary>
/// 资质信息值对象
/// </summary>
public record QualificationInfo(
    string BusinessLicense,
    string TaxNumber,
    string LegalRepresentative,
    DateTime IssueDate,
    DateTime ExpiryDate,
    IReadOnlyList<string> Certificates);

