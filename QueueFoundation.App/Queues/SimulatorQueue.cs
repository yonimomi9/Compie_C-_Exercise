public class SimulatorQueue : IMessageQueue
{
    private readonly List<byte[]> _messages = new List<byte[]>();
    public IReadOnlyList<byte[]> Messages => _messages;
    public bool Send(byte[] msg)
    {
        _messages.Add(msg);
        return true;
    }


}