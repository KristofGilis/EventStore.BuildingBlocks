namespace EventStore.BuildingBlocks.Domain;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }

    Guid CreatedBy { get; }
    Guid UpdatedBy { get; }
}
