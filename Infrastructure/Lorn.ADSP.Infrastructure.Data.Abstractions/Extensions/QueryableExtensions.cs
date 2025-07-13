using Microsoft.EntityFrameworkCore;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Specifications;

/// <summary>
/// IQueryable��չ����
/// ��Щ��չ������Ҫ�ھ����EF Coreʵ����Ŀ���ṩ����ʵ��
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// �������ʵ�壨���ʽ��ʽ��
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="queryable">��ѯ</param>
    /// <param name="include">�������ʽ</param>
    /// <returns>�������ʵ��Ĳ�ѯ</returns>
    public static IQueryable<T> Include<T>(this IQueryable<T> queryable, System.Linq.Expressions.Expression<Func<T, object>> include)
    {
        // �ھ���ʵ���У��⽫���� EntityFrameworkQueryableExtensions.Include
        // �����ṩ����ʵ����ȷ������ͨ��
        return queryable;
    }

    /// <summary>
    /// �������ʵ�壨�ַ�����ʽ��
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <param name="queryable">��ѯ</param>
    /// <param name="include">�����ַ���</param>
    /// <returns>�������ʵ��Ĳ�ѯ</returns>
    public static IQueryable<T> Include<T>(this IQueryable<T> queryable, string include)
    {
        // �ھ���ʵ���У��⽫���� EntityFrameworkQueryableExtensions.Include
        // �����ṩ����ʵ����ȷ������ͨ��
        return queryable;
    }
}