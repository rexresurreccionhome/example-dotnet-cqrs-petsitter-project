namespace Cqrs.Command.Pet;

public class DeletePetCommand: BaseCommand
{
    public required Guid PetId { get; set; }
}
