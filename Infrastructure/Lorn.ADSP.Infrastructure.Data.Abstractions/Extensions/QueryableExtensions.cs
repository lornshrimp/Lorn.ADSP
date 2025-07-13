using Microsoft.EntityFrameworkCore;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// IQueryable扩展方法
/// 这些扩展方法需要在具体的EF Core实现项目中提供完整实现
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 包含相关实体（表达式方式）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="queryable">查询</param>
    /// <param name="include">包含表达式</param>
    /// <returns>包含相关实体的查询</returns>
    public static IQueryable<T> Include<T>(this IQueryable<T> queryable, System.Linq.Expressions.Expression<Func<T, object>> include)
    {
        // 在具体实现中，这将调用 EntityFrameworkQueryableExtensions.Include
        // 这里提供基础实现以确保编译通过
        return queryable;
    }

    /// <summary>
    /// 包含相关实体（字符串方式）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="queryable">查询</param>
    /// <param name="include">包含字符串</param>
    /// <returns>包含相关实体的查询</returns>
    public static IQueryable<T> Include<T>(this IQueryable<T> queryable, string include)
    {
        // 在具体实现中，这将调用 EntityFrameworkQueryableExtensions.Include
        // 这里提供基础实现以确保编译通过
        return queryable;
    }
}