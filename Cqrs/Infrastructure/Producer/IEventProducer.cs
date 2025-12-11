using Cqrs.Event;

namespace Cqrs.Infrastructure.Producer;

public interface IEventProducer
{
    public Task ProduceAsync<T>(string topicName, T @event) where T : BaseEvent;
}
