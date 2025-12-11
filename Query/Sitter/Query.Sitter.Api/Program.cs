using Microsoft.EntityFrameworkCore;
using Cqrs.Infrastructure;
using Cqrs.Query.Sitter;
using Query.Sitter.Domain.Entities;
using Query.Sitter.Infrastructure.DataAccess;
using Query.Sitter.Infrastructure.Repository;
using Query.Sitter.Api.Query;
using Cqrs.Infrastructure.Consumer;
using Query.Sitter.Infrastructure.Consumer;
using Query.Sitter.Infrastructure.Handler;
using Query.Sitter.Infrastructure.Dispatcher;
using Query.Sitter.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<ApplicationDbContext>((options) =>
{
    ConnectionStrings connectionStrings = builder.Configuration.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>()!;
    options.UseSqlServer(connectionStrings.PetSitter, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
    options.EnableSensitiveDataLogging();
});
builder.Services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
builder.Services.AddScoped<ISitterRepository, SitterRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Query.Sitter.Infrastructure.Handler.EventHandler>();
builder.Services.Configure<Confluent.Kafka.ConsumerConfig>(
    builder.Configuration.GetRequiredSection("ConsumerConfig")
);
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

// Register query handler methods
builder.Services.AddScoped<IQueryDispatcher<SitterEntity>, QueryDispatcher>(options =>
{
    IQueryHandler queryHandler = options.GetRequiredService<IQueryHandler>();
    var dispatcher = new QueryDispatcher();
    dispatcher.RegisterHandler<FindAllSittersQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindSitterByIdQuery>(queryHandler.HandleAsync);
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
    // Ensure database is created in development
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
