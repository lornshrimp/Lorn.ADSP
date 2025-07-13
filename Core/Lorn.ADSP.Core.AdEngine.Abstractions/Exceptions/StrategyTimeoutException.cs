namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
    /// <summary>
    /// 策略超时异常
    /// </summary>
    public class StrategyTimeoutException : StrategyExecutionException
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// 实际执行时间
        /// </summary>
        public TimeSpan ActualExecutionTime { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyTimeoutException(string strategyId, TimeSpan timeout, TimeSpan actualExecutionTime)
            : base(strategyId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
        {
            Timeout = timeout;
            ActualExecutionTime = actualExecutionTime;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyTimeoutException(string strategyId, string executionId, TimeSpan timeout, TimeSpan actualExecutionTime)
            : base(strategyId, executionId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
        {
            Timeout = timeout;
            ActualExecutionTime = actualExecutionTime;
        }
    }
}
