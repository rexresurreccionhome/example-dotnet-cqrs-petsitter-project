using Cqrs.Infrastructure.Handler;
using Cqrs.Command.Pet;
using Command.Pet.Domain;

namespace Command.Pet.Api.Command;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PetAggregate> _eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PetAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAsync(CreatePetCommand command)
    {
        PetAggregate aggregate = new(
            petId: command.PetId, memberId: command.MemberId, name: command.Name
        );
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(UpdatePetCommand command)
    {
        PetAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.PetId);
        aggregate.UpdatePet(command.Name);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePetCommand command)
    {
        PetAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.PetId);
        aggregate.DeletePet();
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RepublishPetCommand _)
    {
        await _eventSourcingHandler.RepublishEventsAsync();
    }
}
