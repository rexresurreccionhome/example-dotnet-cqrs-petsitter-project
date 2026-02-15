# Cqrs Project

The **Cqrs** project is a reusable library that provides the foundational components for implementing the **Command Query Responsibility Segregation (CQRS)** pattern with **Event Sourcing** in your applications. This library serves as the core infrastructure for building scalable, maintainable, and event-driven applications.

## Architecture Overview

This library implements the CQRS pattern, which separates read and write operations into distinct models:
- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Retrieve, Search)
- **Events**: Capture state changes for event sourcing
- **Infrastructure**: Provides the foundational components for message routing, event handling, and persistence

## Project Structure

```
Cqrs/
├── Command/           # Command-side components
├── Query/             # Query-side components  
├── Domain/            # Domain entities and aggregates
├── Event/             # Event sourcing components
└── Infrastructure/    # Core infrastructure components
    ├── Config/
    ├── Consumer/
    ├── Dispatcher/
    ├── Handler/
    ├── Producer/
    ├── Repository/
    └── Store/
```

## Core Components

### 1. Command

The Command namespace contains components for handling write operations in the CQRS pattern.

#### `BaseCommand`
An abstract base class that all commands must inherit from. Commands represent intentions to change the system state.

```csharp
public abstract class BaseCommand
{
    // Base implementation for all commands
}
```

**Purpose**: Provides a common contract for all command types, enabling polymorphic handling through the command dispatcher.

### 2. Query

The Query namespace contains components for handling read operations.

#### `BaseQuery`
An abstract base class for all query operations.

```csharp
public abstract class BaseQuery
{
    // Base implementation for all queries
}
```

#### `BaseResponse`
A base response class that provides a standard structure for query results.

```csharp
public class BaseResponse
{
    public required string Message { get; set; }
}
```

**Purpose**: Ensures consistent response formats across all query operations and provides a foundation for extending query responses.

### 3. Domain

The Domain namespace contains domain-driven design components.

#### `AggregateRoot`
The foundation class for all aggregate roots in the domain model. Implements event sourcing capabilities.

```csharp
public abstract class AggregateRoot
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    
    // Event sourcing methods
    public IEnumerable<BaseEvent> GetUncommittedChanges()
    public void MarkAsCommitted()
}
```

**Key Features**:
- **Event Management**: Tracks uncommitted domain events
- **Version Control**: Maintains aggregate version for optimistic concurrency
- **Dynamic Event Application**: Uses reflection to apply events to aggregates
- **Event History Replay**: Supports rebuilding aggregate state from events

**Purpose**: Serves as the base class for all business entities that need to maintain their state through events, providing the foundation for event sourcing.

### 4. Event

The Event namespace contains event sourcing infrastructure.

#### `BaseEvent`
The base class for all domain events in the system.

```csharp
public class BaseEvent
{
    public string EventName { get; set; }
    public int Version { get; set; }
    public Guid Id { get; set; }
}
```

#### `EventModel`
A data model for persisting events in the event store (referenced in repository interfaces).

**Purpose**: Events represent things that have happened in the domain and are used to rebuild aggregate state and communicate changes across bounded contexts.

## Infrastructure Components

The Infrastructure namespace provides the core plumbing for CQRS and event sourcing implementation.

### Dispatcher

The Dispatcher components are responsible for **routing and coordinating** messages within the CQRS system.

#### `ICommandDispatcher` / `CommandDispatcher`
Routes commands to their appropriate handlers.

```csharp
public interface ICommandDispatcher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}
```

**Key Features**:
- **Handler Registration**: Maps command types to their handlers
- **Type-Safe Routing**: Ensures commands reach the correct handler
- **Error Handling**: Throws exceptions for unregistered command types

#### `IQueryDispatcher`
Routes queries to their appropriate handlers (interface only, implementation would be similar to CommandDispatcher).

**Purpose**: Dispatchers act as the **central routing mechanism** that decouples command/query senders from their handlers, enabling loose coupling and testability.

### Handler

The Handler components define contracts for **processing and persisting** aggregates with event sourcing.

