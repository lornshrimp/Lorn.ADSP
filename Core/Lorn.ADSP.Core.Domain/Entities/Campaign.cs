using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Events;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Shared.Constants;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告活动实体（聚合根）
/// </summary>
public class Campaign : AggregateRoot
{
    /// <summary>
    /// 广告活动名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 广告活动描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 广告主ID
    /// </summary>
    public string AdvertiserId { get; private set; } = string.Empty;

    /// <summary>
    /// 活动状态
    /// </summary>
    public CampaignStatus Status { get; private set; } = CampaignStatus.Draft;

    /// <summary>
    /// 总预算（分）
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// 日预算（分）
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// 已消费预算（分）
    /// </summary>
    public decimal SpentBudget { get; private set; } = 0;

    /// <summary>
    /// 活动开始日期
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// 活动结束日期
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string TimeZone { get; private set; } = "UTC";

    /// <summary>
    /// 默认定向策略
    /// </summary>
    public TargetingPolicy DefaultTargeting { get; private set; } = TargetingPolicy.CreateEmpty();

    /// <summary>
    /// 默认投放策略
    /// </summary>
    public DeliveryPolicy DefaultDelivery { get; private set; } = null!;

    /// <summary>
    /// 活动标签
    /// </summary>
    public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

    /// <summary>
    /// 活动目标
    /// </summary>
    public CampaignObjective Objective { get; private set; } = null!;

    /// <summary>
    /// 累计展示次数
    /// </summary>
    public long TotalImpressions { get; private set; } = 0;

    /// <summary>
    /// 累计点击次数
    /// </summary>
    public long TotalClicks { get; private set; } = 0;

    /// <summary>
    /// 累计转化次数
    /// </summary>
    public long TotalConversions { get; private set; } = 0;

    /// <summary>
    /// 关联的广告列表
    /// </summary>
    private readonly List<string> _advertisementIds = new();
    public IReadOnlyList<string> AdvertisementIds => _advertisementIds.AsReadOnly();

    /// <summary>
    /// 私有构造函数（用于ORM）
    /// </summary>
    private Campaign() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Campaign(
        string name,
        string description,
        string advertiserId,
        decimal totalBudget,
        decimal dailyBudget,
        DateTime startDate,
        DateTime endDate,
        DeliveryPolicy defaultDelivery,
        CampaignObjective objective,
        string timeZone = "UTC",
        TargetingPolicy? defaultTargeting = null,
        IList<string>? tags = null)
    {
        ValidateInputs(name, advertiserId, totalBudget, dailyBudget, startDate, endDate);

        Name = name;
        Description = description;
        AdvertiserId = advertiserId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        StartDate = startDate;
        EndDate = endDate;
        DefaultDelivery = defaultDelivery;
        Objective = objective;
        TimeZone = timeZone;
        DefaultTargeting = defaultTargeting ?? TargetingPolicy.CreateEmpty();
        Tags = tags?.ToList() ?? new List<string>();

        // 发布活动创建事件
        AddDomainEvent(new CampaignCreatedEvent(Id, advertiserId, name));
    }

