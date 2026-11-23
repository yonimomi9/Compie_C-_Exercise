public class MessageSender
{
    private readonly Session _session;
    private readonly IMessageQueue _queue;
    private readonly IAudit _audit;

    public MessageSender(Session session, IMessageQueue queue, IAudit audit)
    {
        _session = session;
        _queue = queue;
        _audit = audit;
    }

    public bool Send(byte[] msg)
    {
        if (!_session.CanSend)
        {
            return false;
        }

        bool sent = _queue.Send(msg);

        if (sent)
        {
            _audit.Record(msg.Length);
        }

        return sent;
    }
}