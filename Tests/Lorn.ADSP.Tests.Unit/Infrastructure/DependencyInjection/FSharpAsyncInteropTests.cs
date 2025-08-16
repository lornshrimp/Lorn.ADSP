using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Infrastructure.DependencyInjection.Services;

namespace Lorn.ADSP.Tests.Unit.Infrastructure.DependencyInjection;

/// <summary>
/// F#异步工作流与C#异步调用互操作性测试
/// F# Async Workflow and C# Async Call Interoperability Tests
/// </summary>
public class FSharpAsyncInteropTests
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;

    public FSharpAsyncInteropTests(ITestOutputHelper output)
    {
        _output = output;
        _serviceProvider = SetupServiceProvider();
    }

    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        // 注册模拟的F#匹配器
        // Register mock F# matchers
        services.AddScoped<ITargetingMatcher, MockFSharpDemographicMatcher>();
        services.AddScoped<ITargetingMatcher, MockFSharpGeolocationMatcher>();
        services.AddScoped<ITargetingMatcherManager, TargetingMatcherManager>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Should_Execute_FSharp_Matcher_Async_Correctly()
    {
        // 测试F#匹配器的异步执行
        // Test F# matcher async execution

        // Arrange
        var matcher = _serviceProvider.GetServices<ITargetingMatcher>()
            .First(m => m.MatcherType == "demographic");

        var context = new MockTargetingContext();
        var criteria = new MockTargetingCriteria { CriteriaType = "demographic" };
        var callbackProvider = new MockCallbackProvider();

        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await matcher.CalculateMatchScoreAsync(context, criteria, callbackProvider);

        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsMatched);
        Assert.True(result.MatchScore >= 0);
        Assert.True(result.MatchScore <= 1);
        Assert.NotNull(result.CriteriaType);
        Assert.True(result.ExecutionTime > TimeSpan.Zero);

        _output.WriteLine($"F# Matcher execution completed in {stopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"Match result: Score={result.MatchScore}, Matched={result.IsMatched}");
    }

    [Fact]
    public async Task Should_Execute_Multiple_FSharp_Matchers_In_Parallel()
    {
        // 测试多个F#匹配器的并行执行
        // Test parallel execution of multiple F# matchers

        // Arrange
        var matchers = _serviceProvider.GetServices<ITargetingMatcher>().ToList();
        var context = new MockTargetingContext();
        var callbackProvider = new MockCallbackProvider();

        var tasks = new List<Task<MatchResult>>();

        // Act
        var stopwatch = Stopwatch.StartNew();

        foreach (var matcher in matchers)
        {
            var criteria = new MockTargetingCriteria { CriteriaType = matcher.MatcherType };
            tasks.Add(matcher.CalculateMatchScoreAsync(context, criteria, callbackProvider));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        Assert.Equal(matchers.Count, results.Length);
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.True(result.ExecutionTime > TimeSpan.Zero);
        });

        _output.WriteLine($"Parallel execution of {matchers.Count} matchers completed in {stopwatch.ElapsedMilliseconds}ms");

        for (int i = 0; i < results.Length; i++)
        {
            _output.WriteLine($"Matcher {matchers[i].MatcherType}: Score={results[i].MatchScore}, Time={results[i].ExecutionTime.TotalMilliseconds}ms");
        }
    }

    [Fact]
    public async Task Should_Handle_FSharp_Matcher_Timeout_Correctly()
    {
        // 测试F#匹配器超时处理
        // Test F# matcher timeout handling

        // Arrange
        var timeoutMatcher = new MockSlowFSharpMatcher();
        var context = new MockTargetingContext();
        var criteria = new MockTargetingCriteria { CriteriaType = "slow" };
        var callbackProvider = new MockCallbackProvider();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() =>
            timeoutMatcher.CalculateMatchScoreAsync(context, criteria, callbackProvider, cts.Token));

        Assert.NotNull(exception);
        _output.WriteLine($"Timeout correctly handled: {exception.GetType().Name}");
    }

    [Fact]
    public async Task Should_Handle_FSharp_Matcher_Exception_Correctly()
    {
        // 测试F#匹配器异常处理
        // Test F# matcher exception handling

        // Arrange
        var errorMatcher = new MockErrorFSharpMatcher();
        var context = new MockTargetingContext();
        var criteria = new MockTargetingCriteria { CriteriaType = "error" };
        var callbackProvider = new MockCallbackProvider();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            errorMatcher.CalculateMatchScoreAsync(context, criteria, callbackProvider));

        Assert.NotNull(exception);
        Assert.Contains("Simulated F# matcher error", exception.Message);
        _output.WriteLine($"Exception correctly handled: {exception.Message}");
    }

    [Fact]
    public async Task Should_Execute_TargetingMatcherManager_With_FSharp_Matchers()
    {
        // 测试匹配器管理器与F#匹配器的集成
        // Test targeting matcher manager integration with F# matchers

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();
        var context = new MockTargetingContext();
        var criteriaList = new List<ITargetingCriteria>
        {
            new MockTargetingCriteria { CriteriaType = "demographic" },
            new MockTargetingCriteria { CriteriaType = "geolocation" }
        };
        var callbackProvider = new MockCallbackProvider();

        var stopwatch = Stopwatch.StartNew();

        // Act
        var overallResult = await manager.ExecuteMatchingAsync(context, criteriaList, callbackProvider);

        stopwatch.Stop();

        // Assert
        Assert.NotNull(overallResult);
        Assert.NotNull(overallResult.Results);
        Assert.True(overallResult.Results.Count > 0);
        Assert.True(overallResult.TotalExecutionTime > TimeSpan.Zero);
        Assert.True(overallResult.TotalExecutionTime < TimeSpan.FromSeconds(10)); // 确保不会太慢

        _output.WriteLine($"Manager execution completed in {stopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"Overall result: TotalScore={overallResult.TotalScore}, " +
                         $"MatchedCount={overallResult.Results.Count(r => r.IsMatched)}, " +
                         $"TotalTime={overallResult.TotalExecutionTime.TotalMilliseconds}ms");

        foreach (var result in overallResult.Results)
        {
            _output.WriteLine($"  {result.CriteriaType}: Score={result.MatchScore}, " +
                             $"Matched={result.IsMatched}, Time={result.ExecutionTime.TotalMilliseconds}ms");
        }
    }

    [Fact]
    public async Task Should_Maintain_Thread_Safety_With_Concurrent_FSharp_Calls()
    {
        // 测试F#匹配器的线程安全性
        // Test thread safety of F# matchers

        // Arrange
        var manager = _serviceProvider.GetRequiredService<ITargetingMatcherManager>();
        var context = new MockTargetingContext();
        var criteriaList = new List<ITargetingCriteria>
        {
            new MockTargetingCriteria { CriteriaType = "demographic" }
        };
        var callbackProvider = new MockCallbackProvider();

        const int concurrentCalls = 10;
        var tasks = new List<Task<OverallMatchResult>>();

        // Act
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < concurrentCalls; i++)
        {
            tasks.Add(manager.ExecuteMatchingAsync(context, criteriaList, callbackProvider));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        Assert.Equal(concurrentCalls, results.Length);
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.NotNull(result.Results);
            Assert.True(result.Results.Count > 0);
        });

        _output.WriteLine($"Concurrent execution of {concurrentCalls} calls completed in {stopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"Average time per call: {stopwatch.ElapsedMilliseconds / (double)concurrentCalls:F2}ms");

        // 验证所有结果都是有效的且一致的
        // Verify all results are valid and consistent
        var firstResult = results[0];
        foreach (var result in results)
        {
            Assert.Equal(firstResult.Results.Count, result.Results.Count);
            for (int i = 0; i < firstResult.Results.Count; i++)
            {
                Assert.Equal(firstResult.Results[i].CriteriaType, result.Results[i].CriteriaType);
                // 注意：由于可能有随机性，不要求分数完全相同
                // Note: Don't require exact score match due to potential randomness
            }
        }
    }

    [Fact]
    public async Task Should_Measure_FSharp_Async_Performance()
    {
        // 测试F#异步性能
        // Test F# async performance

        // Arrange
        var matcher = _serviceProvider.GetServices<ITargetingMatcher>()
            .First(m => m.MatcherType == "demographic");

        var context = new MockTargetingContext();
        var criteria = new MockTargetingCriteria { CriteriaType = "demographic" };
        var callbackProvider = new MockCallbackProvider();

        const int iterations = 100;
        var executionTimes = new List<long>();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await matcher.CalculateMatchScoreAsync(context, criteria, callbackProvider);
            stopwatch.Stop();

            executionTimes.Add(stopwatch.ElapsedMilliseconds);
            Assert.NotNull(result);
        }

        // Assert & Analyze
        var avgTime = executionTimes.Average();
        var minTime = executionTimes.Min();
        var maxTime = executionTimes.Max();
        var p95Time = executionTimes.OrderBy(t => t).Skip((int)(iterations * 0.95)).First();

        Assert.True(avgTime < 100, $"Average execution time {avgTime}ms should be less than 100ms");
        Assert.True(p95Time < 200, $"95th percentile execution time {p95Time}ms should be less than 200ms");

        _output.WriteLine($"Performance metrics over {iterations} iterations:");
        _output.WriteLine($"  Average: {avgTime:F2}ms");
        _output.WriteLine($"  Min: {minTime}ms");
        _output.WriteLine($"  Max: {maxTime}ms");
        _output.WriteLine($"  95th percentile: {p95Time}ms");
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}

