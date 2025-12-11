using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cqrs.Event;


public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required DateTime TimeStamp { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid AggregateIdentifier { get; set; }
    public required string AggregateType { get; set; }
    public required int Version { get; set; }
    public required string EventType { get; set; }
    public required BaseEvent EventState { get; set; }
}
