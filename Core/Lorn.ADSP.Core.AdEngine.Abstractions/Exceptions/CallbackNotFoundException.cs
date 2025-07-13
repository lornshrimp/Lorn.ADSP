namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions;

/// <summary>
/// 回调未找到异常
/// </summary>
public class CallbackNotFoundException : Exception
{
    /// <summary>
    /// 回调类型
    /// </summary>
    public Type? CallbackType { get; }

    /// <summary>
    /// 回调名称
    /// </summary>
    public string? CallbackName { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackNotFoundException(Type callbackType)
        : base($"Callback of type '{callbackType.Name}' not found")
    {
        CallbackType = callbackType;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackNotFoundException(string callbackName, Type callbackType)
        : base($"Callback named '{callbackName}' of type '{callbackType.Name}' not found")
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
    }
}











