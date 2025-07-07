using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
/// </summary>
public record CreativeInfo
{
    /// <summary>
    /// ����ID
    /// </summary>
    public required string CreativeId { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// �ز�URL
    /// </summary>
    public required string MaterialUrl { get; init; }

    /// <summary>
    /// �����תURL
    /// </summary>
    public required string ClickUrl { get; init; }

    /// <summary>
    /// ������
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// ����߶�
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// ý������
    /// </summary>
    public required string MimeType { get; init; }

    /// <summary>
    /// �ļ���С���ֽڣ�
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// �����ʽ
    /// </summary>
    public CreativeFormat Format { get; init; } = CreativeFormat.Banner;

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
}