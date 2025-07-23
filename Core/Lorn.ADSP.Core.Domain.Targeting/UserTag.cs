using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// 用户标签上下文
/// [需要存储] - 作为定向上下文存储
/// 继承自TargetingContextBase，提供用户标签数据的定向上下文功能
/// 合并了原UserTag实体的功能，专注于广告定向中的标签数据
/// </summary>
public class UserTag : TargetingContextBase
{
    /// <summary>
    /// 上下文名称
    /// </summary>
    public override string ContextName => "用户标签上下文";

    /// <summary>
    /// 标签名称
    /// </summary>
    public string TagName => GetPropertyValue("TagName", string.Empty);

    /// <summary>
    /// 标签类型（如：Interest, Behavior, Demographics等）
    /// </summary>
    public string TagType => GetPropertyValue("TagType", string.Empty);

    /// <summary>
    /// 标签来源（算法、人工标注等）
    /// </summary>
    public string TagSource => GetPropertyValue("TagSource", string.Empty);

    /// <summary>
    /// 标签置信度（0-1之间的值）
    /// </summary>
    public decimal Confidence => GetPropertyValue("Confidence", 1.0m);

    /// <summary>
    /// 标签权重
    /// </summary>
    public decimal Weight => GetPropertyValue("Weight", 1.0m);

    /// <summary>
    /// 标签分配时间
    /// </summary>
    public DateTime AssignedAt => GetPropertyValue("AssignedAt", DateTime.UtcNow);

    /// <summary>
    /// 标签过期时间（可选）
    /// </summary>
    public DateTime? ExpiresAt => GetPropertyValue<DateTime?>("ExpiresAt");

    /// <summary>
    /// 标签值（用于存储标签的具体值，如数值、布尔值等）
    /// </summary>
    public string? TagValue => GetPropertyValue<string>("TagValue");

    /// <summary>
    /// 标签描述
    /// </summary>
    public string? Description => GetPropertyValue<string>("Description");

    /// <summary>
    /// 标签分类（用于组织标签层次结构）
    /// </summary>
    public string? Category => GetPropertyValue<string>("Category");

    /// <summary>
    /// 相关标签列表
    /// </summary>
    public IReadOnlyList<string> RelatedTags => GetPropertyValue<List<string>>("RelatedTags") ?? new List<string>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserTag() : base("UserTag") { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserTag(
        string tagName,
        string tagType,
        string? tagSource = null,
        decimal confidence = 1.0m,
        decimal weight = 1.0m,
        DateTime? assignedAt = null,
        DateTime? expiresAt = null,
        string? tagValue = null,
        string? description = null,
        string? category = null,
        IList<string>? relatedTags = null,
        string? dataSource = null)
        : base("UserTag", CreateProperties(tagName, tagType, tagSource, confidence, weight, assignedAt, expiresAt, tagValue, description, category, relatedTags), dataSource)
    {
        ValidateInput(tagName, tagType, confidence, weight);
    }

    /// <summary>
    /// 创建属性字典
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        string tagName,
        string tagType,
        string? tagSource,
        decimal confidence,
        decimal weight,
        DateTime? assignedAt,
        DateTime? expiresAt,
        string? tagValue,
        string? description,
        string? category,
        IList<string>? relatedTags)
    {
        var properties = new Dictionary<string, object>
        {
            ["TagName"] = tagName,
            ["TagType"] = tagType,
            ["Confidence"] = Math.Max(0, Math.Min(1, confidence)),
            ["Weight"] = Math.Max(0, weight),
            ["AssignedAt"] = assignedAt ?? DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(tagSource))
            properties["TagSource"] = tagSource;

        if (expiresAt.HasValue)
            properties["ExpiresAt"] = expiresAt.Value;

        if (!string.IsNullOrWhiteSpace(tagValue))
            properties["TagValue"] = tagValue;

        if (!string.IsNullOrWhiteSpace(description))
            properties["Description"] = description;

        if (!string.IsNullOrWhiteSpace(category))
            properties["Category"] = category;

        if (relatedTags != null && relatedTags.Any())
            properties["RelatedTags"] = relatedTags.ToList();

        return properties;
    }

    /// <summary>
    /// 创建兴趣类型标签
    /// </summary>
    public static UserTag CreateInterestTag(
        string tagName,
        string? tagValue = null,
        decimal confidence = 0.8m,
        string? dataSource = null)
    {
        return new UserTag(tagName, "Interest", "Algorithm", confidence, 1.0m, tagValue: tagValue, dataSource: dataSource);
    }

    /// <summary>
    /// 创建行为类型标签
    /// </summary>
    public static UserTag CreateBehaviorTag(
        string tagName,
        string? tagValue = null,
        decimal confidence = 0.9m,
        string? dataSource = null)
    {
        return new UserTag(tagName, "Behavior", "System", confidence, 1.0m, tagValue: tagValue, dataSource: dataSource);
    }

    /// <summary>
    /// 创建人口统计学标签
    /// </summary>
    public static UserTag CreateDemographicTag(
        string tagName,
        string? tagValue = null,
        decimal confidence = 1.0m,
        string? dataSource = null)
    {
        return new UserTag(tagName, "Demographics", "UserInput", confidence, 1.0m, tagValue: tagValue, dataSource: dataSource);
    }

