using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces
{
    /// <summary>
    /// 查询规格接口
    /// </summary>
    /// <typeparam name="T">查询目标类型</typeparam>
    public interface IQuerySpecification<T> where T : class
    {
        /// <summary>
        /// 查询过滤条件
        /// </summary>
        Dictionary<string, object> Filters { get; }

        /// <summary>
        /// 排序条件
        /// </summary>
        IReadOnlyList<SortCriteria> SortCriteria { get; }

        /// <summary>
        /// 分页信息
        /// </summary>
        PaginationInfo? Pagination { get; }

        /// <summary>
        /// 投影字段
        /// </summary>
        string[]? ProjectionFields { get; }

        /// <summary>
        /// 查询超时时间
        /// </summary>
        TimeSpan? Timeout { get; }

        /// <summary>
        /// 是否包含总数统计
        /// </summary>
        bool IncludeTotalCount { get; }
    }
}
