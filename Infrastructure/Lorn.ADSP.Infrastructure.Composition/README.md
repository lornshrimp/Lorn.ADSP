# 统一配置化和依赖注入架构 - 使用指南

## 概述

本基础设施提供了一个完整的、开箱即用的配置管理和依赖注入解决方案，基于约定优于配置的原则，实现零配置的组件扩展能力。

## 快速开始

### 1. 在应用程序中启用基础设施

```csharp
// Program.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Lorn.ADSP.Infrastructure.Composition.Extensions;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // 一键配置整个广告系统基础设施
                services.AddAdSystemInfrastructure(context.Configuration);
                
                // 可选：添加健康检查
                services.AddHealthChecks()
                    .AddComponentHealthChecks();
            })
            .Build();
            
        host.Run();
    }
}
```

### 2. 创建业务组件

遵循约定命名规则，组件将自动被发现和注册：

```csharp
// 策略组件示例
public class UserInterestRecallStrategy : IAdProcessingStrategy, IHealthCheckable
{
    private readonly UserInterestRecallOptions _options;
    private readonly ILogger<UserInterestRecallStrategy> _logger;

    // 配置会自动注入（基于命名约定）
    public UserInterestRecallStrategy(
        IOptionsMonitor<UserInterestRecallOptions> options,
        ILogger<UserInterestRecallStrategy> logger)
    {
        _options = options.CurrentValue;
        _logger = logger;
        
        // 订阅配置变更事件
        options.OnChange(OnConfigurationChanged);
    }

    public async Task<ProcessingResult> ProcessAsync(
        IEnumerable<AdCandidate> candidates, 
        AdContext context)
    {
        // 实现业务逻辑
        return new ProcessingResult { Success = true };
    }

    public async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return _options.IsEnabled ? HealthStatus.Healthy : HealthStatus.Degraded;
    }
    
    private void OnConfigurationChanged(UserInterestRecallOptions newOptions)
    {
        _logger.LogInformation("Configuration updated for {Strategy}", nameof(UserInterestRecallStrategy));
    }
}

// 配置选项类 - 自动绑定到 "Strategies:UserInterestRecall" 配置节
public class UserInterestRecallOptions
{
    public int MaxCandidates { get; set; } = 1000;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public double ScoreThreshold { get; set; } = 0.3;
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; } = 100;
}
```

### 3. 数据提供者组件

```csharp
// 数据提供者示例
public class UserProfileProvider : IDataAccessProvider, IHealthCheckable
{
    private readonly UserProfileProviderOptions _options;
    
    public UserProfileProvider(IOptionsMonitor<UserProfileProviderOptions> options)
    {
        _options = options.CurrentValue;
        options.OnChange(OnConfigurationChanged);
    }

    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        // 使用配置值实现业务逻辑
        using var connection = new SqlConnection(_options.ConnectionString);
        // ...
    }

    public async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsEnabled)
            return HealthStatus.Degraded;
            
        // 检查数据库连接
        try
        {
            using var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return HealthStatus.Healthy;
        }
        catch
        {
            return HealthStatus.Unhealthy;
        }
    }
    
    private void OnConfigurationChanged(UserProfileProviderOptions newOptions)
    {
        // 处理配置变更
    }
}

// 配置选项 - 自动绑定到 "DataProviders:UserProfile" 配置节
public class UserProfileProviderOptions
{
    public string ConnectionString { get; set; } = "";
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
    public bool IsEnabled { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
}
```

## 约定规则

### 组件命名约定

| 组件类型   | 命名格式           | 生命周期  | 配置路径                 |
| ---------- | ------------------ | --------- | ------------------------ |
| 策略组件   | `*Strategy`        | Transient | `Strategies:{功能}`      |
| 服务组件   | `*Service`         | Singleton | `Services:{功能}`        |
| 管理器组件 | `*Manager`         | Singleton | `Managers:{功能}`        |
| 数据提供者 | `*Provider`        | Singleton | `DataProviders:{数据源}` |
| 定向匹配器 | `*Matcher`         | Scoped    | `Matchers:{类型}`        |
| 计算器     | `*Calculator`      | Scoped    | `Calculators:{内容}`     |
| 处理器     | `*Processor`       | Scoped    | `Processors:{内容}`      |

