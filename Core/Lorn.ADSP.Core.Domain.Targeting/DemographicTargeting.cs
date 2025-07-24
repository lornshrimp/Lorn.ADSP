using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 人口属性定向条件
    /// 实现基于人口统计学信息的定向规则配置
    /// </summary>
    public class DemographicTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "人口属性定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "Demographic";

        /// <summary>
        /// 目标性别
        /// </summary>
        public IReadOnlyList<Gender> TargetGenders => GetRule<List<Gender>>("TargetGenders") ?? new List<Gender>();

        /// <summary>
        /// 最小年龄
        /// </summary>
        public int? MinAge => GetRule<int?>("MinAge");

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int? MaxAge => GetRule<int?>("MaxAge");

        /// <summary>
        /// 目标关键词
        /// </summary>
        public IReadOnlyList<string> TargetKeywords => GetRule<List<string>>("TargetKeywords") ?? new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DemographicTargeting(
            IList<Gender>? targetGenders = null,
            int? minAge = null,
            int? maxAge = null,
            IList<string>? targetKeywords = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetGenders, minAge, maxAge, targetKeywords), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<Gender>? targetGenders,
            int? minAge,
            int? maxAge,
            IList<string>? targetKeywords)
        {
            var rules = new List<TargetingRule>();

            // 添加目标性别列表
            var targetGendersList = targetGenders?.ToList() ?? new List<Gender>();
            var gendersRule = new TargetingRule("TargetGenders", string.Empty, "Json").WithValue(targetGendersList);
            rules.Add(gendersRule);

            // 添加目标关键词列表
            var targetKeywordsList = targetKeywords?.ToList() ?? new List<string>();
            var keywordsRule = new TargetingRule("TargetKeywords", string.Empty, "Json").WithValue(targetKeywordsList);
            rules.Add(keywordsRule);

            // 添加最小年龄（如果存在）
            if (minAge.HasValue)
            {
                var minAgeRule = new TargetingRule("MinAge", string.Empty, "Int32").WithValue(minAge.Value);
                rules.Add(minAgeRule);
            }

            // 添加最大年龄（如果存在）
            if (maxAge.HasValue)
            {
                var maxAgeRule = new TargetingRule("MaxAge", string.Empty, "Int32").WithValue(maxAge.Value);
                rules.Add(maxAgeRule);
            }

            return rules;
        }

        /// <summary>
        /// 创建人口属性定向条件
        /// </summary>
        public static DemographicTargeting Create(
            IList<Gender>? targetGenders = null,
            int? minAge = null,
            int? maxAge = null,
            IList<string>? targetKeywords = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new DemographicTargeting(targetGenders, minAge, maxAge, targetKeywords, weight, isEnabled);
        }

        /// <summary>
        /// 设置年龄范围
        /// </summary>
        /// <param name="minAge">最小年龄</param>
        /// <param name="maxAge">最大年龄</param>
        public void SetAgeRange(int? minAge, int? maxAge)
        {
            if (minAge.HasValue && maxAge.HasValue && minAge.Value > maxAge.Value)
                throw new ArgumentException("最小年龄不能大于最大年龄");

            if (minAge.HasValue && minAge.Value < 0)
                throw new ArgumentException("最小年龄不能为负数", nameof(minAge));

            if (maxAge.HasValue && maxAge.Value < 0)
                throw new ArgumentException("最大年龄不能为负数", nameof(maxAge));

            if (minAge.HasValue)
                SetRule("MinAge", minAge.Value);
            else
                RemoveRule("MinAge");

            if (maxAge.HasValue)
                SetRule("MaxAge", maxAge.Value);
            else
                RemoveRule("MaxAge");
        }

        /// <summary>
        /// 添加目标性别
        /// </summary>
        /// <param name="gender">性别</param>
        public void AddTargetGender(Gender gender)
        {
            var currentList = TargetGenders.ToList();
            if (!currentList.Contains(gender))
            {
                currentList.Add(gender);
                SetRule("TargetGenders", currentList);
            }
        }

        /// <summary>
        /// 添加目标关键词
        /// </summary>
        /// <param name="keyword">关键词</param>
        public void AddTargetKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentException("关键词不能为空", nameof(keyword));

            var currentList = TargetKeywords.ToList();
            if (!currentList.Contains(keyword))
            {
                currentList.Add(keyword);
                SetRule("TargetKeywords", currentList);
            }
        }

        /// <summary>
        /// 验证人口属性定向特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // 验证年龄范围
            if (MinAge.HasValue && MaxAge.HasValue && MinAge.Value > MaxAge.Value)
                return false;

            if (MinAge.HasValue && MinAge.Value < 0)
                return false;

            if (MaxAge.HasValue && MaxAge.Value < 0)
                return false;

            // 至少需要配置一个定向条件
            if (!TargetGenders.Any() && !MinAge.HasValue && !MaxAge.HasValue && !TargetKeywords.Any())
                return false;

            // 验证关键词不能为空字符串
            if (TargetKeywords.Any(string.IsNullOrWhiteSpace))
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var ageRange = MinAge.HasValue || MaxAge.HasValue
                ? $"Age: {MinAge?.ToString() ?? "∞"}-{MaxAge?.ToString() ?? "∞"}"
                : "Age: All";
            var details = $"Genders: {TargetGenders.Count}, {ageRange}, Keywords: {TargetKeywords.Count}";
            return $"{summary} - {details}";
        }
    }
}
