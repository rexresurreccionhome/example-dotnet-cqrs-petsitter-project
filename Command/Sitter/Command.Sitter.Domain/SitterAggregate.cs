using Cqrs.Domain;
using Cqrs.Event.Sitter;

namespace Command.Sitter.Domain;

public class SitterAggregate : AggregateRoot
{
    public Guid SitterId { get; set; }
    public Guid MemberId { get; set; }
    public bool IsActive { get; set; }

    public SitterAggregate()
    {
    }

    public SitterAggregate(Guid sitterId, Guid memberId)
    {
        NewUncomittedEvent(
            new SitterCreatedEvent
            {
                Id = sitterId,
                MemberId = memberId,
                IsActive = true,
            }
        );
    }

    public void Apply(SitterCreatedEvent @event)
    {
        Id = @event.Id;
        SitterId = @event.Id;
        MemberId = @event.MemberId;
        IsActive = @event.IsActive;
    }

    public void UpdateSitter(bool isActive)
    {
        NewUncomittedEvent(
            new SitterUpdatedEvent
            {
                IsActive = isActive,
            }
        );
    }

    public void Apply(SitterUpdatedEvent @event)
    {
        IsActive = @event.IsActive;
    }
}