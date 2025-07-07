using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 广告尺寸值对象
/// </summary>
public class AdSize : ValueObject
{
    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// 尺寸类型
    /// </summary>
    public string SizeType { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AdSize(int width, int height, string sizeType)
    {
        Width = width;
        Height = height;
        SizeType = sizeType;
    }

    /// <summary>
    /// 创建广告尺寸
    /// </summary>
    public static AdSize Create(int width, int height, string? sizeType = null)
    {
        if (width <= 0)
            throw new ArgumentException("宽度必须大于0", nameof(width));

        if (height <= 0)
            throw new ArgumentException("高度必须大于0", nameof(height));

        var calculatedSizeType = sizeType ?? GetSizeType(width, height);

        return new AdSize(width, height, calculatedSizeType);
    }

    /// <summary>
    /// 创建标准尺寸
    /// </summary>
    public static AdSize CreateStandard(StandardAdSize standardSize)
    {
        return standardSize switch
        {
            StandardAdSize.Banner => new AdSize(728, 90, "横幅广告"),
            StandardAdSize.Leaderboard => new AdSize(728, 90, "导航条广告"),
            StandardAdSize.Rectangle => new AdSize(300, 250, "矩形广告"),
            StandardAdSize.Skyscraper => new AdSize(160, 600, "摩天大楼广告"),
            StandardAdSize.MobileBanner => new AdSize(320, 50, "移动横幅广告"),
            StandardAdSize.MobileRectangle => new AdSize(300, 250, "移动矩形广告"),
            StandardAdSize.FullScreen => new AdSize(0, 0, "全屏广告"),
            StandardAdSize.Native => new AdSize(0, 0, "原生广告"),
            _ => throw new ArgumentException($"不支持的标准尺寸: {standardSize}", nameof(standardSize))
        };
    }

    /// <summary>
    /// 检查是否为移动端尺寸
    /// </summary>
    public bool IsMobileSize()
    {
        // 移动端常见尺寸
        return (Width == 320 && Height == 50) ||  // Mobile Banner
               (Width == 300 && Height == 250) || // Mobile Rectangle
               (Width == 320 && Height == 100) || // Large Mobile Banner
               (Width == 300 && Height == 50) ||  // Mobile Banner Small
               Width <= 400; // 一般移动端宽度限制
    }

    /// <summary>
    /// 检查是否为全屏尺寸
    /// </summary>
    public bool IsFullScreen()
    {
        return Width == 0 && Height == 0 && SizeType == "全屏广告";
    }

    /// <summary>
    /// 检查是否为原生尺寸
    /// </summary>
    public bool IsNative()
    {
        return Width == 0 && Height == 0 && SizeType == "原生广告";
    }

    /// <summary>
    /// 获取纵横比
    /// </summary>
    public double GetAspectRatio()
    {
        if (Height == 0)
            return 0;

        return (double)Width / Height;
    }

    /// <summary>
    /// 获取面积
    /// </summary>
    public int GetArea()
    {
        return Width * Height;
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    /// <summary>
    /// 获取尺寸类型
    /// </summary>
    private static string GetSizeType(int width, int height)
    {
        return (width, height) switch
        {
            (728, 90) => "横幅广告",
            (300, 250) => "矩形广告",
            (160, 600) => "摩天大楼广告",
            (320, 50) => "移动横幅广告",
            (300, 50) => "移动横幅广告",
            (320, 100) => "大型移动横幅广告",
            (468, 60) => "横幅广告",
            (234, 60) => "半横幅广告",
            (120, 600) => "摩天大楼广告",
            (336, 280) => "大矩形广告",
            (250, 250) => "正方形广告",
            _ => "自定义尺寸"
        };
    }

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
        yield return SizeType;
    }
}


