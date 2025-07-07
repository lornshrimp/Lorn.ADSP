using Lorn.ADSP.Core.AdEngine.Abstractions.Enums;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ����Ԫ����
/// </summary>
public record StrategyMetadata
{
    /// <summary>
    /// ����Ψһ��ʶ��
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// ���԰汾
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public required StrategyType Type { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// ִ�����ȼ�
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// �Ƿ�֧�ֲ���ִ��
    /// </summary>
    public bool CanRunInParallel { get; init; } = true;

    /// <summary>
    /// Ԥ��ִ��ʱ��
    /// </summary>
    public TimeSpan EstimatedExecutionTime { get; init; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// ��Դ����
    /// </summary>
    public ResourceRequirement ResourceRequirement { get; init; } = new();

    /// <summary>
    /// ����ص��ӿ�����
    /// </summary>
    public IReadOnlyList<Type> RequiredCallbackTypes { get; init; } = Array.Empty<Type>();

    /// <summary>
    /// ����ص�����
    /// </summary>
    public IReadOnlyList<string> RequiredCallbackNames { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ����ģʽ����
    /// </summary>
    public IReadOnlyDictionary<string, object> ConfigurationSchema { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ֧�ֵĹ�������
    /// </summary>
    public IReadOnlyList<string> SupportedFeatures { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ���Ա�ǩ
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ���ݵĹ������
    /// </summary>
    public IReadOnlyList<AdType> CompatibleAdTypes { get; init; } = Array.Empty<AdType>();

    /// <summary>
    /// ��С֧�ֵ�������
    /// </summary>
    public int MinDataSize { get; init; } = 1;

    /// <summary>
    /// ���֧�ֵ�������
    /// </summary>
    public int MaxDataSize { get; init; } = int.MaxValue;

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// ��Դ����
/// </summary>
public record ResourceRequirement
{
    /// <summary>
    /// CPU����ȼ���1-10��
    /// </summary>
    public int CpuRequirement { get; init; } = 5;

    /// <summary>
    /// �ڴ�����MB��
    /// </summary>
    public int MemoryRequirementMB { get; init; } = 100;

    /// <summary>
    /// ��������ȼ���1-10��
    /// </summary>
    public int NetworkRequirement { get; init; } = 5;

    /// <summary>
    /// ����IO����ȼ���1-10��
    /// </summary>
    public int DiskIORequirement { get; init; } = 3;

    /// <summary>
    /// �Ƿ���ҪGPU
    /// </summary>
    public bool RequiresGpu { get; init; } = false;

    /// <summary>
    /// ��󲢷���
    /// </summary>
    public int MaxConcurrency { get; init; } = 10;

    /// <summary>
    /// ��ʱʱ��
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// ���Դ���
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// �Ƿ�Ϊ����������
    /// </summary>
    public bool IsLightweight => CpuRequirement <= 3 && MemoryRequirementMB <= 50 && !RequiresGpu;

    /// <summary>
    /// �Ƿ�Ϊ����������
    /// </summary>
    public bool IsHeavyweight => CpuRequirement >= 8 || MemoryRequirementMB >= 500 || RequiresGpu;
}