namespace EventStore.BuildingBlocks.Domain;
public abstract class Entity : IInternalEventHandler
{
    private readonly Action<IDomainEvent> _applier;

    protected Entity(Action<IDomainEvent> applier)
    {
        _applier = applier;
    }

    public void Handle(IDomainEvent @event) => When(@event);

    protected abstract void When(IDomainEvent @event);

    protected static void ApplyToEntity(IInternalEventHandler? entity, IDomainEvent @event) => entity?.Handle(@event);

    protected void Apply(IDomainEvent @event) => _applier(@event);
}