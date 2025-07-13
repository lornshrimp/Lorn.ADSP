using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.Enums;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Aggregates;

/// <summary>
/// 媒体资源实体（聚合根）
/// </summary>
public class MediaResource : AggregateRoot
{
    /// <summary>
    /// 媒体资源名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体类型
    /// </summary>
    public MediaType Type { get; private set; }

    /// <summary>
    /// 媒体URL
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体状态
    /// </summary>
    public MediaStatus Status { get; private set; }

    /// <summary>
    /// 发布商ID
    /// </summary>
    public string PublisherId { get; private set; } = string.Empty;

    /// <summary>
    /// 广告位配置
    /// </summary>
    public AdSlotConfiguration SlotConfig { get; private set; } = null!;

    /// <summary>
    /// 流量概况
    /// </summary>
    public TrafficProfile TrafficProfile { get; private set; } = null!;

    /// <summary>
    /// 广告位集合
    /// </summary>
    private readonly List<AdPlacement> _placements = new();
    public IReadOnlyList<AdPlacement> Placements => _placements.AsReadOnly();

    /// <summary>
    /// 投放记录集合
    /// </summary>
    private readonly List<DeliveryRecord> _deliveryRecords = new();
    public IReadOnlyList<DeliveryRecord> DeliveryRecords => _deliveryRecords.AsReadOnly();

    /// <summary>
    /// 私有构造函数，用于ORM
    /// </summary>
    private MediaResource() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public MediaResource(
        string name,
        MediaType type,
        string url,
        string publisherId,
        AdSlotConfiguration slotConfig,
        TrafficProfile trafficProfile)
    {
        ValidateInputs(name, url, publisherId);

        Name = name;
        Type = type;
        Url = url;
        PublisherId = publisherId;
        SlotConfig = slotConfig;
        TrafficProfile = trafficProfile;
        Status = MediaStatus.Pending;

        // 触发媒体资源创建事件
        AddDomainEvent(new MediaResourceCreatedEvent(Id, name, type, publisherId));
    }

    /// <summary>
    /// 配置广告位
    /// </summary>
    public void Configure(AdSlotConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        SlotConfig = config;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceConfiguredEvent(Id));
    }

    /// <summary>
    /// 更新流量概况
    /// </summary>
    public void UpdateTrafficProfile(TrafficProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        TrafficProfile = profile;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceTrafficUpdatedEvent(Id));
    }

    /// <summary>
    /// 启用媒体资源
    /// </summary>
    public void Enable()
    {
        if (Status == MediaStatus.Active)
            throw new InvalidOperationException("媒体资源已经处于启用状态");

        Status = MediaStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceEnabledEvent(Id));
    }

    /// <summary>
    /// 禁用媒体资源
    /// </summary>
    public void Disable()
    {
        if (Status == MediaStatus.Disabled)
            throw new InvalidOperationException("媒体资源已经处于禁用状态");

        Status = MediaStatus.Disabled;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceDisabledEvent(Id));
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public void Approve()
    {
        if (Status != MediaStatus.Pending)
            throw new InvalidOperationException("只有待审核状态的媒体资源可以审核通过");

        Status = MediaStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceApprovedEvent(Id));
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("拒绝必须提供原因", nameof(reason));

        if (Status != MediaStatus.Pending)
            throw new InvalidOperationException("只有待审核状态的媒体资源可以拒绝");

        Status = MediaStatus.Rejected;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceRejectedEvent(Id, reason));
    }

    /// <summary>
    /// 添加广告位
    /// </summary>
    public void AddPlacement(AdPlacement placement)
    {
        ArgumentNullException.ThrowIfNull(placement);

        if (placement.MediaResourceId != Id)
            throw new ArgumentException("广告位所属媒体资源ID不匹配");

        _placements.Add(placement);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourcePlacementAddedEvent(Id, placement.Id));
    }

    /// <summary>
    /// 移除广告位
    /// </summary>
    public void RemovePlacement(string placementId)
    {
        if (string.IsNullOrWhiteSpace(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        var placement = _placements.FirstOrDefault(p => p.Id == placementId);
        if (placement == null)
            throw new ArgumentException("未找到指定的广告位", nameof(placementId));

        _placements.Remove(placement);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourcePlacementRemovedEvent(Id, placementId));
    }

    /// <summary>
    /// 获取可用库存
    /// </summary>
    public int GetAvailableInventory()
    {
        if (Status != MediaStatus.Active)
            return 0;

        return TrafficProfile.DailyInventory - GetTodayDeliveryCount();
    }

    /// <summary>
    /// 获取今日投放次数
    /// </summary>
    public int GetTodayDeliveryCount()
    {
        var today = DateTime.UtcNow.Date;
        return _deliveryRecords
            .Where(r => r.DeliveredAt.Date == today)
            .Sum(r => (int)r.Metrics.Impressions);
    }

    /// <summary>
    /// 记录投放
    /// </summary>
    public void RecordDelivery(DeliveryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (record.MediaResourceId != Id)
            throw new ArgumentException("投放记录的媒体资源ID不匹配");

        if (Status != MediaStatus.Active)
            throw new InvalidOperationException("只有激活状态的媒体资源可以记录投放");

        _deliveryRecords.Add(record);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceDeliveryRecordedEvent(Id, record.Id));
    }

    /// <summary>
    /// 获取性能统计
    /// </summary>
    public MediaPerformanceStats GetPerformanceStats()
    {
        var totalImpressions = _deliveryRecords.Sum(r => r.Metrics.Impressions);
        var totalClicks = _deliveryRecords.Sum(r => r.Metrics.Clicks);
        var totalRevenue = _deliveryRecords.Sum(r => r.Cost * SlotConfig.RevenueShareRate);

        return new MediaPerformanceStats
        {
            TotalImpressions = totalImpressions,
            TotalClicks = totalClicks,
            TotalRevenue = totalRevenue,
            ClickThroughRate = totalImpressions > 0 ? (decimal)totalClicks / totalImpressions : 0m,
            RevenuePerThousandImpressions = totalImpressions > 0 ? totalRevenue / totalImpressions * 1000 : 0m,
            FillRate = TrafficProfile.DailyInventory > 0 ? (decimal)GetTodayDeliveryCount() / TrafficProfile.DailyInventory : 0m
        };
    }

    /// <summary>
    /// 是否可以投放
    /// </summary>
    public bool CanDeliver => Status == MediaStatus.Active &&
                             GetAvailableInventory() > 0 &&
                             !IsDeleted;

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string name, string url, string publisherId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("媒体资源名称不能为空", nameof(name));

        if (name.Length > ValidationConstants.StringLength.MediaNameMaxLength)
            throw new ArgumentException($"媒体资源名称长度不能超过{ValidationConstants.StringLength.MediaNameMaxLength}个字符", nameof(name));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("媒体URL不能为空", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("媒体URL格式不正确", nameof(url));

        if (string.IsNullOrWhiteSpace(publisherId))
            throw new ArgumentException("发布商ID不能为空", nameof(publisherId));
    }
}






