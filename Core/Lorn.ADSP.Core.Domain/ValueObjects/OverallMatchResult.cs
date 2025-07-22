using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 整体匹配结果值对象
/// 封装广告候选对定向条件的匹配计算结果
/// 设计原则：不可变、基于值相等、计算结果临时存在
/// </summary>
public class OverallMatchResult : ValueObject
{
    /// <summary>
    /// 关联的广告候选ID
    /// </summary>
    public string AdCandidateId { get; }

    /// <summary>
    /// 关联的广告上下文ID
    /// </summary>
    public string AdContextId { get; }

    /// <summary>
    /// 总体匹配分数（0-1）
    /// </summary>
    public decimal OverallScore { get; }

    /// <summary>
    /// 是否总体匹配
    /// </summary>
    public bool IsOverallMatch { get; }

    /// <summary>
    /// 单项匹配结果集合
    /// </summary>
    public IReadOnlyList<MatchResult> IndividualResults { get; }

    /// <summary>
    /// 加权分数分布
    /// </summary>
    public IReadOnlyDictionary<string, decimal> WeightedScores { get; }

    /// <summary>
    /// 总执行时间
    /// </summary>
    public TimeSpan TotalExecutionTime { get; }

    /// <summary>
    /// 计算时间
    /// </summary>
    public DateTime CalculatedAt { get; }

    /// <summary>
    /// 匹配原因码
    /// </summary>
    public string ReasonCode { get; }

    /// <summary>
    /// 匹配置信度
    /// </summary>
    public MatchConfidence? Confidence { get; }

    /// <summary>
    /// 私有构造函数，强制使用工厂方法创建
    /// </summary>
    private OverallMatchResult(
        string adCandidateId,
        string adContextId,
        decimal overallScore,
        bool isOverallMatch,
        IReadOnlyList<MatchResult> individualResults,
        IReadOnlyDictionary<string, decimal> weightedScores,
        TimeSpan totalExecutionTime,
        DateTime calculatedAt,
        string reasonCode,
        MatchConfidence? confidence)
    {
        AdCandidateId = adCandidateId;
        AdContextId = adContextId;
        OverallScore = overallScore;
        IsOverallMatch = isOverallMatch;
        IndividualResults = individualResults;
        WeightedScores = weightedScores;
        TotalExecutionTime = totalExecutionTime;
        CalculatedAt = calculatedAt;
        ReasonCode = reasonCode;
        Confidence = confidence;
    }

    /// <summary>
    /// 创建匹配结果
    /// </summary>
    public static OverallMatchResult Create(
        string adCandidateId,
        string adContextId,
        IList<MatchResult> individualResults,
        MatchConfidence? confidence = null)
    {
        if (string.IsNullOrWhiteSpace(adCandidateId))
            throw new ArgumentException("广告候选ID不能为空", nameof(adCandidateId));

        if (string.IsNullOrWhiteSpace(adContextId))
            throw new ArgumentException("广告上下文ID不能为空", nameof(adContextId));

        if (individualResults == null)
            throw new ArgumentNullException(nameof(individualResults));

        var startTime = DateTime.UtcNow;
        var readOnlyResults = individualResults.ToList().AsReadOnly();

        // 计算整体匹配度和判断结果
        var (overallScore, isOverallMatch, weightedScores, reasonCode) = CalculateOverallScores(readOnlyResults);

        var executionTime = DateTime.UtcNow - startTime;

        return new OverallMatchResult(
            adCandidateId,
            adContextId,
            overallScore,
            isOverallMatch,
            readOnlyResults,
            weightedScores,
            executionTime,
            DateTime.UtcNow,
            reasonCode,
            confidence);
    }

    /// <summary>
    /// 创建不匹配的结果
    /// </summary>
    public static OverallMatchResult CreateNotMatched(
        string adCandidateId,
        string adContextId,
        string reasonCode,
        MatchConfidence? confidence = null)
    {
        if (string.IsNullOrWhiteSpace(adCandidateId))
            throw new ArgumentException("广告候选ID不能为空", nameof(adCandidateId));

        if (string.IsNullOrWhiteSpace(adContextId))
            throw new ArgumentException("广告上下文ID不能为空", nameof(adContextId));

        return new OverallMatchResult(
            adCandidateId,
            adContextId,
            0m,
            false,
            new List<MatchResult>().AsReadOnly(),
            new Dictionary<string, decimal>(),
            TimeSpan.Zero,
            DateTime.UtcNow,
            reasonCode ?? "NOT_MATCHED",
            confidence);
    }

