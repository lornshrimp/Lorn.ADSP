using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 创意信息值对象
/// </summary>
public class CreativeInfo : ValueObject
{
    /// <summary>
    /// 创意ID
    /// </summary>
    public string CreativeId { get; private set; }

    /// <summary>
    /// 创意标题
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// 创意描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 素材URL
    /// </summary>
    public string MaterialUrl { get; private set; }

    /// <summary>
    /// 点击跳转URL
    /// </summary>
    public string ClickUrl { get; private set; }

    /// <summary>
    /// 创意宽度
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// 创意高度
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// 媒体类型
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// 创意格式
    /// </summary>
    public CreativeFormat Format { get; private set; }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public IReadOnlyList<ContextProperty> Attributes { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private CreativeInfo(
        string creativeId,
        string title,
        string materialUrl,
        string clickUrl,
        int width,
        int height,
        string mimeType,
        long fileSize,
        CreativeFormat format,
        string? description = null,
        IReadOnlyList<ContextProperty>? attributes = null)
    {
        CreativeId = creativeId;
        Title = title;
        Description = description;
        MaterialUrl = materialUrl;
        ClickUrl = clickUrl;
        Width = width;
        Height = height;
        MimeType = mimeType;
        FileSize = fileSize;
        Format = format;
        Attributes = attributes?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();
    }

    /// <summary>
    /// 工厂方法：创建创意信息
    /// </summary>
    public static CreativeInfo Create(
        string creativeId,
        string title,
        string materialUrl,
        string clickUrl,
        int width,
        int height,
        string mimeType,
        long fileSize,
        CreativeFormat format = CreativeFormat.Banner,
        string? description = null,
        IReadOnlyList<ContextProperty>? attributes = null)
    {
        ValidateCreativeId(creativeId);
        ValidateTitle(title);
        ValidateUrl(materialUrl, nameof(materialUrl));
        ValidateUrl(clickUrl, nameof(clickUrl));
        ValidateDimensions(width, height);
        ValidateMimeType(mimeType);
        ValidateFileSize(fileSize);

        return new CreativeInfo(
            creativeId,
            title,
            materialUrl,
            clickUrl,
            width,
            height,
            mimeType,
            fileSize,
            format,
            description,
            attributes);
    }

    /// <summary>
    /// 创建图片创意
    /// </summary>
    public static CreativeInfo CreateImageBanner(
        string creativeId,
        string title,
        string imageUrl,
        string clickUrl,
        int width,
        int height,
        string? description = null)
    {
        return Create(
            creativeId,
            title,
            imageUrl,
            clickUrl,
            width,
            height,
            "image/jpeg",
            0,
            CreativeFormat.Banner,
            description);
    }

    /// <summary>
    /// 创建视频创意
    /// </summary>
    public static CreativeInfo CreateVideoAd(
        string creativeId,
        string title,
        string videoUrl,
        string clickUrl,
        int width,
        int height,
        long fileSize,
        string? description = null)
    {
        return Create(
            creativeId,
            title,
            videoUrl,
            clickUrl,
            width,
            height,
            "video/mp4",
            fileSize,
            CreativeFormat.Video,
            description);
    }

    /// <summary>
    /// 创建原生广告创意
    /// </summary>
    public static CreativeInfo CreateNativeAd(
        string creativeId,
        string title,
        string materialUrl,
        string clickUrl,
        string? description = null,
        IReadOnlyList<ContextProperty>? nativeAttributes = null)
    {
        return Create(
            creativeId,
            title,
            materialUrl,
            clickUrl,
            0,
            0,
            "text/html",
            0,
            CreativeFormat.Native,
            description,
            nativeAttributes);
    }

    /// <summary>
    /// 链式操作：更新描述
    /// </summary>
    public CreativeInfo WithDescription(string? description)
    {
        return new CreativeInfo(
            CreativeId,
            Title,
            MaterialUrl,
            ClickUrl,
            Width,
            Height,
            MimeType,
            FileSize,
            Format,
            description,
            Attributes);
    }

    /// <summary>
    /// 链式操作：更新扩展属性
    /// </summary>
    public CreativeInfo WithAttributes(IReadOnlyList<ContextProperty> attributes)
    {
        return new CreativeInfo(
            CreativeId,
            Title,
            MaterialUrl,
            ClickUrl,
            Width,
            Height,
            MimeType,
            FileSize,
            Format,
            Description,
            attributes);
    }

    /// <summary>
    /// 链式操作：添加单个属性
    /// </summary>
    public CreativeInfo WithAttribute(string key, object value)
    {
        var newAttributes = new List<ContextProperty>(Attributes);

        // 移除已存在的相同键的属性
        newAttributes.RemoveAll(attr => attr.PropertyKey == key);

        // 添加新属性
        newAttributes.Add(new ContextProperty(key, value?.ToString() ?? string.Empty));

        return WithAttributes(newAttributes.AsReadOnly());
    }

    /// <summary>
    /// 业务方法：是否为视频广告
    /// </summary>
    public bool IsVideoAd => Format == CreativeFormat.Video;

    /// <summary>
    /// 业务方法：是否为图片广告
    /// </summary>
    public bool IsImageAd => Format == CreativeFormat.Banner && MimeType.StartsWith("image/");

    /// <summary>
    /// 业务方法：是否为原生广告
    /// </summary>
    public bool IsNativeAd => Format == CreativeFormat.Native;

    /// <summary>
    /// 业务方法：是否为富媒体广告
    /// </summary>
    public bool IsRichMediaAd => Format == CreativeFormat.Expandable || Format == CreativeFormat.Interstitial;

    /// <summary>
    /// 业务方法：获取宽高比
    /// </summary>
    public double GetAspectRatio()
    {
        if (Height == 0) return 0;
        return (double)Width / Height;
    }

    /// <summary>
    /// 业务方法：是否为标准尺寸
    /// </summary>
    public bool IsStandardSize()
    {
        return (Width, Height) switch
        {
            (728, 90) => true,   // 排行榜
            (300, 250) => true,  // 中等矩形
            (320, 50) => true,   // 移动横幅
            (160, 600) => true,  // 宽摩天大楼
            (300, 600) => true,  // 半页广告
            (320, 480) => true,  // 移动插屏
            _ => false
        };
    }

    /// <summary>
    /// 业务方法：验证创意是否适用于指定设备
    /// </summary>
    public bool IsCompatibleWithDevice(DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.Smartphone => Width <= 375 && Height <= 667,
            DeviceType.Tablet => Width <= 768 && Height <= 1024,
            DeviceType.PersonalComputer => Width <= 1920 && Height <= 1080,
            _ => true
        };
    }

