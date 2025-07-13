namespace Lorn.ADSP.Core.AdEngine.Abstractions.Exceptions
{
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
}
