using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Cqrs.Infrastructure;
using Cqrs.Infrastructure.Consumer;
using Cqrs.Query.Pet;
using Query.Pet.Api.Query;
using Query.Pet.Domain.Entities;
using Query.Pet.Infrastructure.Repository;
using Query.Pet.Infrastructure.Config;
using Query.Pet.Infrastructure.Consumer;
using Query.Pet.Infrastructure.DataAccess;
using Query.Pet.Infrastructure.Dispatcher;
using Query.Pet.Infrastructure.Handler;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<ApplicationDbContext>((options) =>
{
    ConnectionStrings connectionStrings = builder.Configuration.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>()!;
    options.UseSqlServer(connectionStrings.PetSitter, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
    options.EnableSensitiveDataLogging();
});
builder.Services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Query.Pet.Infrastructure.Handler.EventHandler>();
builder.Services.Configure<ConsumerConfig>(
    builder.Configuration.GetRequiredSection("ConsumerConfig")
);
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

// register query handler methods
builder.Services.AddScoped<IQueryDispatcher<PetEntity>, QueryDispatcher>(options =>
{
    IQueryHandler queryHandler = options.GetRequiredService<IQueryHandler>();
    QueryDispatcher dispatcher = new();
    dispatcher.RegisterHandler<FindAllPetsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPetsByMemberIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPetByIdQuery>(queryHandler.HandleAsync);
    return dispatcher;
});

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Create database and tables from code in local development environment
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
