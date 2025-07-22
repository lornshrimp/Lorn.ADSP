using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Aggregates;

/// <summary>
/// 广告实体（聚合根）
/// [需要存储] - 数据库存储
/// </summary>
public class Advertisement : AggregateRoot
{
    /// <summary>
    /// 广告主ID - 外键（使用Guid）
    /// </summary>
    public Guid AdvertiserId { get; set; }

    /// <summary>
    /// 广告主 - 导航属性
    /// </summary>
    public Advertiser Advertiser { get; set; } = null!;

    /// <summary>
    /// 广告标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 广告描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 广告类型
    /// </summary>
    public AdType Type { get; set; }

    /// <summary>
    /// 广告状态
    /// </summary>
    public AdStatus Status { get; set; } = AdStatus.Draft;

    /// <summary>
    /// 审核信息 - 导航属性
    /// </summary>
    public AuditInfo AuditInfo { get; set; } = null!;

    /// <summary>
    /// 广告活动集合 - 集合导航属性
    /// </summary>
    public List<Campaign> Campaigns { get; set; } = new();

    /// <summary>
    /// 创意集合 - 集合导航属性  
    /// </summary>
    public List<Creative> Creatives { get; set; } = new();

    /// <summary>
    /// 是否活跃（运行时计算属性）
    /// </summary>
    public bool IsActive => Status == AdStatus.Active || Status == AdStatus.Approved;

    /// <summary>
    /// 总花费（运行时计算属性）
    /// </summary>
    public decimal TotalSpent => Campaigns.Sum(c => c.DeliveryRecords.Sum(r => r.Cost));

    /// <summary>
    /// 总展示次数（运行时计算属性）
    /// </summary>
    public long TotalImpressions => Campaigns.Sum(c => c.DeliveryRecords.Count(r => r.Status == DeliveryStatus.Delivered));

    /// <summary>
    /// 总点击次数（运行时计算属性）
    /// 注：点击数据应该通过单独的点击事件或统计表来记录，这里作为占位符返回0
    /// </summary>
    public long TotalClicks => 0; // TODO: 实现真实的点击统计逻辑

    /// <summary>
    /// 私有构造函数，用于EF Core
    /// </summary>
    private Advertisement() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Advertisement(Guid advertiserId, string title, string description, AdType type)
    {
        AdvertiserId = advertiserId;
        Title = title;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// 提交审核
    /// </summary>
    public void SubmitForReview()
    {
        if (Status != AdStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的广告才能提交审核");

        Status = AdStatus.UnderReview;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public void Approve(string reviewerComment)
    {
        if (Status != AdStatus.UnderReview)
            throw new InvalidOperationException("只有审核中的广告才能被批准");

        Status = AdStatus.Approved;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 审核拒绝
    /// </summary>
    public void Reject(string reason)
    {
        if (Status != AdStatus.UnderReview)
            throw new InvalidOperationException("只有审核中的广告才能被拒绝");

        Status = AdStatus.Rejected;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 激活广告
    /// </summary>
    public void Activate()
    {
        if (Status != AdStatus.Approved)
            throw new InvalidOperationException("只有已批准的广告才能被激活");

        Status = AdStatus.Active;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 暂停广告
    /// </summary>
    public void Pause()
    {
        if (Status != AdStatus.Active)
            throw new InvalidOperationException("只有活跃的广告才能被暂停");

        Status = AdStatus.Paused;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 归档广告
    /// </summary>
    public void Archive()
    {
        Status = AdStatus.Archived;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加广告活动
    /// </summary>
    public void AddCampaign(Campaign campaign)
    {
        if (campaign.AdvertisementId != Id)
            throw new InvalidOperationException("活动必须属于当前广告");

        Campaigns.Add(campaign);
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 获取活跃的广告活动
    /// </summary>
    public List<Campaign> GetActiveCampaigns()
    {
        return Campaigns.Where(c => c.Status == CampaignStatus.Active).ToList();
    }
}
