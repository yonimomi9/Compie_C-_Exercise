using Microsoft.Extensions.Options;

public class DatqQueue : IMessageQueue
{
    private readonly DatqOptions _options;

    public DatqQueue(IOptions<DatqOptions> options)
    {
        _options = options.Value;
    }

    public bool Send(byte[] msg)
    {
        Console.WriteLine($"DATQ [{_options.Server}]: sent {msg.Length} bytes");
        return true;
    }
}
