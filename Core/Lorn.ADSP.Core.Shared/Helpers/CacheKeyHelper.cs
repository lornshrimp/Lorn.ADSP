namespace Lorn.ADSP.Core.Shared.Helpers;

/// <summary>
/// 缓存键生成帮助类
/// </summary>
public static class CacheKeyHelper
{
    /// <summary>
    /// 缓存键分隔符
    /// </summary>
    private const string Separator = ":";

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public static class Prefixes
    {
        /// <summary>
        /// 用户特征缓存前缀
        /// </summary>
        public const string UserFeature = "user";

        /// <summary>
        /// 上下文特征缓存前缀
        /// </summary>
        public const string ContextFeature = "context";

        /// <summary>
        /// 广告特征缓存前缀
        /// </summary>
        public const string AdFeature = "ad";

        /// <summary>
        /// 审核结果缓存前缀
        /// </summary>
        public const string AuditResult = "audit";

        /// <summary>
        /// 会话缓存前缀
        /// </summary>
        public const string Session = "session";

        /// <summary>
        /// 配置缓存前缀
        /// </summary>
        public const string Configuration = "config";

        /// <summary>
        /// 指标缓存前缀
        /// </summary>
        public const string Metrics = "metrics";

        /// <summary>
        /// 锁缓存前缀
        /// </summary>
        public const string Lock = "lock";
    }

    /// <summary>
    /// 生成用户特征缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="featureType">特征类型</param>
    /// <returns>缓存键</returns>
    public static string GenerateUserFeatureKey(string userId, string featureType = "feature")
    {
        return BuildKey(Prefixes.UserFeature, userId, featureType);
    }

    /// <summary>
    /// 生成上下文特征缓存键
    /// </summary>
    /// <param name="contextId">上下文ID</param>
    /// <param name="featureType">特征类型</param>
    /// <returns>缓存键</returns>
    public static string GenerateContextFeatureKey(string contextId, string featureType = "feature")
    {
        return BuildKey(Prefixes.ContextFeature, contextId, featureType);
    }

    /// <summary>
    /// 生成广告特征缓存键
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="featureType">特征类型</param>
    /// <returns>缓存键</returns>
    public static string GenerateAdFeatureKey(string adId, string featureType = "feature")
    {
        return BuildKey(Prefixes.AdFeature, adId, featureType);
    }

    /// <summary>
    /// 生成审核结果缓存键
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <returns>缓存键</returns>
    public static string GenerateAuditResultKey(string adId)
    {
        return BuildKey(Prefixes.AuditResult, adId, "result");
    }

    /// <summary>
    /// 生成会话缓存键
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>缓存键</returns>
    public static string GenerateSessionKey(string sessionId)
    {
        return BuildKey(Prefixes.Session, sessionId);
    }

    /// <summary>
    /// 生成配置缓存键
    /// </summary>
    /// <param name="configKey">配置键</param>
    /// <returns>缓存键</returns>
    public static string GenerateConfigurationKey(string configKey)
    {
        return BuildKey(Prefixes.Configuration, configKey);
    }

    /// <summary>
    /// 生成指标缓存键
    /// </summary>
    /// <param name="metricType">指标类型</param>
    /// <param name="metricId">指标ID</param>
    /// <returns>缓存键</returns>
    public static string GenerateMetricsKey(string metricType, string metricId)
    {
        return BuildKey(Prefixes.Metrics, metricType, metricId);
    }

    /// <summary>
    /// 生成锁缓存键
    /// </summary>
    /// <param name="lockType">锁类型</param>
    /// <param name="lockId">锁ID</param>
    /// <returns>缓存键</returns>
    public static string GenerateLockKey(string lockType, string lockId)
    {
        return BuildKey(Prefixes.Lock, lockType, lockId);
    }

    /// <summary>
    /// 生成带时间戳的缓存键
    /// </summary>
    /// <param name="baseKey">基础键</param>
    /// <param name="timestamp">时间戳</param>
    /// <returns>缓存键</returns>
    public static string GenerateTimestampKey(string baseKey, long timestamp)
    {
        return BuildKey(baseKey, timestamp.ToString());
    }

    /// <summary>
    /// 生成带日期的缓存键
    /// </summary>
    /// <param name="baseKey">基础键</param>
    /// <param name="date">日期</param>
    /// <returns>缓存键</returns>
    public static string GenerateDateKey(string baseKey, DateTime date)
    {
        return BuildKey(baseKey, date.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// 构建缓存键
    /// </summary>
    /// <param name="parts">键的各个部分</param>
    /// <returns>完整的缓存键</returns>
    private static string BuildKey(params string[] parts)
    {
        var validParts = parts.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(Separator, validParts);
    }

    /// <summary>
    /// 解析缓存键
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>键的各个部分</returns>
    public static string[] ParseKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Array.Empty<string>();

        return key.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// 获取缓存键的前缀
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>前缀</returns>
    public static string GetPrefix(string key)
    {
        var parts = ParseKey(key);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    /// <summary>
    /// 构建通配符模式
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <param name="pattern">模式</param>
    /// <returns>通配符模式</returns>
    public static string BuildWildcardPattern(string prefix, string pattern = "*")
    {
        return BuildKey(prefix, pattern);
    }
}