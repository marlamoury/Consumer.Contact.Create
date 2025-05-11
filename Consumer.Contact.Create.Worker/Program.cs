using Consumer.Contact.Create.Worker;
using Consumer.Create.Contact.Application.Services;
using Consumer.Create.Contact.Infrastructure.Messaging;
using Consumer.Create.Contact.Infrastructure.Persistence;
using Serilog;

// grava logs em um arquivo no kubernete k8s azure
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/criar-contatos/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Carrega o arquivo appsettings.json
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Carrega as configurações do RabbitMQ do appsettings.json
        var rabbitMqSettings = context.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

        if (rabbitMqSettings == null)        
            throw new InvalidOperationException("RabbitMQSettings não pode ser nulo. Verifique o arquivo appsettings.json.");
        
        services.AddSingleton(rabbitMqSettings);

        // Registrando serviços e repositórios
        services.AddScoped<IContatoRepository, ContatoRepository>();
        services.AddScoped<IContatoService, ContatoService>();

        // Registrando o RabbitMQConsumer como Singleton e o Worker como HostedService
        services.AddSingleton<RabbitMQConsumer>();
        services.AddHostedService<RabbitWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
