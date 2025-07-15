using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ���ݷ���·�����ӿ�
/// ����������ݷ�������������ѡ������ʵ����ݷ����ṩ��
/// ʵ�ֻ������ȡ����ݿ⽵������ƽ̨�л���·�ɲ���
/// </summary>
public interface IDataAccessRouter
{
    /// <summary>
    /// �첽·�����ݷ�������
    /// ���������ĺ����õ�·�ɲ���ѡ�����ŵ����ݷ����ṩ��
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ѡ������ݷ����ṩ��</returns>
    Task<IDataAccessProvider?> RouteAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽��ȡ��ѡ���ݷ����ṩ��
    /// �������п��ܴ����������ṩ���б������ȼ�����
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ѡ���ݷ����ṩ�߼���</returns>
    Task<IEnumerable<IDataAccessProvider>> GetCandidateProvidersAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽����·�ɹ���
    /// ֧�ֶ�̬����·�ɲ�������
    /// </summary>
    /// <param name="rules">�µ�·�ɹ�������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task UpdateRoutingRulesAsync(RoutingRule[] rules, CancellationToken cancellationToken = default);

    /// <summary>
    /// �첽��ȡ·�ɾ�����Ϣ
    /// �ṩ��ϸ��·�ɾ��߹�����Ϣ�����ڵ��Ժͼ��
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>·�ɾ��߽��</returns>
    Task<RoutingDecision> GetRoutingDecisionAsync(DataAccessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// ע��·�ɲ���
    /// ֧���Զ���·�ɲ��Ե�ע��
    /// </summary>
    /// <param name="strategy">·�ɲ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ע������</returns>
    Task RegisterRoutingStrategyAsync(IRoutingStrategy strategy, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ·��ͳ����Ϣ
    /// �ṩ·�����ܺͽ���״̬��Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>·��ͳ����Ϣ</returns>
    Task<RoutingStatistics> GetRoutingStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ����·������
    /// ��֤·�ɹ������Ч��
    /// </summary>
    /// <param name="testContext">����������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���Խ��</returns>
    Task<RoutingTestResult> TestRoutingAsync(DataAccessContext testContext, CancellationToken cancellationToken = default);
}

/// <summary>
/// ·�ɲ��Խӿ�
/// �����Զ���·�ɲ��Եı�׼�淶
/// </summary>
public interface IRoutingStrategy
{
    /// <summary>
    /// ��������
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// �������ȼ�
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// �Ƿ�֧��ָ����������
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <returns>�Ƿ�֧��</returns>
    bool SupportsContext(DataAccessContext context);

    /// <summary>
    /// ѡ�����ݷ����ṩ��
    /// </summary>
    /// <param name="context">���ݷ���������</param>
    /// <param name="candidates">��ѡ�ṩ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ѡ����ṩ��</returns>
    Task<IDataAccessProvider?> SelectProviderAsync(
        DataAccessContext context,
        IEnumerable<IDataAccessProvider> candidates,
        CancellationToken cancellationToken = default);
}