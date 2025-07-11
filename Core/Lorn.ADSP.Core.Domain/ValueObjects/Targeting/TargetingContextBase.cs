using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ���������Ļ���ʵ����
    /// �ṩITargetingContext�ӿڵı�׼ʵ��
    /// </summary>
    public class TargetingContextBase : ValueObject, ITargetingContext
    {
        private readonly Dictionary<string, object> _properties;

        /// <summary>
        /// ���������ͱ�ʶ
        /// </summary>
        public string ContextType { get; private set; }

        /// <summary>
        /// ���������Լ��ϣ�ֻ����
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _properties.AsReadOnly();

        /// <summary>
        /// �����Ĵ���ʱ���
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// �����ĵ�Ψһ��ʶ
        /// </summary>
        public string ContextId { get; private set; }

        /// <summary>
        /// ������������Դ
        /// </summary>
        public string DataSource { get; private set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="contextType">����������</param>
        /// <param name="properties">���Լ���</param>
        /// <param name="dataSource">������Դ</param>
        /// <param name="contextId">�����ı�ʶ</param>
        public TargetingContextBase(
            string contextType,
            IDictionary<string, object>? properties = null,
            string? dataSource = null,
            string? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("���������Ͳ���Ϊ��", nameof(contextType));

            ContextType = contextType;
            _properties = properties?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, object>();
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// ��ȡָ�����͵�����ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        public virtual T? GetProperty<T>(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return default;

            if (_properties.TryGetValue(propertyKey, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                // ��������ת��
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
        /// ��ȡָ�����͵�����ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        public virtual T GetProperty<T>(string propertyKey, T defaultValue)
        {
            var result = GetProperty<T>(propertyKey);
            return result != null ? result : defaultValue;
        }

        /// <summary>
        /// ��ȡ����ֵ���ַ�����ʾ
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ���ַ�����ʾ������������򷵻ؿ��ַ���</returns>
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
        /// ����Ƿ����ָ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ����������</returns>
        public virtual bool HasProperty(string propertyKey)
        {
            return !string.IsNullOrEmpty(propertyKey) && _properties.ContainsKey(propertyKey);
        }

        /// <summary>
        /// ��ȡ�������Լ�
        /// </summary>
        /// <returns>���Լ�����</returns>
        public virtual IReadOnlyCollection<string> GetPropertyKeys()
        {
            return _properties.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// ��������������Ƿ���Ч
        /// </summary>
        /// <returns>�Ƿ���Ч</returns>
        public virtual bool IsValid()
        {
            // ������֤�����������Ͳ���Ϊ�գ�ʱ���������Ĭ��ֵ
            if (string.IsNullOrEmpty(ContextType))
                return false;

            if (Timestamp == default)
                return false;

            return true;
        }

        /// <summary>
        /// ��������������Ƿ��ѹ���
        /// </summary>
        /// <param name="maxAge">�����Ч��</param>
        /// <returns>�Ƿ��ѹ���</returns>
        public virtual bool IsExpired(TimeSpan maxAge)
        {
            return DateTime.UtcNow - Timestamp > maxAge;
        }

        /// <summary>
        /// ��ȡ�����ĵ�Ԫ������Ϣ
        /// </summary>
        /// <returns>Ԫ�����ֵ�</returns>
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
        /// ��ȡ�����ĵĵ�����Ϣ
        /// </summary>
        /// <returns>������Ϣ�ַ���</returns>
        public virtual string GetDebugInfo()
        {
            var propertyInfo = string.Join(", ", _properties.Keys.Take(5));
            if (_properties.Count > 5)
                propertyInfo += $" (and {_properties.Count - 5} more)";

            return $"Context[{ContextType}] Id:{ContextId} Source:{DataSource} " +
                   $"Properties:{_properties.Count} [{propertyInfo}] Age:{DateTime.UtcNow - Timestamp:hh\\:mm\\:ss}";
        }

        /// <summary>
        /// ���������ĵ�����������
        /// </summary>
        /// <param name="includeKeys">Ҫ���������Լ�</param>
        /// <returns>�����������ĸ���</returns>
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
        /// �ϲ���һ�������ĵ�����
        /// </summary>
        /// <param name="other">Ҫ�ϲ���������</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
        /// <returns>�ϲ������������</returns>
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
        /// ��ӻ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="propertyValue">����ֵ</param>
        public virtual void SetProperty(string propertyKey, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("���Լ�����Ϊ��", nameof(propertyKey));

            _properties[propertyKey] = propertyValue;
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ�ɹ��Ƴ�</returns>
        public virtual bool RemoveProperty(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return false;

            return _properties.Remove(propertyKey);
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="properties">���Լ���</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
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
        /// ��ȡ����ԱȽϵ����
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContextType;
            yield return ContextId;
            yield return DataSource;

            // ������������Լ���
            foreach (var property in _properties.OrderBy(kv => kv.Key))
            {
                yield return property.Key;
                yield return property.Value ?? string.Empty;
            }
        }
    }
}