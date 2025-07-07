using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 黑名单状态值对象
/// </summary>
public record BlacklistStatus
{
    /// <summary>
    /// 是否在黑名单中
    /// </summary>
    public bool IsBlacklisted { get; init; }

    /// <summary>
    /// 黑名单类型
    /// </summary>
    public IReadOnlyList<BlacklistType> BlacklistTypes { get; init; } = Array.Empty<BlacklistType>();

    /// <summary>
    /// 黑名单原因
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// 加入黑名单时间
    /// </summary>
    public DateTime? BlacklistedAt { get; init; }

    /// <summary>
    /// 黑名单有效期
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// 是否永久黑名单
    /// </summary>
    public bool IsPermanent => !ExpiresAt.HasValue;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
}