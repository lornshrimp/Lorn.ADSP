using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 单项匹配结果值对象
/// 表示单个定向条件对用户上下文的匹配计算结果，是OverallMatchResult中IndividualResults集合的元素类型
/// </summary>
public class MatchResult : ValueObject
{
    /// <summary>
    /// 条件类型（定向条件类型，如"geo"、"demographic"等）
    /// </summary>
    public string CriteriaType { get; private set; }

    /// <summary>
    /// 具体条件实例标识
    /// </summary>
    public Guid CriteriaId { get; private set; }

    /// <summary>
    /// 是否匹配
    /// </summary>
    public bool IsMatch { get; private set; }

    /// <summary>
    /// 匹配度分数（0-1）
    /// </summary>
    public decimal MatchScore { get; private set; }

    /// <summary>
    /// 匹配原因
    /// </summary>
    public string MatchReason { get; private set; }

    /// <summary>
    /// 不匹配原因
    /// </summary>
    public string NotMatchReason { get; private set; }

    /// <summary>
    /// 详细匹配信息
    /// </summary>
    public IReadOnlyList<ContextProperty> MatchDetails { get; private set; }

    /// <summary>
    /// 单个条件计算耗时
    /// </summary>
    public TimeSpan ExecutionTime { get; private set; }

    /// <summary>
    /// 计算时间戳
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// 权重
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// 是否为必选条件
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private MatchResult(
        string criteriaType,
        Guid criteriaId,
        bool isMatch,
        decimal matchScore,
        string matchReason,
        string notMatchReason,
        IReadOnlyList<ContextProperty> matchDetails,
        TimeSpan executionTime,
        DateTime calculatedAt,
        int priority,
        decimal weight,
        bool isRequired)
    {
        CriteriaType = criteriaType;
        CriteriaId = criteriaId;
        IsMatch = isMatch;
        MatchScore = matchScore;
        MatchReason = matchReason;
        NotMatchReason = notMatchReason;
        MatchDetails = matchDetails;
        ExecutionTime = executionTime;
        CalculatedAt = calculatedAt;
        Priority = priority;
        Weight = weight;
        IsRequired = isRequired;
    }

    /// <summary>
    /// 创建匹配成功的结果
    /// </summary>
    public static MatchResult CreateMatch(
        string criteriaType,
        Guid criteriaId,
        decimal matchScore,
        string matchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IEnumerable<ContextProperty>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, matchScore, weight);

        var details = matchDetails?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchResult(
            criteriaType,
            criteriaId,
            true,
            matchScore,
            matchReason,
            string.Empty,
            details,
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// 创建匹配失败的结果
    /// </summary>
    public static MatchResult CreateNoMatch(
        string criteriaType,
        Guid criteriaId,
        string notMatchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IEnumerable<ContextProperty>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, 0m, weight);

        var details = matchDetails?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchResult(
            criteriaType,
            criteriaId,
            false,
            0m,
            string.Empty,
            notMatchReason,
            details,
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// 创建匹配结果（从字典参数）
    /// </summary>
    public static MatchResult CreateMatchFromDictionary(
        string criteriaType,
        Guid criteriaId,
        decimal matchScore,
        string matchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        var details = ConvertDictionaryToContextProperties(matchDetails);
        return CreateMatch(criteriaType, criteriaId, matchScore, matchReason, executionTime, priority, weight, isRequired, details);
    }

    /// <summary>
    /// 创建不匹配结果（从字典参数）
    /// </summary>
    public static MatchResult CreateNoMatchFromDictionary(
        string criteriaType,
        Guid criteriaId,
        string notMatchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        var details = ConvertDictionaryToContextProperties(matchDetails);
        return CreateNoMatch(criteriaType, criteriaId, notMatchReason, executionTime, priority, weight, isRequired, details);
    }

    /// <summary>
    /// 将字典转换为 ContextProperty 集合
    /// </summary>
    private static IEnumerable<ContextProperty> ConvertDictionaryToContextProperties(IDictionary<string, object>? dictionary)
    {
        if (dictionary == null)
            return Enumerable.Empty<ContextProperty>();

        return dictionary.Select(kvp =>
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
                "MatchDetail",
                false,
                1.0m,
                null,
                "MatchResult");
        });
    }