#### `IEventSourcingHandler<T>`
Defines the contract for handling aggregate persistence and retrieval using event sourcing.

```csharp
public interface IEventSourcingHandler<T>
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<T> GetByIdAsync(Guid id);
    Task RepublishEventsAsync();
}
```

**Key Responsibilities**:
- **Aggregate Persistence**: Saves aggregate changes as events
- **Aggregate Reconstruction**: Rebuilds aggregates from their event history
- **Event Republishing**: Supports event replay scenarios

**Purpose**: Handlers provide the **business logic layer** that coordinates between the domain model and infrastructure, managing the lifecycle of aggregates with event sourcing.

### Producer

The Producer components are responsible for **publishing events** to external systems or message queues.

#### `IEventProducer` / `EventProducer`
Publishes domain events to Apache Kafka topics for cross-service communication.

```csharp
public interface IEventProducer
{
    Task ProduceAsync<T>(string topicName, T @event) where T : BaseEvent;
}
```

**Key Features**:
- **Kafka Integration**: Uses Confluent.Kafka for reliable message delivery
- **Serialization**: JSON serializes events for transport
- **Error Handling**: Validates message delivery success
- **Asynchronous Processing**: Non-blocking event publishing

**Purpose**: Producers enable **inter-service communication** and event-driven architecture by publishing domain events to external consumers.

### Key Differences: Dispatcher vs Handler vs Producer

| Component | **Primary Purpose** | **Scope** | **Responsibility** |
|-----------|-------------------|-----------|-------------------|
| **Dispatcher** | **Message Routing** | Internal (within service) | Routes commands/queries to appropriate handlers within the same process |
| **Handler** | **Business Logic** | Internal (aggregate lifecycle) | Executes business logic, manages aggregate state, and coordinates with infrastructure |
| **Producer** | **Event Publishing** | External (cross-service) | Publishes events to external systems for inter-service communication |

**Flow Example**:
1. **Dispatcher** receives a command and routes it to the appropriate handler
2. **Handler** executes business logic, modifying the aggregate and generating events
3. **Producer** publishes the generated events to external systems/services

### Repository

#### `IEventStoreRepository`
Provides data access methods for event storage and retrieval.

```csharp
public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
    Task<List<EventModel>> FindAllAsync();
}
```

**Purpose**: Abstracts the data access layer for event persistence, enabling different storage implementations.

### Store

#### `IEventStore`
Defines the contract for event store operations at a higher abstraction level.

```csharp
public interface IEventStore
{
    Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int version);
    Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
    Task<List<Guid>> GetAggregateIdsAsync();
}
```

**Purpose**: Provides aggregate-focused event storage operations, working with domain events rather than storage models.

### Consumer

#### `IEventConsumer`
Defines the contract for consuming events from external sources.

```csharp
public interface IEventConsumer
{
    void Consume(string topic);
}
```

**Purpose**: Enables the service to react to events published by other services, supporting event-driven communication.

## Exception Handling

The library includes custom exceptions for common CQRS scenarios:

- **`AggregateNotFoundException`**: Thrown when an aggregate cannot be found
- **`ConcurrencyException`**: Thrown when optimistic concurrency conflicts occur

## Usage Patterns

### Command Flow
1. Client sends command → **Dispatcher** → **Handler** → **Aggregate** → **Event Store** → **Producer** → External Systems

### Query Flow  
1. Client sends query → **Query Dispatcher** → **Query Handler** → **Read Model** → Response

### Event Flow
1. Domain Event → **Producer** → Message Queue → **Consumer** → **Event Handler** → **Read Model Update**

## Benefits

- **Separation of Concerns**: Clear separation between commands, queries, and events
- **Scalability**: Independent scaling of read and write operations
- **Auditability**: Complete event history for compliance and debugging
- **Flexibility**: Support for multiple read models and projections
- **Testability**: Mockable interfaces enable comprehensive unit testing
- **Loose Coupling**: Components are decoupled through interfaces and message passing

This library provides the foundation for building robust, scalable applications using CQRS and Event Sourcing patterns while maintaining clean architecture principles.