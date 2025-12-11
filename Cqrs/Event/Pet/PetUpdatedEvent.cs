namespace Cqrs.Event.Pet;

public class PetUpdatedEvent: BaseEvent
{
    public PetUpdatedEvent() : base(nameof(PetUpdatedEvent))
    {

    }

    public required string Name { get; set; }
}
