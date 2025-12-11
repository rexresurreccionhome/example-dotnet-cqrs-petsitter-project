using System;
using Cqrs.Event;

namespace Cqrs.Infrastructure.Repository;

public interface IEventStoreRepository
{
    public Task SaveAsync(EventModel @event);
    public Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
    public Task<List<EventModel>> FindAllAsync();
}
