using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 策略执行结果
/// </summary>
public record StrategyResult
{
    /// <summary>
    /// 执行是否成功
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// 处理后的广告候选列表
    /// </summary>
    public required IReadOnlyList<AdCandidate> ProcessedCandidates { get; init; }

    /// <summary>
    /// 策略执行统计信息
    /// </summary>
    public required StrategyExecutionStatistics Statistics { get; init; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public IReadOnlyList<StrategyError> Errors { get; init; } = Array.Empty<StrategyError>();

    /// <summary>
    /// 元数据信息
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 执行ID
    /// </summary>
    public string ExecutionId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 策略ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// 执行开始时间
    /// </summary>
    public DateTime StartTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 执行结束时间
    /// </summary>
    public DateTime EndTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 执行时长
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static StrategyResult Success(
        string strategyId,
        IReadOnlyList<AdCandidate> candidates,
        StrategyExecutionStatistics statistics,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        return new StrategyResult
        {
            IsSuccess = true,
            StrategyId = strategyId,
            ProcessedCandidates = candidates,
            Statistics = statistics,
            Metadata = metadata ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static StrategyResult Failure(
        string strategyId,
        IReadOnlyList<StrategyError> errors,
        StrategyExecutionStatistics statistics,
        IReadOnlyList<AdCandidate>? candidates = null)
    {
        return new StrategyResult
        {
            IsSuccess = false,
            StrategyId = strategyId,
            ProcessedCandidates = candidates ?? Array.Empty<AdCandidate>(),
            Statistics = statistics,
            Errors = errors
        };
    }
}



