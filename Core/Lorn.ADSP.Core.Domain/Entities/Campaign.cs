using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告活动实体
/// [需要存储] - 数据库存储
/// </summary>
public class Campaign : AggregateRoot
{
    /// <summary>
    /// 活动名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 活动描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 活动状态
    /// </summary>
    public CampaignStatus Status { get; set; }

    /// <summary>
    /// 广告ID - 外键（使用Guid）
    /// </summary>
    public Guid AdvertisementId { get; set; }

    /// <summary>
    /// 广告 - 导航属性
    /// </summary>
    public Advertisement Advertisement { get; set; } = null!;

    /// <summary>
    /// 广告主ID - 外键（使用Guid）
    /// </summary>
    public Guid AdvertiserId { get; set; }

    /// <summary>
    /// 广告主 - 导航属性
    /// </summary>
    public Advertiser Advertiser { get; set; } = null!;

    /// <summary>
    /// 定向配置 - 导航属性
    /// </summary>
    public TargetingConfig TargetingConfig { get; set; } = null!;

    /// <summary>
    /// 投放策略 - 导航属性
    /// </summary>
    public DeliveryPolicy DeliveryPolicy { get; set; } = null!;

    /// <summary>
    /// 预算信息 - 导航属性
    /// </summary>
    public BudgetInfo BudgetInfo { get; set; } = null!;

    /// <summary>
    /// 竞价策略
    /// </summary>
    public BiddingStrategy BiddingStrategy { get; set; } = BiddingStrategy.FixedBid;

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 投放记录集合 - 集合导航属性
    /// </summary>
    public List<DeliveryRecord> DeliveryRecords { get; set; } = new();

    /// <summary>
    /// 私有构造函数，用于EF Core
    /// </summary>
    private Campaign() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Campaign(string name, string description, Guid advertisementId, Guid advertiserId)
    {
        Name = name;
        Description = description;
        AdvertisementId = advertisementId;
        AdvertiserId = advertiserId;
        Status = CampaignStatus.Draft;
    }

    /// <summary>
    /// 检查是否可以投放
    /// </summary>
    public bool CanDeliver
    {
        get
        {
            // 检查活动状态
            if (Status != CampaignStatus.Active && Status != CampaignStatus.Running)
                return false;

            // 检查时间范围
            var now = DateTime.UtcNow;
            if (now < StartDate || now > EndDate)
                return false;

            // 检查预算
            if (BudgetInfo.DailyBudget <= 0 || BudgetInfo.TotalBudget <= 0)
                return false;

            return true;
        }
    }

    /// <summary>
    /// 启动活动
    /// </summary>
    public void Start()
    {
        if (Status != CampaignStatus.Draft && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("只有草稿或暂停状态的活动才能启动");

        Status = CampaignStatus.Active;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 暂停活动
    /// </summary>
    public void Pause()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("只有活跃状态的活动才能暂停");

        Status = CampaignStatus.Paused;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 恢复活动
    /// </summary>
    public void Resume()
    {
        if (Status != CampaignStatus.Paused)
            throw new InvalidOperationException("只有暂停状态的活动才能恢复");

        Status = CampaignStatus.Active;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 停止活动
    /// </summary>
    public void Stop()
    {
        Status = CampaignStatus.Completed;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新预算信息
    /// </summary>
    public void UpdateBudget(BudgetInfo budgetInfo)
    {
        BudgetInfo = budgetInfo ?? throw new ArgumentNullException(nameof(budgetInfo));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新定向配置
    /// </summary>
    public void UpdateTargeting(TargetingConfig targetingConfig)
    {
        TargetingConfig = targetingConfig ?? throw new ArgumentNullException(nameof(targetingConfig));
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 从策略创建定向配置
    /// </summary>
    public TargetingConfig CreateTargetingFromPolicy(TargetingPolicy policy)
    {
        if (policy == null) throw new ArgumentNullException(nameof(policy));

        var config = TargetingConfig.CreateFromPolicy(policy, Id.ToString());

        TargetingConfig = config;
        UpdateLastModifiedTime();

        return config;
    }

    /// <summary>
    /// 检查预算可用性
    /// </summary>
    public bool CheckBudgetAvailability()
    {
        // 临时计算方法 - 运行时使用，不存储
        return BudgetInfo != null && BudgetInfo.RemainingBudget > 0;
    }

    /// <summary>
    /// 获取日消费
    /// </summary>
    public decimal GetDailySpend()
    {
        // 临时计算方法 - 运行时使用，不存储
        var today = DateTime.Today;
        return DeliveryRecords
            .Where(r => r.DeliveredAt.Date == today)
            .Sum(r => r.Cost);
    }
}
