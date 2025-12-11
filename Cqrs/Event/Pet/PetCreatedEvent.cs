using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cqrs.Event.Pet;

public class PetCreatedEvent : BaseEvent
{
    public PetCreatedEvent() : base(nameof(PetCreatedEvent))
    {

    }

    public required string Name { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid MemberId { get; set; }
    public required bool IsActive {get; set;}
}
