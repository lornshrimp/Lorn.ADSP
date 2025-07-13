using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// Ͷ�ż�¼�ִ��ӿ�
/// �ṩͶ�ż�¼ʵ���רҵ�����ݷ��ʲ���
/// </summary>
public interface IDeliveryRecordRepository : IRepository<DeliveryRecord>
{
    /// <summary>
    /// ���ݻID��ȡͶ�ż�¼
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetByCampaignIdAsync(int campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// �������ڷ�Χ��ȡͶ�ż�¼
    /// </summary>
    /// <param name="startDate">��ʼ����</param>
    /// <param name="endDate">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ�����ڷ�Χ��Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ý����ԴID��ȡͶ�ż�¼
    /// </summary>
    /// <param name="mediaResourceId">ý����ԴID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ý����Դ��Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetByMediaResourceIdAsync(int mediaResourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡͶ��ͳ����Ϣ
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="startDate">��ʼ����</param>
    /// <param name="endDate">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ͷ��ͳ����Ϣ</returns>
    Task<DeliveryStatistics> GetStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ������ѵ�Ͷ�ż�¼
    /// </summary>
    /// <param name="count">��¼����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������ѵ�Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetTopPerformingRecordsAsync(int count, CancellationToken cancellationToken = default);
    new

        /// <summary>
        /// ��������Ͷ�ż�¼
        /// </summary>
        /// <param name="records">Ͷ�ż�¼����</param>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>��������</returns>
        Task BulkInsertAsync(IEnumerable<DeliveryRecord> records, CancellationToken cancellationToken = default);

    /// <summary>
    /// �����ܷ���
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="date">����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ܷ���</returns>
    Task<decimal> CalculateTotalCostAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// �����û�ID��ȡͶ�ż�¼
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="fromDate">��ʼ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�û���Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetByUserIdAsync(string userId, DateTime fromDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡʧ�ܵ�Ͷ�ż�¼
    /// </summary>
    /// <param name="startDate">��ʼ����</param>
    /// <param name="endDate">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʧ�ܵ�Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetFailedDeliveriesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡͶ�ż�¼��Сʱͳ��
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="date">����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��Сʱͳ�Ƶ�Ͷ������</returns>
    Task<IEnumerable<HourlyDeliveryStatistics>> GetHourlyStatisticsAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡͶ�ż�¼������ͳ��
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="startDate">��ʼ����</param>
    /// <param name="endDate">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������ͳ�Ƶ�Ͷ������</returns>
    Task<IEnumerable<RegionalDeliveryStatistics>> GetRegionalStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ������Ͷ�ż�¼
    /// </summary>
    /// <param name="minCtr">��С�����</param>
    /// <param name="minConversionRate">��Сת����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������Ͷ�ż�¼����</returns>
    Task<IEnumerable<DeliveryRecord>> GetHighPerformanceRecordsAsync(decimal minCtr, decimal minConversionRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ʷͶ�ż�¼
    /// </summary>
    /// <param name="retentionDays">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ļ�¼����</returns>
    Task<int> CleanupHistoricalRecordsAsync(int retentionDays, CancellationToken cancellationToken = default);
}