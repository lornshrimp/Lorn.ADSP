using Lorn.ADSP.Core.AdEngine.Abstractions.Events;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 定向匹配器管理器接口
/// 负责管理多个定向匹配器的注册、发现和协调执行
/// </summary>
public interface ITargetingMatcherManager
{
    /// <summary>
    /// 获取所有已注册的匹配器
    /// </summary>
    /// <returns>匹配器列表</returns>
    IReadOnlyList<ITargetingMatcher> GetAllMatchers();

    /// <summary>
    /// 根据匹配器ID获取匹配器
    /// </summary>
    /// <param name="matcherId">匹配器ID</param>
    /// <returns>匹配器实例，如果不存在则返回null</returns>
    ITargetingMatcher? GetMatcher(string matcherId);

    /// <summary>
    /// 根据匹配器类型获取匹配器列表
    /// </summary>
    /// <param name="matcherType">匹配器类型</param>
    /// <returns>匹配器列表</returns>
    IReadOnlyList<ITargetingMatcher> GetMatchersByType(string matcherType);

    /// <summary>
    /// 获取支持指定定向条件类型的匹配器列表
    /// </summary>
    /// <param name="criteriaType">定向条件类型</param>
    /// <returns>支持该条件类型的匹配器列表</returns>
    IReadOnlyList<ITargetingMatcher> GetSupportedMatchers(string criteriaType);

    /// <summary>
    /// 注册匹配器
    /// </summary>
    /// <param name="matcher">要注册的匹配器</param>
    /// <returns>注册是否成功</returns>
    bool RegisterMatcher(ITargetingMatcher matcher);

    /// <summary>
    /// 注销匹配器
    /// </summary>
    /// <param name="matcherId">要注销的匹配器ID</param>
    /// <returns>注销是否成功</returns>
    bool UnregisterMatcher(string matcherId);

    /// <summary>
    /// 执行多维度定向匹配
    /// 并行执行所有启用的匹配器，并合并结果
    /// </summary>
    /// <param name="context">定向上下文</param>
    /// <param name="criteriaList">定向条件列表</param>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>整体匹配结果</returns>
    Task<OverallMatchResult> ExecuteMatchingAsync(
        ITargetingContext context,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量执行定向匹配
    /// 对多个上下文执行批量匹配，优化共同计算部分
    /// </summary>
    /// <param name="contexts">定向上下文列表</param>
    /// <param name="criteriaList">定向条件列表</param>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量匹配结果</returns>
    Task<IReadOnlyList<OverallMatchResult>> ExecuteBatchMatchingAsync(
        IReadOnlyList<ITargetingContext> contexts,
        IReadOnlyList<ITargetingCriteria> criteriaList,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证所有匹配器的配置
    /// </summary>
    /// <returns>验证结果列表</returns>
    IReadOnlyList<ValidationResult> ValidateAllMatchers();

    /// <summary>
    /// 预热所有匹配器
    /// </summary>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预热任务</returns>
    Task WarmUpAllMatchersAsync(ICallbackProvider callbackProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理所有匹配器资源
    /// </summary>
    /// <returns>清理任务</returns>
    Task CleanupAllMatchersAsync();

    /// <summary>
    /// 获取匹配器统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    TargetingMatcherStatistics GetStatistics();

    /// <summary>
    /// 匹配器注册事件
    /// </summary>
    event EventHandler<MatcherRegisteredEventArgs>? MatcherRegistered;

    /// <summary>
    /// 匹配器注销事件
    /// </summary>
    event EventHandler<MatcherUnregisteredEventArgs>? MatcherUnregistered;

    /// <summary>
    /// 匹配器状态变更事件
    /// </summary>
    event EventHandler<MatcherStatusChangedEventArgs>? MatcherStatusChanged;
}