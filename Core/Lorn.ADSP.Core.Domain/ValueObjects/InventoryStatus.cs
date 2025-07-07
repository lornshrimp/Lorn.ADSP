using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ���״ֵ̬����
/// </summary>
public record InventoryStatus
{
    /// <summary>
    /// ���λID
    /// </summary>
    public required string PlacementId { get; init; }

    /// <summary>
    /// ���ÿ������
    /// </summary>
    public int AvailableInventory { get; init; }

    /// <summary>
    /// ��Ԥ���������
    /// </summary>
    public int ReservedInventory { get; init; }

    /// <summary>
    /// �ܿ������
    /// </summary>
    public int TotalInventory { get; init; }

    /// <summary>
    /// ���������
    /// </summary>
    public decimal UtilizationRate => TotalInventory > 0 ? 
        (decimal)ReservedInventory / TotalInventory : 0m;

    /// <summary>
    /// Ԥ�ڿ��
    /// </summary>
    public int? ForecastedInventory { get; init; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ���״̬
    /// </summary>
    public InventoryStatusType Status { get; init; } = InventoryStatusType.Available;
}