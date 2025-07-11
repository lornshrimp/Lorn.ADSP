using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 匹配结果实体
/// 存储AdCandidate对TargetingContext的定向匹配计算结果，由定向策略计算器生成
/// 生命周期：计算生成→缓存存储→结果使用→过期清理
/// </summary>
public class OverallMatchResult : EntityBase
{
    /// <summary>
    /// 结果唯一标识
    /// </summary>
    public string ResultId { get; private set; }

    /// <summary>
    /// 关联的广告候选ID
    /// </summary>
    public string AdCandidateId { get; private set; }

    /// <summary>
    /// 关联的广告上下文ID
    /// </summary>
    public string AdContextId { get; private set; }

    /// <summary>
    /// 总体匹配分数（0-1）
    /// </summary>
    public decimal OverallScore { get; private set; }

    /// <summary>
    /// 是否总体匹配
    /// </summary>
    public bool IsOverallMatch { get; private set; }

    /// <summary>
    /// 单项匹配结果集合
    /// </summary>
    private readonly List<MatchResult> _individualResults = new();
    public IReadOnlyList<MatchResult> IndividualResults => _individualResults.AsReadOnly();

    /// <summary>
    /// 加权分数分布
    /// </summary>
    public IReadOnlyDictionary<string, decimal> WeightedScores { get; private set; }

    /// <summary>
    /// 总执行时间
    /// </summary>
    public TimeSpan TotalExecutionTime { get; private set; }

    /// <summary>
    /// 匹配或不匹配的原因代码
    /// </summary>
    public string ReasonCode { get; private set; }

