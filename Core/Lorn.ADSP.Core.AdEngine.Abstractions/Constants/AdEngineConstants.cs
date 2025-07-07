namespace Lorn.ADSP.Core.AdEngine.Abstractions.Constants;

/// <summary>
/// ���Գ���
/// </summary>
public static class StrategyConstants
{
    /// <summary>
    /// Ĭ�ϲ������ȼ�
    /// </summary>
    public const int DefaultPriority = 5;

    /// <summary>
    /// ������ȼ�
    /// </summary>
    public const int HighestPriority = 1;

    /// <summary>
    /// ������ȼ�
    /// </summary>
    public const int LowestPriority = 10;

    /// <summary>
    /// Ĭ�ϳ�ʱʱ�䣨���룩
    /// </summary>
    public const int DefaultTimeoutMs = 10000;

    /// <summary>
    /// ��С��ʱʱ�䣨���룩
    /// </summary>
    public const int MinTimeoutMs = 100;

    /// <summary>
    /// ���ʱʱ�䣨���룩
    /// </summary>
    public const int MaxTimeoutMs = 60000;

    /// <summary>
    /// Ĭ�����Դ���
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// ������Դ���
    /// </summary>
    public const int MaxRetries = 10;

    /// <summary>
    /// Ĭ���������С
    /// </summary>
    public const int DefaultBatchSize = 100;

    /// <summary>
    /// ����������С
    /// </summary>
    public const int MaxBatchSize = 10000;

    /// <summary>
    /// ���԰汾��ʽ
    /// </summary>
    public const string VersionFormat = @"^\d+\.\d+\.\d+$";

    /// <summary>
    /// ����ID��ʽ
    /// </summary>
    public const string StrategyIdFormat = @"^[a-zA-Z][a-zA-Z0-9_]*$";
}

/// <summary>
/// ���ü�����
/// </summary>
public static class ConfigurationKeys
{
    /// <summary>
    /// ����������ø��ڵ�
    /// </summary>
    public const string AdEngineRoot = "AdEngine";

    /// <summary>
    /// �������ýڵ�
    /// </summary>
    public const string Strategies = "AdEngine:Strategies";

    /// <summary>
    /// �ص����ýڵ�
    /// </summary>
    public const string Callbacks = "AdEngine:Callbacks";

    /// <summary>
    /// ������ýڵ�
    /// </summary>
    public const string Monitoring = "AdEngine:Monitoring";

    /// <summary>
    /// �������ýڵ�
    /// </summary>
    public const string Performance = "AdEngine:Performance";

    /// <summary>
    /// ��ȫ���ýڵ�
    /// </summary>
    public const string Security = "AdEngine:Security";

    /// <summary>
    /// �������ýڵ�
    /// </summary>
    public const string Caching = "AdEngine:Caching";

    /// <summary>
    /// ��־���ýڵ�
    /// </summary>
    public const string Logging = "AdEngine:Logging";

    /// <summary>
    /// ����Դ���ýڵ�
    /// </summary>
    public const string DataSources = "AdEngine:DataSources";

    /// <summary>
    /// A/B�������ýڵ�
    /// </summary>
    public const string ABTesting = "AdEngine:ABTesting";

    /// <summary>
    /// ���Կ������ýڵ�
    /// </summary>
    public const string FeatureFlags = "AdEngine:FeatureFlags";

    /// <summary>
    /// Ĭ�ϳ�ʱ���ü�
    /// </summary>
    public const string DefaultTimeout = "AdEngine:DefaultTimeout";

    /// <summary>
    /// ��󲢷������ü�
    /// </summary>
    public const string MaxConcurrency = "AdEngine:MaxConcurrency";

    /// <summary>
    /// �Ƿ����ü�����ü�
    /// </summary>
    public const string EnableMonitoring = "AdEngine:EnableMonitoring";

