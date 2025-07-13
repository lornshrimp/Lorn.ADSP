using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 云连接信息
/// </summary>
public class CloudConnectionInfo
{
    /// <summary>
    /// 云平台类型
    /// </summary>
    public CloudPlatform Platform { get; set; }

    /// <summary>
    /// 地域
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 可用区
    /// </summary>
    public string AvailabilityZone { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥ID
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// 端点URL
    /// </summary>
    public string EndpointUrl { get; set; } = string.Empty;

    /// <summary>
    /// 资源组ID
    /// </summary>
    public string ResourceGroupId { get; set; } = string.Empty;

    /// <summary>
    /// VPC ID
    /// </summary>
    public string VpcId { get; set; } = string.Empty;

    /// <summary>
    /// 子网ID
    /// </summary>
    public string SubnetId { get; set; } = string.Empty;

    /// <summary>
    /// 安全组ID
    /// </summary>
    public string SecurityGroupId { get; set; } = string.Empty;

    /// <summary>
    /// 标签
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = [];

    /// <summary>
    /// SSL/TLS设置
    /// </summary>
    public SslConfiguration SslConfiguration { get; set; } = new();
}

/// <summary>
/// SSL配置
/// </summary>
public class SslConfiguration
{
    /// <summary>
    /// 是否启用SSL
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// SSL模式
    /// </summary>
    public SslMode Mode { get; set; } = SslMode.Require;

    /// <summary>
    /// 证书文件路径
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// 私钥文件路径
    /// </summary>
    public string? PrivateKeyPath { get; set; }

    /// <summary>
    /// CA证书文件路径
    /// </summary>
    public string? CaCertificatePath { get; set; }

    /// <summary>
    /// 是否验证证书
    /// </summary>
    public bool VerifyCertificate { get; set; } = true;
}

/// <summary>
/// 数据库版本信息
/// </summary>
public class DatabaseVersion
{
    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 主版本号
    /// </summary>
    public int MajorVersion { get; set; }

    /// <summary>
    /// 次版本号
    /// </summary>
    public int MinorVersion { get; set; }

    /// <summary>
    /// 构建号
    /// </summary>
    public int BuildNumber { get; set; }

    /// <summary>
    /// 版本描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 发布日期
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// 支持的功能列表
    /// </summary>
    public List<DatabaseFeature> SupportedFeatures { get; set; } = [];

    /// <summary>
    /// 比较版本
    /// </summary>
    /// <param name="other">其他版本</param>
    /// <returns>比较结果</returns>
    public int CompareTo(DatabaseVersion other)
    {
        var majorComparison = MajorVersion.CompareTo(other.MajorVersion);
        if (majorComparison != 0) return majorComparison;

        var minorComparison = MinorVersion.CompareTo(other.MinorVersion);
        if (minorComparison != 0) return minorComparison;

        return BuildNumber.CompareTo(other.BuildNumber);
    }

    /// <summary>
    /// 是否兼容指定版本
    /// </summary>
    /// <param name="targetVersion">目标版本</param>
    /// <returns>是否兼容</returns>
    public bool IsCompatibleWith(DatabaseVersion targetVersion)
    {
        return MajorVersion == targetVersion.MajorVersion &&
               CompareTo(targetVersion) >= 0;
    }
}

/// <summary>
/// 连接健康状态
/// </summary>
public class ConnectionHealthStatus
{
    /// <summary>
    /// 是否健康
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 响应时间（毫秒）
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// 检查时间
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 详细信息
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = [];

