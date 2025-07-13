namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 缓存策略配置
/// </summary>
public sealed class CachePolicy
{
    /// <summary>
    /// 缓存策略类型
    /// </summary>
    public Enums.CachePolicyType Type { get; init; } = Enums.CachePolicyType.CacheFirst;

    /// <summary>
    /// 缓存有效期
    /// </summary>
    public TimeSpan? TTL { get; init; }

    /// <summary>
    /// 是否允许过期缓存
    /// </summary>
    public bool AllowStale { get; init; } = false;

    /// <summary>
    /// 过期缓存的最大允许年龄
    /// </summary>
    public TimeSpan? MaxStaleAge { get; init; }

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string? KeyPrefix { get; init; }

    /// <summary>
    /// 是否压缩缓存数据
    /// </summary>
    public bool CompressData { get; init; } = false;

    /// <summary>
    /// 缓存优先级
    /// </summary>
    public int Priority { get; init; } = 1;

    /// <summary>
    /// 自定义标签
    /// </summary>
    public IReadOnlyDictionary<string, string>? Tags { get; init; }

    /// <summary>
    /// 默认缓存策略
    /// </summary>
    public static readonly CachePolicy Default = new()
    {
        Type = Enums.CachePolicyType.CacheFirst,
        TTL = TimeSpan.FromMinutes(5),
        AllowStale = true,
        MaxStaleAge = TimeSpan.FromMinutes(10)
    };

    /// <summary>
    /// 无缓存策略
    /// </summary>
    public static readonly CachePolicy NoCache = new()
    {
        Type = Enums.CachePolicyType.NoCache
    };

    /// <summary>
    /// 仅缓存策略
    /// </summary>
    public static readonly CachePolicy CacheOnly = new()
    {
        Type = Enums.CachePolicyType.CacheOnly,
        AllowStale = true
    };

    /// <summary>
    /// 创建自定义缓存策略
    /// </summary>
    public static CachePolicy Create(
        Enums.CachePolicyType type,
        TimeSpan? ttl = null,
        bool allowStale = false,
        TimeSpan? maxStaleAge = null)
    {
        return new CachePolicy
        {
            Type = type,
            TTL = ttl,
            AllowStale = allowStale,
            MaxStaleAge = maxStaleAge
        };
    }
}