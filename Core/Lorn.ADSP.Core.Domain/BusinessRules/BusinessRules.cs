namespace Lorn.ADSP.Core.Domain.BusinessRules
{
    /// <summary>
    /// 业务规则
    /// </summary>
    public record BusinessRules
    {
        /// <summary>
        /// 规则类型
        /// </summary>
        public required string RuleType { get; init; }

        /// <summary>
        /// 规则版本
        /// </summary>
        public required string Version { get; init; }

        /// <summary>
        /// 规则列表
        /// </summary>
        public required IReadOnlyList<BusinessRule> Rules { get; init; }

        /// <summary>
        /// 默认行为
        /// </summary>
        public string? DefaultBehavior { get; init; }

        /// <summary>
        /// 规则评估策略
        /// </summary>
        public RuleEvaluationStrategy EvaluationStrategy { get; init; } = RuleEvaluationStrategy.FirstMatch;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; init; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 扩展属性
        /// </summary>
        public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } =
            new Dictionary<string, object>();
    }

    /// <summary>
    /// 业务规则
    /// </summary>
    public record BusinessRule
    {
        /// <summary>
        /// 规则ID
        /// </summary>
        public required string RuleId { get; init; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public required string RuleName { get; init; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 规则优先级（数值越小优先级越高）
        /// </summary>
        public int Priority { get; init; } = 5;

        /// <summary>
        /// 条件表达式
        /// </summary>
        public required string Condition { get; init; }

        /// <summary>
        /// 执行动作
        /// </summary>
        public required string Action { get; init; }

        /// <summary>
        /// 动作参数
        /// </summary>
        public IReadOnlyDictionary<string, object> ActionParameters { get; init; } =
            new Dictionary<string, object>();

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; init; } = true;

        /// <summary>
        /// 生效开始时间
        /// </summary>
        public DateTime? EffectiveFrom { get; init; }

        /// <summary>
        /// 生效结束时间
        /// </summary>
        public DateTime? EffectiveTo { get; init; }

        /// <summary>
        /// 适用的上下文条件
        /// </summary>
        public IReadOnlyDictionary<string, object> ContextConditions { get; init; } =
            new Dictionary<string, object>();

        /// <summary>
        /// 规则标签
        /// </summary>
        public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 规则是否在指定时间有效
        /// </summary>
        public bool IsEffectiveAt(DateTime time)
        {
            return (!EffectiveFrom.HasValue || time >= EffectiveFrom.Value) &&
                   (!EffectiveTo.HasValue || time <= EffectiveTo.Value);
        }

        /// <summary>
        /// 规则是否当前有效
        /// </summary>
        public bool IsCurrentlyEffective => IsEnabled && IsEffectiveAt(DateTime.UtcNow);
    }

    /// <summary>
    /// 规则评估策略
    /// </summary>
    public enum RuleEvaluationStrategy
    {
        /// <summary>
        /// 第一个匹配的规则
        /// </summary>
        FirstMatch = 1,

        /// <summary>
        /// 所有匹配的规则
        /// </summary>
        AllMatches = 2,

        /// <summary>
        /// 最高优先级的规则
        /// </summary>
        HighestPriority = 3,

        /// <summary>
        /// 加权评估
        /// </summary>
        WeightedEvaluation = 4
    }
}
