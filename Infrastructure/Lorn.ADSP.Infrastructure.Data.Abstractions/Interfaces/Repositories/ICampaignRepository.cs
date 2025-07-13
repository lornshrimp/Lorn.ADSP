using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// ��ִ��ӿ�
/// �ṩ�ʵ���רҵ�����ݷ��ʲ���
/// </summary>
public interface ICampaignRepository : IRepository<Campaign>
{
    /// <summary>
    /// ���ݹ��ID��ȡ�
    /// </summary>
    /// <param name="advertisementId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���Ļ����</returns>
    Task<IEnumerable<Campaign>> GetByAdvertisementIdAsync(int advertisementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��Ծ�Ļ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��Ծ�����</returns>
    Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ����״̬��ȡ�
    /// </summary>
    /// <param name="status">�״̬</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ��״̬�Ļ����</returns>
    Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�����Ͷ�ż�¼
    /// </summary>
    /// <param name="id">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����Ͷ�ż�¼�Ļ</returns>
    Task<Campaign?> GetWithDeliveryRecordsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ѽ��</returns>
    Task<decimal> GetTotalSpentAsync(int campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="date">����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ѽ��</returns>
    Task<decimal> GetDailySpentAsync(int campaignId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡԤ�㳬֧�Ļ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ�㳬֧�Ļ����</returns>
    Task<IEnumerable<Campaign>> GetBudgetExceededCampaignsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ���»Ԥ��
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="spentAmount">���ѽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> UpdateBudgetAsync(int campaignId, decimal spentAmount, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݶ������û�ȡ�
    /// </summary>
    /// <param name="config">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ƥ��Ļ����</returns>
    Task<IEnumerable<Campaign>> GetByTargetingConfigAsync(TargetingConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�������ڵĻ
    /// </summary>
    /// <param name="days">����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������ڵĻ����</returns>
    Task<IEnumerable<Campaign>> GetExpiringSoonAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡԤ�㼴���ľ��Ļ
    /// </summary>
    /// <param name="thresholdPercentage">��ֵ�ٷֱ�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ�㼴���ľ��Ļ����</returns>
    Task<IEnumerable<Campaign>> GetBudgetDepletingSoonAsync(decimal thresholdPercentage, CancellationToken cancellationToken = default);

    /// <summary>
    /// �������»״̬
    /// </summary>
    /// <param name="campaignIds">�ID����</param>
    /// <param name="status">��״̬</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���µĻ����</returns>
    Task<int> BatchUpdateStatusAsync(IEnumerable<int> campaignIds, CampaignStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�����ͳ��
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="startDate">��ʼ����</param>
    /// <param name="endDate">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ͳ��</returns>
    Task<CampaignPerformanceStatistics> GetPerformanceStatisticsAsync(int campaignId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�Ԥ��ʹ�����
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ��ʹ�����</returns>
    Task<CampaignBudgetUsage> GetBudgetUsageAsync(int campaignId, CancellationToken cancellationToken = default);
}