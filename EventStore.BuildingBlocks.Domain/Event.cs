namespace EventStore.BuildingBlocks.Domain
{
    public class Event
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid StreamId { get; private set; }
        public string Data { get; private set; } = default!;
        public string Type { get; private set; } = default!;
        public DateTime Timestamp { get; private set; }
        public string DotNetType { get; private set; } = default!;

        public Event(Guid streamId, string data, string type, DateTime timestamp, string dotNetType)
        {
            StreamId = streamId;
            Data = data;
            Type = type;
            Timestamp = timestamp;
            DotNetType = dotNetType;
        }
    }
}
