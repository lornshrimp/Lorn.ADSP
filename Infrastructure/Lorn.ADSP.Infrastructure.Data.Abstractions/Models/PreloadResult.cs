namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 预加载结果
    /// </summary>
    public record PreloadResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public required bool IsSuccess { get; init; }

        /// <summary>
        /// 预加载项结果
        /// </summary>
        public IReadOnlyList<PreloadItemResult> ItemResults { get; init; } = Array.Empty<PreloadItemResult>();

        /// <summary>
        /// 总预加载时间
        /// </summary>
        public TimeSpan TotalPreloadTime { get; init; }

        /// <summary>
        /// 成功预加载的项数
        /// </summary>
        public int SuccessfulItems => ItemResults.Count(r => r.IsSuccess);

        /// <summary>
        /// 失败预加载的项数
        /// </summary>
        public int FailedItems => ItemResults.Count(r => !r.IsSuccess);

        /// <summary>
        /// 预加载的总数据量
        /// </summary>
        public long TotalDataSize { get; init; }
    }
}
