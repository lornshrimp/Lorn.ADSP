namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

/// <summary>
/// 数据库类型枚举
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
/// 云平台类型枚举
/// </summary>
public enum CloudPlatform
{
    /// <summary>
    /// 本地部署
    /// </summary>
    OnPremise = 1,

    /// <summary>
    /// 阿里云
    /// </summary>
    AlibabaCloud = 2,

    /// <summary>
    /// 微软Azure
    /// </summary>
    Azure = 3,

    /// <summary>
    /// 亚马逊AWS
    /// </summary>
    AWS = 4,

    /// <summary>
    /// 腾讯云
    /// </summary>
    TencentCloud = 5,

    /// <summary>
    /// 华为云
    /// </summary>
    HuaweiCloud = 6,

    /// <summary>
    /// 百度云
    /// </summary>
    BaiduCloud = 7
}

/// <summary>
/// 事务状态枚举
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted = 1,

    /// <summary>
    /// 活跃中
    /// </summary>
    Active = 2,

    /// <summary>
    /// 已提交
    /// </summary>
    Committed = 3,

    /// <summary>
    /// 已回滚
    /// </summary>
    RolledBack = 4,

    /// <summary>
    /// 准备中
    /// </summary>
    Preparing = 5,

    /// <summary>
    /// 已准备
    /// </summary>
    Prepared = 6,

    /// <summary>
    /// 提交中
    /// </summary>
    Committing = 7,

    /// <summary>
    /// 回滚中
    /// </summary>
    RollingBack = 8,

    /// <summary>
    /// 已中止
    /// </summary>
    Aborted = 9,

    /// <summary>
    /// 超时
    /// </summary>
    Timeout = 10
}

/// <summary>
/// 数据库功能枚举
/// </summary>
public enum DatabaseFeature
{
    /// <summary>
    /// 事务支持
    /// </summary>
    Transactions = 1,

    /// <summary>
    /// 外键约束
    /// </summary>
    ForeignKeys = 2,

    /// <summary>
    /// 存储过程
    /// </summary>
    StoredProcedures = 3,

    /// <summary>
    /// 触发器
    /// </summary>
    Triggers = 4,

    /// <summary>
    /// 视图
    /// </summary>
    Views = 5,

    /// <summary>
    /// 索引
    /// </summary>
    Indexes = 6,

    /// <summary>
    /// 全文搜索
    /// </summary>
    FullTextSearch = 7,

    /// <summary>
    /// JSON支持
    /// </summary>
    JsonSupport = 8,

    /// <summary>
    /// XML支持
    /// </summary>
    XmlSupport = 9,

    /// <summary>
    /// 窗口函数
    /// </summary>
    WindowFunctions = 10,

    /// <summary>
    /// 公用表表达式(CTE)
    /// </summary>
    CommonTableExpressions = 11,

    /// <summary>
    /// 递归查询
    /// </summary>
    RecursiveQueries = 12,

    /// <summary>
    /// 分区表
    /// </summary>
    TablePartitioning = 13,

    /// <summary>
    /// 读写分离
    /// </summary>
    ReadWriteSeparation = 14,

    /// <summary>
    /// 数据压缩
    /// </summary>
    DataCompression = 15,

    /// <summary>
    /// 行级安全
    /// </summary>
    RowLevelSecurity = 16,

    /// <summary>
    /// 列级加密
    /// </summary>
    ColumnEncryption = 17,

    /// <summary>
    /// 审计日志
    /// </summary>
    AuditLogging = 18,

    /// <summary>
    /// 备份恢复
    /// </summary>
    BackupRestore = 19,

    /// <summary>
    /// 高可用
    /// </summary>
    HighAvailability = 20
}

/// <summary>
/// 连接类型枚举
/// </summary>
public enum JoinType
{
    /// <summary>
    /// 内连接
    /// </summary>
    Inner = 1,

    /// <summary>
    /// 左外连接
    /// </summary>
    LeftOuter = 2,

    /// <summary>
    /// 右外连接
    /// </summary>
    RightOuter = 3,

    /// <summary>
    /// 全外连接
    /// </summary>
    FullOuter = 4,

    /// <summary>
    /// 交叉连接
    /// </summary>
    Cross = 5
}

/// <summary>
/// 排序方向枚举
/// </summary>
public enum OrderDirection
{
    /// <summary>
    /// 升序
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// 降序
    /// </summary>
    Descending = 2
}

