using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

/// <summary>
/// �˿�ͳ��ѧ��Ϣ����������
/// �̳���TargetingContextBase���ṩ�˿�ͳ��ѧ���ݵĶ��������Ĺ���
/// �����˻����û���Ϣ���˿�ͳ��ѧ���ݣ�רע�ڹ�涨������
/// </summary>
public class DemographicInfo : TargetingContextBase
{
    /// <summary>
    /// ��ʾ����
    /// </summary>
    public string? DisplayName => GetPropertyValue<string>("DisplayName");

    /// <summary>
    /// �Ա�
    /// </summary>
    public Gender Gender => GetPropertyValue<Gender>("Gender", Gender.Unknown);

    /// <summary>
    /// ��������
    /// </summary>
    public DateTime? DateOfBirth => GetPropertyValue<DateTime?>("DateOfBirth");

    /// <summary>
    /// ���䣨���ݳ������ڼ��㣩
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
    /// �����ʼ�
    /// </summary>
    public string? Email => GetPropertyValue<string>("Email");

    /// <summary>
    /// �ֻ�����
    /// </summary>
    public string? PhoneNumber => GetPropertyValue<string>("PhoneNumber");

    /// <summary>
    /// ʱ��
    /// </summary>
    public string? TimeZone => GetPropertyValue<string>("TimeZone");

    /// <summary>
    /// ��ѡ����
    /// </summary>
    public string? PreferredLanguage => GetPropertyValue<string>("PreferredLanguage");

    /// <summary>
    /// �����̶�
    /// </summary>
    public string? Education => GetPropertyValue<string>("Education");

    /// <summary>
    /// ����ˮƽ
    /// </summary>
    public string? IncomeLevel => GetPropertyValue<string>("IncomeLevel");

    /// <summary>
    /// ����״��
    /// </summary>
    public string? MaritalStatus => GetPropertyValue<string>("MaritalStatus");

    /// <summary>
    /// ְҵ
    /// </summary>
    public string? Occupation => GetPropertyValue<string>("Occupation");

    /// <summary>
    /// ��ҵ
    /// </summary>
    public string? Industry => GetPropertyValue<string>("Industry");

    /// <summary>
    /// ����ƫ�ã�������֧�֣�
    /// </summary>
    public IReadOnlyList<string> Languages => GetPropertyValue<List<string>>("Languages") ?? new List<string>();

    /// <summary>
    /// ��ͥ��ģ
    /// </summary>
    public int? HouseholdSize => GetPropertyValue<int?>("HouseholdSize");

    /// <summary>
    /// �Ƿ��к���
    /// </summary>
    public bool? HasChildren => GetPropertyValue<bool?>("HasChildren");

    /// <summary>
    /// ���ʽ��ǩ
    /// </summary>
    public IReadOnlyList<string> LifestyleTags => GetPropertyValue<List<string>>("LifestyleTags") ?? new List<string>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DemographicInfo() : base("Demographic") { }

    /// <summary>
    /// ���캯��
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
    /// ���������ֵ�
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
    /// ���������˿�ͳ����Ϣ��ԭUserBasicInfo���ܣ�
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
    /// ������ϸ�˿�ͳ����Ϣ
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
    /// ����Ĭ���˿�ͳ����Ϣ
    /// </summary>
    public static DemographicInfo CreateDefault(string? dataSource = null)
    {
        return new DemographicInfo(dataSource: dataSource);
    }

    /// <summary>
    /// ��ȡ��������������
    /// </summary>
    public decimal GetCompletenessScore()
    {
        var totalFields = 16; // �����ֶ�����
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
    /// �Ƿ�������䷶Χ����
    /// </summary>
    public bool MatchesAgeRange(int? minAge, int? maxAge)
    {
        if (!Age.HasValue) return true; // δ֪����Ĭ��ƥ��

        if (minAge.HasValue && Age.Value < minAge.Value) return false;
        if (maxAge.HasValue && Age.Value > maxAge.Value) return false;

        return true;
    }

    /// <summary>
    /// �Ƿ�����Ա���
    /// </summary>
    public bool MatchesGender(IEnumerable<Gender> targetGenders)
    {
        if (!targetGenders.Any()) return true; // ���Ա�����ʱĬ��ƥ��
        return targetGenders.Contains(Gender);
    }

    /// <summary>
    /// �Ƿ����ָ������
    /// </summary>
    public bool HasLanguage(string language)
    {
        if (!string.IsNullOrWhiteSpace(PreferredLanguage) &&
            string.Equals(PreferredLanguage, language, StringComparison.OrdinalIgnoreCase))
            return true;

        return Languages.Contains(language, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ����ָ�����ʽ��ǩ
    /// </summary>
    public bool HasLifestyleTag(string tag)
    {
        return LifestyleTags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ���Ͻ����̶ȶ���
    /// </summary>
    public bool MatchesEducation(IEnumerable<string> targetEducations)
    {
        if (!targetEducations.Any()) return true;
        if (string.IsNullOrWhiteSpace(Education)) return false;

        return targetEducations.Contains(Education, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ��������ˮƽ����
    /// </summary>
    public bool MatchesIncomeLevel(IEnumerable<string> targetIncomeLevels)
    {
        if (!targetIncomeLevels.Any()) return true;
        if (string.IsNullOrWhiteSpace(IncomeLevel)) return false;

        return targetIncomeLevels.Contains(IncomeLevel, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var demographicInfo = $"Gender:{Gender} Age:{Age?.ToString() ?? "Unknown"} Education:{Education} Income:{IncomeLevel} Occupation:{Occupation} Languages:{Languages.Count}";
        return $"{baseInfo} | {demographicInfo}";
    }

    /// <summary>
    /// ��֤�����ĵ���Ч��
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // ��֤���������
        if (Age.HasValue && (Age.Value < 0 || Age.Value > 150))
            return false;

        // ��֤��ͥ��ģ��Χ
        if (HouseholdSize.HasValue && (HouseholdSize.Value < 1 || HouseholdSize.Value > 20))
            return false;

        // ��֤�����ʽ������֤��
        if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
            return false;

        return true;
    }

    /// <summary>
    /// ��֤��ͥ��ģ
    /// </summary>
    private static void ValidateHouseholdSize(int? householdSize)
    {
        if (householdSize.HasValue && (householdSize.Value < 1 || householdSize.Value > 20))
            throw new ArgumentOutOfRangeException(nameof(householdSize), "��ͥ��ģ������1-20֮��");
    }

    /// <summary>
    /// ��֤����
    /// </summary>
    private static void ValidateAge(int? age)
    {
        if (age.HasValue && (age.Value < 0 || age.Value > 150))
            throw new ArgumentOutOfRangeException(nameof(age), "���������0-150֮��");
    }
}
