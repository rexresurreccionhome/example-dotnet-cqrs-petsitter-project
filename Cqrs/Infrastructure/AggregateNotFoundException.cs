namespace Cqrs.Infrastructure;

public class AggregateNotFoundException: Exception
{
    public AggregateNotFoundException(string message): base(message)
    {
        
    }
}
