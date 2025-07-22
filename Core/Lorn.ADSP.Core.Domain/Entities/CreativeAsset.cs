using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 创意资产实体
/// [需要存储] - 数据库存储
/// 创意相关的资产文件
/// </summary>
public class CreativeAsset : EntityBase
{
    /// <summary>
    /// 创意ID - 外键
    /// </summary>
    public Guid CreativeId { get; set; }

    /// <summary>
    /// 创意 - 导航属性
    /// </summary>
    public Creative Creative { get; set; } = null!;

    /// <summary>
    /// 资产类型（MainImage, Thumbnail, Video等）
    /// </summary>
    public string AssetType { get; set; } = string.Empty;

    /// <summary>
    /// 资产URL
    /// </summary>
    public string AssetUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件格式（jpg, png, mp4等）
    /// </summary>
    public string FileFormat { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; } = 0;

    /// <summary>
    /// 资产尺寸（宽x高）
    /// </summary>
    public string Dimensions { get; set; } = string.Empty;

    /// <summary>
    /// 是否为主要资产
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private CreativeAsset() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public CreativeAsset(Guid creativeId, string assetType, string assetUrl)
    {
        CreativeId = creativeId;
        AssetType = assetType;
        AssetUrl = assetUrl;
    }
}