    /// <summary>
    /// 创建完全匹配的结果
    /// </summary>
    public static OverallMatchResult CreateFullMatch(
        string adCandidateId,
        string adContextId,
        decimal score = 1.0m,
        MatchConfidence? confidence = null)
    {
        if (string.IsNullOrWhiteSpace(adCandidateId))
            throw new ArgumentException("广告候选ID不能为空", nameof(adCandidateId));

        if (string.IsNullOrWhiteSpace(adContextId))
            throw new ArgumentException("广告上下文ID不能为空", nameof(adContextId));

        if (score < 0 || score > 1)
            throw new ArgumentOutOfRangeException(nameof(score), "匹配分数必须在0-1之间");

        return new OverallMatchResult(
            adCandidateId,
            adContextId,
            score,
            true,
            new List<MatchResult>().AsReadOnly(),
            new Dictionary<string, decimal> { ["overall"] = score },
            TimeSpan.Zero,
            DateTime.UtcNow,
            "FULL_MATCH",
            confidence);
    }

    /// <summary>
    /// 计算整体匹配分数和权重分布
    /// </summary>
    private static (decimal overallScore, bool isOverallMatch, IReadOnlyDictionary<string, decimal> weightedScores, string reasonCode)
        CalculateOverallScores(IReadOnlyList<MatchResult> individualResults)
    {
        if (!individualResults.Any())
        {
            return (0m, false, new Dictionary<string, decimal>(), "NO_CRITERIA");
        }

        // 计算加权分数
        var weightedScores = new Dictionary<string, decimal>();
        decimal totalWeight = individualResults.Sum(r => r.Weight);

        if (totalWeight <= 0)
        {
            return (0m, false, new Dictionary<string, decimal>(), "INVALID_WEIGHTS");
        }

        decimal weightedSum = 0m;
        var allMatched = true;
        var failedCriteria = new List<string>();

        foreach (var result in individualResults)
        {
            var normalizedWeight = result.Weight / totalWeight;
            var weightedScore = result.MatchScore * normalizedWeight;

            weightedScores[result.CriteriaType] = weightedScore;
            weightedSum += weightedScore;

            if (!result.IsMatch)
            {
                allMatched = false;
                failedCriteria.Add(result.CriteriaType);
            }
        }

        var reasonCode = allMatched
            ? "MATCH_SUCCESS"
            : $"MATCH_FAILED: {string.Join(",", failedCriteria)}";

        return (weightedSum, allMatched, weightedScores, reasonCode);
    }

    /// <summary>
    /// 获取指定条件类型的匹配结果
    /// </summary>
    public MatchResult? GetMatchResult(string criteriaType)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            return null;

        return IndividualResults.FirstOrDefault(r => r.CriteriaType == criteriaType);
    }

    /// <summary>
    /// 获取失败的匹配条件
    /// </summary>
    public IReadOnlyList<MatchResult> GetFailedMatches()
    {
        return IndividualResults.Where(r => !r.IsMatch).ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取成功的匹配条件
    /// </summary>
    public IReadOnlyList<MatchResult> GetSuccessfulMatches()
    {
        return IndividualResults.Where(r => r.IsMatch).ToList().AsReadOnly();
    }

    /// <summary>
    /// 检查是否包含指定的条件类型
    /// </summary>
    public bool HasCriteriaType(string criteriaType)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            return false;

        return IndividualResults.Any(r => r.CriteriaType == criteriaType);
    }

    /// <summary>
    /// 获取匹配率（成功条件数/总条件数）
    /// </summary>
    public decimal GetMatchRate()
    {
        if (!IndividualResults.Any())
            return 0m;

        var successfulCount = IndividualResults.Count(r => r.IsMatch);
        return (decimal)successfulCount / IndividualResults.Count;
    }

    /// <summary>
    /// 验证匹配结果的有效性
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(AdCandidateId) &&
               !string.IsNullOrWhiteSpace(AdContextId) &&
               OverallScore >= 0 && OverallScore <= 1 &&
               CalculatedAt > DateTime.MinValue &&
               !string.IsNullOrWhiteSpace(ReasonCode);
    }

    /// <summary>
    /// 创建匹配结果的副本，用于不同的候选广告
    /// </summary>
    public OverallMatchResult CloneForCandidate(string newAdCandidateId)
    {
        if (string.IsNullOrWhiteSpace(newAdCandidateId))
            throw new ArgumentException("新的广告候选ID不能为空", nameof(newAdCandidateId));

        return new OverallMatchResult(
            newAdCandidateId,
            AdContextId,
            OverallScore,
            IsOverallMatch,
            IndividualResults,
            WeightedScores,
            TotalExecutionTime,
            CalculatedAt,
            ReasonCode,
            Confidence);
    }

    /// <summary>
    /// 实现值对象的相等性比较组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AdCandidateId;
        yield return AdContextId;
        yield return OverallScore;
        yield return IsOverallMatch;
        yield return ReasonCode;
        yield return CalculatedAt;

        // 单项匹配结果按类型排序
        foreach (var result in IndividualResults.OrderBy(r => r.CriteriaType))
        {
            yield return result;
        }

        // 加权分数按键排序
        foreach (var kvp in WeightedScores.OrderBy(w => w.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }

        yield return Confidence?.ToString() ?? string.Empty;
    }
}
