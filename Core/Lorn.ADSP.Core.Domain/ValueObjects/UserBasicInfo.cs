using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �û�������Ϣֵ����
/// </summary>
public class UserBasicInfo : ValueObject
{
    /// <summary>
    /// ��ʾ����
    /// </summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// �Ա�
    /// </summary>
    public Gender Gender { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public DateTime? DateOfBirth { get; private set; }

    /// <summary>
    /// �����ʼ�
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// �ֻ�����
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// ͷ��URL
    /// </summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// ʱ��
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// ��ѡ����
    /// </summary>
    public string? PreferredLanguage { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserBasicInfo() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public UserBasicInfo(
        string? displayName = null,
        Gender gender = Gender.Unknown,
        DateTime? dateOfBirth = null,
        string? email = null,
        string? phoneNumber = null,
        string? avatarUrl = null,
        string? timeZone = null,
        string? preferredLanguage = null)
    {
        DisplayName = displayName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Email = email;
        PhoneNumber = phoneNumber;
        AvatarUrl = avatarUrl;
        TimeZone = timeZone;
        PreferredLanguage = preferredLanguage;
    }

    /// <summary>
    /// ����Ĭ�ϻ�����Ϣ
    /// </summary>
    public static UserBasicInfo CreateDefault()
    {
        return new UserBasicInfo();
    }

    /// <summary>
    /// ����������Ϣ
    /// </summary>
    public static UserBasicInfo CreateBasic(
        string? displayName = null,
        Gender? gender = null,
        DateTime? dateOfBirth = null)
    {
        return new UserBasicInfo(displayName, gender ?? Gender.Unknown, dateOfBirth);
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DisplayName ?? string.Empty;
        yield return Gender;
        yield return DateOfBirth ?? DateTime.MinValue;
        yield return Email ?? string.Empty;
        yield return PhoneNumber ?? string.Empty;
        yield return AvatarUrl ?? string.Empty;
        yield return TimeZone ?? string.Empty;
        yield return PreferredLanguage ?? string.Empty;
    }
}