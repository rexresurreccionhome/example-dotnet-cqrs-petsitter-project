using Cqrs.Infrastructure;
using Cqrs.Query;
using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Infrastructure.Dispatcher;

public class QueryDispatcher : IQueryDispatcher<SitterEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<SitterEntity>>>> _handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<SitterEntity>>> handler) where TQuery : BaseQuery
    {
        if (_handlers.ContainsKey(typeof(TQuery)))
        {
            throw new ArgumentException("Handler already exists.");
        }
        _handlers.Add(typeof(TQuery), query => handler((TQuery)query));
    }

    public async Task<List<SitterEntity>> SendAsync(BaseQuery query)
    {
        if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<SitterEntity>>>? handler))
        {
            return await handler(query);
        }

        throw new ArgumentNullException(nameof(query), "No query handler was registered.");
    }
}
