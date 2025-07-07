using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
/// </summary>
public class CreativeInfo : ValueObject
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string CreativeId { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// �ز�URL
    /// </summary>
    public string MaterialUrl { get; private set; }

    /// <summary>
    /// �����תURL
    /// </summary>
    public string ClickUrl { get; private set; }

    /// <summary>
    /// ������
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// ����߶�
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// ý������
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// �ļ���С���ֽڣ�
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// �����ʽ
    /// </summary>
    public CreativeFormat Format { get; private set; }

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyDictionary<string, object> Attributes { get; private set; }

    /// <summary>
    /// ˽�й��캯��
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
        IReadOnlyDictionary<string, object>? attributes = null)
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
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// ��������������������Ϣ
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
        IReadOnlyDictionary<string, object>? attributes = null)
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
    /// ����ͼƬ����
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
    /// ������Ƶ����
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
    /// ����ԭ����洴��
    /// </summary>
    public static CreativeInfo CreateNativeAd(
        string creativeId,
        string title,
        string materialUrl,
        string clickUrl,
        string? description = null,
        IReadOnlyDictionary<string, object>? nativeAttributes = null)
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
    /// ��ʽ��������������
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
    /// ��ʽ������������չ����
    /// </summary>
    public CreativeInfo WithAttributes(IReadOnlyDictionary<string, object> attributes)
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
    /// ��ʽ��������ӵ�������
    /// </summary>
    public CreativeInfo WithAttribute(string key, object value)
    {
        var newAttributes = new Dictionary<string, object>(Attributes)
        {
            [key] = value
        };
        return WithAttributes(newAttributes);
    }

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��Ƶ���
    /// </summary>
    public bool IsVideoAd => Format == CreativeFormat.Video;

    /// <summary>
    /// ҵ�񷽷����Ƿ�ΪͼƬ���
    /// </summary>
    public bool IsImageAd => Format == CreativeFormat.Banner && MimeType.StartsWith("image/");

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊԭ�����
    /// </summary>
    public bool IsNativeAd => Format == CreativeFormat.Native;

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��ý����
    /// </summary>
    public bool IsRichMediaAd => Format == CreativeFormat.Expandable || Format == CreativeFormat.Interstitial;

    /// <summary>
    /// ҵ�񷽷�����ȡ��߱�
    /// </summary>
    public double GetAspectRatio()
    {
        if (Height == 0) return 0;
        return (double)Width / Height;
    }

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��׼�ߴ�
    /// </summary>
    public bool IsStandardSize()
    {
        return (Width, Height) switch
        {
            (728, 90) => true,   // ���а�
            (300, 250) => true,  // �еȾ���
            (320, 50) => true,   // �ƶ����
            (160, 600) => true,  // ��Ħ���¥
            (300, 600) => true,  // ��ҳ���
            (320, 480) => true,  // �ƶ�����
            _ => false
        };
    }

    /// <summary>
    /// ҵ�񷽷�����֤�����Ƿ�������ָ���豸
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
    /// ҵ�񷽷�����ȡ����������Ϣ
    /// </summary>
    public string GetDisplayInfo()
    {
        return $"{Title} ({Width}x{Height}, {Format})";
    }

    #region ����У��

    private static void ValidateCreativeId(string creativeId)
    {
        if (string.IsNullOrWhiteSpace(creativeId))
            throw new ArgumentException("����ID����Ϊ��", nameof(creativeId));

        if (creativeId.Length > 50)
            throw new ArgumentException("����ID���Ȳ��ܳ���50���ַ�", nameof(creativeId));
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("������ⲻ��Ϊ��", nameof(title));

        if (title.Length > 100)
            throw new ArgumentException("������ⳤ�Ȳ��ܳ���100���ַ�", nameof(title));
    }

    private static void ValidateUrl(string url, string paramName)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"{paramName}����Ϊ��", paramName);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException($"{paramName}��ʽ����ȷ", paramName);

        if (uri.Scheme != "http" && uri.Scheme != "https")
            throw new ArgumentException($"{paramName}������HTTP��HTTPSЭ��", paramName);
    }

    private static void ValidateDimensions(int width, int height)
    {
        if (width < 0)
            throw new ArgumentException("��Ȳ���Ϊ����", nameof(width));

        if (height < 0)
            throw new ArgumentException("�߶Ȳ���Ϊ����", nameof(height));

        if (width == 0 && height == 0)
            return; // ԭ��������û�й̶��ߴ�

        if (width > 0 && height == 0)
            throw new ArgumentException("��������˿�ȣ��߶Ȳ���Ϊ0", nameof(height));

        if (height > 0 && width == 0)
            throw new ArgumentException("��������˸߶ȣ���Ȳ���Ϊ0", nameof(width));
    }

    private static void ValidateMimeType(string mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("ý�����Ͳ���Ϊ��", nameof(mimeType));

        if (!mimeType.Contains('/'))
            throw new ArgumentException("ý�����͸�ʽ����ȷ", nameof(mimeType));
    }

    private static void ValidateFileSize(long fileSize)
    {
        if (fileSize < 0)
            throw new ArgumentException("�ļ���С����Ϊ����", nameof(fileSize));

        if (fileSize > 50 * 1024 * 1024) // 50MB
            throw new ArgumentException("�ļ���С���ܳ���50MB", nameof(fileSize));
    }

    #endregion

    #region �ȼ��ԱȽ�

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

        // ע�⣺�����ֵ����ͣ���Ҫ���⴦��
        foreach (var kvp in Attributes.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }

    #endregion
}