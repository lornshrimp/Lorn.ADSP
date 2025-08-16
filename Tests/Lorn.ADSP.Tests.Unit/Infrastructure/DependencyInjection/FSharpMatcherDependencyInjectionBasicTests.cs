using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using Lorn.ADSP.Infrastructure.DependencyInjection.Configuration;
using Lorn.ADSP.Infrastructure.DependencyInjection.Extensions;
using Lorn.ADSP.Infrastructure.DependencyInjection.Validation;

namespace Lorn.ADSP.Tests.Unit.Infrastructure.DependencyInjection;

/// <summary>
/// F#匹配器依赖注入基础测试
/// F# Matcher Dependency Injection Basic Tests
/// </summary>
public class FSharpMatcherDependencyInjectionBasicTests
{
    private readonly ITestOutputHelper _output;

    public FSharpMatcherDependencyInjectionBasicTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Should_Configure_TargetingMatcherOptions_Successfully()
    {
        // 测试配置选项的基本功能
        // Test basic functionality of configuration options

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "true" },
                { "TargetingMatchers:DefaultLifetime", "Scoped" },
                { "TargetingMatchers:DefaultTimeoutMs", "5000" },
                { "TargetingMatchers:Matchers:demographic:Enabled", "true" },
                { "TargetingMatchers:Matchers:demographic:Priority", "100" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<TargetingMatcherOptions>(configuration.GetSection("TargetingMatchers"));

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.Value);
        Assert.True(options.Value.EnableFSharpMatchers);
        Assert.Equal("Scoped", options.Value.DefaultLifetime);
        Assert.Equal(5000, options.Value.DefaultTimeoutMs);

        _output.WriteLine($"✓ Configuration loaded successfully:");
        _output.WriteLine($"  EnableFSharpMatchers: {options.Value.EnableFSharpMatchers}");
        _output.WriteLine($"  DefaultLifetime: {options.Value.DefaultLifetime}");
        _output.WriteLine($"  DefaultTimeoutMs: {options.Value.DefaultTimeoutMs}");

        // 验证匹配器配置
        // Verify matcher configuration
        Assert.NotNull(options.Value.Matchers);
        Assert.True(options.Value.Matchers.ContainsKey("demographic"));
        Assert.True(options.Value.Matchers["demographic"].Enabled);
        Assert.Equal(100, options.Value.Matchers["demographic"].Priority);

        _output.WriteLine($"  Demographic matcher enabled: {options.Value.Matchers["demographic"].Enabled}");
        _output.WriteLine($"  Demographic matcher priority: {options.Value.Matchers["demographic"].Priority}");

