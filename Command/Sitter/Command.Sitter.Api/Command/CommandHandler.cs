using Cqrs.Infrastructure.Handler;
using Cqrs.Command.Sitter;
using Command.Sitter.Domain;

namespace Command.Sitter.Api.Command;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<SitterAggregate> _eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<SitterAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAsync(CreateSitterCommand command)
    {
        SitterAggregate aggregate = new(
            sitterId: command.SitterId,
            memberId: command.MemberId
        );
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(UpdateSitterCommand command)
    {
        SitterAggregate aggregate = await _eventSourcingHandler.GetByIdAsync(command.SitterId);
        aggregate.UpdateSitter(command.IsActive);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }
}