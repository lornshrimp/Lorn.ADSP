namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎回调接口基类
/// </summary>
public interface IAdEngineCallback
{
    /// <summary>
    /// 回调类型标识
    /// </summary>
    string CallbackType { get; }

    /// <summary>
    /// 回调名称
    /// </summary>
    string CallbackName { get; }

    /// <summary>
    /// 是否可用
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// 健康检查
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康检查结果</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}