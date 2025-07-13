using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// ���ģʽ����ʵ����
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// ��ѯ�������ʽ
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    /// �����Ĺ���ʵ����ʽ�б�
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    /// <summary>
    /// �����Ĺ���ʵ���ַ����б�
    /// </summary>
    public List<string> IncludeStrings { get; } = [];

    /// <summary>
    /// ������ʽ
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// ����������ʽ
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// ������ʽ
    /// </summary>
    public Expression<Func<T, object>>? GroupBy { get; private set; }

    /// <summary>
    /// ��ȡ�ļ�¼��������
    /// </summary>
    public int Take { get; private set; }

    /// <summary>
    /// �����ļ�¼����
    /// </summary>
    public int Skip { get; private set; }

    /// <summary>
    /// �Ƿ����÷�ҳ
    /// </summary>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// �Ƿ����ø���
    /// </summary>
    public bool IsTrackingEnabled { get; private set; } = true;

    /// <summary>
    /// �Ƿ����ò�ֲ�ѯ
    /// </summary>
    public bool IsSplitQueryEnabled { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    protected BaseSpecification()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="criteria">��ѯ����</param>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// ��Ӱ����Ĺ���ʵ��
    /// </summary>
    /// <param name="includeExpression">�������ʽ</param>
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// ��Ӱ����Ĺ���ʵ�壨�ַ�����ʽ��
    /// </summary>
    /// <param name="includeString">�����ַ���</param>
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Ӧ�÷�ҳ
    /// </summary>
    /// <param name="skip">�����ļ�¼��</param>
    /// <param name="take">��ȡ�ļ�¼��</param>
    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Ӧ������
    /// </summary>
    /// <param name="orderByExpression">������ʽ</param>
    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Ӧ�õ�������
    /// </summary>
    /// <param name="orderByDescendingExpression">����������ʽ</param>
    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Ӧ�÷���
    /// </summary>
    /// <param name="groupByExpression">������ʽ</param>
    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    /// <summary>
    /// ���ò�ѯ����
    /// </summary>
    /// <param name="criteria">��ѯ����</param>
    protected virtual void SetCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// ���ø���
    /// </summary>
    protected virtual void DisableTracking()
    {
        IsTrackingEnabled = false;
    }

    /// <summary>
    /// ���ò�ֲ�ѯ
    /// </summary>
    protected virtual void EnableSplitQuery()
    {
        IsSplitQueryEnabled = true;
    }

    /// <summary>
    /// ���ʵ���Ƿ�����������
    /// </summary>
    /// <param name="entity">ʵ�����</param>
    /// <returns>�Ƿ���������</returns>
    public virtual bool IsSatisfiedBy(T entity)
    {
        if (Criteria == null)
            return true;

        var compiledExpression = Criteria.Compile();
        return compiledExpression(entity);
    }

    /// <summary>
    /// ������������And���
    /// </summary>
    /// <param name="specification">�������</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    public virtual ISpecification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// ������������Or���
    /// </summary>
    /// <param name="specification">�������</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    public virtual ISpecification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// �Ե�ǰ������Not����
    /// </summary>
    /// <returns>ȡ����Ĺ��</returns>
    public virtual ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

/// <summary>
/// And��Ϲ��
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class AndSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;

        if (left.Criteria != null && right.Criteria != null)
        {
            SetCriteria(CombineExpressions(left.Criteria, right.Criteria, Expression.AndAlso));
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

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> combineFunction)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);

        var leftBody = leftVisitor.Visit(left.Body);
        var rightBody = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(
            combineFunction(leftBody!, rightBody!), parameter);
    }
}

/// <summary>
/// Or��Ϲ��
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class OrSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;

        if (left.Criteria != null && right.Criteria != null)
        {
            SetCriteria(CombineExpressions(left.Criteria, right.Criteria, Expression.OrElse));
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

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> combineFunction)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);

        var leftBody = leftVisitor.Visit(left.Body);
        var rightBody = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(
            combineFunction(leftBody!, rightBody!), parameter);
    }
}

/// <summary>
/// Not���
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
internal class NotSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;

        if (specification.Criteria != null)
        {
            SetCriteria(Expression.Lambda<Func<T, bool>>(
                Expression.Not(specification.Criteria.Body),
                specification.Criteria.Parameters));
        }
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
}

