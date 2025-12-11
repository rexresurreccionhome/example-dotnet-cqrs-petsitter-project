namespace Cqrs.Command.Pet;

public class UpdatePetCommand: BaseCommand
{
    public required Guid PetId { get; set; }
    public required string Name { get; set; }
}
