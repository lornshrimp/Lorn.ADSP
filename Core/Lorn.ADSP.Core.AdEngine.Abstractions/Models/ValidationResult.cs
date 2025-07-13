namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ��֤���
/// </summary>
public sealed class ValidationResult
{
    /// <summary>
    /// �Ƿ���֤�ɹ�
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; init; } = Array.Empty<ValidationError>();

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get; init; } = Array.Empty<ValidationWarning>();

    /// <summary>
    /// ��֤ժҪ��Ϣ
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// ��֤��������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object>? Context { get; init; }

    /// <summary>
    /// ��֤��ʱ
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// �����ɹ�����֤���
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
    /// �������������֤���
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
    /// ��ȡ���д�����Ϣ
    /// </summary>
    public IEnumerable<string> GetAllErrorMessages()
    {
        return Errors.Select(e => e.Message); // Assuming Message is the correct property
    }

    /// <summary>
    /// ��ȡ���о�����Ϣ
    /// </summary>
    public IEnumerable<string> GetAllWarningMessages()
    {
        return Warnings.Select(w => w.Message);
    }
}



