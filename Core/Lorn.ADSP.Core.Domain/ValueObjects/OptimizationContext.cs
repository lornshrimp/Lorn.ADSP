using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 优化上下文值对象
    /// </summary>
    public class OptimizationContext : ValueObject
    {
        /// <summary>
        /// 性能指标
        /// </summary>
        public PerformanceMetrics? PerformanceMetrics { get; private set; }

        /// <summary>
        /// 优化建议集合
        /// </summary>
        public IReadOnlyList<OptimizationRecommendation> OptimizationRecommendations { get; private set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public IReadOnlyList<ContextProperty> AdditionalData { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private OptimizationContext(
            PerformanceMetrics? performanceMetrics,
            IReadOnlyList<OptimizationRecommendation> optimizationRecommendations,
            IReadOnlyList<ContextProperty> additionalData)
        {
            PerformanceMetrics = performanceMetrics;
            OptimizationRecommendations = optimizationRecommendations;
            AdditionalData = additionalData;
        }

        /// <summary>
        /// 创建优化上下文
        /// </summary>
        public static OptimizationContext Create(
            PerformanceMetrics? performanceMetrics = null,
            IEnumerable<OptimizationRecommendation>? recommendations = null,
            IEnumerable<ContextProperty>? additionalData = null)
        {
            var recommendationsList = recommendations?.ToList().AsReadOnly() ?? new List<OptimizationRecommendation>().AsReadOnly();
            var additionalDataList = additionalData?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

            return new OptimizationContext(performanceMetrics, recommendationsList, additionalDataList);
        }

        /// <summary>
        /// 从字典创建优化上下文（向后兼容）
        /// </summary>
        public static OptimizationContext CreateFromDictionary(
            PerformanceMetrics? performanceMetrics = null,
            IEnumerable<OptimizationRecommendation>? recommendations = null,
            Dictionary<string, object>? additionalData = null)
        {
            var additionalProperties = additionalData?.Select(kvp =>
            {
                string propertyValue;
                string dataType;

                if (kvp.Value is string stringValue)
                {
                    propertyValue = stringValue;
                    dataType = "String";
                }
                else if (kvp.Value.GetType().IsPrimitive || kvp.Value is decimal || kvp.Value is DateTime)
                {
                    propertyValue = kvp.Value.ToString() ?? string.Empty;
                    dataType = kvp.Value.GetType().Name;
                }
                else
                {
                    propertyValue = System.Text.Json.JsonSerializer.Serialize(kvp.Value);
                    dataType = "Json";
                }

                return new ContextProperty(
                    kvp.Key,
                    propertyValue,
                    dataType,
                    "OptimizationData",
                    false,
                    1.0m,
                    null,
                    "OptimizationContext");
            }) ?? Enumerable.Empty<ContextProperty>();

            return Create(performanceMetrics, recommendations, additionalProperties);
        }

        /// <summary>
        /// 添加优化建议
        /// </summary>
        public OptimizationContext AddRecommendation(OptimizationRecommendation recommendation)
        {
            if (recommendation == null)
                throw new ArgumentNullException(nameof(recommendation));

            var newRecommendations = OptimizationRecommendations.ToList();
            newRecommendations.Add(recommendation);

            return new OptimizationContext(PerformanceMetrics, newRecommendations.AsReadOnly(), AdditionalData);
        }

        /// <summary>
        /// 添加附加数据
        /// </summary>
        public OptimizationContext AddAdditionalData(string key, object value)
        {
            var newData = AdditionalData.ToList();

            // 移除已存在的相同key的属性
            newData.RemoveAll(p => p.PropertyKey == key);

            string propertyValue;
            string dataType;

            if (value is string stringValue)
            {
                propertyValue = stringValue;
                dataType = "String";
            }
            else if (value.GetType().IsPrimitive || value is decimal || value is DateTime)
            {
                propertyValue = value.ToString() ?? string.Empty;
                dataType = value.GetType().Name;
            }
            else
            {
                propertyValue = System.Text.Json.JsonSerializer.Serialize(value);
                dataType = "Json";
            }

            newData.Add(new ContextProperty(
                key,
                propertyValue,
                dataType,
                "OptimizationData",
                false,
                1.0m,
                null,
                "OptimizationContext"));

            return new OptimizationContext(PerformanceMetrics, OptimizationRecommendations, newData.AsReadOnly());
        }

        /// <summary>
        /// 更新性能指标
        /// </summary>
        public OptimizationContext WithPerformanceMetrics(PerformanceMetrics performanceMetrics)
        {
            return new OptimizationContext(performanceMetrics, OptimizationRecommendations, AdditionalData);
        }

        /// <summary>
        /// 获取附加数据值
        /// </summary>
        public T? GetAdditionalData<T>(string key) where T : struct
        {
            var property = AdditionalData.FirstOrDefault(p => p.PropertyKey == key);
            return property?.GetValue<T>();
        }

        /// <summary>
        /// 获取附加数据值（引用类型）
        /// </summary>
        public T? GetAdditionalDataRef<T>(string key) where T : class
        {
            var property = AdditionalData.FirstOrDefault(p => p.PropertyKey == key);
            return property?.GetValue<T>();
        }

        /// <summary>
        /// 获取附加数据作为字典（向后兼容）
        /// </summary>
        public Dictionary<string, object> GetAdditionalDataAsDictionary()
        {
            var result = new Dictionary<string, object>();
            foreach (var property in AdditionalData)
            {
                try
                {
                    var value = property.GetValue<object>();
                    if (value != null)
                    {
                        result[property.PropertyKey] = value;
                    }
                }
                catch
                {
                    // 如果转换失败，使用原始字符串值
                    result[property.PropertyKey] = property.PropertyValue;
                }
            }
            return result;
        }

        /// <summary>
        /// 检查是否包含特定的附加数据
        /// </summary>
        public bool HasAdditionalData(string key)
        {
            return AdditionalData.Any(p => p.PropertyKey == key);
        }

        /// <summary>
        /// 获取高优先级建议
        /// </summary>
        public IEnumerable<OptimizationRecommendation> GetHighPriorityRecommendations()
        {
            // 假设 OptimizationRecommendation 有优先级属性，这里简化处理
            return OptimizationRecommendations.Take(5); // 返回前5个建议作为高优先级
        }

        /// <summary>
        /// 是否需要优化
        /// </summary>
        public bool NeedsOptimization()
        {
            return OptimizationRecommendations.Any() ||
                   (PerformanceMetrics != null && !PerformanceMetrics.IsHighPerformance());
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PerformanceMetrics ?? (object)"null";

            foreach (var recommendation in OptimizationRecommendations.OrderBy(r => r.GetHashCode()))
            {
                yield return recommendation;
            }

            foreach (var data in AdditionalData.OrderBy(d => d.PropertyKey))
            {
                yield return data;
            }
        }
    }
}
