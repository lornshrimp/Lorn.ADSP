using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Requests;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎状态同步回调接口
/// 负责管理广告投放过程中的各种状态信息，包括预算、频次控制、黑名单等状态数据的获取和同步
/// </summary>
public interface IAdEngineStateCallback : IAdEngineCallback
{
    #region 预算状态管理

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
    /// 批量获取预算状态
    /// </summary>
    /// <param name="campaignIds">活动ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预算状态字典</returns>
    Task<IReadOnlyDictionary<string, BudgetStatus>> GetBudgetStatusBatchAsync(
        IReadOnlyList<string> campaignIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新预算消耗
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="consumedAmount">消耗金额</param>
    /// <param name="transactionId">交易ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateBudgetConsumptionAsync(
        string campaignId,
        decimal consumedAmount,
        string? transactionId = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 频次控制管理

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
    /// 批量获取频次控制状态
    /// </summary>
    /// <param name="requests">频次控制请求列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>频次状态字典</returns>
    Task<IReadOnlyDictionary<string, FrequencyStatus>> GetFrequencyControlBatchAsync(
        IReadOnlyList<FrequencyControlRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新频次计数
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="adId">广告ID</param>
    /// <param name="impressionType">曝光类型（字符串形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateFrequencyCountAsync(
        string userId,
        string adId,
        string impressionType = "Display",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置频次计数
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="adId">广告ID</param>
    /// <param name="resetType">重置类型（字符串形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>重置是否成功</returns>
    Task<bool> ResetFrequencyCountAsync(
        string userId,
        string adId,
        string resetType,
        CancellationToken cancellationToken = default);

    #endregion

    #region 黑名单管理

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
    /// 批量检查黑名单状态
    /// </summary>
    /// <param name="requests">黑名单检查请求列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>黑名单状态字典</returns>
    Task<IReadOnlyDictionary<string, BlacklistStatus>> GetBlacklistStatusBatchAsync(
        IReadOnlyList<BlacklistCheckRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加黑名单项
    /// </summary>
    /// <param name="entityId">实体ID</param>
    /// <param name="entityType">实体类型</param>
    /// <param name="blacklistType">黑名单类型</param>
    /// <param name="reason">添加原因</param>
    /// <param name="expiresAt">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加是否成功</returns>
    Task<bool> AddBlacklistItemAsync(
        string entityId,
        string entityType,
        string blacklistType,
        string? reason = null,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除黑名单项
    /// </summary>
    /// <param name="entityId">实体ID</param>
    /// <param name="entityType">实体类型</param>
    /// <param name="blacklistType">黑名单类型</param>
    /// <param name="reason">移除原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除是否成功</returns>
    Task<bool> RemoveBlacklistItemAsync(
        string entityId,
        string entityType,
        string blacklistType,
        string? reason = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 质量评分管理

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
    /// 批量获取质量评分
    /// </summary>
    /// <param name="adIds">广告ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>质量评分字典</returns>
    Task<IReadOnlyDictionary<string, QualityScore>> GetQualityScoreBatchAsync(
        IReadOnlyList<string> adIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新质量评分
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="qualityScore">新的质量评分</param>
    /// <param name="updateReason">更新原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateQualityScoreAsync(
        string adId,
        QualityScore qualityScore,
        string? updateReason = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 库存状态管理

    /// <summary>
    /// 获取库存状态
    /// </summary>
    /// <param name="placementId">广告位ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存状态</returns>
    Task<InventoryStatus> GetInventoryStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取库存状态
    /// </summary>
    /// <param name="placementIds">广告位ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存状态字典</returns>
    Task<IReadOnlyDictionary<string, InventoryStatus>> GetInventoryStatusBatchAsync(
        IReadOnlyList<string> placementIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预留库存
    /// </summary>
    /// <param name="placementId">广告位ID</param>
    /// <param name="reservationData">预留数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预留结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> ReserveInventoryAsync(
        string placementId,
        IReadOnlyDictionary<string, object> reservationData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 释放库存预留
    /// </summary>
    /// <param name="reservationId">预留ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>释放是否成功</returns>
    Task<bool> ReleaseInventoryReservationAsync(
        string reservationId,
        CancellationToken cancellationToken = default);

    #endregion

    #region 投放状态管理

    /// <summary>
    /// 检查投放资格
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="context">投放上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>投放资格状态</returns>
    Task<DeliveryEligibility> CheckDeliveryEligibilityAsync(
        string adId,
        AdContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量检查投放资格
    /// </summary>
    /// <param name="eligibilityRequests">投放资格检查请求列表（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>投放资格状态字典</returns>
    Task<IReadOnlyDictionary<string, DeliveryEligibility>> CheckDeliveryEligibilityBatchAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> eligibilityRequests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新投放状态
    /// </summary>
    /// <param name="adId">广告ID</param>
    /// <param name="status">新的投放状态（字符串形式）</param>
    /// <param name="reason">状态变更原因</param>
    /// <param name="metadata">状态元数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateDeliveryStatusAsync(
        string adId,
        string status,
        string? reason = null,
        IReadOnlyDictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 竞价上下文管理

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
    /// 创建竞价上下文
    /// </summary>
    /// <param name="biddingRequest">竞价请求（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>竞价上下文</returns>
    Task<BiddingContext> CreateBiddingContextAsync(
        IReadOnlyDictionary<string, object> biddingRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新竞价上下文
    /// </summary>
    /// <param name="requestId">请求ID</param>
    /// <param name="contextUpdate">上下文更新信息（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateBiddingContextAsync(
        string requestId,
        IReadOnlyDictionary<string, object> contextUpdate,
        CancellationToken cancellationToken = default);

    #endregion

    #region 竞争状态管理

    /// <summary>
    /// 获取实时竞争状态
    /// </summary>
    /// <param name="placementId">广告位ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>竞争状态信息（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetCompetitionStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取市场竞争分析
    /// </summary>
    /// <param name="placementId">广告位ID</param>
    /// <param name="timeWindow">时间窗口</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>市场竞争分析（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetMarketCompetitionAnalysisAsync(
        string placementId,
        TimeSpan timeWindow,
        CancellationToken cancellationToken = default);

    #endregion

    #region 状态同步管理

    /// <summary>
    /// 同步状态变更到外部系统
    /// </summary>
    /// <param name="stateChanges">状态变更列表（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>同步结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> SyncStateChangesAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> stateChanges,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量同步状态变更
    /// </summary>
    /// <param name="syncRequests">同步请求列表（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量同步结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> SyncStateChangesBatchAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> syncRequests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取状态同步历史
    /// </summary>
    /// <param name="entityId">实体ID</param>
    /// <param name="entityType">实体类型</param>
    /// <param name="since">开始时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>状态同步历史（字典列表形式）</returns>
    Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> GetStateSyncHistoryAsync(
        string entityId,
        string entityType,
        DateTime? since = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 系统状态监控

    /// <summary>
    /// 获取系统负载状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>系统负载信息（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetSystemLoadStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取组件健康状态
    /// </summary>
    /// <param name="componentName">组件名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>组件健康状态（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetComponentHealthStatusAsync(
        string? componentName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取性能指标快照
    /// </summary>
    /// <param name="metricTypes">指标类型列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>性能指标快照（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetPerformanceSnapshotAsync(
        IReadOnlyList<string>? metricTypes = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region 缓存状态管理

    /// <summary>
    /// 获取缓存状态
    /// </summary>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>缓存状态（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetCacheStatusAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <param name="cacheKeys">缓存键列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>刷新结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> RefreshCacheAsync(
        IReadOnlyList<string> cacheKeys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预热缓存
    /// </summary>
    /// <param name="warmupRequest">预热请求（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预热结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> WarmupCacheAsync(
        IReadOnlyDictionary<string, object> warmupRequest,
        CancellationToken cancellationToken = default);

    #endregion

    #region 状态查询和聚合

    /// <summary>
    /// 获取聚合状态信息
    /// </summary>
    /// <param name="aggregationRequest">聚合请求（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>聚合状态信息（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> GetAggregatedStateInfoAsync(
        IReadOnlyDictionary<string, object> aggregationRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行状态查询
    /// </summary>
    /// <param name="queryRequest">查询请求（字典形式）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>查询结果（字典形式）</returns>
    Task<IReadOnlyDictionary<string, object>> ExecuteStateQueryAsync(
        IReadOnlyDictionary<string, object> queryRequest,
        CancellationToken cancellationToken = default);

    #endregion
}