using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 数据访问路由器接口
/// 负责根据数据访问上下文智能选择最合适的数据访问提供者
/// 实现缓存优先、数据库降级、云平台切换等路由策略
/// </summary>
public interface IDataAccessRouter
{
    /// <summary>
    /// 异步路由数据访问请求
    /// 根据上下文和配置的路由策略选择最优的数据访问提供者
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>选择的数据访问提供者</returns>
    Task<IDataAccessProvider?> RouteAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取候选数据访问提供者
    /// 返回所有可能处理该请求的提供者列表，按优先级排序
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>候选数据访问提供者集合</returns>
    Task<IEnumerable<IDataAccessProvider>> GetCandidateProvidersAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步更新路由规则
    /// 支持动态更新路由策略配置
    /// </summary>
    /// <param name="rules">新的路由规则数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新任务</returns>
    Task UpdateRoutingRulesAsync(RoutingRule[] rules, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取路由决策信息
    /// 提供详细的路由决策过程信息，用于调试和监控
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路由决策结果</returns>
    Task<RoutingDecision> GetRoutingDecisionAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册路由策略
    /// 支持自定义路由策略的注册
    /// </summary>
    /// <param name="strategy">路由策略</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>注册任务</returns>
    Task RegisterRoutingStrategyAsync(IRoutingStrategy strategy, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取路由统计信息
    /// 提供路由性能和健康状态信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路由统计信息</returns>
    Task<RoutingStatistics> GetRoutingStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试路由配置
    /// 验证路由规则的有效性
    /// </summary>
    /// <param name="testContext">测试上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>测试结果</returns>
    Task<RoutingTestResult> TestRoutingAsync(DataAccessContext testContext, CancellationToken cancellationToken = default);
}

/// <summary>
/// 路由策略接口
/// 定义自定义路由策略的标准规范
/// </summary>
public interface IRoutingStrategy
{
    /// <summary>
    /// 策略名称
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// 策略优先级
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 是否支持指定的上下文
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <returns>是否支持</returns>
    bool SupportsContext(DataAccessContext context);

    /// <summary>
    /// 选择数据访问提供者
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="candidates">候选提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>选择的提供者</returns>
    Task<IDataAccessProvider?> SelectProviderAsync(
        DataAccessContext context,
        IEnumerable<IDataAccessProvider> candidates,
        CancellationToken cancellationToken = default);
}