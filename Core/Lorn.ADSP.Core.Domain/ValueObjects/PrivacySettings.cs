using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 隐私设置值对象
/// </summary>
public class PrivacySettings : ValueObject
{
    /// <summary>
    /// 是否同意数据收集
    /// </summary>
    public bool ConsentToDataCollection { get; private set; } = false;

    /// <summary>
    /// 是否同意数据处理
    /// </summary>
    public bool ConsentToDataProcessing { get; private set; } = false;

    /// <summary>
    /// 是否同意数据共享
    /// </summary>
    public bool ConsentToDataSharing { get; private set; } = false;

    /// <summary>
    /// 是否同意营销用途
    /// </summary>
    public bool ConsentToMarketing { get; private set; } = false;

    /// <summary>
    /// 同意时间
    /// </summary>
    public DateTime? ConsentTimestamp { get; private set; }

    /// <summary>
    /// 同意版本
    /// </summary>
    public string? ConsentVersion { get; private set; }

    /// <summary>
    /// 同意IP地址
    /// </summary>
    public string? ConsentIpAddress { get; private set; }

    /// <summary>
    /// 同意用户代理
    /// </summary>
    public string? ConsentUserAgent { get; private set; }

    /// <summary>
    /// 数据保留期限（天数）
    /// </summary>
    public int? DataRetentionDays { get; private set; }

    /// <summary>
    /// 是否请求删除数据
    /// </summary>
    public bool RequestDataDeletion { get; private set; } = false;

    /// <summary>
    /// 数据删除请求时间
    /// </summary>
    public DateTime? DataDeletionRequestedAt { get; private set; }

    /// <summary>
    /// 是否选择退出
    /// </summary>
    public bool OptOut { get; private set; } = false;

    /// <summary>
    /// 选择退出时间
    /// </summary>
    public DateTime? OptOutTimestamp { get; private set; }

    /// <summary>
    /// GDPR相关设置
    /// </summary>
    public GdprSettings? GdprSettings { get; private set; }

    /// <summary>
    /// CCPA相关设置
    /// </summary>
    public CcpaSettings? CcpaSettings { get; private set; }

