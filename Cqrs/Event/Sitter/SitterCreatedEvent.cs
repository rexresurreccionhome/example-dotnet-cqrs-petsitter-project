using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cqrs.Event.Sitter;

public class SitterCreatedEvent : BaseEvent
{
    public SitterCreatedEvent() : base(nameof(SitterCreatedEvent))
    {
    }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid MemberId { get; set; }
    public required bool IsActive { get; set; }
}