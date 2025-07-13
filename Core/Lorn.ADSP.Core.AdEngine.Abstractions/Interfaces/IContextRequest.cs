using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 上下文请求接口，用于封装获取特定类型上下文所需的参数
/// </summary>
/// <typeparam name="T">请求的上下文类型，必须实现ITargetingContext接口</typeparam>
public interface IContextRequest<T> where T : class, ITargetingContext
{
    /// <summary>
    /// 上下文类型标识
    /// </summary>
    string ContextType { get; }

    /// <summary>
    /// 请求参数字典
    /// </summary>
    IReadOnlyDictionary<string, object> Parameters { get; }

    /// <summary>
    /// 请求超时时间
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// 缓存策略配置
    /// </summary>
    CachePolicy CachePolicy { get; }

    /// <summary>
    /// 请求唯一标识符
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// 请求优先级
    /// </summary>
    RequestPriority Priority { get; }

    /// <summary>
    /// 是否允许使用缓存数据
    /// </summary>
    bool AllowCached { get; }

    /// <summary>
    /// 最大可接受的数据延迟时间
    /// </summary>
    TimeSpan MaxDataAge { get; }

    /// <summary>
    /// 获取强类型的参数值
    /// </summary>
    /// <typeparam name="TValue">参数值类型</typeparam>
    /// <param name="key">参数键</param>
    /// <returns>参数值</returns>
    TValue? GetParameter<TValue>(string key);

    /// <summary>
    /// 检查是否包含指定的参数
    /// </summary>
    /// <param name="key">参数键</param>
    /// <returns>如果包含返回true，否则返回false</returns>
    bool HasParameter(string key);

    /// <summary>
    /// 验证请求参数的有效性
    /// </summary>
    /// <returns>验证结果</returns>
    ValidationResult Validate();
}