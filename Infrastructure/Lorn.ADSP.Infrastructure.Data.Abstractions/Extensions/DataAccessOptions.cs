namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Extensions;

/// <summary>
/// ���ݷ���ѡ������
/// </summary>
public class DataAccessOptions
{
    /// <summary>
    /// Ĭ�ϳ�ʱʱ��
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Ĭ�����Դ���
    /// </summary>
    public int DefaultRetryCount { get; set; } = 3;

    /// <summary>
    /// Ĭ������������С
    /// </summary>
    public int DefaultBatchSize { get; set; } = 100;

    /// <summary>
    /// �Ƿ��������ܼ��
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// �Ƿ����ý������
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// ���������
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// ·�ɻ������ʱ��
    /// </summary>
    public TimeSpan RouteCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// �Ƿ�������ϸ��־
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}