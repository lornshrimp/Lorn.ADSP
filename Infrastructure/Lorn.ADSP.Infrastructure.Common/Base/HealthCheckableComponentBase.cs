using Lorn.ADSP.Infrastructure.Common.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.Common.Base;

/// <summary>
/// 可进行健康检查的组件基类
/// 提供健康检查的基础实现
/// </summary>
public abstract class HealthCheckableComponentBase : ComponentBase, IHealthCheckable
{
    /// <summary>
    /// 执行健康检查
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态</returns>
    public virtual async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 检查组件是否已初始化
            if (!IsInitialized)
                return HealthStatus.Unhealthy;

            // 执行具体的健康检查逻辑
            return await OnCheckHealthAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 超时/取消应向上传播，便于调用方区分真正的取消场景
            throw;
        }
        catch (Exception)
        {
            return HealthStatus.Unhealthy;
        }
    }

    /// <summary>
    /// 具体的健康检查实现，由子类重写
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态</returns>
    protected virtual Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthStatus.Healthy);
    }
}