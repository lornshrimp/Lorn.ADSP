using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 定向条件基础实现类
    /// 提供ITargetingCriteria接口的标准实现
    /// </summary>
    public abstract class TargetingCriteriaBase : ValueObject, ITargetingCriteria
    {
        private readonly List<TargetingRule> _rules;

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public abstract string CriteriaType { get; }

        /// <summary>
        /// 定向规则集合（只读）
        /// </summary>
        public IReadOnlyList<TargetingRule> Rules => _rules.AsReadOnly();

        /// <summary>
        /// 条件权重
        /// </summary>
        public decimal Weight { get; protected set; } = 1.0m;

        /// <summary>
        /// 是否启用该条件
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// 条件创建时间
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// 条件最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rules">规则集合</param>
        /// <param name="weight">权重</param>
        /// <param name="isEnabled">是否启用</param>
        protected TargetingCriteriaBase(
            IEnumerable<TargetingRule>? rules = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            _rules = rules?.ToList() ?? new List<TargetingRule>();
            Weight = weight;
            IsEnabled = isEnabled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            ValidateWeight(weight);
        }

        /// <summary>
        /// 获取指定类型的规则值
        /// </summary>
        /// <typeparam name="T">规则值类型</typeparam>
        /// <param name="ruleKey">规则键</param>
        /// <returns>规则值，如果不存在则返回默认值</returns>
        public virtual T? GetRule<T>(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return default;

            var rule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (rule == null)
                return default;

            return rule.GetValue<T>();
        }

        /// <summary>
        /// 获取指定类型的规则值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">规则值类型</typeparam>
        /// <param name="ruleKey">规则键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>规则值或默认值</returns>
        public virtual T GetRule<T>(string ruleKey, T defaultValue)
        {
            var result = GetRule<T>(ruleKey);
            return result ?? defaultValue;
        }

        /// <summary>
        /// 获取指定的规则对象
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>规则对象，如果不存在则返回null</returns>
        public virtual TargetingRule? GetRuleObject(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return null;

            return _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
        }

        /// <summary>
        /// 检查是否包含指定的规则
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>是否包含该规则</returns>
        public virtual bool HasRule(string ruleKey)
        {
            return !string.IsNullOrEmpty(ruleKey) && _rules.Any(r => r.RuleKey == ruleKey);
        }

        /// <summary>
        /// 获取所有规则键
        /// </summary>
        /// <returns>规则键集合</returns>
        public virtual IReadOnlyCollection<string> GetRuleKeys()
        {
            return _rules.Select(r => r.RuleKey).ToList().AsReadOnly();
        }

        /// <summary>
        /// 根据分类获取规则
        /// </summary>
        /// <param name="category">规则分类</param>
        /// <returns>指定分类的规则集合</returns>
        public virtual IReadOnlyList<TargetingRule> GetRulesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new List<TargetingRule>().AsReadOnly();

            return _rules.Where(r => r.Category == category).ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取必需规则
        /// </summary>
        /// <returns>必需规则集合</returns>
        public virtual IReadOnlyList<TargetingRule> GetRequiredRules()
        {
            return _rules.Where(r => r.IsRequired).ToList().AsReadOnly();
        }

        /// <summary>
        /// 验证条件配置的有效性
        /// </summary>
        /// <returns>验证结果</returns>
        public virtual bool IsValid()
        {
            // 基础验证：权重必须为非负数
            if (Weight < 0)
                return false;

            // 子类可以重写此方法添加特定的验证逻辑
            return ValidateSpecificRules();
        }

        /// <summary>
        /// 获取条件的配置摘要信息
        /// </summary>
        /// <returns>配置摘要</returns>
        public virtual string GetConfigurationSummary()
        {
            var ruleCount = _rules.Count;
            var enabledStatus = IsEnabled ? "Enabled" : "Disabled";
            return $"{CriteriaType} - Rules: {ruleCount}, Weight: {Weight:F2}, Status: {enabledStatus}";
        }

        /// <summary>
        /// 添加或更新规则
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <param name="ruleValue">规则值</param>
        protected virtual void SetRule(string ruleKey, object ruleValue)
        {
            if (string.IsNullOrEmpty(ruleKey))
                throw new ArgumentException("规则键不能为空", nameof(ruleKey));

            var existingRule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (existingRule != null)
            {
                _rules.Remove(existingRule);
            }

            var newRule = new TargetingRule(ruleKey, ruleValue?.ToString() ?? string.Empty);
            _rules.Add(newRule);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 移除规则
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>是否成功移除</returns>
        protected virtual bool RemoveRule(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return false;

            var existingRule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (existingRule != null)
            {
                _rules.Remove(existingRule);
                UpdatedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新权重
        /// </summary>
        /// <param name="newWeight">新权重</param>
        protected virtual void UpdateWeight(decimal newWeight)
        {
            ValidateWeight(newWeight);
            Weight = newWeight;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新启用状态
        /// </summary>
        /// <param name="enabled">是否启用</param>
        protected virtual void UpdateEnabled(bool enabled)
        {
            IsEnabled = enabled;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 验证特定规则的有效性
        /// 子类可以重写此方法实现特定的验证逻辑
        /// </summary>
        /// <returns>验证结果</returns>
        protected abstract bool ValidateSpecificRules();

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CriteriaType;
            yield return Weight;
            yield return IsEnabled;

            // 按键排序的规则集合
            foreach (var rule in _rules.OrderBy(r => r.RuleKey))
            {
                yield return rule.RuleKey;
                yield return rule.RuleValue ?? string.Empty;
            }
        }

        /// <summary>
        /// 验证权重值
        /// </summary>
        /// <param name="weight">权重值</param>
        private static void ValidateWeight(decimal weight)
        {
            if (weight < 0)
                throw new ArgumentException("权重不能为负数", nameof(weight));
        }
    }
}