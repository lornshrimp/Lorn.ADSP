using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// 上下文请求基础实现类
/// </summary>
/// <typeparam name="T">上下文类型</typeparam>
public abstract class ContextRequestBase<T> : IContextRequest<T> where T : class, ITargetingContext
{
    private readonly Dictionary<string, object> _parameters = new();

    /// <summary>
    /// 上下文类型标识
    /// </summary>
    public abstract string ContextType { get; }

    /// <summary>
    /// 请求参数字典
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// 请求超时时间
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 缓存策略配置
    /// </summary>
    public CachePolicy CachePolicy { get; init; } = CachePolicy.Default;

    /// <summary>
    /// 请求唯一标识符
    /// </summary>
    public string RequestId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 请求优先级
    /// </summary>
    public RequestPriority Priority { get; init; } = RequestPriority.Normal;

    /// <summary>
    /// 是否允许使用缓存数据
    /// </summary>
    public bool AllowCached { get; init; } = true;

    /// <summary>
    /// 最大可接受的数据延迟时间
    /// </summary>
    public TimeSpan MaxDataAge { get; init; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// 构造函数
    /// </summary>
    protected ContextRequestBase()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="parameters">初始参数</param>
    protected ContextRequestBase(IReadOnlyDictionary<string, object>? parameters)
    {
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                _parameters[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// 获取强类型的参数值
    /// </summary>
    public virtual TValue? GetParameter<TValue>(string key)
    {
        if (!_parameters.TryGetValue(key, out var value))
            return default;

        if (value is TValue typedValue)
            return typedValue;

        // 尝试类型转换
        try
        {
            return (TValue?)Convert.ChangeType(value, typeof(TValue));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 检查是否包含指定的参数
    /// </summary>
    public virtual bool HasParameter(string key)
    {
        return _parameters.ContainsKey(key);
    }

    /// <summary>
    /// 设置参数值
    /// </summary>
    protected void SetParameter(string key, object value)
    {
        _parameters[key] = value;
    }

    /// <summary>
    /// 批量设置参数
    /// </summary>
    protected void SetParameters(IReadOnlyDictionary<string, object> parameters)
    {
        foreach (var kvp in parameters)
        {
            _parameters[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// 验证请求参数的有效性
    /// </summary>
    public virtual ValidationResult Validate()
    {
        var errors = new List<ValidationError>();

        // 验证基本参数
        if (string.IsNullOrWhiteSpace(ContextType))
        {
            errors.Add(new ValidationError
            {
                Message = "ContextType cannot be null or empty",
                Code = "CONTEXT_TYPE_REQUIRED",
                Field = nameof(ContextType)
            });
        }

        if (string.IsNullOrWhiteSpace(RequestId))
        {
            errors.Add(new ValidationError
            {
                Message = "RequestId cannot be null or empty",
                Code = "REQUEST_ID_REQUIRED",
                Field = nameof(RequestId)
            });
        }

        if (Timeout <= TimeSpan.Zero)
        {
            errors.Add(new ValidationError
            {
                Message = "Timeout must be greater than zero",
                Code = "INVALID_TIMEOUT",
                Field = nameof(Timeout),
                AttemptedValue = Timeout
            });
        }

        if (MaxDataAge < TimeSpan.Zero)
        {
            errors.Add(new ValidationError
            {
                Message = "MaxDataAge cannot be negative",
                Code = "INVALID_MAX_DATA_AGE",
                Field = nameof(MaxDataAge),
                AttemptedValue = MaxDataAge
            });
        }

        // 调用子类的自定义验证
        ValidateSpecific(errors);

        return errors.Count == 0
            ? ValidationResult.Success("Request validation passed")
            : ValidationResult.Failure(errors, "Request validation failed");
    }

    /// <summary>
    /// 子类特定的验证逻辑
    /// </summary>
    protected virtual void ValidateSpecific(List<ValidationError> errors)
    {
        // 子类可以重写此方法来添加特定的验证逻辑
    }

    /// <summary>
    /// 获取请求摘要信息
    /// </summary>
    public virtual string GetSummary()
    {
        return $"{ContextType} request {RequestId} with {_parameters.Count} parameters";
    }

    /// <summary>
    /// 克隆请求对象
    /// </summary>
    public virtual IContextRequest<T> Clone()
    {
        var clone = (ContextRequestBase<T>)MemberwiseClone();
        clone._parameters.Clear();
        foreach (var kvp in _parameters)
        {
            clone._parameters[kvp.Key] = kvp.Value;
        }
        return clone;
    }
}