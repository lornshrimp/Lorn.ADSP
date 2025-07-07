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

/// <summary>
/// 策略执行异常
/// </summary>
public class StrategyExecutionException : Exception
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// 执行ID
    /// </summary>
    public string? ExecutionId { get; }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string strategyId, string message) : base(message)
    {
        StrategyId = strategyId;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string strategyId, string executionId, string message) : base(message)
    {
        StrategyId = strategyId;
        ExecutionId = executionId;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string strategyId, string executionId, string errorCode, string message) 
        : base(message)
    {
        StrategyId = strategyId;
        ExecutionId = executionId;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyExecutionException(string strategyId, string executionId, string errorCode, string message, Exception innerException) 
        : base(message, innerException)
    {
        StrategyId = strategyId;
        ExecutionId = executionId;
        ErrorCode = errorCode;
    }
}

/// <summary>
/// 策略配置异常
/// </summary>
public class StrategyConfigurationException : Exception
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// 配置键
    /// </summary>
    public string? ConfigurationKey { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyConfigurationException(string strategyId, string message) : base(message)
    {
        StrategyId = strategyId;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyConfigurationException(string strategyId, string configurationKey, string message) : base(message)
    {
        StrategyId = strategyId;
        ConfigurationKey = configurationKey;
    }
}

/// <summary>
/// 策略验证异常
/// </summary>
public class StrategyValidationException : Exception
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// 验证错误列表
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyValidationException(string message) : base(message)
    {
        ValidationErrors = Array.Empty<string>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyValidationException(string message, IReadOnlyList<string> validationErrors) : base(message)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyValidationException(string strategyId, string message, IReadOnlyList<string> validationErrors) : base(message)
    {
        StrategyId = strategyId;
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// 回调异常
/// </summary>
public class CallbackException : Exception
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
    /// 操作名称
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackException(Type callbackType, string operationName, string message) : base(message)
    {
        CallbackType = callbackType;
        OperationName = operationName;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackException(string callbackName, Type callbackType, string operationName, string message) : base(message)
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
        OperationName = operationName;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CallbackException(string callbackName, Type callbackType, string operationName, string message, Exception innerException) 
        : base(message, innerException)
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
        OperationName = operationName;
    }
}

/// <summary>
/// 策略超时异常
/// </summary>
public class StrategyTimeoutException : StrategyExecutionException
{
    /// <summary>
    /// 超时时间
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// 实际执行时间
    /// </summary>
    public TimeSpan ActualExecutionTime { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyTimeoutException(string strategyId, TimeSpan timeout, TimeSpan actualExecutionTime) 
        : base(strategyId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
    {
        Timeout = timeout;
        ActualExecutionTime = actualExecutionTime;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyTimeoutException(string strategyId, string executionId, TimeSpan timeout, TimeSpan actualExecutionTime) 
        : base(strategyId, executionId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
    {
        Timeout = timeout;
        ActualExecutionTime = actualExecutionTime;
    }
}

/// <summary>
/// 策略依赖异常
/// </summary>
public class StrategyDependencyException : Exception
{
    /// <summary>
    /// 策略ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// 依赖项名称
    /// </summary>
    public string? DependencyName { get; }

    /// <summary>
    /// 依赖项类型
    /// </summary>
    public Type? DependencyType { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyDependencyException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyDependencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyDependencyException(string strategyId, string dependencyName, string message) : base(message)
    {
        StrategyId = strategyId;
        DependencyName = dependencyName;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyDependencyException(string strategyId, string dependencyName, Type dependencyType, string message) : base(message)
    {
        StrategyId = strategyId;
        DependencyName = dependencyName;
        DependencyType = dependencyType;
    }
}