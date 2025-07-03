namespace Lorn.ADSP.Core.Domain.Common;

/// <summary>
/// 聚合根基类
/// </summary>
public abstract class AggregateRoot : EntityBase
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// 获取领域事件列表
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="eventItem">领域事件</param>
    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    /// <summary>
    /// 移除领域事件
    /// </summary>
    /// <param name="eventItem">领域事件</param>
    protected void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    /// <summary>
    /// 清空领域事件
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// 事件发生时间
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// 事件类型
    /// </summary>
    string EventType { get; }
}