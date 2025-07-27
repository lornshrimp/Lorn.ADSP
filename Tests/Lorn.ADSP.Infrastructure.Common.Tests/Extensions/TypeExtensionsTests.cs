using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.Reflection;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Extensions;

/// <summary>
/// TypeExtensions 扩展方法的单元测试
/// 验证扩展功能的正确性
/// </summary>
public class TypeExtensionsTests
{
    /// <summary>
    /// 测试GetFriendlyName方法对普通类型的处理
    /// </summary>
    [Theory]
    [InlineData(typeof(string), "String")]
    [InlineData(typeof(int), "Int32")]
    [InlineData(typeof(bool), "Boolean")]
    [InlineData(typeof(DateTime), "DateTime")]
    [InlineData(typeof(object), "Object")]
    public void GetFriendlyName_Should_Return_Simple_Name_For_Non_Generic_Types(Type type, string expectedName)
    {
        // Act
        var friendlyName = type.GetFriendlyName();

        // Assert
        friendlyName.Should().Be(expectedName);
    }

    /// <summary>
    /// 测试GetFriendlyName方法对泛型类型的处理
    /// </summary>
    [Theory]
    [InlineData(typeof(List<string>), "List<String>")]
    [InlineData(typeof(Dictionary<string, int>), "Dictionary<String,Int32>")]
    [InlineData(typeof(IEnumerable<bool>), "IEnumerable<Boolean>")]
    [InlineData(typeof(Task<string>), "Task<String>")]
    public void GetFriendlyName_Should_Return_Formatted_Name_For_Generic_Types(Type type, string expectedName)
    {
        // Act
        var friendlyName = type.GetFriendlyName();

        // Assert
        friendlyName.Should().Be(expectedName);
    }

    /// <summary>
    /// 测试GetFriendlyName方法对嵌套泛型类型的处理
    /// </summary>
    [Fact]
    public void GetFriendlyName_Should_Handle_Nested_Generic_Types()
    {
        // Arrange
        var nestedGenericType = typeof(Dictionary<string, List<int>>);

        // Act
        var friendlyName = nestedGenericType.GetFriendlyName();

        // Assert
        friendlyName.Should().Be("Dictionary<String,List<Int32>>");
    }

    /// <summary>
    /// 测试ImplementsInterface方法对接口类型的验证
    /// </summary>
    [Fact]
    public void ImplementsInterface_Should_Return_True_For_Implemented_Interface()
    {
        // Arrange
        var type = typeof(List<string>);
        var interfaceType = typeof(IList<string>);

        // Act
        var implementsInterface = type.ImplementsInterface(interfaceType);

        // Assert
        implementsInterface.Should().BeTrue();
    }

    /// <summary>
    /// 测试ImplementsInterface方法对未实现接口的处理
    /// </summary>
    [Fact]
    public void ImplementsInterface_Should_Return_False_For_Non_Implemented_Interface()
    {
        // Arrange
        var type = typeof(string);
        var interfaceType = typeof(IDisposable);

        // Act
        var implementsInterface = type.ImplementsInterface(interfaceType);

        // Assert
        implementsInterface.Should().BeFalse();
    }

    /// <summary>
    /// 测试ImplementsInterface方法对非接口类型的处理
    /// </summary>
    [Fact]
    public void ImplementsInterface_Should_Return_False_For_Non_Interface_Type()
    {
        // Arrange
        var type = typeof(string);
        var nonInterfaceType = typeof(object);

        // Act
        var implementsInterface = type.ImplementsInterface(nonInterfaceType);

        // Assert
        implementsInterface.Should().BeFalse();
    }

    /// <summary>
    /// 测试ImplementsInterface方法对接口名称的验证
    /// </summary>
    [Theory]
    [InlineData(typeof(List<string>), "IList", true)]
    [InlineData(typeof(List<string>), "ICollection", true)]
    [InlineData(typeof(List<string>), "IEnumerable", true)]
    [InlineData(typeof(string), "IDisposable", false)]
    [InlineData(typeof(MemoryStream), "IDisposable", true)]
    public void ImplementsInterface_Should_Validate_Interface_By_Name(Type type, string interfaceName, bool expectedResult)
    {
        // Act
        var implementsInterface = type.ImplementsInterface(interfaceName);

        // Assert
        implementsInterface.Should().Be(expectedResult);
    }

