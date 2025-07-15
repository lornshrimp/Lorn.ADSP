using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 批量操作结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public record BatchOperationResult<T> where T : class
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 成功处理的项
        /// </summary>
        public IEnumerable<T> SuccessfulItems { get; init; } = Array.Empty<T>();

        /// <summary>
        /// 失败的项
        /// </summary>
        public IEnumerable<BatchFailureItem<T>> FailedItems { get; init; } = Array.Empty<BatchFailureItem<T>>();

        /// <summary>
        /// 总处理数量
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// 成功数量
        /// </summary>
        public int SuccessCount { get; init; }

        /// <summary>
        /// 失败数量
        /// </summary>
        public int FailureCount { get; init; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public TimeSpan ExecutionTime { get; init; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public BatchOperationType OperationType { get; init; }
    }
}
