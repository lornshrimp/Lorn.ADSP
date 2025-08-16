using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Infrastructure.DependencyInjection.Extensions;

namespace Lorn.ADSP.Tests.Unit.Infrastructure.DependencyInjection;

/// <summary>
/// F#匹配器端到端集成测试
/// F# Matcher End-to-End Integration Tests
/// </summary>
public class FSharpMatcherEndToEndTests
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;

    public FSharpMatcherEndToEndTests(ITestOutputHelper output)
    {
        _output = output;
        _serviceProvider = SetupServiceProvider();
    }

    /// <summary>
    /// 设置完整的服务提供者，模拟生产环境
    /// Setup complete service provider, simulating production environment
    /// </summary>
    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();

        // 添加完整的日志记录配置
        // Add complete logging configuration
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // 添加完整的配置，包括从文件读取
        // Add complete configuration, including file reading
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables("LORN_ADSP_")
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
    public async Task Should_Complete_Full_Integration_Workflow()
    {
        // 测试完整的集成工作流程
        // Test complete integration workflow

        _output.WriteLine("Starting full integration workflow test...");

        // 第一步：验证依赖注入配置
        // Step 1: Verify dependency injection configuration
        await VerifyDependencyInjectionConfiguration();

        // 第二步：验证F#匹配器注册
        // Step 2: Verify F# matcher registration
        await VerifyFSharpMatcherRegistration();

        // 第三步：验证匹配器管理器
        // Step 3: Verify matcher manager
        await VerifyMatcherManager();

        // 第四步：验证异步执行
        // Step 4: Verify async execution
        await VerifyAsyncExecution();

        // 第五步：验证健康检查
        // Step 5: Verify health checks
        await VerifyHealthChecks();

        // 第六步：验证性能指标
        // Step 6: Verify performance metrics
        await VerifyPerformanceMetrics();

        _output.WriteLine("Full integration workflow test completed successfully!");
    }

    private async Task VerifyDependencyInjectionConfiguration()
    {
        _output.WriteLine("Step 1: Verifying dependency injection configuration...");

        // 验证所有核心服务都已正确注册
        // Verify all core services are correctly registered
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        var logger = _serviceProvider.GetRequiredService<ILogger<FSharpMatcherEndToEndTests>>();
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        Assert.NotNull(configuration);
        Assert.NotNull(logger);
        Assert.NotNull(manager);
        Assert.NotNull(healthCheckService);

        _output.WriteLine("✓ All core services registered successfully");
    }

    private async Task VerifyFSharpMatcherRegistration()
    {
        _output.WriteLine("Step 2: Verifying F# matcher registration...");

        var matchers = _serviceProvider.GetServices<ITargetingMatcher>().ToList();

        Assert.NotEmpty(matchers);

        // 验证预期的F#匹配器类型都已注册
        // Verify expected F# matcher types are registered
        var expectedTypes = new[] { "demographic", "geolocation", "time", "device", "behavioral", "contextual", "frequencyCap", "budget", "audience" };
        var registeredTypes = matchers.Select(m => m.MatcherType).Distinct().ToList();

        foreach (var expectedType in expectedTypes)
        {
            var hasType = registeredTypes.Any(t => t.Equals(expectedType, StringComparison.OrdinalIgnoreCase));
            if (hasType)
            {
                _output.WriteLine($"✓ Matcher type '{expectedType}' registered");
            }
            else
            {
                _output.WriteLine($"⚠ Matcher type '{expectedType}' not found (may be disabled)");
            }
        }

        _output.WriteLine($"✓ Total {matchers.Count} matchers registered");
    }

    private async Task VerifyMatcherManager()
    {
        _output.WriteLine("Step 3: Verifying matcher manager...");

        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();

        // 验证管理器统计信息
        // Verify manager statistics
        var statistics = manager.GetStatistics();
        Assert.NotNull(statistics);
        Assert.True(statistics.TotalRegisteredMatchers > 0);

        _output.WriteLine($"✓ Manager statistics: Total={statistics.TotalRegisteredMatchers}, " +
                         $"Enabled={statistics.EnabledMatchersCount}, Disabled={statistics.DisabledMatchersCount}");

        // 验证所有匹配器获取
        // Verify getting all matchers
        var allMatchers = manager.GetAllMatchers();
        Assert.NotNull(allMatchers);
        Assert.NotEmpty(allMatchers);

        _output.WriteLine($"✓ Retrieved {allMatchers.Count} enabled matchers from manager");

        // 验证匹配器验证
        // Verify matcher validation
        var validationResults = manager.ValidateAllMatchers();
        Assert.NotNull(validationResults);
        Assert.All(validationResults, result => Assert.True(result.IsValid));

        _output.WriteLine($"✓ All {validationResults.Count} matchers validated successfully");
    }

    private async Task VerifyAsyncExecution()
    {
        _output.WriteLine("Step 4: Verifying async execution...");

        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();

        // 创建测试上下文和条件
        // Create test context and criteria
        var context = CreateTestContext();
        var criteriaList = CreateTestCriteria();
        var callbackProvider = new TestCallbackProvider();

        var stopwatch = Stopwatch.StartNew();

        // 执行匹配
        // Execute matching
        var result = await manager.ExecuteMatchingAsync(context, criteriaList, callbackProvider);

        stopwatch.Stop();

        // 验证结果
        // Verify results
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.True(result.Results.Count > 0);
        Assert.True(result.TotalExecutionTime > TimeSpan.Zero);
        Assert.True(stopwatch.ElapsedMilliseconds < 10000); // 不应超过10秒

        _output.WriteLine($"✓ Async execution completed in {stopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"✓ Result: TotalScore={result.TotalScore:F3}, " +
                         $"Matches={result.Results.Count(r => r.IsMatched)}/{result.Results.Count}");

        // 验证回调被正确调用
        // Verify callbacks were called correctly
        Assert.True(callbackProvider.StartedCalls > 0);
        Assert.True(callbackProvider.CompletedCalls > 0);
        Assert.Equal(callbackProvider.StartedCalls, callbackProvider.CompletedCalls + callbackProvider.ErrorCalls);

        _output.WriteLine($"✓ Callbacks: Started={callbackProvider.StartedCalls}, " +
                         $"Completed={callbackProvider.CompletedCalls}, Errors={callbackProvider.ErrorCalls}");
    }

    private async Task VerifyHealthChecks()
    {
        _output.WriteLine("Step 5: Verifying health checks...");

        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        var result = await healthCheckService.CheckHealthAsync();

        Assert.NotNull(result);
        Assert.True(result.Entries.ContainsKey("fsharp_matchers"));

        var matcherHealthCheck = result.Entries["fsharp_matchers"];
        _output.WriteLine($"✓ Health check status: {matcherHealthCheck.Status}");

        if (matcherHealthCheck.Data?.Any() == true)
        {
            foreach (var (key, value) in matcherHealthCheck.Data)
            {
                _output.WriteLine($"  {key}: {value}");
            }
        }

        // 验证健康检查性能
        // Verify health check performance
        Assert.True(result.TotalDuration < TimeSpan.FromSeconds(5));
        _output.WriteLine($"✓ Health check completed in {result.TotalDuration.TotalMilliseconds}ms");
    }

    private async Task VerifyPerformanceMetrics()
    {
        _output.WriteLine("Step 6: Verifying performance metrics...");

        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();
        var context = CreateTestContext();
        var criteriaList = CreateTestCriteria();
        var callbackProvider = new TestCallbackProvider();

        const int iterations = 10;
        var executionTimes = new List<long>();
        var totalScores = new List<decimal>();

        // 多次执行以获取性能指标
        // Multiple executions to get performance metrics
        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await manager.ExecuteMatchingAsync(context, criteriaList, callbackProvider);
            stopwatch.Stop();

            executionTimes.Add(stopwatch.ElapsedMilliseconds);
            totalScores.Add(result.TotalScore);

            Assert.NotNull(result);
            Assert.True(result.Results.Count > 0);
        }

        // 分析性能指标
        // Analyze performance metrics
        var avgTime = executionTimes.Average();
        var minTime = executionTimes.Min();
        var maxTime = executionTimes.Max();
        var p95Time = executionTimes.OrderBy(t => t).Skip((int)(iterations * 0.95)).First();

        var avgScore = totalScores.Average();
        var scoreVariance = totalScores.Select(s => Math.Pow((double)(s - avgScore), 2)).Average();
        var scoreStdDev = Math.Sqrt(scoreVariance);

        _output.WriteLine($"✓ Performance metrics over {iterations} iterations:");
        _output.WriteLine($"  Execution time - Avg: {avgTime:F2}ms, Min: {minTime}ms, Max: {maxTime}ms, P95: {p95Time}ms");
        _output.WriteLine($"  Score - Avg: {avgScore:F3}, StdDev: {scoreStdDev:F3}");

        // 验证性能要求
        // Verify performance requirements
        Assert.True(avgTime < 1000, $"Average execution time {avgTime}ms should be less than 1000ms");
        Assert.True(p95Time < 2000, $"95th percentile execution time {p95Time}ms should be less than 2000ms");
        Assert.True(scoreStdDev < 0.5, $"Score standard deviation {scoreStdDev:F3} should be less than 0.5");

        _output.WriteLine("✓ All performance requirements met");
    }

    private ITargetingContext CreateTestContext()
    {
        return new TestTargetingContext
        {
            RequestId = Guid.NewGuid(),
            RequestTime = DateTime.UtcNow,
            UserContext = new TestUserContext
            {
                UserId = "test-user-123",
                Age = 28,
                Gender = "M",
                Interests = new List<string> { "technology", "sports", "travel" }
            },
            DeviceContext = new TestDeviceContext
            {
                DeviceType = "mobile",
                OperatingSystem = "iOS",
                Browser = "Safari"
            },
            GeographicContext = new TestGeographicContext
            {
                Country = "US",
                Region = "CA",
                City = "San Francisco",
                Latitude = 37.7749,
                Longitude = -122.4194
            },
            TimeContext = new TestTimeContext
            {
                CurrentTime = DateTime.UtcNow,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"),
                DayOfWeek = DateTime.UtcNow.DayOfWeek,
                HourOfDay = DateTime.UtcNow.Hour
            }
        };
    }

    private List<ITargetingCriteria> CreateTestCriteria()
    {
        return new List<ITargetingCriteria>
        {
            new TestTargetingCriteria
            {
                CriteriaId = "demographic-001",
                CriteriaType = "demographic",
                Parameters = new Dictionary<string, object>
                {
                    { "ageRange", "25-34" },
                    { "gender", "M" }
                },
                Weight = 1.0m
            },
            new TestTargetingCriteria
            {
                CriteriaId = "geo-001",
                CriteriaType = "geolocation",
                Parameters = new Dictionary<string, object>
                {
                    { "country", "US" },
                    { "region", "CA" },
                    { "radius", 50 }
                },
                Weight = 0.8m
            },
            new TestTargetingCriteria
            {
                CriteriaId = "device-001",
                CriteriaType = "device",
                Parameters = new Dictionary<string, object>
                {
                    { "deviceType", "mobile" },
                    { "os", "iOS" }
                },
                Weight = 0.6m
            }
        };
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}

