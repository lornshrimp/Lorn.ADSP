using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.Domain.Requests
{
    /// <summary>
    /// 广告候选请求
    /// </summary>
    public record AdCandidateRequest
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public required string RequestId { get; init; }

        /// <summary>
        /// 广告位ID
        /// </summary>
        public required string PlacementId { get; init; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string? UserId { get; init; }

        /// <summary>
        /// 请求数量
        /// </summary>
        public int Count { get; init; } = 10;

        /// <summary>
        /// 过滤条件
        /// </summary>
        public IReadOnlyDictionary<string, object> Filters { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// 排序条件
        /// </summary>
        public IReadOnlyList<SortCriteria> SortBy { get; init; } = Array.Empty<SortCriteria>();

        /// <summary>
        /// 是否包含已过期的广告
        /// </summary>
        public bool IncludeExpired { get; init; } = false;
    }
}
