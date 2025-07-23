using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using System.Text.Json;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向上下文值对象
/// 通过ITargetingContext接口的集合来动态管理各种定向信息，以方便扩展
/// 符合数据模型设计中定向上下文的定义，支持多种上下文信息的动态封装
/// </summary>
public class TargetingContext : ValueObject
{
    /// <summary>
    /// 关联的请求ID
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// 定向上下文集合
    /// 支持多种ITargetingContext实现的集合管理
    /// </summary>
    public IReadOnlyList<ITargetingContext> TargetingContexts { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 上下文元数据属性集合
    /// </summary>
    public IReadOnlyList<ContextProperty> ContextMetadataProperties { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingContext(
        string requestId,
        IEnumerable<ITargetingContext>? targetingContexts,
        DateTime createdAt,
        IDictionary<string, object>? contextMetadata)
    {
        RequestId = requestId;
        TargetingContexts = targetingContexts?.ToList() ?? new List<ITargetingContext>();
        CreatedAt = createdAt;

        // 将元数据字典转换为 ContextProperty 集合
        ContextMetadataProperties = contextMetadata?.Select(kvp =>
            new ContextProperty(
                kvp.Key,
                JsonSerializer.Serialize(kvp.Value),
                kvp.Value?.GetType().Name ?? "String",
                "ContextMetadata",
                false,
                1.0m,
                null,
                "TargetingContext")
        ).ToList() ?? new List<ContextProperty>();
    }

    /// <summary>
    /// 创建定向上下文
    /// </summary>
    public static TargetingContext Create(
        string requestId,
        IEnumerable<ITargetingContext>? targetingContexts = null,
        IDictionary<string, object>? contextMetadata = null)
    {
        ValidateInputs(requestId);

        var createdAt = DateTime.UtcNow;

        return new TargetingContext(
            requestId,
            targetingContexts,
            createdAt,
            contextMetadata);
    }

    /// <summary>
    /// 从字典创建（兼容性方法）
    /// </summary>
    public static TargetingContext CreateFromDictionary(
        string requestId,
        IDictionary<string, ITargetingContext>? targetingContexts = null,
        IDictionary<string, object>? contextMetadata = null)
    {
        return Create(requestId, targetingContexts?.Values, contextMetadata);
    }

    /// <summary>
    /// 添加定向上下文
    /// </summary>
    public TargetingContext WithTargetingContext(ITargetingContext targetingContext)
    {
        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));

        // 移除同类型的现有上下文，添加新的
        var newContexts = TargetingContexts.Where(c => c.ContextType != targetingContext.ContextType)
                                          .Concat(new[] { targetingContext })
                                          .ToList();

        return new TargetingContext(
            RequestId,
            newContexts,
            CreatedAt,
            ContextMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 移除指定类型的定向上下文
    /// </summary>
    public TargetingContext WithoutTargetingContext(string contextType)
    {
        if (string.IsNullOrEmpty(contextType))
            return this;

        var newContexts = TargetingContexts.Where(c => c.ContextType != contextType).ToList();

        return new TargetingContext(
            RequestId,
            newContexts,
            CreatedAt,
            ContextMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 移除指定的定向上下文实例
    /// </summary>
    public TargetingContext WithoutTargetingContext(ITargetingContext targetingContext)
    {
        if (targetingContext == null)
            return this;

        var newContexts = TargetingContexts.Where(c => !ReferenceEquals(c, targetingContext)).ToList();

        return new TargetingContext(
            RequestId,
            newContexts,
            CreatedAt,
            ContextMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 获取指定类型的定向上下文
    /// </summary>
    public T? GetTargetingContext<T>() where T : class, ITargetingContext
    {
        return TargetingContexts.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// 获取指定类型名称的定向上下文
    /// </summary>
    public ITargetingContext? GetTargetingContext(string contextType)
    {
        if (string.IsNullOrEmpty(contextType))
            return null;

        return TargetingContexts.FirstOrDefault(ctx =>
            string.Equals(ctx.ContextType, contextType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取指定类型名称的定向上下文（泛型版本）
    /// </summary>
    public T? GetTargetingContext<T>(string contextType) where T : class, ITargetingContext
    {
        var context = GetTargetingContext(contextType);
        return context as T;
    }

    /// <summary>
    /// 是否包含指定类型的定向上下文
    /// </summary>
    public bool HasTargetingContextOfType<T>() where T : class, ITargetingContext
    {
        return TargetingContexts.OfType<T>().Any();
    }

    /// <summary>
    /// 是否包含指定类型名称的定向上下文
    /// </summary>
    public bool HasTargetingContextOfType(string contextType)
    {
        return GetTargetingContext(contextType) != null;
    }

    /// <summary>
    /// 获取所有定向上下文的类型
    /// </summary>
    public IReadOnlyList<string> GetTargetingContextTypes()
    {
        return TargetingContexts
            .Select(ctx => ctx.ContextType)
            .Distinct()
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// 获取上下文元数据
    /// </summary>
    public T? GetMetadata<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default;

        var contextProperty = ContextMetadataProperties.FirstOrDefault(p => p.PropertyKey == key);
        if (contextProperty == null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(contextProperty.PropertyValue);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 添加或更新上下文元数据
    /// </summary>
    public TargetingContext WithMetadata(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("元数据键不能为空", nameof(key));

        var newProperty = new ContextProperty(
            key,
            JsonSerializer.Serialize(value),
            value?.GetType().Name ?? "String",
            "ContextMetadata",
            false,
            1.0m,
            null,
            "TargetingContext");

        var newProperties = ContextMetadataProperties.Where(p => p.PropertyKey != key)
                                                    .Concat(new[] { newProperty })
                                                    .ToList();

        return new TargetingContext(
            RequestId,
            TargetingContexts,
            CreatedAt,
            newProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 移除上下文元数据
    /// </summary>
    public TargetingContext WithoutMetadata(string key)
    {
        if (string.IsNullOrEmpty(key))
            return this;

        var newProperties = ContextMetadataProperties.Where(p => p.PropertyKey != key).ToList();

        return new TargetingContext(
            RequestId,
            TargetingContexts,
            CreatedAt,
            newProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 是否为有效的定向上下文
    /// </summary>
    public bool IsValid()
    {
        return Id != Guid.Empty &&
               !string.IsNullOrEmpty(RequestId) &&
               TargetingContexts.Any();
    }

    /// <summary>
    /// 获取上下文摘要信息
    /// </summary>
    public Dictionary<string, object> GetSummary()
    {
        var contextTypeCounts = TargetingContexts
            .GroupBy(ctx => ctx.ContextType)
            .ToDictionary(g => g.Key, g => g.Count());

        return new Dictionary<string, object>
        {
            ["ContextId"] = Id,
            ["RequestId"] = RequestId,
            ["TargetingContextsCount"] = TargetingContexts.Count,
            ["TargetingContextTypes"] = GetTargetingContextTypes(),
            ["ContextTypeCounts"] = contextTypeCounts,
            ["MetadataKeysCount"] = ContextMetadataProperties.Count,
            ["CreatedAt"] = CreatedAt,
            ["IsValid"] = IsValid()
        };
    }

    /// <summary>
    /// 创建轻量级副本（仅包含指定的上下文类型）
    /// </summary>
    public TargetingContext CreateLightweightCopy(params string[] includeContextTypes)
    {
        if (includeContextTypes == null || includeContextTypes.Length == 0)
        {
            return this; // 返回当前实例，因为是不可变的
        }

        var filteredContexts = TargetingContexts
            .Where(ctx => includeContextTypes.Contains(ctx.ContextType, StringComparer.OrdinalIgnoreCase))
            .ToList();

        return new TargetingContext(
            RequestId,
            filteredContexts,
            CreatedAt,
            ContextMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 创建包含指定类型的轻量级副本
    /// </summary>
    public TargetingContext CreateLightweightCopyByTypes<T>() where T : class, ITargetingContext
    {
        var filteredContexts = TargetingContexts.OfType<T>().Cast<ITargetingContext>().ToList();

        return new TargetingContext(
            RequestId,
            filteredContexts,
            CreatedAt,
            ContextMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 合并另一个TargetingContext
    /// </summary>
    public TargetingContext Merge(TargetingContext other, bool overwriteExisting = false)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var mergedContexts = new List<ITargetingContext>(TargetingContexts);

        foreach (var context in other.TargetingContexts)
        {
            var existingIndex = mergedContexts.FindIndex(c => c.ContextType == context.ContextType);

            if (existingIndex >= 0)
            {
                if (overwriteExisting)
                {
                    mergedContexts[existingIndex] = context;
                }
            }
            else
            {
                mergedContexts.Add(context);
            }
        }

        var mergedMetadataProperties = new List<ContextProperty>(ContextMetadataProperties);

        foreach (var property in other.ContextMetadataProperties)
        {
            var existingIndex = mergedMetadataProperties.FindIndex(p => p.PropertyKey == property.PropertyKey);

            if (existingIndex >= 0)
            {
                if (overwriteExisting)
                {
                    mergedMetadataProperties[existingIndex] = property;
                }
            }
            else
            {
                mergedMetadataProperties.Add(property);
            }
        }

        return new TargetingContext(
            RequestId, // 保持原有的RequestId
            mergedContexts,
            CreatedAt, // 保持原有的创建时间
            mergedMetadataProperties.ToDictionary(p => p.PropertyKey, p => JsonSerializer.Deserialize<object>(p.PropertyValue) ?? p.PropertyValue)
        );
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return RequestId;
        yield return CreatedAt;

        // 上下文集合的内容哈希
        foreach (var context in TargetingContexts.OrderBy(ctx => ctx.ContextType))
        {
            yield return context.ContextId;
            yield return context.ContextType;
        }

        // 元数据的内容哈希
        foreach (var property in ContextMetadataProperties.OrderBy(p => p.PropertyKey))
        {
            yield return property.PropertyKey;
            yield return property.PropertyValue;
        }
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));
    }
}
