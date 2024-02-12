using Confluent.Kafka;
using External;
using TrackerApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var producerConfig = new ProducerConfig
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"]
    };

    var logger = serviceProvider.GetRequiredService<ILogger<MessagePublisher>>();
    var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    var topicName = configuration["Kafka:Topic"];    

    return new MessagePublisher(logger, producer, topicName);
});

var app = builder.Build();

app.MapGroup("/track")
    .MapTrackerApi()
    .WithTags("Tracker Endpoints");

app.Run();

public partial class Program { }
