namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 定向匹配结果
    /// 表示单个定向条件的匹配结果
    /// </summary>
    public class TargetingMatchResult
    {
        /// <summary>
        /// 匹配分数（0.0 - 1.0）
        /// </summary>
        public decimal Score { get; }

        /// <summary>
        /// 是否匹配
        /// </summary>
        public bool IsMatch { get; }

        /// <summary>
        /// 匹配器类型
        /// </summary>
        public string MatcherType { get; }

        /// <summary>
        /// 条件类型
        /// </summary>
        public string CriteriaType { get; }

        /// <summary>
        /// 匹配详情
        /// </summary>
        public IReadOnlyDictionary<string, object> Details { get; }

        /// <summary>
        /// 匹配原因代码
        /// </summary>
        public string ReasonCode { get; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        /// <summary>
        /// 匹配置信度
        /// </summary>
        public decimal Confidence { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="score">匹配分数</param>
        /// <param name="isMatch">是否匹配</param>
        /// <param name="matcherType">匹配器类型</param>
        /// <param name="criteriaType">条件类型</param>
        /// <param name="reasonCode">原因代码</param>
        /// <param name="executionTime">执行时间</param>
        /// <param name="confidence">置信度</param>
        /// <param name="details">详情</param>
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
                throw new ArgumentException("匹配分数必须在0.0到1.0之间", nameof(score));

            if (confidence < 0 || confidence > 1)
                throw new ArgumentException("置信度必须在0.0到1.0之间", nameof(confidence));

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
        /// 创建成功匹配结果
        /// </summary>
        /// <param name="score">匹配分数</param>
        /// <param name="matcherType">匹配器类型</param>
        /// <param name="criteriaType">条件类型</param>
        /// <param name="executionTime">执行时间</param>
        /// <param name="details">详情</param>
        /// <returns>匹配结果</returns>
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
        /// 创建失败匹配结果
        /// </summary>
        /// <param name="matcherType">匹配器类型</param>
        /// <param name="criteriaType">条件类型</param>
        /// <param name="reasonCode">失败原因</param>
        /// <param name="executionTime">执行时间</param>
        /// <param name="details">详情</param>
        /// <returns>匹配结果</returns>
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
        /// 创建部分匹配结果
        /// </summary>
        /// <param name="score">匹配分数</param>
        /// <param name="matcherType">匹配器类型</param>
        /// <param name="criteriaType">条件类型</param>
        /// <param name="reasonCode">原因代码</param>
        /// <param name="executionTime">执行时间</param>
        /// <param name="confidence">置信度</param>
        /// <param name="details">详情</param>
        /// <returns>匹配结果</returns>
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
                score > 0.5m, // 分数大于0.5认为是匹配
                matcherType,
                criteriaType,
                reasonCode,
                executionTime,
                confidence,
                details);
        }

        /// <summary>
        /// 获取详情信息
        /// </summary>
        /// <param name="key">详情键</param>
        /// <returns>详情值</returns>
        public T? GetDetail<T>(string key)
        {
            if (Details.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return default;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            var matchStatus = IsMatch ? "MATCH" : "NO_MATCH";
            return $"{MatcherType}[{CriteriaType}]: {matchStatus} Score:{Score:F3} Confidence:{Confidence:F3} " +
                   $"Reason:{ReasonCode} Time:{ExecutionTime.TotalMilliseconds:F1}ms";
        }

        /// <summary>
        /// 获取调试信息
        /// </summary>
        /// <returns>调试信息</returns>
        public string GetDebugInfo()
        {
            var detailsInfo = Details.Any() ? 
                string.Join(", ", Details.Take(3).Select(kv => $"{kv.Key}:{kv.Value}")) : 
                "No Details";
            
            return $"{ToString()} Details:[{detailsInfo}]";
        }
    }
}