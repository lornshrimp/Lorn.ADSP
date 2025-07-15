using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ·�ɾ��߽��
/// ����·�ɾ��߹��̵���ϸ��Ϣ
/// </summary>
public class RoutingDecision
{
    /// <summary>
    /// ѡ������ݷ����ṩ��
    /// </summary>
    public IDataAccessProvider? SelectedProvider { get; set; }

    /// <summary>
    /// ��ѡ�ṩ���б�
    /// </summary>
    public IReadOnlyList<CandidateProvider> CandidateProviders { get; set; } = Array.Empty<CandidateProvider>();

    /// <summary>
    /// Ӧ�õ�·�ɹ���
    /// </summary>
    public IReadOnlyList<RoutingRule> AppliedRules { get; set; } = Array.Empty<RoutingRule>();

    /// <summary>
    /// ��������
    /// </summary>
    public string DecisionReason { get; set; } = string.Empty;

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime DecisionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ���ߺ�ʱ
    /// </summary>
    public TimeSpan DecisionDuration { get; set; }

    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// ·��������
    /// </summary>
    public DataAccessContext Context { get; set; } = new();

    /// <summary>
    /// ·��ͳ����Ϣ
    /// </summary>
    public Dictionary<string, object> Statistics { get; set; } = new();
}

/// <summary>
/// ��ѡ�ṩ����Ϣ
/// </summary>
public class CandidateProvider
{
    /// <summary>
    /// ���ݷ����ṩ��
    /// </summary>
    public IDataAccessProvider Provider { get; set; } = null!;

    /// <summary>
    /// ƥ�����
    /// </summary>
    public decimal MatchScore { get; set; }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// ������ԭ��
    /// </summary>
    public string? UnavailableReason { get; set; }

    /// <summary>
    /// ƥ��Ĺ���
    /// </summary>
    public IReadOnlyList<string> MatchedRules { get; set; } = Array.Empty<string>();

    /// <summary>
    /// ����ָ��
    /// </summary>
    public PerformanceMetrics PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// ·�ɹ���
/// </summary>
public class RoutingRule
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// ��������
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// �������ȼ�
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// ƥ������
    /// </summary>
    public RuleCondition Condition { get; set; } = new();

    /// <summary>
    /// ·�ɶ���
    /// </summary>
    public RuleAction Action { get; set; } = new();

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ����޸�ʱ��
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ��������
/// </summary>
public class RuleCondition
{
    /// <summary>
    /// ʵ����������
    /// </summary>
    public string[]? EntityTypes { get; set; }

    /// <summary>
    /// ������������
    /// </summary>
    public string[]? OperationTypes { get; set; }

    /// <summary>
    /// һ���Լ�������
    /// </summary>
    public DataConsistencyLevel[]? ConsistencyLevels { get; set; }

    /// <summary>
    /// ʱ�������
    /// </summary>
    public TimeSpan[]? TimeRanges { get; set; }

    /// <summary>
    /// ��ǩ����
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// �Զ����������ʽ
    /// </summary>
    public string? CustomExpression { get; set; }

    /// <summary>
    /// ��չ����
    /// </summary>
    public Dictionary<string, object> ExtendedConditions { get; set; } = new();
}

/// <summary>
/// ������
/// </summary>
public class RuleAction
{
    /// <summary>
    /// ��������
    /// </summary>
    public RouteActionType ActionType { get; set; } = RouteActionType.SelectProvider;

    /// <summary>
    /// Ŀ���ṩ������
    /// </summary>
    public DataProviderType? TargetProviderType { get; set; }

    /// <summary>
    /// Ŀ�꼼������
    /// </summary>
    public string? TargetTechnologyType { get; set; }

    /// <summary>
    /// Ŀ��ƽ̨����
    /// </summary>
    public string? TargetPlatformType { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public FallbackStrategy FallbackStrategy { get; set; } = FallbackStrategy.NextAvailable;

    /// <summary>
    /// Ȩ�ط���
    /// </summary>
    public Dictionary<string, decimal> WeightDistribution { get; set; } = new();

    /// <summary>
    /// ��չ����
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// ����ָ��
/// </summary>
public class PerformanceMetrics
{
    /// <summary>
    /// ƽ����Ӧʱ�䣨���룩
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// �ɹ���
    /// </summary>
    public double SuccessRate { get; set; } = 1.0;

    /// <summary>
    /// ��ǰ����
    /// </summary>
    public double CurrentLoad { get; set; }

    /// <summary>
    /// �����
    /// </summary>
    public double MaxLoad { get; set; } = 1.0;

    /// <summary>
    /// ����������
    /// </summary>
    public int RecentErrorCount { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public double HealthScore { get; set; } = 1.0;
}