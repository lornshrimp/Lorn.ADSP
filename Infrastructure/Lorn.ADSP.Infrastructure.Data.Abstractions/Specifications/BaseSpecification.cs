using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;
using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// 规格模式基础实现类
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// 查询条件表达式
    /// </summary>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    /// 包含的关联实体表达式列表
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    /// <summary>
    /// 包含的关联实体字符串列表
    /// </summary>
    public List<string> IncludeStrings { get; } = [];

    /// <summary>
    /// 排序表达式
    /// </summary>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <summary>
    /// 倒序排序表达式
    /// </summary>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// 分组表达式
    /// </summary>
    public Expression<Func<T, object>>? GroupBy { get; private set; }

    /// <summary>
    /// 获取的记录数量限制
    /// </summary>
    public int Take { get; private set; }

    /// <summary>
    /// 跳过的记录数量
    /// </summary>
    public int Skip { get; private set; }

    /// <summary>
    /// 是否启用分页
    /// </summary>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// 是否启用跟踪
    /// </summary>
    public bool IsTrackingEnabled { get; private set; } = true;

    /// <summary>
    /// 是否启用拆分查询
    /// </summary>
    public bool IsSplitQueryEnabled { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    protected BaseSpecification()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="criteria">查询条件</param>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// 添加包含的关联实体
    /// </summary>
    /// <param name="includeExpression">包含表达式</param>
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// 添加包含的关联实体（字符串方式）
    /// </summary>
    /// <param name="includeString">包含字符串</param>
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// 应用分页
    /// </summary>
    /// <param name="skip">跳过的记录数</param>
    /// <param name="take">获取的记录数</param>
    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    /// <param name="orderByExpression">排序表达式</param>
    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// 应用倒序排序
    /// </summary>
    /// <param name="orderByDescendingExpression">倒序排序表达式</param>
    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// 应用分组
    /// </summary>
    /// <param name="groupByExpression">分组表达式</param>
    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    /// <summary>
    /// 设置查询条件
    /// </summary>
    /// <param name="criteria">查询条件</param>
    protected virtual void SetCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// 禁用跟踪
    /// </summary>
    protected virtual void DisableTracking()
    {
        IsTrackingEnabled = false;
    }

    /// <summary>
    /// 启用拆分查询
    /// </summary>
    protected virtual void EnableSplitQuery()
    {
        IsSplitQueryEnabled = true;
    }

    /// <summary>
    /// 检查实体是否满足规格条件
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>是否满足条件</returns>
    public virtual bool IsSatisfiedBy(T entity)
    {
        if (Criteria == null)
            return true;

        var compiledExpression = Criteria.Compile();
        return compiledExpression(entity);
    }

    /// <summary>
    /// 与其他规格进行And组合
    /// </summary>
    /// <param name="specification">其他规格</param>
    /// <returns>组合后的规格</returns>
    public virtual ISpecification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// 与其他规格进行Or组合
    /// </summary>
    /// <param name="specification">其他规格</param>
    /// <returns>组合后的规格</returns>
    public virtual ISpecification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// 对当前规格进行Not操作
    /// </summary>
    /// <returns>取反后的规格</returns>
    public virtual ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

/// <summary>
/// And组合规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
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
/// Or组合规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
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
/// Not规格
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
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

