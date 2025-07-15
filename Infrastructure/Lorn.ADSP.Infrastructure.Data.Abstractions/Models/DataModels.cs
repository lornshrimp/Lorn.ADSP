using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;



/// <summary>
/// 分页结果模型
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 数据项集合
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 页索引（从0开始）
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages - 1;

    /// <summary>
    /// 当前页的起始记录号
    /// </summary>
    public int StartRecord => PageIndex * PageSize + 1;

    /// <summary>
    /// 当前页的结束记录号
    /// </summary>
    public int EndRecord => Math.Min((PageIndex + 1) * PageSize, TotalCount);

    /// <summary>
    /// 构造函数
    /// </summary>
    public PagedResult()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="items">数据项</param>
    /// <param name="totalCount">总记录数</param>
    /// <param name="pageIndex">页索引</param>
    /// <param name="pageSize">页大小</param>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// 创建空的分页结果
    /// </summary>
    /// <param name="pageIndex">页索引</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>空分页结果</returns>
    public static PagedResult<T> Empty(int pageIndex, int pageSize)
    {
        return new PagedResult<T>([], 0, pageIndex, pageSize);
    }

    /// <summary>
    /// 创建分页结果
    /// </summary>
    /// <param name="items">数据项</param>
    /// <param name="totalCount">总记录数</param>
    /// <param name="pageIndex">页索引</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>分页结果</returns>
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, pageIndex, pageSize);
    }
}

/// <summary>
/// 查询选项
/// </summary>
public class QueryOptions
{
    /// <summary>
    /// 是否启用跟踪
    /// </summary>
    public bool TrackingEnabled { get; set; } = true;

    /// <summary>
    /// 查询超时时间（秒）
    /// </summary>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// 是否启用缓存
    /// </summary>
    public bool CacheEnabled { get; set; } = false;

    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public TimeSpan? CacheExpiration { get; set; }

    /// <summary>
    /// 是否启用拆分查询
    /// </summary>
    public bool SplitQueryEnabled { get; set; } = false;

    /// <summary>
    /// 包含的导航属性
    /// </summary>
    public List<string> Includes { get; set; } = [];

    /// <summary>
    /// 排序选项
    /// </summary>
    public List<SortOption> SortOptions { get; set; } = [];

    /// <summary>
    /// 创建默认查询选项
    /// </summary>
    /// <returns>默认查询选项</returns>
    public static QueryOptions Default()
    {
        return new QueryOptions();
    }

    /// <summary>
    /// 创建只读查询选项
    /// </summary>
    /// <returns>只读查询选项</returns>
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
/// 排序选项
/// </summary>
public class SortOption
{
    /// <summary>
    /// 字段名称
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 排序方向
    /// </summary>
    public OrderDirection Direction { get; set; } = OrderDirection.Ascending;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SortOption()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fieldName">字段名称</param>
    /// <param name="direction">排序方向</param>
    public SortOption(string fieldName, OrderDirection direction = OrderDirection.Ascending)
    {
        FieldName = fieldName;
        Direction = direction;
    }
}

/// <summary>
/// 连接信息
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 连接超时时间（秒）
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 命令超时时间（秒）
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// 连接池最大大小
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// 连接池最小大小
    /// </summary>
    public int MinPoolSize { get; set; } = 0;

    /// <summary>
    /// 是否启用连接池
    /// </summary>
    public bool PoolingEnabled { get; set; } = true;

    /// <summary>
    /// 额外的连接参数
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; set; } = [];

    /// <summary>
    /// 验证连接信息
    /// </summary>
    /// <returns>验证结果</returns>
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
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 错误消息列表
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// 构造函数
    /// </summary>
    public ValidationResult()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isValid">是否有效</param>
    /// <param name="errors">错误列表</param>
    public ValidationResult(bool isValid, IEnumerable<string>? errors = null)
    {
        IsValid = isValid;
        if (errors != null)
        {
            Errors.AddRange(errors);
        }
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <returns>成功结果</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult(true);
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <param name="errors">错误列表</param>
    /// <returns>失败结果</returns>
    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        return new ValidationResult(false, errors);
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <param name="error">错误消息</param>
    /// <returns>失败结果</returns>
    public static ValidationResult Failure(string error)
    {
        return new ValidationResult(false, [error]);
    }
}