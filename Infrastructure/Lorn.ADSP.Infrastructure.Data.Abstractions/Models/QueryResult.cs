namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 查询结果
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public record QueryResult<T> where T : class
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public IEnumerable<T> Items { get; init; } = Array.Empty<T>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; init; } = true;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public TimeSpan ExecutionTime { get; init; }

        /// <summary>
        /// 是否来自缓存
        /// </summary>
        public bool FromCache { get; init; }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="items">数据项</param>
        /// <param name="totalCount">总记录数</param>
        /// <param name="executionTime">执行时间</param>
        /// <param name="fromCache">是否来自缓存</param>
        /// <returns>查询结果</returns>
        public static QueryResult<T> Success(IEnumerable<T> items, int totalCount, TimeSpan executionTime, bool fromCache = false)
        {
            return new QueryResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                IsSuccess = true,
                ExecutionTime = executionTime,
                FromCache = fromCache
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="executionTime">执行时间</param>
        /// <returns>查询结果</returns>
        public static QueryResult<T> Failure(string errorMessage, TimeSpan executionTime)
        {
            return new QueryResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ExecutionTime = executionTime
            };
        }
    }
}
