namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

/// <summary>
/// �����ṩ������ö��
/// ���ڱ�ʶ�ṩ�ߵ���Ҫ����
/// </summary>
public enum DataProviderType
{
    /// <summary>
    /// ҵ���߼��ṩ��
    /// �������ݡ��û����񡢶�����Ե�ҵ��ʵ���ṩ��
    /// </summary>
    BusinessLogic = 1,

    /// <summary>
    /// �����ṩ��
    /// ��Redis���ڴ滺���
    /// </summary>
    Cache = 2,

    /// <summary>
    /// ���ݿ��ṩ��
    /// ��SQL Server��MySQL��PostgreSQL��
    /// </summary>
    Database = 3,

    /// <summary>
    /// ��ƽ̨�ṩ��
    /// �簢���ơ�Azure��AWS��
    /// </summary>
    Cloud = 4,

    /// <summary>
    /// �ⲿ�����ṩ��
    /// �������API��Web�����
    /// </summary>
    ExternalService = 5,

    /// <summary>
    /// �ļ��洢�ṩ��
    /// �籾���ļ�ϵͳ������洢��
    /// </summary>
    FileStorage = 6,

    /// <summary>
    /// ��Ϣ�����ṩ��
    /// ��Redis���С�RabbitMQ��
    /// </summary>
    MessageQueue = 7,

    /// <summary>
    /// ���������ṩ��
    /// ��Elasticsearch��Solr��
    /// </summary>
    SearchEngine = 8,

    /// <summary>
    /// �����ṩ��
    /// �����ݿ�������������ֲ�ʽ����Э������
    /// </summary>
    Transaction = 9
}

/// <summary>
/// ����һ���Լ���ö��
/// </summary>
public enum DataConsistencyLevel
{
    /// <summary>
    /// ǿһ����
    /// Ҫ���ȡ���µ����ݣ������ڹؼ�ҵ�����
    /// </summary>
    Strong = 1,

    /// <summary>
    /// ����һ����
    /// �����ʱ���ڵ����ݲ�һ�£������ڴ󲿷ֲ�ѯ����
    /// </summary>
    Eventual = 2,

    /// <summary>
    /// �Ựһ����
    /// ��ͬһ�Ự�ڱ�֤һ����
    /// </summary>
    Session = 3,

    /// <summary>
    /// �н�һ����
    /// ��ָ��ʱ�䴰���ڱ�֤һ����
    /// </summary>
    Bounded = 4,

    /// <summary>
    /// ����һ����
    /// ��֤��ȡ�����ݰ汾��������
    /// </summary>
    Monotonic = 5
}

/// <summary>
/// �ֲ�ʽ����״̬ö��
/// </summary>
public enum DistributedTransactionStatus
{
    /// <summary>
    /// δ��ʼ
    /// </summary>
    NotStarted = 1,

    /// <summary>
    /// ��Ծ��
    /// </summary>
    Active = 2,

    /// <summary>
    /// ׼����
    /// </summary>
    Preparing = 3,

    /// <summary>
    /// ��׼��
    /// </summary>
    Prepared = 4,

    /// <summary>
    /// �ύ��
    /// </summary>
    Committing = 5,

    /// <summary>
    /// ���ύ
    /// </summary>
    Committed = 6,

    /// <summary>
    /// �ع���
    /// </summary>
    RollingBack = 7,

    /// <summary>
    /// �ѻع�
    /// </summary>
    RolledBack = 8,

    /// <summary>
    /// ����ֹ
    /// </summary>
    Aborted = 9,

    /// <summary>
    /// �����ύ
    /// </summary>
    PartiallyCommitted = 10,

    /// <summary>
    /// ���ֻع�
    /// </summary>
    PartiallyRolledBack = 11
}

