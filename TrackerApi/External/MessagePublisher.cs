namespace External;

using Confluent.Kafka;
using Models;
using Newtonsoft.Json;

public class MessagePublisher : IMessagePublisher
{
    private readonly ILogger<MessagePublisher> logger;
    private readonly IProducer<Null, string> producer;
    private readonly string topicName;

    public MessagePublisher(
        ILogger<MessagePublisher> logger,
        IProducer<Null, string> producer,
        string topicName)
    {
        this.logger = logger;
        this.producer = producer;
        this.topicName = topicName;
    }

    public async Task Publish(TrackData trackData)
    {
        try
        {
            var message = new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(trackData)
            };

            await this.producer.ProduceAsync(this.topicName, message);
        }
        catch (Exception e)
        {
            this.logger.LogError("Failed to deliver message: {0}", e.Message);
        }
    }
}