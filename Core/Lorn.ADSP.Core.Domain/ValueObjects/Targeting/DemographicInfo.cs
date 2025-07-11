using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// 人口统计学信息定向上下文
/// 继承自TargetingContextBase，提供人口统计学数据的定向上下文功能
/// 整合了基础用户信息和人口统计学数据，专注于广告定向需求
/// </summary>
public class DemographicInfo : TargetingContextBase
{
    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName => GetProperty<string>("DisplayName");

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender => GetProperty<Gender>("Gender", Gender.Unknown);

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? DateOfBirth => GetProperty<DateTime?>("DateOfBirth");

    /// <summary>
    /// 年龄（根据出生日期计算）
    /// </summary>
    public int? Age
    {
        get
        {
            if (!DateOfBirth.HasValue) return GetProperty<int?>("Age");
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string? Email => GetProperty<string>("Email");

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber => GetProperty<string>("PhoneNumber");

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone => GetProperty<string>("TimeZone");

    /// <summary>
    /// 首选语言
    /// </summary>
    public string? PreferredLanguage => GetProperty<string>("PreferredLanguage");

    /// <summary>
    /// 教育程度
    /// </summary>
    public string? Education => GetProperty<string>("Education");

    /// <summary>
    /// 收入水平
    /// </summary>
    public string? IncomeLevel => GetProperty<string>("IncomeLevel");

    /// <summary>
    /// 婚姻状况
    /// </summary>
    public string? MaritalStatus => GetProperty<string>("MaritalStatus");

    /// <summary>
    /// 职业
    /// </summary>
    public string? Occupation => GetProperty<string>("Occupation");

    /// <summary>
    /// 行业
    /// </summary>
    public string? Industry => GetProperty<string>("Industry");

    /// <summary>
    /// 语言偏好（多语言支持）
    /// </summary>
    public IReadOnlyList<string> Languages => GetProperty<List<string>>("Languages") ?? new List<string>();

    /// <summary>
    /// 家庭规模
    /// </summary>
    public int? HouseholdSize => GetProperty<int?>("HouseholdSize");

    /// <summary>
    /// 是否有孩子
    /// </summary>
    public bool? HasChildren => GetProperty<bool?>("HasChildren");

    /// <summary>
    /// 生活方式标签
    /// </summary>
    public IReadOnlyList<string> LifestyleTags => GetProperty<List<string>>("LifestyleTags") ?? new List<string>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private DemographicInfo() : base("Demographic") { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DemographicInfo(
        string? displayName = null,
        Gender gender = Gender.Unknown,
        DateTime? dateOfBirth = null,
        int? age = null,
        string? email = null,
        string? phoneNumber = null,
        string? timeZone = null,
        string? preferredLanguage = null,
        string? education = null,
        string? incomeLevel = null,
        string? maritalStatus = null,
        string? occupation = null,
        string? industry = null,
        IList<string>? languages = null,
        int? householdSize = null,
        bool? hasChildren = null,
        IList<string>? lifestyleTags = null,
        string? dataSource = null)
        : base("Demographic", CreateProperties(displayName, gender, dateOfBirth, age, email, phoneNumber, timeZone, preferredLanguage, education, incomeLevel, maritalStatus, occupation, industry, languages, householdSize, hasChildren, lifestyleTags), dataSource)
    {
        ValidateHouseholdSize(householdSize);
        ValidateAge(age);
    }

    /// <summary>
    /// 创建属性字典
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        string? displayName,
        Gender gender,
        DateTime? dateOfBirth,
        int? age,
        string? email,
        string? phoneNumber,
        string? timeZone,
        string? preferredLanguage,
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
        var properties = new Dictionary<string, object>
        {
            ["Gender"] = gender
        };

        if (!string.IsNullOrWhiteSpace(displayName))
            properties["DisplayName"] = displayName;

        if (dateOfBirth.HasValue)
            properties["DateOfBirth"] = dateOfBirth.Value;

        if (age.HasValue)
            properties["Age"] = age.Value;

        if (!string.IsNullOrWhiteSpace(email))
            properties["Email"] = email;

        if (!string.IsNullOrWhiteSpace(phoneNumber))
            properties["PhoneNumber"] = phoneNumber;

        if (!string.IsNullOrWhiteSpace(timeZone))
            properties["TimeZone"] = timeZone;

        if (!string.IsNullOrWhiteSpace(preferredLanguage))
            properties["PreferredLanguage"] = preferredLanguage;

        if (!string.IsNullOrWhiteSpace(education))
            properties["Education"] = education;

        if (!string.IsNullOrWhiteSpace(incomeLevel))
            properties["IncomeLevel"] = incomeLevel;

        if (!string.IsNullOrWhiteSpace(maritalStatus))
            properties["MaritalStatus"] = maritalStatus;

        if (!string.IsNullOrWhiteSpace(occupation))
            properties["Occupation"] = occupation;

        if (!string.IsNullOrWhiteSpace(industry))
            properties["Industry"] = industry;

        if (languages != null && languages.Any())
            properties["Languages"] = languages.ToList();

        if (householdSize.HasValue)
            properties["HouseholdSize"] = householdSize.Value;

        if (hasChildren.HasValue)
            properties["HasChildren"] = hasChildren.Value;

        if (lifestyleTags != null && lifestyleTags.Any())
            properties["LifestyleTags"] = lifestyleTags.ToList();

        return properties;
    }

    /// <summary>
    /// 创建基础人口统计信息（原UserBasicInfo功能）
    /// </summary>
    public static DemographicInfo CreateBasic(
        string? displayName = null,
        Gender gender = Gender.Unknown,
        DateTime? dateOfBirth = null,
        string? email = null,
        string? phoneNumber = null,
        string? timeZone = null,
        string? preferredLanguage = null,
        string? dataSource = null)
    {
        return new DemographicInfo(
            displayName: displayName,
            gender: gender,
            dateOfBirth: dateOfBirth,
            email: email,
            phoneNumber: phoneNumber,
            timeZone: timeZone,
            preferredLanguage: preferredLanguage,
            dataSource: dataSource);
    }

    /// <summary>
    /// 创建详细人口统计信息
    /// </summary>
    public static DemographicInfo CreateDetailed(
        string? displayName,
        Gender gender,
        DateTime? dateOfBirth,
        string? email,
        string? phoneNumber,
        string? timeZone,
        string? preferredLanguage,
        string? education,
        string? incomeLevel,
        string? maritalStatus,
        string? occupation,
        string? industry,
        IList<string>? languages,
        int? householdSize,
        bool? hasChildren,
        IList<string>? lifestyleTags,
        string? dataSource = null)
    {
        return new DemographicInfo(
            displayName: displayName,
            gender: gender,
            dateOfBirth: dateOfBirth,
            email: email,
            phoneNumber: phoneNumber,
            timeZone: timeZone,
            preferredLanguage: preferredLanguage,
            education: education,
            incomeLevel: incomeLevel,
            maritalStatus: maritalStatus,
            occupation: occupation,
            industry: industry,
            languages: languages,
            householdSize: householdSize,
            hasChildren: hasChildren,
            lifestyleTags: lifestyleTags,
            dataSource: dataSource);
    }

    /// <summary>
    /// 创建默认人口统计信息
    /// </summary>
    public static DemographicInfo CreateDefault(string? dataSource = null)
    {
        return new DemographicInfo(dataSource: dataSource);
    }

    /// <summary>
    /// 获取数据完整度评分
    /// </summary>
    public decimal GetCompletenessScore()
    {
        var totalFields = 16; // 更新字段总数
        var filledFields = 0;

        if (!string.IsNullOrWhiteSpace(DisplayName)) filledFields++;
        if (Gender != Gender.Unknown) filledFields++;
        if (DateOfBirth.HasValue || Age.HasValue) filledFields++;
        if (!string.IsNullOrWhiteSpace(Email)) filledFields++;
        if (!string.IsNullOrWhiteSpace(PhoneNumber)) filledFields++;
        if (!string.IsNullOrWhiteSpace(TimeZone)) filledFields++;
        if (!string.IsNullOrWhiteSpace(PreferredLanguage)) filledFields++;
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
    /// 是否符合年龄范围定向
    /// </summary>
    public bool MatchesAgeRange(int? minAge, int? maxAge)
    {
        if (!Age.HasValue) return true; // 未知年龄默认匹配

        if (minAge.HasValue && Age.Value < minAge.Value) return false;
        if (maxAge.HasValue && Age.Value > maxAge.Value) return false;

        return true;
    }

    /// <summary>
    /// 是否符合性别定向
    /// </summary>
    public bool MatchesGender(IEnumerable<Gender> targetGenders)
    {
        if (!targetGenders.Any()) return true; // 无性别限制时默认匹配
        return targetGenders.Contains(Gender);
    }

    /// <summary>
    /// 是否包含指定语言
    /// </summary>
    public bool HasLanguage(string language)
    {
        if (!string.IsNullOrWhiteSpace(PreferredLanguage) && 
            string.Equals(PreferredLanguage, language, StringComparison.OrdinalIgnoreCase))
            return true;

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
    /// 是否符合教育程度定向
    /// </summary>
    public bool MatchesEducation(IEnumerable<string> targetEducations)
    {
        if (!targetEducations.Any()) return true;
        if (string.IsNullOrWhiteSpace(Education)) return false;

        return targetEducations.Contains(Education, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否符合收入水平定向
    /// </summary>
    public bool MatchesIncomeLevel(IEnumerable<string> targetIncomeLevels)
    {
        if (!targetIncomeLevels.Any()) return true;
        if (string.IsNullOrWhiteSpace(IncomeLevel)) return false;

        return targetIncomeLevels.Contains(IncomeLevel, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var demographicInfo = $"Gender:{Gender} Age:{Age?.ToString() ?? "Unknown"} Education:{Education} Income:{IncomeLevel} Occupation:{Occupation} Languages:{Languages.Count}";
        return $"{baseInfo} | {demographicInfo}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // 验证年龄合理性
        if (Age.HasValue && (Age.Value < 0 || Age.Value > 150))
            return false;

        // 验证家庭规模范围
        if (HouseholdSize.HasValue && (HouseholdSize.Value < 1 || HouseholdSize.Value > 20))
            return false;

        // 验证邮箱格式（简单验证）
        if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
            return false;

        return true;
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
    /// 验证年龄
    /// </summary>
    private static void ValidateAge(int? age)
    {
        if (age.HasValue && (age.Value < 0 || age.Value > 150))
            throw new ArgumentOutOfRangeException(nameof(age), "年龄必须在0-150之间");
    }
}