/// <summary>
/// ��������ö��
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// ��������
    /// </summary>
    Local = 1,

    /// <summary>
    /// �ֲ�ʽ����
    /// </summary>
    Distributed = 2,

    /// <summary>
    /// Ƕ������
    /// </summary>
    Nested = 3,

    /// <summary>
    /// ֻ������
    /// </summary>
    ReadOnly = 4,

    /// <summary>
    /// ��������
    /// </summary>
    Compensating = 5
}

/// <summary>
/// ����������ö��
/// </summary>
public enum TransactionTechnology
{
    /// <summary>
    /// ��ϵ�����ݿ�����
    /// </summary>
    RelationalDatabase = 1,

    /// <summary>
    /// NoSQL���ݿ�����
    /// </summary>
    NoSqlDatabase = 2,

    /// <summary>
    /// ��ֵ�洢����
    /// </summary>
    KeyValueStore = 3,

    /// <summary>
    /// �ֲ�ʽ����Э����
    /// </summary>
    DistributedCoordinator = 4,

    /// <summary>
    /// Sagaģʽ
    /// </summary>
    Saga = 5,

    /// <summary>
    /// �¼���Դ
    /// </summary>
    EventSourcing = 6
}

/// <summary>
/// �������ȼ�ö��
/// </summary>
public enum TransactionPriority
{
    /// <summary>
    /// �����ȼ�
    /// </summary>
    Low = 1,

    /// <summary>
    /// �������ȼ�
    /// </summary>
    Normal = 2,

    /// <summary>
    /// �����ȼ�
    /// </summary>
    High = 3,

    /// <summary>
    /// �ؼ����ȼ�
    /// </summary>
    Critical = 4
}

/// <summary>
/// ����Χö��
/// </summary>
public enum TransactionScope
{
    /// <summary>
    /// ��һ�ṩ������
    /// </summary>
    Single = 1,

    /// <summary>
    /// ���ṩ������
    /// </summary>
    Multiple = 2,

    /// <summary>
    /// ���������
    /// </summary>
    CrossService = 3,

    /// <summary>
    /// ȫ������
    /// </summary>
    Global = 4
}

/// <summary>
/// һ���Լ���ö��
/// </summary>
public enum ConsistencyLevel
{
    /// <summary>
    /// ��һ����
    /// </summary>
    WeakConsistency = 1,

    /// <summary>
    /// ����һ����
    /// </summary>
    EventualConsistency = 2,

    /// <summary>
    /// ǿһ����
    /// </summary>
    StrongConsistency = 3,

    /// <summary>
    /// ���һ����
    /// </summary>
    CausalConsistency = 4,

    /// <summary>
    /// ������һ����
    /// </summary>
    MonotonicReadConsistency = 5,

    /// <summary>
    /// ����дһ����
    /// </summary>
    MonotonicWriteConsistency = 6
}

/// <summary>
/// Э������ö��
/// </summary>
public enum CoordinationStrategy
{
    /// <summary>
    /// ����ʽЭ��
    /// </summary>
    Centralized = 1,

    /// <summary>
    /// �ֲ�ʽЭ��
    /// </summary>
    Distributed = 2,

    /// <summary>
    /// �ֲ�Э��
    /// </summary>
    Hierarchical = 3,

    /// <summary>
    /// ��Ե�Э��
    /// </summary>
    PeerToPeer = 4
}

/// <summary>
/// ������������ö��
/// </summary>
public enum BatchOperationType
{
    /// <summary>
    /// ������ȡ
    /// </summary>
    BatchGet = 1,

    /// <summary>
    /// ��������
    /// </summary>
    BatchSet = 2,

    /// <summary>
    /// ����ɾ��
    /// </summary>
    BatchRemove = 3,

    /// <summary>
    /// ��������
    /// </summary>
    BatchUpdate = 4,

    /// <summary>
    /// ��������
    /// </summary>
    BatchInsert = 5,

    /// <summary>
    /// �����ϲ���Upsert��
    /// </summary>
    BatchUpsert = 6
}

