namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

/// <summary>
/// 数据提供者类型枚举
/// 用于标识提供者的主要分类
/// </summary>
public enum DataProviderType
{
    /// <summary>
    /// 业务逻辑提供者
    /// 如广告数据、用户画像、定向策略等业务实体提供者
    /// </summary>
    BusinessLogic = 1,

    /// <summary>
    /// 缓存提供者
    /// 如Redis、内存缓存等
    /// </summary>
    Cache = 2,

    /// <summary>
    /// 数据库提供者
    /// 如SQL Server、MySQL、PostgreSQL等
    /// </summary>
    Database = 3,

    /// <summary>
    /// 云平台提供者
    /// 如阿里云、Azure、AWS等
    /// </summary>
    Cloud = 4,

    /// <summary>
    /// 外部服务提供者
    /// 如第三方API、Web服务等
    /// </summary>
    ExternalService = 5,

    /// <summary>
    /// 文件存储提供者
    /// 如本地文件系统、对象存储等
    /// </summary>
    FileStorage = 6,

    /// <summary>
    /// 消息队列提供者
    /// 如Redis队列、RabbitMQ等
    /// </summary>
    MessageQueue = 7,

    /// <summary>
    /// 搜索引擎提供者
    /// 如Elasticsearch、Solr等
    /// </summary>
    SearchEngine = 8,

    /// <summary>
    /// 事务提供者
    /// 如数据库事务管理器、分布式事务协调器等
    /// </summary>
    Transaction = 9
}

/// <summary>
/// 数据一致性级别枚举
/// </summary>
public enum DataConsistencyLevel
{
    /// <summary>
    /// 强一致性
    /// 要求读取最新的数据，适用于关键业务操作
    /// </summary>
    Strong = 1,

    /// <summary>
    /// 最终一致性
    /// 允许短时间内的数据不一致，适用于大部分查询操作
    /// </summary>
    Eventual = 2,

    /// <summary>
    /// 会话一致性
    /// 在同一会话内保证一致性
    /// </summary>
    Session = 3,

    /// <summary>
    /// 有界一致性
    /// 在指定时间窗口内保证一致性
    /// </summary>
    Bounded = 4,

    /// <summary>
    /// 单调一致性
    /// 保证读取的数据版本单调递增
    /// </summary>
    Monotonic = 5
}

/// <summary>
/// 分布式事务状态枚举
/// </summary>
public enum DistributedTransactionStatus
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
    /// 准备中
    /// </summary>
    Preparing = 3,

    /// <summary>
    /// 已准备
    /// </summary>
    Prepared = 4,

    /// <summary>
    /// 提交中
    /// </summary>
    Committing = 5,

    /// <summary>
    /// 已提交
    /// </summary>
    Committed = 6,

    /// <summary>
    /// 回滚中
    /// </summary>
    RollingBack = 7,

    /// <summary>
    /// 已回滚
    /// </summary>
    RolledBack = 8,

    /// <summary>
    /// 已中止
    /// </summary>
    Aborted = 9,

    /// <summary>
    /// 部分提交
    /// </summary>
    PartiallyCommitted = 10,

    /// <summary>
    /// 部分回滚
    /// </summary>
    PartiallyRolledBack = 11
}

/// <summary>
/// 事务类型枚举
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// 本地事务
    /// </summary>
    Local = 1,

    /// <summary>
    /// 分布式事务
    /// </summary>
    Distributed = 2,

    /// <summary>
    /// 嵌套事务
    /// </summary>
    Nested = 3,

    /// <summary>
    /// 只读事务
    /// </summary>
    ReadOnly = 4,

    /// <summary>
    /// 补偿事务
    /// </summary>
    Compensating = 5
}

/// <summary>
/// 事务技术类型枚举
/// </summary>
public enum TransactionTechnology
{
    /// <summary>
    /// 关系型数据库事务
    /// </summary>
    RelationalDatabase = 1,

    /// <summary>
    /// NoSQL数据库事务
    /// </summary>
    NoSqlDatabase = 2,

    /// <summary>
    /// 键值存储事务
    /// </summary>
    KeyValueStore = 3,

    /// <summary>
    /// 分布式事务协调器
    /// </summary>
    DistributedCoordinator = 4,

    /// <summary>
    /// Saga模式
    /// </summary>
    Saga = 5,

    /// <summary>
    /// 事件溯源
    /// </summary>
    EventSourcing = 6
}

/// <summary>
/// 事务优先级枚举
/// </summary>
public enum TransactionPriority
{
    /// <summary>
    /// 低优先级
    /// </summary>
    Low = 1,

    /// <summary>
    /// 正常优先级
    /// </summary>
    Normal = 2,

    /// <summary>
    /// 高优先级
    /// </summary>
    High = 3,

    /// <summary>
    /// 关键优先级
    /// </summary>
    Critical = 4
}

/// <summary>
/// 事务范围枚举
/// </summary>
public enum TransactionScope
{
    /// <summary>
    /// 单一提供者事务
    /// </summary>
    Single = 1,

    /// <summary>
    /// 多提供者事务
    /// </summary>
    Multiple = 2,

    /// <summary>
    /// 跨服务事务
    /// </summary>
    CrossService = 3,

    /// <summary>
    /// 全局事务
    /// </summary>
    Global = 4
}

/// <summary>
/// 一致性级别枚举
/// </summary>
public enum ConsistencyLevel
{
    /// <summary>
    /// 弱一致性
    /// </summary>
    WeakConsistency = 1,

