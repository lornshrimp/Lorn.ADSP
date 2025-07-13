namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
    /// <summary>
    /// 策略验证异常
    /// </summary>
    public class StrategyValidationException : Exception
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string? StrategyId { get; }

        /// <summary>
        /// 验证错误列表
        /// </summary>
        public IReadOnlyList<string> ValidationErrors { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyValidationException(string message) : base(message)
        {
            ValidationErrors = Array.Empty<string>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyValidationException(string message, IReadOnlyList<string> validationErrors) : base(message)
        {
            ValidationErrors = validationErrors;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyValidationException(string strategyId, string message, IReadOnlyList<string> validationErrors) : base(message)
        {
            StrategyId = strategyId;
            ValidationErrors = validationErrors;
        }
    }
}
