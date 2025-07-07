using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �˿�ͳ��ѧ��Ϣֵ����
/// </summary>
public class DemographicInfo : ValueObject
{
    /// <summary>
    /// �����̶�
    /// </summary>
    public string? Education { get; private set; }

    /// <summary>
    /// ����ˮƽ
    /// </summary>
    public string? IncomeLevel { get; private set; }

    /// <summary>
    /// ����״��
    /// </summary>
    public string? MaritalStatus { get; private set; }

    /// <summary>
    /// ְҵ
    /// </summary>
    public string? Occupation { get; private set; }

    /// <summary>
    /// ��ҵ
    /// </summary>
    public string? Industry { get; private set; }

    /// <summary>
    /// ����ƫ��
    /// </summary>
    public IReadOnlyList<string> Languages { get; private set; } = new List<string>();

    /// <summary>
    /// ��ͥ��ģ
    /// </summary>
    public int? HouseholdSize { get; private set; }

    /// <summary>
    /// �Ƿ��к���
    /// </summary>
    public bool? HasChildren { get; private set; }

    /// <summary>
    /// ���ʽ��ǩ
    /// </summary>
    public IReadOnlyList<string> LifestyleTags { get; private set; } = new List<string>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DemographicInfo() { }

    /// <summary>
    /// ���캯��
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
    /// ���������˿�ͳ����Ϣ
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
    /// ������ϸ�˿�ͳ����Ϣ
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
    /// ��ȡ����������
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
    /// �Ƿ����ָ������
    /// </summary>
    public bool HasLanguage(string language)
    {
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
    /// ��֤��ͥ��ģ
    /// </summary>
    private static void ValidateHouseholdSize(int? householdSize)
    {
        if (householdSize.HasValue && (householdSize.Value < 1 || householdSize.Value > 20))
            throw new ArgumentOutOfRangeException(nameof(householdSize), "��ͥ��ģ������1-20֮��");
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
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