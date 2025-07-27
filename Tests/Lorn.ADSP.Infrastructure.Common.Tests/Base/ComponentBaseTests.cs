using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Base;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Base;

/// <summary>
/// ComponentBase 基础抽象类的单元测试
/// 确保抽象类提供的默认实现正确
/// </summary>
public class ComponentBaseTests
{
    /// <summary>
    /// 测试组件基类的名称属性默认实现
    /// </summary>
    [Fact]
    public void Name_Property_Should_Return_Type_Name_By_Default()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var name = component.Name;

        // Assert
        name.Should().Be("TestComponent");
    }

    /// <summary>
    /// 测试组件基类的初始化状态
    /// </summary>
    [Fact]
    public void IsInitialized_Should_Be_False_Initially()
    {
        // Arrange
        var component = new TestComponent();

        // Act & Assert
        component.IsInitialized.Should().BeFalse();
    }

    /// <summary>
    /// 测试组件基类的初始化方法
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Set_IsInitialized_To_True()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        await component.InitializeAsync();

        // Assert
        component.IsInitialized.Should().BeTrue();
    }

    /// <summary>
    /// 测试组件基类的重复初始化处理
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Not_Initialize_Twice()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        await component.InitializeAsync();
        await component.InitializeAsync(); // 第二次初始化

        // Assert
        component.IsInitialized.Should().BeTrue();
        component.InitializeCallCount.Should().Be(1); // 只应该调用一次
    }

    /// <summary>
    /// 测试组件基类的初始化取消令牌支持
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Support_Cancellation_Token()
    {
        // Arrange
        var component = new TestComponent();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await component.InitializeAsync(cancellationToken);

        // Assert
        component.IsInitialized.Should().BeTrue();
        component.LastCancellationToken.Should().Be(cancellationToken);
    }

    /// <summary>
    /// 测试组件基类的初始化异常处理
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Propagate_Exceptions()
    {
        // Arrange
        var component = new ExceptionThrowingComponent();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => component.InitializeAsync());

        exception.Message.Should().Be("Initialization failed");
        component.IsInitialized.Should().BeFalse();
    }

    /// <summary>
    /// 测试组件基类的资源释放方法
    /// </summary>
    [Fact]
    public async Task DisposeAsync_Should_Call_OnDisposeAsync()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        await component.DisposeAsync();

        // Assert
        component.DisposeCallCount.Should().Be(1);
    }

    /// <summary>
    /// 测试组件基类的重复释放处理
    /// </summary>
    [Fact]
    public async Task DisposeAsync_Should_Not_Dispose_Twice()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        await component.DisposeAsync();
        await component.DisposeAsync(); // 第二次释放

        // Assert
        component.DisposeCallCount.Should().Be(1); // 只应该调用一次
    }

    /// <summary>
    /// 测试组件基类的释放后初始化异常
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Throw_After_Dispose()
    {
        // Arrange
        var component = new TestComponent();
        await component.DisposeAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ObjectDisposedException>(
            () => component.InitializeAsync());

        exception.ObjectName.Should().Be("TestComponent");
    }

    /// <summary>
    /// 测试组件基类的完整生命周期
    /// </summary>
    [Fact]
    public async Task Component_Lifecycle_Should_Work_Correctly()
    {
        // Arrange
        var component = new TestComponent();

        // Act & Assert - 初始状态
        component.IsInitialized.Should().BeFalse();
        component.InitializeCallCount.Should().Be(0);
        component.DisposeCallCount.Should().Be(0);

        // 初始化
        await component.InitializeAsync();
        component.IsInitialized.Should().BeTrue();
        component.InitializeCallCount.Should().Be(1);

        // 释放
        await component.DisposeAsync();
        component.DisposeCallCount.Should().Be(1);
    }

    /// <summary>
    /// 测试组件基类的IAsyncDisposable接口实现
    /// </summary>
    [Fact]
    public async Task IAsyncDisposable_DisposeAsync_Should_Work()
    {
        // Arrange
        IAsyncDisposable component = new TestComponent();

        // Act
        await component.DisposeAsync();

        // Assert
        // 验证没有异常抛出，说明IAsyncDisposable实现正确
        Assert.True(true);
    }

    /// <summary>
    /// 测试组件基类的虚方法重写
    /// </summary>
    [Fact]
    public void Virtual_Methods_Should_Be_Overridable()
    {
        // Arrange
        var component = new CustomNameComponent();

        // Act
        var name = component.Name;

        // Assert
        name.Should().Be("CustomName");
    }

    /// <summary>
    /// 测试组件基类的并发初始化安全性
    /// </summary>
    [Fact]
    public async Task InitializeAsync_Should_Be_Thread_Safe()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() => component.InitializeAsync()))
                             .ToArray();

        await Task.WhenAll(tasks);

        // Assert
        component.IsInitialized.Should().BeTrue();
        component.InitializeCallCount.Should().Be(1); // 只应该初始化一次
    }

    /// <summary>
    /// 测试组件基类的并发释放安全性
    /// </summary>
    [Fact]
    public async Task DisposeAsync_Should_Be_Thread_Safe()
    {
        // Arrange
        var component = new TestComponent();
        await component.InitializeAsync();

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() => component.DisposeAsync()))
                             .ToArray();

        await Task.WhenAll(tasks);

        // Assert
        component.DisposeCallCount.Should().Be(1); // 只应该释放一次
    }
}

/// <summary>
/// 测试用的组件实现
/// </summary>
public class TestComponent : ComponentBase
{
    public int InitializeCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        InitializeCallCount++;
        LastCancellationToken = cancellationToken;
        return Task.CompletedTask;
    }

    protected override Task OnDisposeAsync()
    {
        DisposeCallCount++;
        return Task.CompletedTask;
    }
}

/// <summary>
/// 测试用的异常抛出组件
/// </summary>
public class ExceptionThrowingComponent : ComponentBase
{
    protected override Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Initialization failed");
    }
}

/// <summary>
/// 测试用的自定义名称组件
/// </summary>
public class CustomNameComponent : ComponentBase
{
    public override string Name => "CustomName";
}