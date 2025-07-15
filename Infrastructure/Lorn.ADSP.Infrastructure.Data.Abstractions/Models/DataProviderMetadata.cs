using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 数据提供者元数据
/// 包含提供者的标识信息、能力声明和配置参数
/// 用于注册表管理和路由决策
/// </summary>
public class DataProviderMetadata
{
    /// <summary>
    /// 提供者唯一标识符
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    /// 提供者名称
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// 提供者类型
    /// </summary>
    public DataProviderType ProviderType { get; set; }

    /// <summary>
    /// 业务实体类型
    /// 如 "Advertisement", "UserProfile", "Targeting", "Delivery"
    /// </summary>
    public string BusinessEntity { get; set; } = string.Empty;

    /// <summary>
    /// 技术类型
    /// 如 "Redis", "SqlServer", "MySQL", "Memory"
    /// </summary>
    public string TechnologyType { get; set; } = string.Empty;

    /// <summary>
    /// 平台类型
    /// 如 "AlibabaCloud", "Azure", "AWS", "Local"
    /// </summary>
    public string PlatformType { get; set; } = string.Empty;

    /// <summary>
    /// 提供者优先级
    /// 数值越高优先级越高
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 扩展元数据
    /// 存储提供者特定的配置和属性信息
    /// </summary>
    public Dictionary<string, object> ExtendedMetadata { get; set; } = new();

    /// <summary>
    /// 支持的操作类型
    /// 如 ["Get", "Set", "Remove", "Query", "Batch"]
    /// </summary>
    public string[] SupportedOperations { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 提供者版本
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 健康状态
    /// </summary>
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Unknown;

    /// <summary>
    /// 性能配置
    /// </summary>
    public PerformanceProfile PerformanceProfile { get; set; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 获取扩展元数据值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <returns>元数据值</returns>
    public T? GetExtendedMetadata<T>(string key)
    {
        if (ExtendedMetadata.TryGetValue(key, out var value))
        {
            return value is T typedValue ? typedValue : default;
        }
        return default;
    }

    /// <summary>
    /// 设置扩展元数据值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void SetExtendedMetadata(string key, object value)
    {
        ExtendedMetadata[key] = value;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 检查是否支持指定操作
    /// </summary>
    /// <param name="operation">操作名称</param>
    /// <returns>是否支持</returns>
    public bool SupportsOperation(string operation)
    {
        return SupportedOperations.Contains(operation, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// 性能配置信息
/// </summary>
public class PerformanceProfile
{
    /// <summary>
    /// 期望响应时间（毫秒）
    /// </summary>
    public int ExpectedResponseTimeMs { get; set; } = 100;

    /// <summary>
    /// 最大响应时间（毫秒）
    /// </summary>
    public int MaxResponseTimeMs { get; set; } = 5000;

    /// <summary>
    /// 最大并发连接数
    /// </summary>
    public int MaxConcurrentConnections { get; set; } = 100;

    /// <summary>
    /// 支持批量操作的最大大小
    /// </summary>
    public int MaxBatchSize { get; set; } = 1000;

    /// <summary>
    /// 是否支持并行操作
    /// </summary>
    public bool SupportsParallelOperations { get; set; } = true;

    /// <summary>
    /// 缓存配置
    /// </summary>
    public CacheConfiguration CacheConfiguration { get; set; } = new();
}

/// <summary>
/// 缓存配置信息
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// 是否启用缓存
    /// </summary>
    public bool CacheEnabled { get; set; } = true;

    /// <summary>
    /// 默认缓存过期时间
    /// </summary>
    public TimeSpan DefaultCacheDuration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 最大缓存过期时间
    /// </summary>
    public TimeSpan MaxCacheDuration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// 缓存策略
    /// </summary>
    public CacheStrategy CacheStrategy { get; set; } = CacheStrategy.WriteThrough;
}