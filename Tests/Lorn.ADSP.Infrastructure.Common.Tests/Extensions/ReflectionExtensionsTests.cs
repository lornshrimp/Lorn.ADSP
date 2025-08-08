using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.Reflection;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Extensions;

/// <summary>
/// ReflectionExtensions 扩展方法的单元测试
/// 验证反射扩展功能的正确性
/// </summary>
public class ReflectionExtensionsTests
{
    /// <summary>
    /// 测试GetConcreteTypes方法获取程序集中的具体类型
    /// </summary>
    [Fact]
    public void GetConcreteTypes_Should_Return_Only_Concrete_Classes()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var concreteTypes = assembly.GetConcreteTypes().ToList();

        // Assert
        concreteTypes.Should().NotBeEmpty();
        concreteTypes.Should().AllSatisfy(type =>
        {
            type.IsClass.Should().BeTrue();
            type.IsAbstract.Should().BeFalse();
            type.IsGenericTypeDefinition.Should().BeFalse();
        });

        // 验证包含测试类
        concreteTypes.Should().Contain(typeof(TestReflectionClass));
        concreteTypes.Should().Contain(typeof(TestServiceImplementation));
    }

    /// <summary>
    /// 测试GetConcreteTypes方法对ReflectionTypeLoadException的处理
    /// </summary>
    [Fact]
    public void GetConcreteTypes_Should_Handle_ReflectionTypeLoadException()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act & Assert - 应该不抛出异常
        var concreteTypes = assembly.GetConcreteTypes().ToList();
        concreteTypes.Should().NotBeNull();
    }

    /// <summary>
    /// 测试GetTypesImplementing方法通过接口类型查找实现类
    /// </summary>
    [Fact]
    public void GetTypesImplementing_Should_Find_Types_By_Interface_Type()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var interfaceType = typeof(ITestReflectionService);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(interfaceType).ToList();

        // Assert
        implementingTypes.Should().NotBeEmpty();
        implementingTypes.Should().Contain(typeof(TestServiceImplementation));
        implementingTypes.Should().AllSatisfy(type =>
        {
            type.ImplementsInterface(interfaceType).Should().BeTrue();
        });
    }

    /// <summary>
    /// 测试GetTypesImplementing方法通过接口名称查找实现类
    /// </summary>
    [Fact]
    public void GetTypesImplementing_Should_Find_Types_By_Interface_Name()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var interfaceName = "ITestReflectionService";

        // Act
        var implementingTypes = assembly.GetTypesImplementing(interfaceName).ToList();

        // Assert
        implementingTypes.Should().NotBeEmpty();
        implementingTypes.Should().Contain(typeof(TestServiceImplementation));
        implementingTypes.Should().AllSatisfy(type =>
        {
            type.ImplementsInterface(interfaceName).Should().BeTrue();
        });
    }

    /// <summary>
    /// 测试GetTypesInheriting方法查找继承指定基类的类型
    /// </summary>
    [Fact]
    public void GetTypesInheriting_Should_Find_Types_Inheriting_Base_Class()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var baseType = typeof(TestReflectionBaseClass);

        // Act
        var inheritingTypes = assembly.GetTypesInheriting(baseType).ToList();

        // Assert
        inheritingTypes.Should().NotBeEmpty();
        inheritingTypes.Should().Contain(typeof(TestReflectionDerivedClass));
        inheritingTypes.Should().NotContain(baseType); // 不应包含基类本身
        inheritingTypes.Should().AllSatisfy(type =>
        {
            baseType.IsAssignableFrom(type).Should().BeTrue();
        });
    }

    /// <summary>
    /// 测试GetTypesWithAttribute方法通过特性类型查找类型
    /// </summary>
    [Fact]
    public void GetTypesWithAttribute_Should_Find_Types_With_Attribute_Type()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var attributeType = typeof(TestReflectionAttribute);

        // Act
        var typesWithAttribute = assembly.GetTypesWithAttribute(attributeType).ToList();

        // Assert
        typesWithAttribute.Should().NotBeEmpty();
        typesWithAttribute.Should().Contain(typeof(TestAttributedClass));
        typesWithAttribute.Should().AllSatisfy(type =>
        {
            type.IsDefined(attributeType, inherit: false).Should().BeTrue();
        });
    }

    /// <summary>
    /// 测试GetTypesWithAttribute泛型方法查找带特性的类型
    /// </summary>
    [Fact]
    public void GetTypesWithAttribute_Generic_Should_Find_Types_With_Attribute()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var typesWithAttribute = assembly.GetTypesWithAttribute<TestReflectionAttribute>().ToList();

        // Assert
        typesWithAttribute.Should().NotBeEmpty();
        typesWithAttribute.Should().Contain(typeof(TestAttributedClass));
        typesWithAttribute.Should().AllSatisfy(type =>
        {
            type.GetCustomAttributes(typeof(TestReflectionAttribute), inherit: false).Any().Should().BeTrue();
        });
    }

    /// <summary>
    /// 测试GetCustomAttributeSafe方法的安全特性获取
    /// </summary>
    [Fact]
    public void GetCustomAttributeSafe_Should_Return_Attribute_Safely()
    {
        // Arrange
        var type = typeof(TestAttributedClass);

        // Act
        var attribute = type.GetCustomAttributeSafe<TestReflectionAttribute>();

        // Assert
        attribute.Should().NotBeNull();
        attribute!.Value.Should().Be("TestValue");
    }

    /// <summary>
    /// 测试GetCustomAttributeSafe方法对不存在特性的处理
    /// </summary>
    [Fact]
    public void GetCustomAttributeSafe_Should_Return_Null_For_Missing_Attribute()
    {
        // Arrange
        var type = typeof(TestReflectionClass);

        // Act
        var attribute = type.GetCustomAttributeSafe<TestReflectionAttribute>();

        // Assert
        attribute.Should().BeNull();
    }

    /// <summary>
    /// 测试GetCustomAttributesSafe方法获取所有特性
    /// </summary>
    [Fact]
    public void GetCustomAttributesSafe_Should_Return_All_Attributes_Safely()
    {
        // Arrange
        var type = typeof(TestMultipleAttributedClass);

        // Act
        var attributes = type.GetCustomAttributesSafe<TestReflectionAttribute>().ToList();

        // Assert
        attributes.Should().HaveCount(2);
        attributes.Should().Contain(attr => attr.Value == "Value1");
        attributes.Should().Contain(attr => attr.Value == "Value2");
    }

    /// <summary>
    /// 测试GetCustomAttributesSafe方法对不存在特性的处理
    /// </summary>
    [Fact]
    public void GetCustomAttributesSafe_Should_Return_Empty_For_Missing_Attributes()
    {
        // Arrange
        var type = typeof(TestReflectionClass);

        // Act
        var attributes = type.GetCustomAttributesSafe<TestReflectionAttribute>().ToList();

        // Assert
        attributes.Should().BeEmpty();
    }

    /// <summary>
    /// 测试CreateInstance方法创建类型实例
    /// </summary>
    [Fact]
    public void CreateInstance_Should_Create_Type_Instance()
    {
        // Arrange
        var type = typeof(TestReflectionClass);

        // Act
        var instance = type.CreateInstance();

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestReflectionClass>();
    }

    /// <summary>
    /// 测试CreateInstance方法带参数创建实例
    /// </summary>
    [Fact]
    public void CreateInstance_Should_Create_Instance_With_Parameters()
    {
        // Arrange
        var type = typeof(TestParameterizedClass);
        var parameter = "TestParameter";

        // Act
        var instance = type.CreateInstance(parameter);

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestParameterizedClass>();
        ((TestParameterizedClass)instance!).Parameter.Should().Be(parameter);
    }

    /// <summary>
    /// 测试CreateInstance方法对无法创建实例的类型的处理
    /// </summary>
    [Fact]
    public void CreateInstance_Should_Return_Null_For_Uncreatable_Type()
    {
        // Arrange
        var type = typeof(TestAbstractClass);

        // Act
        var instance = type.CreateInstance();

        // Assert
        instance.Should().BeNull();
    }

    /// <summary>
    /// 测试CreateInstance泛型方法创建强类型实例
    /// </summary>
    [Fact]
    public void CreateInstance_Generic_Should_Create_Typed_Instance()
    {
        // Arrange
        var type = typeof(TestReflectionClass);

        // Act
        var instance = type.CreateInstance<TestReflectionClass>();

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestReflectionClass>();
    }

    /// <summary>
    /// 测试CreateInstance泛型方法对类型不匹配的处理
    /// </summary>
    [Fact]
    public void CreateInstance_Generic_Should_Return_Null_For_Type_Mismatch()
    {
        // Arrange
        var type = typeof(TestReflectionClass);

        // Act
        var instance = type.CreateInstance<TestServiceImplementation>();

        // Assert
        instance.Should().BeNull();
    }

    /// <summary>
    /// 测试反射扩展方法的性能特征
    /// </summary>
    [Fact]
    public void ReflectionExtensions_Should_Execute_Efficiently()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 100; i++)
        {
            var concreteTypes = assembly.GetConcreteTypes().ToList();
            var implementingTypes = assembly.GetTypesImplementing(typeof(ITestReflectionService)).ToList();
            var inheritingTypes = assembly.GetTypesInheriting(typeof(TestReflectionBaseClass)).ToList();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 应在5秒内完成
    }

    /// <summary>
    /// 测试反射扩展方法的线程安全性
    /// </summary>
    [Fact]
    public async Task ReflectionExtensions_Should_Be_Thread_Safe()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var tasks = Enumerable.Range(0, 5)
                             .Select(_ => Task.Run(() =>
                             {
                                 var concreteTypes = assembly.GetConcreteTypes().ToList();
                                 var implementingTypes = assembly.GetTypesImplementing(typeof(ITestReflectionService)).ToList();
                                 var inheritingTypes = assembly.GetTypesInheriting(typeof(TestReflectionBaseClass)).ToList();
                                 var attributedTypes = assembly.GetTypesWithAttribute<TestReflectionAttribute>().ToList();

                                 return new
                                 {
                                     ConcreteTypesCount = concreteTypes.Count,
                                     ImplementingTypesCount = implementingTypes.Count,
                                     InheritingTypesCount = inheritingTypes.Count,
                                     AttributedTypesCount = attributedTypes.Count
                                 };
                             }))
                             .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.ConcreteTypesCount.Should().BeGreaterThan(0);
            result.ImplementingTypesCount.Should().BeGreaterThan(0);
            result.InheritingTypesCount.Should().BeGreaterThan(0);
            result.AttributedTypesCount.Should().BeGreaterThan(0);
        });

        // 验证所有结果一致
        var firstResult = results[0];
        results.Should().AllSatisfy(result =>
        {
            result.ConcreteTypesCount.Should().Be(firstResult.ConcreteTypesCount);
            result.ImplementingTypesCount.Should().Be(firstResult.ImplementingTypesCount);
            result.InheritingTypesCount.Should().Be(firstResult.InheritingTypesCount);
            result.AttributedTypesCount.Should().Be(firstResult.AttributedTypesCount);
        });
    }
}

// 测试用的接口、类和特性
public interface ITestReflectionService
{
    void DoSomething();
}

public class TestServiceImplementation : ITestReflectionService
{
    public void DoSomething() { }
}

public class TestReflectionClass { }

public abstract class TestReflectionBaseClass { }

public class TestReflectionDerivedClass : TestReflectionBaseClass { }

public abstract class TestAbstractClass { }

public class TestParameterizedClass
{
    public string Parameter { get; }

    public TestParameterizedClass(string parameter)
    {
        Parameter = parameter;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TestReflectionAttribute : Attribute
{
    public string Value { get; }

    public TestReflectionAttribute(string value)
    {
        Value = value;
    }
}

[TestReflection("TestValue")]
public class TestAttributedClass { }

[TestReflection("Value1")]
[TestReflection("Value2")]
public class TestMultipleAttributedClass { }