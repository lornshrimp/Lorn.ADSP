using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 用户基础信息值对象
/// </summary>
public class UserBasicInfo : ValueObject
{
    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; private set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? DateOfBirth { get; private set; }

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// 首选语言
    /// </summary>
    public string? PreferredLanguage { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private UserBasicInfo() { }

    /// <summary>
    /// 构造函数
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
    /// 创建默认基础信息
    /// </summary>
    public static UserBasicInfo CreateDefault()
    {
        return new UserBasicInfo();
    }

    /// <summary>
    /// 创建基础信息
    /// </summary>
    public static UserBasicInfo CreateBasic(
        string? displayName = null,
        Gender? gender = null,
        DateTime? dateOfBirth = null)
    {
        return new UserBasicInfo(displayName, gender ?? Gender.Unknown, dateOfBirth);
    }

    /// <summary>
    /// 获取相等性比较的组件
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