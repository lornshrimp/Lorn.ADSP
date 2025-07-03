using System.ComponentModel;
using System.Reflection;

namespace Lorn.ADSP.Core.Shared.Extensions;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的描述信息
    /// </summary>
    /// <param name="enumValue">枚举值</param>
    /// <returns>描述信息</returns>
    public static string GetDescription(this Enum enumValue)
    {
        if (enumValue == null)
            return string.Empty;

        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null)
            return enumValue.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// 获取枚举的显示名称
    /// </summary>
    /// <param name="enumValue">枚举值</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        if (enumValue == null)
            return string.Empty;

        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null)
            return enumValue.ToString();

        var attribute = field.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        return attribute?.Name ?? GetDescription(enumValue);
    }

    /// <summary>
    /// 判断枚举值是否有效
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="enumValue">枚举值</param>
    /// <returns>是否有效</returns>
    public static bool IsValid<T>(this T enumValue) where T : struct, Enum
    {
        return Enum.IsDefined(typeof(T), enumValue);
    }

    /// <summary>
    /// 将字符串转换为枚举值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <returns>枚举值</returns>
    public static T? ParseEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (Enum.TryParse<T>(value, ignoreCase, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// 获取枚举的所有值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>枚举值列表</returns>
    public static IEnumerable<T> GetValues<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>();
    }

    /// <summary>
    /// 获取枚举的名称和值的字典
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>名称和值的字典</returns>
    public static IDictionary<string, T> GetNameValuePairs<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().ToDictionary(e => e.ToString(), e => e);
    }

    /// <summary>
    /// 获取枚举的描述和值的字典
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>描述和值的字典</returns>
    public static IDictionary<string, T> GetDescriptionValuePairs<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().ToDictionary(e => e.GetDescription(), e => e);
    }
}