using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// �����ṩ��Ԫ����
/// �����ṩ�ߵı�ʶ��Ϣ���������������ò���
/// ����ע�������·�ɾ���
/// </summary>
public class DataProviderMetadata
{
    /// <summary>
    /// �ṩ��Ψһ��ʶ��
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    /// �ṩ������
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// �ṩ������
    /// </summary>
    public DataProviderType ProviderType { get; set; }

    /// <summary>
    /// ҵ��ʵ������
    /// �� "Advertisement", "UserProfile", "Targeting", "Delivery"
    /// </summary>
    public string BusinessEntity { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// �� "Redis", "SqlServer", "MySQL", "Memory"
    /// </summary>
    public string TechnologyType { get; set; } = string.Empty;

    /// <summary>
    /// ƽ̨����
    /// �� "AlibabaCloud", "Azure", "AWS", "Local"
    /// </summary>
    public string PlatformType { get; set; } = string.Empty;

    /// <summary>
    /// �ṩ�����ȼ�
    /// ��ֵԽ�����ȼ�Խ��
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// ��չԪ����
    /// �洢�ṩ���ض������ú�������Ϣ
    /// </summary>
    public Dictionary<string, object> ExtendedMetadata { get; set; } = new();

    /// <summary>
    /// ֧�ֵĲ�������
    /// �� ["Get", "Set", "Remove", "Query", "Batch"]
    /// </summary>
    public string[] SupportedOperations { get; set; } = Array.Empty<string>();

    /// <summary>
    /// �ṩ�߰汾
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// ����״̬
    /// </summary>
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Unknown;

    /// <summary>
    /// ��������
    /// </summary>
    public PerformanceProfile PerformanceProfile { get; set; } = new();

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ��ȡ��չԪ����ֵ
    /// </summary>
    /// <typeparam name="T">ֵ����</typeparam>
    /// <param name="key">��</param>
    /// <returns>Ԫ����ֵ</returns>
    public T? GetExtendedMetadata<T>(string key)
    {
        if (ExtendedMetadata.TryGetValue(key, out var value))
        {
            return value is T typedValue ? typedValue : default;
        }
        return default;
    }

    /// <summary>
    /// ������չԪ����ֵ
    /// </summary>
    /// <param name="key">��</param>
    /// <param name="value">ֵ</param>
    public void SetExtendedMetadata(string key, object value)
    {
        ExtendedMetadata[key] = value;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ����Ƿ�֧��ָ������
    /// </summary>
    /// <param name="operation">��������</param>
    /// <returns>�Ƿ�֧��</returns>
    public bool SupportsOperation(string operation)
    {
        return SupportedOperations.Contains(operation, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// ����������Ϣ
/// </summary>
public class PerformanceProfile
{
    /// <summary>
    /// ������Ӧʱ�䣨���룩
    /// </summary>
    public int ExpectedResponseTimeMs { get; set; } = 100;

    /// <summary>
    /// �����Ӧʱ�䣨���룩
    /// </summary>
    public int MaxResponseTimeMs { get; set; } = 5000;

    /// <summary>
    /// ��󲢷�������
    /// </summary>
    public int MaxConcurrentConnections { get; set; } = 100;

    /// <summary>
    /// ֧����������������С
    /// </summary>
    public int MaxBatchSize { get; set; } = 1000;

    /// <summary>
    /// �Ƿ�֧�ֲ��в���
    /// </summary>
    public bool SupportsParallelOperations { get; set; } = true;

    /// <summary>
    /// ��������
    /// </summary>
    public CacheConfiguration CacheConfiguration { get; set; } = new();
}

/// <summary>
/// ����������Ϣ
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// �Ƿ����û���
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    /// <summary>
    /// Ĭ�ϻ������ʱ��
    /// </summary>
    public TimeSpan DefaultCacheDuration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// ��󻺴����ʱ��
    /// </summary>
    public TimeSpan MaxCacheDuration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// �������
    /// </summary>
    public CacheStrategy CacheStrategy { get; set; } = CacheStrategy.WriteThrough;
}