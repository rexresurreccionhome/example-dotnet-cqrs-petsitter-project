namespace Cqrs.Command.Pet;

public class CreatePetCommand : BaseCommand
{
    public Guid PetId { get; set; } = Guid.NewGuid();
    public required Guid MemberId { get; set; }
    public required string Name { get; set; }
}
