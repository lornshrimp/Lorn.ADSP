using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Constants;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 活动实体（非聚合根）
/// </summary>
public class Campaign : EntityBase
{
    /// <summary>
    /// 活动名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 活动描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 活动状态
    /// </summary>
    public CampaignStatus Status { get; private set; }

    /// <summary>
    /// 广告主ID
    /// </summary>
    public string AdvertisementId { get; private set; } = string.Empty;

    /// <summary>
    /// 定向配置
    /// </summary>
    public TargetingConfig TargetingConfig { get; private set; } = null!;

    /// <summary>
    /// 投放策略
    /// </summary>
    public DeliveryPolicy DeliveryPolicy { get; private set; } = null!;

    /// <summary>
    /// 预算信息
    /// </summary>
    public BudgetInfo Budget { get; private set; } = null!;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartDate { get; private set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// 竞价策略
    /// </summary>
    public BiddingStrategy BiddingStrategy { get; private set; }

    /// <summary>
    /// 私有构造函数，用于ORM
    /// </summary>
    private Campaign() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Campaign(
        string advertisementId,
        string name,
        string description,
        TargetingConfig targetingConfig,
        DeliveryPolicy deliveryPolicy,
        BudgetInfo budget,
        BiddingStrategy biddingStrategy = BiddingStrategy.AutoBid,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        ValidateInputs(advertisementId, name, targetingConfig, deliveryPolicy, budget);

        AdvertisementId = advertisementId;
        Name = name;
        Description = description;
        TargetingConfig = targetingConfig;
        DeliveryPolicy = deliveryPolicy;
        Budget = budget;
        BiddingStrategy = biddingStrategy;
        StartDate = startDate;
        EndDate = endDate;
        Status = CampaignStatus.Draft;
    }

    /// <summary>
    /// 开始活动
    /// </summary>
    public void Start()
    {
        if (Status != CampaignStatus.Draft && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("只有草稿或暂停状态的活动可开始");

        if (!IsWithinScheduledTime())
            throw new InvalidOperationException("不在预定的时间范围内");

        Status = CampaignStatus.Running;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 暂停活动
    /// </summary>
    public void Pause()
    {
        if (Status != CampaignStatus.Running)
            throw new InvalidOperationException("只有运行中的活动可暂停");

        Status = CampaignStatus.Paused;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 恢复活动
    /// </summary>
    public void Resume()
    {
        if (Status != CampaignStatus.Paused)
            throw new InvalidOperationException("只有暂停的活动可恢复");

        Status = CampaignStatus.Running;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 停止活动
    /// </summary>
    public void Stop()
    {
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Cancelled)
            throw new InvalidOperationException("已经完成或取消");

        Status = CampaignStatus.Cancelled;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 完成活动
    /// </summary>
    public void Complete()
    {
        Status = CampaignStatus.Completed;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新预算
    /// </summary>
    public void UpdateBudget(BudgetInfo budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        Budget = budget;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 更新定向配置
    /// </summary>
    public void UpdateTargeting(TargetingConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        TargetingConfig = config;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 从策略模板创建定向配置
    /// </summary>
    public TargetingConfig CreateTargetingFromPolicy(TargetingPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        var config = policy.CreateConfig(Id);
        TargetingConfig = config;
        UpdateLastModifiedTime();
        return config;
    }

    /// <summary>
    /// 检查预算可用性
    /// </summary>
    public bool CheckBudgetAvailability()
    {
        if (Budget.IsExhausted())
        {
            // 预算用尽自动暂停活动
            if (Status == CampaignStatus.Running)
            {
                Pause();
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive => Status == CampaignStatus.Running &&
                           CheckBudgetAvailability() &&
                           IsWithinScheduledTime() &&
                           !IsDeleted;

    public bool CanDeliver { get; internal set; }

    /// <summary>
    /// 是否在预定时间范围内
    /// </summary>
    public bool IsWithinScheduledTime()
    {
        var now = DateTime.UtcNow;

        if (StartDate.HasValue && now < StartDate.Value)
            return false;

        if (EndDate.HasValue && now > EndDate.Value)
        {
            // 超过结束时间状态为运行中，自动完成
            if (Status == CampaignStatus.Running)
            {
                Complete();
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string advertisementId, string name,
        TargetingConfig targetingConfig, DeliveryPolicy deliveryPolicy, BudgetInfo budget)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("活动名不能为空", nameof(name));

        if (name.Length > ValidationConstants.StringLength.CampaignNameMaxLength)
            throw new ArgumentException($"活动名长度不能超过{ValidationConstants.StringLength.CampaignNameMaxLength}个字符", nameof(name));

        ArgumentNullException.ThrowIfNull(targetingConfig, nameof(targetingConfig));
        ArgumentNullException.ThrowIfNull(deliveryPolicy, nameof(deliveryPolicy));
        ArgumentNullException.ThrowIfNull(budget, nameof(budget));
    }
}

