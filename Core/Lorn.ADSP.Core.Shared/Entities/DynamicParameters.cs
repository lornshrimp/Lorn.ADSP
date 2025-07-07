namespace Lorn.ADSP.Core.Shared.Entities
{
    /// <summary>
    /// 动态参数
    /// </summary>
    public record DynamicParameters
    {
        /// <summary>
        /// 配置节名称
        /// </summary>
        public required string Section { get; init; }

        /// <summary>
        /// 参数值
        /// </summary>
        public required IReadOnlyDictionary<string, object> Parameters { get; init; }

        /// <summary>
        /// 参数版本
        /// </summary>
        public string? Version { get; init; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiresAt { get; init; }

        /// <summary>
        /// 参数来源
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

        /// <summary>
        /// 获取参数值
        /// </summary>
        public T GetParameter<T>(string key, T defaultValue = default!)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 尝试获取参数值
        /// </summary>
        public bool TryGetParameter<T>(string key, out T? value)
        {
            value = default;
            if (Parameters.TryGetValue(key, out var objValue))
            {
                try
                {
                    value = (T)Convert.ChangeType(objValue, typeof(T));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
