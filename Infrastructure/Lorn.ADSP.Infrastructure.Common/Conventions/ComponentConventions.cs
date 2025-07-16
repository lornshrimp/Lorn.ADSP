using Lorn.ADSP.Infrastructure.Common.Models;

namespace Lorn.ADSP.Infrastructure.Common.Conventions;

/// <summary>
/// 组件约定规则
/// </summary>
public class ComponentConventionRule
{
    /// <summary>
    /// 组件名称后缀
    /// </summary>
    public string Suffix { get; set; } = "";
    
    /// <summary>
    /// 组件生命周期
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    
    /// <summary>
    /// 要求的接口名称列表
    /// </summary>
    public string[] RequiredInterfaceNames { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// 配置路径模板
    /// </summary>
    public string ConfigurationPathTemplate { get; set; } = "";
    
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// 是否要求特定接口
    /// </summary>
    public bool RequireSpecificInterface { get; set; } = false;
}

/// <summary>
/// 组件约定规范
/// </summary>
public static class ComponentConventions
{
    private static readonly List<ComponentConventionRule> _conventionRules = new()
    {
        new ComponentConventionRule
        {
            Suffix = "Strategy",
            Lifetime = ServiceLifetime.Transient,
            RequiredInterfaceNames = new[] { "IAdProcessingStrategy" },
            ConfigurationPathTemplate = "Strategies:{0}",
            Description = "广告处理策略组件",
            RequireSpecificInterface = true
        },
        new ComponentConventionRule
        {
            Suffix = "Service",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = Array.Empty<string>(),
            ConfigurationPathTemplate = "Services:{0}",
            Description = "业务服务组件",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Manager",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = Array.Empty<string>(),
            ConfigurationPathTemplate = "Managers:{0}",
            Description = "管理器组件",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Provider",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = new[] { "IDataAccessProvider", "IProvider" },
            ConfigurationPathTemplate = "DataProviders:{0}",
            Description = "数据提供者组件",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "CallbackProvider",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = new[] { "ICallbackProvider" },
            ConfigurationPathTemplate = "CallbackProviders:{0}",
            Description = "回调提供者组件",
            RequireSpecificInterface = true
        },
        new ComponentConventionRule
        {
            Suffix = "Matcher",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "ITargetingMatcher", "IMatcher" },
            ConfigurationPathTemplate = "Matchers:{0}",
            Description = "定向匹配器组件",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Calculator",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "ICalculator" },
            ConfigurationPathTemplate = "Calculators:{0}",
            Description = "计算器组件",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Processor",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "IProcessor" },
            ConfigurationPathTemplate = "Processors:{0}",
            Description = "处理器组件",
            RequireSpecificInterface = false
        }
    };

    /// <summary>
    /// 获取所有约定规则
    /// </summary>
    /// <returns>约定规则列表</returns>
    public static IReadOnlyList<ComponentConventionRule> GetAllRules() => _conventionRules.AsReadOnly();

    /// <summary>
    /// 根据类型查找匹配的约定规则
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>匹配的约定规则，如果没有匹配则返回null</returns>
    public static ComponentConventionRule? FindRuleByType(Type type)
    {
        // 优先基于命名约定匹配
        var rule = _conventionRules.FirstOrDefault(r => type.Name.EndsWith(r.Suffix));
        if (rule == null) return null;

        // 如果要求特定接口，进行接口验证
        if (rule.RequireSpecificInterface && rule.RequiredInterfaceNames.Length > 0)
        {
            var implementedInterfaceNames = type.GetInterfaces().Select(i => i.Name).ToArray();
            var hasRequiredInterface = rule.RequiredInterfaceNames.Any(required =>
                implementedInterfaceNames.Contains(required));

            if (!hasRequiredInterface)
            {
                return null; // 不满足接口要求
            }
        }

        return rule;
    }

    /// <summary>
    /// 根据类型获取生命周期
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>生命周期</returns>
    public static ServiceLifetime GetLifetimeByType(Type type)
    {
        var rule = FindRuleByType(type);
        return rule?.Lifetime ?? ServiceLifetime.Transient;
    }

    /// <summary>
    /// 根据类型获取配置路径
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>配置路径</returns>
    public static string? GetConfigurationPath(Type type)
    {
        var rule = FindRuleByType(type);
        if (rule?.ConfigurationPathTemplate == null) return null;

        // 提取组件名称
        var componentName = type.Name;
        if (componentName.EndsWith(rule.Suffix))
        {
            componentName = componentName.Substring(0, componentName.Length - rule.Suffix.Length);
        }

        return string.Format(rule.ConfigurationPathTemplate, componentName);
    }

    /// <summary>
    /// 添加新的约定规则
    /// </summary>
    /// <param name="rule">约定规则</param>
    public static void AddConventionRule(ComponentConventionRule rule)
    {
        if (rule == null) throw new ArgumentNullException(nameof(rule));
        if (string.IsNullOrEmpty(rule.Suffix)) throw new ArgumentException("Suffix cannot be null or empty");

        // 检查是否已存在相同后缀的规则
        if (_conventionRules.Any(r => r.Suffix == rule.Suffix))
        {
            throw new InvalidOperationException($"Convention rule with suffix '{rule.Suffix}' already exists");
        }

        _conventionRules.Add(rule);
    }

    /// <summary>
    /// 检查类型是否符合组件约定
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否符合约定</returns>
    public static bool IsComponent(Type type)
    {
        if (type == null || !type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
            return false;

        // 检查是否符合约定规则
        return FindRuleByType(type) != null;
    }
}