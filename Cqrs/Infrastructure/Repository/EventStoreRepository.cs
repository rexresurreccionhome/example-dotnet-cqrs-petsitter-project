using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Cqrs.Domain;
using Cqrs.Event;
using Cqrs.Infrastructure.Config;

namespace Cqrs.Infrastructure.Repository;


public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        MongoClient mongoClient = new(config.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task<List<EventModel>> FindAllAsync()
    {
        return await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(
            @event => @event.AggregateIdentifier == aggregateId
        ).ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }
}