    /// <summary>
    /// 计算时间
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// 匹配置信度
    /// </summary>
    public MatchConfidence Confidence { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private OverallMatchResult(
        string resultId,
        string adCandidateId,
        string adContextId,
        decimal overallScore,
        bool isOverallMatch,
        IReadOnlyDictionary<string, decimal> weightedScores,
        TimeSpan totalExecutionTime,
        string reasonCode,
        DateTime calculatedAt,
        MatchConfidence confidence)
    {
        ResultId = resultId;
        AdCandidateId = adCandidateId;
        AdContextId = adContextId;
        OverallScore = overallScore;
        IsOverallMatch = isOverallMatch;
        WeightedScores = weightedScores;
        TotalExecutionTime = totalExecutionTime;
        ReasonCode = reasonCode;
        CalculatedAt = calculatedAt;
        Confidence = confidence;
    }

    /// <summary>
    /// 创建总体匹配结果
    /// </summary>
    public static OverallMatchResult Create(
        string adCandidateId,
        string adContextId,
        IList<MatchResult> individualResults,
        decimal matchThreshold = 0.5m,
        string? resultId = null)
    {
        ValidateInputs(adCandidateId, adContextId, individualResults);

        var finalResultId = resultId ?? Guid.NewGuid().ToString();
        var calculatedAt = DateTime.UtcNow;

        // 计算加权分数
        var weightedScores = CalculateWeightedScores(individualResults);
        
        // 计算总体分数
        var overallScore = CalculateOverallScore(individualResults);
        
        // 判断是否匹配
        var isOverallMatch = DetermineOverallMatch(individualResults, overallScore, matchThreshold);
        
        // 计算总执行时间
        var totalExecutionTime = individualResults.Aggregate(TimeSpan.Zero, (total, result) => total + result.ExecutionTime);
        
        // 生成原因代码
        var reasonCode = GenerateReasonCode(isOverallMatch, individualResults);
        
        // 计算置信度
        var confidence = CalculateConfidence(individualResults);

        var result = new OverallMatchResult(
            finalResultId,
            adCandidateId,
            adContextId,
            overallScore,
            isOverallMatch,
            weightedScores,
            totalExecutionTime,
            reasonCode,
            calculatedAt,
            confidence);

        // 添加单项结果
        result._individualResults.AddRange(individualResults);

        return result;
    }

    /// <summary>
    /// 添加单项匹配结果
    /// </summary>
    public void AddIndividualResult(MatchResult matchResult)
    {
        if (matchResult == null)
            throw new ArgumentNullException(nameof(matchResult));

        _individualResults.Add(matchResult);
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 获取匹配详情
    /// </summary>
    public Dictionary<string, object> GetMatchDetails()
    {
        return new Dictionary<string, object>
        {
            ["ResultId"] = ResultId,
            ["AdCandidateId"] = AdCandidateId,
            ["AdContextId"] = AdContextId,
            ["OverallScore"] = OverallScore,
            ["IsOverallMatch"] = IsOverallMatch,
            ["TotalCriteria"] = IndividualResults.Count,
            ["MatchedCriteria"] = IndividualResults.Count(r => r.IsMatch),
            ["RequiredCriteria"] = IndividualResults.Count(r => r.IsRequired),
            ["RequiredMatched"] = IndividualResults.Count(r => r.IsRequired && r.IsMatch),
            ["TotalExecutionTime"] = TotalExecutionTime.TotalMilliseconds,
            ["CalculatedAt"] = CalculatedAt,
            ["ReasonCode"] = ReasonCode,
            ["Confidence"] = Confidence.GetStatisticalSummary()
        };
    }

    /// <summary>
    /// 获取优化建议
    /// </summary>
    public List<string> GetRecommendations()
    {
        var recommendations = new List<string>();

        // 分析未匹配的必需条件
        var failedRequired = IndividualResults.Where(r => r.IsRequired && !r.IsMatch).ToList();
        if (failedRequired.Any())
        {
            recommendations.Add($"优化必需条件: {string.Join(", ", failedRequired.Select(r => r.CriteriaType))}");
        }

        // 分析低分条件
        var lowScoreCriteria = IndividualResults.Where(r => r.IsMatch && r.MatchScore < 0.3m).ToList();
        if (lowScoreCriteria.Any())
        {
            recommendations.Add($"提升低分条件: {string.Join(", ", lowScoreCriteria.Select(r => r.CriteriaType))}");
        }

        // 分析执行时间过长的条件
        var slowCriteria = IndividualResults.Where(r => r.ExecutionTime.TotalMilliseconds > 100).ToList();
        if (slowCriteria.Any())
        {
            recommendations.Add($"优化执行性能: {string.Join(", ", slowCriteria.Select(r => r.CriteriaType))}");
        }

        // 置信度建议
        if (Confidence.ConfidenceScore < 0.6m)
        {
            recommendations.Add("增加样本数据以提高置信度");
        }

        return recommendations;
    }

    /// <summary>
    /// 检查结果是否有效
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(ResultId) &&
               !string.IsNullOrEmpty(AdCandidateId) &&
               !string.IsNullOrEmpty(AdContextId) &&
               OverallScore >= 0m && OverallScore <= 1m &&
               IndividualResults.All(r => r.IsValidResult()) &&
               Confidence != null;
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public string GetDebugInfo()
    {
        var matchedCount = IndividualResults.Count(r => r.IsMatch);
        var totalCount = IndividualResults.Count;
        var confidence = Confidence.ConfidenceScore;

        var details = $"Match: {IsOverallMatch} Score: {OverallScore:F3} " +
                     $"Criteria: {matchedCount}/{totalCount} " +
                     $"Confidence: {confidence:F3} " +
                     $"Time: {TotalExecutionTime.TotalMilliseconds:F1}ms " +
                     $"Reason: {ReasonCode}";

        return $"OverallMatchResult[{ResultId}] {details}";
    }

    /// <summary>
    /// 计算加权分数
    /// </summary>
    private static IReadOnlyDictionary<string, decimal> CalculateWeightedScores(IList<MatchResult> results)
    {
        var weightedScores = new Dictionary<string, decimal>();
        
        foreach (var result in results)
        {
            weightedScores[result.CriteriaType] = result.GetWeightedScore();
        }

        return weightedScores.AsReadOnly();
    }

    /// <summary>
    /// 计算总体分数
    /// </summary>
    private static decimal CalculateOverallScore(IList<MatchResult> results)
    {
        if (!results.Any()) return 0m;

        var totalWeight = results.Sum(r => r.Weight);
        if (totalWeight == 0m) return 0m;

        var weightedSum = results.Sum(r => r.GetWeightedScore());
        return weightedSum / totalWeight;
    }

    /// <summary>
    /// 判断总体匹配
    /// </summary>
    private static bool DetermineOverallMatch(IList<MatchResult> results, decimal overallScore, decimal threshold)
    {
        // 首先检查所有必需条件是否都匹配
        var requiredResults = results.Where(r => r.IsRequired).ToList();
        if (requiredResults.Any() && requiredResults.Any(r => !r.IsMatch))
        {
            return false;
        }

        // 然后检查总体分数是否超过阈值
        return overallScore >= threshold;
    }

    /// <summary>
    /// 生成原因代码
    /// </summary>
    private static string GenerateReasonCode(bool isMatch, IList<MatchResult> results)
    {
        if (isMatch)
        {
            return "OVERALL_MATCH_SUCCESS";
        }

        // 分析失败原因
        var failedRequired = results.Where(r => r.IsRequired && !r.IsMatch).ToList();
        if (failedRequired.Any())
        {
            return $"REQUIRED_CRITERIA_FAILED: {string.Join(",", failedRequired.Select(r => r.CriteriaType))}";
        }

        return "OVERALL_SCORE_BELOW_THRESHOLD";
    }

    /// <summary>
    /// 计算置信度
    /// </summary>
    private static MatchConfidence CalculateConfidence(IList<MatchResult> results)
    {
        if (!results.Any())
        {
            return MatchConfidence.CreateDefault();
        }

        var scores = results.Select(r => r.MatchScore).ToList();
        var avgScore = scores.Average();
        var sampleSize = results.Count;

        // 基于分数分布计算置信度
        var variance = scores.Select(s => (s - avgScore) * (s - avgScore)).Average();
        var standardDeviation = (decimal)Math.Sqrt((double)variance);

        return MatchConfidence.Create(
            avgScore,
            sampleSize,
            standardDeviation,
            standardDeviation * 2 / (decimal)Math.Sqrt(sampleSize),
            "OverallMatch"
        );
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string adCandidateId, string adContextId, IList<MatchResult> individualResults)
    {
        if (string.IsNullOrWhiteSpace(adCandidateId))
            throw new ArgumentException("广告候选ID不能为空", nameof(adCandidateId));

        if (string.IsNullOrWhiteSpace(adContextId))
            throw new ArgumentException("广告上下文ID不能为空", nameof(adContextId));

        if (individualResults == null)
            throw new ArgumentNullException(nameof(individualResults));
    }
}