

using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Shared.Entities
{
    /// <summary>
    /// 验证警告
    /// </summary>
    public sealed record ValidationWarning
    {
        /// <summary>
        /// 警告消息
        /// </summary>
        public required string Message { get; init; }

        /// <summary>
        /// 警告代码
        /// </summary>
        public string? Code { get; init; }

        /// <summary>
        /// 警告字段
        /// </summary>
        public string? Field { get; init; }

        /// <summary>
        /// 警告级别
        /// </summary>
        public ValidationSeverity Severity { get; init; } = ValidationSeverity.Warning;

        /// <summary>
        /// 警告的附加上下文信息
        /// </summary>
        public IReadOnlyDictionary<string, object>? Context { get; init; }

        /// <summary>
        /// 警告的时间戳
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValidationWarning()
        {
        }

        /// <summary>
        /// 使用消息创建验证警告
        /// </summary>
        /// <param name="message">警告消息</param>
        public ValidationWarning(string message)
        {
            Message = message;
        }

        /// <summary>
        /// 使用消息和代码创建验证警告
        /// </summary>
        /// <param name="message">警告消息</param>
        /// <param name="code">警告代码</param>
        public ValidationWarning(string message, string code)
        {
            Message = message;
            Code = code;
        }

        /// <summary>
        /// 创建性能警告
        /// </summary>
        /// <param name="message">警告消息</param>
        /// <param name="field">相关字段</param>
        /// <returns>验证警告</returns>
        public static ValidationWarning Performance(string message, string? field = null)
        {
            return new ValidationWarning
            {
                Message = message,
                Field = field,
                Code = "PERFORMANCE_WARNING",
                Severity = ValidationSeverity.Warning
            };
        }

        /// <summary>
        /// 创建弃用警告
        /// </summary>
        /// <param name="field">弃用的字段</param>
        /// <param name="alternative">替代方案</param>
        /// <returns>验证警告</returns>
        public static ValidationWarning Deprecated(string field, string? alternative = null)
        {
            var message = alternative != null
                ? $"{field} is deprecated. Use {alternative} instead"
                : $"{field} is deprecated";

            return new ValidationWarning
            {
                Message = message,
                Field = field,
                Code = "DEPRECATED_FIELD",
                Context = alternative != null
                    ? new Dictionary<string, object> { ["Alternative"] = alternative }.AsReadOnly()
                    : null
            };
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        public override string ToString()
        {
            var parts = new List<string> { "WARNING" };

            if (!string.IsNullOrEmpty(Code))
                parts.Add($"[{Code}]");

            parts.Add(Message);

            if (!string.IsNullOrEmpty(Field))
                parts.Add($"(Field: {Field})");

            return string.Join(" ", parts);
        }
    }
}
