namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
    /// <summary>
    /// 策略配置异常
    /// </summary>
    public class StrategyConfigurationException : Exception
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string? StrategyId { get; }

        /// <summary>
        /// 配置键
        /// </summary>
        public string? ConfigurationKey { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyConfigurationException(string strategyId, string message) : base(message)
        {
            StrategyId = strategyId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyConfigurationException(string strategyId, string configurationKey, string message) : base(message)
        {
            StrategyId = strategyId;
            ConfigurationKey = configurationKey;
        }
    }
}