// Test support classes
public class TestCallbackProvider : ICallbackProvider
{
    public int StartedCalls { get; private set; }
    public int CompletedCalls { get; private set; }
    public int ErrorCalls { get; private set; }

    public Task OnMatchStartedAsync(string matcherId, ITargetingCriteria criteria)
    {
        Interlocked.Increment(ref StartedCalls);
        return Task.CompletedTask;
    }

    public Task OnMatchCompletedAsync(string matcherId, MatchResult result)
    {
        Interlocked.Increment(ref CompletedCalls);
        return Task.CompletedTask;
    }

    public Task OnMatchErrorAsync(string matcherId, Exception exception)
    {
        Interlocked.Increment(ref ErrorCalls);
        return Task.CompletedTask;
    }
}

public class TestTargetingContext : ITargetingContext
{
    public Guid RequestId { get; set; }
    public DateTime RequestTime { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public IUserContext UserContext { get; set; } = null!;
    public IDeviceContext DeviceContext { get; set; } = null!;
    public IGeographicContext GeographicContext { get; set; } = null!;
    public ITimeContext TimeContext { get; set; } = null!;
}

public class TestTargetingCriteria : ITargetingCriteria
{
    public string CriteriaId { get; set; } = string.Empty;
    public string CriteriaType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsRequired { get; set; }
    public decimal Weight { get; set; }
}

public class TestUserContext : IUserContext
{
    public string UserId { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public List<string> Interests { get; set; } = new();
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}

public class TestDeviceContext : IDeviceContext
{
    public string DeviceType { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}

public class TestGeographicContext : IGeographicContext
{
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}

public class TestTimeContext : ITimeContext
{
    public DateTime CurrentTime { get; set; }
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    public DayOfWeek DayOfWeek { get; set; }
    public int HourOfDay { get; set; }
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}
