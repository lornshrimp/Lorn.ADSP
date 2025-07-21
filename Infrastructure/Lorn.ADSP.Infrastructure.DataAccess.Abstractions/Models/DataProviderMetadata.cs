using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 数据提供者元数据
/// 包含数据提供者的完整描述信息，用于智能路由和管理
/// </summary>
public sealed class DataProviderMetadata
{
    /// <summary>
    /// 提供者唯一标识符
    /// </summary>
    public required string ProviderId { get; init; }

    /// <summary>
    /// 提供者显示名称
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// 提供者类型
    /// </summary>
    public required DataProviderType ProviderType { get; init; }

    /// <summary>
    /// 业务实体名称（业务逻辑提供者使用）
    /// </summary>
    public string? BusinessEntity { get; init; }

    /// <summary>
    /// 技术类型（技术提供者使用，如Redis、SqlServer等）
    /// </summary>
    public string? TechnologyType { get; init; }

    /// <summary>
    /// 平台类型（云平台提供者使用，如AlibabaCloud、Azure等）
    /// </summary>
    public string? PlatformType { get; init; }

    /// <summary>
    /// 优先级（数值越大优先级越高）
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// 扩展元数据
    /// </summary>
    public Dictionary<string, object> ExtendedMetadata { get; init; } = new();

    /// <summary>
    /// 支持的操作列表
    /// </summary>
    public string[] SupportedOperations { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 提供者版本
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 健康状态
    /// </summary>
    public HealthStatus HealthStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 提供者描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 标签集合，用于分类和过滤
    /// </summary>
    public string[] Tags { get; init; } = Array.Empty<string>();
}
