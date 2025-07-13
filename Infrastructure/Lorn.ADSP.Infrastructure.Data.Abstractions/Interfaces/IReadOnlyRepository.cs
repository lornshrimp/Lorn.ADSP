using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ֻ���ִ��ӿ�
/// �ṩ��ѯ����ҳ���ۺϵ�ֻ������
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
public interface IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// ����ID��ȡʵ��
    /// </summary>
    /// <param name="id">ʵ��ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ�����</returns>
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����ʵ��
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ�弯��</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݹ�����ʵ��
    /// </summary>
    /// <param name="specification">��ѯ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ�弯��</returns>
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ҳ��ѯʵ��
    /// </summary>
    /// <param name="specification">��ѯ���</param>
    /// <param name="pageIndex">ҳ����</param>
    /// <param name="pageSize">ҳ��С</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ҳ���</returns>
    Task<PagedResult<T>> GetPagedAsync(ISpecification<T> specification, int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// ͳ��ʵ������
    /// </summary>
    /// <param name="specification">��ѯ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ������</returns>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ʵ���Ƿ����
    /// </summary>
    /// <param name="specification">��ѯ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ����</returns>
    Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// �ۺϲ�ѯ
    /// </summary>
    /// <typeparam name="TResult">�ۺϽ������</typeparam>
    /// <param name="specification">�ۺϹ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ۺϽ��</returns>
    Task<TResult> AggregateAsync<TResult>(IAggregateSpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݱ��ʽ����ʵ��
    /// </summary>
    /// <param name="predicate">��ѯ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ�弯��</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݱ��ʽͳ������
    /// </summary>
    /// <param name="predicate">��ѯ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ʵ������</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݱ��ʽ����Ƿ����
    /// </summary>
    /// <param name="predicate">��ѯ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ����</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}