    /// <summary>
    /// 更新基本信息
    /// </summary>
    public void UpdateBasicInfo(string name, string description, IList<string>? tags = null)
    {
        if (Status == CampaignStatus.Deleted)
            throw new InvalidOperationException("已删除的活动无法修改");

        ValidateName(name);

        Name = name;
        Description = description;
        Tags = tags?.ToList() ?? new List<string>();

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "基本信息"));
    }

    /// <summary>
    /// 更新预算
    /// </summary>
    public void UpdateBudget(decimal totalBudget, decimal dailyBudget)
    {
        if (Status == CampaignStatus.Deleted)
            throw new InvalidOperationException("已删除的活动无法修改预算");

        ValidateBudget(totalBudget, dailyBudget);

        // 不能将预算调整到低于已消费金额
        if (totalBudget < SpentBudget)
            throw new InvalidOperationException("总预算不能低于已消费金额");

        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignBudgetUpdatedEvent(Id, totalBudget, dailyBudget));
    }

    /// <summary>
    /// 更新投放时间
    /// </summary>
    public void UpdateDeliveryTime(DateTime startDate, DateTime endDate)
    {
        if (Status == CampaignStatus.Active)
            throw new InvalidOperationException("活跃状态的活动无法修改投放时间");

        ValidateDeliveryTime(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "投放时间"));
    }

    /// <summary>
    /// 更新默认定向策略
    /// </summary>
    public void UpdateDefaultTargeting(TargetingPolicy targetingPolicy)
    {
        ArgumentNullException.ThrowIfNull(targetingPolicy);

        DefaultTargeting = targetingPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "默认定向策略"));
    }

    /// <summary>
    /// 更新默认投放策略
    /// </summary>
    public void UpdateDefaultDelivery(DeliveryPolicy deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

        DefaultDelivery = deliveryPolicy;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "默认投放策略"));
    }

    /// <summary>
    /// 更新活动目标
    /// </summary>
    public void UpdateObjective(CampaignObjective objective)
    {
        ArgumentNullException.ThrowIfNull(objective);

        Objective = objective;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignUpdatedEvent(Id, "活动目标"));
    }

    /// <summary>
    /// 提交审核
    /// </summary>
    public void SubmitForReview()
    {
        if (Status != CampaignStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的活动可以提交审核");

        if (!_advertisementIds.Any())
            throw new InvalidOperationException("活动必须包含至少一个广告才能提交审核");

        Status = CampaignStatus.PendingReview;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignSubmittedForReviewEvent(Id, AdvertiserId));
    }

    /// <summary>
    /// 激活活动
    /// </summary>
    public void Activate()
    {
        if (Status != CampaignStatus.PendingReview && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("只有待审核或暂停状态的活动可以激活");

        if (IsExpired)
            throw new InvalidOperationException("已过期的活动无法激活");

        if (!HasSufficientBudget(0))
            throw new InvalidOperationException("预算不足，无法激活活动");

        Status = CampaignStatus.Active;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignActivatedEvent(Id));
    }

    /// <summary>
    /// 暂停活动
    /// </summary>
    public void Pause()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("只有活跃状态的活动可以暂停");

        Status = CampaignStatus.Paused;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignPausedEvent(Id));
    }

    /// <summary>
    /// 结束活动
    /// </summary>
    public void End()
    {
        if (Status == CampaignStatus.Ended || Status == CampaignStatus.Deleted)
            return;

        Status = CampaignStatus.Ended;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignEndedEvent(Id));
    }

    /// <summary>
    /// 删除活动
    /// </summary>
    public override void Delete()
    {
        if (Status == CampaignStatus.Active)
            throw new InvalidOperationException("活跃状态的活动无法删除，请先暂停");

        Status = CampaignStatus.Deleted;
        base.Delete();

        AddDomainEvent(new CampaignDeletedEvent(Id));
    }

    /// <summary>
    /// 添加广告
    /// </summary>
    public void AddAdvertisement(string advertisementId)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        if (_advertisementIds.Contains(advertisementId))
            throw new InvalidOperationException("广告已存在于活动中");

        _advertisementIds.Add(advertisementId);

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementAddedToCampaignEvent(Id, advertisementId));
    }

    /// <summary>
    /// 移除广告
    /// </summary>
    public void RemoveAdvertisement(string advertisementId)
    {
        if (string.IsNullOrWhiteSpace(advertisementId))
            throw new ArgumentException("广告ID不能为空", nameof(advertisementId));

        if (!_advertisementIds.Remove(advertisementId))
            throw new InvalidOperationException("广告不存在于活动中");

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertisementRemovedFromCampaignEvent(Id, advertisementId));
    }

    /// <summary>
    /// 记录展示
    /// </summary>
    public void RecordImpression(decimal cost)
    {
        if (!CanDeliver)
            throw new InvalidOperationException("活动当前状态不允许投放");

        TotalImpressions++;
        SpentBudget += cost;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignImpressionRecordedEvent(Id, cost));

        // 检查预算是否耗尽
        if (!HasSufficientBudget(0))
        {
            AddDomainEvent(new CampaignBudgetExhaustedEvent(Id, SpentBudget));
        }
    }

    /// <summary>
    /// 记录点击
    /// </summary>
    public void RecordClick(decimal cost = 0)
    {
        TotalClicks++;
        if (cost > 0)
        {
            SpentBudget += cost;
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignClickRecordedEvent(Id, cost));
    }

    /// <summary>
    /// 记录转化
    /// </summary>
    public void RecordConversion()
    {
        TotalConversions++;

        UpdateLastModifiedTime();
        AddDomainEvent(new CampaignConversionRecordedEvent(Id));
    }

    /// <summary>
    /// 是否可以投放
    /// </summary>
    public bool CanDeliver => Status == CampaignStatus.Active && 
                             !IsExpired && 
                             IsWithinDeliveryTime &&
                             HasSufficientBudget(0) &&
                             !IsDeleted;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => DateTime.UtcNow.Date > EndDate.Date;

    /// <summary>
    /// 是否在投放时间范围内
    /// </summary>
    public bool IsWithinDeliveryTime
    {
        get
        {
            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
            return now.Date >= StartDate.Date && now.Date <= EndDate.Date;
        }
    }

    /// <summary>
    /// 是否有足够预算
    /// </summary>
    public bool HasSufficientBudget(decimal additionalCost)
    {
        return (SpentBudget + additionalCost) <= TotalBudget;
    }

    /// <summary>
    /// 获取剩余预算
    /// </summary>
    public decimal GetRemainingBudget()
    {
        return Math.Max(0, TotalBudget - SpentBudget);
    }

    /// <summary>
    /// 获取预算使用率
    /// </summary>
    public decimal GetBudgetUtilizationRate()
    {
        return TotalBudget > 0 ? SpentBudget / TotalBudget : 0m;
    }

    /// <summary>
    /// 获取点击率
    /// </summary>
    public decimal GetClickThroughRate()
    {
        return TotalImpressions > 0 ? (decimal)TotalClicks / TotalImpressions : 0m;
    }

    /// <summary>
    /// 获取转化率
    /// </summary>
    public decimal GetConversionRate()
    {
        return TotalClicks > 0 ? (decimal)TotalConversions / TotalClicks : 0m;
    }

    /// <summary>
    /// 获取平均单次展示成本
    /// </summary>
    public decimal GetAverageCostPerImpression()
    {
        return TotalImpressions > 0 ? SpentBudget / TotalImpressions : 0m;
    }

    /// <summary>
    /// 获取平均单次点击成本
    /// </summary>
    public decimal GetAverageCostPerClick()
    {
        return TotalClicks > 0 ? SpentBudget / TotalClicks : 0m;
    }

    /// <summary>
    /// 获取活动剩余天数
    /// </summary>
    public int GetRemainingDays()
    {
        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZone);
        return Math.Max(0, (EndDate.Date - now.Date).Days);
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInputs(string name, string advertiserId, decimal totalBudget, 
        decimal dailyBudget, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);

        if (string.IsNullOrWhiteSpace(advertiserId))
            throw new ArgumentException("广告主ID不能为空", nameof(advertiserId));

        ValidateBudget(totalBudget, dailyBudget);
        ValidateDeliveryTime(startDate, endDate);
    }

    /// <summary>
    /// 验证活动名称
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("活动名称不能为空", nameof(name));

        if (name.Length > ValidationConstants.StringLength.AdTitleMaxLength)
            throw new ArgumentException($"活动名称长度不能超过{ValidationConstants.StringLength.AdTitleMaxLength}个字符", nameof(name));
    }

    /// <summary>
    /// 验证预算
    /// </summary>
    private static void ValidateBudget(decimal totalBudget, decimal dailyBudget)
    {
        if (totalBudget <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalBudget), "总预算必须大于0");

        if (dailyBudget <= 0)
            throw new ArgumentOutOfRangeException(nameof(dailyBudget), "日预算必须大于0");

        if (dailyBudget > totalBudget)
            throw new ArgumentException("日预算不能超过总预算");
    }

    /// <summary>
    /// 验证投放时间
    /// </summary>
    private static void ValidateDeliveryTime(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("开始日期必须早于结束日期");

        if (endDate < DateTime.UtcNow.Date)
            throw new ArgumentException("结束日期不能早于当前日期");
    }
}

