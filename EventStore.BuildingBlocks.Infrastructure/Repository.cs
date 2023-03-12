using EventStore.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace EventStore.BuildingBlocks.Infrastructure;

public sealed class Repository<TAggregateRoot, IDbContext> : IRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
    where IDbContext : DbContext, IContext
{
    private readonly IDbContext _context;

    public Repository(IDbContextFactory<IDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public async Task<TAggregateRoot> LoadAggregate(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<TAggregateRoot>().FirstAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TAggregateRoot?> FindAggregate(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Set<TAggregateRoot>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public void RemoveAggregate(TAggregateRoot aggregate)
    {
        _context.Remove(aggregate);
    }

    public void AppendChanges(TAggregateRoot aggregate)
    {
        if (aggregate.ValidationExceptions.Any())
            throw new AggregateException(aggregate.ValidationExceptions);

        foreach (var e in aggregate.DomainEvents)
        {
            var eventName = string.Concat(e.GetType().Name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString().ToLower()));
            _context.Events.Add(new Event(aggregate.Id, JsonConvert.SerializeObject(e), eventName, DateTime.UtcNow, e.GetType().FullName!));
        }

        if (_context.Set<TAggregateRoot>().FirstOrDefault(x => x.Id == aggregate.Id) == null)
            _context.Set<TAggregateRoot>().Add(aggregate);
        else
            _context.Set<TAggregateRoot>().Update(aggregate);
    }

    public Task Save(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
