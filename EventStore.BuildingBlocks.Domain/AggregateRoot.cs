using System.Runtime.Serialization;

namespace EventStore.BuildingBlocks.Domain;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    private readonly List<IIntegrationEvent> _integrationEvents = new List<IIntegrationEvent>();

    protected AggregateRoot()
    {
    }
    protected AggregateRoot(Guid id) => Id = id;

    public Guid Id { get; protected set; }

    [IgnoreDataMember]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    [IgnoreDataMember]
    public IReadOnlyCollection<IIntegrationEvent> IntegrationEvents => _integrationEvents.AsReadOnly();

    protected virtual void ApplyAndAddEvent(IDomainEvent @event) { }

    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    protected void AddIntegrationEvent(IIntegrationEvent eventItem)
    {
        _integrationEvents.Add(eventItem);
    }

    protected void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void ClearIntegrationEvents()
    {
        _integrationEvents.Clear();
    }

    protected void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    protected void RemoveIntegrationEvent(IIntegrationEvent eventItem)
    {
        _integrationEvents.Remove(eventItem);
    }
}
