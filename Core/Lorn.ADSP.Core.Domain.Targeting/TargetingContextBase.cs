using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 定向上下文基础实现类
    /// 提供ITargetingContext接口的标准实现
    /// </summary>
    public class TargetingContextBase : ValueObject, ITargetingContext
    {
        private readonly List<ContextProperty> _properties;

        /// <summary>
        /// 上下文名称
        /// </summary>
        public virtual string ContextName { get; protected set; }

        /// <summary>
        /// 上下文类型标识
        /// </summary>
        public string ContextType { get; protected set; }

        /// <summary>
        /// 上下文属性集合（只读）
        /// </summary>
        public IReadOnlyList<ContextProperty> Properties => _properties.AsReadOnly();

        /// <summary>
        /// 上下文创建时间戳
        /// </summary>
        public DateTime Timestamp { get; protected set; }

        /// <summary>
        /// 上下文的唯一标识
        /// </summary>
        public Guid ContextId { get; protected set; }

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
            IEnumerable<ContextProperty>? properties = null,
            string? dataSource = null,
            Guid? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("上下文类型不能为空", nameof(contextType));

            ContextType = contextType;
            ContextName = contextType; // 默认使用类型作为名称
            _properties = properties?.ToList() ?? new List<ContextProperty>();
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.CreateVersion7();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// 从字典创建的构造函数（兼容性）
        /// </summary>
        /// <param name="contextType">上下文类型</param>
        /// <param name="properties">属性字典</param>
        /// <param name="dataSource">数据来源</param>
        /// <param name="contextId">上下文标识</param>
        public TargetingContextBase(
            string contextType,
            IDictionary<string, object>? properties,
            string? dataSource = null,
            Guid? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("上下文类型不能为空", nameof(contextType));

            ContextType = contextType;
            ContextName = contextType; // 默认使用类型作为名称
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.CreateVersion7();
            Timestamp = DateTime.UtcNow;

            _properties = new List<ContextProperty>();
            if (properties != null)
            {
                foreach (var kvp in properties)
                {
                    string valueStr;
                    string dataType;

                    if (kvp.Value is null)
                    {
                        valueStr = string.Empty;
                        dataType = "String";
                    }
                    else
                    {
                        var t = kvp.Value.GetType();
                        // Simple primitives we can store as plain strings
                        if (t.IsPrimitive || kvp.Value is decimal || kvp.Value is DateTime || kvp.Value is DateTimeOffset || kvp.Value is Guid || kvp.Value is string)
                        {
                            if (kvp.Value is IFormattable formattable && kvp.Value is not string)
                            {
                                valueStr = formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                valueStr = kvp.Value.ToString() ?? string.Empty;
                            }

                            dataType = t == typeof(string) ? "String" : t.Name;
                        }
                        else
                        {
                            // Complex types / collections -> JSON
                            valueStr = System.Text.Json.JsonSerializer.Serialize(kvp.Value);
                            dataType = "Json";
                        }
                    }

                    var property = new ContextProperty(kvp.Key, valueStr, dataType, category: ContextType, isSensitive: false, weight: 1.0m, expiresAt: null, dataSource: DataSource);
                    _properties.Add(property);
                }
            }
        }

        /// <summary>
        /// 获取指定键的属性实体
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性实体，如果不存在则返回null</returns>
        public virtual ContextProperty? GetProperty(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return null;

            return _properties.FirstOrDefault(p => p.PropertyKey == propertyKey);
        }

        /// <summary>
        /// 获取指定类型的属性值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值，如果不存在则返回默认值</returns>
        public virtual T? GetPropertyValue<T>(string propertyKey)
        {
            var property = GetProperty(propertyKey);
            return property == null ? default(T) : property.GetValue<T>();
        }

        /// <summary>
        /// 获取指定类型的属性值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>属性值或默认值</returns>
        public virtual T GetPropertyValue<T>(string propertyKey, T defaultValue)
        {
            var result = GetPropertyValue<T>(propertyKey);
            return result != null ? result : defaultValue;
        }

        /// <summary>
        /// 获取属性值的字符串表示
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值的字符串表示，如果不存在则返回空字符串</returns>
        public virtual string GetPropertyAsString(string propertyKey)
        {
            var property = GetProperty(propertyKey);
            return property?.PropertyValue ?? string.Empty;
        }

        /// <summary>
        /// 检查是否包含指定的属性
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>是否包含该属性</returns>
        public virtual bool HasProperty(string propertyKey)
        {
            return GetProperty(propertyKey) != null;
        }

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        public virtual IReadOnlyCollection<string> GetPropertyKeys()
        {
            return _properties.Select(p => p.PropertyKey).ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取指定分类的属性
        /// </summary>
        /// <param name="category">属性分类</param>
        /// <returns>属性集合</returns>
        public virtual IReadOnlyList<ContextProperty> GetPropertiesByCategory(string category)
        {
            return _properties.Where(p => p.Category == category).ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取未过期的属性
        /// </summary>
        /// <returns>未过期的属性集合</returns>
        public virtual IReadOnlyList<ContextProperty> GetActiveProperties()
        {
            return _properties.Where(p => !p.IsExpired()).ToList().AsReadOnly();
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
        /// <returns>元数据属性集合</returns>
        public virtual IReadOnlyList<ContextProperty> GetMetadata()
        {
            var metadataProperties = new List<ContextProperty>
            {
                new ContextProperty("ContextType", ContextType),
                new ContextProperty("ContextId", ContextId.ToString()),
                new ContextProperty("DataSource", DataSource),
                new ContextProperty("Timestamp", Timestamp.ToString("O")),
                new ContextProperty("PropertyCount", _properties.Count.ToString()),
                new ContextProperty("Age", (DateTime.UtcNow - Timestamp).ToString())
            };

            return metadataProperties.AsReadOnly();
        }

        /// <summary>
        /// 获取上下文的调试信息
        /// </summary>
        /// <returns>调试信息字符串</returns>
        public virtual string GetDebugInfo()
        {
            var propertyInfo = string.Join(", ", _properties.Take(5).Select(p => p.PropertyKey));
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
            var filteredProperties = _properties.Where(p => includeKeys.Contains(p.PropertyKey))
                                               .Select(p => p.Clone())
                                               .ToList();

            return new TargetingContextBase(
                ContextType + "_Lightweight",
                filteredProperties,
                DataSource,
                new Guid(ContextId.ToByteArray()));
        }

        /// <summary>
        /// 创建上下文的分类副本
        /// </summary>
        /// <param name="categories">要包含的属性分类</param>
        /// <returns>分类上下文副本</returns>
        public virtual ITargetingContext CreateCategorizedCopy(IEnumerable<string> categories)
        {
            var filteredProperties = _properties.Where(p => categories.Contains(p.Category))
                                               .Select(p => p.Clone())
                                               .ToList();

            return new TargetingContextBase(
                ContextType + "_Categorized",
                filteredProperties,
                DataSource,
                ContextId);
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

            var mergedProperties = new List<ContextProperty>(_properties);
            var mergedContextId = $"{ContextId}_Merged_{other.ContextId}";
            var mergedGuid = Guid.NewGuid();

            foreach (var property in other.Properties)
            {
                var existingProperty = mergedProperties.FirstOrDefault(p => p.PropertyKey == property.PropertyKey);

                if (existingProperty != null && overwriteExisting)
                {
                    mergedProperties.Remove(existingProperty);
                    mergedProperties.Add(property.Clone());
                }
                else if (existingProperty == null)
                {
                    mergedProperties.Add(property.Clone());
                }
            }

            var mergedContextType = $"{ContextType}_Merged_{other.ContextType}";
            var mergedDataSource = $"{DataSource},{other.DataSource}";

            return new TargetingContextBase(
                mergedContextType,
                mergedProperties,
                mergedDataSource,
                new Guid(mergedGuid.ToByteArray()));
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

            var existingProperty = GetProperty(propertyKey);
            if (existingProperty != null)
            {
                _properties.Remove(existingProperty);
                var updatedProperty = existingProperty.WithValue(propertyValue ?? string.Empty);
                _properties.Add(updatedProperty);
            }
            else
            {
                var newProperty = new ContextProperty(propertyKey, propertyValue?.ToString() ?? string.Empty);
                _properties.Add(newProperty);
            }
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

            var property = GetProperty(propertyKey);
            if (property != null)
            {
                return _properties.Remove(property);
            }
            return false;
        }

        /// <summary>
        /// 批量设置属性
        /// </summary>
        /// <param name="properties">属性字典</param>
        /// <param name="overwriteExisting">是否覆盖已存在的属性</param>
        public virtual void SetProperties(IDictionary<string, object> properties, bool overwriteExisting = true)
        {
            if (properties == null)
                return;

            foreach (var kvp in properties)
            {
                if (overwriteExisting || !HasProperty(kvp.Key))
                {
                    SetProperty(kvp.Key, kvp.Value);
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
            foreach (var property in _properties.OrderBy(p => p.PropertyKey))
            {
                yield return property.PropertyKey;
                yield return property.PropertyValue ?? string.Empty;
            }
        }
    }
}