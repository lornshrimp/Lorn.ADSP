using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;



/// <summary>
/// 分页选项
/// </summary>
public record PageOptions
{
    /// <summary>
    /// 页索引（从0开始）
    /// </summary>
    public int PageIndex { get; init; }

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// 最大页大小限制
    /// </summary>
    public static int MaxPageSize { get; } = 1000;

    /// <summary>
    /// 验证分页选项
    /// </summary>
    /// <returns>验证结果</returns>
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
/// 查询结果
/// </summary>
/// <typeparam name="T">结果类型</typeparam>
public record QueryResult<T> where T : class
{
    /// <summary>
    /// 数据项
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; init; } = true;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    /// 是否来自缓存
    /// </summary>
    public bool FromCache { get; init; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    /// <param name="items">数据项</param>
    /// <param name="totalCount">总记录数</param>
    /// <param name="executionTime">执行时间</param>
    /// <param name="fromCache">是否来自缓存</param>
    /// <returns>查询结果</returns>
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
    /// 创建失败结果
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="executionTime">执行时间</param>
    /// <returns>查询结果</returns>
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
/// 批量操作结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public record BatchOperationResult<T> where T : class
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 成功处理的项
    /// </summary>
    public IEnumerable<T> SuccessfulItems { get; init; } = Array.Empty<T>();

    /// <summary>
    /// 失败的项
    /// </summary>
    public IEnumerable<BatchFailureItem<T>> FailedItems { get; init; } = Array.Empty<BatchFailureItem<T>>();

    /// <summary>
    /// 总处理数量
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// 成功数量
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// 失败数量
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public TimeSpan ExecutionTime { get; init; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public BatchOperationType OperationType { get; init; }
}

/// <summary>
/// 批量操作失败项
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public record BatchFailureItem<T>
{
    /// <summary>
    /// 失败的项
    /// </summary>
    public required T Item { get; init; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public Exception? Exception { get; init; }
}

/// <summary>
/// 仓储接口
/// 继承只读仓储接口，添加增删改操作和批量处理能力
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> : IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>添加的实体集合</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新任务</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新任务</returns>
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除任务</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除任务</returns>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规格删除实体
    /// </summary>
    /// <param name="specification">删除规格</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的记录数</returns>
    Task<int> DeleteAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新
    /// </summary>
    /// <param name="specification">更新条件</param>
    /// <param name="updateValues">更新值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的记录数</returns>
    Task<int> BulkUpdateAsync(ISpecification<T> specification, object updateValues, CancellationToken cancellationToken = default);
}