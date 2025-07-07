namespace Lorn.ADSP.Core.AdEngine.Abstractions.Constants;

/// <summary>
/// 策略常量
/// </summary>
public static class StrategyConstants
{
    /// <summary>
    /// 默认策略优先级
    /// </summary>
    public const int DefaultPriority = 5;

    /// <summary>
    /// 最高优先级
    /// </summary>
    public const int HighestPriority = 1;

    /// <summary>
    /// 最低优先级
    /// </summary>
    public const int LowestPriority = 10;

    /// <summary>
    /// 默认超时时间（毫秒）
    /// </summary>
    public const int DefaultTimeoutMs = 10000;

    /// <summary>
    /// 最小超时时间（毫秒）
    /// </summary>
    public const int MinTimeoutMs = 100;

    /// <summary>
    /// 最大超时时间（毫秒）
    /// </summary>
    public const int MaxTimeoutMs = 60000;

    /// <summary>
    /// 默认重试次数
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public const int MaxRetries = 10;

    /// <summary>
    /// 默认批处理大小
    /// </summary>
    public const int DefaultBatchSize = 100;

    /// <summary>
    /// 最大批处理大小
    /// </summary>
    public const int MaxBatchSize = 10000;

    /// <summary>
    /// 策略版本格式
    /// </summary>
    public const string VersionFormat = @"^\d+\.\d+\.\d+$";

    /// <summary>
    /// 策略ID格式
    /// </summary>
    public const string StrategyIdFormat = @"^[a-zA-Z][a-zA-Z0-9_]*$";
}

/// <summary>
/// 配置键常量
/// </summary>
public static class ConfigurationKeys
{
    /// <summary>
    /// 广告引擎配置根节点
    /// </summary>
    public const string AdEngineRoot = "AdEngine";

    /// <summary>
    /// 策略配置节点
    /// </summary>
    public const string Strategies = "AdEngine:Strategies";

    /// <summary>
    /// 回调配置节点
    /// </summary>
    public const string Callbacks = "AdEngine:Callbacks";

    /// <summary>
    /// 监控配置节点
    /// </summary>
    public const string Monitoring = "AdEngine:Monitoring";

    /// <summary>
    /// 性能配置节点
    /// </summary>
    public const string Performance = "AdEngine:Performance";

    /// <summary>
    /// 安全配置节点
    /// </summary>
    public const string Security = "AdEngine:Security";

    /// <summary>
    /// 缓存配置节点
    /// </summary>
    public const string Caching = "AdEngine:Caching";

    /// <summary>
    /// 日志配置节点
    /// </summary>
    public const string Logging = "AdEngine:Logging";

    /// <summary>
    /// 数据源配置节点
    /// </summary>
    public const string DataSources = "AdEngine:DataSources";

    /// <summary>
    /// A/B测试配置节点
    /// </summary>
    public const string ABTesting = "AdEngine:ABTesting";

    /// <summary>
    /// 特性开关配置节点
    /// </summary>
    public const string FeatureFlags = "AdEngine:FeatureFlags";

    /// <summary>
    /// 默认超时配置键
    /// </summary>
    public const string DefaultTimeout = "AdEngine:DefaultTimeout";

    /// <summary>
    /// 最大并发数配置键
    /// </summary>
    public const string MaxConcurrency = "AdEngine:MaxConcurrency";

    /// <summary>
    /// 是否启用监控配置键
    /// </summary>
    public const string EnableMonitoring = "AdEngine:EnableMonitoring";

    /// <summary>
    /// 是否启用调试模式配置键
    /// </summary>
    public const string EnableDebugMode = "AdEngine:EnableDebugMode";
}

/// <summary>
/// 默认值常量
/// </summary>
public static class DefaultValues
{
    /// <summary>
    /// 默认策略版本
    /// </summary>
    public const string DefaultStrategyVersion = "1.0.0";

    /// <summary>
    /// 默认策略名称
    /// </summary>
    public const string DefaultStrategyName = "UnnamedStrategy";

    /// <summary>
    /// 默认策略作者
    /// </summary>
    public const string DefaultStrategyAuthor = "System";

    /// <summary>
    /// 默认配置版本
    /// </summary>
    public const string DefaultConfigVersion = "1.0";

    /// <summary>
    /// 默认回调超时时间（毫秒）
    /// </summary>
    public const int DefaultCallbackTimeoutMs = 5000;

    /// <summary>
    /// 默认缓存过期时间（分钟）
    /// </summary>
    public const int DefaultCacheExpirationMinutes = 30;

    /// <summary>
    /// 默认最大候选数量
    /// </summary>
    public const int DefaultMaxCandidates = 1000;

    /// <summary>
    /// 默认最小候选数量
    /// </summary>
    public const int DefaultMinCandidates = 1;

    /// <summary>
    /// 默认质量分阈值
    /// </summary>
    public const decimal DefaultQualityThreshold = 0.5m;

    /// <summary>
    /// 默认相关性阈值
    /// </summary>
    public const decimal DefaultRelevanceThreshold = 0.6m;

    /// <summary>
    /// 默认置信度阈值
    /// </summary>
    public const decimal DefaultConfidenceThreshold = 0.7m;

    /// <summary>
    /// 默认权重值
    /// </summary>
    public const decimal DefaultWeight = 1.0m;

    /// <summary>
    /// 默认CPU需求等级
    /// </summary>
    public const int DefaultCpuRequirement = 5;

    /// <summary>
    /// 默认内存需求（MB）
    /// </summary>
    public const int DefaultMemoryRequirementMB = 100;

    /// <summary>
    /// 默认网络需求等级
    /// </summary>
    public const int DefaultNetworkRequirement = 5;

    /// <summary>
    /// 默认最大并发数
    /// </summary>
    public const int DefaultMaxConcurrency = 10;
}

/// <summary>
/// 超时常量
/// </summary>
public static class TimeoutConstants
{
    /// <summary>
    /// 快速操作超时时间
    /// </summary>
    public static readonly TimeSpan FastOperation = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// 正常操作超时时间
    /// </summary>
    public static readonly TimeSpan NormalOperation = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// 慢速操作超时时间
    /// </summary>
    public static readonly TimeSpan SlowOperation = TimeSpan.FromSeconds(2);

    /// <summary>
    /// 数据库查询超时时间
    /// </summary>
    public static readonly TimeSpan DatabaseQuery = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 网络调用超时时间
    /// </summary>
    public static readonly TimeSpan NetworkCall = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 文件操作超时时间
    /// </summary>
    public static readonly TimeSpan FileOperation = TimeSpan.FromSeconds(15);

    /// <summary>
    /// 批处理操作超时时间
    /// </summary>
    public static readonly TimeSpan BatchOperation = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 长时间运行操作超时时间
    /// </summary>
    public static readonly TimeSpan LongRunningOperation = TimeSpan.FromMinutes(5);
}



