using System;
using Cqrs.Event.Sitter;

namespace Query.Sitter.Infrastructure.Handler;

public interface IEventHandler
{
    public Task On(SitterCreatedEvent @event);
    public Task On(SitterUpdatedEvent @event);
}
