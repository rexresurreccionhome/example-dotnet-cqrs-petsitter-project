# Pet Command Service

This service implements the Command side of the CQRS pattern for Pet aggregate management. It handles all write operations (create, update, delete) for Pet entities and publishes events to maintain eventual consistency with the Query side.

## Architecture Overview

The Pet Command service follows Domain-Driven Design (DDD) principles with Event Sourcing and is organized into three main layers:

- **Command.Pet.Api** - Web API layer with controllers and dependency injection setup
- **Command.Pet.Domain** - Domain layer containing aggregates, entities, and domain logic
- **Command.Pet.Infrastructure** - Infrastructure layer with concrete implementations for data persistence and event handling

## Key Components

### CommandDispatcher
**Location**: `Cqrs/Infrastructure/Dispatcher/CommandDispatcher.cs`

The CommandDispatcher is responsible for registering and routing commands to their appropriate handlers. It maintains a dictionary of command types and their corresponding handler functions.

**Key Features**:
- Generic handler registration with `RegisterHandler<T>(Func<T, Task> handler)`
- Type-safe command routing via `SendAsync(BaseCommand command)`
- Runtime validation to ensure handlers are registered before use
- Shared across all command services in the CQRS infrastructure

**Registration Example** (from Program.cs):
```csharp
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>(options =>
{
    ICommandHandler commandHandler = options.GetRequiredService<ICommandHandler>();
    CommandDispatcher dispatcher = new();
    dispatcher.RegisterHandler<CreatePetCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<UpdatePetCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<DeletePetCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RepublishPetCommand>(commandHandler.HandleAsync);
    return dispatcher;
});
```

### CommandHandler
**Location**: `Command/Pet/Command.Pet.Api/Command/CommandHandler.cs`

The CommandHandler implements the concrete business logic for each Pet command. It orchestrates domain operations and coordinates with the EventSourcingHandler for persistence.

**Responsibilities**:
- Execute business logic for commands (CreatePetCommand, UpdatePetCommand, DeletePetCommand, RepublishPetCommand)
- Load existing aggregates when needed (for updates/deletes)
- Apply changes to the aggregate
- Persist changes via EventSourcingHandler

**Dependencies**:
- `IEventSourcingHandler<PetAggregate>` - For loading and saving aggregate state

**Supported Commands**:
- **CreatePetCommand** - Creates a new Pet aggregate
- **UpdatePetCommand** - Updates existing Pet information 
- **DeletePetCommand** - Marks a Pet as deleted
- **RepublishPetCommand** - Republishes all Pet events (for data recovery scenarios)

### EventSourcingHandler
**Location**: `Command/Pet/Command.Pet.Infrastructure/Handler/EventSourcingHandler.cs`

The EventSourcingHandler serves as the interface between the command handlers and the event store. It implements the IEventSourcingHandler interface for interacting with event storage and event publishing.

**Key Responsibilities**:
- **Save Operations**: Persist aggregate uncommitted events to the event store
- **Load Operations**: Reconstruct aggregates by replaying their event history
- **Event Publishing**: Publish events to Kafka for cross-service communication
- **Event Republishing**: Support for replaying all events (disaster recovery)

**Dependencies**:
- `IEventStore` - For event persistence and retrieval
- `IEventProducer` - For publishing events to Kafka

**Key Methods**:
- `SaveAsync(AggregateRoot aggregate)` - Saves uncommitted events and marks them as committed
- `GetByIdAsync(Guid id)` - Reconstructs aggregate by replaying events
- `RepublishEventsAsync()` - Republishes all events for the aggregate type

### EventStore
**Location**: `Command/Pet/Command.Pet.Infrastructure/Store/EventStore.cs`

The EventStore provides concrete implementation for event persistence and retrieval operations.

**Dependencies**:
- `IEventStoreRepository` - MongoDB repository for physical event storage
- `IEventProducer` - Kafka producer for event publishing

## Event Flow

1. **Command Reception**: API receives HTTP request and creates appropriate command
2. **Command Dispatch**: CommandDispatcher routes command to registered handler
3. **Business Logic**: CommandHandler executes business rules and modifies aggregate
4. **Event Generation**: Aggregate generates domain events for state changes
5. **Event Persistence**: EventSourcingHandler saves events to MongoDB via EventStore
6. **Event Publishing**: Events are published to Kafka for Query side consumption
7. **State Commitment**: Aggregate marks events as committed

## Configuration

The service requires configuration for:
- **MongoDB**: Event store persistence (`MongoDbConfig` section)
- **Kafka**: Event publishing (`ProducerConfig` section)
- **Environment Variables**: `KAFKA_TOPIC_PETSITTER_PET_EVENTS` for topic routing

## Dependencies

- **Shared CQRS Infrastructure**: Core interfaces and base classes
- **MongoDB**: Event storage and querying
- **Kafka**: Event publishing for eventual consistency
- **ASP.NET Core**: Web API hosting

## Testing

- **Unit Tests**: `Command.Pet.Api.Tests` - Business logic and handler testing
- **Integration Tests**: `Command.Pet.Api.Tests.Integration` - End-to-end API testing

## Possible Improvements

As noted in the original design notes:
- **State Projection Publishing**: Consider publishing the latest aggregate state alongside events to reduce Query side complexity and remove the responsibility of modeling entities from Query handlers by explicitly providing computed state.