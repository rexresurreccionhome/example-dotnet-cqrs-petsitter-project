using Cqrs.Domain;

namespace Cqrs.Infrastructure.Handler;

public interface IEventSourcingHandler<T>
{
    public Task SaveAsync(AggregateRoot aggregate);
    public Task<T> GetByIdAsync(Guid id);
    public Task RepublishEventsAsync();
}
