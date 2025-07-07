using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Requests;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎状态同步回调接口
/// </summary>
public interface IAdEngineStateCallback : IAdEngineCallback
{
    /// <summary>
    /// 获取预算状态
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预算状态</returns>
    Task<BudgetStatus> GetBudgetStatusAsync(
        string campaignId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取频次控制状态
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="adId">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>频次状态</returns>
    Task<FrequencyStatus> GetFrequencyControlAsync(
        string userId,
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取黑名单状态
    /// </summary>
    /// <param name="request">黑名单检查请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>黑名单状态</returns>
    Task<BlacklistStatus> GetBlacklistStatusAsync(
        BlacklistCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取质量评分
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>质量评分</returns>
    Task<QualityScore> GetQualityScoreAsync(
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告库存状态
    /// </summary>
    /// <param name="placementId">广告位ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存状态</returns>
    Task<InventoryStatus> GetInventoryStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取竞价上下文状态
    /// </summary>
    /// <param name="requestId">请求ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>竞价上下文</returns>
    Task<BiddingContext> GetBiddingContextAsync(
        string requestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取频次控制状态
    /// </summary>
    /// <param name="requests">频次控制请求列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>频次状态字典</returns>
    Task<IReadOnlyDictionary<string, FrequencyStatus>> GetFrequencyControlBatchAsync(
        IReadOnlyList<FrequencyControlRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查广告投放资格
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="context">投放上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>投放资格状态</returns>
    Task<DeliveryEligibility> CheckDeliveryEligibilityAsync(
        string adId,
        AdContext context,
        CancellationToken cancellationToken = default);
}