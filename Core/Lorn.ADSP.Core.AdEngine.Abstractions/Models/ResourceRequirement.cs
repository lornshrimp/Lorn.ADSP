namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models
{
    /// <summary>
    /// 资源需求
    /// </summary>
    public record ResourceRequirement
    {
        /// <summary>
        /// CPU需求等级（1-10）
        /// </summary>
        public int CpuRequirement { get; init; } = 5;

        /// <summary>
        /// 内存需求（MB）
        /// </summary>
        public int MemoryRequirementMB { get; init; } = 100;

        /// <summary>
        /// 网络需求等级（1-10）
        /// </summary>
        public int NetworkRequirement { get; init; } = 5;

        /// <summary>
        /// 磁盘IO需求等级（1-10）
        /// </summary>
        public int DiskIORequirement { get; init; } = 3;

        /// <summary>
        /// 是否需要GPU
        /// </summary>
        public bool RequiresGpu { get; init; } = false;

        /// <summary>
        /// 最大并发数
        /// </summary>
        public int MaxConcurrency { get; init; } = 10;

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 重试次数
        /// </summary>
        public int MaxRetries { get; init; } = 3;

        /// <summary>
        /// 是否为轻量级策略
        /// </summary>
        public bool IsLightweight => CpuRequirement <= 3 && MemoryRequirementMB <= 50 && !RequiresGpu;

        /// <summary>
        /// 是否为重量级策略
        /// </summary>
        public bool IsHeavyweight => CpuRequirement >= 8 || MemoryRequirementMB >= 500 || RequiresGpu;
    }
}
