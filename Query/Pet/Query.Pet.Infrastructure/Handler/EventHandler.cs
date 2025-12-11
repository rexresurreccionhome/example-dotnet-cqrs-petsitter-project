using Cqrs.Event.Pet;
using Query.Pet.Domain.Entities;
using Query.Pet.Infrastructure.Repository;

namespace Query.Pet.Infrastructure.Handler;

public class EventHandler : IEventHandler
{
    private readonly IPetRepository _petRepository;

    public EventHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task On(PetCreatedEvent @event)
    {
        PetEntity petEntity = new()
        {
            PetId = @event.Id,
            MemberId = @event.MemberId,
            Name = @event.Name,
            IsActive = @event.IsActive,
        };

        await _petRepository.CreateAsync(petEntity);
    }

    public async Task On(PetUpdatedEvent @event)
    {
        PetEntity? petEntity = await _petRepository.GetByPetIdAsync(@event.Id);
        if (petEntity is not null)
        {
            petEntity.Name = @event.Name;
            await _petRepository.UpdateAsync(petEntity);
        }
    }

    public async Task On(PetDeletedEvent @event)
    {
        await _petRepository.DeleteAsync(@event.Id);
    }
}
