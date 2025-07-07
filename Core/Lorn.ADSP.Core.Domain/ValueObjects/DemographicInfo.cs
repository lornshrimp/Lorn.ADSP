using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 人口统计学信息值对象
/// </summary>
public class DemographicInfo : ValueObject
{
    /// <summary>
    /// 教育程度
    /// </summary>
    public string? Education { get; private set; }

    /// <summary>
    /// 收入水平
    /// </summary>
    public string? IncomeLevel { get; private set; }

    /// <summary>
    /// 婚姻状况
    /// </summary>
    public string? MaritalStatus { get; private set; }

    /// <summary>
    /// 职业
    /// </summary>
    public string? Occupation { get; private set; }

    /// <summary>
    /// 行业
    /// </summary>
    public string? Industry { get; private set; }

    /// <summary>
    /// 语言偏好
    /// </summary>
    public IReadOnlyList<string> Languages { get; private set; } = new List<string>();

    /// <summary>
    /// 家庭规模
    /// </summary>
    public int? HouseholdSize { get; private set; }

    /// <summary>
    /// 是否有孩子
    /// </summary>
    public bool? HasChildren { get; private set; }

    /// <summary>
    /// 生活方式标签
    /// </summary>
    public IReadOnlyList<string> LifestyleTags { get; private set; } = new List<string>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private DemographicInfo() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DemographicInfo(
        string? education = null,
        string? incomeLevel = null,
        string? maritalStatus = null,
        string? occupation = null,
        string? industry = null,
        IList<string>? languages = null,
        int? householdSize = null,
        bool? hasChildren = null,
        IList<string>? lifestyleTags = null)
    {
        ValidateHouseholdSize(householdSize);

        Education = education;
        IncomeLevel = incomeLevel;
        MaritalStatus = maritalStatus;
        Occupation = occupation;
        Industry = industry;
        Languages = languages?.ToList() ?? new List<string>();
        HouseholdSize = householdSize;
        HasChildren = hasChildren;
        LifestyleTags = lifestyleTags?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// 创建基本人口统计信息
    /// </summary>
    public static DemographicInfo CreateBasic(
        string? education = null,
        string? incomeLevel = null,
        string? occupation = null)
    {
        return new DemographicInfo(
            education: education,
            incomeLevel: incomeLevel,
            occupation: occupation);
    }

    /// <summary>
    /// 创建详细人口统计信息
    /// </summary>
    public static DemographicInfo CreateDetailed(
        string? education,
        string? incomeLevel,
        string? maritalStatus,
        string? occupation,
        string? industry,
        IList<string>? languages,
        int? householdSize,
        bool? hasChildren,
        IList<string>? lifestyleTags)
    {
        return new DemographicInfo(
            education: education,
            incomeLevel: incomeLevel,
            maritalStatus: maritalStatus,
            occupation: occupation,
            industry: industry,
            languages: languages,
            householdSize: householdSize,
            hasChildren: hasChildren,
            lifestyleTags: lifestyleTags);
    }

    /// <summary>
    /// 获取完整性评分
    /// </summary>
    public decimal GetCompletenessScore()
    {
        var totalFields = 9;
        var filledFields = 0;

        if (!string.IsNullOrWhiteSpace(Education)) filledFields++;
        if (!string.IsNullOrWhiteSpace(IncomeLevel)) filledFields++;
        if (!string.IsNullOrWhiteSpace(MaritalStatus)) filledFields++;
        if (!string.IsNullOrWhiteSpace(Occupation)) filledFields++;
        if (!string.IsNullOrWhiteSpace(Industry)) filledFields++;
        if (Languages.Count > 0) filledFields++;
        if (HouseholdSize.HasValue) filledFields++;
        if (HasChildren.HasValue) filledFields++;
        if (LifestyleTags.Count > 0) filledFields++;

        return (decimal)filledFields / totalFields * 100;
    }

    /// <summary>
    /// 是否包含指定语言
    /// </summary>
    public bool HasLanguage(string language)
    {
        return Languages.Contains(language, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否包含指定生活方式标签
    /// </summary>
    public bool HasLifestyleTag(string tag)
    {
        return LifestyleTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 验证家庭规模
    /// </summary>
    private static void ValidateHouseholdSize(int? householdSize)
    {
        if (householdSize.HasValue && (householdSize.Value < 1 || householdSize.Value > 20))
            throw new ArgumentOutOfRangeException(nameof(householdSize), "家庭规模必须在1-20之间");
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Education ?? string.Empty;
        yield return IncomeLevel ?? string.Empty;
        yield return MaritalStatus ?? string.Empty;
        yield return Occupation ?? string.Empty;
        yield return Industry ?? string.Empty;
        yield return string.Join(",", Languages);
        yield return HouseholdSize ?? 0;
        yield return HasChildren ?? false;
        yield return string.Join(",", LifestyleTags);
    }
}