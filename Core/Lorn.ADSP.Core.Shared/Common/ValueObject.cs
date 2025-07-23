namespace Lorn.ADSP.Core.Domain.Common;

/// <summary>
/// 值对象基类
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// 唯一标识符
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    /// <returns>用于相等性比较的组件集合</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// 相等性比较
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// 相等性操作符
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 不等性操作符
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// 创建值对象的副本
    /// </summary>
    public ValueObject Copy()
    {
        return (ValueObject)MemberwiseClone();
    }
}