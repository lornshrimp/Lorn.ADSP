using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 提供者健康状态
    /// </summary>
    public record ProviderHealthStatus
    {
        /// <summary>
        /// 提供者标识
        /// </summary>
        public required string ProviderId { get; init; }

        /// <summary>
        /// 提供者名称
        /// </summary>
        public required string ProviderName { get; init; }

        /// <summary>
        /// 健康状态
        /// </summary>
        public required HealthStatus Status { get; init; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public TimeSpan ResponseTime { get; init; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// 最后检查时间
        /// </summary>
        public DateTime LastCheckTime { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 扩展信息
        /// </summary>
        public IReadOnlyDictionary<string, object> ExtendedInfo { get; init; } = new Dictionary<string, object>();
    }
}
