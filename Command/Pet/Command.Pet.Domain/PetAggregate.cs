using Cqrs.Domain;
using Cqrs.Event.Pet;

namespace Command.Pet.Domain;

public class PetAggregate : AggregateRoot
{
    public Guid PetId { get; set; }
    public Guid MemberId { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }

    public PetAggregate()
    {
        
    }

    public PetAggregate(Guid petId, Guid memberId, string name)
    {
        NewUncomittedEvent(
            new PetCreatedEvent
            {
                Id = petId,
                Name = name,
                MemberId = memberId,
                IsActive = true,
            }
        );
    }

    public void Apply(PetCreatedEvent @event)
    {
        Id = @event.Id;
        PetId = @event.Id;
        MemberId = @event.MemberId;
        Name = @event.Name;
        IsActive = @event.IsActive;
    }

    public void UpdatePet(string name)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Unable to update in-active Pet profile");
        }

        NewUncomittedEvent(
            new PetUpdatedEvent
            {
                Id = PetId,
                Name = name,
            }
        );
    }

    public void Apply(PetUpdatedEvent @event)
    {
        Name = @event.Name;
    }

    public void DeletePet()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Pet profile was already deleted");
        }

        NewUncomittedEvent(
            new PetDeletedEvent
            {
                Id = PetId,
            }
        );
    }

    public void Apply(PetDeletedEvent @event)
    {
        IsActive = false;
    }
}
