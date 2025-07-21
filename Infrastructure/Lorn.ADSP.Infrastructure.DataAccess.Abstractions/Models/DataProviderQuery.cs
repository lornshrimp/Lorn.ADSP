using Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Models;

/// <summary>
/// 数据提供者查询条件
/// 用于从注册表中查找符合条件的数据提供者
/// </summary>
public sealed class DataProviderQuery
{
    /// <summary>
    /// 提供者类型过滤
    /// </summary>
    public DataProviderType? ProviderType { get; init; }

    /// <summary>
    /// 业务实体名称过滤
    /// </summary>
    public string? BusinessEntity { get; init; }

    /// <summary>
    /// 技术类型过滤
    /// </summary>
    public string? TechnologyType { get; init; }

    /// <summary>
    /// 平台类型过滤
    /// </summary>
    public string? PlatformType { get; init; }

    /// <summary>
    /// 标签过滤（包含任一标签即匹配）
    /// </summary>
    public string[] Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 支持的操作过滤
    /// </summary>
    public string[] SupportedOperations { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 是否只返回已启用的提供者
    /// </summary>
    public bool OnlyEnabled { get; init; } = true;

    /// <summary>
    /// 是否只返回健康的提供者
    /// </summary>
    public bool OnlyHealthy { get; init; } = true;

    /// <summary>
    /// 最小优先级过滤
    /// </summary>
    public int? MinPriority { get; init; }

    /// <summary>
    /// 最大优先级过滤
    /// </summary>
    public int? MaxPriority { get; init; }

    /// <summary>
    /// 版本过滤
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// 提供者ID列表过滤（包含任一ID即匹配）
    /// </summary>
    public string[] ProviderIds { get; init; } = Array.Empty<string>();

    /// <summary>
    /// 扩展元数据过滤
    /// </summary>
    public Dictionary<string, object> ExtendedMetadataFilter { get; init; } = new();

    /// <summary>
    /// 结果数量限制
    /// </summary>
    public int? Limit { get; init; }

    /// <summary>
    /// 是否按优先级排序
    /// </summary>
    public bool OrderByPriority { get; init; } = true;

    /// <summary>
    /// 创建一个查询所有提供者的实例
    /// </summary>
    public static DataProviderQuery All => new();

    /// <summary>
    /// 创建一个查询指定业务实体提供者的实例
    /// </summary>
    /// <param name="businessEntity">业务实体名称</param>
    /// <returns>查询实例</returns>
    public static DataProviderQuery ForBusinessEntity(string businessEntity) =>
        new() { BusinessEntity = businessEntity, ProviderType = DataProviderType.BusinessLogic };

    /// <summary>
    /// 创建一个查询指定技术类型提供者的实例
    /// </summary>
    /// <param name="technologyType">技术类型</param>
    /// <returns>查询实例</returns>
    public static DataProviderQuery ForTechnology(string technologyType) =>
        new() { TechnologyType = technologyType };

    /// <summary>
    /// 创建一个查询指定平台类型提供者的实例
    /// </summary>
    /// <param name="platformType">平台类型</param>
    /// <returns>查询实例</returns>
    public static DataProviderQuery ForPlatform(string platformType) =>
        new() { PlatformType = platformType, ProviderType = DataProviderType.Cloud };
}
