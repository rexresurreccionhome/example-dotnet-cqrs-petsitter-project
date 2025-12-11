namespace Cqrs.Event.Pet;

public class PetDeletedEvent: BaseEvent
{
    public PetDeletedEvent() : base(nameof(PetDeletedEvent))
    {

    }
}
