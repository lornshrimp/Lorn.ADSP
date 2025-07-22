using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 定向条件值对象
/// [临时数据] - 运行时使用，不直接存储
/// 从DDD角度看，定向条件是值对象，因为它们由属性值决定身份，是不可变的
/// </summary>
public class TargetingCriteria : ValueObject
{
    /// <summary>
    /// 条件名称
    /// </summary>
    public string CriteriaName { get; private set; }

    /// <summary>
    /// 条件类型（如：age、gender、location等）
    /// </summary>
    public string CriteriaType { get; private set; }

    /// <summary>
    /// 操作符（如：equals、in、between、gt、lt等）
    /// </summary>
    public string Operator { get; private set; }

    /// <summary>
    /// 目标值（JSON格式存储复杂对象）
    /// </summary>
    public string TargetValue { get; private set; }

    /// <summary>
    /// 权重
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private TargetingCriteria(
        string criteriaName,
        string criteriaType,
        string @operator,
        string targetValue,
        decimal weight = 1.0m,
        bool isEnabled = true,
        string description = "")
    {
        CriteriaName = criteriaName ?? throw new ArgumentNullException(nameof(criteriaName));
        CriteriaType = criteriaType ?? throw new ArgumentNullException(nameof(criteriaType));
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        TargetValue = targetValue ?? throw new ArgumentNullException(nameof(targetValue));
        Weight = weight;
        IsEnabled = isEnabled;
        Description = description;

        ValidateWeight(weight);
    }

    /// <summary>
    /// 创建定向条件
    /// </summary>
    public static TargetingCriteria Create(
        string criteriaName,
        string criteriaType,
        string @operator,
        string targetValue,
        decimal weight = 1.0m,
        bool isEnabled = true,
        string description = "")
    {
        return new TargetingCriteria(criteriaName, criteriaType, @operator, targetValue, weight, isEnabled, description);
    }

    /// <summary>
    /// 创建年龄定向条件
    /// </summary>
    public static TargetingCriteria CreateAgeRange(int minAge, int maxAge, decimal weight = 1.0m)
    {
        var targetValue = System.Text.Json.JsonSerializer.Serialize(new { min = minAge, max = maxAge });
        return new TargetingCriteria("age_range", "demographic", "between", targetValue, weight, true, $"年龄范围: {minAge}-{maxAge}");
    }

    /// <summary>
    /// 创建性别定向条件
    /// </summary>
    public static TargetingCriteria CreateGender(string gender, decimal weight = 1.0m)
    {
        return new TargetingCriteria("gender", "demographic", "equals", gender, weight, true, $"性别: {gender}");
    }

    /// <summary>
    /// 创建地理位置定向条件
    /// </summary>
    public static TargetingCriteria CreateLocation(string[] locations, decimal weight = 1.0m)
    {
        var targetValue = System.Text.Json.JsonSerializer.Serialize(locations);
        return new TargetingCriteria("location", "geographic", "in", targetValue, weight, true, $"地理位置: {string.Join(",", locations)}");
    }

    /// <summary>
    /// 创建兴趣定向条件
    /// </summary>
    public static TargetingCriteria CreateInterest(string[] interests, decimal weight = 1.0m)
    {
        var targetValue = System.Text.Json.JsonSerializer.Serialize(interests);
        return new TargetingCriteria("interests", "behavioral", "in", targetValue, weight, true, $"兴趣: {string.Join(",", interests)}");
    }

    /// <summary>
    /// 验证权重
    /// </summary>
    private static void ValidateWeight(decimal weight)
    {
        if (weight < 0 || weight > 10)
            throw new ArgumentException("权重必须在0-10之间", nameof(weight));
    }

    /// <summary>
    /// 获取目标值作为字符串数组
    /// </summary>
    public string[] GetTargetValueAsStringArray()
    {
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<string[]>(TargetValue) ?? Array.Empty<string>();
        }
        catch
        {
            return new[] { TargetValue };
        }
    }

    /// <summary>
    /// 获取目标值作为数值范围
    /// </summary>
    public (int min, int max)? GetTargetValueAsRange()
    {
        try
        {
            var range = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(TargetValue);
            if (range != null && range.ContainsKey("min") && range.ContainsKey("max"))
            {
                return (range["min"], range["max"]);
            }
        }
        catch
        {
            // 忽略解析错误
        }
        return null;
    }

    /// <summary>
    /// 检查是否匹配指定值
    /// </summary>
    public bool IsMatch(object value)
    {
        if (!IsEnabled || value == null)
            return false;

        try
        {
            return Operator.ToLower() switch
            {
                "equals" => value.ToString() == TargetValue,
                "in" => GetTargetValueAsStringArray().Contains(value.ToString()),
                "between" => IsInRange(value),
                "gt" => IsGreaterThan(value),
                "lt" => IsLessThan(value),
                "gte" => IsGreaterThanOrEqual(value),
                "lte" => IsLessThanOrEqual(value),
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }

    private bool IsInRange(object value)
    {
        if (int.TryParse(value.ToString(), out int intValue))
        {
            var range = GetTargetValueAsRange();
            return range.HasValue && intValue >= range.Value.min && intValue <= range.Value.max;
        }
        return false;
    }

    private bool IsGreaterThan(object value)
    {
        if (decimal.TryParse(value.ToString(), out decimal decValue) &&
            decimal.TryParse(TargetValue, out decimal targetDecValue))
        {
            return decValue > targetDecValue;
        }
        return false;
    }

    private bool IsLessThan(object value)
    {
        if (decimal.TryParse(value.ToString(), out decimal decValue) &&
            decimal.TryParse(TargetValue, out decimal targetDecValue))
        {
            return decValue < targetDecValue;
        }
        return false;
    }

    private bool IsGreaterThanOrEqual(object value)
    {
        if (decimal.TryParse(value.ToString(), out decimal decValue) &&
            decimal.TryParse(TargetValue, out decimal targetDecValue))
        {
            return decValue >= targetDecValue;
        }
        return false;
    }

    private bool IsLessThanOrEqual(object value)
    {
        if (decimal.TryParse(value.ToString(), out decimal decValue) &&
            decimal.TryParse(TargetValue, out decimal targetDecValue))
        {
            return decValue <= targetDecValue;
        }
        return false;
    }

    /// <summary>
    /// 值对象相等性比较的属性
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CriteriaName;
        yield return CriteriaType;
        yield return Operator;
        yield return TargetValue;
        yield return Weight;
        yield return IsEnabled;
    }

    public override string ToString()
    {
        return $"{CriteriaName}({CriteriaType}): {Operator} {TargetValue} [Weight: {Weight}, Enabled: {IsEnabled}]";
    }
}
