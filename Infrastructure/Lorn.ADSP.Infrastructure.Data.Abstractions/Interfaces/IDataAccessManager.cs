using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ���ݷ��ʹ������ӿ�
/// ����ͳһ����ӿڵ����ݷ���Э�����������ͳ�Ĺ�����Ԫģʽ
/// ͨ�����ݷ����ṩ��ע���ʵ��͸�������ݷ���·��
/// </summary>
public interface IDataAccessManager : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// ��ȡ���ݷ����ṩ��
    /// ������������Ϣ�Զ�·�ɵ����ʵ������ṩ��
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���ݷ����ṩ��</returns>
    Task<IDataAccessProvider> GetProviderAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�����ṩ��
    /// ��������������·�ɵ����ʵ����������
    /// </summary>
    /// <param name="context">����������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ṩ��</returns>
    Task<ITransactionProvider> GetTransactionProviderAsync(TransactionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// ִ�����ݲ���
    /// ֧�ֵ�һ���������������ͳһ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operation">���ݲ���</param>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������</returns>
    Task<T> ExecuteAsync<T>(Func<IDataAccessProvider, CancellationToken, Task<T>> operation, DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������ִ�в���
    /// �Զ���������Ŀ�ʼ���ύ�ͻع�
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operation">�������</param>
    /// <param name="transactionContext">����������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<IDataAccessProvider, ITransactionScope, CancellationToken, Task<T>> operation,
        TransactionContext transactionContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ִ����������
    /// ֧�ֿ����ṩ�ߵ��������ݲ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operations">������������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����������</returns>
    Task<BatchOperationResult<T>> ExecuteBatchAsync<T>(
        IEnumerable<BatchOperationDefinition<T>> operations,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// ��ȡ����״̬
    /// �������ע��������ṩ�ߵĽ���״̬
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬����</returns>
    Task<HealthStatusReport> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡͳ����Ϣ
    /// �ۺ����������ṩ�ߵ�ͳ����Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���ݷ���ͳ����Ϣ</returns>
    Task<DataAccessStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ԥ��������
    /// ����Ԥ���ز�������Ԥȡ�������ݵ�����
    /// </summary>
    /// <param name="preloadRequests">Ԥ���������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ���ؽ��</returns>
    Task<PreloadResult> PreloadDataAsync(IEnumerable<DataPreloadRequest> preloadRequests, CancellationToken cancellationToken = default);

    /// <summary>
    /// ������Դ
    /// �����桢���ӳص���Դ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task CleanupAsync(CancellationToken cancellationToken = default);
}















