using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Shared.Enums;

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

/// <summary>
/// 策略执行统计信息
/// </summary>
public record StrategyExecutionStatistics
{
    /// <summary>
    /// 输入候选数量
    /// </summary>
    public int InputCount { get; init; }

    /// <summary>
    /// 输出候选数量
    /// </summary>
    public int OutputCount { get; init; }

    /// <summary>
    /// 处理时间（毫秒）
    /// </summary>
    public double ProcessingTimeMs { get; init; }

    /// <summary>
    /// 内存使用量（字节）
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// CPU使用时间（毫秒）
    /// </summary>
    public double CpuTimeMs { get; init; }

    /// <summary>
    /// 缓存命中次数
    /// </summary>
    public int CacheHits { get; init; }

    /// <summary>
    /// 缓存未命中次数
    /// </summary>
    public int CacheMisses { get; init; }

    /// <summary>
    /// 数据库查询次数
    /// </summary>
    public int DatabaseQueries { get; init; }

    /// <summary>
    /// 外部调用次数
    /// </summary>
    public int ExternalCalls { get; init; }

    /// <summary>
    /// 处理率（候选/秒）
    /// </summary>
    public double ProcessingRate => ProcessingTimeMs > 0 ? (InputCount * 1000.0) / ProcessingTimeMs : 0;

    /// <summary>
    /// 缓存命中率
    /// </summary>
    public double CacheHitRate => (CacheHits + CacheMisses) > 0 ? (double)CacheHits / (CacheHits + CacheMisses) : 0;

    /// <summary>
    /// 过滤率
    /// </summary>
    public double FilterRate => InputCount > 0 ? (double)(InputCount - OutputCount) / InputCount : 0;

    /// <summary>
    /// 性能指标
    /// </summary>
    public IReadOnlyDictionary<string, object> PerformanceMetrics { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// 策略错误信息
/// </summary>
public record StrategyError
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public required string ErrorCode { get; init; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// 错误级别
    /// </summary>
    public ErrorLevel Level { get; init; } = ErrorLevel.Error;

    /// <summary>
    /// 异常详情
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// 错误发生时间
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 错误上下文
    /// </summary>
    public IReadOnlyDictionary<string, object> Context { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// 是否可重试
    /// </summary>
    public bool IsRetriable { get; init; } = true;

    /// <summary>
    /// 创建错误信息
    /// </summary>
    public static StrategyError Create(
        string errorCode,
        string errorMessage,
        ErrorLevel level = ErrorLevel.Error,
        Exception? exception = null,
        IReadOnlyDictionary<string, object>? context = null)
    {
        return new StrategyError
        {
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            Level = level,
            Exception = exception,
            Context = context ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// 创建警告信息
    /// </summary>
    public static StrategyError Warning(string errorCode, string errorMessage)
    {
        return Create(errorCode, errorMessage, ErrorLevel.Warning);
    }

    /// <summary>
    /// 创建信息提示
    /// </summary>
    public static StrategyError Info(string errorCode, string errorMessage)
    {
        return Create(errorCode, errorMessage, ErrorLevel.Info);
    }
}