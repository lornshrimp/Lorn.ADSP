using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Domain.Requests;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// �������״̬ͬ���ص��ӿ�
/// </summary>
public interface IAdEngineStateCallback : IAdEngineCallback
{
    /// <summary>
    /// ��ȡԤ��״̬
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ��״̬</returns>
    Task<BudgetStatus> GetBudgetStatusAsync(
        string campaignId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡƵ�ο���״̬
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="adId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ƶ��״̬</returns>
    Task<FrequencyStatus> GetFrequencyControlAsync(
        string userId,
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ������״̬
    /// </summary>
    /// <param name="request">�������������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������״̬</returns>
    Task<BlacklistStatus> GetBlacklistStatusAsync(
        BlacklistCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task<QualityScore> GetQualityScoreAsync(
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�����״̬
    /// </summary>
    /// <param name="placementId">���λID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���״̬</returns>
    Task<InventoryStatus> GetInventoryStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����������״̬
    /// </summary>
    /// <param name="requestId">����ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����������</returns>
    Task<BiddingContext> GetBiddingContextAsync(
        string requestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡƵ�ο���״̬
    /// </summary>
    /// <param name="requests">Ƶ�ο��������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ƶ��״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, FrequencyStatus>> GetFrequencyControlBatchAsync(
        IReadOnlyList<FrequencyControlRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �����Ͷ���ʸ�
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="context">Ͷ��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ͷ���ʸ�״̬</returns>
    Task<DeliveryEligibility> CheckDeliveryEligibilityAsync(
        string adId,
        AdContext context,
        CancellationToken cancellationToken = default);
}