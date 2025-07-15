using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 健康状态报告
    /// </summary>
    public record HealthStatusReport
    {
        /// <summary>
        /// 整体健康状态
        /// </summary>
        public required HealthStatus OverallStatus { get; init; }

        /// <summary>
        /// 提供者健康状态列表
        /// </summary>
        public IReadOnlyList<ProviderHealthStatus> ProviderStatuses { get; init; } = Array.Empty<ProviderHealthStatus>();

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime CheckedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 总检查时间
        /// </summary>
        public TimeSpan TotalCheckTime { get; init; }

        /// <summary>
        /// 健康的提供者数量
        /// </summary>
        public int HealthyProviderCount => ProviderStatuses.Count(p => p.Status == HealthStatus.Healthy);

        /// <summary>
        /// 不健康的提供者数量
        /// </summary>
        public int UnhealthyProviderCount => ProviderStatuses.Count(p => p.Status != HealthStatus.Healthy);
    }
}
