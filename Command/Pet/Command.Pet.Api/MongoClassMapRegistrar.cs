using MongoDB.Bson.Serialization;
using Cqrs.Event;
using Cqrs.Event.Pet;

namespace Command.Pet.Api;

public static class MongoClassMapRegistrar
{
    public static void Register()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEvent)))
            BsonClassMap.RegisterClassMap<BaseEvent>();
        if (!BsonClassMap.IsClassMapRegistered(typeof(PetCreatedEvent)))
            BsonClassMap.RegisterClassMap<PetCreatedEvent>();
        if (!BsonClassMap.IsClassMapRegistered(typeof(PetUpdatedEvent)))
            BsonClassMap.RegisterClassMap<PetUpdatedEvent>();
        if (!BsonClassMap.IsClassMapRegistered(typeof(PetDeletedEvent)))
            BsonClassMap.RegisterClassMap<PetDeletedEvent>();
    }
}