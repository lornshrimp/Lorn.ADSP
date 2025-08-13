using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 定向规则值对象
    /// 用于替代ITargetingCriteria中的Rules字典
    /// </summary>
    public class TargetingRule : ValueObject
    {
        /// <summary>
        /// 规则键
        /// </summary>
        public string RuleKey { get; }

        /// <summary>
        /// 规则值（JSON格式存储复杂对象）
        /// </summary>
        public string RuleValue { get; }

        /// <summary>
        /// 规则数据类型
        /// </summary>
        public string DataType { get; }

        /// <summary>
        /// 规则分类（用于分组和检索优化）
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// 是否必需规则
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// 规则权重（用于重要性排序）
        /// </summary>
        public decimal Weight { get; }

        /// <summary>
        /// 规则操作符（eq、gt、lt、in、contains等）
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// 规则描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 私有构造函数（用于序列化）
        /// </summary>
        private TargetingRule()
        {
            RuleKey = string.Empty;
            RuleValue = string.Empty;
            DataType = "String";
            Category = string.Empty;
            Operator = "eq";
            Description = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TargetingRule(
            string ruleKey,
            string ruleValue,
            string dataType = "String",
            string category = "",
            bool isRequired = false,
            decimal weight = 1.0m,
            string @operator = "eq",
            string description = "")
        {
            if (string.IsNullOrWhiteSpace(ruleKey))
                throw new ArgumentException("规则键不能为空", nameof(ruleKey));

            RuleKey = ruleKey;
            RuleValue = ruleValue ?? string.Empty;
            DataType = dataType;
            Category = category;
            IsRequired = isRequired;
            Weight = weight;
            Operator = @operator;
            Description = description;
        }

        /// <summary>
        /// 获取强类型的规则值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>转换后的规则值</returns>
        public T? GetValue<T>()
        {
            if (string.IsNullOrEmpty(RuleValue))
                return default;

            try
            {
                var targetType = typeof(T);

                // Direct string
                if (targetType == typeof(string))
                    return (T)(object)RuleValue;

                // Nullable<TUnderlying>
                var isNullable = Nullable.GetUnderlyingType(targetType) != null;
                var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

                // JSON payloads
                if (DataType == "Json")
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(RuleValue);
                }

                object? converted = null;

                // Enums
                if (underlying.IsEnum)
                {
                    if (int.TryParse(RuleValue, out var enumInt))
                    {
                        converted = Enum.ToObject(underlying, enumInt);
                    }
                    else
                    {
                        converted = Enum.Parse(underlying, RuleValue, true);
                    }
                }
                // Guid
                else if (underlying == typeof(Guid))
                {
                    if (Guid.TryParse(RuleValue, out var guid))
                        converted = guid;
                }
                // DateTime / DateTimeOffset with invariant culture and round-trip when possible
                else if (underlying == typeof(DateTime))
                {
                    if (DateTime.TryParse(RuleValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                        converted = dt;
                }
                else if (underlying == typeof(DateTimeOffset))
                {
                    if (DateTimeOffset.TryParse(RuleValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var dto))
                        converted = dto;
                }
                // Numerics with invariant culture
                else if (underlying == typeof(decimal))
                {
                    if (decimal.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dec))
                        converted = dec;
                }
                else if (underlying == typeof(double))
                {
                    if (double.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                        converted = dbl;
                }
                else if (underlying == typeof(float))
                {
                    if (float.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var fl))
                        converted = fl;
                }
                else if (underlying == typeof(long))
                {
                    if (long.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var l))
                        converted = l;
                }
                else if (underlying == typeof(int))
                {
                    if (int.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i))
                        converted = i;
                }
                else if (underlying == typeof(short))
                {
                    if (short.TryParse(RuleValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var s))
                        converted = s;
                }
                else if (underlying == typeof(bool))
                {
                    if (bool.TryParse(RuleValue, out var b))
                        converted = b;
                }
                else
                {
                    // Fallback to Convert.ChangeType with invariant culture
                    converted = Convert.ChangeType(RuleValue, underlying, System.Globalization.CultureInfo.InvariantCulture);
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
        /// 创建具有新值的TargetingRule实例（值对象不可变）
        /// </summary>
        /// <param name="newValue">新的规则值</param>
        /// <returns>新的TargetingRule实例</returns>
        public TargetingRule WithValue(object newValue)
        {
            if (newValue == null)
            {
                return new TargetingRule(RuleKey, string.Empty, "String", Category, IsRequired, Weight, Operator, Description);
            }

            string ruleValue;
            string dataType;

            if (newValue is string stringValue)
            {
                ruleValue = stringValue;
                dataType = "String";
            }
            else if (newValue.GetType().IsPrimitive || newValue is decimal || newValue is DateTime)
            {
                ruleValue = newValue.ToString() ?? string.Empty;
                dataType = newValue.GetType().Name;
            }
            else
            {
                ruleValue = System.Text.Json.JsonSerializer.Serialize(newValue);
                dataType = "Json";
            }

            return new TargetingRule(RuleKey, ruleValue, dataType, Category, IsRequired, Weight, Operator, Description);
        }

        /// <summary>
        /// 验证规则值是否符合操作符要求
        /// </summary>
        /// <param name="comparisonValue">比较值</param>
        /// <returns>是否匹配</returns>
        public bool ValidateValue(object comparisonValue)
        {
            try
            {
                var currentValue = GetValue<object>();
                if (currentValue == null && comparisonValue == null)
                    return true;

                if (currentValue == null || comparisonValue == null)
                    return false;

                return Operator.ToLowerInvariant() switch
                {
                    "eq" => currentValue.Equals(comparisonValue),
                    "ne" => !currentValue.Equals(comparisonValue),
                    "gt" => Comparer<object>.Default.Compare(currentValue, comparisonValue) > 0,
                    "ge" => Comparer<object>.Default.Compare(currentValue, comparisonValue) >= 0,
                    "lt" => Comparer<object>.Default.Compare(currentValue, comparisonValue) < 0,
                    "le" => Comparer<object>.Default.Compare(currentValue, comparisonValue) <= 0,
                    "in" => IsValueInCollection(comparisonValue),
                    "contains" => currentValue.ToString()?.Contains(comparisonValue.ToString() ?? string.Empty) == true,
                    _ => currentValue.Equals(comparisonValue)
                };
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查值是否在集合中
        /// </summary>
        private bool IsValueInCollection(object comparisonValue)
        {
            if (DataType == "Json")
            {
                var collection = GetValue<IEnumerable<object>>();
                return collection?.Contains(comparisonValue) == true;
            }

            var stringValue = RuleValue;
            var searchValue = comparisonValue.ToString() ?? string.Empty;
            return stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => v.Trim())
                            .Contains(searchValue);
        }

        /// <summary>
        /// 创建规则副本
        /// </summary>
        /// <returns>规则副本</returns>
        public TargetingRule Clone()
        {
            return new TargetingRule(RuleKey, RuleValue, DataType, Category, IsRequired, Weight, Operator, Description);
        }

        /// <summary>
        /// 获取相等性比较组件
        /// </summary>
        /// <returns>用于相等性比较的组件</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RuleKey;
            yield return RuleValue;
            yield return DataType;
            yield return Category;
            yield return IsRequired;
            yield return Weight;
            yield return Operator;
            yield return Description;
        }

        /// <summary>
        /// 获取规则的字符串表示
        /// </summary>
        /// <returns>规则描述</returns>
        public override string ToString()
        {
            var desc = !string.IsNullOrEmpty(Description) ? $" ({Description})" : string.Empty;
            return $"{RuleKey} {Operator} {RuleValue}{desc}";
        }
    }
}
