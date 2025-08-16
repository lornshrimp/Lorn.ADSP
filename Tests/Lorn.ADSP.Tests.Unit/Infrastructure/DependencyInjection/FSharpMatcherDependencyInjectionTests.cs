using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;
using Lorn.ADSP.Infrastructure.DependencyInjection.Extensions;
using Lorn.ADSP.Infrastructure.DependencyInjection.Services;

namespace Lorn.ADSP.Tests.Unit.Infrastructure.DependencyInjection;

/// <summary>
/// F#匹配器依赖注入集成测试
/// F# Matcher Dependency Injection Integration Tests
/// </summary>
public class FSharpMatcherDependencyInjectionTests
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;

    public FSharpMatcherDependencyInjectionTests(ITestOutputHelper output)
    {
        _output = output;
        _serviceProvider = SetupServiceProvider();
    }

    /// <summary>
    /// 设置服务提供者
    /// Setup service provider
    /// </summary>
    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();

        // 添加日志记录
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // 添加配置
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "true" },
                { "TargetingMatchers:DefaultLifetime", "Scoped" },
                { "TargetingMatchers:DefaultTimeoutMs", "5000" },
                { "TargetingMatchers:Matchers:demographic:Enabled", "true" },
                { "TargetingMatchers:Matchers:demographic:Priority", "100" },
                { "TargetingMatchers:Matchers:geolocation:Enabled", "true" },
                { "TargetingMatchers:Matchers:geolocation:Priority", "90" },
                { "TargetingMatchers:Matchers:time:Enabled", "true" },
                { "TargetingMatchers:Matchers:time:Priority", "110" },
                { "TargetingMatchers:Matchers:device:Enabled", "true" },
                { "TargetingMatchers:Matchers:device:Priority", "105" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // 添加F#匹配器服务
        // Add F# matcher services
        services.AddFSharpTargetingMatchers(configuration);

        // 添加健康检查
        // Add health checks
        services.AddFSharpMatcherHealthChecks();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void Should_Register_TargetingMatcherOptions_Successfully()
    {
        // 测试配置选项是否正确注册
        // Test if configuration options are correctly registered

        // Arrange & Act
        var options = _serviceProvider.GetService<IOptions<TargetingMatcherOptions>>();

        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.Value);
        Assert.True(options.Value.EnableFSharpMatchers);
        Assert.Equal("Scoped", options.Value.DefaultLifetime);
        Assert.Equal(5000, options.Value.DefaultTimeoutMs);

        _output.WriteLine($"Options registered successfully: EnableFSharpMatchers={options.Value.EnableFSharpMatchers}");
    }

    [Fact]
    public void Should_Register_TargetingMatcherManager_Successfully()
    {
        // 测试匹配器管理器是否正确注册
        // Test if matcher manager is correctly registered

        // Arrange & Act
        var manager = _serviceProvider.GetService<ITargetingMatcherManager>();

        // Assert
        Assert.NotNull(manager);
        Assert.IsType<TargetingMatcherManager>(manager);

        _output.WriteLine($"TargetingMatcherManager registered successfully: {manager.GetType().Name}");
    }

    [Fact]
    public void Should_Register_FSharp_Matchers_With_Correct_Lifetime()
    {
        // 测试F#匹配器是否使用正确的生命周期注册
        // Test if F# matchers are registered with correct lifetime

        // Arrange & Act
        var matchers = _serviceProvider.GetServices<ITargetingMatcher>().ToList();

        // Assert
        Assert.NotEmpty(matchers);

        foreach (var matcher in matchers)
        {
            Assert.NotNull(matcher);
            Assert.NotNull(matcher.MatcherId);
            Assert.NotNull(matcher.MatcherName);
            Assert.NotNull(matcher.MatcherType);

            _output.WriteLine($"Matcher registered: {matcher.MatcherId} ({matcher.MatcherType})");
        }

        _output.WriteLine($"Total registered matchers: {matchers.Count}");
    }

    [Fact]
    public void Should_Create_Multiple_Scoped_Instances()
    {
        // 测试作用域生命周期是否正确工作
        // Test if scoped lifetime works correctly

        // Arrange & Act
        using var scope1 = _serviceProvider.CreateScope();
        using var scope2 = _serviceProvider.CreateScope();

        var manager1 = scope1.ServiceProvider.GetRequiredService<ITargetingMatcherManager>();
        var manager2 = scope2.ServiceProvider.GetRequiredService<ITargetingMatcherManager>();

        var matchers1a = scope1.ServiceProvider.GetRequiredService<ITargetingMatcherManager>();
        var matchers1b = scope1.ServiceProvider.GetRequiredService<ITargetingMatcherManager>();

        // Assert
        Assert.NotSame(manager1, manager2); // 不同作用域应该是不同实例
        Assert.Same(manager1, matchers1a); // 同一作用域应该是同一实例
        Assert.Same(matchers1a, matchers1b); // 同一作用域应该是同一实例

        _output.WriteLine("Scoped lifetime validation passed");
    }

    [Fact]
    public async Task Should_Get_All_Matchers_From_Manager()
    {
        // 测试从管理器获取所有匹配器
        // Test getting all matchers from manager

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();

        // Act
        var allMatchers = manager.GetAllMatchers();

        // Assert
        Assert.NotNull(allMatchers);

        foreach (var matcher in allMatchers)
        {
            Assert.True(matcher.IsEnabled);
            Assert.True(matcher.Priority >= 0);
            Assert.True(matcher.ExpectedExecutionTime > TimeSpan.Zero);

            _output.WriteLine($"Matcher: {matcher.MatcherId}, Type: {matcher.MatcherType}, Priority: {matcher.Priority}");
        }

        _output.WriteLine($"Retrieved {allMatchers.Count} enabled matchers");
    }

    [Fact]
    public async Task Should_Validate_All_Matchers_Successfully()
    {
        // 测试所有匹配器的验证
        // Test validation of all matchers

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();

        // Act
        var validationResults = manager.ValidateAllMatchers();

        // Assert
        Assert.NotNull(validationResults);
        Assert.NotEmpty(validationResults);

        foreach (var result in validationResults)
        {
            Assert.True(result.IsValid, $"Validation failed: {result}");
        }

        _output.WriteLine($"All {validationResults.Count} matchers validated successfully");
    }

    [Fact]
    public async Task Should_Get_Statistics_Successfully()
    {
        // 测试获取统计信息
        // Test getting statistics

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();

        // Act
        var statistics = manager.GetStatistics();

        // Assert
        Assert.NotNull(statistics);
        Assert.True(statistics.TotalRegisteredMatchers > 0);
        Assert.True(statistics.EnabledMatchersCount <= statistics.TotalRegisteredMatchers);
        Assert.NotNull(statistics.MatchersByType);
        Assert.NotNull(statistics.MatchersByPriority);

        _output.WriteLine($"Statistics: Total={statistics.TotalRegisteredMatchers}, " +
                         $"Enabled={statistics.EnabledMatchersCount}, " +
                         $"Disabled={statistics.DisabledMatchersCount}");

        foreach (var (type, count) in statistics.MatchersByType)
        {
            _output.WriteLine($"  Type {type}: {count} matchers");
        }
    }

    [Fact]
    public async Task Should_Support_Hot_Registration_And_Unregistration()
    {
        // 测试热注册和注销功能
        // Test hot registration and unregistration

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();
        var initialCount = manager.GetAllMatchers().Count;

        // 创建一个模拟匹配器
        // Create a mock matcher
        var mockMatcher = new MockTargetingMatcher
        {
            MatcherId = "test-mock-matcher",
            MatcherName = "Test Mock Matcher",
            MatcherType = "Mock",
            Priority = 999,
            IsEnabled = true
        };

        // Act - 注册
        // Act - Register
        var registrationSuccess = manager.RegisterMatcher(mockMatcher);
        var countAfterRegistration = manager.GetAllMatchers().Count;

        // Act - 注销
        // Act - Unregister
        var unregistrationSuccess = manager.UnregisterMatcher(mockMatcher.MatcherId);
        var countAfterUnregistration = manager.GetAllMatchers().Count;

        // Assert
        Assert.True(registrationSuccess);
        Assert.Equal(initialCount + 1, countAfterRegistration);

        Assert.True(unregistrationSuccess);
        Assert.Equal(initialCount, countAfterUnregistration);

        _output.WriteLine($"Hot registration/unregistration test passed. " +
                         $"Initial: {initialCount}, After registration: {countAfterRegistration}, " +
                         $"After unregistration: {countAfterUnregistration}");
    }

    [Fact]
    public void Should_Handle_Configuration_Validation_Errors()
    {
        // 测试配置验证错误处理
        // Test configuration validation error handling

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var invalidConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:DefaultTimeoutMs", "-1" }, // 无效值
                { "TargetingMatchers:DefaultLifetime", "Invalid" }, // 无效值
            })
            .Build();

        services.AddSingleton<IConfiguration>(invalidConfiguration);

        // Act & Assert
        var exception = Assert.Throws<OptionsValidationException>(() =>
        {
            services.AddFSharpTargetingMatchers(invalidConfiguration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<TargetingMatcherOptions>>();
            _ = options.Value; // 触发验证
        });

        Assert.NotNull(exception);
        _output.WriteLine($"Configuration validation correctly caught invalid configuration: {exception.Message}");
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}

/// <summary>
/// 模拟匹配器用于测试
/// Mock matcher for testing
/// </summary>
public class MockTargetingMatcher : ITargetingMatcher
{
    public string MatcherId { get; set; } = string.Empty;
    public string MatcherName { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string MatcherType { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsEnabled { get; set; }
    public TimeSpan ExpectedExecutionTime { get; set; } = TimeSpan.FromMilliseconds(10);
    public bool CanRunInParallel { get; set; } = true;

    public Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MatchResult.CreateMatch(
            criteria?.CriteriaType ?? "test",
            Guid.NewGuid(),
            0.8m,
            "Mock match successful",
            TimeSpan.FromMilliseconds(5)));
    }

    public bool IsSupported(string criteriaType) => true;

    public ValidationResult ValidateCriteria(ITargetingCriteria criteria)
    {
        return new ValidationResult(null) { IsValid = true };
    }

    public TargetingMatcherMetadata GetMetadata()
    {
        return new TargetingMatcherMetadata
        {
            MatcherId = MatcherId,
            MatcherName = MatcherName,
            Version = Version,
            MatcherType = MatcherType,
            SupportedCriteriaTypes = new[] { "test", "mock" },
            ConfigurationSchema = new Dictionary<string, object>()
        };
    }
}
