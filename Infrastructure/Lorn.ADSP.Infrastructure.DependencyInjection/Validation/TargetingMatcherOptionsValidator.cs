using Microsoft.Extensions.Options;
using Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Validation;

/// <summary>
/// 定向匹配器配置验证器
/// Targeting Matcher Configuration Validator
/// </summary>
public class TargetingMatcherOptionsValidator : IValidateOptions<TargetingMatcherOptions>
{
    /// <summary>
    /// 验证配置选项
    /// Validate configuration options
    /// </summary>
    /// <param name="name">配置名称 / Configuration name</param>
    /// <param name="options">配置选项 / Configuration options</param>
    /// <returns>验证结果 / Validation result</returns>
    public ValidateOptionsResult Validate(string? name, TargetingMatcherOptions options)
    {
        var failures = new List<string>();

        // 验证默认超时时间
        // Validate default timeout
        if (options.DefaultTimeoutMs <= 0)
        {
            failures.Add("DefaultTimeoutMs must be greater than 0");
        }

        if (options.DefaultTimeoutMs > 30000)
        {
            failures.Add("DefaultTimeoutMs should not exceed 30000ms for performance reasons");
        }

        // 验证默认生命周期
        // Validate default lifetime
        var validLifetimes = new[] { "Singleton", "Scoped", "Transient" };
        if (!validLifetimes.Contains(options.DefaultLifetime, StringComparer.OrdinalIgnoreCase))
        {
            failures.Add($"DefaultLifetime must be one of: {string.Join(", ", validLifetimes)}");
        }

        // 验证各个匹配器配置
        // Validate individual matcher configurations
        foreach (var (matcherKey, matcherConfig) in options.Matchers)
        {
            ValidateMatcherConfiguration(matcherKey, matcherConfig, failures);
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }

    /// <summary>
    /// 验证单个匹配器配置
    /// Validate individual matcher configuration
    /// </summary>
    private static void ValidateMatcherConfiguration(
        string matcherKey,
        MatcherConfiguration config,
        List<string> failures)
    {
        // 验证优先级
        // Validate priority
        if (config.Priority < 0)
        {
            failures.Add($"Matcher '{matcherKey}': Priority cannot be negative");
        }

        // 验证执行时间
        // Validate execution time
        if (config.ExpectedExecutionTimeMs <= 0)
        {
            failures.Add($"Matcher '{matcherKey}': ExpectedExecutionTimeMs must be greater than 0");
        }

        if (config.ExpectedExecutionTimeMs > config.TimeoutMs)
        {
            failures.Add($"Matcher '{matcherKey}': ExpectedExecutionTimeMs cannot exceed TimeoutMs");
        }

        // 验证超时时间
        // Validate timeout
        if (config.TimeoutMs <= 0)
        {
            failures.Add($"Matcher '{matcherKey}': TimeoutMs must be greater than 0");
        }

        if (config.TimeoutMs > 10000)
        {
            failures.Add($"Matcher '{matcherKey}': TimeoutMs should not exceed 10000ms for performance reasons");
        }

        // 验证生命周期
        // Validate lifetime
        var validLifetimes = new[] { "Singleton", "Scoped", "Transient" };
        if (!validLifetimes.Contains(config.Lifetime, StringComparer.OrdinalIgnoreCase))
        {
            failures.Add($"Matcher '{matcherKey}': Lifetime must be one of: {string.Join(", ", validLifetimes)}");
        }
    }
}
