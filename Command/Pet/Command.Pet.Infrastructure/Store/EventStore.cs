using System.Data;
using Cqrs.Event;
using Cqrs.Infrastructure;
using Cqrs.Infrastructure.Store;
using Cqrs.Infrastructure.Repository;
using Cqrs.Infrastructure.Producer;
using Command.Pet.Domain;

namespace Command.Pet.Infrastructure.Store;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        List<EventModel> eventStream = await _eventStoreRepository.FindAllAsync();
        if (eventStream is null || eventStream.Count == 0)
        {
            throw new ArgumentNullException(nameof(eventStream), "Event store repository returned empty.");
        }
        return eventStream.Select(@event => @event.AggregateIdentifier).Distinct().ToList();
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        List<EventModel> eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream.Count == 0)
        {
            throw new AggregateNotFoundException("Invalid aggregateId");
        }

        return eventStream.OrderBy(@event => @event.Version)
                .Select(@event => @event.EventState).ToList();
    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        List<EventModel> eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion != 0 && eventStream[^1].Version != expectedVersion)
        {
            throw new ConcurrencyException("expectedVersion did not match last record");
        }

        int version = expectedVersion;
        foreach (BaseEvent @event in events)
        {
            version++;
            @event.Version = version;
            string eventType = @event.GetType().Name;
            EventModel eventModel = new()
            {
                TimeStamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PetAggregate),
                EventType = eventType,
                EventState = @event,
                Version = version,
            };

            await _eventStoreRepository.SaveAsync(eventModel);

            string? topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_PET_EVENTS");
            if (String.IsNullOrEmpty(topic))
            {
                throw new NoNullAllowedException("Environment variable KAFKA_TOPIC_PETSITTER_PET_EVENTS is empty.");
            }
            await _eventProducer.ProduceAsync(topic, @event);
        }
    }
}
