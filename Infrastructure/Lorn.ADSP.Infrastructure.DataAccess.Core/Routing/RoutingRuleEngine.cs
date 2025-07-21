using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Routing;

/// <summary>
/// 路由规则引擎
/// 负责路由规则的管理和匹配逻辑
/// </summary>
public class RoutingRuleEngine
{
    private readonly ILogger<RoutingRuleEngine> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public RoutingRuleEngine(ILogger<RoutingRuleEngine> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 查找匹配的路由规则
    /// </summary>
    /// <param name="rules">路由规则集合</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>匹配的路由规则，如果没有匹配则返回null</returns>
    public RoutingRule? FindMatchingRule(IEnumerable<RoutingRule> rules, DataAccessContext context)
    {
        if (rules == null)
            throw new ArgumentNullException(nameof(rules));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var enabledRules = rules
            .Where(r => r.IsEnabled)
            .OrderByDescending(r => r.Priority)
            .ToList();

        _logger.LogDebug("Evaluating {Count} enabled routing rules for request {RequestId}",
            enabledRules.Count, context.RequestId);

        foreach (var rule in enabledRules)
        {
            if (EvaluateRule(rule, context))
            {
                _logger.LogDebug("Rule '{RuleName}' matched for request {RequestId}",
                    rule.Name, context.RequestId);
                return rule;
            }
        }

        _logger.LogDebug("No routing rules matched for request {RequestId}", context.RequestId);
        return null;
    }

    /// <summary>
    /// 评估单个路由规则是否匹配
    /// </summary>
    /// <param name="rule">路由规则</param>
    /// <param name="context">数据访问上下文</param>
    /// <returns>如果匹配则返回true，否则返回false</returns>
    public bool EvaluateRule(RoutingRule rule, DataAccessContext context)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (!rule.IsEnabled)
        {
            return false;
        }

        var condition = rule.Condition;

        // 评估操作类型条件
        if (!EvaluateOperationTypes(condition.OperationTypes, context.OperationType))
        {
            _logger.LogTrace("Rule '{RuleName}' failed operation type check", rule.Name);
            return false;
        }

        // 评估实体类型条件
        if (!EvaluateEntityTypes(condition.EntityTypes, context.EntityType))
        {
            _logger.LogTrace("Rule '{RuleName}' failed entity type check", rule.Name);
            return false;
        }

        // 评估标签条件
        if (!EvaluateTags(condition.Tags, context.Tags))
        {
            _logger.LogTrace("Rule '{RuleName}' failed tags check", rule.Name);
            return false;
        }

        // 评估一致性级别条件
        if (!EvaluateConsistencyLevels(condition.ConsistencyLevels, context.ConsistencyLevel))
        {
            _logger.LogTrace("Rule '{RuleName}' failed consistency level check", rule.Name);
            return false;
        }

        // 评估用户ID模式条件
        if (!EvaluateUserIdPattern(condition.UserIdPattern, context.UserId))
        {
            _logger.LogTrace("Rule '{RuleName}' failed user ID pattern check", rule.Name);
            return false;
        }

        // 评估租户ID模式条件
        if (!EvaluateTenantIdPattern(condition.TenantIdPattern, context.TenantId))
        {
            _logger.LogTrace("Rule '{RuleName}' failed tenant ID pattern check", rule.Name);
            return false;
        }

        // 评估时间范围条件
        if (!EvaluateTimeRange(condition.TimeRange))
        {
            _logger.LogTrace("Rule '{RuleName}' failed time range check", rule.Name);
            return false;
        }

        // 评估自定义条件表达式
        if (!EvaluateCustomExpression(condition.CustomExpression, context))
        {
            _logger.LogTrace("Rule '{RuleName}' failed custom expression check", rule.Name);
            return false;
        }

        // 评估扩展条件
        if (!EvaluateExtendedConditions(condition.ExtendedConditions, context))
        {
            _logger.LogTrace("Rule '{RuleName}' failed extended conditions check", rule.Name);
            return false;
        }

        _logger.LogTrace("Rule '{RuleName}' passed all condition checks", rule.Name);
        return true;
    }

