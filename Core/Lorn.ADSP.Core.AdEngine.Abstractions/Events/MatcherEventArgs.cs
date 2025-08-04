using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Events;

/// <summary>
/// 匹配器注册事件参数
/// </summary>
public class MatcherRegisteredEventArgs : EventArgs
{
    /// <summary>
    /// 已注册的匹配器
    /// </summary>
    public required ITargetingMatcher Matcher { get; init; }

    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 注册来源
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// 是否为热注册（运行时注册）
    /// </summary>
    public bool IsHotRegistration { get; init; }
}

/// <summary>
/// 匹配器注销事件参数
/// </summary>
public class MatcherUnregisteredEventArgs : EventArgs
{
    /// <summary>
    /// 已注销的匹配器ID
    /// </summary>
    public required string MatcherId { get; init; }

    /// <summary>
    /// 匹配器名称
    /// </summary>
    public required string MatcherName { get; init; }

    /// <summary>
    /// 匹配器类型
    /// </summary>
    public required string MatcherType { get; init; }

    /// <summary>
    /// 注销时间
    /// </summary>
    public DateTime UnregisteredAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 注销原因
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// 是否为热注销（运行时注销）
    /// </summary>
    public bool IsHotUnregistration { get; init; }
}

/// <summary>
/// 匹配器状态变更事件参数
/// </summary>
public class MatcherStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// 匹配器ID
    /// </summary>
    public required string MatcherId { get; init; }

    /// <summary>
    /// 匹配器名称
    /// </summary>
    public required string MatcherName { get; init; }

    /// <summary>
    /// 匹配器类型
    /// </summary>
    public required string MatcherType { get; init; }

    /// <summary>
    /// 旧状态
    /// </summary>
    public required string OldStatus { get; init; }

    /// <summary>
    /// 新状态
    /// </summary>
    public required string NewStatus { get; init; }

    /// <summary>
    /// 状态变更时间
    /// </summary>
    public DateTime StatusChangedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 状态变更原因
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// 相关的错误信息（如果有）
    /// </summary>
    public string? ErrorMessage { get; init; }
}