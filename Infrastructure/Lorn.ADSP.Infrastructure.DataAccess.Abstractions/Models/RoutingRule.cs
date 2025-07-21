using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 路由规则
/// 定义数据访问路由的决策规则
/// </summary>
public sealed class RoutingRule
{
    /// <summary>
    /// 规则唯一标识符
    /// </summary>
    public required string RuleId { get; init; }

    /// <summary>
    /// 规则名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 规则优先级（数值越大优先级越高）
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 匹配条件
    /// </summary>
    public required RoutingCondition Condition { get; init; }

    /// <summary>
    /// 路由策略
    /// </summary>
    public RoutingStrategy Strategy { get; init; } = RoutingStrategy.Priority;

    /// <summary>
    /// 目标提供者过滤条件
    /// </summary>
    public DataProviderQuery? TargetProviderFilter { get; init; }

    /// <summary>
    /// 扩展配置
    /// </summary>
    public Dictionary<string, object> ExtendedConfiguration { get; init; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// 路由条件
/// 定义触发路由规则的条件
/// </summary>
public sealed class RoutingCondition
{
    /// <summary>
    /// 操作类型匹配模式
    /// </summary>
    public string[]? OperationTypes { get; init; }

    /// <summary>
    /// 实体类型匹配模式
    /// </summary>
    public string[]? EntityTypes { get; init; }

    /// <summary>
    /// 标签匹配模式
    /// </summary>
    public string[]? Tags { get; init; }

    /// <summary>
    /// 一致性级别匹配
    /// </summary>
    public DataConsistencyLevel[]? ConsistencyLevels { get; init; }

    /// <summary>
    /// 用户ID匹配模式（支持正则表达式）
    /// </summary>
    public string? UserIdPattern { get; init; }

    /// <summary>
    /// 租户ID匹配模式（支持正则表达式）
    /// </summary>
    public string? TenantIdPattern { get; init; }

    /// <summary>
    /// 时间范围匹配
    /// </summary>
    public TimeRange? TimeRange { get; init; }

    /// <summary>
    /// 自定义条件表达式
    /// </summary>
    public string? CustomExpression { get; init; }

    /// <summary>
    /// 扩展条件
    /// </summary>
    public Dictionary<string, object> ExtendedConditions { get; init; } = new();
}

/// <summary>
/// 时间范围
/// </summary>
public sealed class TimeRange
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public TimeSpan? StartTime { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public TimeSpan? EndTime { get; init; }

    /// <summary>
    /// 星期几（1-7，1为星期一）
    /// </summary>
    public int[]? DaysOfWeek { get; init; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone { get; init; }
}
