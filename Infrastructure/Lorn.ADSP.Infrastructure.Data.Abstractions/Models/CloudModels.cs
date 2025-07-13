using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ��������Ϣ
/// </summary>
public class CloudConnectionInfo
{
    /// <summary>
    /// ��ƽ̨����
    /// </summary>
    public CloudPlatform Platform { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public string AvailabilityZone { get; set; } = string.Empty;

    /// <summary>
    /// ������ԿID
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// ������Կ
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// �˵�URL
    /// </summary>
    public string EndpointUrl { get; set; } = string.Empty;

    /// <summary>
    /// ��Դ��ID
    /// </summary>
    public string ResourceGroupId { get; set; } = string.Empty;

    /// <summary>
    /// VPC ID
    /// </summary>
    public string VpcId { get; set; } = string.Empty;

    /// <summary>
    /// ����ID
    /// </summary>
    public string SubnetId { get; set; } = string.Empty;

    /// <summary>
    /// ��ȫ��ID
    /// </summary>
    public string SecurityGroupId { get; set; } = string.Empty;

    /// <summary>
    /// ��ǩ
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = [];

    /// <summary>
    /// SSL/TLS����
    /// </summary>
    public SslConfiguration SslConfiguration { get; set; } = new();
}

/// <summary>
/// SSL����
/// </summary>
public class SslConfiguration
{
    /// <summary>
    /// �Ƿ�����SSL
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// SSLģʽ
    /// </summary>
    public SslMode Mode { get; set; } = SslMode.Require;

    /// <summary>
    /// ֤���ļ�·��
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// ˽Կ�ļ�·��
    /// </summary>
    public string? PrivateKeyPath { get; set; }

    /// <summary>
    /// CA֤���ļ�·��
    /// </summary>
    public string? CaCertificatePath { get; set; }

    /// <summary>
    /// �Ƿ���֤֤��
    /// </summary>
    public bool VerifyCertificate { get; set; } = true;
}

/// <summary>
/// ���ݿ�汾��Ϣ
/// </summary>
public class DatabaseVersion
{
    /// <summary>
    /// �汾��
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// ���汾��
    /// </summary>
    public int MajorVersion { get; set; }

    /// <summary>
    /// �ΰ汾��
    /// </summary>
    public int MinorVersion { get; set; }

    /// <summary>
    /// ������
    /// </summary>
    public int BuildNumber { get; set; }

    /// <summary>
    /// �汾����
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// ֧�ֵĹ����б�
    /// </summary>
    public List<DatabaseFeature> SupportedFeatures { get; set; } = [];

    /// <summary>
    /// �Ƚϰ汾
    /// </summary>
    /// <param name="other">�����汾</param>
    /// <returns>�ȽϽ��</returns>
    public int CompareTo(DatabaseVersion other)
    {
        var majorComparison = MajorVersion.CompareTo(other.MajorVersion);
        if (majorComparison != 0) return majorComparison;

        var minorComparison = MinorVersion.CompareTo(other.MinorVersion);
        if (minorComparison != 0) return minorComparison;

        return BuildNumber.CompareTo(other.BuildNumber);
    }

    /// <summary>
    /// �Ƿ����ָ���汾
    /// </summary>
    /// <param name="targetVersion">Ŀ��汾</param>
    /// <returns>�Ƿ����</returns>
    public bool IsCompatibleWith(DatabaseVersion targetVersion)
    {
        return MajorVersion == targetVersion.MajorVersion &&
               CompareTo(targetVersion) >= 0;
    }
}

/// <summary>
/// ���ӽ���״̬
/// </summary>
public class ConnectionHealthStatus
{
    /// <summary>
    /// �Ƿ񽡿�
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// ״̬����
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// ��Ӧʱ�䣨���룩
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// ���ʱ��
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// ��ϸ��Ϣ
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = [];

    /// <summary>
    /// ��������״̬
    /// </summary>
    /// <param name="responseTimeMs">��Ӧʱ��</param>
    /// <returns>����״̬</returns>
    public static ConnectionHealthStatus Healthy(long responseTimeMs)
    {
        return new ConnectionHealthStatus
        {
            IsHealthy = true,
            Status = "Healthy",
            ResponseTimeMs = responseTimeMs
        };
    }

