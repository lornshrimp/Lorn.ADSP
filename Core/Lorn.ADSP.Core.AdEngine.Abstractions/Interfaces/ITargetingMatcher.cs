using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 定向匹配器接口
/// 定义定向匹配器的核心功能，用于判断用户是否符合特定的定向条件
/// </summary>
public interface ITargetingMatcher
{
    /// <summary>
    /// 匹配器唯一标识符
    /// </summary>
    string MatcherId { get; }

    /// <summary>
    /// 匹配器名称
    /// </summary>
    string MatcherName { get; }

    /// <summary>
    /// 匹配器版本
    /// </summary>
    string Version { get; }

    /// <summary>
    /// 匹配器类型（如：demographic, geolocation, time, device等）
    /// </summary>
    string MatcherType { get; }

    /// <summary>
    /// 执行优先级（数值越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 匹配器是否启用
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// 预期执行时间
    /// </summary>
    TimeSpan ExpectedExecutionTime { get; }

    /// <summary>
    /// 是否支持并行执行
    /// </summary>
    bool CanRunInParallel { get; }

    /// <summary>
    /// 计算匹配评分
    /// 根据用户上下文和定向条件计算匹配度评分
    /// </summary>
    /// <param name="context">定向上下文，包含用户画像信息</param>
    /// <param name="criteria">定向条件</param>
    /// <param name="callbackProvider">回调提供者，用于获取额外数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配结果，包含评分、匹配状态、执行时间等信息</returns>
    Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否支持指定的定向条件类型
    /// </summary>
    /// <param name="criteriaType">定向条件类型</param>
    /// <returns>是否支持该类型的定向条件</returns>
    bool IsSupported(string criteriaType);

    /// <summary>
    /// 验证定向条件的有效性
    /// </summary>
    /// <param name="criteria">定向条件</param>
    /// <returns>验证结果</returns>
    ValidationResult ValidateCriteria(ITargetingCriteria criteria);

    /// <summary>
    /// 获取匹配器元数据信息
    /// </summary>
    /// <returns>匹配器元数据</returns>
    TargetingMatcherMetadata GetMetadata();

    /// <summary>
    /// 预热匹配器（可选实现）
    /// 用于预加载数据、初始化缓存等操作
    /// </summary>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预热任务</returns>
    Task WarmUpAsync(ICallbackProvider callbackProvider, CancellationToken cancellationToken = default)
    {
        // 默认实现：无需预热
        return Task.CompletedTask;
    }

    /// <summary>
    /// 清理匹配器资源（可选实现）
    /// </summary>
    /// <returns>清理任务</returns>
    Task CleanupAsync()
    {
        // 默认实现：无需清理
        return Task.CompletedTask;
    }
}