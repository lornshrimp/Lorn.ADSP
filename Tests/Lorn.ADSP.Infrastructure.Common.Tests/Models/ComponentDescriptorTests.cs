using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Models;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Models;

/// <summary>
/// ComponentDescriptor 模型的单元测试
/// 验证组件描述符的正确性和类型转换
/// </summary>
public class ComponentDescriptorTests
{
    /// <summary>
    /// 测试组件描述符的默认构造函数
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Should_Initialize_With_Default_Values()
    {
        // Act
        var descriptor = new ComponentDescriptor();

        // Assert
        descriptor.ImplementationType.Should().BeNull();
        descriptor.ServiceType.Should().BeNull();
        descriptor.Name.Should().Be("");
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
        descriptor.ConfigurationPath.Should().BeNull();
        descriptor.ConfigurationType.Should().BeNull();
        descriptor.IsEnabled.Should().BeTrue();
        descriptor.Priority.Should().Be(0);
        descriptor.Metadata.Should().NotBeNull();
        descriptor.Metadata.Should().BeEmpty();
    }

    /// <summary>
    /// 测试组件描述符的实现类型设置
    /// </summary>
    [Fact]
    public void ComponentDescriptor_ImplementationType_Should_Be_Settable()
    {
        // Arrange
        var descriptor = new ComponentDescriptor();
        var implementationType = typeof(TestImplementation);

        // Act
        descriptor.ImplementationType = implementationType;

        // Assert
        descriptor.ImplementationType.Should().Be(implementationType);
    }

    /// <summary>
    /// 测试组件描述符的服务类型设置
    /// </summary>
    [Fact]
    public void ComponentDescriptor_ServiceType_Should_Be_Settable()
    {
        // Arrange
        var descriptor = new ComponentDescriptor();
        var serviceType = typeof(ITestService);

        // Act
        descriptor.ServiceType = serviceType;

        // Assert
        descriptor.ServiceType.Should().Be(serviceType);
    }

