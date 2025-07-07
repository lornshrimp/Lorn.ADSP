namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 错误码常量
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// 通用错误码
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        public const string UnknownError = "E0001";

        /// <summary>
        /// 参数无效
        /// </summary>
        public const string InvalidParameter = "E0002";

        /// <summary>
        /// 数据不存在
        /// </summary>
        public const string DataNotFound = "E0003";

        /// <summary>
        /// 数据重复
        /// </summary>
        public const string DataDuplicate = "E0004";

        /// <summary>
        /// 权限不足
        /// </summary>
        public const string InsufficientPermission = "E0005";

        /// <summary>
        /// 请求超时
        /// </summary>
        public const string RequestTimeout = "E0006";

        /// <summary>
        /// 系统繁忙
        /// </summary>
        public const string SystemBusy = "E0007";

        /// <summary>
        /// 内部错误
        /// </summary>
        public const string InternalError = "E0008";

        /// <summary>
        /// 操作已取消
        /// </summary>
        public const string OperationCancelled = "E0009";
    }

    /// <summary>
    /// 广告相关错误码
    /// </summary>
    public static class Advertisement
    {
        /// <summary>
        /// 广告不存在
        /// </summary>
        public const string AdNotFound = "E1001";

        /// <summary>
        /// 广告已过期
        /// </summary>
        public const string AdExpired = "E1002";

        /// <summary>
        /// 广告未审核通过
        /// </summary>
        public const string AdNotApproved = "E1003";

        /// <summary>
        /// 广告预算不足
        /// </summary>
        public const string InsufficientBudget = "E1004";

        /// <summary>
        /// 广告定向不匹配
        /// </summary>
        public const string TargetingMismatch = "E1005";

        /// <summary>
        /// 广告频次超限
        /// </summary>
        public const string FrequencyCapExceeded = "E1006";

        /// <summary>
        /// 广告素材无效
        /// </summary>
        public const string InvalidCreative = "E1007";
    }

    /// <summary>
    /// 竞价相关错误码
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// 竞价请求无效
        /// </summary>
        public const string InvalidBidRequest = "E2001";

        /// <summary>
        /// 竞价价格过低
        /// </summary>
        public const string BidPriceTooLow = "E2002";

        /// <summary>
        /// 竞价价格过高
        /// </summary>
        public const string BidPriceTooHigh = "E2003";

        /// <summary>
        /// 竞价超时
        /// </summary>
        public const string BidTimeout = "E2004";

        /// <summary>
        /// 无有效竞价
        /// </summary>
        public const string NoBidAvailable = "E2005";

        /// <summary>
        /// 竞价失败
        /// </summary>
        public const string BidFailed = "E2006";
    }

    /// <summary>
    /// 用户相关错误码
    /// </summary>
    public static class User
    {
        /// <summary>
        /// 用户不存在
        /// </summary>
        public const string UserNotFound = "E3001";

        /// <summary>
        /// 用户已存在
        /// </summary>
        public const string UserAlreadyExists = "E3002";

        /// <summary>
        /// 用户未激活
        /// </summary>
        public const string UserNotActivated = "E3003";

        /// <summary>
        /// 用户已被禁用
        /// </summary>
        public const string UserDisabled = "E3004";

        /// <summary>
        /// 用户认证失败
        /// </summary>
        public const string AuthenticationFailed = "E3005";

        /// <summary>
        /// 用户授权失败
        /// </summary>
        public const string AuthorizationFailed = "E3006";
    }

    /// <summary>
    /// 数据访问错误码
    /// </summary>
    public static class DataAccess
    {
        /// <summary>
        /// 数据库连接失败
        /// </summary>
        public const string DatabaseConnectionFailed = "E4001";

        /// <summary>
        /// 数据库查询失败
        /// </summary>
        public const string DatabaseQueryFailed = "E4002";

        /// <summary>
        /// 数据库更新失败
        /// </summary>
        public const string DatabaseUpdateFailed = "E4003";

        /// <summary>
        /// 缓存访问失败
        /// </summary>
        public const string CacheAccessFailed = "E4004";

        /// <summary>
        /// 数据序列化失败
        /// </summary>
        public const string DataSerializationFailed = "E4005";

        /// <summary>
        /// 数据反序列化失败
        /// </summary>
        public const string DataDeserializationFailed = "E4006";

        /// <summary>
        /// 数据验证失败
        /// </summary>
        public const string DataValidationFailed = "E4007";

        /// <summary>
        /// 数据转换失败
        /// </summary>
        public const string DataTransformationFailed = "E4008";

        /// <summary>
        /// 数据源不可用
        /// </summary>
        public const string DataSourceUnavailable = "E4009";
    }

    /// <summary>
    /// 外部服务错误码
    /// </summary>
    public static class ExternalService
    {
        /// <summary>
        /// 外部服务不可用
        /// </summary>
        public const string ServiceUnavailable = "E5001";

        /// <summary>
        /// 外部服务响应超时
        /// </summary>
        public const string ServiceTimeout = "E5002";

        /// <summary>
        /// 外部服务响应格式错误
        /// </summary>
        public const string InvalidServiceResponse = "E5003";

        /// <summary>
        /// 外部服务认证失败
        /// </summary>
        public const string ServiceAuthenticationFailed = "E5004";

        /// <summary>
        /// 外部服务限流
        /// </summary>
        public const string ServiceRateLimited = "E5005";
    }

    /// <summary>
    /// 广告引擎错误码
    /// </summary>
    public static class AdEngine
    {
        /// <summary>
        /// 策略相关错误码
        /// </summary>
        public static class Strategy
        {
            /// <summary>
            /// 策略未找到
            /// </summary>
            public const string StrategyNotFound = "E6001";

            /// <summary>
            /// 策略执行失败
            /// </summary>
            public const string StrategyExecutionFailed = "E6002";

            /// <summary>
            /// 策略执行超时
            /// </summary>
            public const string StrategyTimeout = "E6003";

            /// <summary>
            /// 策略配置无效
            /// </summary>
            public const string StrategyConfigurationInvalid = "E6004";

            /// <summary>
            /// 策略依赖缺失
            /// </summary>
            public const string StrategyDependencyMissing = "E6005";

            /// <summary>
            /// 策略验证失败
            /// </summary>
            public const string StrategyValidationFailed = "E6006";
        }

        /// <summary>
        /// 回调相关错误码
        /// </summary>
        public static class Callback
        {
            /// <summary>
            /// 回调未找到
            /// </summary>
            public const string CallbackNotFound = "E6101";

            /// <summary>
            /// 回调执行失败
            /// </summary>
            public const string CallbackExecutionFailed = "E6102";

            /// <summary>
            /// 回调执行超时
            /// </summary>
            public const string CallbackTimeout = "E6103";

            /// <summary>
            /// 回调配置无效
            /// </summary>
            public const string CallbackConfigurationInvalid = "E6104";

            /// <summary>
            /// 回调健康检查失败
            /// </summary>
            public const string CallbackHealthCheckFailed = "E6105";
        }

        /// <summary>
        /// 配置相关错误码
        /// </summary>
        public static class Configuration
        {
            /// <summary>
            /// 配置未找到
            /// </summary>
            public const string ConfigurationNotFound = "E6201";

            /// <summary>
            /// 配置无效
            /// </summary>
            public const string ConfigurationInvalid = "E6202";

            /// <summary>
            /// 配置加载失败
            /// </summary>
            public const string ConfigurationLoadFailed = "E6203";
        }

        /// <summary>
        /// 资源相关错误码
        /// </summary>
        public static class Resource
        {
            /// <summary>
            /// 资源耗尽
            /// </summary>
            public const string ResourceExhausted = "E6301";

            /// <summary>
            /// 资源不可用
            /// </summary>
            public const string ResourceUnavailable = "E6302";

            /// <summary>
            /// 资源限制超出
            /// </summary>
            public const string ResourceLimitExceeded = "E6303";
        }

        /// <summary>
        /// 业务规则相关错误码
        /// </summary>
        public static class BusinessRule
        {
            /// <summary>
            /// 业务规则违反
            /// </summary>
            public const string BusinessRuleViolation = "E6401";

            /// <summary>
            /// 业务规则评估失败
            /// </summary>
            public const string BusinessRuleEvaluationFailed = "E6402";
        }
    }
}