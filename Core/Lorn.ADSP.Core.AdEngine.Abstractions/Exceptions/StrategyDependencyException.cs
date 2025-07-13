namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
    /// <summary>
    /// 策略依赖异常
    /// </summary>
    public class StrategyDependencyException : Exception
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string? StrategyId { get; }

        /// <summary>
        /// 依赖项名称
        /// </summary>
        public string? DependencyName { get; }

        /// <summary>
        /// 依赖项类型
        /// </summary>
        public Type? DependencyType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyDependencyException(string message) : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyDependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyDependencyException(string strategyId, string dependencyName, string message) : base(message)
        {
            StrategyId = strategyId;
            DependencyName = dependencyName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StrategyDependencyException(string strategyId, string dependencyName, Type dependencyType, string message) : base(message)
        {
            StrategyId = strategyId;
            DependencyName = dependencyName;
            DependencyType = dependencyType;
        }
    }
}