/// <summary>
/// SSL模式枚举
/// </summary>
public enum SslMode
{
    /// <summary>
    /// 禁用SSL
    /// </summary>
    Disable = 1,

    /// <summary>
    /// 优先使用SSL
    /// </summary>
    Prefer = 2,

    /// <summary>
    /// 要求SSL
    /// </summary>
    Require = 3,

    /// <summary>
    /// 验证CA证书
    /// </summary>
    VerifyCA = 4,

    /// <summary>
    /// 验证完整身份
    /// </summary>
    VerifyFull = 5
}

/// <summary>
/// 实例状态枚举
/// </summary>
public enum InstanceStatus
{
    /// <summary>
    /// 创建中
    /// </summary>
    Creating = 1,

    /// <summary>
    /// 运行中
    /// </summary>
    Running = 2,

    /// <summary>
    /// 停止中
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// 已停止
    /// </summary>
    Stopped = 4,

    /// <summary>
    /// 重启中
    /// </summary>
    Restarting = 5,

    /// <summary>
    /// 维护中
    /// </summary>
    Maintenance = 6,

    /// <summary>
    /// 备份中
    /// </summary>
    Backing = 7,

    /// <summary>
    /// 恢复中
    /// </summary>
    Restoring = 8,

    /// <summary>
    /// 故障
    /// </summary>
    Failed = 9,

    /// <summary>
    /// 删除中
    /// </summary>
    Deleting = 10,

    /// <summary>
    /// 已删除
    /// </summary>
    Deleted = 11
}

/// <summary>
/// 存储类型枚举
/// </summary>
public enum StorageType
{
    /// <summary>
    /// 标准存储
    /// </summary>
    Standard = 1,

    /// <summary>
    /// SSD存储
    /// </summary>
    SSD = 2,

    /// <summary>
    /// 高性能SSD
    /// </summary>
    HighPerformanceSSD = 3,

    /// <summary>
    /// 超高性能SSD
    /// </summary>
    UltraHighPerformanceSSD = 4,

    /// <summary>
    /// 冷存储
    /// </summary>
    Cold = 5,

    /// <summary>
    /// 归档存储
    /// </summary>
    Archive = 6
}

/// <summary>
/// 防火墙动作枚举
/// </summary>
public enum FirewallAction
{
    /// <summary>
    /// 允许
    /// </summary>
    Allow = 1,

    /// <summary>
    /// 拒绝
    /// </summary>
    Deny = 2,

    /// <summary>
    /// 记录
    /// </summary>
    Log = 3
}

/// <summary>
/// 审计级别枚举
/// </summary>
public enum AuditLevel
{
    /// <summary>
    /// 关闭
    /// </summary>
    Off = 1,

    /// <summary>
    /// 基础
    /// </summary>
    Basic = 2,

    /// <summary>
    /// 标准
    /// </summary>
    Standard = 3,

    /// <summary>
    /// 详细
    /// </summary>
    Detailed = 4,

    /// <summary>
    /// 完整
    /// </summary>
    Full = 5
}

/// <summary>
/// 审计事件类型枚举
/// </summary>
public enum AuditEventType
{
    /// <summary>
    /// 登录事件
    /// </summary>
    Login = 1,

    /// <summary>
    /// 登出事件
    /// </summary>
    Logout = 2,

    /// <summary>
    /// 数据查询
    /// </summary>
    DataQuery = 3,

    /// <summary>
    /// 数据修改
    /// </summary>
    DataModification = 4,

    /// <summary>
    /// 架构变更
    /// </summary>
    SchemaChange = 5,

    /// <summary>
    /// 权限变更
    /// </summary>
    PermissionChange = 6,

    /// <summary>
    /// 配置变更
    /// </summary>
    ConfigurationChange = 7,

    /// <summary>
    /// 备份操作
    /// </summary>
    BackupOperation = 8,

    /// <summary>
    /// 恢复操作
    /// </summary>
    RestoreOperation = 9,

    /// <summary>
    /// 失败操作
    /// </summary>
    FailedOperation = 10
}

/// <summary>
/// 备份类型枚举
/// </summary>
public enum BackupType
{
    /// <summary>
    /// 完整备份
    /// </summary>
    Full = 1,

    /// <summary>
    /// 增量备份
    /// </summary>
    Incremental = 2,

    /// <summary>
    /// 差异备份
    /// </summary>
    Differential = 3,

    /// <summary>
    /// 日志备份
    /// </summary>
    Log = 4,

    /// <summary>
    /// 快照备份
    /// </summary>
    Snapshot = 5
}