    /// <summary>
    /// ����������״̬
    /// </summary>
    /// <param name="errorMessage">������Ϣ</param>
    /// <param name="responseTimeMs">��Ӧʱ��</param>
    /// <returns>������״̬</returns>
    public static ConnectionHealthStatus Unhealthy(string errorMessage, long responseTimeMs = 0)
    {
        return new ConnectionHealthStatus
        {
            IsHealthy = false,
            Status = "Unhealthy",
            ErrorMessage = errorMessage,
            ResponseTimeMs = responseTimeMs
        };
    }
}

/// <summary>
/// ���ݿ�ʵ����Ϣ
/// </summary>
public class DatabaseInstanceInfo
{
    /// <summary>
    /// ʵ��ID
    /// </summary>
    public string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// ʵ������
    /// </summary>
    public string InstanceName { get; set; } = string.Empty;

    /// <summary>
    /// ���ݿ�����
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// ʵ��״̬
    /// </summary>
    public InstanceStatus Status { get; set; }

    /// <summary>
    /// ʵ�����
    /// </summary>
    public string InstanceClass { get; set; } = string.Empty;

    /// <summary>
    /// �洢��С��GB��
    /// </summary>
    public int StorageSize { get; set; }

    /// <summary>
    /// �洢����
    /// </summary>
    public StorageType StorageType { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ���Ӷ˵�
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// �˿�
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// �汾��Ϣ
    /// </summary>
    public DatabaseVersion Version { get; set; } = new();

    /// <summary>
    /// ��������
    /// </summary>
    public BackupConfiguration BackupConfiguration { get; set; } = new();

    /// <summary>
    /// ����ָ��
    /// </summary>
    public InstancePerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// ���ݿ�ʵ��ѡ��
/// </summary>
public class DatabaseInstanceOptions
{
    /// <summary>
    /// ʵ������
    /// </summary>
    public string InstanceName { get; set; } = string.Empty;

    /// <summary>
    /// ���ݿ�����
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// ���ݿ�汾
    /// </summary>
    public string EngineVersion { get; set; } = string.Empty;

    /// <summary>
    /// ʵ�����
    /// </summary>
    public string InstanceClass { get; set; } = string.Empty;

    /// <summary>
    /// �洢��С��GB��
    /// </summary>
    public int StorageSize { get; set; }

    /// <summary>
    /// �洢����
    /// </summary>
    public StorageType StorageType { get; set; } = StorageType.SSD;

    /// <summary>
    /// ���û���
    /// </summary>
    public string MasterUsername { get; set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public string MasterPassword { get; set; } = string.Empty;

    /// <summary>
    /// VPC ID
    /// </summary>
    public string VpcId { get; set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public string SubnetGroup { get; set; } = string.Empty;

    /// <summary>
    /// ��ȫ���б�
    /// </summary>
    public List<string> SecurityGroups { get; set; } = [];

    /// <summary>
    /// �Ƿ�������
    /// </summary>
    public bool MultiAZ { get; set; } = false;

    /// <summary>
    /// �Ƿ񹫿�����
    /// </summary>
    public bool PubliclyAccessible { get; set; } = false;

    /// <summary>
    /// ���ݱ����ڣ��죩
    /// </summary>
    public int BackupRetentionDays { get; set; } = 7;

    /// <summary>
    /// ά������
    /// </summary>
    public string MaintenanceWindow { get; set; } = string.Empty;

    /// <summary>
    /// ��ǩ
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = [];
}

/// <summary>
/// ���ݿⰲȫѡ��
/// </summary>
public class DatabaseSecurityOptions
{
    /// <summary>
    /// �Ƿ����ü���
    /// </summary>
    public bool EncryptionEnabled { get; set; } = true;

    /// <summary>
    /// ������ԿID
    /// </summary>
    public string? EncryptionKeyId { get; set; }

    /// <summary>
    /// SSL����
    /// </summary>
    public SslConfiguration SslConfiguration { get; set; } = new();

    /// <summary>
    /// ����ǽ����
    /// </summary>
    public List<FirewallRule> FirewallRules { get; set; } = [];

    /// <summary>
    /// �������
    /// </summary>
    public AuditConfiguration AuditConfiguration { get; set; } = new();

    /// <summary>
    /// ���ʿ����б�
    /// </summary>
    public List<AccessControlRule> AccessControlRules { get; set; } = [];
}

/// <summary>
/// ����ǽ����
/// </summary>
public class FirewallRule
{
    /// <summary>
    /// ��������
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ԴIP��Χ
    /// </summary>
    public string SourceIpRange { get; set; } = string.Empty;

    /// <summary>
    /// Ŀ��˿�
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Э��
    /// </summary>
    public string Protocol { get; set; } = "TCP";

    /// <summary>
    /// ����
    /// </summary>
    public FirewallAction Action { get; set; } = FirewallAction.Allow;
}

/// <summary>
/// �������
/// </summary>
public class AuditConfiguration
{
    /// <summary>
    /// �Ƿ��������
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// �����־�洢λ��
    /// </summary>
    public string LogStorageLocation { get; set; } = string.Empty;

    /// <summary>
    /// ��Ƽ���
    /// </summary>
    public AuditLevel Level { get; set; } = AuditLevel.Standard;

    /// <summary>
    /// ����¼�����
    /// </summary>
    public List<AuditEventType> EventTypes { get; set; } = [];
}

/// <summary>
/// ���ʿ��ƹ���
/// </summary>
public class AccessControlRule
{
    /// <summary>
    /// ��������
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// �û����ɫ
    /// </summary>
    public string Principal { get; set; } = string.Empty;

    /// <summary>
    /// Ȩ���б�
    /// </summary>
    public List<string> Permissions { get; set; } = [];

    /// <summary>
    /// ��Դ·��
    /// </summary>
    public string ResourcePath { get; set; } = string.Empty;

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool Allow { get; set; } = true;
}

/// <summary>
/// ��������
/// </summary>
public class BackupConfiguration
{
    /// <summary>
    /// �Ƿ������Զ�����
    /// </summary>
    public bool AutoBackupEnabled { get; set; } = true;

    /// <summary>
    /// ���ݱ����ڣ��죩
    /// </summary>
    public int RetentionDays { get; set; } = 7;

    /// <summary>
    /// ���ݴ���
    /// </summary>
    public string BackupWindow { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public BackupType BackupType { get; set; } = BackupType.Full;

    /// <summary>
    /// ����ѹ��
    /// </summary>
    public bool CompressionEnabled { get; set; } = true;

    /// <summary>
    /// ���ݼ���
    /// </summary>
    public bool EncryptionEnabled { get; set; } = true;
}

/// <summary>
/// ʵ������ָ��
/// </summary>
public class InstancePerformanceMetrics
{
    /// <summary>
    /// CPUʹ���ʣ�%��
    /// </summary>
    public double CpuUtilization { get; set; }

    /// <summary>
    /// �ڴ�ʹ���ʣ�%��
    /// </summary>
    public double MemoryUtilization { get; set; }

    /// <summary>
    /// �洢ʹ���ʣ�%��
    /// </summary>
    public double StorageUtilization { get; set; }

    /// <summary>
    /// ������
    /// </summary>
    public int ConnectionCount { get; set; }

    /// <summary>
    /// ��IOPS
    /// </summary>
    public double ReadIOPS { get; set; }

    /// <summary>
    /// дIOPS
    /// </summary>
    public double WriteIOPS { get; set; }

    /// <summary>
    /// ��������ֽ���
    /// </summary>
    public long NetworkReceiveBytes { get; set; }

    /// <summary>
    /// ���紫���ֽ���
    /// </summary>
    public long NetworkTransmitBytes { get; set; }

    /// <summary>
    /// ָ���ռ�ʱ��
    /// </summary>
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}