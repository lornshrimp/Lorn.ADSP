namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

/// <summary>
/// 路由策略枚举
/// 定义数据访问路由的策略类型
/// </summary>
public enum RoutingStrategy
{
    /// <summary>
    /// 优先级路由 - 根据提供者优先级选择
    /// </summary>
    Priority,

    /// <summary>
    /// 轮询路由 - 轮流选择可用的提供者
    /// </summary>
    RoundRobin,

    /// <summary>
    /// 随机路由 - 随机选择可用的提供者
    /// </summary>
    Random,

    /// <summary>
    /// 负载均衡路由 - 根据负载情况选择提供者
    /// </summary>
    LoadBalance,

    /// <summary>
    /// 故障转移路由 - 主提供者失败时自动转移到备用提供者
    /// </summary>
    Failover,

    /// <summary>
    /// 自定义路由 - 基于自定义规则进行路由
    /// </summary>
    Custom
}
