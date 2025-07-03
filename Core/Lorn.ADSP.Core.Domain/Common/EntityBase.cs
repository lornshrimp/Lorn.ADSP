using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.Common;

/// <summary>
/// ʵ�����
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// ʵ��Ψһ��ʶ
    /// </summary>
    public string Id { get; protected set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreateTime { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// ����޸�ʱ��
    /// </summary>
    public DateTime LastModifiedTime { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// �Ƿ���ɾ������ɾ����
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// �汾���������ֹ�����
    /// </summary>
    public long Version { get; protected set; } = 1;

    /// <summary>
    /// ��������޸�ʱ��
    /// </summary>
    protected virtual void UpdateLastModifiedTime()
    {
        LastModifiedTime = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// ��ɾ��
    /// </summary>
    public virtual void Delete()
    {
        IsDeleted = true;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �ָ�ɾ��
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ����ԱȽ�
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other || GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// ��ȡ��ϣ��
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// ����Բ�����
    /// </summary>
    public static bool operator ==(EntityBase? left, EntityBase? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// �����Բ�����
    /// </summary>
    public static bool operator !=(EntityBase? left, EntityBase? right)
    {
        return !Equals(left, right);
    }
}