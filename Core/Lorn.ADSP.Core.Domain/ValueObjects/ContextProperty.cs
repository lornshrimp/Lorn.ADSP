using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 上下文属性值对象
    /// [需要存储] - 作为TargetingContext的组成部分存储
    /// 用于替代ITargetingContext中的Properties字典
    /// </summary>
    public class ContextProperty : ValueObject
    {
        /// <summary>
        /// 属性键
        /// </summary>
        public string PropertyKey { get; }

        /// <summary>
        /// 属性值（JSON格式存储复杂对象）
        /// </summary>
        public string PropertyValue { get; }

        /// <summary>
        /// 属性数据类型
        /// </summary>
        public string DataType { get; }

        /// <summary>
        /// 属性分类（用于分组和检索优化）
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// 是否敏感数据
        /// </summary>
        public bool IsSensitive { get; }

        /// <summary>
        /// 属性权重（用于重要性排序）
        /// </summary>
        public decimal Weight { get; }

        /// <summary>
        /// 属性过期时间（可选）
        /// </summary>
        public DateTime? ExpiresAt { get; }

        /// <summary>
        /// 数据来源
        /// </summary>
        public string DataSource { get; }

        /// <summary>
        /// 私有构造函数（用于序列化）
        /// </summary>
        private ContextProperty()
        {
            PropertyKey = string.Empty;
            PropertyValue = string.Empty;
            DataType = "String";
            Category = string.Empty;
            DataSource = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContextProperty(
            string propertyKey,
            string propertyValue,
            string dataType = "String",
            string category = "",
            bool isSensitive = false,
            decimal weight = 1.0m,
            DateTime? expiresAt = null,
            string dataSource = "")
        {
            if (string.IsNullOrWhiteSpace(propertyKey))
                throw new ArgumentException("属性键不能为空", nameof(propertyKey));

            PropertyKey = propertyKey;
            PropertyValue = propertyValue ?? string.Empty;
            DataType = dataType;
            Category = category;
            IsSensitive = isSensitive;
            Weight = weight;
            ExpiresAt = expiresAt;
            DataSource = dataSource;
        }

        /// <summary>
        /// 获取强类型的属性值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>转换后的属性值</returns>
        public T? GetValue<T>()
        {
            if (string.IsNullOrEmpty(PropertyValue))
                return default;

            try
            {
                var targetType = typeof(T);

                // Direct string
                if (targetType == typeof(string))
                    return (T)(object)PropertyValue;

                // Nullable<TUnderlying>
                var isNullable = Nullable.GetUnderlyingType(targetType) != null;
                var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // JSON payloads
                if (DataType == "Json")
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(PropertyValue);
                }

                object? converted = null;

                // Enums
                if (underlying.IsEnum)
                {
                    if (int.TryParse(PropertyValue, out var enumInt))
                    {
                        converted = Enum.ToObject(underlying, enumInt);
                    }
                    else
                    {
                        converted = Enum.Parse(underlying, PropertyValue, true);
                    }
                }
                // Guid
                else if (underlying == typeof(Guid))
                {
                    if (Guid.TryParse(PropertyValue, out var guid))
                        converted = guid;
                }
                // DateTime / DateTimeOffset with invariant culture and round-trip when possible
                else if (underlying == typeof(DateTime))
                {
                    if (DateTime.TryParse(PropertyValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                        converted = dt;
                }
                else if (underlying == typeof(DateTimeOffset))
                {
                    if (DateTimeOffset.TryParse(PropertyValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var dto))
                        converted = dto;
                }
                // Numerics with invariant culture (important for decimal, double, etc.)
                else if (underlying == typeof(decimal))
                {
                    if (decimal.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dec))
                        converted = dec;
                }
                else if (underlying == typeof(double))
                {
                    if (double.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                        converted = dbl;
                }
                else if (underlying == typeof(float))
                {
                    if (float.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var fl))
                        converted = fl;
                }
                else if (underlying == typeof(long))
                {
                    if (long.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var l))
                        converted = l;
                }
                else if (underlying == typeof(int))
                {
                    if (int.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i))
                        converted = i;
                }
                else if (underlying == typeof(short))
                {
                    if (short.TryParse(PropertyValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var s))
                        converted = s;
                }
                else if (underlying == typeof(bool))
                {
                    if (bool.TryParse(PropertyValue, out var b))
                        converted = b;
                }
                else
                {
                    // Fallback to Convert.ChangeType with invariant culture
                    converted = Convert.ChangeType(PropertyValue, underlying, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (converted == null)
                    return default;

                // If T is nullable, box into Nullable<TUnderlying>
                if (isNullable)
                {
                    return (T)converted;
                }

                return (T)converted;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 创建具有新值的ContextProperty实例（值对象不可变）
        /// </summary>
        /// <param name="newValue">新的属性值</param>
        /// <returns>新的ContextProperty实例</returns>
        public ContextProperty WithValue(object newValue)
        {
            if (newValue == null)
            {
                return new ContextProperty(PropertyKey, string.Empty, "String", Category, IsSensitive, Weight, ExpiresAt, DataSource);
            }

            string propertyValue;
            string dataType;

            if (newValue is string stringValue)
            {
                propertyValue = stringValue;
                dataType = "String";
            }
            else if (newValue.GetType().IsPrimitive || newValue is decimal || newValue is DateTime)
            {
                propertyValue = newValue.ToString() ?? string.Empty;
                dataType = newValue.GetType().Name;
            }
            else
            {
                propertyValue = System.Text.Json.JsonSerializer.Serialize(newValue);
                dataType = "Json";
            }

            return new ContextProperty(PropertyKey, propertyValue, dataType, Category, IsSensitive, Weight, ExpiresAt, DataSource);
        }

        /// <summary>
        /// 检查属性是否已过期
        /// </summary>
        /// <returns>是否已过期</returns>
        public bool IsExpired()
        {
            return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
        }

        /// <summary>
        /// 创建属性副本
        /// </summary>
        /// <returns>属性副本</returns>
        public ContextProperty Clone()
        {
            return new ContextProperty(PropertyKey, PropertyValue, DataType, Category, IsSensitive, Weight, ExpiresAt, DataSource);
        }

        /// <summary>
        /// 获取相等性比较组件
        /// </summary>
        /// <returns>用于相等性比较的组件</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PropertyKey;
            yield return PropertyValue;
            yield return DataType;
            yield return Category;
            yield return IsSensitive;
            yield return Weight;
            yield return ExpiresAt ?? (object)"null";
            yield return DataSource;
        }
    }
}
