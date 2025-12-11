using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cqrs.Event;


public class BaseEvent
{
    protected BaseEvent(string eventName)
    {
        EventName = eventName;
    }

    public string EventName { get; set; }
    public int Version { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
}
