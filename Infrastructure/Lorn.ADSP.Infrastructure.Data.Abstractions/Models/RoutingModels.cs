using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 路由决策结果
/// 包含路由决策过程的详细信息
/// </summary>
public class RoutingDecision
{
    /// <summary>
    /// 选择的数据访问提供者
    /// </summary>
    public IDataAccessProvider? SelectedProvider { get; set; }

    /// <summary>
    /// 候选提供者列表
    /// </summary>
    public IReadOnlyList<CandidateProvider> CandidateProviders { get; set; } = Array.Empty<CandidateProvider>();

    /// <summary>
    /// 应用的路由规则
    /// </summary>
    public IReadOnlyList<RoutingRule> AppliedRules { get; set; } = Array.Empty<RoutingRule>();

    /// <summary>
    /// 决策理由
    /// </summary>
    public string DecisionReason { get; set; } = string.Empty;

    /// <summary>
    /// 决策时间
    /// </summary>
    public DateTime DecisionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 决策耗时
    /// </summary>
    public TimeSpan DecisionDuration { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 路由上下文
    /// </summary>
    public DataAccessContext Context { get; set; } = new();

    /// <summary>
    /// 路由统计信息
    /// </summary>
    public Dictionary<string, object> Statistics { get; set; } = new();
}

/// <summary>
/// 候选提供者信息
/// </summary>
public class CandidateProvider
{
    /// <summary>
    /// 数据访问提供者
    /// </summary>
    public IDataAccessProvider Provider { get; set; } = null!;

    /// <summary>
    /// 匹配分数
    /// </summary>
    public decimal MatchScore { get; set; }

    /// <summary>
    /// 是否可用
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// 不可用原因
    /// </summary>
    public string? UnavailableReason { get; set; }

    /// <summary>
    /// 匹配的规则
    /// </summary>
    public IReadOnlyList<string> MatchedRules { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 性能指标
    /// </summary>
    public PerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// 路由规则
/// </summary>
public class RoutingRule
{
    /// <summary>
    /// 规则ID
    /// </summary>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// 规则优先级
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 匹配条件
    /// </summary>
    public RuleCondition Condition { get; set; } = new();

    /// <summary>
    /// 路由动作
    /// </summary>
    public RuleAction Action { get; set; } = new();

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 规则条件
/// </summary>
public class RuleCondition
{
    /// <summary>
    /// 实体类型条件
    /// </summary>
    public string[]? EntityTypes { get; set; }

    /// <summary>
    /// 操作类型条件
    /// </summary>
    public string[]? OperationTypes { get; set; }

    /// <summary>
    /// 一致性级别条件
    /// </summary>
    public DataConsistencyLevel[]? ConsistencyLevels { get; set; }

    /// <summary>
    /// 时间段条件
    /// </summary>
    public TimeSpan[]? TimeRanges { get; set; }

    /// <summary>
    /// 标签条件
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// 自定义条件表达式
    /// </summary>
    public string? CustomExpression { get; set; }

    /// <summary>
    /// 扩展条件
    /// </summary>
    public Dictionary<string, object> ExtendedConditions { get; set; } = new();
}

/// <summary>
/// 规则动作
/// </summary>
public class RuleAction
{
    /// <summary>
    /// 动作类型
    /// </summary>
    public RouteActionType ActionType { get; set; } = RouteActionType.SelectProvider;

    /// <summary>
    /// 目标提供者类型
    /// </summary>
    public DataProviderType? TargetProviderType { get; set; }

    /// <summary>
    /// 目标技术类型
    /// </summary>
    public string? TargetTechnologyType { get; set; }

    /// <summary>
    /// 目标平台类型
    /// </summary>
    public string? TargetPlatformType { get; set; }

    /// <summary>
    /// 降级策略
    /// </summary>
    public FallbackStrategy FallbackStrategy { get; set; } = FallbackStrategy.NextAvailable;

    /// <summary>
    /// 权重分配
    /// </summary>
    public Dictionary<string, decimal> WeightDistribution { get; set; } = new();

    /// <summary>
    /// 扩展参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 性能指标
/// </summary>
public class PerformanceMetrics
{
    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate { get; set; } = 1.0;

    /// <summary>
    /// 当前负载
    /// </summary>
    public double CurrentLoad { get; set; }

    /// <summary>
    /// 最大负载
    /// </summary>
    public double MaxLoad { get; set; } = 1.0;

    /// <summary>
    /// 最近错误次数
    /// </summary>
    public int RecentErrorCount { get; set; }

    /// <summary>
    /// 健康分数
    /// </summary>
    public double HealthScore { get; set; } = 1.0;
}