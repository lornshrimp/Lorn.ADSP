namespace Lorn.ADSP.Core.AdEngine.Abstractions.Enums;

/// <summary>
/// 错误代码枚举
/// 定义广告引擎中可能出现的各种错误类型
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// 未知错误
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 超时错误
    /// </summary>
    Timeout = 1000,

    /// <summary>
    /// 取消操作
    /// </summary>
    Cancelled = 1001,

    /// <summary>
    /// 配置错误
    /// </summary>
    Configuration = 2000,

    /// <summary>
    /// 配置缺失
    /// </summary>
    ConfigurationMissing = 2001,

    /// <summary>
    /// 配置无效
    /// </summary>
    ConfigurationInvalid = 2002,

    /// <summary>
    /// 数据访问错误
    /// </summary>
    DataAccess = 3000,

    /// <summary>
    /// 数据不存在
    /// </summary>
    DataNotFound = 3001,

    /// <summary>
    /// 数据格式错误
    /// </summary>
    DataFormatError = 3002,

    /// <summary>
    /// 数据验证失败
    /// </summary>
    DataValidationFailed = 3003,

    /// <summary>
    /// 网络错误
    /// </summary>
    Network = 4000,

    /// <summary>
    /// 网络连接失败
    /// </summary>
    NetworkConnectionFailed = 4001,

    /// <summary>
    /// 网络请求超时
    /// </summary>
    NetworkTimeout = 4002,

    /// <summary>
    /// 服务不可用
    /// </summary>
    ServiceUnavailable = 4003,

    /// <summary>
    /// 匹配器错误
    /// </summary>
    Matcher = 5000,

    /// <summary>
    /// 匹配器未找到
    /// </summary>
    MatcherNotFound = 5001,

    /// <summary>
    /// 匹配器初始化失败
    /// </summary>
    MatcherInitializationFailed = 5002,

    /// <summary>
    /// 匹配器执行失败
    /// </summary>
    MatcherExecutionFailed = 5003,

    /// <summary>
    /// 匹配器不支持
    /// </summary>
    MatcherNotSupported = 5004,

    /// <summary>
    /// 策略错误
    /// </summary>
    Strategy = 6000,

    /// <summary>
    /// 策略加载失败
    /// </summary>
    StrategyLoadFailed = 6001,

    /// <summary>
    /// 策略执行失败
    /// </summary>
    StrategyExecutionFailed = 6002,

    /// <summary>
    /// 策略配置错误
    /// </summary>
    StrategyConfigurationError = 6003,

    /// <summary>
    /// 缓存错误
    /// </summary>
    Cache = 7000,

    /// <summary>
    /// 缓存连接失败
    /// </summary>
    CacheConnectionFailed = 7001,

    /// <summary>
    /// 缓存操作失败
    /// </summary>
    CacheOperationFailed = 7002,

    /// <summary>
    /// 缓存数据损坏
    /// </summary>
    CacheDataCorrupted = 7003,

    /// <summary>
    /// 序列化错误
    /// </summary>
    Serialization = 8000,

    /// <summary>
    /// 序列化失败
    /// </summary>
    SerializationFailed = 8001,

    /// <summary>
    /// 反序列化失败
    /// </summary>
    DeserializationFailed = 8002,

    /// <summary>
    /// 类型转换错误
    /// </summary>
    TypeConversion = 8003,

    /// <summary>
    /// 权限错误
    /// </summary>
    Permission = 9000,

    /// <summary>
    /// 访问被拒绝
    /// </summary>
    AccessDenied = 9001,

    /// <summary>
    /// 认证失败
    /// </summary>
    AuthenticationFailed = 9002,

    /// <summary>
    /// 授权失败
    /// </summary>
    AuthorizationFailed = 9003,

    /// <summary>
    /// 资源错误
    /// </summary>
    Resource = 10000,

    /// <summary>
    /// 资源不足
    /// </summary>
    ResourceInsufficient = 10001,

    /// <summary>
    /// 资源耗尽
    /// </summary>
    ResourceExhausted = 10002,

    /// <summary>
    /// 资源锁定
    /// </summary>
    ResourceLocked = 10003,

    /// <summary>
    /// 业务逻辑错误
    /// </summary>
    BusinessLogic = 11000,

    /// <summary>
    /// 业务规则违反
    /// </summary>
    BusinessRuleViolation = 11001,

    /// <summary>
    /// 业务状态无效
    /// </summary>
    BusinessStateInvalid = 11002,

    /// <summary>
    /// 业务操作不允许
    /// </summary>
    BusinessOperationNotAllowed = 11003,

    /// <summary>
    /// 外部服务错误
    /// </summary>
    ExternalService = 12000,

    /// <summary>
    /// 外部API调用失败
    /// </summary>
    ExternalApiCallFailed = 12001,

    /// <summary>
    /// 外部服务响应无效
    /// </summary>
    ExternalServiceInvalidResponse = 12002,

    /// <summary>
    /// 外部服务限流
    /// </summary>
    ExternalServiceRateLimited = 12003
}