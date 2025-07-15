using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// �����ṩ��ע���ӿ�
/// ��������������ݷ����ṩ�ߵ�ע�ᡢ��ѯ����������
/// ͨ��Ԫ���ݱ�ʶʵ������·�ɺͶ�̬����
/// </summary>
public interface IDataProviderRegistry
{
    /// <summary>
    /// �첽ע�����ݷ����ṩ��
    /// ͨ��Ԫ�����Զ�����������ṩ��
    /// </summary>
    /// <param name="provider">���ݷ����ṩ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ע������</returns>
    Task RegisterProviderAsync(IDataAccessProvider provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽��ȡ���ݷ����ṩ��
    /// ���ݲ�ѯ�����������ƥ����ṩ��
    /// </summary>
    /// <param name="query">�����ṩ�߲�ѯ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ƥ������ݷ����ṩ��</returns>
    Task<IDataAccessProvider?> GetProviderAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽��ȡ������ݷ����ṩ��
    /// ���ݲ�ѯ������������ƥ����ṩ���б�
    /// </summary>
    /// <param name="query">�����ṩ�߲�ѯ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ƥ������ݷ����ṩ�߼���</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersAsync(DataProviderQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽ע�����ݷ����ṩ��
    /// ��ע������Ƴ�ָ�����ṩ��
    /// </summary>
    /// <param name="providerId">�ṩ��ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ�ɹ�ע��</returns>
    Task<bool> UnregisterProviderAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽��ȡ�����ṩ��Ԫ����
    /// ����ע����������ṩ�ߵ�Ԫ������Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ṩ��Ԫ���ݼ���</returns>
    Task<IEnumerable<DataProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ֧��ָ��ҵ��ʵ����ṩ��
    /// �����ȼ����򷵻�
    /// </summary>
    /// <param name="businessEntity">ҵ��ʵ������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>֧�ָ�ҵ��ʵ����ṩ�߼���</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByBusinessEntityAsync(string businessEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ֧��ָ���������͵��ṩ��
    /// ��Redis��SqlServer��
    /// </summary>
    /// <param name="technologyType">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>֧�ָü������͵��ṩ�߼���</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByTechnologyAsync(string technologyType, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ֧��ָ����ƽ̨���ṩ��
    /// ��AlibabaCloud��Azure��
    /// </summary>
    /// <param name="platformType">ƽ̨����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>֧�ָ�ƽ̨���͵��ṩ�߼���</returns>
    Task<IEnumerable<IDataAccessProvider>> GetProvidersByPlatformAsync(string platformType, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ṩ���Ƿ���ע��
    /// </summary>
    /// <param name="providerId">�ṩ��ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ���ע��</returns>
    Task<bool> IsProviderRegisteredAsync(string providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡע���ͳ����Ϣ
    /// �ṩע���Ľ���״̬��ʹ�����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ע���ͳ����Ϣ</returns>
    Task<RegistryStatistics> GetRegistryStatisticsAsync(CancellationToken cancellationToken = default);
}