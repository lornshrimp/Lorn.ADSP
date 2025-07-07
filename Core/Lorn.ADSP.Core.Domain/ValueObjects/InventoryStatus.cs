using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 库存状态值对象
/// </summary>
public class InventoryStatus : ValueObject
{
    /// <summary>
    /// 广告位ID
    /// </summary>
    public string PlacementId { get; private set; }

    /// <summary>
    /// 可用库存数量
    /// </summary>
    public int AvailableInventory { get; private set; }

    /// <summary>
    /// 已预订库存数量
    /// </summary>
    public int ReservedInventory { get; private set; }

    /// <summary>
    /// 总库存数量
    /// </summary>
    public int TotalInventory { get; private set; }

    /// <summary>
    /// 库存利用率
    /// </summary>
    public decimal UtilizationRate => TotalInventory > 0 ?
        (decimal)ReservedInventory / TotalInventory : 0m;

    /// <summary>
    /// 预期库存
    /// </summary>
    public int? ForecastedInventory { get; private set; }

    /// <summary>
    /// 库存更新时间
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// 库存状态
    /// </summary>
    public InventoryStatusType Status { get; private set; }

    /// <summary>
    /// 是否有库存可用
    /// </summary>
    public bool HasAvailableInventory => AvailableInventory > 0 && Status == InventoryStatusType.Available;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private InventoryStatus()
    {
        PlacementId = string.Empty;
        AvailableInventory = 0;
        ReservedInventory = 0;
        TotalInventory = 0;
        LastUpdated = DateTime.UtcNow;
        Status = InventoryStatusType.Available;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public InventoryStatus(
        string placementId,
        int availableInventory,
        int reservedInventory,
        int totalInventory,
        InventoryStatusType status = InventoryStatusType.Available,
        int? forecastedInventory = null,
        DateTime? lastUpdated = null)
    {
        ValidateInput(placementId, availableInventory, reservedInventory, totalInventory);

        PlacementId = placementId;
        AvailableInventory = availableInventory;
        ReservedInventory = reservedInventory;
        TotalInventory = totalInventory;
        Status = status;
        ForecastedInventory = forecastedInventory;
        LastUpdated = lastUpdated ?? DateTime.UtcNow;
    }

    /// <summary>
    /// 创建库存状态
    /// </summary>
    public static InventoryStatus Create(
        string placementId,
        int totalInventory,
        int reservedInventory = 0)
    {
        var availableInventory = totalInventory - reservedInventory;
        return new InventoryStatus(placementId, availableInventory, reservedInventory, totalInventory);
    }

    /// <summary>
    /// 预订库存
    /// </summary>
    public InventoryStatus ReserveInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("预订数量必须大于0", nameof(quantity));

        if (quantity > AvailableInventory)
            throw new InvalidOperationException("可用库存不足");

        return new InventoryStatus(
            PlacementId,
            AvailableInventory - quantity,
            ReservedInventory + quantity,
            TotalInventory,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 释放库存
    /// </summary>
    public InventoryStatus ReleaseInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("释放数量必须大于0", nameof(quantity));

        if (quantity > ReservedInventory)
            throw new InvalidOperationException("释放数量超过已预订库存");

        return new InventoryStatus(
            PlacementId,
            AvailableInventory + quantity,
            ReservedInventory - quantity,
            TotalInventory,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 增加库存
    /// </summary>
    public InventoryStatus AddInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("增加数量必须大于0", nameof(quantity));

        return new InventoryStatus(
            PlacementId,
            AvailableInventory + quantity,
            ReservedInventory,
            TotalInventory + quantity,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public InventoryStatus UpdateStatus(InventoryStatusType newStatus)
    {
        return new InventoryStatus(
            PlacementId,
            AvailableInventory,
            ReservedInventory,
            TotalInventory,
            newStatus,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// 设置预期库存
    /// </summary>
    public InventoryStatus WithForecast(int forecastedInventory)
    {
        return new InventoryStatus(
            PlacementId,
            AvailableInventory,
            ReservedInventory,
            TotalInventory,
            Status,
            forecastedInventory,
            LastUpdated);
    }

    /// <summary>
    /// 是否库存不足
    /// </summary>
    public bool IsLowInventory(decimal threshold = 0.1m)
    {
        return UtilizationRate > (1m - threshold);
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlacementId;
        yield return AvailableInventory;
        yield return ReservedInventory;
        yield return TotalInventory;
        yield return Status;
        yield return ForecastedInventory ?? 0;
        yield return LastUpdated;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        string placementId,
        int availableInventory,
        int reservedInventory,
        int totalInventory)
    {
        if (string.IsNullOrWhiteSpace(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (availableInventory < 0)
            throw new ArgumentException("可用库存数量不能为负数", nameof(availableInventory));

        if (reservedInventory < 0)
            throw new ArgumentException("已预订库存数量不能为负数", nameof(reservedInventory));

        if (totalInventory < 0)
            throw new ArgumentException("总库存数量不能为负数", nameof(totalInventory));

        if (availableInventory + reservedInventory != totalInventory)
            throw new ArgumentException("可用库存和已预订库存之和必须等于总库存");
    }
}