    /// <summary>
    /// 最终一致性
    /// </summary>
    EventualConsistency = 2,

    /// <summary>
    /// 强一致性
    /// </summary>
    StrongConsistency = 3,

    /// <summary>
    /// 因果一致性
    /// </summary>
    CausalConsistency = 4,

    /// <summary>
    /// 单调读一致性
    /// </summary>
    MonotonicReadConsistency = 5,

    /// <summary>
    /// 单调写一致性
    /// </summary>
    MonotonicWriteConsistency = 6
}

/// <summary>
/// 协调策略枚举
/// </summary>
public enum CoordinationStrategy
{
    /// <summary>
    /// 集中式协调
    /// </summary>
    Centralized = 1,

    /// <summary>
    /// 分布式协调
    /// </summary>
    Distributed = 2,

    /// <summary>
    /// 分层协调
    /// </summary>
    Hierarchical = 3,

    /// <summary>
    /// 点对点协调
    /// </summary>
    PeerToPeer = 4
}

/// <summary>
/// 批量操作类型枚举
/// </summary>
public enum BatchOperationType
{
    /// <summary>
    /// 批量获取
    /// </summary>
    BatchGet = 1,

    /// <summary>
    /// 批量设置
    /// </summary>
    BatchSet = 2,

    /// <summary>
    /// 批量删除
    /// </summary>
    BatchRemove = 3,

    /// <summary>
    /// 批量更新
    /// </summary>
    BatchUpdate = 4,

    /// <summary>
    /// 批量插入
    /// </summary>
    BatchInsert = 5,

    /// <summary>
    /// 批量合并（Upsert）
    /// </summary>
    BatchUpsert = 6
}

/// <summary>
/// 请求优先级枚举
/// </summary>
public enum RequestPriority
{
    /// <summary>
    /// 低优先级
    /// </summary>
    Low = 1,

    /// <summary>
    /// 正常优先级
    /// </summary>
    Normal = 2,

    /// <summary>
    /// 高优先级
    /// </summary>
    High = 3,

    /// <summary>
    /// 关键优先级
    /// </summary>
    Critical = 4
}

/// <summary>
/// 健康状态枚举
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// 未知状态
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 健康
    /// </summary>
    Healthy = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warning = 2,

    /// <summary>
    /// 不健康
    /// </summary>
    Unhealthy = 3,

    /// <summary>
    /// 离线
    /// </summary>
    Offline = 4
}

/// <summary>
/// 缓存策略枚举
/// </summary>
public enum CacheStrategy
{
    /// <summary>
    /// 写穿透
    /// 写操作同时更新缓存和存储
    /// </summary>
    WriteThrough = 1,

    /// <summary>
    /// 写回
    /// 写操作仅更新缓存，延迟写入存储
    /// </summary>
    WriteBack = 2,

    /// <summary>
    /// 旁路缓存
    /// 应用程序直接管理缓存和存储的同步
    /// </summary>
    CacheAside = 3,

    /// <summary>
    /// 只写缓存
    /// 仅写入缓存，不写入存储
    /// </summary>
    WriteOnly = 4,

    /// <summary>
    /// 刷新提前
    /// 在缓存过期前主动刷新
    /// </summary>
    RefreshAhead = 5
}

/// <summary>
/// 退避策略枚举
/// </summary>
public enum BackoffStrategy
{
    /// <summary>
    /// 固定间隔
    /// </summary>
    Fixed = 1,

    /// <summary>
    /// 线性退避
    /// </summary>
    Linear = 2,

    /// <summary>
    /// 指数退避
    /// </summary>
    Exponential = 3,

    /// <summary>
    /// 随机退避
    /// </summary>
    Random = 4
}

/// <summary>
/// 路由动作类型枚举
/// </summary>
public enum RouteActionType
{
    /// <summary>
    /// 选择提供者
    /// </summary>
    SelectProvider = 1,

    /// <summary>
    /// 负载均衡
    /// </summary>
    LoadBalance = 2,

    /// <summary>
    /// 故障转移
    /// </summary>
    Failover = 3,

    /// <summary>
    /// 拒绝请求
    /// </summary>
    Reject = 4,

    /// <summary>
    /// 重定向
    /// </summary>
    Redirect = 5,

    /// <summary>
    /// 降级服务
    /// </summary>
    Degrade = 6
}

/// <summary>
/// 降级策略枚举
/// </summary>
public enum FallbackStrategy
{
    /// <summary>
    /// 下一个可用提供者
    /// </summary>
    NextAvailable = 1,

    /// <summary>
    /// 返回缓存数据
    /// </summary>
    ReturnCached = 2,

    /// <summary>
    /// 返回默认值
    /// </summary>
    ReturnDefault = 3,

    /// <summary>
    /// 抛出异常
    /// </summary>
    ThrowException = 4,

    /// <summary>
    /// 重试
    /// </summary>
    Retry = 5,

    /// <summary>
    /// 熔断
    /// </summary>
    CircuitBreaker = 6
}

/// <summary>
/// 预加载策略枚举
/// </summary>
public enum PreloadStrategy
{
    /// <summary>
    /// 正常预加载
    /// </summary>
    Normal = 1,

    /// <summary>
    /// 激进预加载
    /// </summary>
    Aggressive = 2,

    /// <summary>
    /// 保守预加载
    /// </summary>
    Conservative = 3,

    /// <summary>
    /// 智能预加载
    /// </summary>
    Intelligent = 4
}