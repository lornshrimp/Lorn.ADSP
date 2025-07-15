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
    /// ��������
    /// </summary>
    TransactionType Type { get; }

    /// <summary>
    /// ������뼶��
    /// </summary>
    IsolationLevel IsolationLevel { get; }

    /// <summary>
    /// ����״̬
    /// </summary>
    TransactionStatus Status { get; }

    /// <summary>
    /// ����ʼʱ��
    /// </summary>
    DateTime StartedAt { get; }

    /// <summary>
    /// �������ʱ��
    /// </summary>
    TimeSpan Duration { get; }

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
    /// ���������
    /// </summary>
    /// <param name="name">���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���������Χ</returns>
    Task<ITransactionScope> CreateSavepointAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع��������
    /// </summary>
    /// <param name="name">���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// ע�Ჹ������
    /// ������ع�ʱִ�еĲ���
    /// </summary>
    /// <param name="compensation">��������</param>
    void RegisterCompensation(Func<CancellationToken, Task> compensation);

    /// <summary>
    /// ��ȡ����ͳ����Ϣ
    /// </summary>
    /// <returns>����ͳ��</returns>
    Task<TransactionStatistics> GetStatisticsAsync();
}

/// <summary>
/// �ֲ�ʽ����Χ�ӿ�
/// ���������Դ�ķֲ�ʽ����
/// </summary>
public interface IDistributedTransactionScope : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// ȫ�������ʶ
    /// </summary>
    string GlobalTransactionId { get; }

    /// <summary>
    /// ��������Χ�б�
    /// </summary>
    IReadOnlyList<ITransactionScope> LocalScopes { get; }

    /// <summary>
    /// ������������Χ
    /// </summary>
    /// <param name="scopeName">��Χ����</param>
    /// <param name="options">����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������Χ</returns>
    Task<ITransactionScope> CreateLocalScopeAsync(string scopeName, TransactionOptions options, CancellationToken cancellationToken = default);

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
    /// ׼���׶�
    /// ���׶��ύ�ĵ�һ�׶�
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>׼�����</returns>
    Task<bool> PreparePhaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�ֲ�ʽ������
    /// </summary>
    /// <returns>������</returns>
    Task<DistributedTransactionResult> GetResultAsync();
}