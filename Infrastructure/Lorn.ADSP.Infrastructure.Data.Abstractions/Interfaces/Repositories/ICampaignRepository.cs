using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// 活动仓储接口
/// 提供活动实体的专业化数据访问操作
/// </summary>
public interface ICampaignRepository : IRepository<Campaign>
{
    /// <summary>
    /// 根据广告ID获取活动
    /// </summary>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>广告的活动集合</returns>
    Task<IEnumerable<Campaign>> GetByAdvertisementIdAsync(int advertisementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的活动
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>活跃活动集合</returns>
    Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据状态获取活动
    /// </summary>
    /// <param name="status">活动状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定状态的活动集合</returns>
    Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活动及其投放记录
    /// </summary>
    /// <param name="id">活动ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>包含投放记录的活动</returns>
    Task<Campaign?> GetWithDeliveryRecordsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 计算活动总消费
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总消费金额</returns>
    Task<decimal> GetTotalSpentAsync(int campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 计算活动日消费
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="date">日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>日消费金额</returns>
    Task<decimal> GetDailySpentAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取预算超支的活动
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预算超支的活动集合</returns>
    Task<IEnumerable<Campaign>> GetBudgetExceededCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新活动预算
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="spentAmount">消费金额</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateBudgetAsync(int campaignId, decimal spentAmount, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据定向配置获取活动
    /// </summary>
    /// <param name="config">定向配置</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的活动集合</returns>
    Task<IEnumerable<Campaign>> GetByTargetingConfigAsync(TargetingConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取即将到期的活动
    /// </summary>
    /// <param name="days">天数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>即将到期的活动集合</returns>
    Task<IEnumerable<Campaign>> GetExpiringSoonAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取预算即将耗尽的活动
    /// </summary>
    /// <param name="thresholdPercentage">阈值百分比</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预算即将耗尽的活动集合</returns>
    Task<IEnumerable<Campaign>> GetBudgetDepletingSoonAsync(decimal thresholdPercentage, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新活动状态
    /// </summary>
    /// <param name="campaignIds">活动ID集合</param>
    /// <param name="status">新状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的活动数量</returns>
    Task<int> BatchUpdateStatusAsync(IEnumerable<int> campaignIds, CampaignStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活动性能统计
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>性能统计</returns>
    Task<CampaignPerformanceStatistics> GetPerformanceStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活动预算使用情况
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预算使用情况</returns>
    Task<CampaignBudgetUsage> GetBudgetUsageAsync(int campaignId, CancellationToken cancellationToken = default);
}