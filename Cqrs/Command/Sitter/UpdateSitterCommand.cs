namespace Cqrs.Command.Sitter;

public class UpdateSitterCommand : BaseCommand
{
    public required Guid SitterId { get; set; }
    public required bool IsActive { get; set; }
}