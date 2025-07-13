using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 规格模式接口
/// 用于封装查询条件和业务逻辑
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// 查询条件表达式
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// 包含的关联实体表达式列表
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// 包含的关联实体字符串列表
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// 排序表达式
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// 倒序排序表达式
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// 分组表达式
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// 获取的记录数量限制
    /// </summary>
    int Take { get; }

    /// <summary>
    /// 跳过的记录数量
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// 是否启用分页
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// 是否启用跟踪
    /// </summary>
    bool IsTrackingEnabled { get; }

    /// <summary>
    /// 是否启用拆分查询
    /// </summary>
    bool IsSplitQueryEnabled { get; }

    /// <summary>
    /// 检查实体是否满足规格条件
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>是否满足条件</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// 与其他规格进行And组合
    /// </summary>
    /// <param name="specification">其他规格</param>
    /// <returns>组合后的规格</returns>
    ISpecification<T> And(ISpecification<T> specification);

    /// <summary>
    /// 与其他规格进行Or组合
    /// </summary>
    /// <param name="specification">其他规格</param>
    /// <returns>组合后的规格</returns>
    ISpecification<T> Or(ISpecification<T> specification);

    /// <summary>
    /// 对当前规格进行Not操作
    /// </summary>
    /// <returns>取反后的规格</returns>
    ISpecification<T> Not();
}

/// <summary>
/// 聚合规格接口
/// 用于封装聚合查询逻辑
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TResult">聚合结果类型</typeparam>
public interface IAggregateSpecification<T, TResult>
{
    /// <summary>
    /// 查询条件表达式
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// 聚合表达式
    /// </summary>
    Expression<Func<IQueryable<T>, TResult>> AggregateExpression { get; }

    /// <summary>
    /// 分组表达式
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }
}