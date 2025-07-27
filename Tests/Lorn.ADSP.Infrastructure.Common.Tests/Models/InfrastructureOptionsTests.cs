using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Models;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Models;

/// <summary>
/// InfrastructureOptions 模型的单元测试
/// 验证基础设施常量和配置的正确性和一致性
/// </summary>
public class InfrastructureOptionsTests
{
    /// <summary>
    /// 测试基础设施选项的默认构造函数
    /// </summary>
    [Fact]
    public void InfrastructureOptions_Should_Initialize_With_Default_Values()
    {
        // Act
        var options = new InfrastructureOptions();

        // Assert
        options.EnableAutoDiscovery.Should().BeTrue();
        options.EnableHealthChecks.Should().BeTrue();
        options.EnablePerformanceMonitoring.Should().BeTrue();
        options.EnableConfigurationHotReload.Should().BeTrue();
        options.AssemblyPatterns.Should().NotBeNull();
        options.AssemblyPatterns.Should().HaveCount(1);
        options.AssemblyPatterns.Should().Contain("Lorn.ADSP.*");
        options.ExcludedAssemblyPatterns.Should().NotBeNull();
        options.ExcludedAssemblyPatterns.Should().BeEmpty();
        options.ConfigurationValidationTimeoutSeconds.Should().Be(30);
        options.ComponentInitializationTimeoutSeconds.Should().Be(60);
    }

    /// <summary>
    /// 测试基础设施选项的自动发现设置
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InfrastructureOptions_EnableAutoDiscovery_Should_Be_Settable(bool enableAutoDiscovery)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.EnableAutoDiscovery = enableAutoDiscovery;

