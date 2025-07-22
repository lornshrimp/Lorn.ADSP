using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 优化建议值对象
    /// </summary>
    public class OptimizationRecommendation : ValueObject
    {
        /// <summary>
        /// 优化类型
        /// </summary>
        public OptimizationType Type { get; private set; }

        /// <summary>
        /// 条件类型
        /// </summary>
        public string? CriteriaType { get; private set; }

        /// <summary>
        /// 新权重
        /// </summary>
        public decimal? NewWeight { get; private set; }

        /// <summary>
        /// 参数键
        /// </summary>
        public string? ParameterKey { get; private set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string? ParameterValue { get; private set; }

        /// <summary>
        /// 参数数据类型
        /// </summary>
        public string? ParameterDataType { get; private set; }

        /// <summary>
        /// 建议原因
        /// </summary>
        public string? Reason { get; private set; }

        /// <summary>
        /// 优先级（1-10，数字越大优先级越高）
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// 预期影响分数
        /// </summary>
        public decimal ExpectedImpact { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private OptimizationRecommendation(
            OptimizationType type,
            string? criteriaType,
            decimal? newWeight,
            string? parameterKey,
            string? parameterValue,
            string? parameterDataType,
            string? reason,
            int priority,
            decimal expectedImpact)
        {
            Type = type;
            CriteriaType = criteriaType;
            NewWeight = newWeight;
            ParameterKey = parameterKey;
            ParameterValue = parameterValue;
            ParameterDataType = parameterDataType;
            Reason = reason;
            Priority = priority;
            ExpectedImpact = expectedImpact;
        }

        /// <summary>
        /// 创建权重优化建议
        /// </summary>
        public static OptimizationRecommendation CreateWeightOptimization(
            string criteriaType,
            decimal newWeight,
            string reason,
            int priority = 5,
            decimal expectedImpact = 0.1m)
        {
            ValidateInputs(criteriaType, reason, priority, expectedImpact);

            return new OptimizationRecommendation(
                OptimizationType.AdjustWeight,
                criteriaType,
                newWeight,
                null,
                null,
                null,
                reason,
                priority,
                expectedImpact);
        }

        /// <summary>
        /// 创建参数优化建议
        /// </summary>
        public static OptimizationRecommendation CreateParameterOptimization(
            string parameterKey,
            object parameterValue,
            string reason,
            int priority = 5,
            decimal expectedImpact = 0.1m)
        {
            ValidateInputs(parameterKey, reason, priority, expectedImpact);

            string valueString;
            string dataType;

            if (parameterValue is string stringValue)
            {
                valueString = stringValue;
                dataType = "String";
            }
            else if (parameterValue.GetType().IsPrimitive || parameterValue is decimal || parameterValue is DateTime)
            {
                valueString = parameterValue.ToString() ?? string.Empty;
                dataType = parameterValue.GetType().Name;
            }
            else
            {
                valueString = System.Text.Json.JsonSerializer.Serialize(parameterValue);
                dataType = "Json";
            }

            return new OptimizationRecommendation(
                OptimizationType.SetDynamicParameter,
                null,
                null,
                parameterKey,
                valueString,
                dataType,
                reason,
                priority,
                expectedImpact);
        }

        /// <summary>
        /// 创建配置优化建议
        /// </summary>
        public static OptimizationRecommendation CreateConfigOptimization(
            string reason,
            int priority = 5,
            decimal expectedImpact = 0.1m)
        {
            ValidateInputs("config", reason, priority, expectedImpact);

            return new OptimizationRecommendation(
                OptimizationType.EnableCriteria,
                null,
                null,
                null,
                null,
                null,
                reason,
                priority,
                expectedImpact);
        }

        /// <summary>
        /// 获取参数值（强类型）
        /// </summary>
        public T? GetParameterValue<T>()
        {
            if (string.IsNullOrEmpty(ParameterValue))
                return default;

            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)ParameterValue;

                if (ParameterDataType == "Json")
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(ParameterValue);
                }

                return (T)Convert.ChangeType(ParameterValue, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 是否为高优先级建议
        /// </summary>
        public bool IsHighPriority()
        {
            return Priority >= 7;
        }

        /// <summary>
        /// 是否为高影响建议
        /// </summary>
        public bool IsHighImpact()
        {
            return ExpectedImpact >= 0.2m;
        }

        /// <summary>
        /// 获取建议摘要
        /// </summary>
        public string GetSummary()
        {
            var summary = $"[{Type}] ";

            switch (Type)
            {
                case OptimizationType.AdjustWeight:
                    summary += $"调整 {CriteriaType} 权重至 {NewWeight}";
                    break;
                case OptimizationType.SetDynamicParameter:
                    summary += $"设置参数 {ParameterKey} = {ParameterValue}";
                    break;
                case OptimizationType.EnableCriteria:
                    summary += "启用配置";
                    break;
                case OptimizationType.DisableCriteria:
                    summary += "禁用配置";
                    break;
                default:
                    summary += "其他优化";
                    break;
            }

            if (!string.IsNullOrEmpty(Reason))
            {
                summary += $" - {Reason}";
            }

            summary += $" (优先级: {Priority}, 预期影响: {ExpectedImpact:P})";

            return summary;
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return CriteriaType ?? string.Empty;
            yield return NewWeight ?? 0m;
            yield return ParameterKey ?? string.Empty;
            yield return ParameterValue ?? string.Empty;
            yield return ParameterDataType ?? string.Empty;
            yield return Reason ?? string.Empty;
            yield return Priority;
            yield return ExpectedImpact;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInputs(string key, string reason, int priority, decimal expectedImpact)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("键不能为空", nameof(key));

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("原因不能为空", nameof(reason));

            if (priority < 1 || priority > 10)
                throw new ArgumentException("优先级必须在1-10之间", nameof(priority));

            if (expectedImpact < 0 || expectedImpact > 1)
                throw new ArgumentException("预期影响必须在0-1之间", nameof(expectedImpact));
        }
    }
}
