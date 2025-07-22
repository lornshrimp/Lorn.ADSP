using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;

/// <summary>
/// 数据访问路由器接口
/// 负责根据上下文信息智能选择合适的数据提供者或构建提供者执行链
/// </summary>
public interface IDataAccessRouter
{
    /// <summary>
    /// 异步路由到合适的数据提供者
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>选中的数据提供者</returns>
    Task<IDataAccessProvider> RouteAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取候选数据提供者列表
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>候选数据提供者列表</returns>
    Task<IEnumerable<IDataAccessProvider>> GetCandidateProvidersAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步构建数据访问提供者链
    /// 根据路由策略创建有序的提供者执行链，支持缓存优先和自动回写
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>构建的提供者执行链</returns>
    Task<IDataAccessProviderChain> BuildProviderChainAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步更新路由规则
    /// </summary>
    /// <param name="rules">路由规则数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task UpdateRoutingRulesAsync(RoutingRule[] rules, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取路由决策详情
    /// </summary>
    /// <param name="context">数据访问上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路由决策结果</returns>
    Task<RoutingDecision> GetRoutingDecisionAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取当前路由规则
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>当前路由规则集合</returns>
    Task<IEnumerable<RoutingRule>> GetRoutingRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步添加路由规则
    /// </summary>
    /// <param name="rule">路由规则</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task AddRoutingRuleAsync(RoutingRule rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步移除路由规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果成功移除则返回true，否则返回false</returns>
    Task<bool> RemoveRoutingRuleAsync(string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步清空所有路由规则
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task ClearRoutingRulesAsync(CancellationToken cancellationToken = default);
}
