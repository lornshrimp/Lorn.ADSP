using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Common;

/// <summary>
/// 实体基类
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    public string Id { get; protected set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime LastModifiedTime { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否已删除（软删除）
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// 版本戳（用于乐观锁）
    /// </summary>
    public long Version { get; protected set; } = 1;

    /// <summary>
    /// 更新最后修改时间
    /// </summary>
    protected virtual void UpdateLastModifiedTime()
    {
        LastModifiedTime = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// 软删除
    /// </summary>
    public virtual void Delete()
    {
        IsDeleted = true;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 恢复删除
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 相等性比较
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other || GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// 相等性操作符
    /// </summary>
    public static bool operator ==(EntityBase? left, EntityBase? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 不等性操作符
    /// </summary>
    public static bool operator !=(EntityBase? left, EntityBase? right)
    {
        return !Equals(left, right);
    }
}