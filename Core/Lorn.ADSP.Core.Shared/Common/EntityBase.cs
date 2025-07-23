using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Common;

/// <summary>
/// 实体基类
/// 所有领域实体的基础类，提供标识、时间戳、软删除等通用功能
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// 实体唯一标识 - 使用Guid支持高并发场景
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// 更新者
    /// </summary>
    public string UpdatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// 是否已删除（软删除）
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// 更新最后修改时间
    /// </summary>
    protected virtual void UpdateLastModifiedTime()
    {
        UpdatedAt = DateTime.UtcNow;
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
    /// 实体验证
    /// </summary>
    /// <returns>验证是否通过</returns>
    public virtual bool ValidateEntity()
    {
        return Id != Guid.Empty;
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