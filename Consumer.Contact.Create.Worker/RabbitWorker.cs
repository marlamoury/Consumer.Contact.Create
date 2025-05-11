using Consumer.Create.Contact.Infrastructure.Messaging;

namespace Consumer.Contact.Create.Worker;

public class RabbitWorker : BackgroundService
{
    private readonly ILogger<RabbitWorker> _logger;
    private readonly RabbitMQConsumer _consumer;

    public RabbitWorker(ILogger<RabbitWorker> logger, RabbitMQConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await _consumer.StartAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Worker finalizado.");
    }

}
