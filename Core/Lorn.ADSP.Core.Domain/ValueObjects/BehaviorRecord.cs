using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 行为记录值对象
    /// 表示单个用户行为记录，用于存储在UserBehavior上下文中
    /// </summary>
    public class BehaviorRecord : ValueObject
    {
        /// <summary>
        /// 行为类型（如：Click, View, Purchase等）
        /// </summary>
        public string BehaviorType { get; private set; }

        /// <summary>
        /// 行为值或描述
        /// </summary>
        public string BehaviorValue { get; private set; }

        /// <summary>
        /// 行为发生时间戳
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// 行为频次
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        /// 行为权重
        /// </summary>
        public decimal Weight { get; private set; }

        /// <summary>
        /// 行为上下文信息（JSON格式）
        /// </summary>
        public string? Context { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private BehaviorRecord()
        {
            BehaviorType = string.Empty;
            BehaviorValue = string.Empty;
            Timestamp = DateTime.UtcNow;
            Frequency = 1;
            Weight = 1.0m;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorRecord(
            string behaviorType,
            string behaviorValue,
            DateTime? timestamp = null,
            int frequency = 1,
            decimal weight = 1.0m,
            string? context = null)
        {
            ValidateInput(behaviorType, behaviorValue, frequency, weight);

            BehaviorType = behaviorType;
            BehaviorValue = behaviorValue;
            Timestamp = timestamp ?? DateTime.UtcNow;
            Frequency = frequency;
            Weight = weight;
            Context = context;
        }

        /// <summary>
        /// 创建行为记录
        /// </summary>
        public static BehaviorRecord Create(
            string behaviorType,
            string behaviorValue,
            DateTime? timestamp = null,
            int frequency = 1,
            decimal weight = 1.0m,
            string? context = null)
        {
            return new BehaviorRecord(behaviorType, behaviorValue, timestamp, frequency, weight, context);
        }

        /// <summary>
        /// 创建点击行为记录
        /// </summary>
        public static BehaviorRecord CreateClick(
            string target,
            DateTime? timestamp = null,
            string? context = null)
        {
            return new BehaviorRecord("Click", target, timestamp, 1, 1.0m, context);
        }

        /// <summary>
        /// 创建浏览行为记录
        /// </summary>
        public static BehaviorRecord CreateView(
            string content,
            DateTime? timestamp = null,
            decimal duration = 0.0m,
            string? context = null)
        {
            return new BehaviorRecord("View", content, timestamp, 1, Math.Max(1.0m, duration), context);
        }

        /// <summary>
        /// 创建购买行为记录
        /// </summary>
        public static BehaviorRecord CreatePurchase(
            string product,
            DateTime? timestamp = null,
            decimal amount = 0.0m,
            string? context = null)
        {
            return new BehaviorRecord("Purchase", product, timestamp, 1, Math.Max(1.0m, amount), context);
        }

        /// <summary>
        /// 创建搜索行为记录
        /// </summary>
        public static BehaviorRecord CreateSearch(
            string query,
            DateTime? timestamp = null,
            string? context = null)
        {
            return new BehaviorRecord("Search", query, timestamp, 1, 1.0m, context);
        }

        /// <summary>
        /// 是否为指定类型的行为
        /// </summary>
        public bool IsOfType(string behaviorType)
        {
            return string.Equals(BehaviorType, behaviorType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取行为的年龄（距离现在的时间）
        /// </summary>
        public TimeSpan GetAge()
        {
            return DateTime.UtcNow - Timestamp;
        }

        /// <summary>
        /// 是否为最近的行为（指定时间段内）
        /// </summary>
        public bool IsRecent(TimeSpan timeSpan)
        {
            return GetAge() <= timeSpan;
        }

        /// <summary>
        /// 获取加权分数（频次 * 权重）
        /// </summary>
        public decimal GetWeightedScore()
        {
            return Frequency * Weight;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(string behaviorType, string behaviorValue, int frequency, decimal weight)
        {
            if (string.IsNullOrWhiteSpace(behaviorType))
                throw new ArgumentException("行为类型不能为空", nameof(behaviorType));

            if (string.IsNullOrWhiteSpace(behaviorValue))
                throw new ArgumentException("行为值不能为空", nameof(behaviorValue));

            if (frequency < 1)
                throw new ArgumentException("频次必须大于0", nameof(frequency));

            if (weight < 0)
                throw new ArgumentException("权重不能为负数", nameof(weight));
        }

        /// <summary>
        /// 获取相等性比较组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BehaviorType;
            yield return BehaviorValue;
            yield return Timestamp;
            yield return Frequency;
            yield return Weight;
            yield return Context ?? string.Empty;
        }

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            var contextInfo = !string.IsNullOrEmpty(Context) ? $" ({Context})" : "";
            return $"{BehaviorType}: {BehaviorValue} x{Frequency} @{Timestamp:yyyy-MM-dd HH:mm:ss}{contextInfo}";
        }
    }
}