    /// <summary>
    /// 测试IsConcreteClass方法对具体类的识别
    /// </summary>
    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(List<string>), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(TestConcreteClass), true)]
    public void IsConcreteClass_Should_Return_True_For_Concrete_Classes(Type type, bool expectedResult)
    {
        // Act
        var isConcreteClass = type.IsConcreteClass();

        // Assert
        isConcreteClass.Should().Be(expectedResult);
    }

    /// <summary>
    /// 测试IsConcreteClass方法对抽象类和接口的处理
    /// </summary>
    [Theory]
    [InlineData(typeof(IDisposable), false)] // 接口
    [InlineData(typeof(Stream), false)] // 抽象类
    [InlineData(typeof(List<>), false)] // 泛型定义
    public void IsConcreteClass_Should_Return_False_For_Non_Concrete_Types(Type type, bool expectedResult)
    {
        // Act
        var isConcreteClass = type.IsConcreteClass();

        // Assert
        isConcreteClass.Should().Be(expectedResult);
    }

    /// <summary>
    /// 测试GetAllInterfaces方法获取所有接口
    /// </summary>
    [Fact]
    public void GetAllInterfaces_Should_Return_All_Implemented_Interfaces()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var interfaces = type.GetAllInterfaces().ToList();

        // Assert
        interfaces.Should().NotBeEmpty();
        interfaces.Should().Contain(typeof(IList<string>));
        interfaces.Should().Contain(typeof(ICollection<string>));
        interfaces.Should().Contain(typeof(IEnumerable<string>));
        interfaces.Should().Contain(typeof(System.Collections.IEnumerable));
    }

    /// <summary>
    /// 测试GetAllInterfaces方法对接口继承的处理
    /// </summary>
    [Fact]
    public void GetAllInterfaces_Should_Include_Inherited_Interfaces()
    {
        // Arrange
        var type = typeof(TestImplementation);

        // Act
        var interfaces = type.GetAllInterfaces().ToList();

        // Assert
        interfaces.Should().Contain(typeof(ITestDerived));
        interfaces.Should().Contain(typeof(ITestBase));
    }

    /// <summary>
    /// 测试GetDefaultConstructor方法获取默认构造函数
    /// </summary>
    [Fact]
    public void GetDefaultConstructor_Should_Return_Parameterless_Constructor()
    {
        // Arrange
        var type = typeof(TestClassWithDefaultConstructor);

        // Act
        var constructor = type.GetDefaultConstructor();

        // Assert
        constructor.Should().NotBeNull();
        constructor!.GetParameters().Should().BeEmpty();
    }

    /// <summary>
    /// 测试GetDefaultConstructor方法对无默认构造函数类的处理
    /// </summary>
    [Fact]
    public void GetDefaultConstructor_Should_Return_Null_For_No_Default_Constructor()
    {
        // Arrange
        var type = typeof(TestClassWithoutDefaultConstructor);

        // Act
        var constructor = type.GetDefaultConstructor();

        // Assert
        constructor.Should().BeNull();
    }

    /// <summary>
    /// 测试HasDefaultConstructor方法的正确性
    /// </summary>
    [Theory]
    [InlineData(typeof(TestClassWithDefaultConstructor), true)]
    [InlineData(typeof(TestClassWithoutDefaultConstructor), false)]
    [InlineData(typeof(string), false)] // string没有公共默认构造函数
    [InlineData(typeof(object), true)]
    public void HasDefaultConstructor_Should_Return_Correct_Result(Type type, bool expectedResult)
    {
        // Act
        var hasDefaultConstructor = type.HasDefaultConstructor();

        // Assert
        hasDefaultConstructor.Should().Be(expectedResult);
    }

    /// <summary>
    /// 测试GetPublicConstructors方法获取所有公共构造函数
    /// </summary>
    [Fact]
    public void GetPublicConstructors_Should_Return_All_Public_Constructors()
    {
        // Arrange
        var type = typeof(TestClassWithMultipleConstructors);

        // Act
        var constructors = type.GetPublicConstructors().ToList();

        // Assert
        constructors.Should().HaveCount(3); // 默认构造函数 + 2个参数构造函数
        constructors.Should().Contain(c => c.GetParameters().Length == 0);
        constructors.Should().Contain(c => c.GetParameters().Length == 1);
        constructors.Should().Contain(c => c.GetParameters().Length == 2);
    }

    /// <summary>
    /// 测试扩展方法的性能特征
    /// </summary>
    [Fact]
    public void TypeExtensions_Should_Execute_Quickly()
    {
        // Arrange
        var types = new[]
        {
            typeof(string),
            typeof(List<string>),
            typeof(Dictionary<string, int>),
            typeof(IEnumerable<bool>),
            typeof(TestConcreteClass)
        };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 1000; i++)
        {
            foreach (var type in types)
            {
                var friendlyName = type.GetFriendlyName();
                var isConcreteClass = type.IsConcreteClass();
                var hasDefaultConstructor = type.HasDefaultConstructor();
                var interfaces = type.GetAllInterfaces().ToList();
            }
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 应在1秒内完成
    }

    /// <summary>
    /// 测试扩展方法的线程安全性
    /// </summary>
    [Fact]
    public void TypeExtensions_Should_Be_Thread_Safe()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var friendlyName = type.GetFriendlyName();
                                 var isConcreteClass = type.IsConcreteClass();
                                 var hasDefaultConstructor = type.HasDefaultConstructor();
                                 var implementsIList = type.ImplementsInterface(typeof(IList<string>));
                                 var interfaces = type.GetAllInterfaces().ToList();

                                 return new
                                 {
                                     FriendlyName = friendlyName,
                                     IsConcreteClass = isConcreteClass,
                                     HasDefaultConstructor = hasDefaultConstructor,
                                     ImplementsIList = implementsIList,
                                     InterfaceCount = interfaces.Count
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.FriendlyName.Should().Be("List<String>");
            result.IsConcreteClass.Should().BeTrue();
            result.HasDefaultConstructor.Should().BeTrue();
            result.ImplementsIList.Should().BeTrue();
            result.InterfaceCount.Should().BeGreaterThan(0);
        });
    }

    /// <summary>
    /// 测试扩展方法对null类型的处理
    /// </summary>
    [Fact]
    public void TypeExtensions_Should_Handle_Null_Type_Gracefully()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => nullType!.GetFriendlyName());
        Assert.Throws<NullReferenceException>(() => nullType!.IsConcreteClass());
        Assert.Throws<NullReferenceException>(() => nullType!.HasDefaultConstructor());
    }
}

// 测试用的类和接口
public interface ITestBase { }
public interface ITestDerived : ITestBase { }
public class TestImplementation : ITestDerived { }
public class TestConcreteClass { }

public class TestClassWithDefaultConstructor
{
    public TestClassWithDefaultConstructor() { }
}

public class TestClassWithoutDefaultConstructor
{
    public TestClassWithoutDefaultConstructor(string parameter) { }
}

public class TestClassWithMultipleConstructors
{
    public TestClassWithMultipleConstructors() { }
    public TestClassWithMultipleConstructors(string parameter) { }
    public TestClassWithMultipleConstructors(string parameter1, int parameter2) { }
}