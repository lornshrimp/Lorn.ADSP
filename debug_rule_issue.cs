using System;
using Lorn.ADSP.Core.Domain.Targeting;

class Program
{
    static void Main()
    {
        // 测试 TargetingRule 的 WithValue 和 GetValue 方法
        Console.WriteLine("=== 调试 TargetingRule 问题 ===");

        // 创建一个基础规则
        var baseRule = new TargetingRule("MinAge", string.Empty, "Int32");
        Console.WriteLine($"基础规则: RuleKey={baseRule.RuleKey}, RuleValue='{baseRule.RuleValue}', DataType={baseRule.DataType}");

        // 使用 WithValue 设置值
        var ruleWithValue = baseRule.WithValue(25);
        Console.WriteLine($"WithValue后: RuleKey={ruleWithValue.RuleKey}, RuleValue='{ruleWithValue.RuleValue}', DataType={ruleWithValue.DataType}");

        // 尝试获取不同类型的值
        var intValue = ruleWithValue.GetValue<int>();
        var nullableIntValue = ruleWithValue.GetValue<int?>();

        Console.WriteLine($"GetValue<int>(): {intValue}");
        Console.WriteLine($"GetValue<int?>(): {nullableIntValue}");

        // 测试类型转换
        try
        {
            var converted = Convert.ChangeType("25", typeof(int?));
            Console.WriteLine($"Convert.ChangeType('25', typeof(int?)): {converted}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Convert.ChangeType失败: {ex.Message}");
        }

        try
        {
            var converted = Convert.ChangeType("25", typeof(int));
            Console.WriteLine($"Convert.ChangeType('25', typeof(int)): {converted}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Convert.ChangeType失败: {ex.Message}");
        }
    }
}
