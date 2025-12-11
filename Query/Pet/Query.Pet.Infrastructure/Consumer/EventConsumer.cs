using System.Text.Json;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Cqrs.Infrastructure.Consumer;
using Cqrs.Event;
using Query.Pet.Infrastructure.Converter;
using Query.Pet.Infrastructure.Handler;

namespace Query.Pet.Infrastructure.Consumer;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly IEventHandler _eventHandler;

    public EventConsumer(IOptions<ConsumerConfig> consumerConfig, IEventHandler eventHandler)
    {
        _consumerConfig = consumerConfig.Value;
        _eventHandler = eventHandler;
    }

    public void Consume(string topic)
    {
        using IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message is null) continue;
            
            JsonSerializerOptions options = new()
            {
                Converters = { new EventJsonConverter() }
            };
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options)!;

            var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
            if (handlerMethod is null)
            {
                throw new ArgumentException($"{nameof(handlerMethod)} consumer handler method not found.");

            }

            handlerMethod.Invoke(_eventHandler, new object[] { @event });
            consumer.Commit(consumerResult);
        }
    }
}