/// <summary>
/// 活动目标值对象
/// </summary>
public class CampaignObjective : ValueObject
{
    /// <summary>
    /// 目标类型
    /// </summary>
    public string ObjectiveType { get; private set; } = string.Empty;

    /// <summary>
    /// 目标展示次数
    /// </summary>
    public long? TargetImpressions { get; private set; }

    /// <summary>
    /// 目标点击次数
    /// </summary>
    public long? TargetClicks { get; private set; }

    /// <summary>
    /// 目标转化次数
    /// </summary>
    public long? TargetConversions { get; private set; }

    /// <summary>
    /// 目标点击率
    /// </summary>
    public decimal? TargetClickThroughRate { get; private set; }

    /// <summary>
    /// 目标转化率
    /// </summary>
    public decimal? TargetConversionRate { get; private set; }

    /// <summary>
    /// 目标每次点击成本
    /// </summary>
    public decimal? TargetCostPerClick { get; private set; }

    /// <summary>
    /// 目标每次转化成本
    /// </summary>
    public decimal? TargetCostPerConversion { get; private set; }

    /// <summary>
    /// 目标广告支出回报率
    /// </summary>
    public decimal? TargetReturnOnAdSpend { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private CampaignObjective() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CampaignObjective(
        string objectiveType,
        long? targetImpressions = null,
        long? targetClicks = null,
        long? targetConversions = null,
        decimal? targetClickThroughRate = null,
        decimal? targetConversionRate = null,
        decimal? targetCostPerClick = null,
        decimal? targetCostPerConversion = null,
        decimal? targetReturnOnAdSpend = null)
    {
        if (string.IsNullOrWhiteSpace(objectiveType))
            throw new ArgumentException("目标类型不能为空", nameof(objectiveType));

        ObjectiveType = objectiveType;
        TargetImpressions = targetImpressions;
        TargetClicks = targetClicks;
        TargetConversions = targetConversions;
        TargetClickThroughRate = targetClickThroughRate;
        TargetConversionRate = targetConversionRate;
        TargetCostPerClick = targetCostPerClick;
        TargetCostPerConversion = targetCostPerConversion;
        TargetReturnOnAdSpend = targetReturnOnAdSpend;
    }

    /// <summary>
    /// 创建品牌知名度目标
    /// </summary>
    public static CampaignObjective CreateBrandAwareness(long targetImpressions)
    {
        return new CampaignObjective("BrandAwareness", targetImpressions: targetImpressions);
    }

    /// <summary>
    /// 创建流量目标
    /// </summary>
    public static CampaignObjective CreateTraffic(long targetClicks, decimal? targetCostPerClick = null)
    {
        return new CampaignObjective("Traffic", targetClicks: targetClicks, targetCostPerClick: targetCostPerClick);
    }

    /// <summary>
    /// 创建转化目标
    /// </summary>
    public static CampaignObjective CreateConversions(long targetConversions, decimal? targetCostPerConversion = null)
    {
        return new CampaignObjective("Conversions", targetConversions: targetConversions, targetCostPerConversion: targetCostPerConversion);
    }

    /// <summary>
    /// 创建销售目标
    /// </summary>
    public static CampaignObjective CreateSales(decimal targetReturnOnAdSpend)
    {
        return new CampaignObjective("Sales", targetReturnOnAdSpend: targetReturnOnAdSpend);
    }

    /// <summary>
    /// 计算目标完成率
    /// </summary>
    public decimal CalculateCompletionRate(long actualImpressions, long actualClicks, long actualConversions, decimal actualSpent)
    {
        return ObjectiveType switch
        {
            "BrandAwareness" => TargetImpressions.HasValue ? (decimal)actualImpressions / TargetImpressions.Value : 0m,
            "Traffic" => TargetClicks.HasValue ? (decimal)actualClicks / TargetClicks.Value : 0m,
            "Conversions" => TargetConversions.HasValue ? (decimal)actualConversions / TargetConversions.Value : 0m,
            _ => 0m
        };
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ObjectiveType;
        yield return TargetImpressions ?? 0;
        yield return TargetClicks ?? 0;
        yield return TargetConversions ?? 0;
        yield return TargetClickThroughRate ?? 0m;
        yield return TargetConversionRate ?? 0m;
        yield return TargetCostPerClick ?? 0m;
        yield return TargetCostPerConversion ?? 0m;
        yield return TargetReturnOnAdSpend ?? 0m;
    }
}