using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ��������ֵ����
/// </summary>
public record QualityScore
{
    /// <summary>
    /// ���ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// ���������֣�0-10��
    /// </summary>
    public decimal OverallScore { get; init; }

    /// <summary>
    /// ����Ե÷֣�0-10��
    /// </summary>
    public decimal RelevanceScore { get; init; }

    /// <summary>
    /// �û�����÷֣�0-10��
    /// </summary>
    public decimal UserExperienceScore { get; init; }

    /// <summary>
    /// Ԥ�ڵ����
    /// </summary>
    public decimal ExpectedCtr { get; init; }

    /// <summary>
    /// Ԥ��ת����
    /// </summary>
    public decimal ExpectedCvr { get; init; }

    /// <summary>
    /// ��ʷ���ֵ÷�
    /// </summary>
    public decimal HistoricalPerformanceScore { get; init; }

    /// <summary>
    /// ���������÷�
    /// </summary>
    public decimal CreativeQualityScore { get; init; }

    /// <summary>
    /// ���ҳ�����÷�
    /// </summary>
    public decimal LandingPageQualityScore { get; init; }

    /// <summary>
    /// ���ּ���ʱ��
    /// </summary>
    public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ���ְ汾
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// ���Ŷ�
    /// </summary>
    public decimal Confidence { get; init; } = 1.0m;
}