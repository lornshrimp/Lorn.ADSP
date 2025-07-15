namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

/// <summary>
/// ���ݿ�����ö��
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// SQL Server
    /// </summary>
    SqlServer = 1,

    /// <summary>
    /// MySQL
    /// </summary>
    MySQL = 2,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSQL = 3,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite = 4,

    /// <summary>
    /// Oracle
    /// </summary>
    Oracle = 5,

    /// <summary>
    /// PolarDB
    /// </summary>
    PolarDB = 6,

    /// <summary>
    /// Redis
    /// </summary>
    Redis = 10,

    /// <summary>
    /// MongoDB
    /// </summary>
    MongoDB = 11,

    /// <summary>
    /// InfluxDB
    /// </summary>
    InfluxDB = 12,

    /// <summary>
    /// Elasticsearch
    /// </summary>
    Elasticsearch = 13
}

/// <summary>
/// ��ƽ̨����ö��
/// </summary>
public enum CloudPlatform
{
    /// <summary>
    /// ���ز���
    /// </summary>
    OnPremise = 1,

    /// <summary>
    /// ������
    /// </summary>
    AlibabaCloud = 2,

    /// <summary>
    /// ΢��Azure
    /// </summary>
    Azure = 3,

    /// <summary>
    /// ����ѷAWS
    /// </summary>
    AWS = 4,

    /// <summary>
    /// ��Ѷ��
    /// </summary>
    TencentCloud = 5,

    /// <summary>
    /// ��Ϊ��
    /// </summary>
    HuaweiCloud = 6,

    /// <summary>
    /// �ٶ���
    /// </summary>
    BaiduCloud = 7
}

/// <summary>
/// ����״̬ö��
/// </summary>
public enum TransactionStatus
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
    /// ���ύ
    /// </summary>
    Committed = 3,

    /// <summary>
    /// �ѻع�
    /// </summary>
    RolledBack = 4,

    /// <summary>
    /// ׼����
    /// </summary>
    Preparing = 5,

    /// <summary>
    /// ��׼��
    /// </summary>
    Prepared = 6,

    /// <summary>
    /// �ύ��
    /// </summary>
    Committing = 7,

    /// <summary>
    /// �ع���
    /// </summary>
    RollingBack = 8,

    /// <summary>
    /// ����ֹ
    /// </summary>
    Aborted = 9,

    /// <summary>
    /// ��ʱ
    /// </summary>
    Timeout = 10
}

/// <summary>
/// ���ݿ⹦��ö��
/// </summary>
public enum DatabaseFeature
{
    /// <summary>
    /// ����֧��
    /// </summary>
    Transactions = 1,

    /// <summary>
    /// ���Լ��
    /// </summary>
    ForeignKeys = 2,

    /// <summary>
    /// �洢����
    /// </summary>
    StoredProcedures = 3,

    /// <summary>
    /// ������
    /// </summary>
    Triggers = 4,

    /// <summary>
    /// ��ͼ
    /// </summary>
    Views = 5,

    /// <summary>
    /// ����
    /// </summary>
    Indexes = 6,

    /// <summary>
    /// ȫ������
    /// </summary>
    FullTextSearch = 7,

    /// <summary>
    /// JSON֧��
    /// </summary>
    JsonSupport = 8,

    /// <summary>
    /// XML֧��
    /// </summary>
    XmlSupport = 9,

    /// <summary>
    /// ���ں���
    /// </summary>
    WindowFunctions = 10,

    /// <summary>
    /// ���ñ���ʽ(CTE)
    /// </summary>
    CommonTableExpressions = 11,

    /// <summary>
    /// �ݹ��ѯ
    /// </summary>
    RecursiveQueries = 12,

    /// <summary>
    /// ������
    /// </summary>
    TablePartitioning = 13,

    /// <summary>
    /// ��д����
    /// </summary>
    ReadWriteSeparation = 14,

    /// <summary>
    /// ����ѹ��
    /// </summary>
    DataCompression = 15,

    /// <summary>
    /// �м���ȫ
    /// </summary>
    RowLevelSecurity = 16,

    /// <summary>
    /// �м�����
    /// </summary>
    ColumnEncryption = 17,

    /// <summary>
    /// �����־
    /// </summary>
    AuditLogging = 18,

    /// <summary>
    /// ���ݻָ�
    /// </summary>
    BackupRestore = 19,

    /// <summary>
    /// �߿���
    /// </summary>
    HighAvailability = 20
}

/// <summary>
/// ��������ö��
/// </summary>
public enum JoinType
{
    /// <summary>
    /// ������
    /// </summary>
    Inner = 1,

    /// <summary>
    /// ��������
    /// </summary>
    LeftOuter = 2,

    /// <summary>
    /// ��������
    /// </summary>
    RightOuter = 3,

    /// <summary>
    /// ȫ������
    /// </summary>
    FullOuter = 4,

