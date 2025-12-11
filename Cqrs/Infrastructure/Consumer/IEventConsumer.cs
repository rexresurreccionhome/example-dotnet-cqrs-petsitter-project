namespace Cqrs.Infrastructure.Consumer;

public interface IEventConsumer
{
    public void Consume(string topic);
}
