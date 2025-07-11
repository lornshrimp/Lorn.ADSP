using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ������������ʵ����
    /// �ṩITargetingCriteria�ӿڵı�׼ʵ��
    /// </summary>
    public abstract class TargetingCriteriaBase : ValueObject, ITargetingCriteria
    {
        private readonly Dictionary<string, object> _rules;

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public abstract string CriteriaType { get; }

        /// <summary>
        /// ������򼯺ϣ�ֻ����
        /// </summary>
        public IReadOnlyDictionary<string, object> Rules => _rules.AsReadOnly();

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public decimal Weight { get; protected set; } = 1.0m;

        /// <summary>
        /// �Ƿ����ø�����
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// ����������ʱ��
        /// </summary>
        public DateTime UpdatedAt { get; protected set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="rules">���򼯺�</param>
        /// <param name="weight">Ȩ��</param>
        /// <param name="isEnabled">�Ƿ�����</param>
        protected TargetingCriteriaBase(
            IDictionary<string, object>? rules = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            _rules = rules?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, object>();
            Weight = weight;
            IsEnabled = isEnabled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            ValidateWeight(weight);
        }

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        public virtual T? GetRule<T>(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return default;

            if (_rules.TryGetValue(ruleKey, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                // ��������ת��
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default;
                }
            }

            return default;
        }

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        public virtual T GetRule<T>(string ruleKey, T defaultValue)
        {
            var result = GetRule<T>(ruleKey);
            return result != null ? result : defaultValue;
        }

        /// <summary>
        /// ����Ƿ����ָ���Ĺ���
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�Ƿ�����ù���</returns>
        public virtual bool HasRule(string ruleKey)
        {
            return !string.IsNullOrEmpty(ruleKey) && _rules.ContainsKey(ruleKey);
        }

        /// <summary>
        /// ��ȡ���й����
        /// </summary>
        /// <returns>���������</returns>
        public virtual IReadOnlyCollection<string> GetRuleKeys()
        {
            return _rules.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// ��֤�������õ���Ч��
        /// </summary>
        /// <returns>��֤���</returns>
        public virtual bool IsValid()
        {
            // ������֤��Ȩ�ر���Ϊ�Ǹ���
            if (Weight < 0)
                return false;

            // ���������д�˷�������ض�����֤�߼�
            return ValidateSpecificRules();
        }

        /// <summary>
        /// ��ȡ����������ժҪ��Ϣ
        /// </summary>
        /// <returns>����ժҪ</returns>
        public virtual string GetConfigurationSummary()
        {
            var ruleCount = _rules.Count;
            var enabledStatus = IsEnabled ? "Enabled" : "Disabled";
            return $"{CriteriaType} - Rules: {ruleCount}, Weight: {Weight:F2}, Status: {enabledStatus}";
        }

        /// <summary>
        /// ��ӻ���¹���
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <param name="ruleValue">����ֵ</param>
        protected virtual void SetRule(string ruleKey, object ruleValue)
        {
            if (string.IsNullOrEmpty(ruleKey))
                throw new ArgumentException("���������Ϊ��", nameof(ruleKey));

            _rules[ruleKey] = ruleValue;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�Ƿ�ɹ��Ƴ�</returns>
        protected virtual bool RemoveRule(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return false;

            var result = _rules.Remove(ruleKey);
            if (result)
            {
                UpdatedAt = DateTime.UtcNow;
            }
            return result;
        }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="newWeight">��Ȩ��</param>
        protected virtual void UpdateWeight(decimal newWeight)
        {
            ValidateWeight(newWeight);
            Weight = newWeight;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="enabled">�Ƿ�����</param>
        protected virtual void UpdateEnabled(bool enabled)
        {
            IsEnabled = enabled;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// ��֤�ض��������Ч��
        /// ���������д�˷���ʵ���ض�����֤�߼�
        /// </summary>
        /// <returns>��֤���</returns>
        protected abstract bool ValidateSpecificRules();

        /// <summary>
        /// ��ȡ����ԱȽϵ����
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CriteriaType;
            yield return Weight;
            yield return IsEnabled;

            // ��������Ĺ��򼯺�
            foreach (var rule in _rules.OrderBy(kv => kv.Key))
            {
                yield return rule.Key;
                yield return rule.Value ?? string.Empty;
            }
        }

        /// <summary>
        /// ��֤Ȩ��ֵ
        /// </summary>
        /// <param name="weight">Ȩ��ֵ</param>
        private static void ValidateWeight(decimal weight)
        {
            if (weight < 0)
                throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
        }
    }
}