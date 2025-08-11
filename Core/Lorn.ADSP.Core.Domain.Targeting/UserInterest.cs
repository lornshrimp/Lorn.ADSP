using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Targeting;

/// <summary>
/// 用户兴趣上下文
/// [需要存储] - 作为定向上下文存储
/// 继承自TargetingContextBase，提供用户兴趣数据的定向上下文功能
/// 合并了原UserInterest实体的功能，专注于广告定向中的兴趣数据
/// </summary>
public class UserInterest : TargetingContextBase
{
    /// <summary>
    /// 上下文名称
    /// </summary>
    public override string ContextName => "用户兴趣上下文";

    /// <summary>
    /// 兴趣类别（如：Sports, Technology, Fashion等）
    /// </summary>
    public string Category => GetPropertyValue("Category", string.Empty);

    /// <summary>
    /// 兴趣分数（0-1之间的值）
    /// </summary>
    public decimal Score => GetPropertyValue("Score", 0.0m);

    /// <summary>
    /// 兴趣权重
    /// </summary>
    public decimal Weight => GetPropertyValue("Weight", 1.0m);

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated => GetPropertyValue("LastUpdated", DateTime.UtcNow);

    /// <summary>
    /// 兴趣子类别列表
    /// </summary>
    public IReadOnlyList<string> SubCategories => GetPropertyValue<List<string>>("SubCategories") ?? new List<string>();

    /// <summary>
    /// 兴趣标签
    /// </summary>
    public IReadOnlyList<string> Tags => GetPropertyValue<List<string>>("Tags") ?? new List<string>();

    /// <summary>
    /// 置信度（0-1之间的值）
    /// </summary>
    public decimal Confidence => GetPropertyValue("Confidence", 1.0m);

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserInterest() : base("UserInterest") { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserInterest(
        string category,
        decimal score,
        decimal weight = 1.0m,
        decimal confidence = 1.0m,
        DateTime? lastUpdated = null,
        IList<string>? subCategories = null,
        IList<string>? tags = null,
        string? dataSource = null)
        : base("UserInterest", CreatePropertyObjects(category, score, weight, confidence, lastUpdated, subCategories, tags), dataSource)
    {
        ValidateInput(category, score, weight, confidence);
    }

    /// <summary>
    /// 创建属性字典
    /// </summary>
    private static IEnumerable<ContextProperty> CreatePropertyObjects(
        string category,
        decimal score,
        decimal weight,
        decimal confidence,
        DateTime? lastUpdated,
        IList<string>? subCategories,
        IList<string>? tags)
    {
        // 基础标量属性保持 String/Decimal 等简单类型
        var props = new List<ContextProperty>
        {
            new ContextProperty("Category", category, "String", "UserInterest"),
            new ContextProperty("Score", Math.Max(0, Math.Min(1, score)).ToString(System.Globalization.CultureInfo.InvariantCulture), "Decimal", "UserInterest"),
            new ContextProperty("Weight", Math.Max(0, weight).ToString(System.Globalization.CultureInfo.InvariantCulture), "Decimal", "UserInterest"),
            new ContextProperty("Confidence", Math.Max(0, Math.Min(1, confidence)).ToString(System.Globalization.CultureInfo.InvariantCulture), "Decimal", "UserInterest"),
            new ContextProperty("LastUpdated", (lastUpdated ?? DateTime.UtcNow).ToString("O"), "DateTime", "UserInterest")
        };

        // 复杂集合属性以 JSON 方式存储，便于后续反序列化
        if (subCategories != null && subCategories.Any())
        {
            var json = System.Text.Json.JsonSerializer.Serialize(subCategories.ToList());
            props.Add(new ContextProperty("SubCategories", json, "Json", "UserInterest"));
        }

        if (tags != null && tags.Any())
        {
            var json = System.Text.Json.JsonSerializer.Serialize(tags.ToList());
            props.Add(new ContextProperty("Tags", json, "Json", "UserInterest"));
        }

        return props;
    }

    /// <summary>
    /// 创建默认用户兴趣
    /// </summary>
    public static UserInterest CreateDefault(string category, string? dataSource = null)
    {
        return new UserInterest(category, 0.5m, 1.0m, 0.8m, dataSource: dataSource);
    }

    /// <summary>
    /// 创建高兴趣度用户兴趣
    /// </summary>
    public static UserInterest CreateHighInterest(
        string category,
        decimal score = 0.8m,
        IList<string>? subCategories = null,
        IList<string>? tags = null,
        string? dataSource = null)
    {
        return new UserInterest(category, score, 1.0m, 0.9m, subCategories: subCategories, tags: tags, dataSource: dataSource);
    }

    /// <summary>
    /// 是否为高兴趣度
    /// </summary>
    public bool IsHighInterest => Score >= 0.7m;

    /// <summary>
    /// 是否为中等兴趣度
    /// </summary>
    public bool IsMediumInterest => Score >= 0.4m && Score < 0.7m;

    /// <summary>
    /// 是否为低兴趣度
    /// </summary>
    public bool IsLowInterest => Score < 0.4m;

    /// <summary>
    /// 是否包含指定子类别
    /// </summary>
    public bool HasSubCategory(string subCategory)
    {
        return SubCategories.Any(sc => string.Equals(sc, subCategory, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 是否包含指定标签
    /// </summary>
    public bool HasTag(string tag)
    {
        return Tags.Any(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取兴趣等级
    /// </summary>
    public string GetInterestLevel()
    {
        return Score switch
        {
            >= 0.8m => "Very High",
            >= 0.6m => "High",
            >= 0.4m => "Medium",
            >= 0.2m => "Low",
            _ => "Very Low"
        };
    }

    /// <summary>
    /// 与另一个兴趣的相似度评分
    /// </summary>
    public decimal GetSimilarityScore(UserInterest other)
    {
        if (other == null || !string.Equals(Category, other.Category, StringComparison.OrdinalIgnoreCase))
            return 0m;

        // 基于分数差异计算相似度
        var scoreDifference = Math.Abs(Score - other.Score);
        var similarityScore = 1m - scoreDifference;

        // 如果有共同的子类别或标签，增加相似度
        var commonSubCategories = SubCategories.Intersect(other.SubCategories, StringComparer.OrdinalIgnoreCase).Count();
        var commonTags = Tags.Intersect(other.Tags, StringComparer.OrdinalIgnoreCase).Count();

        if (commonSubCategories > 0 || commonTags > 0)
            similarityScore += 0.1m * (commonSubCategories + commonTags);

        return Math.Min(1m, similarityScore);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(string category, decimal score, decimal weight, decimal confidence)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("兴趣类别不能为空", nameof(category));

        if (score < 0 || score > 1)
            throw new ArgumentException("兴趣分数必须在0-1之间", nameof(score));

        if (weight < 0)
            throw new ArgumentException("权重不能为负数", nameof(weight));

        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("置信度必须在0-1之间", nameof(confidence));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        return $"UserInterest: Category={Category}, Score={Score:F2}, Weight={Weight:F2}, Confidence={Confidence:F2}, SubCategories={SubCategories.Count}, Tags={Tags.Count}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        if (string.IsNullOrWhiteSpace(Category))
            return false;

        if (Score < 0 || Score > 1)
            return false;

        if (Weight < 0)
            return false;

        if (Confidence < 0 || Confidence > 1)
            return false;

        return true;
    }
}
