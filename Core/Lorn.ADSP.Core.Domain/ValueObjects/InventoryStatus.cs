using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 库存状态值对象
/// </summary>
public record InventoryStatus
{
    /// <summary>
    /// 广告位ID
    /// </summary>
    public required string PlacementId { get; init; }

    /// <summary>
    /// 可用库存数量
    /// </summary>
    public int AvailableInventory { get; init; }

    /// <summary>
    /// 已预订库存数量
    /// </summary>
    public int ReservedInventory { get; init; }

    /// <summary>
    /// 总库存数量
    /// </summary>
    public int TotalInventory { get; init; }

    /// <summary>
    /// 库存利用率
    /// </summary>
    public decimal UtilizationRate => TotalInventory > 0 ? 
        (decimal)ReservedInventory / TotalInventory : 0m;

    /// <summary>
    /// 预期库存
    /// </summary>
    public int? ForecastedInventory { get; init; }

    /// <summary>
    /// 库存更新时间
    /// </summary>
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 库存状态
    /// </summary>
    public InventoryStatusType Status { get; init; } = InventoryStatusType.Available;
}