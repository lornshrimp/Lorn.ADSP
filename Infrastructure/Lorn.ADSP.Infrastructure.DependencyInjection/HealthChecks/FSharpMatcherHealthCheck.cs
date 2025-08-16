using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.HealthChecks;

/// <summary>
/// F#匹配器健康检查
/// F# Matcher Health Check
/// </summary>
public class FSharpMatcherHealthCheck : IHealthCheck
{
    private readonly ITargetingMatcherManager _matcherManager;
    private readonly ILogger<FSharpMatcherHealthCheck> _logger;

    /// <summary>
    /// 构造函数
    /// Constructor
    /// </summary>
    public FSharpMatcherHealthCheck(
        ITargetingMatcherManager matcherManager,
        ILogger<FSharpMatcherHealthCheck> logger)
    {
        _matcherManager = matcherManager ?? throw new ArgumentNullException(nameof(matcherManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 执行健康检查
    /// Execute health check
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var matchers = _matcherManager.GetAllMatchers();
            var fsharpMatchers = matchers.Where(m => m.GetType().Assembly.FullName?.Contains("FSharp") == true).ToList();

            var data = new Dictionary<string, object>
            {
                { "total_matchers", matchers.Count },
                { "fsharp_matchers", fsharpMatchers.Count },
                { "enabled_matchers", matchers.Count(m => m.IsEnabled) }
            };

            // 检查F#匹配器是否正常
            // Check if F# matchers are working properly
            if (fsharpMatchers.Count == 0)
            {
                _logger.LogWarning("No F# matchers found");
                return HealthCheckResult.Degraded("No F# matchers registered", data: data);
            }

            // 验证所有匹配器配置
            // Validate all matcher configurations
            var validationResults = _matcherManager.ValidateAllMatchers();
            var failedValidations = validationResults.Where(r => !r.IsValid).ToList();

            if (failedValidations.Any())
            {
                data["failed_validations"] = failedValidations.Count;
                _logger.LogWarning("Found {Count} matcher validation failures", failedValidations.Count);
                return HealthCheckResult.Degraded("Some matcher validations failed", data: data);
            }

            data["validation_results"] = validationResults.Count;
            _logger.LogDebug("F# matcher health check passed with {MatcherCount} matchers", fsharpMatchers.Count);

            return HealthCheckResult.Healthy("F# matchers are working properly", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "F# matcher health check failed");
            return HealthCheckResult.Unhealthy("F# matcher health check failed", ex);
        }
    }
}
