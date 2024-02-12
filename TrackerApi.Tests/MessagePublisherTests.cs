using Confluent.Kafka;
using External;
using Microsoft.Extensions.Logging;
using Models;
using Moq;

namespace TrackerApi.Tests;

public class MessagePublisherTests
{
    private readonly Mock<ILogger<MessagePublisher>> loggerMock;

    public MessagePublisherTests()
    {
        this.loggerMock = new Mock<ILogger<MessagePublisher>>();
    }

    [Fact]
    public async Task Publish_TrackData_MessageProduced()
    {
        // Arrange
        var producerMock = new Mock<IProducer<Null, string>>();
        var topicName = "topic-name";
        var deliveryReport = new DeliveryReport<Null, string>();

        producerMock.Setup(p => p.ProduceAsync(topicName, It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(deliveryReport);

        var messagePublisher = new  MessagePublisher(this.loggerMock.Object, producerMock.Object, topicName);

        var trackData = new TrackData
        {
            Referrer = "tests",
            UserAgent = "tests-agent",
            VisitorIp = "127.0.0.1",
            VisitTime = DateTime.UtcNow
        };

        // Act
        await messagePublisher.Publish(trackData);

        // Assert
        producerMock.Verify(p => p.ProduceAsync(topicName, It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}