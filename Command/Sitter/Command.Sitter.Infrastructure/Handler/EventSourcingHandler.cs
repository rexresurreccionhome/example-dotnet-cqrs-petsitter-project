using System.Data;
using Cqrs.Domain;
using Cqrs.Event;
using Cqrs.Infrastructure.Store;
using Cqrs.Infrastructure.Producer;
using Cqrs.Infrastructure.Handler;
using Command.Sitter.Domain;

namespace Command.Sitter.Infrastructure.Handler;

public class EventSourcingHandler : IEventSourcingHandler<SitterAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }

    public async Task<SitterAggregate> GetByIdAsync(Guid id)
    {
        SitterAggregate aggregate = new();
        List<BaseEvent> events = await _eventStore.GetEventsAsync(id);
        if (events is null || events.Count == 0)
        {
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(@event => @event.Version).Max();

        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        List<Guid> aggregateIdentifiers = await _eventStore.GetAggregateIdsAsync();

        if (aggregateIdentifiers is null || aggregateIdentifiers.Count == 0) return;

        string? topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_SITTER_EVENTS");

        if (String.IsNullOrEmpty(topic))
        {
            throw new NoNullAllowedException("Environment variable KAFKA_TOPIC_PETSITTER_SITTER_EVENTS is empty.");
        }
        
        foreach (Guid aggregateId in aggregateIdentifiers)
        {
            SitterAggregate? aggregate = await GetByIdAsync(aggregateId);
            if (aggregate is null || !aggregate.IsActive) continue;

            List<BaseEvent> events = await _eventStore.GetEventsAsync(aggregate.Id);
            foreach (BaseEvent @event in events)
            {
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(
            aggregate.Id,
            aggregate.GetUncommittedChanges(),
            aggregate.Version
        );
        aggregate.MarkAsCommitted();
    }
}