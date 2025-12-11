using Confluent.Kafka;
using Cqrs.Infrastructure;
using Cqrs.Infrastructure.Config;
using Cqrs.Infrastructure.Producer;
using Cqrs.Infrastructure.Repository;
using Cqrs.Infrastructure.Store;
using Cqrs.Infrastructure.Handler;
using Cqrs.Infrastructure.Dispatcher;
using Cqrs.Command.Sitter;
using Command.Sitter.Domain;
using Command.Sitter.Infrastructure.Handler;
using Command.Sitter.Infrastructure.Store;
using Command.Sitter.Api.Command;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register MongoDB config
builder.Services.Configure<MongoDbConfig>(
    builder.Configuration.GetRequiredSection("MongoDbConfig")
);

// Register Kafka ProducerConfig
builder.Services.Configure<ProducerConfig>(
    builder.Configuration.GetRequiredSection("ProducerConfig")
);

// Register infrastructure services
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<SitterAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();

// Register command dispatcher and handlers
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>(options =>
{
    ICommandHandler commandHandler = options.GetRequiredService<ICommandHandler>();
    CommandDispatcher dispatcher = new();
    dispatcher.RegisterHandler<CreateSitterCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<UpdateSitterCommand>(commandHandler.HandleAsync);
    return dispatcher;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
