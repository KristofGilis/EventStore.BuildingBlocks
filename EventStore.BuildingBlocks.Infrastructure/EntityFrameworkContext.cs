using EventStore.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventStore.BuildingBlocks.Infrastructure;

public class EntityFrameworkContext<TContext> :
    DbContext, IContext where TContext : DbContext
{
    protected EntityFrameworkContext(DbContextOptions options) : base(options) { }

    public DbSet<Event> Events { get; set; } = default!;
}


public interface IContext
{
    public DbSet<Event> Events { get; set; }
}