    /// <summary>
    /// 验证路由规则
    /// </summary>
    /// <param name="rule">路由规则</param>
    /// <returns>验证结果</returns>
    public RuleValidationResult ValidateRule(RoutingRule rule)
    {
        if (rule == null)
            return new RuleValidationResult(false, "Rule cannot be null");

        if (string.IsNullOrWhiteSpace(rule.RuleId))
            return new RuleValidationResult(false, "Rule ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(rule.Name))
            return new RuleValidationResult(false, "Rule name cannot be null or empty");

        if (rule.Condition == null)
            return new RuleValidationResult(false, "Rule condition cannot be null");

        // 验证时间范围
        if (rule.Condition.TimeRange != null)
        {
            var timeRange = rule.Condition.TimeRange;

            if (timeRange.StartTime.HasValue && timeRange.EndTime.HasValue &&
                timeRange.StartTime.Value > timeRange.EndTime.Value)
            {
                return new RuleValidationResult(false, "Start time cannot be greater than end time");
            }

            if (timeRange.DaysOfWeek?.Any(d => d < 1 || d > 7) == true)
            {
                return new RuleValidationResult(false, "Days of week must be between 1 and 7");
            }
        }

        return new RuleValidationResult(true, "Rule is valid");
    }

    /// <summary>
    /// 评估操作类型条件
    /// </summary>
    private bool EvaluateOperationTypes(string[]? operationTypes, string actualOperationType)
    {
        if (operationTypes == null || !operationTypes.Any())
            return true; // 没有限制则通过

        return operationTypes.Contains(actualOperationType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 评估实体类型条件
    /// </summary>
    private bool EvaluateEntityTypes(string[]? entityTypes, string actualEntityType)
    {
        if (entityTypes == null || !entityTypes.Any())
            return true; // 没有限制则通过

        return entityTypes.Contains(actualEntityType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 评估标签条件
    /// </summary>
    private bool EvaluateTags(string[]? requiredTags, string[] actualTags)
    {
        if (requiredTags == null || !requiredTags.Any())
            return true; // 没有限制则通过

        return requiredTags.Any(tag => actualTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 评估一致性级别条件
    /// </summary>
    private bool EvaluateConsistencyLevels(DataConsistencyLevel[]? consistencyLevels, DataConsistencyLevel actualLevel)
    {
        if (consistencyLevels == null || !consistencyLevels.Any())
            return true; // 没有限制则通过

        return consistencyLevels.Contains(actualLevel);
    }

    /// <summary>
    /// 评估用户ID模式条件
    /// </summary>
    private bool EvaluateUserIdPattern(string? userIdPattern, string? actualUserId)
    {
        if (string.IsNullOrWhiteSpace(userIdPattern))
            return true; // 没有限制则通过

        if (string.IsNullOrWhiteSpace(actualUserId))
            return false; // 有模式要求但实际值为空

        // 简单的模式匹配实现，可以扩展为正则表达式
        return actualUserId.Contains(userIdPattern, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 评估租户ID模式条件
    /// </summary>
    private bool EvaluateTenantIdPattern(string? tenantIdPattern, string? actualTenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantIdPattern))
            return true; // 没有限制则通过

        if (string.IsNullOrWhiteSpace(actualTenantId))
            return false; // 有模式要求但实际值为空

        // 简单的模式匹配实现，可以扩展为正则表达式
        return actualTenantId.Contains(tenantIdPattern, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 评估时间范围条件
    /// </summary>
    private bool EvaluateTimeRange(TimeRange? timeRange)
    {
        if (timeRange == null)
            return true; // 没有限制则通过

        var now = DateTime.Now;

        // 检查时间范围
        if (timeRange.StartTime.HasValue && now.TimeOfDay < timeRange.StartTime.Value)
            return false;

        if (timeRange.EndTime.HasValue && now.TimeOfDay > timeRange.EndTime.Value)
            return false;

        // 检查星期几
        if (timeRange.DaysOfWeek?.Any() == true)
        {
            var dayOfWeek = (int)now.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // 将周日从0改为7

            if (!timeRange.DaysOfWeek.Contains(dayOfWeek))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 评估自定义条件表达式
    /// </summary>
    private bool EvaluateCustomExpression(string? customExpression, DataAccessContext context)
    {
        if (string.IsNullOrWhiteSpace(customExpression))
            return true; // 没有自定义表达式则通过

        // 这里可以实现自定义表达式的解析和执行
        // 为了简化，目前总是返回true
        _logger.LogDebug("Custom expression evaluation not implemented: {Expression}", customExpression);
        return true;
    }

    /// <summary>
    /// 评估扩展条件
    /// </summary>
    private bool EvaluateExtendedConditions(Dictionary<string, object> extendedConditions, DataAccessContext context)
    {
        if (!extendedConditions.Any())
            return true; // 没有扩展条件则通过

        // 检查上下文的扩展属性是否匹配
        foreach (var condition in extendedConditions)
        {
            if (!context.ExtendedProperties.TryGetValue(condition.Key, out var actualValue) ||
                !Equals(actualValue, condition.Value))
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// 规则验证结果
/// </summary>
public class RuleValidationResult
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isValid">是否有效</param>
    /// <param name="message">验证消息</param>
    public RuleValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; }
}
