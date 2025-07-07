namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 用户行为
    /// </summary>
    public class UserBehavior
    {
        /// <summary>
        /// 兴趣标签
        /// </summary>
        public IList<string> InterestTags { get; set; } = new List<string>();

        /// <summary>
        /// 行为历史
        /// </summary>
        public IList<BehaviorRecord> BehaviorHistory { get; set; } = new List<BehaviorRecord>();
    }
}
