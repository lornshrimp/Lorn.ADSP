using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Models;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Models;

/// <summary>
/// ComponentMetadata 模型的单元测试
/// 验证基础类型和枚举的正确性
/// </summary>
public class ComponentMetadataTests
{
    /// <summary>
    /// 测试组件元数据的默认构造函数
    /// </summary>
    [Fact]
    public void ComponentMetadata_Should_Initialize_With_Default_Values()
    {
        // Act
        var metadata = new ComponentMetadata();

        // Assert
        metadata.Properties.Should().NotBeNull();
        metadata.Properties.Should().BeEmpty();
        metadata.Dependencies.Should().NotBeNull();
        metadata.Dependencies.Should().BeEmpty();
        metadata.ProvidedServices.Should().NotBeNull();
        metadata.ProvidedServices.Should().BeEmpty();
        metadata.Tags.Should().NotBeNull();
        metadata.Tags.Should().BeEmpty();
        metadata.Version.Should().Be("1.0.0");
        metadata.Description.Should().Be("");
        metadata.Author.Should().Be("");
        metadata.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 测试组件元数据的属性设置
    /// </summary>
    [Fact]
    public void ComponentMetadata_Properties_Should_Be_Settable()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        var properties = new Dictionary<string, object>
        {
            ["Key1"] = "Value1",
            ["Key2"] = 42,
            ["Key3"] = true
        };

        // Act
        metadata.Properties = properties;

        // Assert
        metadata.Properties.Should().BeSameAs(properties);
        metadata.Properties.Should().HaveCount(3);
        metadata.Properties["Key1"].Should().Be("Value1");
        metadata.Properties["Key2"].Should().Be(42);
        metadata.Properties["Key3"].Should().Be(true);
    }

