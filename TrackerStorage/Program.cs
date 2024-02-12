using TrackerStorage;
using External;
using Confluent.Kafka;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IFileWriter, FileWriter>(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<FileWriter>>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var path = configuration["File:Path"];

    return new FileWriter(logger, path);
});

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var consumerConfig = new ConsumerConfig
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"],
        GroupId = configuration["Kafka:GroupId"],
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
});

builder.Services.AddHostedService(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<TrackerWorker>>();
    var consumer = serviceProvider.GetRequiredService<IConsumer<Ignore, string>>();
    var fileWriter = serviceProvider.GetRequiredService<IFileWriter>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var topicName = configuration["Kafka:Topic"];

    return new TrackerWorker(logger, consumer, fileWriter, topicName);
});

var host = builder.Build();
host.Run();
