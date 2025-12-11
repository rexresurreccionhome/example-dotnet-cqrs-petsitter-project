using Cqrs.Event;

namespace Cqrs.Infrastructure.Store;

public interface IEventStore
{
    public Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int version);
    public Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
    public Task<List<Guid>> GetAggregateIdsAsync();
}
