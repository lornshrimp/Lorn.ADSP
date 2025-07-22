using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 请求上下文项值对象
/// 存储广告候选处理过程中的临时上下文信息
/// 设计原则：不可变、基于值相等、临时存在（请求期间使用）
/// </summary>
public class RequestContextItem : ValueObject
{
    /// <summary>
    /// 上下文键
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 上下文值（JSON格式存储）
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; }

    /// <summary>
    /// 类别（用于组织相关的上下文项）
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// 是否敏感信息
    /// </summary>
    public bool IsSensitive { get; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpiresAt { get; }

    /// <summary>
    /// 私有构造函数，强制使用工厂方法创建
    /// </summary>
    private RequestContextItem(
        string key,
        string value,
        string dataType,
        string category,
        bool isSensitive,
        DateTime? expiresAt)
    {
        Key = key;
        Value = value;
        DataType = dataType;
        Category = category;
        IsSensitive = isSensitive;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// 创建基础上下文项
    /// </summary>
    public static RequestContextItem Create(
        string key,
        string value,
        string dataType = "String",
        string category = "General")
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return new RequestContextItem(
            key,
            value,
            dataType ?? "String",
            category ?? "General",
            false,
            null);
    }

    /// <summary>
    /// 创建敏感信息上下文项
    /// </summary>
    public static RequestContextItem CreateSensitive(
        string key,
        string value,
        string dataType = "String",
        string category = "Security",
        DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return new RequestContextItem(
            key,
            value,
            dataType ?? "String",
            category ?? "Security",
            true,
            expiresAt);
    }

    /// <summary>
    /// 创建有过期时间的上下文项
    /// </summary>
    public static RequestContextItem CreateWithExpiry(
        string key,
        string value,
        DateTime expiresAt,
        string dataType = "String",
        string category = "Temporary")
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("过期时间必须是未来时间", nameof(expiresAt));

        return new RequestContextItem(
            key,
            value,
            dataType ?? "String",
            category ?? "Temporary",
            false,
            expiresAt);
    }

    /// <summary>
    /// 从对象创建上下文项（自动序列化为JSON）
    /// </summary>
    public static RequestContextItem FromObject(
        string key,
        object obj,
        string category = "Object")
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var value = System.Text.Json.JsonSerializer.Serialize(obj);
        var dataType = obj.GetType().Name;

        return new RequestContextItem(
            key,
            value,
            dataType,
            category ?? "Object",
            false,
            null);
    }

    /// <summary>
    /// 获取强类型值
    /// </summary>
    public T? GetValue<T>()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return default(T);

        try
        {
            // 对于基础类型，直接转换
            if (typeof(T) == typeof(string))
                return (T)(object)Value;

            if (typeof(T) == typeof(int) && int.TryParse(Value, out var intValue))
                return (T)(object)intValue;

            if (typeof(T) == typeof(decimal) && decimal.TryParse(Value, out var decimalValue))
                return (T)(object)decimalValue;

            if (typeof(T) == typeof(bool) && bool.TryParse(Value, out var boolValue))
                return (T)(object)boolValue;

            if (typeof(T) == typeof(DateTime) && DateTime.TryParse(Value, out var dateValue))
                return (T)(object)dateValue;

            // 对于复杂类型，尝试JSON反序列化
            return System.Text.Json.JsonSerializer.Deserialize<T>(Value);
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// 获取值作为字符串
    /// </summary>
    public string GetStringValue()
    {
        return IsSensitive ? "***SENSITIVE***" : Value;
    }

    /// <summary>
    /// 检查是否已过期
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// 检查是否为指定类别
    /// </summary>
    public bool IsCategory(string category)
    {
        return string.Equals(Category, category, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查是否为指定数据类型
    /// </summary>
    public bool IsDataType(string dataType)
    {
        return string.Equals(DataType, dataType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 创建非敏感版本的副本
    /// </summary>
    public RequestContextItem ToNonSensitive()
    {
        if (!IsSensitive)
            return this;

        return new RequestContextItem(
            Key,
            "***REDACTED***",
            DataType,
            Category,
            false, // 设置为非敏感
            ExpiresAt);
    }

    /// <summary>
    /// 创建延长过期时间的副本
    /// </summary>
    public RequestContextItem WithExtendedExpiry(DateTime newExpiresAt)
    {
        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("新的过期时间必须是未来时间", nameof(newExpiresAt));

        return new RequestContextItem(
            Key,
            Value,
            DataType,
            Category,
            IsSensitive,
            newExpiresAt);
    }

    /// <summary>
    /// 创建更新值的副本
    /// </summary>
    public RequestContextItem WithValue(string newValue)
    {
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        return new RequestContextItem(
            Key,
            newValue,
            DataType,
            Category,
            IsSensitive,
            ExpiresAt);
    }

    /// <summary>
    /// 验证上下文项的有效性
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Key) &&
               Value != null &&
               !string.IsNullOrWhiteSpace(DataType) &&
               !string.IsNullOrWhiteSpace(Category) &&
               !IsExpired();
    }

    /// <summary>
    /// 获取上下文项的摘要信息
    /// </summary>
    public string GetSummary()
    {
        var valueDisplay = IsSensitive ? "***SENSITIVE***" :
                          Value.Length > 50 ? Value.Substring(0, 47) + "..." : Value;

        var expiryInfo = ExpiresAt.HasValue ? $" (expires: {ExpiresAt:yyyy-MM-dd HH:mm})" : string.Empty;

        return $"{Category}.{Key}: {valueDisplay} [{DataType}]{expiryInfo}";
    }

    /// <summary>
    /// 实现值对象的相等性比较组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
        yield return Value;
        yield return DataType;
        yield return Category;
        yield return IsSensitive;
        yield return ExpiresAt?.Ticks ?? 0;
    }
}
