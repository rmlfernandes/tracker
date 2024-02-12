namespace External;

public interface IFileWriter
{
    Task AppendAsync(string line);
}
