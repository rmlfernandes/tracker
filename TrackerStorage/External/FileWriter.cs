namespace External;

public class FileWriter : IFileWriter
{
    private readonly ILogger<FileWriter> logger;
    private readonly string filePath;

    public FileWriter(ILogger<FileWriter> logger, string filePath)
    {
        this.logger = logger;
        this.filePath = filePath;
    }

    public async Task AppendAsync(string line)
    {
        try
        {
            using var fileStream = new FileStream(this.filePath, FileMode.Append);
            using var streamWriter = new StreamWriter(fileStream);
            await streamWriter.WriteLineAsync(line);
        }
        catch (Exception e)
        {
            this.logger.LogError("An error occurred while appending data to file. [{0}]", e.Message);
        }
    }
}