// Mock implementations for testing
public class MockFSharpDemographicMatcher : ITargetingMatcher
{
    public string MatcherId => "mock-demographic";
    public string MatcherName => "Mock Demographic Matcher";
    public string Version => "1.0.0";
    public string MatcherType => "demographic";
    public int Priority => 100;
    public bool IsEnabled => true;
    public TimeSpan ExpectedExecutionTime => TimeSpan.FromMilliseconds(10);
    public bool CanRunInParallel => true;

    public async Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        // 模拟F#异步工作流
        // Simulate F# async workflow
        await Task.Delay(Random.Shared.Next(5, 15), cancellationToken);

        var score = Random.Shared.NextSingle();
        return MatchResult.CreateMatch(
            criteria.CriteriaType,
            Guid.NewGuid(),
            (decimal)score,
            "Mock demographic match",
            TimeSpan.FromMilliseconds(Random.Shared.Next(5, 15)));
    }

    public bool IsSupported(string criteriaType) => criteriaType == "demographic";
    public ValidationResult ValidateCriteria(ITargetingCriteria criteria) => new ValidationResult(null) { IsValid = true };
    public TargetingMatcherMetadata GetMetadata() => new() { MatcherId = MatcherId, MatcherName = MatcherName };
}

public class MockFSharpGeolocationMatcher : ITargetingMatcher
{
    public string MatcherId => "mock-geolocation";
    public string MatcherName => "Mock Geolocation Matcher";
    public string Version => "1.0.0";
    public string MatcherType => "geolocation";
    public int Priority => 90;
    public bool IsEnabled => true;
    public TimeSpan ExpectedExecutionTime => TimeSpan.FromMilliseconds(20);
    public bool CanRunInParallel => true;

