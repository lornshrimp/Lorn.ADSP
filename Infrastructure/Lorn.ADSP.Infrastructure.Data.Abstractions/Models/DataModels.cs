using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;



/// <summary>
/// ��ҳ���ģ��
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// �������
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// �ܼ�¼��
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// ҳ��������0��ʼ��
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// ҳ��С
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// ��ҳ��
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// �Ƿ�����һҳ
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;

    /// <summary>
    /// �Ƿ�����һҳ
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages - 1;

    /// <summary>
    /// ��ǰҳ����ʼ��¼��
    /// </summary>
    public int StartRecord => PageIndex * PageSize + 1;

    /// <summary>
    /// ��ǰҳ�Ľ�����¼��
    /// </summary>
    public int EndRecord => Math.Min((PageIndex + 1) * PageSize, TotalCount);

    /// <summary>
    /// ���캯��
    /// </summary>
    public PagedResult()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="items">������</param>
    /// <param name="totalCount">�ܼ�¼��</param>
    /// <param name="pageIndex">ҳ����</param>
    /// <param name="pageSize">ҳ��С</param>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// �����յķ�ҳ���
    /// </summary>
    /// <param name="pageIndex">ҳ����</param>
    /// <param name="pageSize">ҳ��С</param>
    /// <returns>�շ�ҳ���</returns>
    public static PagedResult<T> Empty(int pageIndex, int pageSize)
    {
        return new PagedResult<T>([], 0, pageIndex, pageSize);
    }

    /// <summary>
    /// ������ҳ���
    /// </summary>
    /// <param name="items">������</param>
    /// <param name="totalCount">�ܼ�¼��</param>
    /// <param name="pageIndex">ҳ����</param>
    /// <param name="pageSize">ҳ��С</param>
    /// <returns>��ҳ���</returns>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, pageIndex, pageSize);
    }
}

/// <summary>
/// ��ѯѡ��
/// </summary>
public class QueryOptions
{
    /// <summary>
    /// �Ƿ����ø���
    /// </summary>
    public bool TrackingEnabled { get; set; } = true;

    /// <summary>
    /// ��ѯ��ʱʱ�䣨�룩
    /// </summary>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// �Ƿ����û���
    /// </summary>
    public bool CacheEnabled { get; set; } = false;

    /// <summary>
    /// �������ʱ��
    /// </summary>
    public TimeSpan? CacheExpiration { get; set; }

    /// <summary>
    /// �Ƿ����ò�ֲ�ѯ
    /// </summary>
    public bool SplitQueryEnabled { get; set; } = false;

    /// <summary>
    /// �����ĵ�������
    /// </summary>
    public List<string> Includes { get; set; } = [];

    /// <summary>
    /// ����ѡ��
    /// </summary>
    public List<SortOption> SortOptions { get; set; } = [];

    /// <summary>
    /// ����Ĭ�ϲ�ѯѡ��
    /// </summary>
    /// <returns>Ĭ�ϲ�ѯѡ��</returns>
    public static QueryOptions Default()
    {
        return new QueryOptions();
    }

    /// <summary>
    /// ����ֻ����ѯѡ��
    /// </summary>
    /// <returns>ֻ����ѯѡ��</returns>
    public static QueryOptions ReadOnly()
    {
        return new QueryOptions
        {
            TrackingEnabled = false,
            CacheEnabled = true,
            CacheExpiration = TimeSpan.FromMinutes(5)
        };
    }
}

/// <summary>
/// ����ѡ��
/// </summary>
public class SortOption
{
    /// <summary>
    /// �ֶ�����
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public OrderDirection Direction { get; set; } = OrderDirection.Ascending;

    /// <summary>
    /// ���캯��
    /// </summary>
    public SortOption()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="fieldName">�ֶ�����</param>
    /// <param name="direction">������</param>
    public SortOption(string fieldName, OrderDirection direction = OrderDirection.Ascending)
    {
        FieldName = fieldName;
        Direction = direction;
    }
}

/// <summary>
/// ������Ϣ
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// �����ַ���
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// ���ݿ�����
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// ���ӳ�ʱʱ�䣨�룩
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// ���ʱʱ�䣨�룩
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// ���ӳ�����С
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// ���ӳ���С��С
    /// </summary>
    public int MinPoolSize { get; set; } = 0;

    /// <summary>
    /// �Ƿ��������ӳ�
    /// </summary>
    public bool PoolingEnabled { get; set; } = true;

    /// <summary>
    /// ��������Ӳ���
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; set; } = [];

    /// <summary>
    /// ��֤������Ϣ
    /// </summary>
    /// <returns>��֤���</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            errors.Add("Connection string cannot be empty");
        }

        if (ConnectionTimeoutSeconds <= 0)
        {
            errors.Add("Connection timeout must be greater than 0");
        }

        if (CommandTimeoutSeconds <= 0)
        {
            errors.Add("Command timeout must be greater than 0");
        }

        if (MaxPoolSize <= 0)
        {
            errors.Add("Max pool size must be greater than 0");
        }

        if (MinPoolSize < 0)
        {
            errors.Add("Min pool size cannot be negative");
        }

        if (MinPoolSize > MaxPoolSize)
        {
            errors.Add("Min pool size cannot be greater than max pool size");
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors);
    }
}

/// <summary>
/// ��֤���
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// �Ƿ���֤�ɹ�
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// ���캯��
    /// </summary>
    public ValidationResult()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="isValid">�Ƿ���Ч</param>
    /// <param name="errors">�����б�</param>
    public ValidationResult(bool isValid, IEnumerable<string>? errors = null)
    {
        IsValid = isValid;
        if (errors != null)
        {
            Errors.AddRange(errors);
        }
    }

    /// <summary>
    /// �����ɹ����
    /// </summary>
    /// <returns>�ɹ����</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult(true);
    }

    /// <summary>
    /// ����ʧ�ܽ��
    /// </summary>
    /// <param name="errors">�����б�</param>
    /// <returns>ʧ�ܽ��</returns>
    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        return new ValidationResult(false, errors);
    }

    /// <summary>
    /// ����ʧ�ܽ��
    /// </summary>
    /// <param name="error">������Ϣ</param>
    /// <returns>ʧ�ܽ��</returns>
    public static ValidationResult Failure(string error)
    {
        return new ValidationResult(false, [error]);
    }
}