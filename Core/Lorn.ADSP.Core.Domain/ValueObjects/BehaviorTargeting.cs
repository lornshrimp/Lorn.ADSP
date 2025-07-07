using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 行为定向
    /// </summary>
    public class BehaviorTargeting : ValueObject
    {
        /// <summary>
        /// 目标兴趣标签
        /// </summary>
        public IReadOnlyList<string> InterestTags { get; private set; } = new List<string>();

        /// <summary>
        /// 目标行为类型
        /// </summary>
        public IReadOnlyList<string> BehaviorTypes { get; private set; } = new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorTargeting(
            IList<string>? interestTags = null,
            IList<string>? behaviorTypes = null)
        {
            InterestTags = interestTags?.ToList() ?? new List<string>();
            BehaviorTypes = behaviorTypes?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// 计算行为匹配度
        /// </summary>
        public decimal CalculateMatchScore(UserBehavior? userBehavior)
        {
            if (userBehavior == null)
                return 1.0m;

            decimal score = 1.0m;
            int criteriaCount = 0;

            // 兴趣标签匹配
            if (InterestTags.Any())
            {
                var interestMatch = InterestTags.Any(tag =>
                    userBehavior.InterestTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
                score *= interestMatch ? 1.0m : 0m;
                criteriaCount++;
            }

            // 行为类型匹配
            if (BehaviorTypes.Any())
            {
                var behaviorMatch = BehaviorTypes.Any(type =>
                    userBehavior.BehaviorHistory.Any(b =>
                        string.Equals(b.BehaviorType, type, StringComparison.OrdinalIgnoreCase)));
                score *= behaviorMatch ? 1.0m : 0m;
                criteriaCount++;
            }

            return criteriaCount == 0 ? 1.0m : score;
        }

        /// <summary>
        /// 获取相等性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var tag in InterestTags)
                yield return tag;
            foreach (var type in BehaviorTypes)
                yield return type;
        }
    }
}
