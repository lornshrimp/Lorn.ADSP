using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Shared.Entities;

/// <summary>
/// 验证错误
/// </summary>
public sealed record ValidationError
{
    /// <summary>
    /// 错误消息
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// 错误字段
    /// </summary>
    public string? Field { get; init; }

    /// <summary>
    /// 错误值
    /// </summary>
    public object? AttemptedValue { get; init; }

    /// <summary>
    /// 错误级别
    /// </summary>
    public ValidationSeverity Severity { get; init; } = ValidationSeverity.Error;

    /// <summary>
    /// 错误的内部异常
    /// </summary>
    public Exception? InnerException { get; init; }

    /// <summary>
    /// 错误的附加上下文信息
    /// </summary>
    public IReadOnlyDictionary<string, object>? Context { get; init; }

    /// <summary>
    /// 错误发生的位置信息
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// 错误的时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ValidationError()
    {
    }

    /// <summary>
    /// 使用消息创建验证错误
    /// </summary>
    /// <param name="message">错误消息</param>
    public ValidationError(string message)
    {
        Message = message;
    }

    /// <summary>
    /// 使用消息和代码创建验证错误
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误代码</param>
    public ValidationError(string message, string code)
    {
        Message = message;
        Code = code;
    }

    /// <summary>
    /// 创建字段级别的验证错误
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="field">字段名称</param>
    /// <param name="attemptedValue">尝试设置的值</param>
    /// <returns>验证错误</returns>
    public static ValidationError ForField(string message, string field, object? attemptedValue = null)
    {
        return new ValidationError
        {
            Message = message,
            Field = field,
            AttemptedValue = attemptedValue,
            Code = $"FIELD_{field.ToUpperInvariant()}_INVALID"
        };
    }

    /// <summary>
    /// 创建必填字段错误
    /// </summary>
    /// <param name="field">字段名称</param>
    /// <returns>验证错误</returns>
    public static ValidationError RequiredField(string field)
    {
        return new ValidationError
        {
            Message = $"{field} is required",
            Field = field,
            Code = $"FIELD_{field.ToUpperInvariant()}_REQUIRED",
            Severity = ValidationSeverity.Error
        };
    }

    /// <summary>
    /// 创建无效值错误
    /// </summary>
    /// <param name="field">字段名称</param>
    /// <param name="attemptedValue">尝试设置的值</param>
    /// <param name="expectedFormat">期望的格式</param>
    /// <returns>验证错误</returns>
    public static ValidationError InvalidValue(string field, object? attemptedValue, string? expectedFormat = null)
    {
        var message = expectedFormat != null
            ? $"{field} has invalid value. Expected format: {expectedFormat}"
            : $"{field} has invalid value";

        return new ValidationError
        {
            Message = message,
            Field = field,
            AttemptedValue = attemptedValue,
            Code = $"FIELD_{field.ToUpperInvariant()}_INVALID_VALUE"
        };
    }

    /// <summary>
    /// 创建范围错误
    /// </summary>
    /// <param name="field">字段名称</param>
    /// <param name="attemptedValue">尝试设置的值</param>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns>验证错误</returns>
    public static ValidationError OutOfRange(string field, object? attemptedValue, object? minValue = null, object? maxValue = null)
    {
        var rangeText = (minValue, maxValue) switch
        {
            (not null, not null) => $"between {minValue} and {maxValue}",
            (not null, null) => $"greater than or equal to {minValue}",
            (null, not null) => $"less than or equal to {maxValue}",
            _ => "within valid range"
        };

        return new ValidationError
        {
            Message = $"{field} must be {rangeText}",
            Field = field,
            AttemptedValue = attemptedValue,
            Code = $"FIELD_{field.ToUpperInvariant()}_OUT_OF_RANGE",
            Context = new Dictionary<string, object>
            {
                ["MinValue"] = minValue ?? "N/A",
                ["MaxValue"] = maxValue ?? "N/A"
            }.AsReadOnly()
        };
    }

    /// <summary>
    /// 创建自定义错误
    /// </summary>
    /// <param name="code">错误代码</param>
    /// <param name="message">错误消息</param>
    /// <param name="severity">错误级别</param>
    /// <param name="context">上下文信息</param>
    /// <returns>验证错误</returns>
    public static ValidationError Custom(
        string code,
        string message,
        ValidationSeverity severity = ValidationSeverity.Error,
        IReadOnlyDictionary<string, object>? context = null)
    {
        return new ValidationError
        {
            Code = code,
            Message = message,
            Severity = severity,
            Context = context
        };
    }

    /// <summary>
    /// 从异常创建验证错误
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="field">相关字段</param>
    /// <returns>验证错误</returns>
    public static ValidationError FromException(Exception exception, string? field = null)
    {
        return new ValidationError
        {
            Message = exception.Message,
            Field = field,
            InnerException = exception,
            Code = "VALIDATION_EXCEPTION",
            Severity = ValidationSeverity.Error,
            Location = exception.StackTrace?.Split('\n').FirstOrDefault()?.Trim()
        };
    }

    /// <summary>
    /// 转换为字符串表示
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(Code))
            parts.Add($"[{Code}]");

        parts.Add(Message);

        if (!string.IsNullOrEmpty(Field))
            parts.Add($"(Field: {Field})");

        if (AttemptedValue != null)
            parts.Add($"(Value: {AttemptedValue})");

        return string.Join(" ", parts);
    }
}