    /// <summary>
    /// 业务方法：获取创意描述信息
    /// </summary>
    public string GetDisplayInfo()
    {
        return $"{Title} ({Width}x{Height}, {Format})";
    }

    /// <summary>
    /// 获取指定键的属性值
    /// </summary>
    /// <param name="key">属性键</param>
    /// <returns>属性值，如果不存在则返回null</returns>
    public string? GetAttributeValue(string key)
    {
        return Attributes.FirstOrDefault(attr => attr.PropertyKey == key)?.PropertyValue;
    }

    /// <summary>
    /// 获取指定键的强类型属性值
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">属性键</param>
    /// <returns>转换后的属性值</returns>
    public T? GetAttributeValue<T>(string key) where T : class
    {
        var property = Attributes.FirstOrDefault(attr => attr.PropertyKey == key);
        return property?.GetValue<T>();
    }

    /// <summary>
    /// 检查是否包含指定键的属性
    /// </summary>
    /// <param name="key">属性键</param>
    /// <returns>是否包含</returns>
    public bool HasAttribute(string key)
    {
        return Attributes.Any(attr => attr.PropertyKey == key);
    }

    /// <summary>
    /// 获取所有属性键
    /// </summary>
    /// <returns>属性键列表</returns>
    public IEnumerable<string> GetAttributeKeys()
    {
        return Attributes.Select(attr => attr.PropertyKey);
    }

    #region 参数校验

    private static void ValidateCreativeId(string creativeId)
    {
        if (string.IsNullOrWhiteSpace(creativeId))
            throw new ArgumentException("创意ID不能为空", nameof(creativeId));

        if (creativeId.Length > 50)
            throw new ArgumentException("创意ID长度不能超过50个字符", nameof(creativeId));
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("创意标题不能为空", nameof(title));

        if (title.Length > 100)
            throw new ArgumentException("创意标题长度不能超过100个字符", nameof(title));
    }

    private static void ValidateUrl(string url, string paramName)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"{paramName}不能为空", paramName);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException($"{paramName}格式不正确", paramName);

        if (uri.Scheme != "http" && uri.Scheme != "https")
            throw new ArgumentException($"{paramName}必须是HTTP或HTTPS协议", paramName);
    }

    private static void ValidateDimensions(int width, int height)
    {
        if (width < 0)
            throw new ArgumentException("宽度不能为负数", nameof(width));

        if (height < 0)
            throw new ArgumentException("高度不能为负数", nameof(height));

        if (width == 0 && height == 0)
            return; // 原生广告可以没有固定尺寸

        if (width > 0 && height == 0)
            throw new ArgumentException("如果设置了宽度，高度不能为0", nameof(height));

        if (height > 0 && width == 0)
            throw new ArgumentException("如果设置了高度，宽度不能为0", nameof(width));
    }

    private static void ValidateMimeType(string mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("媒体类型不能为空", nameof(mimeType));

        if (!mimeType.Contains('/'))
            throw new ArgumentException("媒体类型格式不正确", nameof(mimeType));
    }

    private static void ValidateFileSize(long fileSize)
    {
        if (fileSize < 0)
            throw new ArgumentException("文件大小不能为负数", nameof(fileSize));

        if (fileSize > 50 * 1024 * 1024) // 50MB
            throw new ArgumentException("文件大小不能超过50MB", nameof(fileSize));
    }

    #endregion

    #region 等价性比较

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreativeId;
        yield return Title;
        yield return Description ?? string.Empty;
        yield return MaterialUrl;
        yield return ClickUrl;
        yield return Width;
        yield return Height;
        yield return MimeType;
        yield return FileSize;
        yield return Format;

        // 注意：对于ContextProperty集合，按键排序以确保一致的相等性比较
        foreach (var attr in Attributes.OrderBy(x => x.PropertyKey))
        {
            yield return attr.PropertyKey;
            yield return attr.PropertyValue;
        }
    }

    #endregion
}