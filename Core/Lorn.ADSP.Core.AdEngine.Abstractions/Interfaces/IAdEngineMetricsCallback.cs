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
    /// <returns>上报结果</returns>
    Task ReportStrategyMetricsAsync(
        StrategyMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录错误信息
    /// </summary>
    /// <param name="error">错误信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordErrorAsync(
        StrategyError error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录性能数据
    /// </summary>
    /// <param name="performance">性能数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordPerformanceAsync(
        PerformanceMetrics performance,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录业务指标
    /// </summary>
    /// <param name="metrics">业务指标</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordBusinessMetricsAsync(
        BusinessMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录执行统计信息
    /// </summary>
    /// <param name="statistics">执行统计</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordExecutionStatisticsAsync(
        ExecutionStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量上报指标
    /// </summary>
    /// <param name="metricsBatch">指标批次</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task ReportMetricsBatchAsync(
        IReadOnlyList<MetricsBatch> metricsBatch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报告警事件
    /// </summary>
    /// <param name="alert">告警事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task ReportAlertAsync(
        AlertEvent alert,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录审计日志
    /// </summary>
    /// <param name="auditLog">审计日志</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordAuditLogAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增量上报指标数据
    /// </summary>
    /// <param name="metricName">指标名称</param>
    /// <param name="value">指标值</param>
    /// <param name="tags">标签字典</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task IncrementMetricAsync(
        string metricName,
        double value,
        IReadOnlyDictionary<string, string>? tags = null,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 计数器指标上报
    /// </summary>
    /// <param name="counterName">计数器名称</param>
    /// <param name="increment">增量值</param>
    /// <param name="tags">标签字典</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task IncrementCounterAsync(
        string counterName,
        long increment = 1,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直方图指标上报
    /// </summary>
    /// <param name="histogramName">直方图名称</param>
    /// <param name="value">测量值</param>
    /// <param name="tags">标签字典</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task RecordHistogramAsync(
        string histogramName,
        double value,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 计时器指标上报
    /// </summary>
    /// <param name="timerName">计时器名称</param>
    /// <param name="duration">持续时间</param>
    /// <param name="tags">标签字典</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上报结果</returns>
    Task RecordTimerAsync(
        string timerName,
        TimeSpan duration,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录自定义事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="properties">事件属性</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录结果</returns>
    Task RecordCustomEventAsync(
        string eventName,
        IReadOnlyDictionary<string, object> properties,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开始性能追踪
    /// </summary>
    /// <param name="operationName">操作名称</param>
    /// <param name="tags">标签字典</param>
    /// <returns>追踪上下文</returns>
    IPerformanceTracker StartPerformanceTracking(
        string operationName,
        IReadOnlyDictionary<string, string>? tags = null);
}