    /// <summary>
    /// 测试组件描述符的名称设置
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("TestComponent")]
    [InlineData("Component_With_Underscores")]
    [InlineData("ComponentWithNumbers123")]
    [InlineData("中文组件名称")]
    public void ComponentDescriptor_Name_Should_Accept_Various_Strings(string name)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.Name = name;

        // Assert
        descriptor.Name.Should().Be(name);
    }

    /// <summary>
    /// 测试组件描述符的生命周期枚举值
    /// </summary>
    [Theory]
    [InlineData(ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton)]
    public void ComponentDescriptor_Lifetime_Should_Accept_All_ServiceLifetime_Values(ServiceLifetime lifetime)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.Lifetime = lifetime;

        // Assert
        descriptor.Lifetime.Should().Be(lifetime);
    }

    /// <summary>
    /// 测试组件描述符的配置路径设置
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Configuration:Section")]
    [InlineData("App:Settings:Component")]
    [InlineData("Complex:Nested:Configuration:Path")]
    public void ComponentDescriptor_ConfigurationPath_Should_Accept_Various_Paths(string? configurationPath)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.ConfigurationPath = configurationPath;

        // Assert
        descriptor.ConfigurationPath.Should().Be(configurationPath);
    }

    /// <summary>
    /// 测试组件描述符的配置类型设置
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(TestConfiguration))]
    [InlineData(typeof(List<string>))]
    public void ComponentDescriptor_ConfigurationType_Should_Accept_Various_Types(Type? configurationType)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.ConfigurationType = configurationType;

        // Assert
        descriptor.ConfigurationType.Should().Be(configurationType);
    }

    /// <summary>
    /// 测试组件描述符的启用状态设置
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComponentDescriptor_IsEnabled_Should_Accept_Boolean_Values(bool isEnabled)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.IsEnabled = isEnabled;

        // Assert
        descriptor.IsEnabled.Should().Be(isEnabled);
    }

    /// <summary>
    /// 测试组件描述符的优先级设置
    /// </summary>
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void ComponentDescriptor_Priority_Should_Accept_Integer_Values(int priority)
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.Priority = priority;

        // Assert
        descriptor.Priority.Should().Be(priority);
    }

    /// <summary>
    /// 测试组件描述符的元数据字典
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Metadata_Should_Support_Dictionary_Operations()
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.Metadata["Key1"] = "Value1";
        descriptor.Metadata["Key2"] = 42;
        descriptor.Metadata["Key3"] = true;
        descriptor.Metadata["Key4"] = DateTime.Now;

        // Assert
        descriptor.Metadata.Should().HaveCount(4);
        descriptor.Metadata["Key1"].Should().Be("Value1");
        descriptor.Metadata["Key2"].Should().Be(42);
        descriptor.Metadata["Key3"].Should().Be(true);
        descriptor.Metadata["Key4"].Should().BeOfType<DateTime>();
    }

    /// <summary>
    /// 测试组件描述符的完整配置场景
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Should_Support_Complete_Configuration()
    {
        // Arrange
        var descriptor = new ComponentDescriptor();

        // Act
        descriptor.ImplementationType = typeof(TestImplementation);
        descriptor.ServiceType = typeof(ITestService);
        descriptor.Name = "TestComponent";
        descriptor.Lifetime = ServiceLifetime.Singleton;
        descriptor.ConfigurationPath = "Components:TestComponent";
        descriptor.ConfigurationType = typeof(TestConfiguration);
        descriptor.IsEnabled = true;
        descriptor.Priority = 100;
        descriptor.Metadata["Author"] = "Test Author";
        descriptor.Metadata["Version"] = "1.0.0";
        descriptor.Metadata["Tags"] = new[] { "Service", "Component" };

        // Assert
        descriptor.ImplementationType.Should().Be(typeof(TestImplementation));
        descriptor.ServiceType.Should().Be(typeof(ITestService));
        descriptor.Name.Should().Be("TestComponent");
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ConfigurationPath.Should().Be("Components:TestComponent");
        descriptor.ConfigurationType.Should().Be(typeof(TestConfiguration));
        descriptor.IsEnabled.Should().BeTrue();
        descriptor.Priority.Should().Be(100);
        descriptor.Metadata.Should().HaveCount(3);
        descriptor.Metadata["Author"].Should().Be("Test Author");
        descriptor.Metadata["Version"].Should().Be("1.0.0");
        descriptor.Metadata["Tags"].Should().BeEquivalentTo(new[] { "Service", "Component" });
    }

    /// <summary>
    /// 测试组件描述符的泛型类型支持
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Should_Support_Generic_Types()
    {
        // Arrange
        var descriptor = new ComponentDescriptor();
        var genericImplementationType = typeof(GenericImplementation<string>);
        var genericServiceType = typeof(IGenericService<string>);
        var genericConfigurationType = typeof(List<TestConfiguration>);

        // Act
        descriptor.ImplementationType = genericImplementationType;
        descriptor.ServiceType = genericServiceType;
        descriptor.ConfigurationType = genericConfigurationType;

        // Assert
        descriptor.ImplementationType.Should().Be(genericImplementationType);
        descriptor.ServiceType.Should().Be(genericServiceType);
        descriptor.ConfigurationType.Should().Be(genericConfigurationType);
        descriptor.ImplementationType.IsGenericType.Should().BeTrue();
        descriptor.ServiceType.IsGenericType.Should().BeTrue();
        descriptor.ConfigurationType!.IsGenericType.Should().BeTrue();
    }

    /// <summary>
    /// 测试组件描述符的复制和比较
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Should_Support_Independent_Instances()
    {
        // Arrange
        var descriptor1 = new ComponentDescriptor
        {
            ImplementationType = typeof(TestImplementation),
            ServiceType = typeof(ITestService),
            Name = "Component1",
            Lifetime = ServiceLifetime.Singleton,
            Priority = 100
        };

        var descriptor2 = new ComponentDescriptor
        {
            ImplementationType = typeof(TestImplementation),
            ServiceType = typeof(ITestService),
            Name = "Component2",
            Lifetime = ServiceLifetime.Transient,
            Priority = 200
        };

        // Act & Assert
        descriptor1.Name.Should().Be("Component1");
        descriptor2.Name.Should().Be("Component2");
        descriptor1.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor2.Lifetime.Should().Be(ServiceLifetime.Transient);
        descriptor1.Priority.Should().Be(100);
        descriptor2.Priority.Should().Be(200);

        // 验证元数据字典是独立的
        descriptor1.Metadata["Test"] = "Value1";
        descriptor2.Metadata["Test"] = "Value2";
        descriptor1.Metadata["Test"].Should().Be("Value1");
        descriptor2.Metadata["Test"].Should().Be("Value2");
    }

    /// <summary>
    /// 测试组件描述符的线程安全性
    /// </summary>
    [Fact]
    public void ComponentDescriptor_Should_Be_Thread_Safe_For_Reading()
    {
        // Arrange
        var descriptor = new ComponentDescriptor
        {
            ImplementationType = typeof(TestImplementation),
            ServiceType = typeof(ITestService),
            Name = "ThreadSafeComponent",
            Lifetime = ServiceLifetime.Singleton,
            IsEnabled = true,
            Priority = 50
        };

        descriptor.Metadata["Key1"] = "Value1";
        descriptor.Metadata["Key2"] = "Value2";

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var name = descriptor.Name;
                                 var lifetime = descriptor.Lifetime;
                                 var isEnabled = descriptor.IsEnabled;
                                 var priority = descriptor.Priority;
                                 var metadataCount = descriptor.Metadata.Count;

                                 return new
                                 {
                                     Name = name,
                                     Lifetime = lifetime,
                                     IsEnabled = isEnabled,
                                     Priority = priority,
                                     MetadataCount = metadataCount
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.Name.Should().Be("ThreadSafeComponent");
            result.Lifetime.Should().Be(ServiceLifetime.Singleton);
            result.IsEnabled.Should().BeTrue();
            result.Priority.Should().Be(50);
            result.MetadataCount.Should().Be(2);
        });
    }
}

