
using Confluent.Kafka;
using Cqrs.Infrastructure;
using Cqrs.Infrastructure.Producer;
using Cqrs.Infrastructure.Repository;
using Cqrs.Infrastructure.Config;
using Cqrs.Infrastructure.Store;
using Cqrs.Infrastructure.Handler;
using Cqrs.Infrastructure.Dispatcher;
using Cqrs.Command.Pet;
using Command.Pet.Api.Command;
using Command.Pet.Domain;
using Command.Pet.Infrastructure.Handler;
using Command.Pet.Infrastructure.Store;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register MongoDB class maps
Command.Pet.Api.MongoClassMapRegistrar.Register();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<MongoDbConfig>(
    builder.Configuration.GetRequiredSection("MongoDbConfig")
);
builder.Services.Configure<ProducerConfig>(
    builder.Configuration.GetRequiredSection("ProducerConfig")
);

builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PetAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();

// register command handler methods
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