    public async Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(Random.Shared.Next(10, 25), cancellationToken);

        var score = Random.Shared.NextSingle();
        return MatchResult.CreateMatch(
            criteria.CriteriaType,
            Guid.NewGuid(),
            (decimal)score,
            "Mock geolocation match",
            TimeSpan.FromMilliseconds(Random.Shared.Next(10, 25)));
    }

    public bool IsSupported(string criteriaType) => criteriaType == "geolocation";
    public ValidationResult ValidateCriteria(ITargetingCriteria criteria) => new ValidationResult(null) { IsValid = true };
    public TargetingMatcherMetadata GetMetadata() => new() { MatcherId = MatcherId, MatcherName = MatcherName };
}

public class MockSlowFSharpMatcher : ITargetingMatcher
{
    public string MatcherId => "mock-slow";
    public string MatcherName => "Mock Slow Matcher";
    public string Version => "1.0.0";
    public string MatcherType => "slow";
    public int Priority => 50;
    public bool IsEnabled => true;
    public TimeSpan ExpectedExecutionTime => TimeSpan.FromSeconds(1);
    public bool CanRunInParallel => true;

    public async Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        // 故意延迟以测试超时
        // Intentional delay to test timeout
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        return MatchResult.CreateMatch(criteria.CriteriaType, Guid.NewGuid(), 0.5m, "Slow match", TimeSpan.FromSeconds(5));
    }

    public bool IsSupported(string criteriaType) => criteriaType == "slow";
    public ValidationResult ValidateCriteria(ITargetingCriteria criteria) => new ValidationResult(null) { IsValid = true };
    public TargetingMatcherMetadata GetMetadata() => new() { MatcherId = MatcherId, MatcherName = MatcherName };
}