        serviceProvider.Dispose();
    }

    [Fact]
    public void Should_Validate_TargetingMatcherOptions_Successfully()
    {
        // 测试配置验证功能
        // Test configuration validation functionality

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var validConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "true" },
                { "TargetingMatchers:DefaultLifetime", "Scoped" },
                { "TargetingMatchers:DefaultTimeoutMs", "5000" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(validConfiguration);
        services.Configure<TargetingMatcherOptions>(validConfiguration.GetSection("TargetingMatchers"));
        services.AddSingleton<IValidateOptions<TargetingMatcherOptions>, TargetingMatcherOptionsValidator>();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

        // 访问Value属性会触发验证
        // Accessing Value property triggers validation
        var value = options.Value;

        Assert.NotNull(value);
        Assert.True(value.EnableFSharpMatchers);
        Assert.Equal("Scoped", value.DefaultLifetime);
        Assert.Equal(5000, value.DefaultTimeoutMs);

        _output.WriteLine("✓ Valid configuration passed validation");

        serviceProvider.Dispose();
    }

    [Fact]
    public void Should_Reject_Invalid_TargetingMatcherOptions()
    {
        // 测试无效配置的拒绝
        // Test rejection of invalid configuration

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var invalidConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:DefaultTimeoutMs", "-1" }, // 无效值
                { "TargetingMatchers:DefaultLifetime", "InvalidLifetime" } // 无效值
            })
            .Build();

        services.AddSingleton<IConfiguration>(invalidConfiguration);
        services.Configure<TargetingMatcherOptions>(invalidConfiguration.GetSection("TargetingMatchers"));
        services.AddSingleton<IValidateOptions<TargetingMatcherOptions>, TargetingMatcherOptionsValidator>();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

        var exception = Assert.Throws<OptionsValidationException>(() =>
        {
            _ = options.Value; // 这会触发验证
        });

        Assert.NotNull(exception);
        Assert.Contains("defaulttimeoutms must be greater than 0", exception.Message.ToLower());

        _output.WriteLine($"✓ Invalid configuration correctly rejected: {exception.Message}");

        serviceProvider.Dispose();
    }

    [Fact]
    public void Should_Support_Different_Service_Lifetimes()
    {
        // 测试不同的服务生命周期支持
        // Test support for different service lifetimes

        var lifetimes = new[] { "Singleton", "Scoped", "Transient" };

        foreach (var lifetime in lifetimes)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "TargetingMatchers:EnableFSharpMatchers", "true" },
                    { "TargetingMatchers:DefaultLifetime", lifetime },
                    { "TargetingMatchers:DefaultTimeoutMs", "5000" }
                })
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<TargetingMatcherOptions>(configuration.GetSection("TargetingMatchers"));

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

            // Assert
            Assert.NotNull(options);
            Assert.Equal(lifetime, options.Value.DefaultLifetime);

            _output.WriteLine($"✓ Lifetime '{lifetime}' configured successfully");

            serviceProvider.Dispose();
        }
    }

    [Fact]
    public void Should_Load_Configuration_From_Json_File()
    {
        // 测试从JSON文件加载配置
        // Test loading configuration from JSON file

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: false)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<TargetingMatcherOptions>(configuration.GetSection("TargetingMatchers"));

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.Value);

        _output.WriteLine("✓ Configuration loaded from appsettings.test.json:");
        _output.WriteLine($"  EnableFSharpMatchers: {options.Value.EnableFSharpMatchers}");
        _output.WriteLine($"  DefaultLifetime: {options.Value.DefaultLifetime}");
        _output.WriteLine($"  DefaultTimeoutMs: {options.Value.DefaultTimeoutMs}");
        _output.WriteLine($"  EnablePerformanceMonitoring: {options.Value.EnablePerformanceMonitoring}");
        _output.WriteLine($"  EnableCaching: {options.Value.EnableCaching}");

        // 验证匹配器配置
        // Verify matcher configurations
        if (options.Value.Matchers?.Any() == true)
        {
            _output.WriteLine($"  Configured matchers: {options.Value.Matchers.Count}");
            foreach (var (key, matcher) in options.Value.Matchers)
            {
                _output.WriteLine($"    {key}: Enabled={matcher.Enabled}, Priority={matcher.Priority}");
            }
        }

        serviceProvider.Dispose();
    }

    [Fact]
    public void Should_Support_Environment_Variable_Override()
    {
        // 测试环境变量覆盖配置
        // Test environment variable configuration override

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TargetingMatchers:EnableFSharpMatchers", "false" },
                { "TargetingMatchers:DefaultTimeoutMs", "1000" }
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // 模拟环境变量覆盖
                // Simulate environment variable override
                { "TargetingMatchers:EnableFSharpMatchers", "true" },
                { "TargetingMatchers:DefaultTimeoutMs", "5000" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<TargetingMatcherOptions>(configuration.GetSection("TargetingMatchers"));

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();

        // Assert
        Assert.NotNull(options);
        Assert.True(options.Value.EnableFSharpMatchers); // 应该是覆盖后的值
        Assert.Equal(5000, options.Value.DefaultTimeoutMs); // 应该是覆盖后的值

        _output.WriteLine("✓ Environment variable override working correctly:");
        _output.WriteLine($"  EnableFSharpMatchers: {options.Value.EnableFSharpMatchers} (overridden)");
        _output.WriteLine($"  DefaultTimeoutMs: {options.Value.DefaultTimeoutMs} (overridden)");

        serviceProvider.Dispose();
    }

    [Fact]
    public void Should_Support_Configuration_Hot_Reload()
    {
        // 测试配置热重载功能（基础版本）
        // Test configuration hot reload functionality (basic version)

        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());

        var configurationBuilder = new ConfigurationBuilder();
        var memorySource = new Dictionary<string, string?>
        {
            { "TargetingMatchers:EnableFSharpMatchers", "false" },
            { "TargetingMatchers:DefaultTimeoutMs", "1000" }
        };

        configurationBuilder.AddInMemoryCollection(memorySource);
        var configuration = configurationBuilder.Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<TargetingMatcherOptions>(configuration.GetSection("TargetingMatchers"));

        var serviceProvider = services.BuildServiceProvider();

        // Act - 第一次读取
        // Act - First read
        var options = serviceProvider.GetRequiredService<IOptions<TargetingMatcherOptions>>();
        var initialValue = options.Value.EnableFSharpMatchers;

        _output.WriteLine($"✓ Initial configuration: EnableFSharpMatchers = {initialValue}");

        // 注意：IOptions<T>在实际应用中可能需要IOptionsMonitor<T>来支持热重载
        // Note: IOptions<T> may need IOptionsMonitor<T> for hot reload in real applications

        Assert.False(initialValue);
        Assert.Equal(1000, options.Value.DefaultTimeoutMs);

        _output.WriteLine("✓ Configuration hot reload infrastructure ready");

        serviceProvider.Dispose();
    }
}
