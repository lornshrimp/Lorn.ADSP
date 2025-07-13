using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// 投放记录仓储接口
/// 提供投放记录实体的专业化数据访问操作
/// </summary>
public interface IDeliveryRecordRepository : IRepository<DeliveryRecord>
{
    /// <summary>
    /// 根据活动ID获取投放记录
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>活动的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetByCampaignIdAsync(int campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据日期范围获取投放记录
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定日期范围的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据媒体资源ID获取投放记录
    /// </summary>
    /// <param name="mediaResourceId">媒体资源ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>媒体资源的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetByMediaResourceIdAsync(int mediaResourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取投放统计信息
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>投放统计信息</returns>
    Task<DeliveryStatistics> GetStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取表现最佳的投放记录
    /// </summary>
    /// <param name="count">记录数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表现最佳的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetTopPerformingRecordsAsync(int count, CancellationToken cancellationToken = default);
    new

        /// <summary>
        /// 批量插入投放记录
        /// </summary>
        /// <param name="records">投放记录集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入任务</returns>
        Task BulkInsertAsync(IEnumerable<DeliveryRecord> records, CancellationToken cancellationToken = default);

    /// <summary>
    /// 计算活动总费用
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="date">日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总费用</returns>
    Task<decimal> CalculateTotalCostAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据用户ID获取投放记录
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="fromDate">开始日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetByUserIdAsync(string userId, DateTime fromDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取失败的投放记录
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>失败的投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetFailedDeliveriesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取投放记录按小时统计
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="date">日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>按小时统计的投放数据</returns>
    Task<IEnumerable<HourlyDeliveryStatistics>> GetHourlyStatisticsAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取投放记录按地域统计
    /// </summary>
    /// <param name="campaignId">活动ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>按地域统计的投放数据</returns>
    Task<IEnumerable<RegionalDeliveryStatistics>> GetRegionalStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高性能投放记录
    /// </summary>
    /// <param name="minCtr">最小点击率</param>
    /// <param name="minConversionRate">最小转化率</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>高性能投放记录集合</returns>
    Task<IEnumerable<DeliveryRecord>> GetHighPerformanceRecordsAsync(decimal minCtr, decimal minConversionRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理历史投放记录
    /// </summary>
    /// <param name="retentionDays">保留天数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>清理的记录数量</returns>
    Task<int> CleanupHistoricalRecordsAsync(int retentionDays, CancellationToken cancellationToken = default);
}