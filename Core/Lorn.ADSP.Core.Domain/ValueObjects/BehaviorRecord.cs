namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 行为记录
    /// </summary>
    public class BehaviorRecord
    {
        /// <summary>
        /// 行为类型
        /// </summary>
        public string BehaviorType { get; set; } = string.Empty;

        /// <summary>
        /// 行为时间
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
