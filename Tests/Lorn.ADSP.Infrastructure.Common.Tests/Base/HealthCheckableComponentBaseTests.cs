using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Base;

/// <summary>
/// HealthCheckableComponentBase 基础抽象类的单元测试
/// 确保抽象类提供的默认实现正确
/// </summary>
public class HealthCheckableComponentBaseTests
{
    /// <summary>
    /// 测试健康检查组件基类的默认健康检查实现
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Healthy_By_Default()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var result = await component.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Healthy);
    }

    /// <summary>
    /// 测试健康检查组件基类对未初始化组件的处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Unhealthy_When_Not_Initialized()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        // 不调用InitializeAsync

        // Act
        var result = await component.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Unhealthy);
    }

    /// <summary>
    /// 测试健康检查组件基类的异常处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Unhealthy_On_Exception()
    {
        // Arrange
        var component = new ExceptionThrowingHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var result = await component.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Unhealthy);
    }

    /// <summary>
    /// 测试健康检查组件基类的取消令牌支持
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Support_Cancellation_Token()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        await component.InitializeAsync();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        var result = await component.CheckHealthAsync(cancellationToken);

        // Assert
        result.Should().Be(HealthStatus.Healthy);
        component.LastCancellationToken.Should().Be(cancellationToken);
    }

    /// <summary>
    /// 测试健康检查组件基类的自定义健康检查实现
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Use_Custom_Implementation()
    {
        // Arrange
        var component = new CustomHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var result = await component.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Degraded);
        component.HealthCheckCallCount.Should().Be(1);
    }

    /// <summary>
    /// 测试健康检查组件基类的多次健康检查调用
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Support_Multiple_Calls()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var result1 = await component.CheckHealthAsync();
        var result2 = await component.CheckHealthAsync();
        var result3 = await component.CheckHealthAsync();

        // Assert
        result1.Should().Be(HealthStatus.Healthy);
        result2.Should().Be(HealthStatus.Healthy);
        result3.Should().Be(HealthStatus.Healthy);
        component.HealthCheckCallCount.Should().Be(3);
    }

    /// <summary>
    /// 测试健康检查组件基类的并发健康检查安全性
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Be_Thread_Safe()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() => component.CheckHealthAsync()))
                             .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllBeEquivalentTo(HealthStatus.Healthy);
        component.HealthCheckCallCount.Should().Be(10);
    }

    /// <summary>
    /// 测试健康检查组件基类的性能特征
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Execute_Quickly()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, 100)
                             .Select(_ => component.CheckHealthAsync())
                             .ToArray();

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 应在1秒内完成100次健康检查
        component.HealthCheckCallCount.Should().Be(100);
    }

    /// <summary>
    /// 测试健康检查组件基类的生命周期集成
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Integrate_With_Component_Lifecycle()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();

        // Act & Assert - 未初始化状态
        var resultBeforeInit = await component.CheckHealthAsync();
        resultBeforeInit.Should().Be(HealthStatus.Unhealthy);

        // 初始化后
        await component.InitializeAsync();
        var resultAfterInit = await component.CheckHealthAsync();
        resultAfterInit.Should().Be(HealthStatus.Healthy);

        // 释放后
        await component.DisposeAsync();
        // 注意：释放后的健康检查行为取决于具体实现
    }

    /// <summary>
    /// 测试健康检查组件基类的异步异常处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Handle_Async_Exceptions()
    {
        // Arrange
        var component = new AsyncExceptionHealthCheckableComponent();
        await component.InitializeAsync();

        // Act
        var result = await component.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Unhealthy);
    }

    /// <summary>
    /// 测试健康检查组件基类的超时处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Handle_Timeout()
    {
        // Arrange
        var component = new SlowHealthCheckableComponent();
        await component.InitializeAsync();
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        var cancellationToken = cancellationTokenSource.Token;

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => component.CheckHealthAsync(cancellationToken));
    }

    /// <summary>
    /// 测试健康检查组件基类的状态变化响应
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Reflect_Component_State_Changes()
    {
        // Arrange
        var component = new StatefulHealthCheckableComponent();
        await component.InitializeAsync();

        // Act & Assert - 初始健康状态
        var initialResult = await component.CheckHealthAsync();
        initialResult.Should().Be(HealthStatus.Healthy);

        // 改变组件状态
        component.SetUnhealthy();
        var unhealthyResult = await component.CheckHealthAsync();
        unhealthyResult.Should().Be(HealthStatus.Unhealthy);

        // 恢复健康状态
        component.SetHealthy();
        var recoveredResult = await component.CheckHealthAsync();
        recoveredResult.Should().Be(HealthStatus.Healthy);
    }

    /// <summary>
    /// 测试健康检查组件基类的继承链正确性
    /// </summary>
    [Fact]
    public void HealthCheckableComponentBase_Should_Inherit_From_ComponentBase()
    {
        // Arrange
        var component = new TestHealthCheckableComponent();

        // Act & Assert
        component.Should().BeAssignableTo<ComponentBase>();
        component.Should().BeAssignableTo<Lorn.ADSP.Infrastructure.Common.Abstractions.IHealthCheckable>();
        component.Name.Should().Be("TestHealthCheckableComponent");
    }
}

/// <summary>
/// 测试用的健康检查组件实现
/// </summary>
public class TestHealthCheckableComponent : HealthCheckableComponentBase
{
    public int HealthCheckCallCount { get; private set; }
    public CancellationToken LastCancellationToken { get; private set; }

    protected override Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        HealthCheckCallCount++;
        LastCancellationToken = cancellationToken;
        return Task.FromResult(HealthStatus.Healthy);
    }
}

/// <summary>
/// 测试用的异常抛出健康检查组件
/// </summary>
public class ExceptionThrowingHealthCheckableComponent : HealthCheckableComponentBase
{
    protected override Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Health check failed");
    }
}

/// <summary>
/// 测试用的自定义健康检查组件
/// </summary>
public class CustomHealthCheckableComponent : HealthCheckableComponentBase
{
    public int HealthCheckCallCount { get; private set; }

    protected override Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        HealthCheckCallCount++;
        return Task.FromResult(HealthStatus.Degraded);
    }
}

/// <summary>
/// 测试用的异步异常健康检查组件
/// </summary>
public class AsyncExceptionHealthCheckableComponent : HealthCheckableComponentBase
{
    protected override async Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);
        throw new InvalidOperationException("Async health check failed");
    }
}

/// <summary>
/// 测试用的慢速健康检查组件
/// </summary>
public class SlowHealthCheckableComponent : HealthCheckableComponentBase
{
    protected override async Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(200, cancellationToken); // 延迟200ms
        return HealthStatus.Healthy;
    }
}

/// <summary>
/// 测试用的有状态健康检查组件
/// </summary>
public class StatefulHealthCheckableComponent : HealthCheckableComponentBase
{
    private bool _isHealthy = true;

    public void SetHealthy() => _isHealthy = true;
    public void SetUnhealthy() => _isHealthy = false;

    protected override Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isHealthy ? HealthStatus.Healthy : HealthStatus.Unhealthy);
    }
}