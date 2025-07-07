namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ��������
/// </summary>
public record StrategyConfig
{
    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ���ð汾
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// ִ�����ȼ�
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// ��ʱ���ã����룩
    /// </summary>
    public int TimeoutMs { get; init; } = 10000;

    /// <summary>
    /// ���Դ���
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// ���ò���
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// �����ض�����
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentConfig { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Ȩ������
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Weights { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// ��ֵ����
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Thresholds { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// ���Կ���
    /// </summary>
    public IReadOnlyDictionary<string, bool> FeatureFlags { get; init; } = new Dictionary<string, bool>();

    /// <summary>
    /// A/B��������
    /// </summary>
    public ABTestConfig? ABTestConfig { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ���ô�����
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// ���ø�����
    /// </summary>
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// ��ȡ����ֵ
    /// </summary>
    public T GetParameter<T>(string key, T defaultValue = default!)
    {
        if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// ��ȡȨ��ֵ
    /// </summary>
    public decimal GetWeight(string key, decimal defaultValue = 1.0m)
    {
        return Weights.TryGetValue(key, out var weight) ? weight : defaultValue;
    }

    /// <summary>
    /// ��ȡ��ֵ
    /// </summary>
    public decimal GetThreshold(string key, decimal defaultValue = 0.5m)
    {
        return Thresholds.TryGetValue(key, out var threshold) ? threshold : defaultValue;
    }

    /// <summary>
    /// ��ȡ���Կ���״̬
    /// </summary>
    public bool GetFeatureFlag(string key, bool defaultValue = false)
    {
        return FeatureFlags.TryGetValue(key, out var flag) ? flag : defaultValue;
    }
}

/// <summary>
/// A/B��������
/// </summary>
public record ABTestConfig
{
    /// <summary>
    /// ʵ��ID
    /// </summary>
    public required string ExperimentId { get; init; }

    /// <summary>
    /// ʵ������
    /// </summary>
    public required string ExperimentName { get; init; }

    /// <summary>
    /// ʵ��������
    /// </summary>
    public required IReadOnlyList<ExperimentGroup> Groups { get; init; }

    /// <summary>
    /// �����������
    /// </summary>
    public decimal TrafficAllocation { get; init; } = 1.0m;

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime? StartTime { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public string GroupingStrategy { get; init; } = "hash";

    /// <summary>
    /// �û�����
    /// </summary>
    public ExperimentGroup? GetUserGroup(string userId)
    {
        if (!IsEnabled || Groups.Count == 0)
            return null;

        var hash = Math.Abs(userId.GetHashCode());
        var totalWeight = Groups.Sum(g => g.Weight);
        var targetWeight = (hash % 100) * totalWeight / 100;

        decimal currentWeight = 0;
        foreach (var group in Groups)
        {
            currentWeight += group.Weight;
            if (targetWeight <= currentWeight)
                return group;
        }

        return Groups.Last();
    }
}

/// <summary>
/// ʵ����
/// </summary>
public record ExperimentGroup
{
    /// <summary>
    /// ��ID
    /// </summary>
    public required string GroupId { get; init; }

    /// <summary>
    /// ������
    /// </summary>
    public required string GroupName { get; init; }

    /// <summary>
    /// Ȩ�أ��ٷֱȣ�
    /// </summary>
    public decimal Weight { get; init; } = 50.0m;

    /// <summary>
    /// �����ò���
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// �Ƿ�Ϊ������
    /// </summary>
    public bool IsControl { get; init; } = false;
}

/// <summary>
/// ��֤���
/// </summary>
public record ValidationResult
{
    /// <summary>
    /// ��֤�Ƿ�ɹ�
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// ��֤������Ϣ
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; init; } = Array.Empty<ValidationError>();

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get; init; } = Array.Empty<ValidationWarning>();

    /// <summary>
    /// ��֤ʱ��
    /// </summary>
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// �����ɹ�����֤���
    /// </summary>
    public static ValidationResult Success(IReadOnlyList<ValidationWarning>? warnings = null)
    {
        return new ValidationResult
        {
            IsValid = true,
            Warnings = warnings ?? Array.Empty<ValidationWarning>()
        };
    }

    /// <summary>
    /// ����ʧ�ܵ���֤���
    /// </summary>
    public static ValidationResult Failure(IReadOnlyList<ValidationError> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
    }

    /// <summary>
    /// ���������������֤���
    /// </summary>
    public static ValidationResult Failure(string errorCode, string errorMessage)
    {
        return Failure(new[] { new ValidationError(errorCode, errorMessage) });
    }
}

/// <summary>
/// ��֤����
/// </summary>
public record ValidationError(string ErrorCode, string ErrorMessage)
{
    /// <summary>
    /// �����ֶ�
    /// </summary>
    public string? Field { get; init; }

    /// <summary>
    /// ����ֵ
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// �������س̶�
    /// </summary>
    public ErrorSeverity Severity { get; init; } = ErrorSeverity.Error;
}

/// <summary>
/// ��֤����
/// </summary>
public record ValidationWarning(string WarningCode, string WarningMessage)
{
    /// <summary>
    /// �����ֶ�
    /// </summary>
    public string? Field { get; init; }

    /// <summary>
    /// ����ֵ
    /// </summary>
    public object? Value { get; init; }
}

/// <summary>
/// �������س̶�
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// ��Ϣ
    /// </summary>
    Info = 1,

    /// <summary>
    /// ����
    /// </summary>
    Warning = 2,

    /// <summary>
    /// ����
    /// </summary>
    Error = 3,

    /// <summary>
    /// ��������
    /// </summary>
    Critical = 4
}