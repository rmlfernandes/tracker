namespace TrackerStorage;

using Confluent.Kafka;
using External;
using Models;
using Newtonsoft.Json;
using System.Globalization;

public class TrackerWorker : BackgroundService
{
    private readonly ILogger<TrackerWorker> logger;
    private readonly IConsumer<Ignore, string> consumer;
    private readonly IFileWriter fileWriter;
    private readonly string topicName;

    public TrackerWorker(
        ILogger<TrackerWorker> logger,
        IConsumer<Ignore, string> consumer,
        IFileWriter fileWriter,
        string topicName)
    {
        this.logger = logger;
        this.consumer = consumer;
        this.fileWriter = fileWriter;
        this.topicName = topicName;
    }

    private static string BuildLine(TrackData trackData)
    {
        var time = trackData.VisitTime.ToString("o", CultureInfo.InvariantCulture) ?? "null";
        var referrer = trackData.Referrer ?? "null";
        var userAgent = trackData.UserAgent ?? "null";
        var visitorIp = trackData.VisitorIp ?? "null";

        return String.Format("{0}|{1}|{2}|{3}", time, referrer, userAgent, visitorIp);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            this.logger.LogInformation("TrackerWorker starting at: {time}", DateTimeOffset.Now);

            this.consumer.Subscribe(this.topicName);
            try
            {
                while (true)
                {
                    var message = this.consumer.Consume();

                    var trackData = JsonConvert.DeserializeObject<TrackData>(message.Value);

                    await this.fileWriter.AppendAsync(BuildLine(trackData));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error occurred. [{0}]", ex.Message);
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("TrackerWorker finishing at: {time}", DateTimeOffset.Now);

        await base.StopAsync(stoppingToken);
    }
}
