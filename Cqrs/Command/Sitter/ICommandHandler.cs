namespace Cqrs.Command.Sitter;

public interface ICommandHandler
{
    Task HandleAsync(CreateSitterCommand command);
    Task HandleAsync(UpdateSitterCommand command);
}