using System.Text.Json;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Cqrs.Event;

namespace Cqrs.Infrastructure.Producer;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _producerConfig;

    public EventProducer(IOptions<ProducerConfig> producerConfig)
    {
        _producerConfig = producerConfig.Value;
    }
    
    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using IProducer<string, string> producer = new ProducerBuilder<string, string>(_producerConfig)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

        Message<string, string> message = new()
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        DeliveryResult<string, string> deliveryResult = await producer.ProduceAsync(topic, message);
        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"Failed to produce {@event.GetType().Name} message to topic {topic}: {deliveryResult.Message}.");
        }
    }
}
