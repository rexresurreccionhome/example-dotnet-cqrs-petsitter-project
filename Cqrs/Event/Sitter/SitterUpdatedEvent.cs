using System;

namespace Cqrs.Event.Sitter;

public class SitterUpdatedEvent : BaseEvent
{
    public SitterUpdatedEvent() : base(nameof(SitterUpdatedEvent))
    {
    }

    public required bool IsActive { get; set; }
}