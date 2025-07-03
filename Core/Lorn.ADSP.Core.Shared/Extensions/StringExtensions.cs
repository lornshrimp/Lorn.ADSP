namespace Lorn.ADSP.Core.Shared.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 判断字符串是否为空或空白字符
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>是否为空或空白</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 判断字符串是否不为空且不为空白字符
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>是否有值</returns>
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 安全截取字符串
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">超长时的后缀</param>
    /// <returns>截取后的字符串</returns>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Length <= maxLength)
            return value;

        var truncateLength = Math.Max(0, maxLength - suffix.Length);
        return value[..truncateLength] + suffix;
    }

    /// <summary>
    /// 转换为帕斯卡命名法 (PascalCase)
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>帕斯卡命名的字符串</returns>
    public static string ToPascalCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = value.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = string.Join("", words.Select(word => 
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
        
        return result;
    }

    /// <summary>
    /// 转换为驼峰命名法 (camelCase)
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>驼峰命名的字符串</returns>
    public static string ToCamelCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var pascalCase = value.ToPascalCase();
        if (pascalCase.Length == 0)
            return pascalCase;

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
    }

    /// <summary>
    /// 转换为下划线命名法 (snake_case)
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>下划线命名的字符串</returns>
    public static string ToSnakeCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = System.Text.RegularExpressions.Regex.Replace(
            value, 
            @"([a-z])([A-Z])", 
            "$1_$2");
        
        return result.ToLowerInvariant();
    }

    /// <summary>
    /// 移除字符串中的HTML标签
    /// </summary>
    /// <param name="value">包含HTML的字符串</param>
    /// <returns>纯文本字符串</returns>
    public static string StripHtml(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return System.Text.RegularExpressions.Regex.Replace(value, "<.*?>", string.Empty);
    }

    /// <summary>
    /// 掩码敏感信息
    /// </summary>
    /// <param name="value">原始字符串</param>
    /// <param name="visibleLength">可见字符长度</param>
    /// <param name="maskChar">掩码字符</param>
    /// <returns>掩码后的字符串</returns>
    public static string Mask(this string? value, int visibleLength = 4, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        if (value.Length <= visibleLength)
            return new string(maskChar, value.Length);

        var visiblePart = value[..visibleLength];
        var maskPart = new string(maskChar, value.Length - visibleLength);
        
        return visiblePart + maskPart;
    }
}