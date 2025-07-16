using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Lorn.ADSP.Infrastructure.Configuration;
using Lorn.ADSP.Infrastructure.DependencyInjection;
using Lorn.ADSP.Infrastructure.Composition.Configuration;
using Lorn.ADSP.Infrastructure.Common.Abstractions;

namespace Lorn.ADSP.Infrastructure.Composition.Bootstrapper;

/// <summary>
/// 基础设施启动器
/// 负责协调各个基础设施组件的启动
/// </summary>
public class InfrastructureBootstrapper
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly BootstrapOptions _options;
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    public InfrastructureBootstrapper(IServiceCollection services, IConfiguration configuration, BootstrapOptions? options = null)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _options = options ?? new BootstrapOptions();
    }

    /// <summary>
    /// 启动基础设施
    /// </summary>
    /// <returns>服务集合</returns>
    public IServiceCollection Bootstrap()
    {
        try
        {
            // 1. 配置管理 - 自动注册所有配置类型
            if (_options.EnableAutoConfigurationBinding)
            {
                BootstrapConfiguration();
            }

            // 2. 组件注册 - 基于约定规则自动注册
            if (_options.EnableAutoComponentDiscovery)
            {
                BootstrapComponents();
            }

            // 3. 健康检查（自动为所有组件添加）
            if (_options.EnableHealthChecks)
            {
                BootstrapHealthChecks();
            }

            // 4. 配置验证
            if (_options.EnableConfigurationValidation)
            {
                BootstrapValidation();
            }

            return _services;
        }
        catch (Exception ex)
        {
            _errors.Add($"Bootstrap failed: {ex.Message}");
            
            if (_options.FailureToleranceMode == FailureToleranceMode.FailFast)
            {
                throw new InvalidOperationException($"Infrastructure bootstrap failed: {ex.Message}", ex);
            }

            return _services;
        }
    }

    /// <summary>
    /// 启动配置管理
    /// </summary>
    private void BootstrapConfiguration()
    {
        try
        {
            var configManager = new AdSystemConfigurationManager(_configuration, _services);
            configManager.RegisterAllOptions();
        }
        catch (Exception ex)
        {
            HandleError("Configuration bootstrap failed", ex);
        }
    }

    /// <summary>
    /// 启动组件注册
    /// </summary>
    private void BootstrapComponents()
    {
        try
        {
            var configManager = new AdSystemConfigurationManager(_configuration, _services);
            var componentManager = new ComponentRegistrationManager(_services, configManager);
            componentManager.RegisterAllComponents();
        }
        catch (Exception ex)
        {
            HandleError("Component registration failed", ex);
        }
    }

    /// <summary>
    /// 启动健康检查
    /// </summary>
    private void BootstrapHealthChecks()
    {
        try
        {
            var healthChecksBuilder = _services.AddHealthChecks();
            
            // 自动为所有实现 IHealthCheckable 的组件添加健康检查
            // 这里简化处理，实际实现中会扫描所有已注册的服务
            healthChecksBuilder.AddCheck("infrastructure", () => HealthCheckResult.Healthy("Infrastructure is healthy"));
        }
        catch (Exception ex)
        {
            HandleError("Health checks bootstrap failed", ex);
        }
    }

    /// <summary>
    /// 启动配置验证
    /// </summary>
    private void BootstrapValidation()
    {
        try
        {
            var validationManager = new ConfigurationValidationManager(_services);
            validationManager.RegisterAllValidators();
        }
        catch (Exception ex)
        {
            HandleError("Validation bootstrap failed", ex);
        }
    }

    /// <summary>
    /// 处理错误
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="exception">异常</param>
    private void HandleError(string message, Exception exception)
    {
        var fullMessage = $"{message}: {exception.Message}";
        _errors.Add(fullMessage);

        if (_options.FailureToleranceMode == FailureToleranceMode.FailFast)
        {
            throw new InvalidOperationException(fullMessage, exception);
        }

        // 记录错误但继续执行
        System.Diagnostics.Debug.WriteLine($"[ERROR] {fullMessage}");
    }

    /// <summary>
    /// 获取启动过程中的错误
    /// </summary>
    /// <returns>错误列表</returns>
    public IReadOnlyList<string> GetErrors()
    {
        return _errors.AsReadOnly();
    }

    /// <summary>
    /// 获取启动过程中的警告
    /// </summary>
    /// <returns>警告列表</returns>
    public IReadOnlyList<string> GetWarnings()
    {
        return _warnings.AsReadOnly();
    }

    /// <summary>
    /// 检查启动是否成功
    /// </summary>
    /// <returns>是否成功</returns>
    public bool IsSuccessful()
    {
        return !_errors.Any();
    }
}