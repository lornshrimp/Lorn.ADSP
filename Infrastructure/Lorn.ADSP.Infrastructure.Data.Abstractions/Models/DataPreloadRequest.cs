using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 数据预加载请求
    /// </summary>
    public record DataPreloadRequest
    {
        /// <summary>
        /// 数据访问上下文
        /// </summary>
        public required DataAccessContext Context { get; init; }

        /// <summary>
        /// 预加载策略
        /// </summary>
        public PreloadStrategy Strategy { get; init; } = PreloadStrategy.Normal;

        /// <summary>
        /// 优先级
        /// </summary>
        public RequestPriority Priority { get; init; } = RequestPriority.Normal;

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public TimeSpan? CacheExpiration { get; init; }

        /// <summary>
        /// 预加载标签
        /// </summary>
        public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 扩展参数
        /// </summary>
        public IReadOnlyDictionary<string, object> ExtendedParameters { get; init; } = new Dictionary<string, object>();
    }
}
