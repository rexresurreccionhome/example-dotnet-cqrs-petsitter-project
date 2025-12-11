namespace Cqrs.Command.Pet;

public interface ICommandHandler
{
    public Task HandleAsync(CreatePetCommand command);
    public Task HandleAsync(UpdatePetCommand command);
    public Task HandleAsync(DeletePetCommand command);
    public Task HandleAsync(RepublishPetCommand _);
}
