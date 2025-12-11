using Cqrs.Infrastructure;
using Cqrs.Query;
using Query.Pet.Domain.Entities;

namespace Query.Pet.Infrastructure.Dispatcher;

public class QueryDispatcher : IQueryDispatcher<PetEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PetEntity>>>> _handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PetEntity>>> handler) where TQuery : BaseQuery
    {
        if (_handlers.ContainsKey(typeof(TQuery)))
        {
            throw new ArgumentException("Handler already exists.");
        }
        _handlers.Add(typeof(TQuery), query => handler((TQuery)query));
    }

    public async Task<List<PetEntity>> SendAsync(BaseQuery query)
    {
        if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<PetEntity>>>? handler))
        {
            return await handler(query);
        }

        throw new ArgumentNullException(nameof(query), "No query handler was registered.");
    }
}
