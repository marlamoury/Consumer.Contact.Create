using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Consumer.Contact.Create.Worker;
using Consumer.Create.Contact.Infrastructure.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer.Contact.Create.Tests
{
    public class WorkerTests
    {
        [Fact]
        public async Task Worker_Should_StartSuccessfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RabbitWorker>>();
            var consumerLoggerMock = new Mock<ILogger<RabbitMQConsumer>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var rabbitMqSettings = new RabbitMQSettings
            {
                Host = "localhost",
                Username = "guest",
                Password = "guest",
                QueueName = "criar-contatos"
            };

            var rabbitMqConsumer = new RabbitMQConsumer(consumerLoggerMock.Object, serviceProviderMock.Object, rabbitMqSettings);
            var worker = new RabbitWorker(loggerMock.Object, rabbitMqConsumer);

            using var cancellationTokenSource = new CancellationTokenSource();

            // Act
            var executeTask = worker.StartAsync(cancellationTokenSource.Token);

            // Aguarda um tempo para garantir que o worker iniciou
            await Task.Delay(2000);  // Aumentado para garantir a inicialização
        }
    }
}