using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 策略执行错误信息
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
    public required string Message { get; init; }

    /// <summary>
    /// 错误级别
    /// </summary>
    public ErrorLevel Level { get; init; } = ErrorLevel.Error;

    /// <summary>
    /// 错误发生时间
    /// </summary>
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 错误来源（组件名称）
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// 错误分类
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// 详细错误信息
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// 异常堆栈跟踪
    /// </summary>
    public string? StackTrace { get; init; }

    /// <summary>
    /// 内部异常信息
    /// </summary>
    public string? InnerException { get; init; }

    /// <summary>
    /// 错误上下文信息
    /// </summary>
    public IReadOnlyDictionary<string, object> Context { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// 是否为可重试错误
    /// </summary>
    public bool IsRetryable { get; init; } = false;

    /// <summary>
    /// 建议的重试延迟时间
    /// </summary>
    public TimeSpan? SuggestedRetryDelay { get; init; }

    /// <summary>
    /// 错误影响范围
    /// </summary>
    public ErrorImpact Impact { get; init; } = ErrorImpact.Minor;

    /// <summary>
    /// 创建策略错误
    /// </summary>
    /// <param name="errorCode">错误代码</param>
    /// <param name="message">错误消息</param>
    /// <param name="level">错误级别</param>
    /// <param name="source">错误来源</param>
    /// <param name="category">错误分类</param>
    /// <param name="details">详细信息</param>
    /// <param name="context">上下文信息</param>
    /// <returns>策略错误实例</returns>
    public static StrategyError Create(
        string errorCode,
        string message,
        ErrorLevel level = ErrorLevel.Error,
        string? source = null,
        string? category = null,
        string? details = null,
        IReadOnlyDictionary<string, object>? context = null)
    {
        return new StrategyError
        {
            ErrorCode = errorCode,
            Message = message,
            Level = level,
            Source = source,
            Category = category,
            Details = details,
            Context = context ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// 从异常创建策略错误
    /// </summary>
    /// <param name="errorCode">错误代码</param>
    /// <param name="exception">异常对象</param>
    /// <param name="level">错误级别</param>
    /// <param name="source">错误来源</param>
    /// <param name="category">错误分类</param>
    /// <param name="context">上下文信息</param>
    /// <returns>策略错误实例</returns>
    public static StrategyError FromException(
        string errorCode,
        Exception exception,
        ErrorLevel level = ErrorLevel.Error,
        string? source = null,
        string? category = null,
        IReadOnlyDictionary<string, object>? context = null)
    {
        return new StrategyError
        {
            ErrorCode = errorCode,
            Message = exception.Message,
            Level = level,
            Source = source,
            Category = category,
            Details = exception.ToString(),
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.ToString(),
            Context = context ?? new Dictionary<string, object>(),
            IsRetryable = IsRetryableException(exception)
        };
    }

    /// <summary>
    /// 判断异常是否可重试
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <returns>是否可重试</returns>
    private static bool IsRetryableException(Exception exception)
    {
        return exception switch
        {
            TimeoutException => true,
            HttpRequestException => true,
            TaskCanceledException => true,
            OperationCanceledException => false,
            ArgumentNullException => false,
            ArgumentException => false,
            InvalidOperationException => false,
            _ => false
        };
    }

    /// <summary>
    /// 获取错误的简短描述
    /// </summary>
    /// <returns>简短描述</returns>
    public string GetShortDescription()
    {
        var sourceInfo = !string.IsNullOrEmpty(Source) ? $"[{Source}] " : "";
        return $"{sourceInfo}{ErrorCode}: {Message}";
    }

    /// <summary>
    /// 获取错误的完整描述
    /// </summary>
    /// <returns>完整描述</returns>
    public string GetFullDescription()
    {
        var description = GetShortDescription();

        if (!string.IsNullOrEmpty(Details))
        {
            description += $"\n详细信息: {Details}";
        }

        if (Context.Any())
        {
            var contextInfo = string.Join(", ", Context.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            description += $"\n上下文: {contextInfo}";
        }

        return description;
    }
}

/// <summary>
/// 错误影响范围
/// </summary>
public enum ErrorImpact
{
    /// <summary>
    /// 轻微影响
    /// </summary>
    Minor,

    /// <summary>
    /// 中等影响
    /// </summary>
    Moderate,

    /// <summary>
    /// 严重影响
    /// </summary>
    Major,

    /// <summary>
    /// 关键影响
    /// </summary>
    Critical
}