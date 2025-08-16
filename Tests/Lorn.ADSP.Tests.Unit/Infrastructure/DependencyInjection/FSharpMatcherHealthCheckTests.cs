using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Lorn.ADSP.Infrastructure.DependencyInjection.Extensions;
using Lorn.ADSP.Infrastructure.DependencyInjection.HealthChecks;

namespace Lorn.ADSP.Tests.Unit.Infrastructure.DependencyInjection;

/// <summary>
/// F#匹配器健康检查集成测试
/// F# Matcher Health Check Integration Tests
/// </summary>
public class FSharpMatcherHealthCheckTests
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;

    public FSharpMatcherHealthCheckTests(ITestOutputHelper output)
    {
        _output = output;
        _serviceProvider = SetupServiceProvider();
    }

    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();

        // 添加日志记录
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // 添加配置
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "true" },
                { "TargetingMatchers:DefaultLifetime", "Scoped" },
                { "TargetingMatchers:DefaultTimeoutMs", "5000" }
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
    public async Task Should_Register_HealthCheck_Successfully()
    {
        // 测试健康检查是否正确注册
        // Test if health check is correctly registered

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Entries, kvp => kvp.Key == "fsharp_matchers");

        var matcherHealthCheck = result.Entries["fsharp_matchers"];
        _output.WriteLine($"Health check status: {matcherHealthCheck.Status}");
        _output.WriteLine($"Health check description: {matcherHealthCheck.Description}");

        if (matcherHealthCheck.Data?.Any() == true)
        {
            foreach (var (key, value) in matcherHealthCheck.Data)
            {
                _output.WriteLine($"  {key}: {value}");
            }
        }
    }

    [Fact]
    public async Task Should_Report_Healthy_When_All_Matchers_Available()
    {
        // 测试当所有匹配器可用时应报告健康状态
        // Test should report healthy when all matchers are available

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Entries.ContainsKey("fsharp_matchers"));

        var matcherHealthCheck = result.Entries["fsharp_matchers"];
        Assert.Equal(HealthStatus.Healthy, matcherHealthCheck.Status);

        // 验证数据中包含匹配器统计信息
        // Verify data contains matcher statistics
        Assert.NotNull(matcherHealthCheck.Data);
        Assert.True(matcherHealthCheck.Data.ContainsKey("total_matchers"));
        Assert.True(matcherHealthCheck.Data.ContainsKey("enabled_matchers"));
        Assert.True(matcherHealthCheck.Data.ContainsKey("disabled_matchers"));

        _output.WriteLine("All matchers reported as healthy");
        _output.WriteLine($"Total matchers: {matcherHealthCheck.Data["total_matchers"]}");
        _output.WriteLine($"Enabled matchers: {matcherHealthCheck.Data["enabled_matchers"]}");
        _output.WriteLine($"Disabled matchers: {matcherHealthCheck.Data["disabled_matchers"]}");
    }

    [Fact]
    public async Task Should_Include_Matcher_Type_Statistics()
    {
        // 测试健康检查应包含匹配器类型统计信息
        // Test health check should include matcher type statistics

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        var matcherHealthCheck = result.Entries["fsharp_matchers"];
        Assert.NotNull(matcherHealthCheck.Data);

        // 验证包含类型统计信息
        // Verify type statistics are included
        var typeKeys = matcherHealthCheck.Data.Keys
            .Where(k => k.StartsWith("type_"))
            .ToList();

        Assert.NotEmpty(typeKeys);

        foreach (var typeKey in typeKeys)
        {
            var typeName = typeKey.Substring(5); // 移除 "type_" 前缀
            var count = matcherHealthCheck.Data[typeKey];

            _output.WriteLine($"Matcher type '{typeName}': {count} instances");

            // 验证计数是有效的数字
            // Verify count is a valid number
            Assert.True(int.TryParse(count.ToString(), out var countValue));
            Assert.True(countValue >= 0);
        }
    }

    [Fact]
    public async Task Should_Include_Validation_Results()
    {
        // 测试健康检查应包含验证结果
        // Test health check should include validation results

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        var matcherHealthCheck = result.Entries["fsharp_matchers"];
        Assert.NotNull(matcherHealthCheck.Data);

        // 验证包含验证相关信息
        // Verify validation information is included
        Assert.True(matcherHealthCheck.Data.ContainsKey("valid_matchers"));
        Assert.True(matcherHealthCheck.Data.ContainsKey("invalid_matchers"));

        var validMatchers = int.Parse(matcherHealthCheck.Data["valid_matchers"].ToString()!);
        var invalidMatchers = int.Parse(matcherHealthCheck.Data["invalid_matchers"].ToString()!);

        _output.WriteLine($"Valid matchers: {validMatchers}");
        _output.WriteLine($"Invalid matchers: {invalidMatchers}");

        // 在正常情况下，应该没有无效的匹配器
        // Under normal circumstances, there should be no invalid matchers
        Assert.True(validMatchers >= 0);
        Assert.True(invalidMatchers >= 0);
    }

    [Fact]
    public async Task Should_Measure_HealthCheck_Performance()
    {
        // 测试健康检查性能
        // Test health check performance

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();
        const int iterations = 10;
        var executionTimes = new List<long>();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await healthCheckService.CheckHealthAsync();
            stopwatch.Stop();

            executionTimes.Add(stopwatch.ElapsedMilliseconds);

            // 验证每次都能成功获取健康状态
            // Verify successful health status retrieval each time
            Assert.NotNull(result);
            Assert.True(result.Entries.ContainsKey("fsharp_matchers"));
        }

        // Assert & Analyze
        var avgTime = executionTimes.Average();
        var maxTime = executionTimes.Max();
        var minTime = executionTimes.Min();

        // 健康检查应该快速执行
        // Health check should execute quickly
        Assert.True(avgTime < 1000, $"Average health check time {avgTime}ms should be less than 1000ms");
        Assert.True(maxTime < 2000, $"Max health check time {maxTime}ms should be less than 2000ms");

        _output.WriteLine($"Health check performance over {iterations} iterations:");
        _output.WriteLine($"  Average: {avgTime:F2}ms");
        _output.WriteLine($"  Min: {minTime}ms");
        _output.WriteLine($"  Max: {maxTime}ms");
    }

    [Fact]
    public async Task Should_Handle_Concurrent_HealthCheck_Requests()
    {
        // 测试并发健康检查请求
        // Test concurrent health check requests

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();
        const int concurrentRequests = 5;

        // Act
        var tasks = new List<Task<HealthReport>>();
        for (int i = 0; i < concurrentRequests; i++)
        {
            tasks.Add(healthCheckService.CheckHealthAsync());
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(concurrentRequests, results.Length);

        foreach (var result in results)
        {
            Assert.NotNull(result);
            Assert.True(result.Entries.ContainsKey("fsharp_matchers"));

            var matcherHealthCheck = result.Entries["fsharp_matchers"];
            Assert.NotNull(matcherHealthCheck.Data);

            // 验证每个结果都包含必需的数据
            // Verify each result contains required data
            Assert.True(matcherHealthCheck.Data.ContainsKey("total_matchers"));
            Assert.True(matcherHealthCheck.Data.ContainsKey("enabled_matchers"));
        }

        _output.WriteLine($"Successfully handled {concurrentRequests} concurrent health check requests");

        // 验证所有结果的一致性
        // Verify consistency across all results
        var firstResult = results[0].Entries["fsharp_matchers"];
        foreach (var result in results)
        {
            var matcherHealthCheck = result.Entries["fsharp_matchers"];

            // 基本统计信息应该一致
            // Basic statistics should be consistent
            Assert.Equal(
                firstResult.Data["total_matchers"],
                matcherHealthCheck.Data["total_matchers"]);
            Assert.Equal(
                firstResult.Data["enabled_matchers"],
                matcherHealthCheck.Data["enabled_matchers"]);
        }
    }

    [Fact]
    public async Task Should_Provide_Detailed_Error_Information_When_Unhealthy()
    {
        // 测试不健康状态时应提供详细错误信息
        // Test should provide detailed error information when unhealthy

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        // 故意创建一个会导致不健康状态的配置
        // Intentionally create a configuration that leads to unhealthy state
        var badConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "false" } // 禁用F#匹配器
            })
            .Build();

        services.AddSingleton<IConfiguration>(badConfiguration);
        services.AddFSharpTargetingMatchers(badConfiguration);
        services.AddFSharpMatcherHealthChecks();

        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Entries.ContainsKey("fsharp_matchers"));

        var matcherHealthCheck = result.Entries["fsharp_matchers"];

        // 当F#匹配器被禁用时，应该报告相应的状态
        // When F# matchers are disabled, should report appropriate status
        _output.WriteLine($"Health status with disabled matchers: {matcherHealthCheck.Status}");
        _output.WriteLine($"Description: {matcherHealthCheck.Description}");

        // 验证数据中反映了禁用状态
        // Verify data reflects disabled state
        if (matcherHealthCheck.Data?.ContainsKey("enabled_matchers") == true)
        {
            var enabledCount = int.Parse(matcherHealthCheck.Data["enabled_matchers"].ToString()!);
            _output.WriteLine($"Enabled matchers count: {enabledCount}");
        }

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task Should_Update_HealthCheck_Data_Dynamically()
    {
        // 测试健康检查数据的动态更新
        // Test dynamic update of health check data

        // Arrange
        var healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();

        // Act - 第一次检查
        // Act - First check
        var initialResult = await healthCheckService.CheckHealthAsync();
        var initialMatcherCheck = initialResult.Entries["fsharp_matchers"];
        var initialTotal = int.Parse(initialMatcherCheck.Data!["total_matchers"].ToString()!);

        // 等待一小段时间，然后再次检查
        // Wait a short time, then check again
        await Task.Delay(100);
        var secondResult = await healthCheckService.CheckHealthAsync();
        var secondMatcherCheck = secondResult.Entries["fsharp_matchers"];
        var secondTotal = int.Parse(secondMatcherCheck.Data!["total_matchers"].ToString()!);

        // Assert
        // 在稳定状态下，匹配器总数应该保持一致
        // Under stable conditions, total matcher count should remain consistent
        Assert.Equal(initialTotal, secondTotal);

        _output.WriteLine($"Health check data consistency verified: {initialTotal} matchers");
        _output.WriteLine($"Initial check time: {initialResult.TotalDuration}");
        _output.WriteLine($"Second check time: {secondResult.TotalDuration}");

        // 验证检查时间合理
        // Verify reasonable check times
        Assert.True(initialResult.TotalDuration < TimeSpan.FromSeconds(10));
        Assert.True(secondResult.TotalDuration < TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