    /// <summary>
    /// ��������
    /// </summary>
    Cross = 5
}

/// <summary>
/// ������ö��
/// </summary>
public enum OrderDirection
{
    /// <summary>
    /// ����
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// ����
    /// </summary>
    Descending = 2
}

/// <summary>
/// SSLģʽö��
/// </summary>
public enum SslMode
{
    /// <summary>
    /// ����SSL
    /// </summary>
    Disable = 1,

    /// <summary>
    /// ����ʹ��SSL
    /// </summary>
    Prefer = 2,

    /// <summary>
    /// Ҫ��SSL
    /// </summary>
    Require = 3,

    /// <summary>
    /// ��֤CA֤��
    /// </summary>
    VerifyCA = 4,

    /// <summary>
    /// ��֤�������
    /// </summary>
    VerifyFull = 5
}

/// <summary>
/// ʵ��״̬ö��
/// </summary>
public enum InstanceStatus
{
    /// <summary>
    /// ������
    /// </summary>
    Creating = 1,

    /// <summary>
    /// ������
    /// </summary>
    Running = 2,

    /// <summary>
    /// ֹͣ��
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// ��ֹͣ
    /// </summary>
    Stopped = 4,

    /// <summary>
    /// ������
    /// </summary>
    Restarting = 5,

    /// <summary>
    /// ά����
    /// </summary>
    Maintenance = 6,

    /// <summary>
    /// ������
    /// </summary>
    Backing = 7,

    /// <summary>
    /// �ָ���
    /// </summary>
    Restoring = 8,

    /// <summary>
    /// ����
    /// </summary>
    Failed = 9,

    /// <summary>
    /// ɾ����
    /// </summary>
    Deleting = 10,

    /// <summary>
    /// ��ɾ��
    /// </summary>
    Deleted = 11
}

/// <summary>
/// �洢����ö��
/// </summary>
public enum StorageType
{
    /// <summary>
    /// ��׼�洢
    /// </summary>
    Standard = 1,

    /// <summary>
    /// SSD�洢
    /// </summary>
    SSD = 2,

    /// <summary>
    /// ������SSD
    /// </summary>
    HighPerformanceSSD = 3,

    /// <summary>
    /// ��������SSD
    /// </summary>
    UltraHighPerformanceSSD = 4,

    /// <summary>
    /// ��洢
    /// </summary>
    Cold = 5,

    /// <summary>
    /// �鵵�洢
    /// </summary>
    Archive = 6
}

/// <summary>
/// ����ǽ����ö��
/// </summary>
public enum FirewallAction
{
    /// <summary>
    /// ����
    /// </summary>
    Allow = 1,

    /// <summary>
    /// �ܾ�
    /// </summary>
    Deny = 2,

    /// <summary>
    /// ��¼
    /// </summary>
    Log = 3
}

/// <summary>
/// ��Ƽ���ö��
/// </summary>
public enum AuditLevel
{
    /// <summary>
    /// �ر�
    /// </summary>
    Off = 1,

    /// <summary>
    /// ����
    /// </summary>
    Basic = 2,

    /// <summary>
    /// ��׼
    /// </summary>
    Standard = 3,

    /// <summary>
    /// ��ϸ
    /// </summary>
    Detailed = 4,

    /// <summary>
    /// ����
    /// </summary>
    Full = 5
}

/// <summary>
/// ����¼�����ö��
/// </summary>
public enum AuditEventType
{
    /// <summary>
    /// ��¼�¼�
    /// </summary>
    Login = 1,

    /// <summary>
    /// �ǳ��¼�
    /// </summary>
    Logout = 2,

    /// <summary>
    /// ���ݲ�ѯ
    /// </summary>
    DataQuery = 3,

    /// <summary>
    /// �����޸�
    /// </summary>
    DataModification = 4,

    /// <summary>
    /// �ܹ����
    /// </summary>
    SchemaChange = 5,

    /// <summary>
    /// Ȩ�ޱ��
    /// </summary>
    PermissionChange = 6,

    /// <summary>
    /// ���ñ��
    /// </summary>
    ConfigurationChange = 7,

    /// <summary>
    /// ���ݲ���
    /// </summary>
    BackupOperation = 8,

    /// <summary>
    /// �ָ�����
    /// </summary>
    RestoreOperation = 9,

    /// <summary>
    /// ʧ�ܲ���
    /// </summary>
    FailedOperation = 10
}

/// <summary>
/// ��������ö��
/// </summary>
public enum BackupType
{
    /// <summary>
    /// ��������
    /// </summary>
    Full = 1,

    /// <summary>
    /// ��������
    /// </summary>
    Incremental = 2,

    /// <summary>
    /// ���챸��
    /// </summary>
    Differential = 3,

    /// <summary>
    /// ��־����
    /// </summary>
    Log = 4,

    /// <summary>
    /// ���ձ���
    /// </summary>
    Snapshot = 5
}