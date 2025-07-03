namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 超时配置常量
/// </summary>
public static class TimeoutConstants
{
    /// <summary>
    /// 广告请求处理超时时间 (毫秒)
    /// </summary>
    public const int AdRequestTimeout = 100;

    /// <summary>
    /// 竞价请求超时时间 (毫秒)
    /// </summary>
    public const int BidRequestTimeout = 80;

    /// <summary>
    /// 数据库查询超时时间 (秒)
    /// </summary>
    public const int DatabaseQueryTimeout = 30;

    /// <summary>
    /// 缓存访问超时时间 (毫秒)
    /// </summary>
    public const int CacheAccessTimeout = 50;

    /// <summary>
    /// HTTP客户端超时时间 (秒)
    /// </summary>
    public const int HttpClientTimeout = 10;

    /// <summary>
    /// 外部服务调用超时时间 (毫秒)
    /// </summary>
    public const int ExternalServiceTimeout = 200;

    /// <summary>
    /// 用户会话超时时间 (分钟)
    /// </summary>
    public const int UserSessionTimeout = 30;

    /// <summary>
    /// 缓存默认过期时间 (分钟)
    /// </summary>
    public const int DefaultCacheExpiration = 60;
}