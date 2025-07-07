namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 回调提供者接口，用于提供类型安全的回调访问机制
/// </summary>
public interface ICallbackProvider
{
    /// <summary>
    /// 根据类型获取回调接口（类型安全）
    /// </summary>
    /// <typeparam name="T">回调接口类型</typeparam>
    /// <returns>指定类型的回调接口</returns>
    /// <exception cref="CallbackNotFoundException">当回调不存在时抛出</exception>
    T GetCallback<T>() where T : class, IAdEngineCallback;

    /// <summary>
    /// 根据名称和类型获取回调接口
    /// </summary>
    /// <typeparam name="T">回调接口类型</typeparam>
    /// <param name="name">回调名称</param>
    /// <returns>指定类型的回调接口</returns>
    /// <exception cref="CallbackNotFoundException">当回调不存在时抛出</exception>
    T GetCallback<T>(string name) where T : class, IAdEngineCallback;

    /// <summary>
    /// 检查是否存在指定类型的回调
    /// </summary>
    /// <typeparam name="T">回调接口类型</typeparam>
    /// <returns>如果存在则返回true，否则返回false</returns>
    bool HasCallback<T>() where T : class, IAdEngineCallback;

    /// <summary>
    /// 检查是否存在指定名称的回调
    /// </summary>
    /// <param name="callbackName">回调名称</param>
    /// <returns>如果存在则返回true，否则返回false</returns>
    bool HasCallback(string callbackName);

    /// <summary>
    /// 获取所有可用的回调接口
    /// </summary>
    /// <returns>回调名称和接口的只读字典</returns>
    IReadOnlyDictionary<string, IAdEngineCallback> GetAllCallbacks();

    /// <summary>
    /// 尝试获取回调接口（不抛异常）
    /// </summary>
    /// <typeparam name="T">回调接口类型</typeparam>
    /// <param name="callback">输出的回调接口</param>
    /// <returns>如果获取成功则返回true，否则返回false</returns>
    bool TryGetCallback<T>(out T? callback) where T : class, IAdEngineCallback;

    /// <summary>
    /// 尝试根据名称获取回调接口（不抛异常）
    /// </summary>
    /// <typeparam name="T">回调接口类型</typeparam>
    /// <param name="name">回调名称</param>
    /// <param name="callback">输出的回调接口</param>
    /// <returns>如果获取成功则返回true，否则返回false</returns>
    bool TryGetCallback<T>(string name, out T? callback) where T : class, IAdEngineCallback;
}