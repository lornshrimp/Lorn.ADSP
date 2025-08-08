using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Abstractions;

/// <summary>
/// IHealthCheckable 接口的单元测试
/// 验证健康检查功能的正确性
/// </summary>
public class IHealthCheckableTests
{
    /// <summary>
    /// 测试健康检查方法返回健康状态
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Healthy_Status()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationToken = CancellationToken.None;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .ReturnsAsync(HealthStatus.Healthy);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync(cancellationToken);

        // Assert
        result.Should().Be(HealthStatus.Healthy);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法返回不健康状态
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Unhealthy_Status()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationToken = CancellationToken.None;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .ReturnsAsync(HealthStatus.Unhealthy);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync(cancellationToken);

        // Assert
        result.Should().Be(HealthStatus.Unhealthy);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法返回降级状态
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Degraded_Status()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationToken = CancellationToken.None;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .ReturnsAsync(HealthStatus.Degraded);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync(cancellationToken);

        // Assert
        result.Should().Be(HealthStatus.Degraded);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法支持取消令牌
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Support_Cancellation_Token()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .ReturnsAsync(HealthStatus.Healthy);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync(cancellationToken);

        // Assert
        result.Should().Be(HealthStatus.Healthy);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法的默认取消令牌处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Handle_Default_Cancellation_Token()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        mockHealthCheckable.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(HealthStatus.Healthy);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync();

        // Assert
        result.Should().Be(HealthStatus.Healthy);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法的异常处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Handle_Exceptions_Gracefully()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationToken = CancellationToken.None;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .ThrowsAsync(new InvalidOperationException("Health check failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => mockHealthCheckable.Object.CheckHealthAsync(cancellationToken));

        exception.Message.Should().Be("Health check failed");
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法的超时处理
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Handle_Timeout()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        var cancellationToken = cancellationTokenSource.Token;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(cancellationToken))
                          .Returns(async (CancellationToken ct) =>
                          {
                              await Task.Delay(200, ct); // 延迟超过超时时间
                              return HealthStatus.Healthy;
                          });

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => mockHealthCheckable.Object.CheckHealthAsync(cancellationToken));

        mockHealthCheckable.Verify(x => x.CheckHealthAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// 测试健康检查方法的并发执行
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Support_Concurrent_Execution()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        var callCount = 0;

        mockHealthCheckable.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                          .Returns(async (CancellationToken ct) =>
                          {
                              Interlocked.Increment(ref callCount);
                              await Task.Delay(10, ct);
                              return HealthStatus.Healthy;
                          });

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => mockHealthCheckable.Object.CheckHealthAsync())
                             .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllBeEquivalentTo(HealthStatus.Healthy);
        callCount.Should().Be(10);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()), Times.Exactly(10));
    }

    /// <summary>
    /// 测试健康检查方法的性能特征
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Execute_Quickly()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        mockHealthCheckable.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(HealthStatus.Healthy);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, 100)
                             .Select(_ => mockHealthCheckable.Object.CheckHealthAsync())
                             .ToArray();

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 应在1秒内完成100次健康检查
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()), Times.Exactly(100));
    }

    /// <summary>
    /// 测试健康检查状态的所有可能值
    /// </summary>
    [Theory]
    [InlineData(HealthStatus.Healthy)]
    [InlineData(HealthStatus.Degraded)]
    [InlineData(HealthStatus.Unhealthy)]
    public async Task CheckHealthAsync_Should_Support_All_Health_Status_Values(HealthStatus expectedStatus)
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        mockHealthCheckable.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(expectedStatus);

        // Act
        var result = await mockHealthCheckable.Object.CheckHealthAsync();

        // Assert
        result.Should().Be(expectedStatus);
    }

    /// <summary>
    /// 测试健康检查方法的重复调用一致性
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Consistent_Results()
    {
        // Arrange
        var mockHealthCheckable = new Mock<IHealthCheckable>();
        mockHealthCheckable.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(HealthStatus.Healthy);

        // Act
        var result1 = await mockHealthCheckable.Object.CheckHealthAsync();
        var result2 = await mockHealthCheckable.Object.CheckHealthAsync();
        var result3 = await mockHealthCheckable.Object.CheckHealthAsync();

        // Assert
        result1.Should().Be(HealthStatus.Healthy);
        result2.Should().Be(HealthStatus.Healthy);
        result3.Should().Be(HealthStatus.Healthy);
        mockHealthCheckable.Verify(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    /// <summary>
    /// 测试健康检查接口的多态性支持
    /// </summary>
    [Fact]
    public async Task IHealthCheckable_Should_Support_Polymorphism()
    {
        // Arrange
        var mockHealthCheckable1 = new Mock<IHealthCheckable>();
        var mockHealthCheckable2 = new Mock<IHealthCheckable>();

        mockHealthCheckable1.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(HealthStatus.Healthy);
        mockHealthCheckable2.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(HealthStatus.Degraded);

        var healthCheckables = new List<IHealthCheckable>
        {
            mockHealthCheckable1.Object,
            mockHealthCheckable2.Object
        };

        // Act
        var results = new List<HealthStatus>();
        foreach (var healthCheckable in healthCheckables)
        {
            results.Add(await healthCheckable.CheckHealthAsync());
        }

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(HealthStatus.Healthy);
        results.Should().Contain(HealthStatus.Degraded);
    }
}