    /// <summary>
    /// 创建健康状态
    /// </summary>
    /// <param name="responseTimeMs">响应时间</param>
    /// <returns>健康状态</returns>
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
    /// 创建不健康状态
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="responseTimeMs">响应时间</param>
    /// <returns>不健康状态</returns>
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
/// 数据库实例信息
/// </summary>
public class DatabaseInstanceInfo
{
    /// <summary>
    /// 实例ID
    /// </summary>
    public string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// 实例名称
    /// </summary>
    public string InstanceName { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 实例状态
    /// </summary>
    public InstanceStatus Status { get; set; }

    /// <summary>
    /// 实例规格
    /// </summary>
    public string InstanceClass { get; set; } = string.Empty;

    /// <summary>
    /// 存储大小（GB）
    /// </summary>
    public int StorageSize { get; set; }

    /// <summary>
    /// 存储类型
    /// </summary>
    public StorageType StorageType { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 连接端点
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 版本信息
    /// </summary>
    public DatabaseVersion Version { get; set; } = new();

    /// <summary>
    /// 备份设置
    /// </summary>
    public BackupConfiguration BackupConfiguration { get; set; } = new();

    /// <summary>
    /// 性能指标
    /// </summary>
    public InstancePerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// 数据库实例选项
/// </summary>
public class DatabaseInstanceOptions
{
    /// <summary>
    /// 实例名称
    /// </summary>
    public string InstanceName { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 数据库版本
    /// </summary>
    public string EngineVersion { get; set; } = string.Empty;

    /// <summary>
    /// 实例规格
    /// </summary>
    public string InstanceClass { get; set; } = string.Empty;

    /// <summary>
    /// 存储大小（GB）
    /// </summary>
    public int StorageSize { get; set; }

    /// <summary>
    /// 存储类型
    /// </summary>
    public StorageType StorageType { get; set; } = StorageType.SSD;

    /// <summary>
    /// 主用户名
    /// </summary>
    public string MasterUsername { get; set; } = string.Empty;

    /// <summary>
    /// 主密码
    /// </summary>
    public string MasterPassword { get; set; } = string.Empty;

    /// <summary>
    /// VPC ID
    /// </summary>
    public string VpcId { get; set; } = string.Empty;

    /// <summary>
    /// 子网组
    /// </summary>
    public string SubnetGroup { get; set; } = string.Empty;

    /// <summary>
    /// 安全组列表
    /// </summary>
    public List<string> SecurityGroups { get; set; } = [];

    /// <summary>
    /// 是否多可用区
    /// </summary>
    public bool MultiAZ { get; set; } = false;

    /// <summary>
    /// 是否公开访问
    /// </summary>
    public bool PubliclyAccessible { get; set; } = false;

    /// <summary>
    /// 备份保留期（天）
    /// </summary>
    public int BackupRetentionDays { get; set; } = 7;

    /// <summary>
    /// 维护窗口
    /// </summary>
    public string MaintenanceWindow { get; set; } = string.Empty;

    /// <summary>
    /// 标签
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = [];
}

/// <summary>
/// 数据库安全选项
/// </summary>
public class DatabaseSecurityOptions
{
    /// <summary>
    /// 是否启用加密
    /// </summary>
    public bool EncryptionEnabled { get; set; } = true;

    /// <summary>
    /// 加密密钥ID
    /// </summary>
    public string? EncryptionKeyId { get; set; }

    /// <summary>
    /// SSL配置
    /// </summary>
    public SslConfiguration SslConfiguration { get; set; } = new();

    /// <summary>
    /// 防火墙规则
    /// </summary>
    public List<FirewallRule> FirewallRules { get; set; } = [];

    /// <summary>
    /// 审计配置
    /// </summary>
    public AuditConfiguration AuditConfiguration { get; set; } = new();

    /// <summary>
    /// 访问控制列表
    /// </summary>
    public List<AccessControlRule> AccessControlRules { get; set; } = [];
}

/// <summary>
/// 防火墙规则
/// </summary>
public class FirewallRule
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 源IP范围
    /// </summary>
    public string SourceIpRange { get; set; } = string.Empty;

    /// <summary>
    /// 目标端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 协议
    /// </summary>
    public string Protocol { get; set; } = "TCP";

    /// <summary>
    /// 动作
    /// </summary>
    public FirewallAction Action { get; set; } = FirewallAction.Allow;
}

/// <summary>
/// 审计配置
/// </summary>
public class AuditConfiguration
{
    /// <summary>
    /// 是否启用审计
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 审计日志存储位置
    /// </summary>
    public string LogStorageLocation { get; set; } = string.Empty;

    /// <summary>
    /// 审计级别
    /// </summary>
    public AuditLevel Level { get; set; } = AuditLevel.Standard;

    /// <summary>
    /// 审计事件类型
    /// </summary>
    public List<AuditEventType> EventTypes { get; set; } = [];
}

/// <summary>
/// 访问控制规则
/// </summary>
public class AccessControlRule
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 用户或角色
    /// </summary>
    public string Principal { get; set; } = string.Empty;

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = [];

    /// <summary>
    /// 资源路径
    /// </summary>
    public string ResourcePath { get; set; } = string.Empty;

    /// <summary>
    /// 是否允许
    /// </summary>
    public bool Allow { get; set; } = true;
}

/// <summary>
/// 备份配置
/// </summary>
public class BackupConfiguration
{
    /// <summary>
    /// 是否启用自动备份
    /// </summary>
    public bool AutoBackupEnabled { get; set; } = true;

    /// <summary>
    /// 备份保留期（天）
    /// </summary>
    public int RetentionDays { get; set; } = 7;

    /// <summary>
    /// 备份窗口
    /// </summary>
    public string BackupWindow { get; set; } = string.Empty;

    /// <summary>
    /// 备份类型
    /// </summary>
    public BackupType BackupType { get; set; } = BackupType.Full;

    /// <summary>
    /// 备份压缩
    /// </summary>
    public bool CompressionEnabled { get; set; } = true;

    /// <summary>
    /// 备份加密
    /// </summary>
    public bool EncryptionEnabled { get; set; } = true;
}

/// <summary>
/// 实例性能指标
/// </summary>
public class InstancePerformanceMetrics
{
    /// <summary>
    /// CPU使用率（%）
    /// </summary>
    public double CpuUtilization { get; set; }

    /// <summary>
    /// 内存使用率（%）
    /// </summary>
    public double MemoryUtilization { get; set; }

    /// <summary>
    /// 存储使用率（%）
    /// </summary>
    public double StorageUtilization { get; set; }

    /// <summary>
    /// 连接数
    /// </summary>
    public int ConnectionCount { get; set; }

    /// <summary>
    /// 读IOPS
    /// </summary>
    public double ReadIOPS { get; set; }

    /// <summary>
    /// 写IOPS
    /// </summary>
    public double WriteIOPS { get; set; }

    /// <summary>
    /// 网络接收字节数
    /// </summary>
    public long NetworkReceiveBytes { get; set; }

    /// <summary>
    /// 网络传输字节数
    /// </summary>
    public long NetworkTransmitBytes { get; set; }

    /// <summary>
    /// 指标收集时间
    /// </summary>
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}