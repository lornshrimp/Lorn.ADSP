using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Requests;

/// <summary>
/// 黑名单检查请求
/// </summary>
public record BlacklistCheckRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public string? DeviceId { get; init; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// 广告ID
    /// </summary>
    public string? AdId { get; init; }

    /// <summary>
    /// 广告主ID
    /// </summary>
    public string? AdvertiserId { get; init; }

    /// <summary>
    /// 检查类型
    /// </summary>
    public IReadOnlyList<BlacklistType> CheckTypes { get; init; } = Array.Empty<BlacklistType>();
}

/// <summary>
/// 频次控制请求
/// </summary>
public record FrequencyControlRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// 广告ID
    /// </summary>
    public required string AdId { get; init; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; init; } = DateTime.UtcNow;
}