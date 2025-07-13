using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ����Χ�ӿ�
/// �������������������
/// </summary>
public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// �����ʶ
    /// </summary>
    string TransactionId { get; }

    /// <summary>
    /// ����״̬
    /// </summary>
    TransactionStatus Status { get; }

    /// <summary>
    /// ����ʼʱ��
    /// </summary>
    DateTime StartedAt { get; }

    /// <summary>
    /// ����ʱʱ��
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// ������뼶��
    /// </summary>
    IsolationLevel IsolationLevel { get; }

    /// <summary>
    /// �ύ����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ύ����</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع�����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��Ӳ�������
    /// ������ع�ʱִ�еĲ���
    /// </summary>
    /// <param name="compensation">��������</param>
    void AddCompensation(Func<Task> compensation);

    /// <summary>
    /// ��������Ƿ�����ύ
    /// </summary>
    /// <returns>�Ƿ�����ύ</returns>
    Task<bool> CanCommitAsync();

    /// <summary>
    /// ��ȡ����ͳ����Ϣ
    /// </summary>
    /// <returns>����ͳ��</returns>
    TransactionStatistics GetStatistics();

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="savepointName">���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع��������
    /// </summary>
    /// <param name="savepointName">���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default);
}

/// <summary>
/// �ֲ�ʽ����ӿ�
/// ���������Դ�ķֲ�ʽ����
/// </summary>
public interface IDistributedTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// ȫ�������ʶ
    /// </summary>
    string GlobalTransactionId { get; }

    /// <summary>
    /// ����Χ�б�
    /// </summary>
    IReadOnlyList<ITransactionScope> Scopes { get; }

    /// <summary>
    /// �ֲ�ʽ����״̬
    /// </summary>
    DistributedTransactionStatus Status { get; }

    /// <summary>
    /// ��������Χ
    /// </summary>
    /// <param name="scopeName">��Χ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����Χ</returns>
    Task<ITransactionScope> CreateScopeAsync(string scopeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ύ��������Χ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ύ����</returns>
    Task CommitAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع���������Χ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ִ�зֲ�ʽ�������
    /// </summary>
    /// <param name="operation">�������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������</returns>
    Task<TransactionResult> ExecuteAsync(Func<ITransactionScope, Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�ֲ�ʽ����ͳ����Ϣ
    /// </summary>
    /// <returns>ͳ����Ϣ</returns>
    DistributedTransactionStatistics GetStatistics();
}