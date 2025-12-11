using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DnsClient.Internal;
using Cqrs.Infrastructure.Consumer;

namespace Query.Sitter.Infrastructure.Consumer;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service running");
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_SITTER_EVENTS");
            if (String.IsNullOrEmpty(topic))
            {
                throw new NoNullAllowedException("Environment variable KAFKA_TOPIC_PETSITTER_SITTER_EVENTS is empty.");
            }
            
            Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("Event consumer service stopped");

        return Task.CompletedTask;
    }
}
