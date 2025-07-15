using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;



/// <summary>
/// ��ҳѡ��
/// </summary>
public record PageOptions
{
    /// <summary>
    /// ҳ��������0��ʼ��
    /// </summary>
    public int PageIndex { get; init; }

    /// <summary>
    /// ҳ��С
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// ���ҳ��С����
    /// </summary>
    public static int MaxPageSize { get; } = 1000;

    /// <summary>
    /// ��֤��ҳѡ��
    /// </summary>
    /// <returns>��֤���</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (PageIndex < 0)
        {
            errors.Add("Page index cannot be negative");
        }

        if (PageSize <= 0)
        {
            errors.Add("Page size must be greater than 0");
        }

        if (PageSize > MaxPageSize)
        {
            errors.Add($"Page size cannot exceed {MaxPageSize}");
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// ��ѯ���
/// </summary>
/// <typeparam name="T">�������</typeparam>
public record QueryResult<T> where T : class
{
    /// <summary>
    /// ������
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// �ܼ�¼��
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccess { get; init; } = true;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    /// �Ƿ����Ի���
    /// </summary>
    public bool FromCache { get; init; }

    /// <summary>
    /// �����ɹ����
    /// </summary>
    /// <param name="items">������</param>
    /// <param name="totalCount">�ܼ�¼��</param>
    /// <param name="executionTime">ִ��ʱ��</param>
    /// <param name="fromCache">�Ƿ����Ի���</param>
    /// <returns>��ѯ���</returns>
    public static QueryResult<T> Success(IEnumerable<T> items, int totalCount, TimeSpan executionTime, bool fromCache = false)
    {
        return new QueryResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            IsSuccess = true,
            ExecutionTime = executionTime,
            FromCache = fromCache
        };
    }

    /// <summary>
    /// ����ʧ�ܽ��
    /// </summary>
    /// <param name="errorMessage">������Ϣ</param>
    /// <param name="executionTime">ִ��ʱ��</param>
    /// <returns>��ѯ���</returns>
    public static QueryResult<T> Failure(string errorMessage, TimeSpan executionTime)
    {
        return new QueryResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ExecutionTime = executionTime
        };
    }
}

/// <summary>
/// �����������
/// </summary>
/// <typeparam name="T">��������</typeparam>
public record BatchOperationResult<T> where T : class
{
    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// �ɹ��������
    /// </summary>
    public IEnumerable<T> SuccessfulItems { get; init; } = Array.Empty<T>();

    /// <summary>
    /// ʧ�ܵ���
    /// </summary>
    public IEnumerable<BatchFailureItem<T>> FailedItems { get; init; } = Array.Empty<BatchFailureItem<T>>();

    /// <summary>
    /// �ܴ�������
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// �ɹ�����
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// ʧ������
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public BatchOperationType OperationType { get; init; }
}

/// <summary>
/// ��������ʧ����
/// </summary>
/// <typeparam name="T">��������</typeparam>
public record BatchFailureItem<T>
{
    /// <summary>
    /// ʧ�ܵ���
    /// </summary>
    public required T Item { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// �쳣��Ϣ
    /// </summary>
    public Exception? Exception { get; init; }
}

/// <summary>
/// �ִ��ӿ�
/// �̳�ֻ���ִ��ӿڣ������ɾ�Ĳ�����������������
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
public interface IRepository<T> : IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// ���ʵ��
    /// </summary>
    /// <param name="entity">ʵ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ӵ�ʵ��</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// �������ʵ��
    /// </summary>
    /// <param name="entities">ʵ�弯��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ӵ�ʵ�弯��</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ʵ��
    /// </summary>
    /// <param name="entity">ʵ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������ʵ��
    /// </summary>
    /// <param name="entities">ʵ�弯��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// ɾ��ʵ��
    /// </summary>
    /// <param name="entity">ʵ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ɾ������</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ɾ��ʵ��
    /// </summary>
    /// <param name="entities">ʵ�弯��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ɾ������</returns>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// ���ݹ��ɾ��ʵ��
    /// </summary>
    /// <param name="specification">ɾ�����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ɾ���ļ�¼��</returns>
    Task<int> DeleteAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="specification">��������</param>
    /// <param name="updateValues">����ֵ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���µļ�¼��</returns>
    Task<int> BulkUpdateAsync(ISpecification<T> specification, object updateValues, CancellationToken cancellationToken = default);
}