namespace Lorn.ADSP.Core.Shared.Entities;

/// <summary>
/// 验证结果
/// </summary>
public sealed class ValidationResult : System.ComponentModel.DataAnnotations.ValidationResult
{
    private IReadOnlyList<ValidationError> errors = Array.Empty<ValidationError>();

    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool IsValid { get; init; }

    private IReadOnlyList<ValidationWarning> warnings = Array.Empty<ValidationWarning>();

    public ValidationResult(string? errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get => errors; init => errors = value?.ToList() ?? new List<ValidationError>(); }
    /// <summary>
    /// 警告信息列表
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get => warnings; init => warnings = value?.ToList() ?? new List<ValidationWarning>(); }

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
    public static new ValidationResult Success(string? summary = null, TimeSpan duration = default)
    {
        return new ValidationResult(summary)
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
        return new ValidationResult(summary)
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
        return new ValidationResult(summary)
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

    public void AddError(string v)
    {
        // 检查错误消息是否为空或空白
        if (string.IsNullOrWhiteSpace(v))
        {
            throw new ArgumentException("错误消息不能为空", nameof(v));
        }

        // 将错误消息添加到Errors列表中  
        var errorsList = Errors.ToList();
        errorsList.Add(new ValidationError { Message = v }); // 使用对象初始值设定项设置Message属性  
        errors = errorsList.AsReadOnly();
    }

    public void AddWarning(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
        {
            throw new ArgumentException("警告信息不能为空或空白。", nameof(v));
        }

        var warningsList = Warnings.ToList();
        warningsList.Add(new ValidationWarning { Message = v }); // 使用对象初始值设定项设置Message属性  
        warnings = warningsList.AsReadOnly();
    }

    public static ValidationResult Failure(List<string> errors)
    {
        // 将错误信息转换为ValidationError对象
        var validationErrors = errors.Select(error => new ValidationError { Message = error }).ToList();
        // 返回ValidationResult，标记为无效并包含错误信息
        return new ValidationResult(errors.FirstOrDefault())
        {
            IsValid = false,
            Errors = validationErrors
        };
    }

    public static ValidationResult Create(bool isValid, List<string> list)
    {
        if (isValid)
        {
            return Success(null, TimeSpan.Zero);
        }
        else
        {
            var validationErrors = list.Select(error => new ValidationError { Message = error }).ToList();
            return ValidationResult.Failure(validationErrors, "Validation failed.", TimeSpan.Zero);
        }
    }
}



