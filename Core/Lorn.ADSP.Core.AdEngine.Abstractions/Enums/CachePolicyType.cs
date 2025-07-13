namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums
{

    /// <summary>
    /// 缓存策略
    /// </summary>
    public enum CachePolicyType
    {
        /// <summary>
        /// 不使用缓存，总是获取最新数据
        /// </summary>
        NoCache = 0,

        /// <summary>
        /// 优先使用缓存，缓存不存在时获取新数据
        /// </summary>
        CacheFirst = 1,

        /// <summary>
        /// 优先获取新数据，失败时使用缓存
        /// </summary>
        FreshFirst = 2,

        /// <summary>
        /// 仅使用缓存，不获取新数据
        /// </summary>
        CacheOnly = 3,

        /// <summary>
        /// 如果缓存过期则刷新，否则使用缓存
        /// </summary>
        RefreshIfExpired = 4,

        /// <summary>
        /// 后台刷新缓存，立即返回缓存数据
        /// </summary>
        BackgroundRefresh = 5
    }
}
