using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EventStore.BuildingBlocks.Domain;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<IIntegrationEvent> _integrationEvents = new();
    private readonly List<ValidationException> _validationExceptions = new();
    private readonly IDictionary<Type, List<Action<IDomainEvent>>> _eventHandlers = new Dictionary<Type, List<Action<IDomainEvent>>>();

    protected AggregateRoot() { }

    public Guid Id { get; protected set; } = default!;

    [IgnoreDataMember]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    [IgnoreDataMember]
    public IReadOnlyCollection<IIntegrationEvent> IntegrationEvents => _integrationEvents.AsReadOnly();

    [IgnoreDataMember, NotMapped]
    public IReadOnlyCollection<ValidationException> ValidationExceptions => _validationExceptions.AsReadOnly();

    protected void ApplyToEntity(IInternalEventHandler? entity, IDomainEvent @event) => entity?.Handle(@event);

    protected void On<TEvent>(Action<TEvent> handler) where TEvent : IDomainEvent
    {
        void ActionToRegister(IDomainEvent domainEvent)
        {
            var typedDomainEvent = (TEvent)domainEvent;
            handler(typedDomainEvent);
        }

        if (_eventHandlers.TryGetValue(typeof(TEvent), out var value))
        {
            value.Add(ActionToRegister);
        }
        else
        {
            _eventHandlers.Add(typeof(TEvent), new List<Action<IDomainEvent>> { ActionToRegister });
        }
    }

    protected void Handle<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
    {
        _domainEvents.Add(@event);

        var eventType = @event.GetType();
        if (!_eventHandlers.ContainsKey(eventType))
            throw new InvalidOperationException($"There are no actions registered to handle {eventType}");

        foreach (var action in _eventHandlers[eventType])
            action(@event);
    }

    protected void AddIntegrationEvent(IIntegrationEvent eventItem)
    {
        _integrationEvents.Add(eventItem);
    }

    protected void AddValidationException(ValidationException validationException, bool throwDirectly = false)
    {
        if (throwDirectly)
            throw validationException;

        _validationExceptions.Add(validationException);
    }

    protected void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void ClearIntegrationEvents()
    {
        _integrationEvents.Clear();
    }

    protected void ClearValidationExceptions()
    {
        _validationExceptions.Clear();
    }

    protected void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    protected void RemoveIntegrationEvent(IIntegrationEvent eventItem)
    {
        _integrationEvents.Remove(eventItem);
    }

    protected void RemoveValidationException(ValidationException validationException)
    {
        _validationExceptions.Remove(validationException);
    }
}
