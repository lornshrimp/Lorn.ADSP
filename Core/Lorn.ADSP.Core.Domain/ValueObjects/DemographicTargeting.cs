using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 人口属性定向
    /// </summary>
    public class DemographicTargeting : ValueObject
    {
        /// <summary>
        /// 目标性别
        /// </summary>
        public IReadOnlyList<Gender> TargetGenders { get; private set; } = new List<Gender>();

        /// <summary>
        /// 最小年龄
        /// </summary>
        public int? MinAge { get; private set; }

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int? MaxAge { get; private set; }

        /// <summary>
        /// 目标关键词
        /// </summary>
        public IReadOnlyList<string> TargetKeywords { get; private set; } = new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DemographicTargeting(
            IList<Gender>? targetGenders = null,
            int? minAge = null,
            int? maxAge = null,
            IList<string>? targetKeywords = null)
        {
            TargetGenders = targetGenders?.ToList() ?? new List<Gender>();
            MinAge = minAge;
            MaxAge = maxAge;
            TargetKeywords = targetKeywords?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// 计算人口属性匹配度
        /// </summary>
        public decimal CalculateMatchScore(UserProfile? userProfile)
        {
            if (userProfile == null)
                return 1.0m;

            decimal score = 1.0m;
            int criteriaCount = 0;

            // 性别匹配
            if (TargetGenders.Any())
            {
                score *= TargetGenders.Contains(userProfile.BasicInfo.Gender) ? 1.0m : 0m;
                criteriaCount++;
            }

            // 年龄匹配
            if (MinAge.HasValue || MaxAge.HasValue)
            {
                var ageMatch = true;
                if (MinAge.HasValue && userProfile.GetAge() < MinAge.Value)
                    ageMatch = false;
                if (MaxAge.HasValue && userProfile.GetAge() > MaxAge.Value)
                    ageMatch = false;

                score *= ageMatch ? 1.0m : 0m;
                criteriaCount++;
            }

            // 关键词匹配
            if (TargetKeywords.Any())
            {
                var keywordMatch = TargetKeywords.Any(keyword =>
                    userProfile.Keywords.Any(userKeyword =>
                        userKeyword.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
                score *= keywordMatch ? 1.0m : 0m;
                criteriaCount++;
            }

            return criteriaCount == 0 ? 1.0m : score;
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var gender in TargetGenders)
                yield return gender;
            yield return MinAge ?? 0;
            yield return MaxAge ?? 0;
            foreach (var keyword in TargetKeywords)
                yield return keyword;
        }
    }
}
