namespace EventStore.BuildingBlocks.Domain;
public interface IInternalEventHandler
{
    void Handle(IDomainEvent @event);
}
