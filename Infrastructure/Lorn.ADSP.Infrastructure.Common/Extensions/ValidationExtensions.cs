using System.ComponentModel.DataAnnotations;

namespace Lorn.ADSP.Infrastructure.Common.Extensions;

/// <summary>
/// 验证扩展方法
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// 验证对象是否符合数据注解验证规则
    /// </summary>
    /// <param name="obj">要验证的对象</param>
    /// <returns>验证结果</returns>
    public static Lorn.ADSP.Core.Shared.Entities.ValidationResult ValidateObject(this object obj)
    {
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);

        return Lorn.ADSP.Core.Shared.Entities.ValidationResult.Create(isValid, results.Select(r => r.ErrorMessage ?? "Unknown error").ToList());

    }

    /// <summary>
    /// 验证对象属性值
    /// </summary>
    /// <param name="obj">要验证的对象</param>
    /// <param name="propertyName">属性名称</param>
    /// <param name="value">属性值</param>
    /// <returns>验证结果</returns>
    public static Lorn.ADSP.Core.Shared.Entities.ValidationResult ValidateProperty(this object obj, string propertyName, object? value)
    {
        var context = new ValidationContext(obj) { MemberName = propertyName };
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateProperty(value, context, results);

        return Lorn.ADSP.Core.Shared.Entities.ValidationResult.Create(isValid, results.Select(r => r.ErrorMessage ?? "Unknown error").ToList());
    }

    /// <summary>
    /// 验证多个对象
    /// </summary>
    /// <param name="objects">要验证的对象列表</param>
    /// <returns>验证结果字典</returns>
    public static Dictionary<object, Lorn.ADSP.Core.Shared.Entities.ValidationResult> ValidateObjects(this IEnumerable<object> objects)
    {
        var results = new Dictionary<object, Lorn.ADSP.Core.Shared.Entities.ValidationResult>();

        foreach (var obj in objects)
        {
            results[obj] = obj.ValidateObject();
        }

        return results;
    }

    /// <summary>
    /// 检查验证结果是否全部有效
    /// </summary>
    /// <param name="validationResults">验证结果字典</param>
    /// <returns>是否全部有效</returns>
    public static bool AllValid(this Dictionary<object, Lorn.ADSP.Core.Shared.Entities.ValidationResult> validationResults)
    {
        return validationResults.Values.All(r => r.IsValid);
    }

    /// <summary>
    /// 获取所有验证错误信息
    /// </summary>
    /// <param name="validationResults">验证结果字典</param>
    /// <returns>错误信息列表</returns>
    public static List<string> GetAllErrors(this Dictionary<object, Lorn.ADSP.Core.Shared.Entities.ValidationResult> validationResults)
    {
        var errors = new List<string>();

        foreach (var kvp in validationResults)
        {
            if (!kvp.Value.IsValid)
            {
                errors.AddRange(kvp.Value.Errors.Select(e => e.Message));
            }
        }

        return errors;
    }
}