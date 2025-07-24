using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎上下文回调接口 - 支持泛型化的上下文获取机制
/// </summary>
public interface IAdEngineContextCallback : IAdEngineCallback
{
    /// <summary>
    /// 泛型方式获取上下文信息
    /// </summary>
    /// <typeparam name="T">上下文类型，必须实现ITargetingContext接口</typeparam>
    /// <param name="request">上下文请求对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定类型的上下文信息</returns>
    Task<T> GetContextAsync<T>(IContextRequest<T> request, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// 字典参数方式获取上下文信息
    /// </summary>
    /// <typeparam name="T">上下文类型，必须实现ITargetingContext接口</typeparam>
    /// <param name="contextType">上下文类型标识</param>
    /// <param name="parameters">请求参数字典</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定类型的上下文信息</returns>
    Task<T> GetContextAsync<T>(string contextType, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// 批量获取上下文信息
    /// </summary>
    /// <typeparam name="T">上下文类型，必须实现ITargetingContext接口</typeparam>
    /// <param name="requests">上下文请求列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>上下文信息列表</returns>
    Task<IReadOnlyList<T>> GetBatchContextAsync<T>(IReadOnlyList<IContextRequest<T>> requests, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// 检查是否支持指定的上下文类型（泛型方式）
    /// </summary>
    /// <typeparam name="T">上下文类型</typeparam>
    /// <returns>如果支持返回true，否则返回false</returns>
    bool IsContextTypeSupported<T>() where T : class, ITargetingContext;

    /// <summary>
    /// 检查是否支持指定的上下文类型（字符串方式）
    /// </summary>
    /// <param name="contextType">上下文类型标识</param>
    /// <returns>如果支持返回true，否则返回false</returns>
    bool IsContextTypeSupported(string contextType);

    /// <summary>
    /// 获取所有支持的上下文类型列表
    /// </summary>
    /// <returns>支持的上下文类型标识列表</returns>
    IReadOnlyList<string> GetSupportedContextTypes();

    /// <summary>
    /// 尝试获取上下文信息，不抛出异常
    /// </summary>
    /// <typeparam name="T">上下文类型</typeparam>
    /// <param name="request">上下文请求对象</param>
    /// <param name="context">输出的上下文信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果获取成功返回true，否则返回false</returns>
    Task<(bool Success, T? Context)> TryGetContextAsync<T>(IContextRequest<T> request, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;
}