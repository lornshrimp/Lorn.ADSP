using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// ���ģʽ��չ����
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// ������������And���
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="left">�����</param>
    /// <param name="right">�Ҳ���</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new CompositeSpecification<T>(left, right, ExpressionType.AndAlso);
    }

    /// <summary>
    /// ������������Or���
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="left">�����</param>
    /// <param name="right">�Ҳ���</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new CompositeSpecification<T>(left, right, ExpressionType.OrElse);
    }

    /// <summary>
    /// �Թ�����Not����
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specification">���</param>
    /// <returns>ȡ����Ĺ��</returns>
    public static ISpecification<T> Not<T>(this ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }

    /// <summary>
    /// �������ʽ���
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="expression">���ʽ</param>
    /// <returns>���</returns>
    public static ISpecification<T> FromExpression<T>(Expression<Func<T, bool>> expression)
    {
        return new ExpressionSpecification<T>(expression);
    }

    /// <summary>
    /// �����ת��Ϊ���ʽ
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specification">���</param>
    /// <returns>���ʽ</returns>
    public static Expression<Func<T, bool>>? ToExpression<T>(this ISpecification<T> specification)
    {
        return specification.Criteria;
    }

    /// <summary>
    /// �����ת��ΪFuncί��
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specification">���</param>
    /// <returns>Funcί��</returns>
    public static Func<T, bool>? ToFunc<T>(this ISpecification<T> specification)
    {
        return specification.Criteria?.Compile();
    }

    /// <summary>
    /// ������Ƿ�Ϊ��
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specification">���</param>
    /// <returns>�Ƿ�Ϊ��</returns>
    public static bool IsEmpty<T>(this ISpecification<T> specification)
    {
        return specification.Criteria == null;
    }

    /// <summary>
    /// ��϶�����
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specifications">����б�</param>
    /// <param name="combineType">�������</param>
    /// <returns>��Ϻ�Ĺ��</returns>
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
    /// ������ҳ���
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="specification">�������</param>
    /// <param name="pageIndex">ҳ����</param>
    /// <param name="pageSize">ҳ��С</param>
    /// <returns>��ҳ���</returns>
    public static ISpecification<T> WithPaging<T>(
        this ISpecification<T> specification,
        int pageIndex,
        int pageSize)
    {
        return new PagingSpecification<T>(specification, pageIndex, pageSize);
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <typeparam name="TKey">���������</typeparam>
    /// <param name="specification">�������</param>
    /// <param name="orderBy">������ʽ</param>
    /// <param name="ascending">�Ƿ�����</param>
    /// <returns>������</returns>
    public static ISpecification<T> WithOrdering<T, TKey>(
        this ISpecification<T> specification,
        Expression<Func<T, TKey>> orderBy,
        bool ascending = true)
    {
        return new OrderingSpecification<T, TKey>(specification, orderBy, ascending);
    }
}

/// <summary>
/// ��Ϲ��
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
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
/// ���ʽ���
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class ExpressionSpecification<T> : BaseSpecification<T>
{
    public ExpressionSpecification(Expression<Func<T, bool>> expression)
    {
        SetCriteria(expression);
    }
}

/// <summary>
/// �չ��
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class EmptySpecification<T> : BaseSpecification<T>
{
    public EmptySpecification()
    {
        // �չ�������κ�����
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return true; // �չ�������ʵ�嶼����true
    }
}

/// <summary>
/// ��ҳ���
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class PagingSpecification<T> : BaseSpecification<T>
{
    public PagingSpecification(ISpecification<T> specification, int pageIndex, int pageSize)
    {
        if (specification.Criteria != null)
        {
            SetCriteria(specification.Criteria);
        }

        // ���ư���
        foreach (var include in specification.Includes)
        {
            AddInclude(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            AddInclude(includeString);
        }

        // Ӧ�÷�ҳ
        ApplyPaging(pageIndex * pageSize, pageSize);
    }
}

/// <summary>
/// ������
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
/// <typeparam name="TKey">���������</typeparam>
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

        // ���ư���
        foreach (var include in specification.Includes)
        {
            AddInclude(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            AddInclude(includeString);
        }

        // Ӧ������
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