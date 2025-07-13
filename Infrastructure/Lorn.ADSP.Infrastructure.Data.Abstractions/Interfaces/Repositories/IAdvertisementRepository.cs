using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces.Repositories;

/// <summary>
/// ���ִ��ӿ�
/// �ṩ���ʵ���רҵ�����ݷ��ʲ���
/// </summary>
public interface IAdvertisementRepository : IRepository<Advertisement>
{
    /// <summary>
    /// ��ȡ��Ծ�Ĺ��
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��Ծ��漯��</returns>
    Task<IEnumerable<Advertisement>> GetActiveAdvertisementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ����״̬��ȡ���
    /// </summary>
    /// <param name="status">���״̬</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ��״̬�Ĺ�漯��</returns>
    Task<IEnumerable<Advertisement>> GetByStatusAsync(AdStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��漰������Ļ
    /// </summary>
    /// <param name="id">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������Ĺ��</returns>
    Task<Advertisement?> GetWithCampaignsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��漰�䴴���ز�
    /// </summary>
    /// <param name="id">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������Ĺ��</returns>
    Task<Advertisement?> GetWithCreativesAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ������Ƿ��л�Ծ�Ļ
    /// </summary>
    /// <param name="advertisementId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ��л�Ծ�</returns>
    Task<bool> HasActiveCampaignsAsync(int advertisementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݶ�������ƥ����
    /// </summary>
    /// <param name="criteria">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ƥ��Ĺ�漯��</returns>
    Task<IEnumerable<Advertisement>> GetMatchingAdvertisementsAsync(ITargetingCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݹ����ID��ȡ���
    /// </summary>
    /// <param name="advertiserId">�����ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������Ĺ�漯��</returns>
    Task<IEnumerable<Advertisement>> GetByAdvertiserIdAsync(int advertiserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��漰�������ʷ
    /// </summary>
    /// <param name="id">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���������ʷ�Ĺ��</returns>
    Task<Advertisement?> GetWithAuditHistoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݹؼ����������
    /// </summary>
    /// <param name="keywords">�ؼ����б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ƥ��Ĺ�漯��</returns>
    Task<IEnumerable<Advertisement>> SearchByKeywordsAsync(IEnumerable<string> keywords, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����˵Ĺ��
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����˹�漯��</returns>
    Task<IEnumerable<Advertisement>> GetPendingReviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �������¹��״̬
    /// </summary>
    /// <param name="advertisementIds">���ID����</param>
    /// <param name="status">��״̬</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���µĹ������</returns>
    Task<int> BatchUpdateStatusAsync(IEnumerable<int> advertisementIds, AdStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ���ͳ����Ϣ
    /// </summary>
    /// <param name="advertiserId">�����ID����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���ͳ����Ϣ</returns>
    Task<AdvertisementStatistics> GetStatisticsAsync(int? advertiserId = null, CancellationToken cancellationToken = default);
}