    /// <summary>
    /// 测试组件元数据的依赖类型列表
    /// </summary>
    [Fact]
    public void ComponentMetadata_Dependencies_Should_Support_Type_List()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        var dependencies = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(IDisposable)
        };

        // Act
        metadata.Dependencies = dependencies;

        // Assert
        metadata.Dependencies.Should().BeSameAs(dependencies);
        metadata.Dependencies.Should().HaveCount(3);
        metadata.Dependencies.Should().Contain(typeof(string));
        metadata.Dependencies.Should().Contain(typeof(int));
        metadata.Dependencies.Should().Contain(typeof(IDisposable));
    }

    /// <summary>
    /// 测试组件元数据的提供服务列表
    /// </summary>
    [Fact]
    public void ComponentMetadata_ProvidedServices_Should_Support_Type_List()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        var providedServices = new List<Type>
        {
            typeof(IEnumerable<string>),
            typeof(ICollection<int>),
            typeof(IList<bool>)
        };

        // Act
        metadata.ProvidedServices = providedServices;

        // Assert
        metadata.ProvidedServices.Should().BeSameAs(providedServices);
        metadata.ProvidedServices.Should().HaveCount(3);
        metadata.ProvidedServices.Should().Contain(typeof(IEnumerable<string>));
        metadata.ProvidedServices.Should().Contain(typeof(ICollection<int>));
        metadata.ProvidedServices.Should().Contain(typeof(IList<bool>));
    }

    /// <summary>
    /// 测试组件元数据的标签集合
    /// </summary>
    [Fact]
    public void ComponentMetadata_Tags_Should_Support_String_Set()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        var tags = new HashSet<string> { "Tag1", "Tag2", "Tag3" };

        // Act
        metadata.Tags = tags;

        // Assert
        metadata.Tags.Should().BeSameAs(tags);
        metadata.Tags.Should().HaveCount(3);
        metadata.Tags.Should().Contain("Tag1");
        metadata.Tags.Should().Contain("Tag2");
        metadata.Tags.Should().Contain("Tag3");
    }

    /// <summary>
    /// 测试组件元数据的标签去重功能
    /// </summary>
    [Fact]
    public void ComponentMetadata_Tags_Should_Prevent_Duplicates()
    {
        // Arrange
        var metadata = new ComponentMetadata();

        // Act
        metadata.Tags.Add("DuplicateTag");
        metadata.Tags.Add("DuplicateTag");
        metadata.Tags.Add("UniqueTag");

        // Assert
        metadata.Tags.Should().HaveCount(2);
        metadata.Tags.Should().Contain("DuplicateTag");
        metadata.Tags.Should().Contain("UniqueTag");
    }

    /// <summary>
    /// 测试组件元数据的版本字符串
    /// </summary>
    [Theory]
    [InlineData("1.0.0")]
    [InlineData("2.1.3")]
    [InlineData("1.0.0-alpha")]
    [InlineData("2.0.0-beta.1")]
    [InlineData("3.0.0-rc.1")]
    public void ComponentMetadata_Version_Should_Accept_Valid_Version_Strings(string version)
    {
        // Arrange
        var metadata = new ComponentMetadata();

        // Act
        metadata.Version = version;

        // Assert
        metadata.Version.Should().Be(version);
    }

    /// <summary>
    /// 测试组件元数据的描述字段
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Simple description")]
    [InlineData("A very long description with multiple sentences. This component does many things and provides various services.")]
    [InlineData("描述可以包含中文字符")]
    public void ComponentMetadata_Description_Should_Accept_Various_Strings(string description)
    {
        // Arrange
        var metadata = new ComponentMetadata();

        // Act
        metadata.Description = description;

        // Assert
        metadata.Description.Should().Be(description);
    }

    /// <summary>
    /// 测试组件元数据的作者字段
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("John Doe")]
    [InlineData("john.doe@example.com")]
    [InlineData("张三")]
    public void ComponentMetadata_Author_Should_Accept_Various_Strings(string author)
    {
        // Arrange
        var metadata = new ComponentMetadata();

        // Act
        metadata.Author = author;

        // Assert
        metadata.Author.Should().Be(author);
    }

    /// <summary>
    /// 测试组件元数据的创建时间
    /// </summary>
    [Fact]
    public void ComponentMetadata_CreatedAt_Should_Be_Settable()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        var specificTime = new DateTime(2023, 12, 25, 10, 30, 45, DateTimeKind.Utc);

        // Act
        metadata.CreatedAt = specificTime;

        // Assert
        metadata.CreatedAt.Should().Be(specificTime);
    }

    /// <summary>
    /// 测试组件元数据的复杂属性组合
    /// </summary>
    [Fact]
    public void ComponentMetadata_Should_Support_Complex_Property_Combinations()
    {
        // Arrange
        var metadata = new ComponentMetadata();

        // Act
        metadata.Properties["StringProperty"] = "TestValue";
        metadata.Properties["IntProperty"] = 42;
        metadata.Properties["BoolProperty"] = true;
        metadata.Properties["DateProperty"] = DateTime.Now;
        metadata.Properties["ListProperty"] = new List<string> { "Item1", "Item2" };
        metadata.Properties["DictProperty"] = new Dictionary<string, int> { ["Key1"] = 1, ["Key2"] = 2 };

        metadata.Dependencies.Add(typeof(IDisposable));
        metadata.Dependencies.Add(typeof(IAsyncDisposable));

        metadata.ProvidedServices.Add(typeof(IEnumerable<string>));
        metadata.ProvidedServices.Add(typeof(ICollection<int>));

        metadata.Tags.Add("Service");
        metadata.Tags.Add("Component");
        metadata.Tags.Add("Infrastructure");

        metadata.Version = "2.1.0";
        metadata.Description = "A complex component with multiple features";
        metadata.Author = "Test Author";

        // Assert
        metadata.Properties.Should().HaveCount(6);
        metadata.Dependencies.Should().HaveCount(2);
        metadata.ProvidedServices.Should().HaveCount(2);
        metadata.Tags.Should().HaveCount(3);
        metadata.Version.Should().Be("2.1.0");
        metadata.Description.Should().Be("A complex component with multiple features");
        metadata.Author.Should().Be("Test Author");
    }

    /// <summary>
    /// 测试组件元数据的线程安全性
    /// </summary>
    [Fact]
    public void ComponentMetadata_Should_Be_Thread_Safe_For_Reading()
    {
        // Arrange
        var metadata = new ComponentMetadata();
        metadata.Properties["TestKey"] = "TestValue";
        metadata.Dependencies.Add(typeof(string));
        metadata.ProvidedServices.Add(typeof(IDisposable));
        metadata.Tags.Add("TestTag");

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var props = metadata.Properties.Count;
                                 var deps = metadata.Dependencies.Count;
                                 var services = metadata.ProvidedServices.Count;
                                 var tags = metadata.Tags.Count;
                                 return props + deps + services + tags;
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllBeEquivalentTo(4); // 1 + 1 + 1 + 1 = 4
    }

    /// <summary>
    /// 测试组件元数据的内存使用效率
    /// </summary>
    [Fact]
    public void ComponentMetadata_Should_Be_Memory_Efficient()
    {
        // Arrange & Act
        var metadataList = new List<ComponentMetadata>();

        for (int i = 0; i < 1000; i++)
        {
            var metadata = new ComponentMetadata
            {
                Version = $"1.0.{i}",
                Description = $"Component {i}",
                Author = "Test Author"
            };
            metadata.Tags.Add($"Tag{i}");
            metadataList.Add(metadata);
        }

        // Assert
        metadataList.Should().HaveCount(1000);

        // 验证每个元数据对象都是独立的
        for (int i = 0; i < 1000; i++)
        {
            metadataList[i].Version.Should().Be($"1.0.{i}");
            metadataList[i].Description.Should().Be($"Component {i}");
            metadataList[i].Tags.Should().Contain($"Tag{i}");
        }
    }
}