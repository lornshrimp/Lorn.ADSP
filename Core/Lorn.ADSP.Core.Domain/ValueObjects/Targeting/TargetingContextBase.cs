using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 定向上下文基础实现类
    /// 提供ITargetingContext接口的标准实现
    /// </summary>
    public class TargetingContextBase : ValueObject, ITargetingContext
    {
        private readonly Dictionary<string, object> _properties;

        /// <summary>
        /// 上下文类型标识
        /// </summary>
        public string ContextType { get; private set; }

        /// <summary>
        /// 上下文属性集合（只读）
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _properties.AsReadOnly();

        /// <summary>
        /// 上下文创建时间戳
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// 上下文的唯一标识
        /// </summary>
        public string ContextId { get; private set; }

        /// <summary>
        /// 上下文数据来源
        /// </summary>
        public string DataSource { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contextType">上下文类型</param>
        /// <param name="properties">属性集合</param>
        /// <param name="dataSource">数据来源</param>
        /// <param name="contextId">上下文标识</param>
        public TargetingContextBase(
            string contextType,
            IDictionary<string, object>? properties = null,
            string? dataSource = null,
            string? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("上下文类型不能为空", nameof(contextType));

            ContextType = contextType;
            _properties = properties?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, object>();
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 获取指定类型的属性值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值，如果不存在则返回默认值</returns>
        public virtual T? GetProperty<T>(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return default;

            if (_properties.TryGetValue(propertyKey, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                // 尝试类型转换
                try
                {
                    if (typeof(T) == typeof(string))
                        return (T)(object)value.ToString()!;

                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default;
                }
            }

            return default;
        }

        /// <summary>
        /// 获取指定类型的属性值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>属性值或默认值</returns>
        public virtual T GetProperty<T>(string propertyKey, T defaultValue)
        {
            var result = GetProperty<T>(propertyKey);
            return result != null ? result : defaultValue;
        }

        /// <summary>
        /// 获取属性值的字符串表示
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值的字符串表示，如果不存在则返回空字符串</returns>
        public virtual string GetPropertyAsString(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return string.Empty;

            if (_properties.TryGetValue(propertyKey, out var value))
            {
                return value?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 检查是否包含指定的属性
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>是否包含该属性</returns>
        public virtual bool HasProperty(string propertyKey)
        {
            return !string.IsNullOrEmpty(propertyKey) && _properties.ContainsKey(propertyKey);
        }

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        public virtual IReadOnlyCollection<string> GetPropertyKeys()
        {
            return _properties.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// 检查上下文数据是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public virtual bool IsValid()
        {
            // 基础验证：上下文类型不能为空，时间戳不能是默认值
            if (string.IsNullOrEmpty(ContextType))
                return false;

            if (Timestamp == default)
                return false;

            return true;
        }

        /// <summary>
        /// 检查上下文数据是否已过期
        /// </summary>
        /// <param name="maxAge">最大有效期</param>
        /// <returns>是否已过期</returns>
        public virtual bool IsExpired(TimeSpan maxAge)
        {
            return DateTime.UtcNow - Timestamp > maxAge;
        }

        /// <summary>
        /// 获取上下文的元数据信息
        /// </summary>
        /// <returns>元数据字典</returns>
        public virtual IReadOnlyDictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["ContextType"] = ContextType,
                ["ContextId"] = ContextId,
                ["DataSource"] = DataSource,
                ["Timestamp"] = Timestamp,
                ["PropertyCount"] = _properties.Count,
                ["Age"] = DateTime.UtcNow - Timestamp
            }.AsReadOnly();
        }

        /// <summary>
        /// 获取上下文的调试信息
        /// </summary>
        /// <returns>调试信息字符串</returns>
        public virtual string GetDebugInfo()
        {
            var propertyInfo = string.Join(", ", _properties.Keys.Take(5));
            if (_properties.Count > 5)
                propertyInfo += $" (and {_properties.Count - 5} more)";

            return $"Context[{ContextType}] Id:{ContextId} Source:{DataSource} " +
                   $"Properties:{_properties.Count} [{propertyInfo}] Age:{DateTime.UtcNow - Timestamp:hh\\:mm\\:ss}";
        }

        /// <summary>
        /// 创建上下文的轻量级副本
        /// </summary>
        /// <param name="includeKeys">要包含的属性键</param>
        /// <returns>轻量级上下文副本</returns>
        public virtual ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys)
        {
            var filteredProperties = new Dictionary<string, object>();
            
            foreach (var key in includeKeys)
            {
                if (_properties.TryGetValue(key, out var value))
                {
                    filteredProperties[key] = value;
                }
            }

            return new TargetingContextBase(
                ContextType + "_Lightweight",
                filteredProperties,
                DataSource,
                ContextId + "_Copy");
        }

        /// <summary>
        /// 合并另一个上下文的属性
        /// </summary>
        /// <param name="other">要合并的上下文</param>
        /// <param name="overwriteExisting">是否覆盖已存在的属性</param>
        /// <returns>合并后的新上下文</returns>
        public virtual ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var mergedProperties = new Dictionary<string, object>(_properties);

            foreach (var property in other.Properties)
            {
                if (overwriteExisting || !mergedProperties.ContainsKey(property.Key))
                {
                    mergedProperties[property.Key] = property.Value;
                }
            }

            var mergedContextType = $"{ContextType}_Merged_{other.ContextType}";
            var mergedDataSource = $"{DataSource},{other.DataSource}";
            var mergedContextId = $"{ContextId}_Merged_{other.ContextId}";

            return new TargetingContextBase(
                mergedContextType,
                mergedProperties,
                mergedDataSource,
                mergedContextId);
        }

        /// <summary>
        /// 添加或更新属性
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <param name="propertyValue">属性值</param>
        public virtual void SetProperty(string propertyKey, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("属性键不能为空", nameof(propertyKey));

            _properties[propertyKey] = propertyValue;
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>是否成功移除</returns>
        public virtual bool RemoveProperty(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return false;

            return _properties.Remove(propertyKey);
        }

        /// <summary>
        /// 批量设置属性
        /// </summary>
        /// <param name="properties">属性集合</param>
        /// <param name="overwriteExisting">是否覆盖已存在的属性</param>
        public virtual void SetProperties(IDictionary<string, object> properties, bool overwriteExisting = true)
        {
            if (properties == null)
                return;

            foreach (var property in properties)
            {
                if (overwriteExisting || !_properties.ContainsKey(property.Key))
                {
                    _properties[property.Key] = property.Value;
                }
            }
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContextType;
            yield return ContextId;
            yield return DataSource;

            // 按键排序的属性集合
            foreach (var property in _properties.OrderBy(kv => kv.Key))
            {
                yield return property.Key;
                yield return property.Value ?? string.Empty;
            }
        }
    }
}