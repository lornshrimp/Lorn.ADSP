using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ������������
/// </summary>
public class BatchOperationError
{
    /// <summary>
    /// ��������
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// �쳣����
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// ����������
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
}



/// <summary>
/// ��������
/// </summary>
public class SortCriteria
{
    /// <summary>
    /// �����ֶ�
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}

/// <summary>
/// ��ҳ��Ϣ
/// </summary>
public class PaginationInfo
{
    /// <summary>
    /// ҳ�루��0��ʼ��
    /// </summary>
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// ҳ��С
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// ��������Ŀ��
    /// </summary>
    public int Skip => PageIndex * PageSize;

    /// <summary>
    /// �α����ƣ������α��ҳ��
    /// </summary>
    public string? CursorToken { get; set; }
}

/// <summary>
/// �����ṩ��ͳ����Ϣ
/// </summary>
public class DataProviderStatistics
{
    /// <summary>
    /// ��������
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// �ɹ�������
    /// </summary>
    public long SuccessfulRequests { get; set; }

    /// <summary>
    /// ʧ��������
    /// </summary>
    public long FailedRequests { get; set; }

    /// <summary>
    /// ƽ����Ӧʱ�䣨���룩
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// �����Ӧʱ�䣨���룩
    /// </summary>
    public double MaxResponseTimeMs { get; set; }

    /// <summary>
    /// ��С��Ӧʱ�䣨���룩
    /// </summary>
    public double MinResponseTimeMs { get; set; }

    /// <summary>
    /// ��ǰ����������
    /// </summary>
    public int CurrentConnections { get; set; }

    /// <summary>
    /// ��󲢷�������
    /// </summary>
    public int MaxConnections { get; set; }

    /// <summary>
    /// ����������
    /// </summary>
    public double CacheHitRate { get; set; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ��չͳ����Ϣ
    /// </summary>
    public Dictionary<string, object> ExtendedStatistics { get; set; } = new();
}

/// <summary>
/// ע���ͳ����Ϣ
/// </summary>
public class RegistryStatistics
{
    /// <summary>
    /// ��ע���ṩ������
    /// </summary>
    public int TotalProviders { get; set; }

    /// <summary>
    /// ���õ��ṩ����
    /// </summary>
    public int EnabledProviders { get; set; }

    /// <summary>
    /// �������ṩ����
    /// </summary>
    public int HealthyProviders { get; set; }

    /// <summary>
    /// �����ͷ�����ṩ��ͳ��
    /// </summary>
    public Dictionary<DataProviderType, int> ProvidersByType { get; set; } = new();

    /// <summary>
    /// ��ҵ��ʵ�������ṩ��ͳ��
    /// </summary>
    public Dictionary<string, int> ProvidersByBusinessEntity { get; set; } = new();

    /// <summary>
    /// ���������ͷ�����ṩ��ͳ��
    /// </summary>
    public Dictionary<string, int> ProvidersByTechnology { get; set; } = new();

    /// <summary>
    /// ���ͳ��ʱ��
    /// </summary>
    public DateTime LastCalculated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ·��ͳ����Ϣ
/// </summary>
public class RoutingStatistics
{
    /// <summary>
    /// ·����������
    /// </summary>
    public long TotalRoutingRequests { get; set; }

    /// <summary>
    /// �ɹ�·����
    /// </summary>
    public long SuccessfulRoutes { get; set; }

    /// <summary>
    /// ʧ��·����
    /// </summary>
    public long FailedRoutes { get; set; }

    /// <summary>
    /// ƽ��·��ʱ�䣨���룩
    /// </summary>
    public double AverageRoutingTimeMs { get; set; }

    /// <summary>
    /// ���ṩ�����ͷ����·��ͳ��
    /// </summary>
    public Dictionary<DataProviderType, long> RoutesByProviderType { get; set; } = new();

    /// <summary>
    /// ��ҵ��ʵ������·��ͳ��
    /// </summary>
    public Dictionary<string, long> RoutesByBusinessEntity { get; set; } = new();

    /// <summary>
    /// ��Ծ·�ɹ�����
    /// </summary>
    public int ActiveRoutingRules { get; set; }

    /// <summary>
    /// ���ͳ��ʱ��
    /// </summary>
    public DateTime LastCalculated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ·�ɲ��Խ��
/// </summary>
public class RoutingTestResult
{
    /// <summary>
    /// �����Ƿ�ɹ�
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ѡ����ṩ��
    /// </summary>
    public IDataAccessProvider? SelectedProvider { get; set; }

    /// <summary>
    /// Ӧ�õĹ���
    /// </summary>
    public IReadOnlyList<string> AppliedRules { get; set; } = Array.Empty<string>();

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime TestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ���Ժ�ʱ
    /// </summary>
    public TimeSpan Duration { get; set; }
}