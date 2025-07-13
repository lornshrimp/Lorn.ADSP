using System.Linq.Expressions;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ���ģʽ�ӿ�
/// ���ڷ�װ��ѯ������ҵ���߼�
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// ��ѯ�������ʽ
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// �����Ĺ���ʵ����ʽ�б�
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// �����Ĺ���ʵ���ַ����б�
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// ������ʽ
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// ����������ʽ
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// ������ʽ
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// ��ȡ�ļ�¼��������
    /// </summary>
    int Take { get; }

    /// <summary>
    /// �����ļ�¼����
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// �Ƿ����÷�ҳ
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// �Ƿ����ø���
    /// </summary>
    bool IsTrackingEnabled { get; }

    /// <summary>
    /// �Ƿ����ò�ֲ�ѯ
    /// </summary>
    bool IsSplitQueryEnabled { get; }

    /// <summary>
    /// ���ʵ���Ƿ�����������
    /// </summary>
    /// <param name="entity">ʵ�����</param>
    /// <returns>�Ƿ���������</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// ������������And���
    /// </summary>
    /// <param name="specification">�������</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    ISpecification<T> And(ISpecification<T> specification);

    /// <summary>
    /// ������������Or���
    /// </summary>
    /// <param name="specification">�������</param>
    /// <returns>��Ϻ�Ĺ��</returns>
    ISpecification<T> Or(ISpecification<T> specification);

    /// <summary>
    /// �Ե�ǰ������Not����
    /// </summary>
    /// <returns>ȡ����Ĺ��</returns>
    ISpecification<T> Not();
}

/// <summary>
/// �ۺϹ��ӿ�
/// ���ڷ�װ�ۺϲ�ѯ�߼�
/// </summary>
/// <typeparam name="T">ʵ������</typeparam>
/// <typeparam name="TResult">�ۺϽ������</typeparam>
public interface IAggregateSpecification<T, TResult>
{
    /// <summary>
    /// ��ѯ�������ʽ
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// �ۺϱ��ʽ
    /// </summary>
    Expression<Func<IQueryable<T>, TResult>> AggregateExpression { get; }

    /// <summary>
    /// ������ʽ
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }
}