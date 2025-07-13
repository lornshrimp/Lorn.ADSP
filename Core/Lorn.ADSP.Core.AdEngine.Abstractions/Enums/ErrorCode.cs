namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 策略未找到
        /// </summary>
        StrategyNotFound = 1001,

        /// <summary>
        /// 策略配置无效
        /// </summary>
        InvalidStrategyConfig = 1002,

        /// <summary>
        /// 策略执行超时
        /// </summary>
        StrategyTimeout = 1003,

        /// <summary>
        /// 策略执行异常
        /// </summary>
        StrategyException = 1004,

        /// <summary>
        /// 回调未找到
        /// </summary>
        CallbackNotFound = 2001,

        /// <summary>
        /// 回调执行失败
        /// </summary>
        CallbackFailed = 2002,

        /// <summary>
        /// 上下文数据无效
        /// </summary>
        InvalidContext = 3001,

        /// <summary>
        /// 上下文数据过期
        /// </summary>
        ContextExpired = 3002,

        /// <summary>
        /// 配置参数无效
        /// </summary>
        InvalidConfiguration = 4001,

        /// <summary>
        /// 资源不足
        /// </summary>
        InsufficientResources = 5001,

        /// <summary>
        /// 网络连接失败
        /// </summary>
        NetworkFailure = 6001
    }
}