/// <summary>
/// ServiceLifetime 枚举的单元测试
/// 验证枚举值的正确性和类型转换
/// </summary>
public class ServiceLifetimeTests
{
    /// <summary>
    /// 测试ServiceLifetime枚举的所有值
    /// </summary>
    [Fact]
    public void ServiceLifetime_Should_Have_All_Expected_Values()
    {
        // Act & Assert
        var values = Enum.GetValues<ServiceLifetime>();

        values.Should().HaveCount(3);
        values.Should().Contain(ServiceLifetime.Transient);
        values.Should().Contain(ServiceLifetime.Scoped);
        values.Should().Contain(ServiceLifetime.Singleton);
    }

    /// <summary>
    /// 测试ServiceLifetime枚举的数值
    /// </summary>
    [Theory]
    [InlineData(ServiceLifetime.Transient, 0)]
    [InlineData(ServiceLifetime.Scoped, 1)]
    [InlineData(ServiceLifetime.Singleton, 2)]
    public void ServiceLifetime_Should_Have_Correct_Numeric_Values(ServiceLifetime lifetime, int expectedValue)
    {
        // Act
        var actualValue = (int)lifetime;

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    /// <summary>
    /// 测试ServiceLifetime枚举的字符串转换
    /// </summary>
    [Theory]
    [InlineData(ServiceLifetime.Transient, "Transient")]
    [InlineData(ServiceLifetime.Scoped, "Scoped")]
    [InlineData(ServiceLifetime.Singleton, "Singleton")]
    public void ServiceLifetime_Should_Convert_To_String_Correctly(ServiceLifetime lifetime, string expectedString)
    {
        // Act
        var actualString = lifetime.ToString();

        // Assert
        actualString.Should().Be(expectedString);
    }

    /// <summary>
    /// 测试ServiceLifetime枚举的解析
    /// </summary>
    [Theory]
    [InlineData("Transient", ServiceLifetime.Transient)]
    [InlineData("Scoped", ServiceLifetime.Scoped)]
    [InlineData("Singleton", ServiceLifetime.Singleton)]
    public void ServiceLifetime_Should_Parse_From_String_Correctly(string lifetimeString, ServiceLifetime expectedLifetime)
    {
        // Act
        var actualLifetime = Enum.Parse<ServiceLifetime>(lifetimeString);

        // Assert
        actualLifetime.Should().Be(expectedLifetime);
    }

    /// <summary>
    /// 测试ServiceLifetime枚举的TryParse
    /// </summary>
    [Theory]
    [InlineData("Transient", true, ServiceLifetime.Transient)]
    [InlineData("Scoped", true, ServiceLifetime.Scoped)]
    [InlineData("Singleton", true, ServiceLifetime.Singleton)]
    [InlineData("Invalid", false, default(ServiceLifetime))]
    [InlineData("", false, default(ServiceLifetime))]
    public void ServiceLifetime_TryParse_Should_Work_Correctly(string lifetimeString, bool expectedSuccess, ServiceLifetime expectedLifetime)
    {
        // Act
        var success = Enum.TryParse<ServiceLifetime>(lifetimeString, out var actualLifetime);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            actualLifetime.Should().Be(expectedLifetime);
        }
    }
}

// 测试用的接口和类
public interface ITestService { }
public interface IGenericService<T> { }
public class TestImplementation : ITestService { }
public class GenericImplementation<T> : IGenericService<T> { }

/// <summary>
/// 测试用的配置类
/// </summary>
public class TestConfiguration
{
    public string Value { get; set; } = "";
}