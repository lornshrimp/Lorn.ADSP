using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// �������ݷ����ṩ�߽ӿ�
/// ����ͳһ����ӿ�ʵ�ֵ���������ṩ��
/// ͨ��Ԫ���ݱ�ʶ���������ͺ�����
/// </summary>
public interface ITransactionProvider : IDataAccessProvider
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="context">����������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����Χ</returns>
    Task<ITransactionScope> BeginTransactionAsync(TransactionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ύ����
    /// </summary>
    /// <param name="transaction">����Χ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ύ����</returns>
    Task CommitAsync(ITransactionScope transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع�����
    /// </summary>
    /// <param name="transaction">����Χ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackAsync(ITransactionScope transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����״̬
    /// </summary>
    /// <param name="transactionId">�����ʶ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬</returns>
    Task<TransactionStatus> GetStatusAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������ִ�в���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operation">�������</param>
    /// <param name="context">����������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<ITransactionScope, CancellationToken, Task<T>> operation,
        TransactionContext context,
        CancellationToken cancellationToken = default);
}