/// <summary>
/// �������ȼ�ö��
/// </summary>
public enum RequestPriority
{
    /// <summary>
    /// �����ȼ�
    /// </summary>
    Low = 1,

    /// <summary>
    /// �������ȼ�
    /// </summary>
    Normal = 2,

    /// <summary>
    /// �����ȼ�
    /// </summary>
    High = 3,

    /// <summary>
    /// �ؼ����ȼ�
    /// </summary>
    Critical = 4
}

/// <summary>
/// ����״̬ö��
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// δ֪״̬
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// ����
    /// </summary>
    Healthy = 1,

    /// <summary>
    /// ����
    /// </summary>
    Warning = 2,

    /// <summary>
    /// ������
    /// </summary>
    Unhealthy = 3,

    /// <summary>
    /// ����
    /// </summary>
    Offline = 4
}

/// <summary>
/// �������ö��
/// </summary>
public enum CacheStrategy
{
    /// <summary>
    /// д��͸
    /// д����ͬʱ���»���ʹ洢
    /// </summary>
    WriteThrough = 1,

    /// <summary>
    /// д��
    /// д���������»��棬�ӳ�д��洢
    /// </summary>
    WriteBack = 2,

    /// <summary>
    /// ��·����
    /// Ӧ�ó���ֱ�ӹ�����ʹ洢��ͬ��
    /// </summary>
    CacheAside = 3,

    /// <summary>
    /// ֻд����
    /// ��д�뻺�棬��д��洢
    /// </summary>
    WriteOnly = 4,

    /// <summary>
    /// ˢ����ǰ
    /// �ڻ������ǰ����ˢ��
    /// </summary>
    RefreshAhead = 5
}

/// <summary>
/// �˱ܲ���ö��
/// </summary>
public enum BackoffStrategy
{
    /// <summary>
    /// �̶����
    /// </summary>
    Fixed = 1,

    /// <summary>
    /// �����˱�
    /// </summary>
    Linear = 2,

    /// <summary>
    /// ָ���˱�
    /// </summary>
    Exponential = 3,

    /// <summary>
    /// ����˱�
    /// </summary>
    Random = 4
}

/// <summary>
/// ·�ɶ�������ö��
/// </summary>
public enum RouteActionType
{
    /// <summary>
    /// ѡ���ṩ��
    /// </summary>
    SelectProvider = 1,

    /// <summary>
    /// ���ؾ���
    /// </summary>
    LoadBalance = 2,

    /// <summary>
    /// ����ת��
    /// </summary>
    Failover = 3,

    /// <summary>
    /// �ܾ�����
    /// </summary>
    Reject = 4,

    /// <summary>
    /// �ض���
    /// </summary>
    Redirect = 5,

    /// <summary>
    /// ��������
    /// </summary>
    Degrade = 6
}

/// <summary>
/// ��������ö��
/// </summary>
public enum FallbackStrategy
{
    /// <summary>
    /// ��һ�������ṩ��
    /// </summary>
    NextAvailable = 1,

    /// <summary>
    /// ���ػ�������
    /// </summary>
    ReturnCached = 2,

    /// <summary>
    /// ����Ĭ��ֵ
    /// </summary>
    ReturnDefault = 3,

    /// <summary>
    /// �׳��쳣
    /// </summary>
    ThrowException = 4,

    /// <summary>
    /// ����
    /// </summary>
    Retry = 5,

    /// <summary>
    /// �۶�
    /// </summary>
    CircuitBreaker = 6
}

/// <summary>
/// Ԥ���ز���ö��
/// </summary>
public enum PreloadStrategy
{
    /// <summary>
    /// ����Ԥ����
    /// </summary>
    Normal = 1,

    /// <summary>
    /// ����Ԥ����
    /// </summary>
    Aggressive = 2,

    /// <summary>
    /// ����Ԥ����
    /// </summary>
    Conservative = 3,

    /// <summary>
    /// ����Ԥ����
    /// </summary>
    Intelligent = 4
}