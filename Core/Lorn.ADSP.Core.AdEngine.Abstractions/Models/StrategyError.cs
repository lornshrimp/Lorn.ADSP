using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models
{
    /// <summary>
    /// 策略错误信息
    /// </summary>
    public record StrategyError
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public required string ErrorCode { get; init; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public required string ErrorMessage { get; init; }

        /// <summary>
        /// 错误级别
        /// </summary>
        public ErrorLevel Level { get; init; } = ErrorLevel.Error;

        /// <summary>
        /// 异常详情
        /// </summary>
        public Exception? Exception { get; init; }

        /// <summary>
        /// 错误发生时间
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 错误上下文
        /// </summary>
        public IReadOnlyDictionary<string, object> Context { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; init; }

        /// <summary>
        /// 是否可重试
        /// </summary>
        public bool IsRetriable { get; init; } = true;

        /// <summary>
        /// 创建错误信息
        /// </summary>
        public static StrategyError Create(
            string errorCode,
            string errorMessage,
            ErrorLevel level = ErrorLevel.Error,
            Exception? exception = null,
            IReadOnlyDictionary<string, object>? context = null)
        {
            return new StrategyError
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                Level = level,
                Exception = exception,
                Context = context ?? new Dictionary<string, object>()
            };
        }

        /// <summary>
        /// 创建警告信息
        /// </summary>
        public static StrategyError Warning(string errorCode, string errorMessage)
        {
            return Create(errorCode, errorMessage, ErrorLevel.Warning);
        }

        /// <summary>
        /// 创建信息提示
        /// </summary>
        public static StrategyError Info(string errorCode, string errorMessage)
        {
            return Create(errorCode, errorMessage, ErrorLevel.Info);
        }
    }
}
