using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.Common.Abstractions;

/// <summary>
/// 定义可进行健康检查的组件接口
/// 实现此接口的组件将自动被添加到健康检查系统中
/// </summary>
public interface IHealthCheckable
{
    /// <summary>
    /// 执行健康检查
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态</returns>
    Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default);
}