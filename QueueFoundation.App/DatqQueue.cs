public class DatqQueue : IMessageQueue
{
    private readonly DatqOptions _options;

    public DatqQueue(DatqOptions options)
    {
        _options = options;
    }

    public bool Send(byte[] msg)
    {
        Console.WriteLine($"DATQ [{_options.Server}]: sent {msg.Length} bytes");
        return true;
    }
}
