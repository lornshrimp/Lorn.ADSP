using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ����ƥ����ֵ����
/// ��ʾ���������������û������ĵ�ƥ�����������OverallMatchResult��IndividualResults���ϵ�Ԫ������
/// </summary>
public class MatchResult : ValueObject
{
    /// <summary>
    /// �������ͣ������������ͣ���"geo"��"demographic"�ȣ�
    /// </summary>
    public string CriteriaType { get; private set; }

    /// <summary>
    /// ��������ʵ����ʶ
    /// </summary>
    public string CriteriaId { get; private set; }

    /// <summary>
    /// �Ƿ�ƥ��
    /// </summary>
    public bool IsMatch { get; private set; }

    /// <summary>
    /// ƥ��ȷ�����0-1��
    /// </summary>
    public decimal MatchScore { get; private set; }

    /// <summary>
    /// ƥ��ԭ��
    /// </summary>
    public string MatchReason { get; private set; }

    /// <summary>
    /// ��ƥ��ԭ��
    /// </summary>
    public string NotMatchReason { get; private set; }

    /// <summary>
    /// ��ϸƥ����Ϣ�ֵ�
    /// </summary>
    public IReadOnlyDictionary<string, object> MatchDetails { get; private set; }

    /// <summary>
    /// �������������ʱ
    /// </summary>
    public TimeSpan ExecutionTime { get; private set; }

    /// <summary>
    /// ����ʱ���
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// ���ȼ�
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Ȩ��
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// �Ƿ�Ϊ��ѡ����
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private MatchResult(
        string criteriaType,
        string criteriaId,
        bool isMatch,
        decimal matchScore,
        string matchReason,
        string notMatchReason,
        IReadOnlyDictionary<string, object> matchDetails,
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
    /// ����ƥ��ɹ��Ľ��
    /// </summary>
    public static MatchResult CreateMatch(
        string criteriaType,
        string criteriaId,
        decimal matchScore,
        string matchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, matchScore, weight);

        return new MatchResult(
            criteriaType,
            criteriaId,
            true,
            matchScore,
            matchReason,
            string.Empty,
            matchDetails?.AsReadOnly() ?? new Dictionary<string, object>().AsReadOnly(),
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// ����ƥ��ʧ�ܵĽ��
    /// </summary>
    public static MatchResult CreateNoMatch(
        string criteriaType,
        string criteriaId,
        string notMatchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, 0m, weight);

        return new MatchResult(
            criteriaType,
            criteriaId,
            false,
            0m,
            string.Empty,
            notMatchReason,
            matchDetails?.AsReadOnly() ?? new Dictionary<string, object>().AsReadOnly(),
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// ��ȡ��Ȩ����
    /// </summary>
    public decimal GetWeightedScore()
    {
        return MatchScore * Weight;
    }

    /// <summary>
    /// ��ȡִ��ָ��
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
    /// ����Ƿ���ָ������
    /// </summary>
    public bool HasDetail(string key)
    {
        return MatchDetails.ContainsKey(key);
    }

    /// <summary>
    /// ��ȡָ������
    /// </summary>
    public T? GetDetail<T>(string key)
    {
        if (MatchDetails.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
        return default;
    }

    /// <summary>
    /// �Ƿ�Ϊ��Ч���
    /// </summary>
    public bool IsValidResult()
    {
        return !string.IsNullOrEmpty(CriteriaType) && 
               !string.IsNullOrEmpty(CriteriaId) &&
               (IsMatch ? !string.IsNullOrEmpty(MatchReason) : !string.IsNullOrEmpty(NotMatchReason));
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public string GetDebugInfo()
    {
        var status = IsMatch ? "MATCH" : "NO_MATCH";
        var reason = IsMatch ? MatchReason : NotMatchReason;
        var details = MatchDetails.Any() 
            ? string.Join(", ", MatchDetails.Take(3).Select(kv => $"{kv.Key}:{kv.Value}"))
            : "No Details";

        return $"{CriteriaType}[{CriteriaId}]: {status} Score:{MatchScore:F3} Weight:{Weight:F2} " +
               $"Reason:{reason} Time:{ExecutionTime.TotalMilliseconds:F1}ms Details:[{details}]";
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string criteriaType, string criteriaId, decimal matchScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            throw new ArgumentException("�������Ͳ���Ϊ��", nameof(criteriaType));

        if (string.IsNullOrWhiteSpace(criteriaId))
            throw new ArgumentException("����ID����Ϊ��", nameof(criteriaId));

        if (matchScore < 0m || matchScore > 1m)
            throw new ArgumentException("ƥ�����������0-1֮��", nameof(matchScore));

        if (weight < 0m)
            throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
    }
}