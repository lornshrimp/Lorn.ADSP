using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Infrastructure.Monitoring.Models;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ����������ϱ��ص��ӿ�
/// </summary>
public interface IAdEngineMetricsCallback : IAdEngineCallback
{
    /// <summary>
    /// �ϱ�����ִ��ָ��
    /// </summary>
    /// <param name="metrics">����ָ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ�����</returns>
    Task ReportStrategyMetricsAsync(
        StrategyMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼������Ϣ
    /// </summary>
    /// <param name="error">������Ϣ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼����</returns>
    Task RecordErrorAsync(
        StrategyError error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼��������
    /// </summary>
    /// <param name="performance">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼����</returns>
    Task RecordPerformanceAsync(
        PerformanceMetrics performance,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼ҵ��ָ��
    /// </summary>
    /// <param name="metrics">ҵ��ָ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼����</returns>
    Task RecordBusinessMetricsAsync(
        BusinessMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼ִ��ͳ����Ϣ
    /// </summary>
    /// <param name="statistics">ִ��ͳ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼����</returns>
    Task RecordExecutionStatisticsAsync(
        ExecutionStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �����ϱ�ָ��
    /// </summary>
    /// <param name="metricsBatch">ָ������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ�����</returns>
    Task ReportMetricsBatchAsync(
        IReadOnlyList<MetricsBatch> metricsBatch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �ϱ��澯�¼�
    /// </summary>
    /// <param name="alert">�澯�¼�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ�����</returns>
    Task ReportAlertAsync(
        AlertEvent alert,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼�����־
    /// </summary>
    /// <param name="auditLog">�����־</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼����</returns>
    Task RecordAuditLogAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);
}