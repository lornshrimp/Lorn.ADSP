using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// ����������
/// ���ڴ���������ص���������Ϣ
/// </summary>
public record TransactionContext
{
    /// <summary>
    /// �����ʶ
    /// </summary>
    public string TransactionId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// ��������
    /// </summary>
    public TransactionType TransactionType { get; init; } = TransactionType.Local;

    /// <summary>
    /// ����Χ
    /// </summary>
    public TransactionScope Scope { get; init; } = TransactionScope.Single;

    /// <summary>
    /// һ���Լ���
    /// </summary>
    public ConsistencyLevel ConsistencyLevel { get; init; } = ConsistencyLevel.StrongConsistency;

    /// <summary>
    /// ������ṩ���б�
    /// </summary>
    public IReadOnlyList<string> ParticipatingProviders { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ����Ԫ����
    /// </summary>
    public IReadOnlyDictionary<string, object> TransactionMetadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ��ʱʱ��
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// ���뼶��
    /// </summary>
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.ReadCommitted;

    /// <summary>
    /// �Ƿ������Զ��ύ
    /// </summary>
    public bool AutoCommit { get; init; } = false;

    /// <summary>
    /// �����ʶ
    /// </summary>
    public string RequestId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// �û���ʶ
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// �Ự��ʶ
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// ׷�ٱ�ʶ
    /// </summary>
    public string? TraceId { get; init; }
}



/// <summary>
/// ����Ԫ����
/// �������������������������
/// </summary>
public record TransactionMetadata
{
    /// <summary>
    /// ��������
    /// </summary>
    public required TransactionTechnology Technology { get; init; }

    /// <summary>
    /// ֧�ֵĸ��뼶��
    /// </summary>
    public required IsolationLevel SupportedIsolationLevels { get; init; }

    /// <summary>
    /// �Ƿ�֧�ֲַ�ʽ����
    /// </summary>
    public required bool SupportsDistributed { get; init; }

    /// <summary>
    /// �Ƿ�֧��Ƕ������
    /// </summary>
    public required bool SupportsNested { get; init; }

    /// <summary>
    /// �Ƿ�֧�ֱ����
    /// </summary>
    public required bool SupportsSavepoints { get; init; }

    /// <summary>
    /// ����������ʱ��
    /// </summary>
    public required TimeSpan MaxTransactionDuration { get; init; }

    /// <summary>
    /// ��󲢷�������
    /// </summary>
    public required int MaxConcurrentTransactions { get; init; }

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// ����ѡ��
/// ���������������Ϊ
/// </summary>
public record TransactionOptions
{
    /// <summary>
    /// ���뼶��
    /// </summary>
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.ReadCommitted;

    /// <summary>
    /// ��ʱʱ��
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// �Ƿ������Զ�����
    /// </summary>
    public bool EnableAutoRetry { get; init; } = false;

    /// <summary>
    /// ������Դ���
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// �������ȼ�
    /// </summary>
    public TransactionPriority Priority { get; init; } = TransactionPriority.Normal;

