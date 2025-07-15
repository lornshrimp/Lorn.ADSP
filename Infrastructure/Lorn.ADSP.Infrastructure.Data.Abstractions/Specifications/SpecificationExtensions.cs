using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// 规格模式扩展方法
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// 与其他规格进行And组合
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="left">左侧规格</param>
    /// <param name="right">右侧规格</param>
    /// <returns>组合后的规格</returns>
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new CompositeSpecification<T>(left, right, ExpressionType.AndAlso);
    }

    /// <summary>
    /// 与其他规格进行Or组合
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="left">左侧规格</param>
    /// <param name="right">右侧规格</param>
    /// <returns>组合后的规格</returns>
    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new CompositeSpecification<T>(left, right, ExpressionType.OrElse);
    }

    /// <summary>
    /// 对规格进行Not操作
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specification">规格</param>
    /// <returns>取反后的规格</returns>
    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }

    /// <summary>
    /// 创建表达式规格
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="expression">表达式</param>
    /// <returns>规格</returns>
    public static ISpecification<T> FromExpression<T>(Expression<Func<T, bool>> expression)
    {
        return new ExpressionSpecification<T>(expression);
    }

    /// <summary>
    /// 将规格转换为表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specification">规格</param>
    /// <returns>表达式</returns>
    public static Expression<Func<T, bool>>? ToExpression<T>(this ISpecification<T> specification)
    {
        return specification.Criteria;
    }

    /// <summary>
    /// 将规格转换为Func委托
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specification">规格</param>
    /// <returns>Func委托</returns>
    public static Func<T, bool>? ToFunc<T>(this ISpecification<T> specification)
    {
        return specification.Criteria?.Compile();
    }

    /// <summary>
    /// 检查规格是否为空
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specification">规格</param>
    /// <returns>是否为空</returns>
    public static bool IsEmpty<T>(this ISpecification<T> specification)
    {
        return specification.Criteria == null;
    }

    /// <summary>
    /// 组合多个规格
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specifications">规格列表</param>
    /// <param name="combineType">组合类型</param>
    /// <returns>组合后的规格</returns>
    public static ISpecification<T> Combine<T>(
        this IEnumerable<ISpecification<T>> specifications,
        ExpressionType combineType = ExpressionType.AndAlso)
    {
        var specList = specifications.ToList();
        if (!specList.Any())
        {
            return new EmptySpecification<T>();
        }

        var result = specList.First();
        foreach (var spec in specList.Skip(1))
        {
            result = combineType == ExpressionType.AndAlso
                ? result.And(spec)
                : result.Or(spec);
        }

        return result;
    }

    /// <summary>
    /// 创建分页规格
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="specification">基础规格</param>
    /// <param name="pageIndex">页索引</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>分页规格</returns>
    public static ISpecification<T> WithPaging<T>(
        this ISpecification<T> specification,
        int pageIndex,
        int pageSize)
    {
        return new PagingSpecification<T>(specification, pageIndex, pageSize);
    }

    /// <summary>
    /// 创建排序规格
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">排序键类型</typeparam>
    /// <param name="specification">基础规格</param>
    /// <param name="orderBy">排序表达式</param>
    /// <param name="ascending">是否升序</param>
    /// <returns>排序规格</returns>
    public static ISpecification<T> WithOrdering<T, TKey>(
        this ISpecification<T> specification,
        Expression<Func<T, TKey>> orderBy,
        bool ascending = true)
    {
        return new OrderingSpecification<T, TKey>(specification, orderBy, ascending);
    }
}

/// <summary>
/// 组合规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
internal class CompositeSpecification<T> : BaseSpecification<T>
{
    public CompositeSpecification(
        ISpecification<T> left,
        ISpecification<T> right,
        ExpressionType combineType)
    {
        if (left.Criteria != null && right.Criteria != null)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(left.Criteria.Parameters[0], parameter);
            var rightVisitor = new ReplaceExpressionVisitor(right.Criteria.Parameters[0], parameter);

            var leftBody = leftVisitor.Visit(left.Criteria.Body);
            var rightBody = rightVisitor.Visit(right.Criteria.Body);

            var combinedBody = combineType == ExpressionType.AndAlso
                ? Expression.AndAlso(leftBody!, rightBody!)
                : Expression.OrElse(leftBody!, rightBody!);

            SetCriteria(Expression.Lambda<Func<T, bool>>(combinedBody, parameter));
        }
        else if (left.Criteria != null)
        {
            SetCriteria(left.Criteria);
        }
        else if (right.Criteria != null)
        {
            SetCriteria(right.Criteria);
        }
    }
}

/// <summary>
/// 表达式规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
internal class ExpressionSpecification<T> : BaseSpecification<T>
{
    public ExpressionSpecification(Expression<Func<T, bool>> expression)
    {
        SetCriteria(expression);
    }
}

/// <summary>
/// 空规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
internal class EmptySpecification<T> : BaseSpecification<T>
{
    public EmptySpecification()
    {
        // 空规格不设置任何条件
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return true; // 空规格对所有实体都返回true
    }
}

/// <summary>
/// 分页规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
internal class PagingSpecification<T> : BaseSpecification<T>
{
    public PagingSpecification(ISpecification<T> specification, int pageIndex, int pageSize)
    {
        if (specification.Criteria != null)
        {
            SetCriteria(specification.Criteria);
        }

        // 复制包含
        foreach (var include in specification.Includes)
        {
            AddInclude(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            AddInclude(includeString);
        }

        // 应用分页
        ApplyPaging(pageIndex * pageSize, pageSize);
    }
}

/// <summary>
/// 排序规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TKey">排序键类型</typeparam>
internal class OrderingSpecification<T, TKey> : BaseSpecification<T>
{
    public OrderingSpecification(
        ISpecification<T> specification,
        Expression<Func<T, TKey>> orderBy,
        bool ascending)
    {
        if (specification.Criteria != null)
        {
            SetCriteria(specification.Criteria);
        }

        // 复制包含
        foreach (var include in specification.Includes)
        {
            AddInclude(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            AddInclude(includeString);
        }

        // 应用排序
        if (ascending)
        {
            ApplyOrderBy(orderBy as Expression<Func<T, object>> ??
                        Expression.Lambda<Func<T, object>>(
                            Expression.Convert(orderBy.Body, typeof(object)),
                            orderBy.Parameters));
        }
        else
        {
            ApplyOrderByDescending(orderBy as Expression<Func<T, object>> ??
                                 Expression.Lambda<Func<T, object>>(
                                     Expression.Convert(orderBy.Body, typeof(object)),
                                     orderBy.Parameters));
        }
    }
}