    /// <summary>
    /// �Ƿ����õ���ģʽ���ü�
    /// </summary>
    public const string EnableDebugMode = "AdEngine:EnableDebugMode";
}

/// <summary>
/// Ĭ��ֵ����
/// </summary>
public static class DefaultValues
{
    /// <summary>
    /// Ĭ�ϲ��԰汾
    /// </summary>
    public const string DefaultStrategyVersion = "1.0.0";

    /// <summary>
    /// Ĭ�ϲ�������
    /// </summary>
    public const string DefaultStrategyName = "UnnamedStrategy";

    /// <summary>
    /// Ĭ�ϲ�������
    /// </summary>
    public const string DefaultStrategyAuthor = "System";

    /// <summary>
    /// Ĭ�����ð汾
    /// </summary>
    public const string DefaultConfigVersion = "1.0";

    /// <summary>
    /// Ĭ�ϻص���ʱʱ�䣨���룩
    /// </summary>
    public const int DefaultCallbackTimeoutMs = 5000;

    /// <summary>
    /// Ĭ�ϻ������ʱ�䣨���ӣ�
    /// </summary>
    public const int DefaultCacheExpirationMinutes = 30;

    /// <summary>
    /// Ĭ������ѡ����
    /// </summary>
    public const int DefaultMaxCandidates = 1000;

    /// <summary>
    /// Ĭ����С��ѡ����
    /// </summary>
    public const int DefaultMinCandidates = 1;

    /// <summary>
    /// Ĭ����������ֵ
    /// </summary>
    public const decimal DefaultQualityThreshold = 0.5m;

    /// <summary>
    /// Ĭ���������ֵ
    /// </summary>
    public const decimal DefaultRelevanceThreshold = 0.6m;

    /// <summary>
    /// Ĭ�����Ŷ���ֵ
    /// </summary>
    public const decimal DefaultConfidenceThreshold = 0.7m;

    /// <summary>
    /// Ĭ��Ȩ��ֵ
    /// </summary>
    public const decimal DefaultWeight = 1.0m;

    /// <summary>
    /// Ĭ��CPU����ȼ�
    /// </summary>
    public const int DefaultCpuRequirement = 5;

    /// <summary>
    /// Ĭ���ڴ�����MB��
    /// </summary>
    public const int DefaultMemoryRequirementMB = 100;

    /// <summary>
    /// Ĭ����������ȼ�
    /// </summary>
    public const int DefaultNetworkRequirement = 5;

    /// <summary>
    /// Ĭ����󲢷���
    /// </summary>
    public const int DefaultMaxConcurrency = 10;
}

/// <summary>
/// ��ʱ����
/// </summary>
public static class TimeoutConstants
{
    /// <summary>
    /// ���ٲ�����ʱʱ��
    /// </summary>
    public static readonly TimeSpan FastOperation = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// ����������ʱʱ��
    /// </summary>
    public static readonly TimeSpan NormalOperation = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// ���ٲ�����ʱʱ��
    /// </summary>
    public static readonly TimeSpan SlowOperation = TimeSpan.FromSeconds(2);

    /// <summary>
    /// ���ݿ��ѯ��ʱʱ��
    /// </summary>
    public static readonly TimeSpan DatabaseQuery = TimeSpan.FromSeconds(5);

    /// <summary>
    /// ������ó�ʱʱ��
    /// </summary>
    public static readonly TimeSpan NetworkCall = TimeSpan.FromSeconds(10);

    /// <summary>
    /// �ļ�������ʱʱ��
    /// </summary>
    public static readonly TimeSpan FileOperation = TimeSpan.FromSeconds(15);

    /// <summary>
    /// �����������ʱʱ��
    /// </summary>
    public static readonly TimeSpan BatchOperation = TimeSpan.FromSeconds(30);

    /// <summary>
    /// ��ʱ�����в�����ʱʱ��
    /// </summary>
    public static readonly TimeSpan LongRunningOperation = TimeSpan.FromMinutes(5);
}



