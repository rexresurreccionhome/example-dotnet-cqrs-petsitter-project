using Cqrs.Command;

namespace Cqrs.Infrastructure.Dispatcher;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new ArgumentException("Handler already exists");
        }
        // T is the Concrete Command while command is the BaseCommand casting it to be the Concrete Command type.
        _handlers.Add(typeof(T), command => handler((T)command));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task>? handler))
        {
            await handler(command);
        }
        else
        {
            throw new ArgumentNullException(nameof(command), "Handler was not registered");   
        }
    }
}
