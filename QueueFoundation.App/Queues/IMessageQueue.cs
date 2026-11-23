public interface IMessageQueue
{
    public bool Send(byte[] msg);
}