namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions;

/// <summary>
/// �ص�δ�ҵ��쳣
/// </summary>
public class CallbackNotFoundException : Exception
{
    /// <summary>
    /// �ص�����
    /// </summary>
    public Type? CallbackType { get; }

    /// <summary>
    /// �ص�����
    /// </summary>
    public string? CallbackName { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(Type callbackType) 
        : base($"Callback of type '{callbackType.Name}' not found")
    {
        CallbackType = callbackType;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string callbackName, Type callbackType) 
        : base($"Callback named '{callbackName}' of type '{callbackType.Name}' not found")
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
    }
}

/// <summary>
/// ����ִ���쳣
/// </summary>
public class StrategyExecutionException : Exception
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// ִ��ID
    /// </summary>
    public string? ExecutionId { get; }

    /// <summary>
    /// �������
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyExecutionException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyExecutionException(string strategyId, string message) : base(message)
    {
        StrategyId = strategyId;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyExecutionException(string strategyId, string executionId, string message) : base(message)
    {
        StrategyId = strategyId;
        ExecutionId = executionId;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyExecutionException(string strategyId, string executionId, string errorCode, string message) 
        : base(message)
    {
        StrategyId = strategyId;
        ExecutionId = executionId;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// ���캯��
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
/// ���������쳣
/// </summary>
public class StrategyConfigurationException : Exception
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// ���ü�
    /// </summary>
    public string? ConfigurationKey { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyConfigurationException(string strategyId, string message) : base(message)
    {
        StrategyId = strategyId;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyConfigurationException(string strategyId, string configurationKey, string message) : base(message)
    {
        StrategyId = strategyId;
        ConfigurationKey = configurationKey;
    }
}

/// <summary>
/// ������֤�쳣
/// </summary>
public class StrategyValidationException : Exception
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// ��֤�����б�
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyValidationException(string message) : base(message)
    {
        ValidationErrors = Array.Empty<string>();
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyValidationException(string message, IReadOnlyList<string> validationErrors) : base(message)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyValidationException(string strategyId, string message, IReadOnlyList<string> validationErrors) : base(message)
    {
        StrategyId = strategyId;
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// �ص��쳣
/// </summary>
public class CallbackException : Exception
{
    /// <summary>
    /// �ص�����
    /// </summary>
    public Type? CallbackType { get; }

    /// <summary>
    /// �ص�����
    /// </summary>
    public string? CallbackName { get; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackException(Type callbackType, string operationName, string message) : base(message)
    {
        CallbackType = callbackType;
        OperationName = operationName;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackException(string callbackName, Type callbackType, string operationName, string message) : base(message)
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
        OperationName = operationName;
    }

    /// <summary>
    /// ���캯��
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
/// ���Գ�ʱ�쳣
/// </summary>
public class StrategyTimeoutException : StrategyExecutionException
{
    /// <summary>
    /// ��ʱʱ��
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// ʵ��ִ��ʱ��
    /// </summary>
    public TimeSpan ActualExecutionTime { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyTimeoutException(string strategyId, TimeSpan timeout, TimeSpan actualExecutionTime) 
        : base(strategyId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
    {
        Timeout = timeout;
        ActualExecutionTime = actualExecutionTime;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyTimeoutException(string strategyId, string executionId, TimeSpan timeout, TimeSpan actualExecutionTime) 
        : base(strategyId, executionId, $"Strategy '{strategyId}' execution timeout. Expected: {timeout}, Actual: {actualExecutionTime}")
    {
        Timeout = timeout;
        ActualExecutionTime = actualExecutionTime;
    }
}

/// <summary>
/// ���������쳣
/// </summary>
public class StrategyDependencyException : Exception
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string? StrategyId { get; }

    /// <summary>
    /// ����������
    /// </summary>
    public string? DependencyName { get; }

    /// <summary>
    /// ����������
    /// </summary>
    public Type? DependencyType { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyDependencyException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyDependencyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyDependencyException(string strategyId, string dependencyName, string message) : base(message)
    {
        StrategyId = strategyId;
        DependencyName = dependencyName;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public StrategyDependencyException(string strategyId, string dependencyName, Type dependencyType, string message) : base(message)
    {
        StrategyId = strategyId;
        DependencyName = dependencyName;
        DependencyType = dependencyType;
    }
}