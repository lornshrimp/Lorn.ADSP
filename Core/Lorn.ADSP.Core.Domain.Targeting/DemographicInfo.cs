using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Targeting;

/// <summary>
/// 人口统计学信息上下文
/// 继承自TargetingContextBase，提供人口统计学数据的定向上下文功能
/// 合并了原UserDemographic实体和原DemographicInfo的功能，专注于广告定向中的人口统计数据
/// </summary>
public class DemographicInfo : TargetingContextBase
{
    /// <summary>
    /// 上下文名称
    /// </summary>
    public override string ContextName => "人口统计学上下文";

    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName => GetPropertyValue<string>("DisplayName");

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender => GetPropertyValue<Gender>("Gender", Gender.Unknown);

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? DateOfBirth => GetPropertyValue<DateTime?>("DateOfBirth");

    /// <summary>
    /// 年龄（根据出生日期计算）
    /// </summary>
    public int? Age
    {
        get
        {
            if (!DateOfBirth.HasValue) return GetPropertyValue<int?>("Age");
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string? Email => GetPropertyValue<string>("Email");

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber => GetPropertyValue<string>("PhoneNumber");

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone => GetPropertyValue<string>("TimeZone");

    /// <summary>
    /// 首选语言
    /// </summary>
    public string? PreferredLanguage => GetPropertyValue<string>("PreferredLanguage");

    /// <summary>
    /// 教育程度
    /// </summary>
    public string? Education => GetPropertyValue<string>("Education");

    /// <summary>
    /// 收入水平
    /// </summary>
    public string? IncomeLevel => GetPropertyValue<string>("IncomeLevel");

    /// <summary>
    /// 婚姻状态
    /// </summary>
    public string? MaritalStatus => GetPropertyValue<string>("MaritalStatus");

    /// <summary>
    /// 职业
    /// </summary>
    public string? Occupation => GetPropertyValue<string>("Occupation");

    /// <summary>
    /// 行业
    /// </summary>
    public string? Industry => GetPropertyValue<string>("Industry");

    /// <summary>
    /// 语言偏好（多语言支持）
    /// </summary>
    public IReadOnlyList<string> Languages => GetPropertyValue<List<string>>("Languages") ?? new List<string>();

    /// <summary>
    /// 家庭规模
    /// </summary>
    public int? HouseholdSize => GetPropertyValue<int?>("HouseholdSize");

    /// <summary>
    /// 是否有孩子
    /// </summary>
    public bool? HasChildren => GetPropertyValue<bool?>("HasChildren");

    /// <summary>
    /// 生活方式标签
    /// </summary>
    public IReadOnlyList<string> LifestyleTags => GetPropertyValue<List<string>>("LifestyleTags") ?? new List<string>();

    /// <summary>
    /// 置信度（从原UserDemographic合并）- 0-1之间的值
    /// </summary>
    public decimal Confidence => GetPropertyValue("Confidence", 1.0m);

    /// <summary>
    /// 自定义属性集合（从原UserDemographic合并）
    /// 用于存储不在预定义字段中的人口统计学信息
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomProperties
    {
        get
        {
            var customProps = new Dictionary<string, object>();
            var predefinedKeys = new HashSet<string>
            {
                "DisplayName", "Gender", "DateOfBirth", "Age", "Email", "PhoneNumber",
                "TimeZone", "PreferredLanguage", "Education", "IncomeLevel", "MaritalStatus",
                "Occupation", "Industry", "Languages", "HouseholdSize", "HasChildren",
                "LifestyleTags", "Confidence"
            };

            foreach (var property in Properties)
            {
                if (!predefinedKeys.Contains(property.PropertyKey))
                {
                    try
                    {
                        var value = System.Text.Json.JsonSerializer.Deserialize<object>(property.PropertyValue);
                        customProps[property.PropertyKey] = value ?? property.PropertyValue;
                    }
                    catch
                    {
                        customProps[property.PropertyKey] = property.PropertyValue;
                    }
                }
            }

            return customProps.AsReadOnly();
        }
    }

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
        decimal confidence = 1.0m,
        IDictionary<string, object>? customProperties = null,
        string? dataSource = null)
        : base("Demographic", CreateProperties(displayName, gender, dateOfBirth, age, email, phoneNumber, timeZone, preferredLanguage, education, incomeLevel, maritalStatus, occupation, industry, languages, householdSize, hasChildren, lifestyleTags, confidence, customProperties), dataSource)
    {
        ValidateInput(confidence, age, householdSize);
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
        IList<string>? lifestyleTags,
        decimal confidence,
        IDictionary<string, object>? customProperties)
    {
        var properties = new Dictionary<string, object>
        {
            ["Gender"] = gender,
            ["Confidence"] = Math.Max(0, Math.Min(1, confidence))
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

        // 添加自定义属性
        if (customProperties != null)
        {
            foreach (var kvp in customProperties)
            {
                if (!properties.ContainsKey(kvp.Key))
                    properties[kvp.Key] = kvp.Value;
            }
        }

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
            displayName, gender, dateOfBirth, null, email, phoneNumber, timeZone,
            preferredLanguage, education, incomeLevel, maritalStatus, occupation,
            industry, languages, householdSize, hasChildren, lifestyleTags,
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
    /// 从原UserDemographic数据创建（兼容方法）
    /// </summary>
    public static DemographicInfo FromUserDemographic(
        string propertyName,
        string propertyValue,
        string dataType = "String",
        decimal confidence = 1.0m,
        string? dataSource = null)
    {
        var customProperties = new Dictionary<string, object>
        {
            [propertyName] = propertyValue
        };

        return new DemographicInfo(
            confidence: confidence,
            customProperties: customProperties,
            dataSource: dataSource);
    }

    /// <summary>
    /// 获取信息完整度（0-1之间的值）
    /// </summary>
    public decimal GetCompleteness()
    {
        var totalFields = 16; // 预定义字段数量
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

        return (decimal)filledFields / totalFields;
    }

    /// <summary>
    /// 获取年龄组
    /// </summary>
    public string GetAgeGroup()
    {
        if (!Age.HasValue) return "Unknown";

        return Age.Value switch
        {
            < 18 => "Minor",
            >= 18 and < 25 => "Young Adult",
            >= 25 and < 35 => "Adult",
            >= 35 and < 50 => "Middle-aged",
            >= 50 and < 65 => "Mature",
            >= 65 => "Senior"
        };
    }

    /// <summary>
    /// 是否为目标人群
    /// </summary>
    public bool IsTargetDemographic(Gender? targetGender = null, int? minAge = null, int? maxAge = null, string? targetEducation = null, string? targetIncomeLevel = null)
    {
        if (targetGender.HasValue && Gender != targetGender.Value)
            return false;

        if (Age.HasValue)
        {
            if (minAge.HasValue && Age.Value < minAge.Value)
                return false;
            if (maxAge.HasValue && Age.Value > maxAge.Value)
                return false;
        }

        if (!string.IsNullOrWhiteSpace(targetEducation) && !string.Equals(Education, targetEducation, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(targetIncomeLevel) && !string.Equals(IncomeLevel, targetIncomeLevel, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    /// <summary>
    /// 是否包含生活方式标签
    /// </summary>
    public bool HasLifestyleTag(string tag)
    {
        return LifestyleTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 是否支持指定语言
    /// </summary>
    public bool SupportsLanguage(string language)
    {
        return Languages.Contains(language, StringComparer.OrdinalIgnoreCase) ||
               string.Equals(PreferredLanguage, language, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取自定义属性值
    /// </summary>
    public T? GetCustomProperty<T>(string propertyName)
    {
        if (CustomProperties.TryGetValue(propertyName, out var value))
        {
            try
            {
                if (value is T directValue)
                    return directValue;

                if (value is string stringValue)
                    return System.Text.Json.JsonSerializer.Deserialize<T>(stringValue);

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
        return default;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(decimal confidence, int? age, int? householdSize)
    {
        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("置信度必须在0-1之间", nameof(confidence));

        if (age.HasValue && (age.Value < 0 || age.Value > 150))
            throw new ArgumentException("年龄必须在0-150之间", nameof(age));

        if (householdSize.HasValue && (householdSize.Value < 1 || householdSize.Value > 20))
            throw new ArgumentException("家庭规模必须在1-20之间", nameof(householdSize));
    }

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public override string GetDebugInfo()
    {
        var ageInfo = Age.HasValue ? $"Age={Age}" : "Age=Unknown";
        var completeness = GetCompleteness();
        return $"DemographicInfo: Gender={Gender}, {ageInfo}, Completeness={completeness:P1}, Confidence={Confidence:F2}, CustomProps={CustomProperties.Count}";
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        if (Confidence < 0 || Confidence > 1)
            return false;

        if (Age.HasValue && (Age.Value < 0 || Age.Value > 150))
            return false;

        if (HouseholdSize.HasValue && (HouseholdSize.Value < 1 || HouseholdSize.Value > 20))
            return false;

        // 验证邮箱格式（如果提供）
        if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
            return false;

        return true;
    }
}
