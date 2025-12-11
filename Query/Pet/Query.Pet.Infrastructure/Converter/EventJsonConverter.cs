using System.Text.Json.Serialization;
using System.Text.Json;
using Cqrs.Event;
using Cqrs.Event.Pet;

namespace Query.Pet.Infrastructure.Converter;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }

    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}.");
        }
        if (!doc.RootElement.TryGetProperty("EventName", out var type))
        {
            throw new JsonException($"Failed to detect the Type discriminator property.");
        }

        string typeDiscriminator = type.GetString()!;
        string json = doc.RootElement.GetRawText();

        return typeDiscriminator switch
        {
            nameof(PetCreatedEvent) => JsonSerializer.Deserialize<PetCreatedEvent>(json, options),
            nameof(PetUpdatedEvent) => JsonSerializer.Deserialize<PetUpdatedEvent>(json, options),
            nameof(PetDeletedEvent) => JsonSerializer.Deserialize<PetDeletedEvent>(json, options),
            _ => throw new JsonException($"{typeDiscriminator} unsupported event."),
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
