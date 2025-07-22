using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 动态参数值对象
/// 定向配置中的参数项，描述定向策略的具体配置信息
/// 设计原则：不可变、基于值相等、作为TargetingConfig的组成部分
/// </summary>
public class DynamicParameter : ValueObject
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// 参数值（JSON格式）
    /// </summary>
    public string ParameterValue { get; }

    /// <summary>
    /// 参数类型（String, Number, Boolean, Array, Object等）
    /// </summary>
    public string ParameterType { get; }

    /// <summary>
    /// 参数来源（Manual, Algorithm, System等）
    /// </summary>
    public string ParameterSource { get; }

    /// <summary>
    /// 是否为系统参数
    /// </summary>
    public bool IsSystemParameter { get; }

    /// <summary>
    /// 参数描述
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 参数优先级（用于冲突解决）
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// 参数有效期
    /// </summary>
    public DateTime? ExpiresAt { get; }

    /// <summary>
    /// 私有构造函数，强制使用工厂方法创建
    /// </summary>
    private DynamicParameter(
        string parameterName,
        string parameterValue,
        string parameterType,
        string parameterSource,
        bool isSystemParameter,
        string description,
        int priority,
        DateTime? expiresAt)
    {
        ParameterName = parameterName;
        ParameterValue = parameterValue;
        ParameterType = parameterType;
        ParameterSource = parameterSource;
        IsSystemParameter = isSystemParameter;
        Description = description;
        Priority = priority;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// 创建基础动态参数
    /// </summary>
    public static DynamicParameter Create(
        string parameterName,
        string parameterValue,
        string parameterType = "String",
        string description = "")
    {
        ValidateBasicParameters(parameterName, parameterValue, parameterType);

        return new DynamicParameter(
            parameterName,
            parameterValue,
            parameterType,
            "Manual",
            false,
            description ?? string.Empty,
            0,
            null);
    }

    /// <summary>
    /// 创建系统参数
    /// </summary>
    public static DynamicParameter CreateSystemParameter(
        string parameterName,
        string parameterValue,
        string parameterType = "String",
        string description = "",
        int priority = 100)
    {
        ValidateBasicParameters(parameterName, parameterValue, parameterType);

        return new DynamicParameter(
            parameterName,
            parameterValue,
            parameterType,
            "System",
            true,
            description ?? string.Empty,
            priority,
            null);
    }

    /// <summary>
    /// 创建算法生成的参数
    /// </summary>
    public static DynamicParameter CreateAlgorithmParameter(
        string parameterName,
        string parameterValue,
        string parameterType = "String",
        string description = "",
        DateTime? expiresAt = null,
        int priority = 50)
    {
        ValidateBasicParameters(parameterName, parameterValue, parameterType);

        return new DynamicParameter(
            parameterName,
            parameterValue,
            parameterType,
            "Algorithm",
            false,
            description ?? string.Empty,
            priority,
            expiresAt);
    }

    /// <summary>
    /// 从强类型对象创建参数
    /// </summary>
    public static DynamicParameter FromObject<T>(
        string parameterName,
        T value,
        string description = "",
        string parameterSource = "Manual")
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("参数名称不能为空", nameof(parameterName));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var jsonValue = System.Text.Json.JsonSerializer.Serialize(value);
        var typeName = typeof(T).Name;

        return new DynamicParameter(
            parameterName,
            jsonValue,
            typeName,
            parameterSource,
            parameterSource == "System",
            description ?? string.Empty,
            0,
            null);
    }

    /// <summary>
    /// 获取强类型参数值
    /// </summary>
    public T? GetValue<T>()
    {
        if (string.IsNullOrWhiteSpace(ParameterValue))
            return default(T);

        try
        {
            // 对于基础类型，直接转换
            if (typeof(T) == typeof(string))
                return (T)(object)ParameterValue;

            if (typeof(T) == typeof(int) && int.TryParse(ParameterValue, out var intValue))
                return (T)(object)intValue;

            if (typeof(T) == typeof(decimal) && decimal.TryParse(ParameterValue, out var decimalValue))
                return (T)(object)decimalValue;

            if (typeof(T) == typeof(bool) && bool.TryParse(ParameterValue, out var boolValue))
                return (T)(object)boolValue;

            if (typeof(T) == typeof(DateTime) && DateTime.TryParse(ParameterValue, out var dateValue))
                return (T)(object)dateValue;

            // 对于复杂类型，尝试JSON反序列化
            return System.Text.Json.JsonSerializer.Deserialize<T>(ParameterValue);
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// 获取参数值作为字符串
    /// </summary>
    public string GetStringValue()
    {
        return ParameterValue;
    }

    /// <summary>
    /// 检查参数是否已过期
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// 检查是否为指定类型
    /// </summary>
    public bool IsType(string parameterType)
    {
        return string.Equals(ParameterType, parameterType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查是否为指定来源
    /// </summary>
    public bool IsSource(string parameterSource)
    {
        return string.Equals(ParameterSource, parameterSource, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 创建更新值的副本
    /// </summary>
    public DynamicParameter WithValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("参数值不能为空", nameof(newValue));

        return new DynamicParameter(
            ParameterName,
            newValue,
            ParameterType,
            ParameterSource,
            IsSystemParameter,
            Description,
            Priority,
            ExpiresAt);
    }

    /// <summary>
    /// 创建更新描述的副本
    /// </summary>
    public DynamicParameter WithDescription(string newDescription)
    {
        return new DynamicParameter(
            ParameterName,
            ParameterValue,
            ParameterType,
            ParameterSource,
            IsSystemParameter,
            newDescription ?? string.Empty,
            Priority,
            ExpiresAt);
    }

    /// <summary>
    /// 创建更新优先级的副本
    /// </summary>
    public DynamicParameter WithPriority(int newPriority)
    {
        return new DynamicParameter(
            ParameterName,
            ParameterValue,
            ParameterType,
            ParameterSource,
            IsSystemParameter,
            Description,
            newPriority,
            ExpiresAt);
    }

    /// <summary>
    /// 创建设置过期时间的副本
    /// </summary>
    public DynamicParameter WithExpiry(DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("过期时间必须是未来时间", nameof(expiresAt));

        return new DynamicParameter(
            ParameterName,
            ParameterValue,
            ParameterType,
            ParameterSource,
            IsSystemParameter,
            Description,
            Priority,
            expiresAt);
    }

    /// <summary>
    /// 创建移除过期时间的副本
    /// </summary>
    public DynamicParameter WithoutExpiry()
    {
        return new DynamicParameter(
            ParameterName,
            ParameterValue,
            ParameterType,
            ParameterSource,
            IsSystemParameter,
            Description,
            Priority,
            null);
    }

    /// <summary>
    /// 验证参数的有效性
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ParameterName) &&
               !string.IsNullOrWhiteSpace(ParameterValue) &&
               !string.IsNullOrWhiteSpace(ParameterType) &&
               !string.IsNullOrWhiteSpace(ParameterSource) &&
               !IsExpired();
    }

    /// <summary>
    /// 检查是否与另一个参数冲突
    /// </summary>
    public bool ConflictsWith(DynamicParameter other)
    {
        if (other == null)
            return false;

        // 同名参数但值不同视为冲突
        return string.Equals(ParameterName, other.ParameterName, StringComparison.OrdinalIgnoreCase) &&
               !string.Equals(ParameterValue, other.ParameterValue, StringComparison.Ordinal);
    }

    /// <summary>
    /// 解决与另一个参数的冲突（优先级高者胜出）
    /// </summary>
    public DynamicParameter ResolveConflictWith(DynamicParameter other)
    {
        if (other == null)
            return this;

        if (!ConflictsWith(other))
            return this;

        // 系统参数优先级最高
        if (IsSystemParameter && !other.IsSystemParameter)
            return this;

        if (!IsSystemParameter && other.IsSystemParameter)
            return other;

        // 比较数值优先级
        if (Priority > other.Priority)
            return this;

        if (Priority < other.Priority)
            return other;

        // 优先级相同时，选择非过期的
        if (IsExpired() && !other.IsExpired())
            return other;

        if (!IsExpired() && other.IsExpired())
            return this;

        // 都相同时返回当前参数
        return this;
    }

    /// <summary>
    /// 获取参数的摘要信息
    /// </summary>
    public string GetSummary()
    {
        var valueDisplay = ParameterValue.Length > 50
            ? ParameterValue.Substring(0, 47) + "..."
            : ParameterValue;

        var sourcePrefix = IsSystemParameter ? "[SYS]" : $"[{ParameterSource}]";
        var priorityInfo = Priority != 0 ? $" (P:{Priority})" : string.Empty;
        var expiryInfo = ExpiresAt.HasValue ? $" (expires: {ExpiresAt:yyyy-MM-dd HH:mm})" : string.Empty;

        return $"{sourcePrefix} {ParameterName}: {valueDisplay} [{ParameterType}]{priorityInfo}{expiryInfo}";
    }

    /// <summary>
    /// 基础参数验证
    /// </summary>
    private static void ValidateBasicParameters(string parameterName, string parameterValue, string parameterType)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("参数名称不能为空", nameof(parameterName));

        if (string.IsNullOrWhiteSpace(parameterValue))
            throw new ArgumentException("参数值不能为空", nameof(parameterValue));

        if (string.IsNullOrWhiteSpace(parameterType))
            throw new ArgumentException("参数类型不能为空", nameof(parameterType));
    }

    /// <summary>
    /// 实现值对象的相等性比较组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ParameterName;
        yield return ParameterValue;
        yield return ParameterType;
        yield return ParameterSource;
        yield return IsSystemParameter;
        yield return Description;
        yield return Priority;
        yield return ExpiresAt?.Ticks ?? 0;
    }
}
