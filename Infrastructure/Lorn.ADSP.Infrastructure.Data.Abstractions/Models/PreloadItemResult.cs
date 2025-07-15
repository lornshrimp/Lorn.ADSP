namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 预加载项结果
    /// </summary>
    public record PreloadItemResult
    {
        /// <summary>
        /// 预加载请求
        /// </summary>
        public required DataPreloadRequest Request { get; init; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public required bool IsSuccess { get; init; }

        /// <summary>
        /// 预加载的数据量
        /// </summary>
        public long DataSize { get; init; }

        /// <summary>
        /// 预加载时间
        /// </summary>
        public TimeSpan PreloadTime { get; init; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// 缓存键
        /// </summary>
        public string? CacheKey { get; init; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpirationTime { get; init; }
    }
}
