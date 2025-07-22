using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 创意素材实体
/// [需要存储] - 数据库存储
/// 广告创意的具体素材内容
/// </summary>
public class Creative : EntityBase
{
    /// <summary>
    /// 广告活动ID - 外键
    /// </summary>
    public Guid CampaignId { get; set; }

    /// <summary>
    /// 广告活动 - 导航属性
    /// </summary>
    public Campaign Campaign { get; set; } = null!;

    /// <summary>
    /// 创意名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 创意类型（Image, Video, Html等）
    /// </summary>
    public string CreativeType { get; set; } = string.Empty;

    /// <summary>
    /// 创意状态
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// 创意内容URL
    /// </summary>
    public string ContentUrl { get; set; } = string.Empty;

    /// <summary>
    /// 点击URL
    /// </summary>
    public string ClickUrl { get; set; } = string.Empty;

    /// <summary>
    /// 创意尺寸（宽x高）
    /// </summary>
    public string Dimensions { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; } = 0;

    /// <summary>
    /// 创意资产集合 - 集合导航属性
    /// </summary>
    public List<CreativeAsset> Assets { get; set; } = new();

    /// <summary>
    /// 私有构造函数（用于EF Core）
    /// </summary>
    private Creative() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public Creative(Guid campaignId, string name, string creativeType)
    {
        CampaignId = campaignId;
        Name = name;
        CreativeType = creativeType;
    }
}
