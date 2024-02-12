using External;
using Microsoft.Extensions.Logging;
using Moq;

namespace TrackerStorage.Tests;

public class FileWriterTests
{
    private const string PATH = "tmp.txt";
    private readonly Mock<ILogger<FileWriter>> loggerMock;
    private readonly FileWriter fileWriter;

    public FileWriterTests()
    {
        this.loggerMock = new Mock<ILogger<FileWriter>>();
        this.fileWriter = new FileWriter(this.loggerMock.Object, PATH);
    }

    [Fact]
    public async Task AppendAsync_Line_LineWritten()
    {
        // Arrange
        File.Delete(PATH);

        // Act
        this.fileWriter.AppendAsync("test");

        // Assert
        var text = await File.ReadAllTextAsync(PATH, default(CancellationToken));

        Assert.True(File.Exists(PATH));
        Assert.True(text.Length > 0);
    }
}