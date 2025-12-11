using Cqrs.Event.Sitter;
using Query.Sitter.Domain.Entities;
using Query.Sitter.Infrastructure.Repository;

namespace Query.Sitter.Infrastructure.Handler;

public class EventHandler : IEventHandler
{
    private readonly ISitterRepository _sitterRepository;

    public EventHandler(ISitterRepository sitterRepository)
    {
        _sitterRepository = sitterRepository;
    }

    public async Task On(SitterCreatedEvent @event)
    {
        SitterEntity sitterEntity = new()
        {
            SitterId = @event.Id,
            MemberId = @event.MemberId,
            IsActive = @event.IsActive,
        };

        await _sitterRepository.CreateAsync(sitterEntity);
    }

    public async Task On(SitterUpdatedEvent @event)
    {
        SitterEntity? sitterEntity = await _sitterRepository.GetByIdAsync(@event.Id);
        if (sitterEntity is not null)
        {
            sitterEntity.IsActive = @event.IsActive;
            await _sitterRepository.UpdateAsync(sitterEntity);
        }
    }
}