    /// <summary>
    /// 获取加权分数
    /// </summary>
    public decimal GetWeightedScore()
    {
        return MatchScore * Weight;
    }

    /// <summary>
    /// 获取执行指标
    /// </summary>
    public Dictionary<string, object> GetExecutionMetrics()
    {
        return new Dictionary<string, object>
        {
            ["CriteriaType"] = CriteriaType,
            ["CriteriaId"] = CriteriaId,
            ["IsMatch"] = IsMatch,
            ["MatchScore"] = MatchScore,
            ["Weight"] = Weight,
            ["WeightedScore"] = GetWeightedScore(),
            ["ExecutionTime"] = ExecutionTime.TotalMilliseconds,
            ["Priority"] = Priority,
            ["IsRequired"] = IsRequired
        };
    }

    /// <summary>
    /// 检查是否有指定详情
    /// </summary>
    public bool HasDetail(string key)
    {
        return MatchDetails.Any(p => p.PropertyKey == key);
    }

    /// <summary>
    /// 获取指定详情
    /// </summary>
    public T? GetDetail<T>(string key) where T : struct
    {
        var detail = MatchDetails.FirstOrDefault(p => p.PropertyKey == key);
        return detail?.GetValue<T>();
    }

    /// <summary>
    /// 获取指定详情（引用类型）
    /// </summary>
    public T? GetDetailRef<T>(string key) where T : class
    {
        var detail = MatchDetails.FirstOrDefault(p => p.PropertyKey == key);
        return detail?.GetValue<T>();
    }

    /// <summary>
    /// 获取所有详情作为字典（向后兼容）
    /// </summary>
    public Dictionary<string, object> GetDetailsAsDictionary()
    {
        var result = new Dictionary<string, object>();
        foreach (var detail in MatchDetails)
        {
            try
            {
                var value = detail.GetValue<object>();
                if (value != null)
                {
                    result[detail.PropertyKey] = value;
                }
            }
            catch
            {
                // 如果转换失败，使用原始字符串值
                result[detail.PropertyKey] = detail.PropertyValue;
            }
        }
        return result;
    }

    /// <summary>
    /// 是否为有效结果
    /// </summary>
    public bool IsValidResult()
    {
        return !string.IsNullOrEmpty(CriteriaType) &&
               CriteriaId != Guid.Empty &&
               (IsMatch ? !string.IsNullOrEmpty(MatchReason) : !string.IsNullOrEmpty(NotMatchReason));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public string GetDebugInfo()
    {
        var status = IsMatch ? "MATCH" : "NO_MATCH";
        var reason = IsMatch ? MatchReason : NotMatchReason;
        var details = MatchDetails.Any()
            ? string.Join(", ", MatchDetails.Take(3).Select(detail => $"{detail.PropertyKey}:{detail.PropertyValue}"))
            : "No Details";

        return $"{CriteriaType}[{CriteriaId}]: {status} Score:{MatchScore:F3} Weight:{Weight:F2} " +
               $"Reason:{reason} Time:{ExecutionTime.TotalMilliseconds:F1}ms Details:[{details}]";
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CriteriaType;
        yield return CriteriaId;
        yield return IsMatch;
        yield return MatchScore;
        yield return Weight;
        yield return Priority;
        yield return IsRequired;
        yield return CalculatedAt;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string criteriaType, Guid criteriaId, decimal matchScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            throw new ArgumentException("条件类型不能为空", nameof(criteriaType));

        if (criteriaId == Guid.Empty)
            throw new ArgumentException("条件ID不能为空", nameof(criteriaId));

        if (matchScore < 0m || matchScore > 1m)
            throw new ArgumentException("匹配分数必须在0-1之间", nameof(matchScore));

        if (weight < 0m)
            throw new ArgumentException("权重不能为负数", nameof(weight));
    }
}