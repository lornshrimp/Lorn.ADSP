using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 黑名单状态值对象
/// </summary>
public class BlacklistStatus : ValueObject
{
    /// <summary>
    /// 是否在黑名单中
    /// </summary>
    public bool IsBlacklisted { get; private set; }

    /// <summary>
    /// 黑名单类型
    /// </summary>
    public IReadOnlyList<BlacklistType> BlacklistTypes { get; private set; }

    /// <summary>
    /// 黑名单原因
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// 加入黑名单时间
    /// </summary>
    public DateTime? BlacklistedAt { get; private set; }

    /// <summary>
    /// 黑名单有效期
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// 是否永久黑名单
    /// </summary>
    public bool IsPermanent => !ExpiresAt.HasValue;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// 是否当前有效
    /// </summary>
    public bool IsCurrentlyActive => IsBlacklisted && !IsExpired;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private BlacklistStatus()
    {
        IsBlacklisted = false;
        BlacklistTypes = Array.Empty<BlacklistType>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BlacklistStatus(
        bool isBlacklisted,
        IReadOnlyList<BlacklistType>? blacklistTypes = null,
        string? reason = null,
        DateTime? blacklistedAt = null,
        DateTime? expiresAt = null)
    {
        ValidateInput(isBlacklisted, blacklistTypes, blacklistedAt, expiresAt);

        IsBlacklisted = isBlacklisted;
        BlacklistTypes = blacklistTypes ?? Array.Empty<BlacklistType>();
        Reason = reason;
        BlacklistedAt = blacklistedAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// 创建正常状态（非黑名单）
    /// </summary>
    public static BlacklistStatus CreateNormal()
    {
        return new BlacklistStatus(false);
    }

    /// <summary>
    /// 创建黑名单状态
    /// </summary>
    public static BlacklistStatus CreateBlacklisted(
        BlacklistType blacklistType,
        string? reason = null,
        DateTime? expiresAt = null)
    {
        return new BlacklistStatus(
            true,
            new[] { blacklistType },
            reason,
            DateTime.UtcNow,
            expiresAt);
    }

    /// <summary>
    /// 创建多类型黑名单状态
    /// </summary>
    public static BlacklistStatus CreateBlacklisted(
        IReadOnlyList<BlacklistType> blacklistTypes,
        string? reason = null,
        DateTime? expiresAt = null)
    {
        return new BlacklistStatus(
            true,
            blacklistTypes,
            reason,
            DateTime.UtcNow,
            expiresAt);
    }

    /// <summary>
    /// 创建永久黑名单状态
    /// </summary>
    public static BlacklistStatus CreatePermanentBlacklist(
        BlacklistType blacklistType,
        string? reason = null)
    {
        return new BlacklistStatus(
            true,
            new[] { blacklistType },
            reason,
            DateTime.UtcNow,
            null);
    }

    /// <summary>
    /// 添加黑名单类型
    /// </summary>
    public BlacklistStatus AddBlacklistType(BlacklistType blacklistType)
    {
        if (BlacklistTypes.Contains(blacklistType))
            return this;

        var newTypes = BlacklistTypes.Concat(new[] { blacklistType }).ToArray();
        return new BlacklistStatus(
            true,
            newTypes,
            Reason,
            BlacklistedAt ?? DateTime.UtcNow,
            ExpiresAt);
    }

    /// <summary>
    /// 移除黑名单类型
    /// </summary>
    public BlacklistStatus RemoveBlacklistType(BlacklistType blacklistType)
    {
        if (!BlacklistTypes.Contains(blacklistType))
            return this;

        var newTypes = BlacklistTypes.Where(t => t != blacklistType).ToArray();
        var stillBlacklisted = newTypes.Length > 0;

        return new BlacklistStatus(
            stillBlacklisted,
            newTypes,
            stillBlacklisted ? Reason : null,
            stillBlacklisted ? BlacklistedAt : null,
            stillBlacklisted ? ExpiresAt : null);
    }

    /// <summary>
    /// 更新黑名单原因
    /// </summary>
    public BlacklistStatus WithReason(string? reason)
    {
        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            reason,
            BlacklistedAt,
            ExpiresAt);
    }

    /// <summary>
    /// 延长黑名单有效期
    /// </summary>
    public BlacklistStatus ExtendExpiration(DateTime newExpiresAt)
    {
        if (!IsBlacklisted)
            throw new InvalidOperationException("无法延长非黑名单状态的有效期");

        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("新的有效期必须在当前时间之后", nameof(newExpiresAt));

        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            Reason,
            BlacklistedAt,
            newExpiresAt);
    }

    /// <summary>
    /// 设置为永久黑名单
    /// </summary>
    public BlacklistStatus MakePermanent()
    {
        if (!IsBlacklisted)
            throw new InvalidOperationException("无法将非黑名单状态设置为永久");

        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            Reason,
            BlacklistedAt,
            null);
    }

    /// <summary>
    /// 清除黑名单状态
    /// </summary>
    public BlacklistStatus Clear()
    {
        return CreateNormal();
    }

    /// <summary>
    /// 检查是否包含特定黑名单类型
    /// </summary>
    public bool ContainsType(BlacklistType blacklistType)
    {
        return BlacklistTypes.Contains(blacklistType);
    }

    /// <summary>
    /// 检查是否包含任一黑名单类型
    /// </summary>
    public bool ContainsAnyType(params BlacklistType[] blacklistTypes)
    {
        return blacklistTypes.Any(type => BlacklistTypes.Contains(type));
    }

    /// <summary>
    /// 获取剩余有效时间
    /// </summary>
    public TimeSpan? GetRemainingTime()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var remaining = ExpiresAt.Value - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// 获取等价性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsBlacklisted;
        yield return Reason ?? string.Empty;
        yield return BlacklistedAt ?? DateTime.MinValue;
        yield return ExpiresAt ?? DateTime.MinValue;

        foreach (var blacklistType in BlacklistTypes.OrderBy(x => x))
        {
            yield return blacklistType;
        }
    }

    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(
        bool isBlacklisted,
        IReadOnlyList<BlacklistType>? blacklistTypes,
        DateTime? blacklistedAt,
        DateTime? expiresAt)
    {
        if (isBlacklisted && (blacklistTypes == null || blacklistTypes.Count == 0))
            throw new ArgumentException("黑名单状态必须包含至少一个黑名单类型", nameof(blacklistTypes));

        if (!isBlacklisted && blacklistTypes != null && blacklistTypes.Count > 0)
            throw new ArgumentException("非黑名单状态不能包含黑名单类型", nameof(blacklistTypes));

        if (isBlacklisted && blacklistedAt.HasValue && blacklistedAt.Value > DateTime.UtcNow)
            throw new ArgumentException("黑名单加入时间不能是未来时间", nameof(blacklistedAt));

        if (expiresAt.HasValue && blacklistedAt.HasValue && expiresAt.Value <= blacklistedAt.Value)
            throw new ArgumentException("黑名单有效期必须在加入时间之后", nameof(expiresAt));
    }
}