namespace Cqrs.Command.Sitter;

public class CreateSitterCommand : BaseCommand
{
    public Guid SitterId { get; set; } = Guid.NewGuid();
    public required Guid MemberId { get; set; }
}