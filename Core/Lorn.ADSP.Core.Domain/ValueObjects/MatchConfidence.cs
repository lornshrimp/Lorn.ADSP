using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 匹配置信度值对象
/// 表示OverallMatchResult计算结果的统计置信度，提供匹配结果可靠性的量化评估
/// </summary>
public class MatchConfidence : ValueObject
{
    /// <summary>
    /// 置信度分数（0-1）
    /// </summary>
    public decimal ConfidenceScore { get; private set; }

    /// <summary>
    /// 样本数量
    /// </summary>
    public int SampleSize { get; private set; }

    /// <summary>
    /// 标准差
    /// </summary>
    public decimal StandardDeviation { get; private set; }

    /// <summary>
    /// 置信区间
    /// </summary>
    public decimal ConfidenceInterval { get; private set; }

    /// <summary>
    /// 置信度等级枚举
    /// </summary>
    public ConfidenceLevel Level { get; private set; }

    /// <summary>
    /// 置信度计算方法描述
    /// </summary>
    public string CalculationMethod { get; private set; }

    /// <summary>
    /// 详细统计指标
    /// </summary>
    public IReadOnlyList<ContextProperty> StatisticalMetrics { get; private set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// 是否可靠标识
    /// </summary>
    public bool IsReliable { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private MatchConfidence(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        ConfidenceLevel level,
        string calculationMethod,
        IReadOnlyList<ContextProperty> statisticalMetrics,
        DateTime lastUpdated,
        bool isReliable)
    {
        ConfidenceScore = confidenceScore;
        SampleSize = sampleSize;
        StandardDeviation = standardDeviation;
        ConfidenceInterval = confidenceInterval;
        Level = level;
        CalculationMethod = calculationMethod;
        StatisticalMetrics = statisticalMetrics;
        LastUpdated = lastUpdated;
        IsReliable = isReliable;
    }

    /// <summary>
    /// 创建匹配置信度
    /// </summary>
    public static MatchConfidence Create(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        string calculationMethod = "Standard",
        IEnumerable<ContextProperty>? statisticalMetrics = null)
    {
        ValidateInputs(confidenceScore, sampleSize, standardDeviation, confidenceInterval);

        var level = DetermineConfidenceLevel(confidenceScore);
        var isReliable = DetermineReliability(confidenceScore, sampleSize);
        var metrics = statisticalMetrics?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchConfidence(
            confidenceScore,
            sampleSize,
            standardDeviation,
            confidenceInterval,
            level,
            calculationMethod,
            metrics,
            DateTime.UtcNow,
            isReliable);
    }

    /// <summary>
    /// 创建匹配置信度（从字典参数）
    /// </summary>
    public static MatchConfidence CreateFromDictionary(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        string calculationMethod = "Standard",
        IDictionary<string, decimal>? statisticalMetrics = null)
    {
        var metrics = statisticalMetrics?.Select(kvp => new ContextProperty(
            kvp.Key,
            kvp.Value.ToString(),
            "Decimal",
            "StatisticalMetric",
            false,
            1.0m,
            null,
            "MatchConfidence")) ?? Enumerable.Empty<ContextProperty>();

        return Create(confidenceScore, sampleSize, standardDeviation, confidenceInterval, calculationMethod, metrics);
    }

    /// <summary>
    /// 创建默认置信度（用于缺乏统计数据的场景）
    /// </summary>
    public static MatchConfidence CreateDefault()
    {
        return Create(
            0.5m,
            1,
            0.0m,
            0.0m,
            "Default"
        );
    }

    /// <summary>
    /// 创建高置信度
    /// </summary>
    public static MatchConfidence CreateHigh(int sampleSize, decimal standardDeviation = 0.1m)
    {
        return Create(
            0.85m,
            sampleSize,
            standardDeviation,
            0.05m,
            "HighConfidence"
        );
    }

    /// <summary>
    /// 获取统计度量值
    /// </summary>
    public T? GetStatisticalMetric<T>(string metricKey) where T : struct
    {
        var metric = StatisticalMetrics.FirstOrDefault(m => m.PropertyKey == metricKey);
        return metric?.GetValue<T>();
    }

    /// <summary>
    /// 获取统计度量值（引用类型）
    /// </summary>
    public T? GetStatisticalMetricRef<T>(string metricKey) where T : class
    {
        var metric = StatisticalMetrics.FirstOrDefault(m => m.PropertyKey == metricKey);
        return metric?.GetValue<T>();
    }

    /// <summary>
    /// 获取所有统计度量作为字典（向后兼容）
    /// </summary>
    public Dictionary<string, decimal> GetStatisticalMetricsAsDictionary()
    {
        var result = new Dictionary<string, decimal>();
        foreach (var metric in StatisticalMetrics)
        {
            if (decimal.TryParse(metric.PropertyValue, out decimal value))
            {
                result[metric.PropertyKey] = value;
            }
        }
        return result;
    }

    /// <summary>
    /// 检查是否包含特定的统计度量
    /// </summary>
    public bool HasStatisticalMetric(string metricKey)
    {
        return StatisticalMetrics.Any(m => m.PropertyKey == metricKey);
    }

    /// <summary>
    /// 获取置信度等级
    /// </summary>
    public ConfidenceLevel GetConfidenceLevel()
    {
        return Level;
    }

    /// <summary>
    /// 基于新样本更新统计信息
    /// </summary>
    public MatchConfidence UpdateStatistics(IList<decimal> newSamples)
    {
        if (newSamples == null || !newSamples.Any())
            return this;

        var totalSamples = SampleSize + newSamples.Count;
        var allSamples = new List<decimal> { ConfidenceScore }.Concat(newSamples).ToList();

        var newMean = allSamples.Average();
        var newStandardDeviation = CalculateStandardDeviation(allSamples, newMean);
        var newConfidenceInterval = CalculateConfidenceInterval(newStandardDeviation, totalSamples);

        // 创建更新的统计度量，添加新的样本统计信息
        var updatedMetrics = StatisticalMetrics.ToList();
        updatedMetrics.Add(new ContextProperty("SampleCount", newSamples.Count.ToString(), "Int32", "UpdatedStatistic", false, 1.0m, null, "UpdateStatistics"));
        updatedMetrics.Add(new ContextProperty("NewMean", newMean.ToString(), "Decimal", "UpdatedStatistic", false, 1.0m, null, "UpdateStatistics"));

        return Create(
            newMean,
            totalSamples,
            newStandardDeviation,
            newConfidenceInterval,
            $"{CalculationMethod}_Updated",
            updatedMetrics
        );
    }

    /// <summary>
    /// 获取统计摘要
    /// </summary>
    public Dictionary<string, object> GetStatisticalSummary()
    {
        return new Dictionary<string, object>
        {
            ["ConfidenceScore"] = ConfidenceScore,
            ["Level"] = Level.ToString(),
            ["SampleSize"] = SampleSize,
            ["StandardDeviation"] = StandardDeviation,
            ["ConfidenceInterval"] = ConfidenceInterval,
            ["IsReliable"] = IsReliable,
            ["ReliabilityRating"] = GetReliabilityRating().ToString(),
            ["CalculationMethod"] = CalculationMethod,
            ["LastUpdated"] = LastUpdated
        };
    }

    /// <summary>
    /// 是否超过阈值
    /// </summary>
    public bool IsAboveThreshold(decimal threshold)
    {
        return ConfidenceScore >= threshold;
    }

    /// <summary>
    /// 获取可靠性评级
    /// </summary>
    public ReliabilityRating GetReliabilityRating()
    {
        if (!IsReliable) return ReliabilityRating.Unreliable;

        return Level switch
        {
            ConfidenceLevel.VeryHigh => ReliabilityRating.FullReliability,
            ConfidenceLevel.High => ReliabilityRating.HighReliability,
            ConfidenceLevel.Medium => ReliabilityRating.BasicReliability,
            ConfidenceLevel.Low => ReliabilityRating.LimitedReliability,
            _ => ReliabilityRating.Unreliable
        };
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ConfidenceScore;
        yield return SampleSize;
        yield return StandardDeviation;
        yield return ConfidenceInterval;
        yield return Level;
        yield return CalculationMethod;
        yield return IsReliable;

        // 集合的相等性比较
        foreach (var metric in StatisticalMetrics.OrderBy(m => m.PropertyKey))
        {
            yield return metric;
        }
    }

    /// <summary>
    /// 确定置信度等级
    /// </summary>
    private static ConfidenceLevel DetermineConfidenceLevel(decimal confidenceScore)
    {
        return confidenceScore switch
        {
            >= 0.8m => ConfidenceLevel.VeryHigh,
            >= 0.6m => ConfidenceLevel.High,
            >= 0.4m => ConfidenceLevel.Medium,
            >= 0.2m => ConfidenceLevel.Low,
            _ => ConfidenceLevel.VeryLow
        };
    }

    /// <summary>
    /// 确定可靠性
    /// </summary>
    private static bool DetermineReliability(decimal confidenceScore, int sampleSize)
    {
        // 置信度超过0.6且样本数量大于10被认为是可靠的
        return confidenceScore >= 0.6m && sampleSize >= 10;
    }

    /// <summary>
    /// 计算标准差
    /// </summary>
    private static decimal CalculateStandardDeviation(IList<decimal> samples, decimal mean)
    {
        if (samples.Count <= 1) return 0m;

        var variance = samples.Select(x => (x - mean) * (x - mean)).Average();
        return (decimal)Math.Sqrt((double)variance);
    }

    /// <summary>
    /// 计算置信区间
    /// </summary>
    private static decimal CalculateConfidenceInterval(decimal standardDeviation, int sampleSize)
    {
        if (sampleSize <= 1) return 0m;

        // 使用95%置信区间的t值近似计算
        var tValue = 1.96m; // 95%置信区间的z值
        return tValue * standardDeviation / (decimal)Math.Sqrt(sampleSize);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(decimal confidenceScore, int sampleSize, decimal standardDeviation, decimal confidenceInterval)
    {
        if (confidenceScore < 0m || confidenceScore > 1m)
            throw new ArgumentException("置信度分数必须在0-1之间", nameof(confidenceScore));

        if (sampleSize < 1)
            throw new ArgumentException("样本数量必须大于0", nameof(sampleSize));

        if (standardDeviation < 0m)
            throw new ArgumentException("标准差不能为负数", nameof(standardDeviation));

        if (confidenceInterval < 0m)
            throw new ArgumentException("置信区间不能为负数", nameof(confidenceInterval));
    }
}