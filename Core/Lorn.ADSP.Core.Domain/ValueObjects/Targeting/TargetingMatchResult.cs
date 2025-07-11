namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ����ƥ����
    /// ��ʾ��������������ƥ����
    /// </summary>
    public class TargetingMatchResult
    {
        /// <summary>
        /// ƥ�������0.0 - 1.0��
        /// </summary>
        public decimal Score { get; }

        /// <summary>
        /// �Ƿ�ƥ��
        /// </summary>
        public bool IsMatch { get; }

        /// <summary>
        /// ƥ��������
        /// </summary>
        public string MatcherType { get; }

        /// <summary>
        /// ��������
        /// </summary>
        public string CriteriaType { get; }

        /// <summary>
        /// ƥ������
        /// </summary>
        public IReadOnlyDictionary<string, object> Details { get; }

        /// <summary>
        /// ƥ��ԭ�����
        /// </summary>
        public string ReasonCode { get; }

        /// <summary>
        /// ִ��ʱ��
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        /// <summary>
        /// ƥ�����Ŷ�
        /// </summary>
        public decimal Confidence { get; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="score">ƥ�����</param>
        /// <param name="isMatch">�Ƿ�ƥ��</param>
        /// <param name="matcherType">ƥ��������</param>
        /// <param name="criteriaType">��������</param>
        /// <param name="reasonCode">ԭ�����</param>
        /// <param name="executionTime">ִ��ʱ��</param>
        /// <param name="confidence">���Ŷ�</param>
        /// <param name="details">����</param>
        public TargetingMatchResult(
            decimal score,
            bool isMatch,
            string matcherType,
            string criteriaType,
            string reasonCode,
            TimeSpan executionTime,
            decimal confidence = 1.0m,
            IDictionary<string, object>? details = null)
        {
            if (score < 0 || score > 1)
                throw new ArgumentException("ƥ�����������0.0��1.0֮��", nameof(score));

            if (confidence < 0 || confidence > 1)
                throw new ArgumentException("���Ŷȱ�����0.0��1.0֮��", nameof(confidence));

            Score = score;
            IsMatch = isMatch;
            MatcherType = matcherType ?? throw new ArgumentNullException(nameof(matcherType));
            CriteriaType = criteriaType ?? throw new ArgumentNullException(nameof(criteriaType));
            ReasonCode = reasonCode ?? throw new ArgumentNullException(nameof(reasonCode));
            ExecutionTime = executionTime;
            Confidence = confidence;
            Details = details?.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly() ?? 
                     new Dictionary<string, object>().AsReadOnly();
        }

        /// <summary>
        /// �����ɹ�ƥ����
        /// </summary>
        /// <param name="score">ƥ�����</param>
        /// <param name="matcherType">ƥ��������</param>
        /// <param name="criteriaType">��������</param>
        /// <param name="executionTime">ִ��ʱ��</param>
        /// <param name="details">����</param>
        /// <returns>ƥ����</returns>
        public static TargetingMatchResult Success(
            decimal score,
            string matcherType,
            string criteriaType,
            TimeSpan executionTime,
            IDictionary<string, object>? details = null)
        {
            return new TargetingMatchResult(
                score,
                true,
                matcherType,
                criteriaType,
                "MATCH_SUCCESS",
                executionTime,
                1.0m,
                details);
        }

        /// <summary>
        /// ����ʧ��ƥ����
        /// </summary>
        /// <param name="matcherType">ƥ��������</param>
        /// <param name="criteriaType">��������</param>
        /// <param name="reasonCode">ʧ��ԭ��</param>
        /// <param name="executionTime">ִ��ʱ��</param>
        /// <param name="details">����</param>
        /// <returns>ƥ����</returns>
        public static TargetingMatchResult Failure(
            string matcherType,
            string criteriaType,
            string reasonCode,
            TimeSpan executionTime,
            IDictionary<string, object>? details = null)
        {
            return new TargetingMatchResult(
                0.0m,
                false,
                matcherType,
                criteriaType,
                reasonCode,
                executionTime,
                1.0m,
                details);
        }

        /// <summary>
        /// ��������ƥ����
        /// </summary>
        /// <param name="score">ƥ�����</param>
        /// <param name="matcherType">ƥ��������</param>
        /// <param name="criteriaType">��������</param>
        /// <param name="reasonCode">ԭ�����</param>
        /// <param name="executionTime">ִ��ʱ��</param>
        /// <param name="confidence">���Ŷ�</param>
        /// <param name="details">����</param>
        /// <returns>ƥ����</returns>
        public static TargetingMatchResult Partial(
            decimal score,
            string matcherType,
            string criteriaType,
            string reasonCode,
            TimeSpan executionTime,
            decimal confidence = 0.8m,
            IDictionary<string, object>? details = null)
        {
            return new TargetingMatchResult(
                score,
                score > 0.5m, // ��������0.5��Ϊ��ƥ��
                matcherType,
                criteriaType,
                reasonCode,
                executionTime,
                confidence,
                details);
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="key">�����</param>
        /// <returns>����ֵ</returns>
        public T? GetDetail<T>(string key)
        {
            if (Details.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return default;
        }

        /// <summary>
        /// ת��Ϊ�ַ�����ʾ
        /// </summary>
        /// <returns>�ַ�����ʾ</returns>
        public override string ToString()
        {
            var matchStatus = IsMatch ? "MATCH" : "NO_MATCH";
            return $"{MatcherType}[{CriteriaType}]: {matchStatus} Score:{Score:F3} Confidence:{Confidence:F3} " +
                   $"Reason:{ReasonCode} Time:{ExecutionTime.TotalMilliseconds:F1}ms";
        }

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <returns>������Ϣ</returns>
        public string GetDebugInfo()
        {
            var detailsInfo = Details.Any() ? 
                string.Join(", ", Details.Take(3).Select(kv => $"{kv.Key}:{kv.Value}")) : 
                "No Details";
            
            return $"{ToString()} Details:[{detailsInfo}]";
        }
    }
}