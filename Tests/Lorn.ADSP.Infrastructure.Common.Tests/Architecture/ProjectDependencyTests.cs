using FluentAssertions;
using System.Reflection;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Architecture;

/// <summary>
/// 项目依赖关系的验证测试
/// 确保分层架构的依赖约束
/// </summary>
public class ProjectDependencyTests
{
    /// <summary>
    /// 测试Common项目不应依赖其他基础设施项目
    /// </summary>
    [Fact]
    public void Common_Project_Should_Not_Depend_On_Other_Infrastructure_Projects()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var infrastructureReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("Lorn.ADSP.Infrastructure") &&
                              assembly.Name != "Lorn.ADSP.Infrastructure.Common")
            .ToList();

        // Assert
        infrastructureReferences.Should().BeEmpty(
            "Common项目不应依赖其他基础设施项目，以保持分层架构的完整性");
    }

    /// <summary>
    /// 测试Common项目应该只依赖Core.Shared项目
    /// </summary>
    [Fact]
    public void Common_Project_Should_Only_Depend_On_Core_Shared()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var lornReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("Lorn.ADSP"))
            .ToList();

        // Assert
        lornReferences.Should().HaveCount(1, "Common项目应该只依赖Core.Shared项目");
        lornReferences.Should().Contain(assembly => assembly.Name == "Lorn.ADSP.Core.Shared",
            "Common项目应该依赖Core.Shared项目");
    }

    /// <summary>
    /// 测试Common项目应该只依赖必要的Microsoft扩展包
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Minimal_Microsoft_Dependencies()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var microsoftExtensionsReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("Microsoft.Extensions"))
            .ToList();

        // Assert
        microsoftExtensionsReferences.Should().NotBeEmpty("Common项目需要Microsoft.Extensions依赖");
        microsoftExtensionsReferences.Should().Contain(assembly =>
            assembly.Name!.Contains("HealthChecks") ||
            assembly.Name!.Contains("Diagnostics"),
            "Common项目应该包含健康检查相关的依赖");
    }

    /// <summary>
    /// 测试Common项目的程序集属性
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Have_Correct_Properties()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act & Assert
        commonAssembly.Should().NotBeNull();
        commonAssembly!.GetName().Name.Should().Be("Lorn.ADSP.Infrastructure.Common");
        commonAssembly.GetName().Version.Should().NotBeNull();
    }

    /// <summary>
    /// 测试Common项目的命名空间结构
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Correct_Namespace_Structure()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var types = commonAssembly!.GetTypes();
        var namespaces = types.Select(t => t.Namespace).Distinct().Where(ns => ns != null).ToList();

        // Assert
        namespaces.Should().Contain("Lorn.ADSP.Infrastructure.Common.Abstractions");
        namespaces.Should().Contain("Lorn.ADSP.Infrastructure.Common.Base");
        namespaces.Should().Contain("Lorn.ADSP.Infrastructure.Common.Models");
        namespaces.Should().Contain("Lorn.ADSP.Infrastructure.Common.Extensions");
        namespaces.Should().Contain("Lorn.ADSP.Infrastructure.Common.Conventions");

        // 验证所有命名空间都以正确的前缀开始
        namespaces.Should().AllSatisfy(ns =>
            ns!.Should().StartWith("Lorn.ADSP.Infrastructure.Common"));
    }

    /// <summary>
    /// 测试Common项目的公共类型可访问性
    /// </summary>
    [Fact]
    public void Common_Project_Should_Expose_Required_Public_Types()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var publicTypeNames = publicTypes.Select(t => t.Name).ToList();

        // Assert
        publicTypeNames.Should().Contain("IComponent");
        publicTypeNames.Should().Contain("IConfigurable");
        publicTypeNames.Should().Contain("IHealthCheckable");
        publicTypeNames.Should().Contain("ComponentBase");
        publicTypeNames.Should().Contain("ConfigurableComponentBase");
        publicTypeNames.Should().Contain("HealthCheckableComponentBase");
        publicTypeNames.Should().Contain("ComponentMetadata");
        publicTypeNames.Should().Contain("ComponentDescriptor");
        publicTypeNames.Should().Contain("InfrastructureOptions");
        publicTypeNames.Should().Contain("ServiceLifetime");
    }

    /// <summary>
    /// 测试Common项目的扩展方法类可访问性
    /// </summary>
    [Fact]
    public void Common_Project_Should_Expose_Extension_Methods()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var extensionClasses = publicTypes.Where(t => t.IsClass && t.IsSealed && t.IsAbstract).ToList();
        var extensionClassNames = extensionClasses.Select(t => t.Name).ToList();

        // Assert
        extensionClassNames.Should().Contain("TypeExtensions");
        extensionClassNames.Should().Contain("ReflectionExtensions");
        extensionClassNames.Should().Contain("ValidationExtensions");

        // 验证扩展方法类包含扩展方法
        foreach (var extensionClass in extensionClasses)
        {
            var methods = extensionClass.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var extensionMethods = methods.Where(m => m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute))).ToList();

            if (extensionClass.Name.EndsWith("Extensions"))
            {
                extensionMethods.Should().NotBeEmpty($"{extensionClass.Name} 应该包含扩展方法");
            }
        }
    }

    /// <summary>
    /// 测试Common项目的约定类结构
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Convention_Classes()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var conventionTypes = publicTypes.Where(t => t.Namespace?.Contains("Conventions") == true).ToList();
        var conventionTypeNames = conventionTypes.Select(t => t.Name).ToList();

        // Assert
        conventionTypeNames.Should().Contain("ComponentConventions");
        conventionTypeNames.Should().Contain("ConfigurationConventions");
        conventionTypeNames.Should().Contain("NamingConventions");
    }

    /// <summary>
    /// 测试Common项目不应包含具体的业务逻辑实现
    /// </summary>
    [Fact]
    public void Common_Project_Should_Not_Contain_Business_Logic_Implementations()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var types = commonAssembly!.GetTypes();
        var businessLogicTypes = types.Where(t =>
            t.Name.Contains("Service") && !t.Name.Contains("ServiceLifetime") ||
            t.Name.Contains("Manager") ||
            t.Name.Contains("Provider") && !t.Name.Contains("Provider") ||
            t.Name.Contains("Strategy") ||
            t.Name.Contains("Engine") ||
            t.Name.Contains("Processor")).ToList();

        // Assert
        businessLogicTypes.Should().BeEmpty(
            "Common项目不应包含具体的业务逻辑实现，只应包含基础抽象和工具类");
    }

    /// <summary>
    /// 测试Common项目的性能特征
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Load_Quickly()
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 100; i++)
        {
            var assembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));
            var types = assembly!.GetTypes();
            var publicTypes = assembly.GetExportedTypes();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            "Common项目程序集应该能够快速加载和反射");
    }

    /// <summary>
    /// 测试Common项目的线程安全性
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Be_Thread_Safe()
    {
        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var assembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));
                                 var types = assembly!.GetTypes();
                                 var publicTypes = assembly.GetExportedTypes();
                                 var referencedAssemblies = assembly.GetReferencedAssemblies();

                                 return new
                                 {
                                     TypeCount = types.Length,
                                     PublicTypeCount = publicTypes.Length,
                                     ReferencedAssemblyCount = referencedAssemblies.Length
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.TypeCount.Should().BeGreaterThan(0);
            result.PublicTypeCount.Should().BeGreaterThan(0);
            result.ReferencedAssemblyCount.Should().BeGreaterThan(0);
        });

        // 验证所有结果一致
        var firstResult = results[0];
        results.Should().AllSatisfy(result =>
        {
            result.TypeCount.Should().Be(firstResult.TypeCount);
            result.PublicTypeCount.Should().Be(firstResult.PublicTypeCount);
            result.ReferencedAssemblyCount.Should().Be(firstResult.ReferencedAssemblyCount);
        });
    }

    /// <summary>
    /// 测试Common项目的版本兼容性
    /// </summary>
    [Fact]
    public void Common_Project_Should_Target_Correct_Framework_Version()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(Lorn.ADSP.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var targetFrameworkAttribute = commonAssembly!.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();

        // Assert
        targetFrameworkAttribute.Should().NotBeNull("程序集应该有目标框架属性");
        targetFrameworkAttribute!.FrameworkName.Should().StartWith(".NETCoreApp,Version=v9.0",
            "Common项目应该目标.NET 9.0框架");
    }
}