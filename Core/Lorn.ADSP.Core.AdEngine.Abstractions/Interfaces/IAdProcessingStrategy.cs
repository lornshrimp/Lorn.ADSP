using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告处理策略统一接口
/// </summary>
public interface IAdProcessingStrategy
{
    /// <summary>
    /// 策略唯一标识符
    /// </summary>
    string StrategyId { get; }

    /// <summary>
    /// 策略版本信息
    /// </summary>
    string Version { get; }

    /// <summary>
    /// 策略类型
    /// </summary>
    StrategyType Type { get; }

    /// <summary>
    /// 执行优先级（数值越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 策略是否启用
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
    /// 执行策略
    /// </summary>
    /// <param name="context">广告处理上下文</param>
    /// <param name="candidates">广告候选列表</param>
    /// <param name="callbackProvider">回调提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>策略执行结果</returns>
    Task<StrategyResult> ExecuteAsync(
        AdContext context,
        IReadOnlyList<AdCandidate> candidates,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证策略配置
    /// </summary>
    /// <param name="config">策略配置</param>
    /// <returns>验证结果</returns>
    ValidationResult ValidateConfiguration(StrategyConfig config);

    /// <summary>
    /// 获取策略元数据
    /// </summary>
    /// <returns>策略元数据</returns>
    StrategyMetadata GetMetadata();
}