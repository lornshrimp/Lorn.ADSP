namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

/// <summary>
/// 数据提供者类型枚举
/// 定义数据访问层中不同类型的数据提供者分类
/// </summary>
public enum DataProviderType
{
    /// <summary>
    /// 缓存提供者 - 如Redis、Memory Cache等
    /// </summary>
    Cache,

    /// <summary>
    /// 数据库提供者 - 如SQL Server、MySQL、PostgreSQL等
    /// </summary>
    Database,

    /// <summary>
    /// 云平台提供者 - 如阿里云、Azure、AWS等
    /// </summary>
    Cloud,

    /// <summary>
    /// 业务逻辑提供者 - 如广告数据提供者、用户档案提供者等
    /// </summary>
    BusinessLogic,

    /// <summary>
    /// 外部服务提供者 - 如第三方API、消息队列等
    /// </summary>
    ExternalService
}
