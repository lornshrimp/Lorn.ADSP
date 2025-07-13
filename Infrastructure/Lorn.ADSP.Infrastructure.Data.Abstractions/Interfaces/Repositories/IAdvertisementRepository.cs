using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// 广告仓储接口
/// 提供广告实体的专业化数据访问操作
/// </summary>
public interface IAdvertisementRepository : IRepository<Advertisement>
{
    /// <summary>
    /// 获取活跃的广告
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>活跃广告集合</returns>
    Task<IEnumerable<Advertisement>> GetActiveAdvertisementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据状态获取广告
    /// </summary>
    /// <param name="status">广告状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定状态的广告集合</returns>
    Task<IEnumerable<Advertisement>> GetByStatusAsync(AdStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告及其关联的活动
    /// </summary>
    /// <param name="id">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>包含活动的广告</returns>
    Task<Advertisement?> GetWithCampaignsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告及其创意素材
    /// </summary>
    /// <param name="id">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>包含创意的广告</returns>
    Task<Advertisement?> GetWithCreativesAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查广告是否有活跃的活动
    /// </summary>
    /// <param name="advertisementId">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有活跃活动</returns>
    Task<bool> HasActiveCampaignsAsync(int advertisementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据定向条件匹配广告
    /// </summary>
    /// <param name="criteria">定向条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的广告集合</returns>
    Task<IEnumerable<Advertisement>> GetMatchingAdvertisementsAsync(ITargetingCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据广告主ID获取广告
    /// </summary>
    /// <param name="advertiserId">广告主ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>广告主的广告集合</returns>
    Task<IEnumerable<Advertisement>> GetByAdvertiserIdAsync(int advertiserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告及其审核历史
    /// </summary>
    /// <param name="id">广告ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>包含审核历史的广告</returns>
    Task<Advertisement?> GetWithAuditHistoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据关键词搜索广告
    /// </summary>
    /// <param name="keywords">关键词列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>匹配的广告集合</returns>
    Task<IEnumerable<Advertisement>> SearchByKeywordsAsync(IEnumerable<string> keywords, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取待审核的广告
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>待审核广告集合</returns>
    Task<IEnumerable<Advertisement>> GetPendingReviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新广告状态
    /// </summary>
    /// <param name="advertisementIds">广告ID集合</param>
    /// <param name="status">新状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的广告数量</returns>
    Task<int> BatchUpdateStatusAsync(IEnumerable<int> advertisementIds, AdStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告统计信息
    /// </summary>
    /// <param name="advertiserId">广告主ID（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>广告统计信息</returns>
    Task<AdvertisementStatistics> GetStatisticsAsync(int? advertiserId = null, CancellationToken cancellationToken = default);
}