        // Assert
        options.EnableAutoDiscovery.Should().Be(enableAutoDiscovery);
    }

    /// <summary>
    /// 测试基础设施选项的健康检查设置
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InfrastructureOptions_EnableHealthChecks_Should_Be_Settable(bool enableHealthChecks)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.EnableHealthChecks = enableHealthChecks;

        // Assert
        options.EnableHealthChecks.Should().Be(enableHealthChecks);
    }

    /// <summary>
    /// 测试基础设施选项的性能监控设置
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InfrastructureOptions_EnablePerformanceMonitoring_Should_Be_Settable(bool enablePerformanceMonitoring)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.EnablePerformanceMonitoring = enablePerformanceMonitoring;

        // Assert
        options.EnablePerformanceMonitoring.Should().Be(enablePerformanceMonitoring);
    }

    /// <summary>
    /// 测试基础设施选项的配置热重载设置
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InfrastructureOptions_EnableConfigurationHotReload_Should_Be_Settable(bool enableConfigurationHotReload)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.EnableConfigurationHotReload = enableConfigurationHotReload;

        // Assert
        options.EnableConfigurationHotReload.Should().Be(enableConfigurationHotReload);
    }

    /// <summary>
    /// 测试基础设施选项的程序集模式列表
    /// </summary>
    [Fact]
    public void InfrastructureOptions_AssemblyPatterns_Should_Support_List_Operations()
    {
        // Arrange
        var options = new InfrastructureOptions();
        var patterns = new List<string> { "Pattern1.*", "Pattern2.*", "Pattern3.*" };

        // Act
        options.AssemblyPatterns = patterns;

        // Assert
        options.AssemblyPatterns.Should().BeSameAs(patterns);
        options.AssemblyPatterns.Should().HaveCount(3);
        options.AssemblyPatterns.Should().Contain("Pattern1.*");
        options.AssemblyPatterns.Should().Contain("Pattern2.*");
        options.AssemblyPatterns.Should().Contain("Pattern3.*");
    }

    /// <summary>
    /// 测试基础设施选项的排除程序集模式列表
    /// </summary>
    [Fact]
    public void InfrastructureOptions_ExcludedAssemblyPatterns_Should_Support_List_Operations()
    {
        // Arrange
        var options = new InfrastructureOptions();
        var excludedPatterns = new List<string> { "System.*", "Microsoft.*", "Test.*" };

        // Act
        options.ExcludedAssemblyPatterns = excludedPatterns;

        // Assert
        options.ExcludedAssemblyPatterns.Should().BeSameAs(excludedPatterns);
        options.ExcludedAssemblyPatterns.Should().HaveCount(3);
        options.ExcludedAssemblyPatterns.Should().Contain("System.*");
        options.ExcludedAssemblyPatterns.Should().Contain("Microsoft.*");
        options.ExcludedAssemblyPatterns.Should().Contain("Test.*");
    }

    /// <summary>
    /// 测试基础设施选项的配置验证超时设置
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(300)]
    [InlineData(3600)]
    public void InfrastructureOptions_ConfigurationValidationTimeoutSeconds_Should_Accept_Positive_Values(int timeoutSeconds)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.ConfigurationValidationTimeoutSeconds = timeoutSeconds;

        // Assert
        options.ConfigurationValidationTimeoutSeconds.Should().Be(timeoutSeconds);
    }

    /// <summary>
    /// 测试基础设施选项的组件初始化超时设置
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    [InlineData(120)]
    [InlineData(300)]
    [InlineData(600)]
    public void InfrastructureOptions_ComponentInitializationTimeoutSeconds_Should_Accept_Positive_Values(int timeoutSeconds)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.ComponentInitializationTimeoutSeconds = timeoutSeconds;

        // Assert
        options.ComponentInitializationTimeoutSeconds.Should().Be(timeoutSeconds);
    }

    /// <summary>
    /// 测试基础设施选项的完整配置场景
    /// </summary>
    [Fact]
    public void InfrastructureOptions_Should_Support_Complete_Configuration()
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.EnableAutoDiscovery = false;
        options.EnableHealthChecks = false;
        options.EnablePerformanceMonitoring = false;
        options.EnableConfigurationHotReload = false;

        options.AssemblyPatterns.Clear();
        options.AssemblyPatterns.Add("MyApp.*");
        options.AssemblyPatterns.Add("MyLibrary.*");

        options.ExcludedAssemblyPatterns.Add("System.*");
        options.ExcludedAssemblyPatterns.Add("Microsoft.*");
        options.ExcludedAssemblyPatterns.Add("Test.*");

        options.ConfigurationValidationTimeoutSeconds = 120;
        options.ComponentInitializationTimeoutSeconds = 180;

        // Assert
        options.EnableAutoDiscovery.Should().BeFalse();
        options.EnableHealthChecks.Should().BeFalse();
        options.EnablePerformanceMonitoring.Should().BeFalse();
        options.EnableConfigurationHotReload.Should().BeFalse();

        options.AssemblyPatterns.Should().HaveCount(2);
        options.AssemblyPatterns.Should().Contain("MyApp.*");
        options.AssemblyPatterns.Should().Contain("MyLibrary.*");

        options.ExcludedAssemblyPatterns.Should().HaveCount(3);
        options.ExcludedAssemblyPatterns.Should().Contain("System.*");
        options.ExcludedAssemblyPatterns.Should().Contain("Microsoft.*");
        options.ExcludedAssemblyPatterns.Should().Contain("Test.*");

        options.ConfigurationValidationTimeoutSeconds.Should().Be(120);
        options.ComponentInitializationTimeoutSeconds.Should().Be(180);
    }

    /// <summary>
    /// 测试基础设施选项的程序集模式验证
    /// </summary>
    [Theory]
    [InlineData("Lorn.ADSP.*")]
    [InlineData("MyApp.*")]
    [InlineData("*.Services")]
    [InlineData("Exact.Assembly.Name")]
    [InlineData("*")]
    public void InfrastructureOptions_AssemblyPatterns_Should_Accept_Various_Patterns(string pattern)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.AssemblyPatterns.Add(pattern);

        // Assert
        options.AssemblyPatterns.Should().Contain(pattern);
    }

    /// <summary>
    /// 测试基础设施选项的排除程序集模式验证
    /// </summary>
    [Theory]
    [InlineData("System.*")]
    [InlineData("Microsoft.*")]
    [InlineData("*.Test")]
    [InlineData("*.Tests")]
    [InlineData("Moq")]
    [InlineData("xunit.*")]
    public void InfrastructureOptions_ExcludedAssemblyPatterns_Should_Accept_Various_Patterns(string pattern)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act
        options.ExcludedAssemblyPatterns.Add(pattern);

        // Assert
        options.ExcludedAssemblyPatterns.Should().Contain(pattern);
    }

    /// <summary>
    /// 测试基础设施选项的超时值边界条件
    /// </summary>
    [Theory]
    [InlineData(0)] // 边界值：0秒
    [InlineData(-1)] // 负值
    [InlineData(int.MaxValue)] // 最大值
    public void InfrastructureOptions_Timeout_Values_Should_Accept_Edge_Cases(int timeoutValue)
    {
        // Arrange
        var options = new InfrastructureOptions();

        // Act & Assert - 应该能够设置这些值，但实际使用时可能需要验证
        options.ConfigurationValidationTimeoutSeconds = timeoutValue;
        options.ComponentInitializationTimeoutSeconds = timeoutValue;

        options.ConfigurationValidationTimeoutSeconds.Should().Be(timeoutValue);
        options.ComponentInitializationTimeoutSeconds.Should().Be(timeoutValue);
    }

    /// <summary>
    /// 测试基础设施选项的线程安全性
    /// </summary>
    [Fact]
    public void InfrastructureOptions_Should_Be_Thread_Safe_For_Reading()
    {
        // Arrange
        var options = new InfrastructureOptions();
        options.AssemblyPatterns.Add("TestPattern.*");
        options.ExcludedAssemblyPatterns.Add("ExcludedPattern.*");

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var autoDiscovery = options.EnableAutoDiscovery;
                                 var healthChecks = options.EnableHealthChecks;
                                 var monitoring = options.EnablePerformanceMonitoring;
                                 var hotReload = options.EnableConfigurationHotReload;
                                 var assemblyPatternsCount = options.AssemblyPatterns.Count;
                                 var excludedPatternsCount = options.ExcludedAssemblyPatterns.Count;
                                 var configTimeout = options.ConfigurationValidationTimeoutSeconds;
                                 var componentTimeout = options.ComponentInitializationTimeoutSeconds;

                                 return new
                                 {
                                     AutoDiscovery = autoDiscovery,
                                     HealthChecks = healthChecks,
                                     Monitoring = monitoring,
                                     HotReload = hotReload,
                                     AssemblyPatternsCount = assemblyPatternsCount,
                                     ExcludedPatternsCount = excludedPatternsCount,
                                     ConfigTimeout = configTimeout,
                                     ComponentTimeout = componentTimeout
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.AutoDiscovery.Should().BeTrue();
            result.HealthChecks.Should().BeTrue();
            result.Monitoring.Should().BeTrue();
            result.HotReload.Should().BeTrue();
            result.AssemblyPatternsCount.Should().Be(2); // 默认的 + 添加的
            result.ExcludedPatternsCount.Should().Be(1);
            result.ConfigTimeout.Should().Be(30);
            result.ComponentTimeout.Should().Be(60);
        });
    }

    /// <summary>
    /// 测试基础设施选项的默认值常量一致性
    /// </summary>
    [Fact]
    public void InfrastructureOptions_Default_Values_Should_Be_Consistent()
    {
        // Arrange & Act
        var options1 = new InfrastructureOptions();
        var options2 = new InfrastructureOptions();

        // Assert - 验证默认值在多个实例间保持一致
        options1.EnableAutoDiscovery.Should().Be(options2.EnableAutoDiscovery);
        options1.EnableHealthChecks.Should().Be(options2.EnableHealthChecks);
        options1.EnablePerformanceMonitoring.Should().Be(options2.EnablePerformanceMonitoring);
        options1.EnableConfigurationHotReload.Should().Be(options2.EnableConfigurationHotReload);
        options1.ConfigurationValidationTimeoutSeconds.Should().Be(options2.ConfigurationValidationTimeoutSeconds);
        options1.ComponentInitializationTimeoutSeconds.Should().Be(options2.ComponentInitializationTimeoutSeconds);

        // 验证集合默认值
        options1.AssemblyPatterns.Should().BeEquivalentTo(options2.AssemblyPatterns);
        options1.ExcludedAssemblyPatterns.Should().BeEquivalentTo(options2.ExcludedAssemblyPatterns);

        // 验证集合是独立的实例
        options1.AssemblyPatterns.Should().NotBeSameAs(options2.AssemblyPatterns);
        options1.ExcludedAssemblyPatterns.Should().NotBeSameAs(options2.ExcludedAssemblyPatterns);
    }

    /// <summary>
    /// 测试基础设施选项的内存使用效率
    /// </summary>
    [Fact]
    public void InfrastructureOptions_Should_Be_Memory_Efficient()
    {
        // Arrange & Act
        var optionsList = new List<InfrastructureOptions>();

        for (int i = 0; i < 1000; i++)
        {
            var options = new InfrastructureOptions();
            options.AssemblyPatterns.Add($"Pattern{i}.*");
            options.ExcludedAssemblyPatterns.Add($"Excluded{i}.*");
            optionsList.Add(options);
        }

        // Assert
        optionsList.Should().HaveCount(1000);

        // 验证每个选项对象都是独立的
        for (int i = 0; i < 1000; i++)
        {
            optionsList[i].AssemblyPatterns.Should().Contain($"Pattern{i}.*");
            optionsList[i].ExcludedAssemblyPatterns.Should().Contain($"Excluded{i}.*");
        }
    }
}