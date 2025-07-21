using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 路由决策结果
/// 包含路由器做出的决策信息和相关上下文
/// </summary>
public sealed class RoutingDecision
{
    /// <summary>
    /// 选中的数据提供者
    /// </summary>
    public required IDataAccessProvider SelectedProvider { get; init; }

    /// <summary>
    /// 候选提供者列表
    /// </summary>
    public IReadOnlyList<IDataAccessProvider> CandidateProviders { get; init; } = Array.Empty<IDataAccessProvider>();

    /// <summary>
    /// 应用的路由规则
    /// </summary>
    public RoutingRule? AppliedRule { get; init; }

    /// <summary>
    /// 路由原因
    /// </summary>
    public required string Reason { get; init; }

    /// <summary>
    /// 决策时间
    /// </summary>
    public DateTimeOffset DecisionTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 决策耗时（毫秒）
    /// </summary>
    public double DecisionDuration { get; init; }

    /// <summary>
    /// 是否为故障转移决策
    /// </summary>
    public bool IsFailoverDecision { get; init; }

    /// <summary>
    /// 置信度（0-1之间的值）
    /// </summary>
    public double Confidence { get; init; } = 1.0;

    /// <summary>
    /// 扩展信息
    /// </summary>
    public Dictionary<string, object> ExtendedInformation { get; init; } = new();
}
