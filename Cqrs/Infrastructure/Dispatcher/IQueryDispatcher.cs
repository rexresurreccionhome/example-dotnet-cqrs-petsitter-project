using Cqrs.Query;

namespace Cqrs.Infrastructure;

public interface IQueryDispatcher<TEntity>
{
    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery;
    public Task<List<TEntity>> SendAsync(BaseQuery query);
}
