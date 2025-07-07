using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ����ִ�н��
/// </summary>
public record StrategyResult
{
    /// <summary>
    /// ִ���Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// �����Ĺ���ѡ�б�
    /// </summary>
    public required IReadOnlyList<AdCandidate> ProcessedCandidates { get; init; }

    /// <summary>
    /// ����ִ��ͳ����Ϣ
    /// </summary>
    public required StrategyExecutionStatistics Statistics { get; init; }

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<StrategyError> Errors { get; init; } = Array.Empty<StrategyError>();

    /// <summary>
    /// Ԫ������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ִ��ID
    /// </summary>
    public string ExecutionId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ִ�п�ʼʱ��
    /// </summary>
    public DateTime StartTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ִ�н���ʱ��
    /// </summary>
    public DateTime EndTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// �����ɹ����
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
    /// ����ʧ�ܽ��
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
/// ����ִ��ͳ����Ϣ
/// </summary>
public record StrategyExecutionStatistics
{
    /// <summary>
    /// �����ѡ����
    /// </summary>
    public int InputCount { get; init; }

    /// <summary>
    /// �����ѡ����
    /// </summary>
    public int OutputCount { get; init; }

    /// <summary>
    /// ����ʱ�䣨���룩
    /// </summary>
    public double ProcessingTimeMs { get; init; }

    /// <summary>
    /// �ڴ�ʹ�������ֽڣ�
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// CPUʹ��ʱ�䣨���룩
    /// </summary>
    public double CpuTimeMs { get; init; }

    /// <summary>
    /// �������д���
    /// </summary>
    public int CacheHits { get; init; }

    /// <summary>
    /// ����δ���д���
    /// </summary>
    public int CacheMisses { get; init; }

    /// <summary>
    /// ���ݿ��ѯ����
    /// </summary>
    public int DatabaseQueries { get; init; }

    /// <summary>
    /// �ⲿ���ô���
    /// </summary>
    public int ExternalCalls { get; init; }

    /// <summary>
    /// �����ʣ���ѡ/�룩
    /// </summary>
    public double ProcessingRate => ProcessingTimeMs > 0 ? (InputCount * 1000.0) / ProcessingTimeMs : 0;

    /// <summary>
    /// ����������
    /// </summary>
    public double CacheHitRate => (CacheHits + CacheMisses) > 0 ? (double)CacheHits / (CacheHits + CacheMisses) : 0;

    /// <summary>
    /// ������
    /// </summary>
    public double FilterRate => InputCount > 0 ? (double)(InputCount - OutputCount) / InputCount : 0;

    /// <summary>
    /// ����ָ��
    /// </summary>
    public IReadOnlyDictionary<string, object> PerformanceMetrics { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// ���Դ�����Ϣ
/// </summary>
public record StrategyError
{
    /// <summary>
    /// �������
    /// </summary>
    public required string ErrorCode { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// ���󼶱�
    /// </summary>
    public ErrorLevel Level { get; init; } = ErrorLevel.Error;

    /// <summary>
    /// �쳣����
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ����������
    /// </summary>
    public IReadOnlyDictionary<string, object> Context { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ���Դ���
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// �Ƿ������
    /// </summary>
    public bool IsRetriable { get; init; } = true;

    /// <summary>
    /// ����������Ϣ
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
    /// ����������Ϣ
    /// </summary>
    public static StrategyError Warning(string errorCode, string errorMessage)
    {
        return Create(errorCode, errorMessage, ErrorLevel.Warning);
    }

    /// <summary>
    /// ������Ϣ��ʾ
    /// </summary>
    public static StrategyError Info(string errorCode, string errorMessage)
    {
        return Create(errorCode, errorMessage, ErrorLevel.Info);
    }
}