### 配置类约定

- 配置类命名：`{模块名}Options`
- 自动映射规则：
  - `AdEngineOptions` → `"AdEngine"` 配置节
  - `UserInterestRecallOptions` → `"Strategies:UserInterestRecall"` 配置节

### 接口约定

- 健康检查：实现 `IHealthCheckable` 接口自动添加健康检查
- 可配置组件：实现 `IConfigurable` 接口支持自定义配置更新逻辑

## 高级功能

### 显式组件标记

```csharp
[Component(Priority = 100, IsEnabled = true, Description = "高级用户兴趣策略")]
[ConfigurationBinding("CustomPath:AdvancedStrategy")]
[Singleton]
public class AdvancedUserInterestStrategy : IAdProcessingStrategy
{
    // 组件实现
}
```

### 命名选项模式

当多个组件共享相同的配置选项类时：

```csharp
public class RTBAdEngineService : IAdEngineService
{
    public RTBAdEngineService(IOptionsSnapshot<AdEngineOptions> optionsSnapshot)
    {
        // 使用命名配置 "RTB"
        var options = optionsSnapshot.Get("RTB");
    }
}

public class DirectSaleAdEngineService : IAdEngineService
{
    public DirectSaleAdEngineService(IOptionsSnapshot<AdEngineOptions> optionsSnapshot)
    {
        // 使用命名配置 "DirectSale"
        var options = optionsSnapshot.Get("DirectSale");
    }
}
```

配置文件：
```json
{
  "AdEngine": {
    "RTB": {
      "DefaultTimeout": "00:00:30",
      "MaxRetries": 3
    },
    "DirectSale": {
      "DefaultTimeout": "00:01:00", 
      "MaxRetries": 5
    }
  }
}
```

### 配置验证

```csharp
public class UserInterestRecallOptionsValidator : IValidateOptions<UserInterestRecallOptions>
{
    public ValidateOptionsResult Validate(string name, UserInterestRecallOptions options)
    {
        var failures = new List<string>();

        if (options.MaxCandidates <= 0)
            failures.Add("MaxCandidates must be greater than 0");

        if (options.ScoreThreshold < 0 || options.ScoreThreshold > 1)
            failures.Add("ScoreThreshold must be between 0 and 1");

        return failures.Any() 
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
```

## 配置文件模板

请参考 `appsettings.example.json` 文件，了解完整的配置文件结构。

## 最佳实践

1. **遵循约定**：使用标准的命名约定，享受零配置的便利
2. **配置热重载**：使用 `IOptionsMonitor<T>` 而非 `IOptions<T>`
3. **健康检查**：为关键组件实现 `IHealthCheckable` 接口
4. **配置验证**：为复杂配置实现 `IValidateOptions<T>` 验证器
5. **错误处理**：组件实现中要有适当的异常处理
6. **日志记录**：使用 `ILogger<T>` 记录关键操作和错误

## 故障排除

### 组件未被自动注册

1. 检查组件命名是否符合约定（如 `*Strategy`、`*Service` 等）
2. 确认组件类是公共具体类（非抽象、非泛型定义）
3. 检查程序集是否在扫描范围内

### 配置未生效

1. 检查配置类名称是否符合约定（如 `*Options`）
2. 确认配置文件路径正确
3. 验证 JSON 格式正确性

### 健康检查不工作

1. 确认组件实现了 `IHealthCheckable` 接口
2. 检查是否调用了 `AddComponentHealthChecks()`
3. 验证健康检查端点配置

这个基础设施将极大简化广告系统的开发和维护工作，让开发者专注于业务逻辑实现。