using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ƥ����ʵ��
/// �洢AdCandidate��TargetingContext�Ķ���ƥ����������ɶ�����Լ���������
/// �������ڣ��������ɡ�����洢�����ʹ�á���������
/// </summary>
public class OverallMatchResult : EntityBase
{
    /// <summary>
    /// ���Ψһ��ʶ
    /// </summary>
    public string ResultId { get; private set; }

    /// <summary>
    /// �����Ĺ���ѡID
    /// </summary>
    public string AdCandidateId { get; private set; }

    /// <summary>
    /// �����Ĺ��������ID
    /// </summary>
    public string AdContextId { get; private set; }

    /// <summary>
    /// ����ƥ�������0-1��
    /// </summary>
    public decimal OverallScore { get; private set; }

    /// <summary>
    /// �Ƿ�����ƥ��
    /// </summary>
    public bool IsOverallMatch { get; private set; }

    /// <summary>
    /// ����ƥ��������
    /// </summary>
    private readonly List<MatchResult> _individualResults = new();
    public IReadOnlyList<MatchResult> IndividualResults => _individualResults.AsReadOnly();

    /// <summary>
    /// ��Ȩ�����ֲ�
    /// </summary>
    public IReadOnlyDictionary<string, decimal> WeightedScores { get; private set; }

    /// <summary>
    /// ��ִ��ʱ��
    /// </summary>
    public TimeSpan TotalExecutionTime { get; private set; }

    /// <summary>
    /// ƥ���ƥ���ԭ�����
    /// </summary>
    public string ReasonCode { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// ƥ�����Ŷ�
    /// </summary>
    public MatchConfidence Confidence { get; private set; }

    /// <summary>
    /// ˽�й��캯��
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
    /// ��������ƥ����
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

        // �����Ȩ����
        var weightedScores = CalculateWeightedScores(individualResults);
        
        // �����������
        var overallScore = CalculateOverallScore(individualResults);
        
        // �ж��Ƿ�ƥ��
        var isOverallMatch = DetermineOverallMatch(individualResults, overallScore, matchThreshold);
        
        // ������ִ��ʱ��
        var totalExecutionTime = individualResults.Aggregate(TimeSpan.Zero, (total, result) => total + result.ExecutionTime);
        
        // ����ԭ�����
        var reasonCode = GenerateReasonCode(isOverallMatch, individualResults);
        
        // �������Ŷ�
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

        // ��ӵ�����
        result._individualResults.AddRange(individualResults);

        return result;
    }

    /// <summary>
    /// ��ӵ���ƥ����
    /// </summary>
    public void AddIndividualResult(MatchResult matchResult)
    {
        if (matchResult == null)
            throw new ArgumentNullException(nameof(matchResult));

        _individualResults.Add(matchResult);
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ȡƥ������
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
    /// ��ȡ�Ż�����
    /// </summary>
    public List<string> GetRecommendations()
    {
        var recommendations = new List<string>();

        // ����δƥ��ı�������
        var failedRequired = IndividualResults.Where(r => r.IsRequired && !r.IsMatch).ToList();
        if (failedRequired.Any())
        {
            recommendations.Add($"�Ż���������: {string.Join(", ", failedRequired.Select(r => r.CriteriaType))}");
        }

        // �����ͷ�����
        var lowScoreCriteria = IndividualResults.Where(r => r.IsMatch && r.MatchScore < 0.3m).ToList();
        if (lowScoreCriteria.Any())
        {
            recommendations.Add($"�����ͷ�����: {string.Join(", ", lowScoreCriteria.Select(r => r.CriteriaType))}");
        }

        // ����ִ��ʱ�����������
        var slowCriteria = IndividualResults.Where(r => r.ExecutionTime.TotalMilliseconds > 100).ToList();
        if (slowCriteria.Any())
        {
            recommendations.Add($"�Ż�ִ������: {string.Join(", ", slowCriteria.Select(r => r.CriteriaType))}");
        }

        // ���ŶȽ���
        if (Confidence.ConfidenceScore < 0.6m)
        {
            recommendations.Add("��������������������Ŷ�");
        }

        return recommendations;
    }

    /// <summary>
    /// ������Ƿ���Ч
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
    /// ��ȡ������Ϣ
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
    /// �����Ȩ����
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
    /// �����������
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
    /// �ж�����ƥ��
    /// </summary>
    private static bool DetermineOverallMatch(IList<MatchResult> results, decimal overallScore, decimal threshold)
    {
        // ���ȼ�����б��������Ƿ�ƥ��
        var requiredResults = results.Where(r => r.IsRequired).ToList();
        if (requiredResults.Any() && requiredResults.Any(r => !r.IsMatch))
        {
            return false;
        }

        // Ȼ������������Ƿ񳬹���ֵ
        return overallScore >= threshold;
    }

    /// <summary>
    /// ����ԭ�����
    /// </summary>
    private static string GenerateReasonCode(bool isMatch, IList<MatchResult> results)
    {
        if (isMatch)
        {
            return "OVERALL_MATCH_SUCCESS";
        }

        // ����ʧ��ԭ��
        var failedRequired = results.Where(r => r.IsRequired && !r.IsMatch).ToList();
        if (failedRequired.Any())
        {
            return $"REQUIRED_CRITERIA_FAILED: {string.Join(",", failedRequired.Select(r => r.CriteriaType))}";
        }

        return "OVERALL_SCORE_BELOW_THRESHOLD";
    }

    /// <summary>
    /// �������Ŷ�
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

        // ���ڷ����ֲ��������Ŷ�
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
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string adCandidateId, string adContextId, IList<MatchResult> individualResults)
    {
        if (string.IsNullOrWhiteSpace(adCandidateId))
            throw new ArgumentException("����ѡID����Ϊ��", nameof(adCandidateId));

        if (string.IsNullOrWhiteSpace(adContextId))
            throw new ArgumentException("���������ID����Ϊ��", nameof(adContextId));

        if (individualResults == null)
            throw new ArgumentNullException(nameof(individualResults));
    }
}