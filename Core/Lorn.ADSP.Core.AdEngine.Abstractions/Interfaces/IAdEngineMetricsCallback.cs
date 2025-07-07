using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Infrastructure.Monitoring.Models;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎监控上报回调接口
/// </summary>
public interface IAdEngineMetricsCallback : IAdEngineCallback
{
    /// <summary>
    /// 上报策略执行指标
    /// </summary>
    /// <param name="metrics">策略指标</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报任务</returns>
    Task ReportStrategyMetricsAsync(
        StrategyMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录错误信息
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录任务</returns>
    Task RecordErrorAsync(
        StrategyError error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录性能数据
    /// </summary>
    /// <param name="performance">性能数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录任务</returns>
    Task RecordPerformanceAsync(
        PerformanceMetrics performance,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录业务指标
    /// </summary>
    /// <param name="metrics">业务指标</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录任务</returns>
    Task RecordBusinessMetricsAsync(
        BusinessMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录执行统计信息
    /// </summary>
    /// <param name="statistics">执行统计</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录任务</returns>
    Task RecordExecutionStatisticsAsync(
        ExecutionStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量上报指标
    /// </summary>
    /// <param name="metricsBatch">指标批次</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报任务</returns>
    Task ReportMetricsBatchAsync(
        IReadOnlyList<MetricsBatch> metricsBatch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报告警事件
    /// </summary>
    /// <param name="alert">告警事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报任务</returns>
    Task ReportAlertAsync(
        AlertEvent alert,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录审计日志
    /// </summary>
    /// <param name="auditLog">审计日志</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录任务</returns>
    Task RecordAuditLogAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);
}