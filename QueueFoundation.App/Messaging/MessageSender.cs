public class MessageSender
{
    private readonly object _lock = new();
    private readonly Queue<byte[]> _pending = new();
    private readonly Session _session;
    private readonly IMessageQueue _queue;
    private readonly IAudit _audit;

    public MessageSender(Session session, IMessageQueue queue, IAudit audit)
    {
        _session = session;
        _queue = queue;
        _audit = audit;
        _session.StateChanged += OnStateChanged;
    }

    public int PendingCount
    {
        get
        {
            lock (_lock)
            {
                return _pending.Count;
            }
        }
    }

    public bool Send(byte[] msg)
    {
        lock (_lock)
        {
            if (_session.State is SuspendedState)
            {
                _pending.Enqueue(msg);
                return false;
            }

            if (!_session.CanSend)
            {
                return false;
            }

            FlushPending();
            return SendNow(msg);
        }
    }

    private void OnStateChanged(SessionState newState)
    {
        if (!newState.CanSend)
        {
            return;
        }

        lock (_lock)
        {
            FlushPending();
        }
    }

    private void FlushPending()
    {
        while (_pending.Count > 0)
        {
            if (!SendNow(_pending.Peek()))
            {
                return;
            }

            _pending.Dequeue();
        }
    }

    private bool SendNow(byte[] msg)
    {
        bool sent = _queue.Send(msg);

        if (sent)
        {
            _audit.Record(msg.Length);
        }

        return sent;
    }
}