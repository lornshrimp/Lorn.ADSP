using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向上下文值对象
/// 通过ITargetingContext接口的字典来动态管理各种定向信息，以方便扩展
/// 符合数据模型设计中定向上下文的定义，支持多种上下文信息的动态封装
/// </summary>
public class TargetingContext : ValueObject
{
    /// <summary>
    /// 上下文唯一标识
    /// </summary>
    public string ContextId { get; private set; }

    /// <summary>
    /// 关联的请求ID
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// 定向上下文集合
    /// 支持多种ITargetingContext实现的动态字典管理
    /// </summary>
    private readonly Dictionary<string, ITargetingContext> _targetingContexts = new();
    public IReadOnlyDictionary<string, ITargetingContext> TargetingContexts => _targetingContexts.AsReadOnly();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 上下文元数据
    /// </summary>
    public IReadOnlyDictionary<string, object> ContextMetadata { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingContext(
        string contextId,
        string requestId,
        IDictionary<string, ITargetingContext>? targetingContexts,
        DateTime createdAt,
        IDictionary<string, object>? contextMetadata)
    {
        ContextId = contextId;
        RequestId = requestId;

        if (targetingContexts != null)
        {
            foreach (var context in targetingContexts)
            {
                _targetingContexts[context.Key] = context.Value;
            }
        }

        CreatedAt = createdAt;
        ContextMetadata = contextMetadata?.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly() ??
                          new Dictionary<string, object>().AsReadOnly();
    }

    /// <summary>
    /// 创建定向上下文
    /// </summary>
    public static TargetingContext Create(
        string requestId,
        IDictionary<string, ITargetingContext>? targetingContexts = null,
        IDictionary<string, object>? contextMetadata = null)
    {
        ValidateInputs(requestId);

        var contextId = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow;

        return new TargetingContext(
            contextId,
            requestId,
            targetingContexts,
            createdAt,
            contextMetadata);
    }

    /// <summary>
    /// 添加定向上下文
    /// </summary>
    public TargetingContext AddTargetingContext(string key, ITargetingContext targetingContext)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));

        var newContexts = new Dictionary<string, ITargetingContext>(_targetingContexts)
        {
            [key] = targetingContext
        };

        return new TargetingContext(
            ContextId,
            RequestId,
            newContexts,
            CreatedAt,
            ContextMetadata.ToDictionary(kv => kv.Key, kv => kv.Value)
        );
    }

    /// <summary>
    /// 移除定向上下文
    /// </summary>
    public TargetingContext RemoveTargetingContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("上下文键不能为空", nameof(key));

        var newContexts = new Dictionary<string, ITargetingContext>(_targetingContexts);
        newContexts.Remove(key);

        return new TargetingContext(
            ContextId,
            RequestId,
            newContexts,
            CreatedAt,
            ContextMetadata.ToDictionary(kv => kv.Key, kv => kv.Value)
        );
    }

    /// <summary>
    /// 获取指定类型的定向上下文
    /// </summary>
    public T? GetTargetingContext<T>() where T : class, ITargetingContext
    {
        return _targetingContexts.Values.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// 获取指定键的定向上下文
    /// </summary>
    public ITargetingContext? GetTargetingContext(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        return _targetingContexts.TryGetValue(key, out var context) ? context : null;
    }

    /// <summary>
    /// 获取指定键和类型的定向上下文
    /// </summary>
    public T? GetTargetingContext<T>(string key) where T : class, ITargetingContext
    {
        var context = GetTargetingContext(key);
        return context as T;
    }

    /// <summary>
    /// 获取指定类型名称的定向上下文
    /// </summary>
    public ITargetingContext? GetTargetingContextByType(string contextType)
    {
        if (string.IsNullOrEmpty(contextType))
            return null;

        return _targetingContexts.Values.FirstOrDefault(ctx =>
            string.Equals(ctx.ContextType, contextType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 是否包含指定键的定向上下文
    /// </summary>
    public bool HasTargetingContext(string key)
    {
        return !string.IsNullOrEmpty(key) && _targetingContexts.ContainsKey(key);
    }

    /// <summary>
    /// 是否包含指定类型的定向上下文
    /// </summary>
    public bool HasTargetingContextOfType<T>() where T : class, ITargetingContext
    {
        return _targetingContexts.Values.OfType<T>().Any();
    }

    /// <summary>
    /// 是否包含指定类型名称的定向上下文
    /// </summary>
    public bool HasTargetingContextOfType(string contextType)
    {
        return GetTargetingContextByType(contextType) != null;
    }

    /// <summary>
    /// 获取所有定向上下文的键
    /// </summary>
    public IReadOnlyList<string> GetTargetingContextKeys()
    {
        return _targetingContexts.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取所有定向上下文的类型
    /// </summary>
    public IReadOnlyList<string> GetTargetingContextTypes()
    {
        return _targetingContexts.Values
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

        if (ContextMetadata.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// 添加或更新上下文元数据
    /// </summary>
    public TargetingContext WithMetadata(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("元数据键不能为空", nameof(key));

        var newMetadata = new Dictionary<string, object>(ContextMetadata)
        {
            [key] = value
        };

        return new TargetingContext(
            ContextId,
            RequestId,
            _targetingContexts,
            CreatedAt,
            newMetadata
        );
    }

    /// <summary>
    /// 移除上下文元数据
    /// </summary>
    public TargetingContext WithoutMetadata(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("元数据键不能为空", nameof(key));

        var newMetadata = new Dictionary<string, object>(ContextMetadata);
        newMetadata.Remove(key);

        return new TargetingContext(
            ContextId,
            RequestId,
            _targetingContexts,
            CreatedAt,
            newMetadata
        );
    }

    /// <summary>
    /// 是否为有效的定向上下文
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(ContextId) &&
               !string.IsNullOrEmpty(RequestId) &&
               _targetingContexts.Any();
    }

    /// <summary>
    /// 获取上下文摘要信息
    /// </summary>
    public Dictionary<string, object> GetSummary()
    {
        var contextTypeCounts = _targetingContexts.Values
            .GroupBy(ctx => ctx.ContextType)
            .ToDictionary(g => g.Key, g => g.Count());

        return new Dictionary<string, object>
        {
            ["ContextId"] = ContextId,
            ["RequestId"] = RequestId,
            ["TargetingContextsCount"] = _targetingContexts.Count,
            ["TargetingContextTypes"] = GetTargetingContextTypes(),
            ["ContextTypeCounts"] = contextTypeCounts,
            ["MetadataKeysCount"] = ContextMetadata.Count,
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

        var filteredContexts = _targetingContexts
            .Where(kv => includeContextTypes.Contains(kv.Value.ContextType, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return new TargetingContext(
            ContextId,
            RequestId,
            filteredContexts,
            CreatedAt,
            ContextMetadata.ToDictionary(kv => kv.Key, kv => kv.Value)
        );
    }

    /// <summary>
    /// 创建包含指定键的轻量级副本
    /// </summary>
    public TargetingContext CreateLightweightCopyByKeys(params string[] includeKeys)
    {
        if (includeKeys == null || includeKeys.Length == 0)
        {
            return this; // 返回当前实例，因为是不可变的
        }

        var filteredContexts = _targetingContexts
            .Where(kv => includeKeys.Contains(kv.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return new TargetingContext(
            ContextId,
            RequestId,
            filteredContexts,
            CreatedAt,
            ContextMetadata.ToDictionary(kv => kv.Key, kv => kv.Value)
        );
    }

    /// <summary>
    /// 合并另一个TargetingContext
    /// </summary>
    public TargetingContext Merge(TargetingContext other, bool overwriteExisting = false)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var mergedContexts = new Dictionary<string, ITargetingContext>(_targetingContexts);

        foreach (var context in other._targetingContexts)
        {
            if (overwriteExisting || !mergedContexts.ContainsKey(context.Key))
            {
                mergedContexts[context.Key] = context.Value;
            }
        }

        var mergedMetadata = new Dictionary<string, object>(ContextMetadata);

        foreach (var metadata in other.ContextMetadata)
        {
            if (overwriteExisting || !mergedMetadata.ContainsKey(metadata.Key))
            {
                mergedMetadata[metadata.Key] = metadata.Value;
            }
        }

        return new TargetingContext(
            ContextId, // 保持原有的ContextId
            RequestId, // 保持原有的RequestId
            mergedContexts,
            CreatedAt, // 保持原有的创建时间
            mergedMetadata
        );
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ContextId;
        yield return RequestId;
        yield return CreatedAt;

        // 上下文字典的内容哈希
        foreach (var context in _targetingContexts.OrderBy(kv => kv.Key))
        {
            yield return context.Key;
            yield return context.Value.ContextId;
            yield return context.Value.ContextType;
        }

        // 元数据的内容哈希
        foreach (var metadata in ContextMetadata.OrderBy(kv => kv.Key))
        {
            yield return metadata.Key;
            yield return metadata.Value?.GetHashCode() ?? 0;
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
