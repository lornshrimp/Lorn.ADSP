namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 批量操作失败项
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public record BatchFailureItem<T>
    {
        /// <summary>
        /// 失败的项
        /// </summary>
        public required T Item { get; init; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public required string ErrorMessage { get; init; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string? ErrorCode { get; init; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception? Exception { get; init; }
    }
}
