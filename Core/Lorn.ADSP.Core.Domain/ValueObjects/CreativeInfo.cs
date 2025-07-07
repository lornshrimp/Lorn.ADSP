using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 创意信息值对象
/// </summary>
public record CreativeInfo
{
    /// <summary>
    /// 创意ID
    /// </summary>
    public required string CreativeId { get; init; }

    /// <summary>
    /// 创意标题
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// 创意描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 素材URL
    /// </summary>
    public required string MaterialUrl { get; init; }

    /// <summary>
    /// 点击跳转URL
    /// </summary>
    public required string ClickUrl { get; init; }

    /// <summary>
    /// 创意宽度
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// 创意高度
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// 媒体类型
    /// </summary>
    public required string MimeType { get; init; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// 创意格式
    /// </summary>
    public CreativeFormat Format { get; init; } = CreativeFormat.Banner;

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
}