public class MockErrorFSharpMatcher : ITargetingMatcher
{
    public string MatcherId => "mock-error";
    public string MatcherName => "Mock Error Matcher";
    public string Version => "1.0.0";
    public string MatcherType => "error";
    public int Priority => 10;
    public bool IsEnabled => true;
    public TimeSpan ExpectedExecutionTime => TimeSpan.FromMilliseconds(5);
    public bool CanRunInParallel => true;

    public async Task<MatchResult> CalculateMatchScoreAsync(
        ITargetingContext context,
        ITargetingCriteria criteria,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);
        throw new InvalidOperationException("Simulated F# matcher error for testing");
    }

    public bool IsSupported(string criteriaType) => criteriaType == "error";
    public ValidationResult ValidateCriteria(ITargetingCriteria criteria) => new ValidationResult(null) { IsValid = true };
    public TargetingMatcherMetadata GetMetadata() => new() { MatcherId = MatcherId, MatcherName = MatcherName };
}

// Mock support classes
public class MockTargetingContext : ITargetingContext
{
    public Guid RequestId { get; } = Guid.NewGuid();
    public DateTime RequestTime { get; } = DateTime.UtcNow;
    public Dictionary<string, object> Properties { get; } = new();
    public IUserContext UserContext { get; } = new MockUserContext();
    public IDeviceContext DeviceContext { get; } = new MockDeviceContext();
    public IGeographicContext GeographicContext { get; } = new MockGeographicContext();
    public ITimeContext TimeContext { get; } = new MockTimeContext();
}

public class MockTargetingCriteria : ITargetingCriteria
{
    public string CriteriaId { get; set; } = Guid.NewGuid().ToString();
    public string CriteriaType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsRequired { get; set; } = false;
    public decimal Weight { get; set; } = 1.0m;
}

public class MockCallbackProvider : ICallbackProvider
{
    public Task OnMatchStartedAsync(string matcherId, ITargetingCriteria criteria) => Task.CompletedTask;
    public Task OnMatchCompletedAsync(string matcherId, MatchResult result) => Task.CompletedTask;
    public Task OnMatchErrorAsync(string matcherId, Exception exception) => Task.CompletedTask;
}

public class MockUserContext : IUserContext
{
    public string UserId { get; } = "test-user";
    public int? Age { get; } = 25;
    public string? Gender { get; } = "M";
    public List<string> Interests { get; } = new() { "technology", "sports" };
    public Dictionary<string, object> CustomAttributes { get; } = new();
}

public class MockDeviceContext : IDeviceContext
{
    public string DeviceType { get; } = "mobile";
    public string OperatingSystem { get; } = "iOS";
    public string Browser { get; } = "Safari";
    public string DeviceId { get; } = "test-device";
    public Dictionary<string, object> CustomAttributes { get; } = new();
}

public class MockGeographicContext : IGeographicContext
{
    public string? Country { get; } = "US";
    public string? Region { get; } = "CA";
    public string? City { get; } = "San Francisco";
    public double? Latitude { get; } = 37.7749;
    public double? Longitude { get; } = -122.4194;
    public Dictionary<string, object> CustomAttributes { get; } = new();
}

public class MockTimeContext : ITimeContext
{
    public DateTime CurrentTime { get; } = DateTime.UtcNow;
    public TimeZoneInfo TimeZone { get; } = TimeZoneInfo.Utc;
    public DayOfWeek DayOfWeek { get; } = DateTime.UtcNow.DayOfWeek;
    public int HourOfDay { get; } = DateTime.UtcNow.Hour;
    public Dictionary<string, object> CustomAttributes { get; } = new();
}