    /// <summary>
    /// �Զ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// �ֲ�ʽ����ѡ��
/// �������÷ֲ�ʽ�������Ϊ
/// </summary>
public record DistributedTransactionOptions
{
    /// <summary>
    /// ȫ�ֳ�ʱʱ��
    /// </summary>
    public TimeSpan GlobalTimeout { get; init; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// һ���Լ���
    /// </summary>
    public ConsistencyLevel ConsistencyLevel { get; init; } = ConsistencyLevel.StrongConsistency;

    /// <summary>
    /// �Ƿ��������׶��ύ
    /// </summary>
    public bool EnableTwoPhaseCommit { get; init; } = true;

    /// <summary>
    /// �����߳�ʱʱ��
    /// </summary>
    public TimeSpan ParticipantTimeout { get; init; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// ������������
    /// </summary>
    public int MaxParticipants { get; init; } = 10;

    /// <summary>
    /// Э������
    /// </summary>
    public CoordinationStrategy CoordinationStrategy { get; init; } = CoordinationStrategy.Centralized;

    /// <summary>
    /// �Զ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// ����ͳ����Ϣ
/// </summary>
public record TransactionStatistics
{
    /// <summary>
    /// �����ʶ
    /// </summary>
    public required string TransactionId { get; init; }

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? EndedAt { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public TimeSpan Duration => EndedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// ��������
    /// </summary>
    public int OperationCount { get; init; }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    public int ReadCount { get; init; }

    /// <summary>
    /// д�����
    /// </summary>
    public int WriteCount { get; init; }

    /// <summary>
    /// ���ȴ�ʱ��
    /// </summary>
    public TimeSpan LockWaitTime { get; init; }

    /// <summary>
    /// ��չͳ����Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedStatistics { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// �ֲ�ʽ����ͳ����Ϣ
/// </summary>
public record DistributedTransactionStatistics
{
    /// <summary>
    /// ȫ�������ʶ
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime? EndedAt { get; init; }

    /// <summary>
    /// �ܳ���ʱ��
    /// </summary>
    public TimeSpan TotalDuration => EndedAt?.Subtract(StartedAt) ?? DateTime.UtcNow.Subtract(StartedAt);

    /// <summary>
    /// ����������
    /// </summary>
    public int ParticipantCount { get; init; }

    /// <summary>
    /// ��������ͳ���б�
    /// </summary>
    public IReadOnlyList<TransactionStatistics> LocalTransactionStatistics { get; init; } = Array.Empty<TransactionStatistics>();

    /// <summary>
    /// ׼���׶γ���ʱ��
    /// </summary>
    public TimeSpan PreparePhaseTime { get; init; }

    /// <summary>
    /// �ύ�׶γ���ʱ��
    /// </summary>
    public TimeSpan CommitPhaseTime { get; init; }

    /// <summary>
    /// Э������ʱ��
    /// </summary>
    public TimeSpan CoordinationOverhead { get; init; }
}

/// <summary>
/// ��������
/// </summary>
public record TransactionCapabilities
{
    /// <summary>
    /// ֧�ֵ���������
    /// </summary>
    public required TransactionType[] SupportedTypes { get; init; }

    /// <summary>
    /// ֧�ֵĸ��뼶��
    /// </summary>
    public required IsolationLevel[] SupportedIsolationLevels { get; init; }

    /// <summary>
    /// ���Ƕ�����
    /// </summary>
    public int MaxNestingDepth { get; init; }

    /// <summary>
    /// �Ƿ�֧��ֻ������
    /// </summary>
    public bool SupportsReadOnlyTransactions { get; init; }

    /// <summary>
    /// �Ƿ�֧�ֿ����ݿ�����
    /// </summary>
    public bool SupportsCrossDatabaseTransactions { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public PerformanceCharacteristics Performance { get; init; } = new();
}

/// <summary>
/// ��������
/// </summary>
public record PerformanceCharacteristics
{
    /// <summary>
    /// ƽ���ύʱ��
    /// </summary>
    public TimeSpan AverageCommitTime { get; init; }

    /// <summary>
    /// ƽ���ع�ʱ��
    /// </summary>
    public TimeSpan AverageRollbackTime { get; init; }

    /// <summary>
    /// �����������ÿ����������
    /// </summary>
    public int MaxThroughput { get; init; }

    /// <summary>
    /// �����ø���
    /// </summary>
    public double LockContentionProbability { get; init; }
}

/// <summary>
/// Э�����
/// </summary>
public record CoordinationResult
{
    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// ȫ�������ʶ
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// �����߽��
    /// </summary>
    public IReadOnlyList<ParticipantResult> ParticipantResults { get; init; } = Array.Empty<ParticipantResult>();

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Э������ʱ��
    /// </summary>
    public TimeSpan CoordinationTime { get; init; }
}

/// <summary>
/// �����߽��
/// </summary>
public record ParticipantResult
{
    /// <summary>
    /// �����߱�ʶ
    /// </summary>
    public required string ParticipantId { get; init; }

    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public TimeSpan ProcessingTime { get; init; }
}

/// <summary>
/// �ֲ�ʽ������
/// </summary>
public record DistributedTransactionResult
{
    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// ȫ�������ʶ
    /// </summary>
    public required string GlobalTransactionId { get; init; }

    /// <summary>
    /// ����״̬
    /// </summary>
    public required DistributedTransactionStatus FinalStatus { get; init; }

    /// <summary>
    /// ����������
    /// </summary>
    public IReadOnlyList<TransactionResult> LocalResults { get; init; } = Array.Empty<TransactionResult>();

    /// <summary>
    /// ͳ����Ϣ
    /// </summary>
    public DistributedTransactionStatistics? Statistics { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ������
/// </summary>
public record TransactionResult
{
    /// <summary>
    /// �Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// �����ʶ
    /// </summary>
    public required string TransactionId { get; init; }

    /// <summary>
    /// ����״̬
    /// </summary>
    public required TransactionStatus FinalStatus { get; init; }

    /// <summary>
    /// Ӱ��ļ�¼��
    /// </summary>
    public int AffectedRows { get; init; }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// ͳ����Ϣ
    /// </summary>
    public TransactionStatistics? Statistics { get; init; }
}