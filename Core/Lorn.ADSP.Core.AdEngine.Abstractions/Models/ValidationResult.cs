namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 验证结果
/// </summary>
public sealed class ValidationResult
{
    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; init; } = Array.Empty<ValidationError>();

    /// <summary>
    /// 警告信息列表
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get; init; } = Array.Empty<ValidationWarning>();

    /// <summary>
    /// 验证摘要信息
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// 验证上下文信息
    /// </summary>
    public IReadOnlyDictionary<string, object>? Context { get; init; }

    /// <summary>
    /// 验证耗时
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// 创建成功的验证结果
    /// </summary>
    public static ValidationResult Success(string? summary = null, TimeSpan duration = default)
    {
        return new ValidationResult
        {
            IsValid = true,
            Summary = summary,
            Duration = duration
        };
    }

    // Fix for CA1826: Use the collection directly instead of Enumerable methods
    public static ValidationResult Failure(
        IEnumerable<ValidationError> errors,
        string? summary = null,
        TimeSpan duration = default)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors is List<ValidationError> errorList ? errorList.AsReadOnly() : errors.ToList().AsReadOnly(),
            Summary = summary,
            Duration = duration
        };
    }

    /// <summary>
    /// 创建带警告的验证结果
    /// </summary>
    public static ValidationResult SuccessWithWarnings(
    IEnumerable<ValidationWarning> warnings,
    string? summary = null,
    TimeSpan duration = default)
    {
        return new ValidationResult
        {
            IsValid = true,
            Warnings = warnings is List<ValidationWarning> warningList ? warningList.AsReadOnly() : warnings.ToList().AsReadOnly(),
            Summary = summary,
            Duration = duration
        };
    }

    // Fix for CS1061: Ensure ValidationError has a Message property or use ErrorMessage
    public string? GetFirstErrorMessage()
    {
        return Errors.FirstOrDefault()?.Message; // Assuming ErrorMessage is the correct property
    }

    /// <summary>
    /// 获取所有错误信息
    /// </summary>
    public IEnumerable<string> GetAllErrorMessages()
    {
        return Errors.Select(e => e.Message); // Assuming Message is the correct property
    }

    /// <summary>
    /// 获取所有警告信息
    /// </summary>
    public IEnumerable<string> GetAllWarningMessages()
    {
        return Warnings.Select(w => w.Message);
    }
}



