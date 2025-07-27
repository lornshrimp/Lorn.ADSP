using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Base;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Base;

/// <summary>
/// ConfigurableComponentBase 基础抽象类的单元测试
/// 确保抽象类提供的默认实现正确
/// </summary>
public class ConfigurableComponentBaseTests
{
    /// <summary>
    /// 测试可配置组件基类的配置类型属性
    /// </summary>
    [Fact]
    public void ConfigurationType_Property_Should_Return_Correct_Type()
    {
        // Arrange
        var component = new TestConfigurableComponent();

        // Act
        var configurationType = component.ConfigurationType;

        // Assert
        configurationType.Should().Be(typeof(TestConfiguration));
    }

    /// <summary>
    /// 测试可配置组件基类的配置方法正确调用
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Call_OnConfigurationChanged()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var configuration = new TestConfiguration { Value = "TestValue" };

        // Act
        component.Configure(configuration);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(1);
        component.LastConfiguration.Should().Be(configuration);
    }

    /// <summary>
    /// 测试可配置组件基类的配置类型验证
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Validate_Configuration_Type()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var invalidConfiguration = "InvalidConfiguration"; // 错误的配置类型

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => component.Configure(invalidConfiguration));

        exception.Message.Should().Contain("Invalid configuration type");
        exception.Message.Should().Contain("Expected: TestConfiguration");
        exception.Message.Should().Contain("Actual: String");
    }

    /// <summary>
    /// 测试可配置组件基类对null配置的处理
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Handle_Null_Configuration()
    {
        // Arrange
        var component = new TestConfigurableComponent();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => component.Configure(null));

        exception.Message.Should().Contain("Invalid configuration type");
        exception.Message.Should().Contain("Expected: TestConfiguration");
    }

    /// <summary>
    /// 测试可配置组件基类的多次配置调用
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Support_Multiple_Calls()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var config1 = new TestConfiguration { Value = "Config1" };
        var config2 = new TestConfiguration { Value = "Config2" };
        var config3 = new TestConfiguration { Value = "Config3" };

        // Act
        component.Configure(config1);
        component.Configure(config2);
        component.Configure(config3);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(3);
        component.LastConfiguration.Should().Be(config3);
    }

    /// <summary>
    /// 测试可配置组件基类的配置变更通知
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Notify_Configuration_Changes()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var configuration = new TestConfiguration { Value = "NotificationTest" };

        // Act
        component.Configure(configuration);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(1);
        component.LastConfiguration.Should().NotBeNull();
        ((TestConfiguration)component.LastConfiguration!).Value.Should().Be("NotificationTest");
    }

    /// <summary>
    /// 测试可配置组件基类的配置类型继承支持
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Support_Derived_Configuration_Types()
    {
        // Arrange
        var component = new DerivedConfigurableComponent();
        var configuration = new DerivedTestConfiguration
        {
            Value = "BaseValue",
            AdditionalValue = "DerivedValue"
        };

        // Act
        component.Configure(configuration);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(1);
        component.LastConfiguration.Should().Be(configuration);
    }

    /// <summary>
    /// 测试可配置组件基类的泛型配置类型支持
    /// </summary>
    [Fact]
    public void ConfigurationType_Should_Support_Generic_Types()
    {
        // Arrange
        var component = new GenericConfigurableComponent();

        // Act
        var configurationType = component.ConfigurationType;

        // Assert
        configurationType.Should().Be(typeof(List<string>));
        configurationType.IsGenericType.Should().BeTrue();
    }

    /// <summary>
    /// 测试可配置组件基类的复杂配置对象处理
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Handle_Complex_Configuration()
    {
        // Arrange
        var component = new ComplexConfigurableComponent();
        var configuration = new ComplexConfiguration
        {
            StringProperty = "TestString",
            IntProperty = 123,
            BoolProperty = true,
            ListProperty = new List<string> { "Item1", "Item2" },
            NestedProperty = new TestConfiguration { Value = "NestedValue" }
        };

        // Act
        component.Configure(configuration);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(1);
        component.LastConfiguration.Should().Be(configuration);
    }

    /// <summary>
    /// 测试可配置组件基类的线程安全性
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Be_Thread_Safe()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var configuration = new TestConfiguration { Value = "ThreadSafeTest" };

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() => component.Configure(configuration)))
                             .ToArray();

        Task.WaitAll(tasks);

        // Assert
        component.ConfigurationChangedCallCount.Should().Be(10);
    }

    /// <summary>
    /// 测试可配置组件基类的性能特征
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Execute_Quickly()
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var configuration = new TestConfiguration { Value = "PerformanceTest" };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 1000; i++)
        {
            component.Configure(configuration);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // 应在100ms内完成1000次配置
        component.ConfigurationChangedCallCount.Should().Be(1000);
    }

    /// <summary>
    /// 测试可配置组件基类的配置验证边界条件
    /// </summary>
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(DateTime))]
    public void Configure_Method_Should_Reject_Wrong_Types(Type wrongType)
    {
        // Arrange
        var component = new TestConfigurableComponent();
        var wrongConfiguration = Activator.CreateInstance(wrongType);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => component.Configure(wrongConfiguration));

        exception.Message.Should().Contain("Invalid configuration type");
    }

    /// <summary>
    /// 测试可配置组件基类的抽象方法实现要求
    /// </summary>
    [Fact]
    public void ConfigurableComponentBase_Should_Require_Abstract_Method_Implementation()
    {
        // Arrange & Act
        var component = new TestConfigurableComponent();

        // Assert
        component.Should().NotBeNull();
        component.ConfigurationType.Should().NotBeNull();

        // 验证抽象方法必须被实现
        var configurationType = component.GetType().GetProperty(nameof(component.ConfigurationType));
        configurationType.Should().NotBeNull();
        configurationType!.GetGetMethod()!.IsAbstract.Should().BeFalse();
    }
}

