namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 定向条件抽象接口
    /// 定义统一的定向条件访问规范，支持不同类型的定向规则配置
    /// </summary>
    public interface ITargetingCriteria
    {
        /// <summary>
        /// 条件唯一标识
        /// 用于条件识别和数据库关联
        /// </summary>
        Guid CriteriaId { get; }

        /// <summary>
        /// 条件名称
        /// 用于标识和描述定向条件的友好名称
        /// </summary>
        string CriteriaName { get; }

        /// <summary>
        /// 条件类型标识
        /// 例如：Geo、Demographic、Device、Time、Behavior
        /// </summary>
        string CriteriaType { get; }

        /// <summary>
        /// 定向规则集合
        /// 存储该条件类型的具体规则配置
        /// </summary>
        IReadOnlyList<TargetingRule> Rules { get; }

        /// <summary>
        /// 条件权重
        /// 用于在多个条件组合时计算加权分数，默认为1.0
        /// </summary>
        decimal Weight { get; }

        /// <summary>
        /// 是否启用该条件
        /// 可用于A/B测试或动态开关条件
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// 条件创建时间
        /// 用于版本管理和缓存失效
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// 条件最后更新时间
        /// 用于追踪条件变更历史
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// 获取指定类型的规则值
        /// </summary>
        /// <typeparam name="T">规则值类型</typeparam>
        /// <param name="ruleKey">规则键</param>
        /// <returns>规则值，如果不存在则返回默认值</returns>
        T? GetRule<T>(string ruleKey);

        /// <summary>
        /// 获取指定类型的规则值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">规则值类型</typeparam>
        /// <param name="ruleKey">规则键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>规则值或默认值</returns>
        T GetRule<T>(string ruleKey, T defaultValue);

        /// <summary>
        /// 获取指定的规则对象
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>规则对象，如果不存在则返回null</returns>
        TargetingRule? GetRuleObject(string ruleKey);

        /// <summary>
        /// 检查是否包含指定的规则
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>是否包含该规则</returns>
        bool HasRule(string ruleKey);

        /// <summary>
        /// 获取所有规则键
        /// </summary>
        /// <returns>规则键集合</returns>
        IReadOnlyCollection<string> GetRuleKeys();

        /// <summary>
        /// 根据分类获取规则
        /// </summary>
        /// <param name="category">规则分类</param>
        /// <returns>指定分类的规则集合</returns>
        IReadOnlyList<TargetingRule> GetRulesByCategory(string category);

        /// <summary>
        /// 获取必需规则
        /// </summary>
        /// <returns>必需规则集合</returns>
        IReadOnlyList<TargetingRule> GetRequiredRules();

        /// <summary>
        /// 验证条件配置的有效性
        /// </summary>
        /// <returns>验证结果</returns>
        bool IsValid();

        /// <summary>
        /// 获取条件的配置摘要信息
        /// 用于日志记录和调试
        /// </summary>
        /// <returns>配置摘要</returns>
        string GetConfigurationSummary();
    }
}