    /// <summary>
    /// 自定义隐私设置
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomSettings { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private PrivacySettings() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PrivacySettings(
        bool consentToDataCollection = false,
        bool consentToDataProcessing = false,
        bool consentToDataSharing = false,
        bool consentToMarketing = false,
        DateTime? consentTimestamp = null,
        string? consentVersion = null,
        string? consentIpAddress = null,
        string? consentUserAgent = null,
        int? dataRetentionDays = null,
        bool requestDataDeletion = false,
        DateTime? dataDeletionRequestedAt = null,
        bool optOut = false,
        DateTime? optOutTimestamp = null,
        GdprSettings? gdprSettings = null,
        CcpaSettings? ccpaSettings = null,
        IDictionary<string, object>? customSettings = null)
    {
        ValidateDataRetentionDays(dataRetentionDays);

        ConsentToDataCollection = consentToDataCollection;
        ConsentToDataProcessing = consentToDataProcessing;
        ConsentToDataSharing = consentToDataSharing;
        ConsentToMarketing = consentToMarketing;
        ConsentTimestamp = consentTimestamp;
        ConsentVersion = consentVersion;
        ConsentIpAddress = consentIpAddress;
        ConsentUserAgent = consentUserAgent;
        DataRetentionDays = dataRetentionDays;
        RequestDataDeletion = requestDataDeletion;
        DataDeletionRequestedAt = dataDeletionRequestedAt;
        OptOut = optOut;
        OptOutTimestamp = optOutTimestamp;
        GdprSettings = gdprSettings;
        CcpaSettings = ccpaSettings;
        CustomSettings = customSettings?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// 创建默认隐私设置
    /// </summary>
    public static PrivacySettings CreateDefault()
    {
        return new PrivacySettings();
    }

    /// <summary>
    /// 创建全部同意的隐私设置
    /// </summary>
    public static PrivacySettings CreateFullConsent(
        string consentVersion,
        string ipAddress,
        string userAgent)
    {
        return new PrivacySettings(
            consentToDataCollection: true,
            consentToDataProcessing: true,
            consentToDataSharing: true,
            consentToMarketing: true,
            consentTimestamp: DateTime.UtcNow,
            consentVersion: consentVersion,
            consentIpAddress: ipAddress,
            consentUserAgent: userAgent);
    }

    /// <summary>
    /// 创建最小同意的隐私设置
    /// </summary>
    public static PrivacySettings CreateMinimalConsent(
        string consentVersion,
        string ipAddress,
        string userAgent)
    {
        return new PrivacySettings(
            consentToDataCollection: true,
            consentToDataProcessing: true,
            consentToDataSharing: false,
            consentToMarketing: false,
            consentTimestamp: DateTime.UtcNow,
            consentVersion: consentVersion,
            consentIpAddress: ipAddress,
            consentUserAgent: userAgent);
    }

    /// <summary>
    /// 是否有效同意
    /// </summary>
    public bool HasValidConsent()
    {
        return ConsentTimestamp.HasValue && 
               !string.IsNullOrWhiteSpace(ConsentVersion) &&
               (ConsentToDataCollection || ConsentToDataProcessing);
    }

    /// <summary>
    /// 是否可以用于营销
    /// </summary>
    public bool CanUseForMarketing()
    {
        return HasValidConsent() && 
               ConsentToMarketing && 
               !OptOut && 
               !RequestDataDeletion;
    }

    /// <summary>
    /// 是否可以共享数据
    /// </summary>
    public bool CanShareData()
    {
        return HasValidConsent() && 
               ConsentToDataSharing && 
               !OptOut && 
               !RequestDataDeletion;
    }

    /// <summary>
    /// 是否需要删除数据
    /// </summary>
    public bool ShouldDeleteData()
    {
        if (RequestDataDeletion)
            return true;

        if (DataRetentionDays.HasValue && ConsentTimestamp.HasValue)
        {
            var retentionExpiry = ConsentTimestamp.Value.AddDays(DataRetentionDays.Value);
            return DateTime.UtcNow > retentionExpiry;
        }

        return false;
    }

    /// <summary>
    /// 获取自定义设置值
    /// </summary>
    public T? GetCustomSetting<T>(string key)
    {
        if (CustomSettings.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// 验证数据保留天数
    /// </summary>
    private static void ValidateDataRetentionDays(int? dataRetentionDays)
    {
        if (dataRetentionDays.HasValue && (dataRetentionDays.Value < 1 || dataRetentionDays.Value > 3650))
            throw new ArgumentOutOfRangeException(nameof(dataRetentionDays), "数据保留天数必须在1-3650天之间");
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ConsentToDataCollection;
        yield return ConsentToDataProcessing;
        yield return ConsentToDataSharing;
        yield return ConsentToMarketing;
        yield return ConsentTimestamp ?? DateTime.MinValue;
        yield return ConsentVersion ?? string.Empty;
        yield return ConsentIpAddress ?? string.Empty;
        yield return ConsentUserAgent ?? string.Empty;
        yield return DataRetentionDays ?? 0;
        yield return RequestDataDeletion;
        yield return DataDeletionRequestedAt ?? DateTime.MinValue;
        yield return OptOut;
        yield return OptOutTimestamp ?? DateTime.MinValue;
        yield return GdprSettings ?? new object();
        yield return CcpaSettings ?? new object();
    }
}

/// <summary>
/// GDPR设置
/// </summary>
public class GdprSettings : ValueObject
{
    /// <summary>
    /// 是否适用GDPR
    /// </summary>
    public bool IsApplicable { get; private set; } = false;

    /// <summary>
    /// 同意撤回时间
    /// </summary>
    public DateTime? ConsentWithdrawnAt { get; private set; }

    /// <summary>
    /// 数据可携权请求
    /// </summary>
    public bool RequestDataPortability { get; private set; } = false;

    /// <summary>
    /// 数据更正权请求
    /// </summary>
    public bool RequestDataRectification { get; private set; } = false;

    /// <summary>
    /// 处理限制权请求
    /// </summary>
    public bool RequestProcessingRestriction { get; private set; } = false;

    public GdprSettings(
        bool isApplicable = false,
        DateTime? consentWithdrawnAt = null,
        bool requestDataPortability = false,
        bool requestDataRectification = false,
        bool requestProcessingRestriction = false)
    {
        IsApplicable = isApplicable;
        ConsentWithdrawnAt = consentWithdrawnAt;
        RequestDataPortability = requestDataPortability;
        RequestDataRectification = requestDataRectification;
        RequestProcessingRestriction = requestProcessingRestriction;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsApplicable;
        yield return ConsentWithdrawnAt ?? DateTime.MinValue;
        yield return RequestDataPortability;
        yield return RequestDataRectification;
        yield return RequestProcessingRestriction;
    }
}

/// <summary>
/// CCPA设置
/// </summary>
public class CcpaSettings : ValueObject
{
    /// <summary>
    /// 是否适用CCPA
    /// </summary>
    public bool IsApplicable { get; private set; } = false;

    /// <summary>
    /// 选择退出销售个人信息
    /// </summary>
    public bool OptOutOfSale { get; private set; } = false;

    /// <summary>
    /// 选择退出时间
    /// </summary>
    public DateTime? OptOutOfSaleTimestamp { get; private set; }

    /// <summary>
    /// 请求删除个人信息
    /// </summary>
    public bool RequestPersonalInfoDeletion { get; private set; } = false;

    public CcpaSettings(
        bool isApplicable = false,
        bool optOutOfSale = false,
        DateTime? optOutOfSaleTimestamp = null,
        bool requestPersonalInfoDeletion = false)
    {
        IsApplicable = isApplicable;
        OptOutOfSale = optOutOfSale;
        OptOutOfSaleTimestamp = optOutOfSaleTimestamp;
        RequestPersonalInfoDeletion = requestPersonalInfoDeletion;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsApplicable;
        yield return OptOutOfSale;
        yield return OptOutOfSaleTimestamp ?? DateTime.MinValue;
        yield return RequestPersonalInfoDeletion;
    }
}