using System.Data;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ����������ӿ�
/// �ṩͳһ������������֧�ֱ�������ͷֲ�ʽ����
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// ���������Ԫ����
    /// </summary>
    TransactionMetadata Metadata { get; }

    /// <summary>
    /// ��ʼ��������
    /// </summary>
    /// <param name="options">����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����Χ</returns>
    Task<ITransactionScope> BeginTransactionAsync(TransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ʼ�ֲ�ʽ����
    /// </summary>
    /// <param name="options">�ֲ�ʽ����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ֲ�ʽ����Χ</returns>
    Task<IDistributedTransactionScope> BeginDistributedTransactionAsync(DistributedTransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������ִ�в���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="operation">�������</param>
    /// <param name="options">����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<ITransactionScope, CancellationToken, Task<T>> operation, TransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����Ƿ�֧��ָ������������
    /// </summary>
    /// <param name="type">��������</param>
    /// <returns>�Ƿ�֧��</returns>
    Task<bool> SupportsTransactionType(TransactionType type);

    /// <summary>
    /// ��ȡ����������Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task<TransactionCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// ����Э�����ӿ�
/// ����ֲ�ʽ�����Э����һ����
/// </summary>
public interface ITransactionCoordinator
{
    /// <summary>
    /// �����ֲ�ʽ����
    /// </summary>
    /// <param name="options">�ֲ�ʽ����ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ֲ�ʽ����Χ</returns>
    Task<IDistributedTransactionScope> CreateDistributedTransactionAsync(DistributedTransactionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// ע�����������
    /// </summary>
    /// <param name="participant">���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ע����</returns>
    Task<bool> RegisterParticipantAsync(ITransactionManager participant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Э���ύ
    /// </summary>
    /// <param name="globalTransactionId">ȫ�������ʶ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Э�����</returns>
    Task<CoordinationResult> CoordinateCommitAsync(string globalTransactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Э���ع�
    /// </summary>
    /// <param name="globalTransactionId">ȫ�������ʶ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Э�����</returns>
    Task<CoordinationResult> CoordinateRollbackAsync(string globalTransactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��Ծ�����������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����������б�</returns>
    Task<IEnumerable<ITransactionManager>> GetActiveParticipantsAsync(CancellationToken cancellationToken = default);
}