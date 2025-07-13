namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models
{
    /// <summary>
    /// 实验组
    /// </summary>
    public record ExperimentGroup
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public required string GroupId { get; init; }

        /// <summary>
        /// 组名称
        /// </summary>
        public required string GroupName { get; init; }

        /// <summary>
        /// 权重（百分比）
        /// </summary>
        public decimal Weight { get; init; } = 50.0m;

        /// <summary>
        /// 组配置参数
        /// </summary>
        public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// 是否为对照组
        /// </summary>
        public bool IsControl { get; init; } = false;
    }
}
