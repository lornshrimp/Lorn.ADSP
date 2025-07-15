using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ͳһ���ݷ����ṩ�߽ӿ�
/// �������ݷ���ʵ�֣�ҵ��ʵ�塢���桢���ݿ⡢��ƽ̨����ʵ�ִ�ͳһ�ӿ�
/// ͨ��Ԫ��������ʵ�ָ߶ȿ���չ�����ݷ��ʼܹ�
/// </summary>
public interface IDataAccessProvider
{
    /// <summary>
    /// ��ȡ�ṩ��Ԫ����
    /// ����ҵ��ʵ�����͡��������͡�ƽ̨���͵ȱ�ʶ��Ϣ
    /// </summary>
    /// <returns>�����ṩ��Ԫ����</returns>
    DataProviderMetadata GetMetadata();

    /// <summary>
    /// �첽��ȡ����
    /// ֧�ַ��ͷ������ͣ��ṩ���Ͱ�ȫ�����ݷ���
    /// </summary>
    /// <typeparam name="T">������������</typeparam>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ�����͵����ݽ��</returns>
    Task<T?> GetAsync<T>(DataAccessContext context, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// �첽��������
    /// ֧�����ݵĴ��������²���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="context">���ݷ���������</param>
    /// <param name="value">Ҫ���õ�����ֵ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���ò�������</returns>
    Task SetAsync<T>(DataAccessContext context, T value, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// �첽��������Ƿ����
    /// �ṩ���ݴ����Լ������
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ����</returns>
    Task<bool> ExistsAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽ɾ������
    /// �ṩ����ɾ������
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ɾ����������</returns>
    Task RemoveAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// �������ݲ���
    /// ֧��������ȡ�����á�ɾ���ȸ����ܲ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operation">������������</param>
    /// <param name="contexts">���ݷ��������ļ���</param>
    /// <param name="values">����ֵ���ϣ������������ã�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����������</returns>
    Task<BatchOperationResult<T>> BatchOperationAsync<T>(
        BatchOperationType operation,
        IEnumerable<DataAccessContext> contexts,
        IEnumerable<T>? values = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// ִ�в�ѯ����
    /// ֧�ָ��Ӳ�ѯ��������ҳ������ȸ߼���ѯ����
    /// </summary>
    /// <typeparam name="T">��ѯ�������</typeparam>
    /// <param name="query">��ѯ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ѯ���</returns>
    Task<QueryResult<T>> QueryAsync<T>(IQuerySpecification<T> query, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// ��ȡ�ṩ��ͳ����Ϣ
    /// �ṩ���ܼ�غͽ��������Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ṩ��ͳ����Ϣ</returns>
    Task<DataProviderStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �������
    /// ����ṩ���Ƿ����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ񽡿�</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}