using System.Linq.Expressions;

namespace EventStore.BuildingBlocks.Domain;

public interface IRepository<TAggregateRoot>
{
    Task<TAggregateRoot> LoadAggregate(Guid id, CancellationToken cancellationToken);
    Task<TAggregateRoot?> FindAggregate(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken);
    void RemoveAggregate(TAggregateRoot aggregate);
    void AppendChanges(TAggregateRoot aggregate);
    Task Save(CancellationToken cancellationToken);
}
