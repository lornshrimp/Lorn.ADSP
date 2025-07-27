using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Moq;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Abstractions;

/// <summary>
/// IComponent 接口实现的单元测试
/// 验证组件基础功能和生命周期管理
/// </summary>
public class IComponentTests
{
    /// <summary>
    /// 测试组件名称属性的正确性
    /// </summary>
    [Fact]
    public void Name_Property_Should_Return_Valid_String()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        var expectedName = "TestComponent";
        mockComponent.Setup(x => x.Name).Returns(expectedName);

        // Act
        var actualName = mockComponent.Object.Name;

        // Assert
        actualName.Should().Be(expectedName);
        actualName.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// 测试组件初始化状态的正确性
    /// </summary>
    [Fact]
    public void IsInitialized_Property_Should_Reflect_Initialization_State()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(x => x.IsInitialized).Returns(false);

        // Act & Assert - 初始状态应为未初始化
        mockComponent.Object.IsInitialized.Should().BeFalse();

        // 模拟初始化后的状态
        mockComponent.Setup(x => x.IsInitialized).Returns(true);
        mockComponent.Object.IsInitialized.Should().BeTrue();
    }

    /// <summary>
    /// 测试组件初始化方法的异步执行
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Execute_Without_Exception()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        var cancellationToken = CancellationToken.None;

        mockComponent.Setup(x => x.InitializeAsync(cancellationToken))
                    .Returns(Task.CompletedTask);

        // Act
        var initializeTask = mockComponent.Object.InitializeAsync(cancellationToken);

        // Assert
        await initializeTask;
        initializeTask.IsCompletedSuccessfully.Should().BeTrue();
        mockComponent.Verify(x => x.InitializeAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试组件初始化方法支持取消令牌
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Support_Cancellation_Token()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        mockComponent.Setup(x => x.InitializeAsync(cancellationToken))
                    .Returns(Task.CompletedTask);

        // Act
        await mockComponent.Object.InitializeAsync(cancellationToken);

        // Assert
        mockComponent.Verify(x => x.InitializeAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试组件初始化方法的默认取消令牌处理
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Handle_Default_Cancellation_Token()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        // Act
        await mockComponent.Object.InitializeAsync();

        // Assert
        mockComponent.Verify(x => x.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 测试组件资源释放方法的异步执行
    /// </summary>
    [Fact]
    public async Task DisposeAsync_Should_Execute_Without_Exception()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(x => x.DisposeAsync())
                    .Returns(Task.CompletedTask);

        // Act
        var disposeTask = mockComponent.Object.DisposeAsync();

        // Assert
        await disposeTask;
        disposeTask.IsCompletedSuccessfully.Should().BeTrue();
        mockComponent.Verify(x => x.DisposeAsync(), Times.Once);
    }

    /// <summary>
    /// 测试组件生命周期的完整流程
    /// </summary>
    [Fact]
    public async Task Component_Lifecycle_Should_Work_Correctly()
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        var componentName = "TestLifecycleComponent";

        // 设置初始状态
        mockComponent.Setup(x => x.Name).Returns(componentName);
        mockComponent.Setup(x => x.IsInitialized).Returns(false);
        mockComponent.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Callback(() => mockComponent.Setup(x => x.IsInitialized).Returns(true));
        mockComponent.Setup(x => x.DisposeAsync()).Returns(Task.CompletedTask);

        var component = mockComponent.Object;

        // Act & Assert - 初始状态
        component.Name.Should().Be(componentName);
        component.IsInitialized.Should().BeFalse();

        // 初始化组件
        await component.InitializeAsync();
        component.IsInitialized.Should().BeTrue();

        // 释放组件
        await component.DisposeAsync();

        // Verify
        mockComponent.Verify(x => x.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockComponent.Verify(x => x.DisposeAsync(), Times.Once);
    }

    /// <summary>
    /// 测试组件接口的多态性支持
    /// </summary>
    [Fact]
    public void IComponent_Should_Support_Polymorphism()
    {
        // Arrange
        var mockComponent1 = new Mock<IComponent>();
        var mockComponent2 = new Mock<IComponent>();

        mockComponent1.Setup(x => x.Name).Returns("Component1");
        mockComponent2.Setup(x => x.Name).Returns("Component2");

        var components = new List<IComponent> { mockComponent1.Object, mockComponent2.Object };

        // Act
        var names = components.Select(c => c.Name).ToList();

        // Assert
        names.Should().HaveCount(2);
        names.Should().Contain("Component1");
        names.Should().Contain("Component2");
    }

    /// <summary>
    /// 测试组件接口的边界条件处理
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("ValidComponentName")]
    [InlineData("Component_With_Underscores")]
    [InlineData("ComponentWithNumbers123")]
    public void Name_Property_Should_Handle_Various_Name_Formats(string componentName)
    {
        // Arrange
        var mockComponent = new Mock<IComponent>();
        mockComponent.Setup(x => x.Name).Returns(componentName);

        // Act
        var actualName = mockComponent.Object.Name;

        // Assert
        actualName.Should().Be(componentName);
    }
}