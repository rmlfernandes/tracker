using System.Net;
using External;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Moq;
using TrackerApi.Tests.Helpers;

namespace TrackerApi.Tests;

public class TrackerEndpointsTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> factory;

    public TrackerEndpointsTests(TestWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Get_Track()
    {
        // Arrange
        var messagePublisherMock = new Mock<IMessagePublisher>();

        messagePublisherMock.Setup(mp => mp.Publish(It.IsAny<TrackData>())).Returns(Task.CompletedTask);

        var client = this.factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(ServiceProvider => { return messagePublisherMock.Object; });
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/track");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
        messagePublisherMock.Verify(mp => mp.Publish(It.IsAny<TrackData>()), Times.Once);
    }
}