    /// <summary>
    /// 创建分群标签
    /// </summary>
    public static UserTag CreateSegmentTag(
        string tagName,
        string? description = null,
        decimal confidence = 0.85m,
        string? dataSource = null)
    {
        return new UserTag(tagName, "Segment", "Algorithm", confidence, 1.0m, description: description, dataSource: dataSource);
    }

    /// <summary>
    /// 创建临时标签（带过期时间）
    /// </summary>
    public static UserTag CreateTemporaryTag(
        string tagName,
        string tagType,
        TimeSpan validDuration,
        string? tagValue = null,
        decimal confidence = 0.7m,
        string? dataSource = null)
    {
        var expiresAt = DateTime.UtcNow.Add(validDuration);
        return new UserTag(tagName, tagType, "Temporary", confidence, 1.0m, expiresAt: expiresAt, tagValue: tagValue, dataSource: dataSource);
    }

    /// <summary>
    /// 是否为高置信度标签
    /// </summary>
    public bool IsHighConfidence => Confidence >= 0.8m;

    /// <summary>
    /// 是否为中等置信度标签
    /// </summary>
    public bool IsMediumConfidence => Confidence >= 0.5m && Confidence < 0.8m;

    /// <summary>
    /// 是否为低置信度标签
    /// </summary>
    public bool IsLowConfidence => Confidence < 0.5m;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsTagExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    /// <summary>
    /// 是否为临时标签
    /// </summary>
    public bool IsTemporary => ExpiresAt.HasValue;

    /// <summary>
    /// 是否为指定类型的标签
    /// </summary>
    public bool IsOfType(string type)
    {
        return string.Equals(TagType, type, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否包含指定的相关标签
    /// </summary>
    public bool HasRelatedTag(string relatedTag)
    {
        return RelatedTags.Any(tag => string.Equals(tag, relatedTag, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取标签的生命周期状态
    /// </summary>
    public string GetLifecycleStatus()
    {
        if (IsTagExpired)
            return "Expired";

        if (IsTemporary)
        {
            var remainingTime = ExpiresAt!.Value - DateTime.UtcNow;
            if (remainingTime.TotalHours < 1)
                return "Expiring Soon";
            if (remainingTime.TotalDays < 1)
                return "Temporary";
        }

        return "Active";
    }

    /// <summary>
    /// 获取置信度等级
    /// </summary>
    public string GetConfidenceLevel()
    {
        return Confidence switch
        {
            >= 0.9m => "Very High",
            >= 0.7m => "High",
            >= 0.5m => "Medium",
            >= 0.3m => "Low",
            _ => "Very Low"
        };
    }

    /// <summary>
    /// 与另一个标签的匹配度评分
    /// </summary>
    public decimal GetMatchScore(UserTag other)
    {
        if (other == null)
            return 0m;

        var nameMatch = string.Equals(TagName, other.TagName, StringComparison.OrdinalIgnoreCase) ? 1m : 0m;
        var typeMatch = string.Equals(TagType, other.TagType, StringComparison.OrdinalIgnoreCase) ? 0.5m : 0m;
        var confidenceMatch = 1m - Math.Abs(Confidence - other.Confidence);

        // 检查相关标签的重叠
        var relatedTagsOverlap = 0m;
        if (RelatedTags.Any() && other.RelatedTags.Any())
        {
            var commonTags = RelatedTags.Intersect(other.RelatedTags, StringComparer.OrdinalIgnoreCase).Count();
            var totalTags = RelatedTags.Count + other.RelatedTags.Count;
            relatedTagsOverlap = totalTags > 0 ? (decimal)commonTags / totalTags : 0m;
        }

        return (nameMatch * 0.4m + typeMatch * 0.2m + confidenceMatch * 0.2m + relatedTagsOverlap * 0.2m);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(string tagName, string tagType, decimal confidence, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("标签名称不能为空", nameof(tagName));

        if (string.IsNullOrWhiteSpace(tagType))
            throw new ArgumentException("标签类型不能为空", nameof(tagType));

        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("置信度必须在0-1之间", nameof(confidence));

        if (weight < 0)
            throw new ArgumentException("权重不能为负数", nameof(weight));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var status = GetLifecycleStatus();
        var expireInfo = ExpiresAt.HasValue ? $", ExpiresAt={ExpiresAt:yyyy-MM-dd HH:mm}" : "";
        return $"UserTag: Name={TagName}, Type={TagType}, Confidence={Confidence:F2}, Weight={Weight:F2}, Status={status}{expireInfo}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        if (string.IsNullOrWhiteSpace(TagName))
            return false;

        if (string.IsNullOrWhiteSpace(TagType))
            return false;

        if (Confidence < 0 || Confidence > 1)
            return false;

        if (Weight < 0)
            return false;

        // 检查过期时间的合理性
        if (ExpiresAt.HasValue && ExpiresAt.Value <= AssignedAt)
            return false;

        return true;
    }
}
