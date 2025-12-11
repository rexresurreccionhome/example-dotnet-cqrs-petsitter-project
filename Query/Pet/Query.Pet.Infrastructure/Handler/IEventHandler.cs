using System;
using Cqrs.Event.Pet;

namespace Query.Pet.Infrastructure.Handler;

public interface IEventHandler
{
    public Task On(PetCreatedEvent @event);
    public Task On(PetUpdatedEvent @event);
    public Task On(PetDeletedEvent @event);
}
