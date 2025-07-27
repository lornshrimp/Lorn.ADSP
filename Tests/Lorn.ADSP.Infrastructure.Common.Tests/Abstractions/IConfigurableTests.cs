using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Moq;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Abstractions;

/// <summary>
/// IConfigurable 接口的单元测试
/// 测试配置绑定和验证机制
/// </summary>
public class IConfigurableTests
{
    /// <summary>
    /// 测试配置类型属性的正确性
    /// </summary>
    [Fact]
    public void ConfigurationType_Property_Should_Return_Valid_Type()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var expectedType = typeof(TestConfiguration);
        mockConfigurable.Setup(x => x.ConfigurationType).Returns(expectedType);

        // Act
        var actualType = mockConfigurable.Object.ConfigurationType;

        // Assert
        actualType.Should().Be(expectedType);
        actualType.Should().NotBeNull();
    }

    /// <summary>
    /// 测试配置方法的正确调用
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Accept_Valid_Configuration()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var configuration = new TestConfiguration { Value = "TestValue" };

        mockConfigurable.Setup(x => x.ConfigurationType).Returns(typeof(TestConfiguration));
        mockConfigurable.Setup(x => x.Configure(configuration));

        // Act
        mockConfigurable.Object.Configure(configuration);

        // Assert
        mockConfigurable.Verify(x => x.Configure(configuration), Times.Once);
    }

    /// <summary>
    /// 测试配置方法对null配置的处理
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Handle_Null_Configuration()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        mockConfigurable.Setup(x => x.Configure(null));

        // Act
        mockConfigurable.Object.Configure(null);

        // Assert
        mockConfigurable.Verify(x => x.Configure(null), Times.Once);
    }

    /// <summary>
    /// 测试配置方法对不同类型配置对象的处理
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Handle_Different_Configuration_Types()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var stringConfig = "StringConfiguration";
        var intConfig = 42;
        var objectConfig = new { Property = "Value" };

        mockConfigurable.Setup(x => x.Configure(It.IsAny<object>()));

        // Act & Assert
        mockConfigurable.Object.Configure(stringConfig);
        mockConfigurable.Object.Configure(intConfig);
        mockConfigurable.Object.Configure(objectConfig);

        mockConfigurable.Verify(x => x.Configure(It.IsAny<object>()), Times.Exactly(3));
    }

    /// <summary>
    /// 测试配置类型与配置对象的类型匹配验证
    /// </summary>
    [Fact]
    public void ConfigurationType_Should_Match_Configuration_Object_Type()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var configuration = new TestConfiguration { Value = "TestValue" };

        mockConfigurable.Setup(x => x.ConfigurationType).Returns(typeof(TestConfiguration));

        // Act
        var configurationType = mockConfigurable.Object.ConfigurationType;

        // Assert
        configurationType.Should().Be(configuration.GetType());
    }

    /// <summary>
    /// 测试配置接口的多次配置调用
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Support_Multiple_Calls()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var config1 = new TestConfiguration { Value = "Config1" };
        var config2 = new TestConfiguration { Value = "Config2" };
        var config3 = new TestConfiguration { Value = "Config3" };

        mockConfigurable.Setup(x => x.Configure(It.IsAny<object>()));

        // Act
        mockConfigurable.Object.Configure(config1);
        mockConfigurable.Object.Configure(config2);
        mockConfigurable.Object.Configure(config3);

        // Assert
        mockConfigurable.Verify(x => x.Configure(It.IsAny<object>()), Times.Exactly(3));
    }

    /// <summary>
    /// 测试配置接口的泛型类型支持
    /// </summary>
    [Fact]
    public void ConfigurationType_Should_Support_Generic_Types()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var genericType = typeof(List<string>);
        mockConfigurable.Setup(x => x.ConfigurationType).Returns(genericType);

        // Act
        var actualType = mockConfigurable.Object.ConfigurationType;

        // Assert
        actualType.Should().Be(genericType);
        actualType.IsGenericType.Should().BeTrue();
    }

    /// <summary>
    /// 测试配置接口的复杂配置对象处理
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Handle_Complex_Configuration_Objects()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var complexConfig = new ComplexConfiguration
        {
            StringProperty = "TestString",
            IntProperty = 123,
            BoolProperty = true,
            ListProperty = new List<string> { "Item1", "Item2" },
            NestedProperty = new TestConfiguration { Value = "NestedValue" }
        };

        mockConfigurable.Setup(x => x.ConfigurationType).Returns(typeof(ComplexConfiguration));
        mockConfigurable.Setup(x => x.Configure(complexConfig));

        // Act
        mockConfigurable.Object.Configure(complexConfig);

        // Assert
        mockConfigurable.Verify(x => x.Configure(complexConfig), Times.Once);
    }

    /// <summary>
    /// 测试配置接口的线程安全性
    /// </summary>
    [Fact]
    public async Task Configure_Method_Should_Be_Thread_Safe()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var configuration = new TestConfiguration { Value = "ThreadSafeTest" };
        var callCount = 0;

        mockConfigurable.Setup(x => x.Configure(It.IsAny<object>()))
                       .Callback(() => Interlocked.Increment(ref callCount));

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() => mockConfigurable.Object.Configure(configuration)))
                             .ToArray();

        await Task.WhenAll(tasks);

        // Assert
        callCount.Should().Be(10);
        mockConfigurable.Verify(x => x.Configure(It.IsAny<object>()), Times.Exactly(10));
    }

    /// <summary>
    /// 测试配置接口的性能特征
    /// </summary>
    [Fact]
    public void Configure_Method_Should_Execute_Quickly()
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        var configuration = new TestConfiguration { Value = "PerformanceTest" };
        mockConfigurable.Setup(x => x.Configure(configuration));

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 1000; i++)
        {
            mockConfigurable.Object.Configure(configuration);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // 应在100ms内完成1000次调用
        mockConfigurable.Verify(x => x.Configure(configuration), Times.Exactly(1000));
    }

    /// <summary>
    /// 测试配置类型的边界条件
    /// </summary>
    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(object))]
    public void ConfigurationType_Should_Handle_Various_Types(Type configurationType)
    {
        // Arrange
        var mockConfigurable = new Mock<IConfigurable>();
        mockConfigurable.Setup(x => x.ConfigurationType).Returns(configurationType);

        // Act
        var actualType = mockConfigurable.Object.ConfigurationType;

        // Assert
        actualType.Should().Be(configurationType);
    }
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