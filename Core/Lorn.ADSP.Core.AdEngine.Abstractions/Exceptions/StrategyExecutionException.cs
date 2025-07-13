namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
    /// <summary>
    /// 策略执行异常
    /// </summary>
    public class StrategyExecutionException : Exception
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string? StrategyId { get; }

        /// <summary>
        /// 执行ID
        /// </summary>
        public string? ExecutionId { get; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string strategyId, string message) : base(message)
        {
            StrategyId = strategyId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string strategyId, string executionId, string message) : base(message)
        {
            StrategyId = strategyId;
            ExecutionId = executionId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string strategyId, string executionId, string errorCode, string message)
            : base(message)
        {
            StrategyId = strategyId;
            ExecutionId = executionId;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyExecutionException(string strategyId, string executionId, string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StrategyId = strategyId;
            ExecutionId = executionId;
            ErrorCode = errorCode;
        }
    }
}