/// <summary>
/// 测试用的可配置组件实现
/// </summary>
public class TestConfigurableComponent : ConfigurableComponentBase
{
    public override Type ConfigurationType => typeof(TestConfiguration);

    public int ConfigurationChangedCallCount { get; private set; }
    public object? LastConfiguration { get; private set; }

    protected override void OnConfigurationChanged(object configuration)
    {
        ConfigurationChangedCallCount++;
        LastConfiguration = configuration;
    }
}

/// <summary>
/// 测试用的派生可配置组件
/// </summary>
public class DerivedConfigurableComponent : ConfigurableComponentBase
{
    public override Type ConfigurationType => typeof(DerivedTestConfiguration);

    public int ConfigurationChangedCallCount { get; private set; }
    public object? LastConfiguration { get; private set; }

    protected override void OnConfigurationChanged(object configuration)
    {
        ConfigurationChangedCallCount++;
        LastConfiguration = configuration;
    }
}

/// <summary>
/// 测试用的泛型可配置组件
/// </summary>
public class GenericConfigurableComponent : ConfigurableComponentBase
{
    public override Type ConfigurationType => typeof(List<string>);

    public int ConfigurationChangedCallCount { get; private set; }
    public object? LastConfiguration { get; private set; }

    protected override void OnConfigurationChanged(object configuration)
    {
        ConfigurationChangedCallCount++;
        LastConfiguration = configuration;
    }
}

/// <summary>
/// 测试用的复杂可配置组件
/// </summary>
public class ComplexConfigurableComponent : ConfigurableComponentBase
{
    public override Type ConfigurationType => typeof(ComplexConfiguration);

    public int ConfigurationChangedCallCount { get; private set; }
    public object? LastConfiguration { get; private set; }

    protected override void OnConfigurationChanged(object configuration)
    {
        ConfigurationChangedCallCount++;
        LastConfiguration = configuration;
    }
}

/// <summary>
/// 测试用的派生配置类
/// </summary>
public class DerivedTestConfiguration : TestConfiguration
{
    public string AdditionalValue { get; set; } = "";
}

/// <summary>
/// 测试用的简单配置类
/// </summary>
public class TestConfiguration
{
    public string Value { get; set; } = "";
}

/// <summary>
/// 测试用的复杂配置类
/// </summary>
public class ComplexConfiguration
{
    public string StringProperty { get; set; } = "";
    public int IntProperty { get; set; }
    public bool BoolProperty { get; set; }
    public List<string> ListProperty { get; set; } = new();
    public TestConfiguration? NestedProperty { get; set; }
}